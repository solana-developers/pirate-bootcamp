import {
    getAccount as getTokenAccount,
    getAssociatedTokenAddressSync,
    TokenAccountNotFoundError,
    createAccount as createTokenAccount,
} from '@solana/spl-token'
import { PublicKey, SendTransactionError } from '@solana/web3.js'
import { sleepSeconds } from './util'
import assetsConfig from './util/assets.json'
import { ARBITRAGE_PROGRAM, CONNECTION, PAYER } from './util/const'
import { createArbitrageInstruction, getPoolAddress } from './util/instruction'
import { mintExistingTokens } from './util/token'
import {
    buildTransactionV0,
    buildTransactionV0WithLookupTable,
    createAddressLookupTable,
    extendAddressLookupTable,
    printAddressLookupTable,
} from './util/transaction'
import { before, describe, it } from 'mocha'

// Swap programs to arbitrage trade
const SWAP_PROGRAM_1 = new PublicKey(
    '5koF84vG5xwah17PNRyge3HmqdJZ4rqdqPvZnMKqi8Bq'
)
const SWAP_PROGRAM_2 = new PublicKey(
    'DRP4K7yv8EBftb3roP81idoPtRDJwpak1Apw8d4Df14T'
)

// Temperature `t`: How aggressive should the model be? 0..99
const temperature = 60
// Concurrency `n`: Try `n` assets at a time
const concurrency = 8
// Iterations `i`: Check all asset pairings `i` times
const iterations = 2

/**
 * Test the Arbitrage Program
 */
