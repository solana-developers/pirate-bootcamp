import {
    Account as TokenAccount,
    getAssociatedTokenAddressSync,
    getMint,
    getAccount as getTokenAccount,
} from '@solana/spl-token'
import { Connection, PublicKey } from '@solana/web3.js'
import { fromBigIntQuantity } from './token'
import { calculateBalances } from './faucet'

/**
 * Log a line break
 */
function lineBreak() {
    console.log('----------------------------------------------------')
}

/**
 *
 * Log information about a newly created asset mint
 *
 * @param name Asset name
 * @param decimals Asset mint decimals
 * @param quantity Quantity of asset minted
 * @param mint Asset mint address
 * @param signature Transaction signature of the minting
 */
export function logNewMint(
    name: string,
    decimals: number,
    quantity: number,
    mint: PublicKey,
    signature: string
) {
    lineBreak()
    console.log(`   Mint: ${name}`)
    console.log(`       Address:    ${mint.toBase58()}`)
    console.log(`       Decimals:   ${decimals}`)
    console.log(`       Quantity:   ${quantity}`)
    console.log(`       Transaction Signature: ${signature}`)
    lineBreak()
}

// Logs information about a swap - can be pre- or post-swap

export async function logPreSwap(
    connection: Connection,
    owner: PublicKey,
    faucet: PublicKey,
    receive: {
        name: string
        quantity: number
        decimals: number
        address: PublicKey
    },
    pay: {
        name: string
        quantity: number
        decimals: number
        address: PublicKey
    },
    amount: number
) {
    const [
        receiveUserBalance,
        receiveFaucetBalance,
        payUserBalance,
        payFaucetBalance,
    ] = await calculateBalances(
        connection,
        owner,
        faucet,
        receive.address,
        receive.decimals,
        pay.address,
        pay.decimals
    )
    lineBreak()
    console.log('   PRE-SWAP:')
    console.log()
    console.log(
        `       PAY: ${pay.name.padEnd(
            18,
            ' '
        )}  RECEIVE: ${receive.name.padEnd(18, ' ')}`
    )
    console.log(`       OFFERING TO PAY: ${amount}`)
    console.log()
    console.log('   |====================|==============|==============|')
    console.log('   | Asset:             | User:        | Faucet:        |')
    console.log('   |====================|==============|==============|')
    console.log(
        `   | ${pay.name.padEnd(18, ' ')} | ${payUserBalance.padStart(
            12,
            ' '
        )} | ${payFaucetBalance.padStart(12, ' ')} |`
    )
    console.log(
        `   | ${receive.name.padEnd(18, ' ')} | ${receiveUserBalance.padStart(
            12,
            ' '
        )} | ${receiveFaucetBalance.padStart(12, ' ')} |`
    )
    console.log('   |====================|==============|==============|')
    console.log()
}

export async function logPostSwap(
    connection: Connection,
    owner: PublicKey,
    faucet: PublicKey,
    receive: {
        name: string
        quantity: number
        decimals: number
        address: PublicKey
    },
    pay: {
        name: string
        quantity: number
        decimals: number
        address: PublicKey
    }
) {
    const [
        receiveUserBalance,
        receiveFaucetBalance,
        payUserBalance,
        payFaucetBalance,
    ] = await calculateBalances(
        connection,
        owner,
        faucet,
        receive.address,
        receive.decimals,
        pay.address,
        pay.decimals
    )
    console.log('   POST-SWAP:')
    console.log()
    console.log('   |====================|==============|==============|')
    console.log('   | Asset:             | User:        | Faucet:        |')
    console.log('   |====================|==============|==============|')
    console.log(
        `   | ${pay.name.padEnd(18, ' ')} | ${payUserBalance.padStart(
            12,
            ' '
        )} | ${payFaucetBalance.padStart(12, ' ')} |`
    )
    console.log(
        `   | ${receive.name.padEnd(18, ' ')} | ${receiveUserBalance.padStart(
            12,
            ' '
        )} | ${receiveFaucetBalance.padStart(12, ' ')} |`
    )
    console.log('   |====================|==============|==============|')
    console.log()
    lineBreak()
}

/**
 *
 * Logs the Liquidity Faucet's holdings (assets held in each token account)
 *
 * @param connection Connection to Solana RPC
 * @param faucetAddress Address of the Liquidity Faucet
 * @param tokenAccounts All token accounts owned by the Liquidity Faucet
 * @param assets The assets from the configuration file
 * @param k The constant-product `K` (Constant-Product Algorithm)
 */
export async function logFaucet(
    connection: Connection,
    faucetAddress: PublicKey,
    tokenAccounts: TokenAccount[],
    assets: {
        name: string
        quantity: number
        decimals: number
        address: PublicKey
    }[],
    k: bigint
) {
    function getHoldings(
        mint: PublicKey,
        tokenAccounts: TokenAccount[]
    ): bigint {
        const holding = tokenAccounts.find((account) =>
            account.mint.equals(mint)
        )
        return holding.amount
    }
    const padding = assets.reduce((max, a) => Math.max(max, a.name.length), 0)
    lineBreak()
    console.log('   Liquidity Faucet:')
    console.log(`       Address:    ${faucetAddress.toBase58()}`)
    console.log('       Holdings:')
    for (const a of assets) {
        const holding = getHoldings(a.address, tokenAccounts)
        const mint = await getMint(connection, a.address)
        const normalizedHolding = fromBigIntQuantity(holding, mint.decimals)
        console.log(
            `                   ${a.name.padEnd(
                padding,
                ' '
            )} : ${normalizedHolding.padStart(
                12,
                ' '
            )} : ${a.address.toBase58()}`
        )
    }
    logK(k)
    lineBreak()
}

/**
 *
 * Logs `K`
 *
 * @param k The constant-product `K` (Constant-Product Algorithm)
 */
export function logK(k: bigint) {
    console.log(`   ** Constant-Product (K): ${k.toString()}`)
}

/**
 *
 * Logs `ΔK` ("delta K", or "change in K")
 *
 * @param changeInK The change in the constant-product `K` (Constant-Product Algorithm)
 */
export function logChangeInK(changeInK: string) {
    console.log(`   ** Δ Change in Constant-Product (K): ${changeInK}`)
}