describe('Arbitrage Bot', async () => {
    const connection = CONNECTION
    const payer = PAYER
    const arbProgram = ARBITRAGE_PROGRAM
    const assets = assetsConfig.assets.map((o) => {
        return {
            name: o.name,
            quantity: o.quantity,
            decimals: o.decimals,
            address: new PublicKey(o.address),
        }
    })

    // The address of our Address Lookup Table, which we'll set when we create the table
    let lookupTable: PublicKey

    // The lists of accounts required for the arbitrage program, which we'll build
    // as we iterate through the list of assets in `util/assets.json` and derive the
    // associated token accounts in the `before` step
    let tokenAccountsUser: PublicKey[] = []
    let tokenAccountsSwap1: PublicKey[] = []
    let tokenAccountsSwap2: PublicKey[] = []
    let mints: PublicKey[] = []

    /**
     * Collects all necessary accounts and mints tokens to the payer if necessary
     */
    before(
        'Collect all token accounts & mints and mint some assets to the payer if necessary',
        async () => {
            for (const a of assets) {
                const tokenAddressUser = getAssociatedTokenAddressSync(
                    a.address,
                    payer.publicKey
                )
                try {
                    // Check if the token account holds any tokens currently
                    const tokenAccount = await getTokenAccount(
                        connection,
                        tokenAddressUser
                    )
                    if (tokenAccount.amount === BigInt(0)) {
                        // If not, mint some tokens to it
                        await mintExistingTokens(
                            connection,
                            payer,
                            a.address,
                            10,
                            a.decimals
                        )
                    }
                } catch (e) {
                    // Catch the error if the token account doesn't exist
                    if (e === TokenAccountNotFoundError) {
                        // Create the token account
                        await createTokenAccount(
                            connection,
                            payer,
                            a.address,
                            payer.publicKey
                        )
                        // Mint some tokens to it
                        await mintExistingTokens(
                            connection,
                            payer,
                            a.address,
                            10,
                            a.decimals
                        )
                    }
                }
                // Add each account to its respective list
                tokenAccountsUser.push(tokenAddressUser)
                tokenAccountsSwap1.push(
                    getAssociatedTokenAddressSync(
                        a.address,
                        getPoolAddress(SWAP_PROGRAM_1),
                        true
                    )
                )
                tokenAccountsSwap2.push(
                    getAssociatedTokenAddressSync(
                        a.address,
                        getPoolAddress(SWAP_PROGRAM_2),
                        true
                    )
                )
                mints.push(a.address)
            }
        }
    )

    /**
     * Creates the Address Lookup Table for our arbitrage instruction
     */
    it('Create a Lookup Table', async () => {
        lookupTable = await createAddressLookupTable(connection, payer)
        await sleepSeconds(2)
        // Helper function to avoid code repetition
        //
        // We have to send each list one at a time, since sending all addresses
        // would max out the transaction size limit
        async function inlineExtend(addresses: PublicKey[]) {
            await extendAddressLookupTable(
                connection,
                payer,
                lookupTable,
                addresses
            )
            await sleepSeconds(2)
        }
        inlineExtend(tokenAccountsUser)
        inlineExtend(tokenAccountsSwap1)
        inlineExtend(tokenAccountsSwap2)
        inlineExtend(mints)
        printAddressLookupTable(connection, lookupTable)
    })

    /**
     * Function to send the arbitrage instruction to the program
     * To be used in the loop below
     */
    async function sendArbitrageInstruction(
        tokenAccountsUserSubList: PublicKey[],
        tokenAccountsSwap1SubList: PublicKey[],
        tokenAccountsSwap2SubList: PublicKey[],
        mintsSubList: PublicKey[],
        concurrencyVal: number
    ) {
        const ix = createArbitrageInstruction(
            arbProgram.publicKey,
            payer.publicKey,
            tokenAccountsUserSubList,
            tokenAccountsSwap1SubList,
            tokenAccountsSwap2SubList,
            mintsSubList,
            concurrencyVal,
            temperature,
            SWAP_PROGRAM_1,
            SWAP_PROGRAM_2
        )
        const tx = await buildTransactionV0WithLookupTable(
            connection,
            [ix],
            payer.publicKey,
            [payer],
            lookupTable
        )
        // const txNoLT = await buildTransactionV0(
        //     connection,
        //     [ix],
        //     payer.publicKey,
        //     [payer]
        // )
        console.log(`Sending transaction with ${concurrencyVal} accounts...`)
        console.log(`Tx size with Lookup Table      : ${tx.serialize().length}`)
        // console.log(
        //     `Tx size WITHOUT Lookup Table   : ${txNoLT.serialize().length}`
        // )
        try {
            await connection.sendTransaction(tx, { skipPreflight: true })
            console.log('====================================')
            console.log('   Arbitrage trade placed!')
            console.log('====================================')
        } catch (error) {
            if (error instanceof SendTransactionError) {
                if (error.message.includes('custom program error: 0x3')) {
                    console.log('====================================')
                    console.log('   No arbitrage opportunity found')
                    console.log('====================================')
                } else {
                    throw error
                }
            } else {
                throw error
            }
        }
        await sleepSeconds(2)
    }

    /**
     * Hit our arbitrage program with some of the total list of accounts,
     * based on the `concurrency` config, and see if we can obtain arbitrage
     * opportunities
     */
    it('Try Arbitrage', async () => {
        await sleepSeconds(4)
        // Loop through number of `iterations`
        for (let x = 0; x < iterations; x++) {
            console.log(`Iteration: ${x + 1}`)
            let len = mints.length
            // Loop through all combinations of assets based on `concurrency`
            let step = 0
            let brake = concurrency
            let tokenAccountsUserSubList = []
            let tokenAccountsSwap1SubList = []
            let tokenAccountsSwap2SubList = []
            let mintsSubList = []
            for (let i = 0; i < len; i++) {
                for (let j = i; j < len; j++) {
                    if (step == brake) {
                        // Send an arbitrage instruction to the program
                        const end = brake + concurrency
                        await sendArbitrageInstruction(
                            tokenAccountsUserSubList,
                            tokenAccountsSwap1SubList,
                            tokenAccountsSwap2SubList,
                            mintsSubList,
                            end - brake
                        )
                        await sleepSeconds(2)
                        brake = end
                        tokenAccountsUserSubList = []
                        tokenAccountsSwap1SubList = []
                        tokenAccountsSwap2SubList = []
                        mintsSubList = []
                    }
                    // Build sub-lists of accounts to send to the arb program
                    tokenAccountsUserSubList.push(tokenAccountsUser[j])
                    tokenAccountsSwap1SubList.push(tokenAccountsSwap1[j])
                    tokenAccountsSwap2SubList.push(tokenAccountsSwap2[j])
                    mintsSubList.push(mints[j])
                    step++
                }
            }
            // Send the instruction with any remaining accounts for this iteration
            if (mintsSubList.length! - 0) {
                await sendArbitrageInstruction(
                    tokenAccountsUserSubList,
                    tokenAccountsSwap1SubList,
                    tokenAccountsSwap2SubList,
                    mintsSubList,
                    mintsSubList.length
                )
            }
        }
    })
})
