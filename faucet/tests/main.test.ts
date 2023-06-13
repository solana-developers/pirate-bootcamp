import * as anchor from '@coral-xyz/anchor'
import { PublicKey } from '@solana/web3.js'
import { PirateFaucet } from '../target/types/pirate_faucet'
import assetsConfig from './util/assets.json'
import { createFaucet, fundFaucet, requestAirdrop } from './instructions'
import {
    calculateK,
    fetchFaucet,
    fetchFaucetTokenAccounts,
} from './util/faucet'
import { logFaucet } from './util/log'
import { mintExistingTokens } from './util/token'

// Seed prefix for the Faucet from our program
const FAUCET_SEED_PREFIX = 'faucet'

/**
 * Our main unit tests module
 */
describe('[Running Unit Tests]: Pirate Faucet', async () => {
    // Configurations
    const provider = anchor.AnchorProvider.env()
    anchor.setProvider(provider)
    const program = anchor.workspace
        .PirateFaucet as anchor.Program<PirateFaucet>
    const payer = (provider.wallet as anchor.Wallet).payer
    const faucetAddress = PublicKey.findProgramAddressSync(
        [Buffer.from(FAUCET_SEED_PREFIX)],
        program.programId
    )[0]
    // If you're reading this, you'll probably notice that we're manually
    // overriding Gold, Cannon, and Rum with the real mints from the bootcamp.
    //
    // That's correct, the rest of the assets won't serve to upgrade your ship
    // in the battle. RIP.
    enum AssetFilter {
        Gold,
        Cannon,
        Rum,
        OnlyBootcamp,
    }
    const activeFilter = [] // Adjust this filter to control how to fund the faucet
    const assets = assetsConfig.assets
        .filter((o) => {
            if (activeFilter.includes(AssetFilter.Gold) && o.name == 'Gold') {
                return true
            }
            if (
                activeFilter.includes(AssetFilter.Cannon) &&
                o.name == 'Cannon'
            ) {
                return true
            }
            if (activeFilter.includes(AssetFilter.Rum) && o.name == 'Rum') {
                return true
            }
            if (
                activeFilter.includes(AssetFilter.OnlyBootcamp) &&
                !(o.name != 'Gold' && o.name != 'Cannon' && o.name != 'Rum')
            ) {
                return false
            }
            if (
                !activeFilter.includes(AssetFilter.OnlyBootcamp) &&
                o.name != 'Gold' &&
                o.name != 'Cannon' &&
                o.name != 'Rum'
            ) {
                return true
            }
        })
        .map((o) => {
            if (o.name == 'Gold') {
                return {
                    name: o.name,
                    quantity: o.quantity,
                    decimals: 9,
                    address: new PublicKey(
                        'goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3' // Place your own Gold token address here
                    ),
                    mintNew: false,
                }
            } else if (o.name == 'Cannon') {
                return {
                    name: o.name,
                    quantity: o.quantity,
                    decimals: 9,
                    address: new PublicKey(
                        'boomkN8rQpbgGAKcWvR3yyVVkjucNYcq7gTav78NQAG' // Place your own Cannon token address here
                    ),
                    mintNew: false,
                }
            } else if (o.name == 'Rum') {
                return {
                    name: o.name,
                    quantity: o.quantity,
                    decimals: 9,
                    address: new PublicKey(
                        'rumwqxXmjKAmSdkfkc5qDpHTpETYJRyXY22DWYUmWDt' // Place your own Rum token address here
                    ),
                    mintNew: false,
                }
            } else {
                return {
                    name: o.name,
                    quantity: o.quantity,
                    decimals: o.decimals,
                    address: new PublicKey(o.address),
                    mintNew: true,
                }
            }
        })

    // Used as a flag to only initialize the Faucet once
    let programInitialized = false

    /**
     * Check if the Faucet exists and set the flag
     */
    before('          Check if Faucet exists', async () => {
        let faucetAccountInfo = await provider.connection.getAccountInfo(
            faucetAddress
        )
        if (faucetAccountInfo != undefined && faucetAccountInfo.lamports != 0) {
            console.log('   Faucet already initialized!')
            console.log(`     Address: ${faucetAddress.toBase58()}`)
            programInitialized = true
        }
    })

    /**
     * Initialize the Faucet if it doesn't exist already
     */
    it('          Create Faucet', async () => {
        if (!programInitialized) {
            await createFaucet(program, payer, faucetAddress)
        }
    })

    /**
     * Fund the Faucet
     */
    for (const asset of assets) {
        it(`          Fund Faucet with: ${asset.name}`, async () => {
            if (asset.mintNew) {
                await mintExistingTokens(
                    provider.connection,
                    payer,
                    asset.address,
                    asset.quantity,
                    asset.decimals
                )
            } else {
                console.log(`Minting overridden for ${asset.name}`)
            }
            await fundFaucet(
                program,
                payer,
                faucetAddress,
                asset.address,
                asset.quantity,
                asset.decimals
            )
        })
    }

    /**
     *
     * Calculates the Faucet's holdings (assets held in each token account)
     *
     * @param log A flag provided telling this function whether or not to print to logs
     * @returns The constant-product `K` (Constant-Product Algorithm)
     */
    async function getFaucetData(log: boolean): Promise<bigint> {
        const faucet = await fetchFaucet(program, faucetAddress)
        const faucetTokenAccounts = await fetchFaucetTokenAccounts(
            provider.connection,
            faucetAddress,
            faucet
        )
        const k = calculateK(faucetTokenAccounts)
        if (log) {
            await logFaucet(
                provider.connection,
                faucetAddress,
                faucetTokenAccounts,
                assets,
                k
            )
        }
        return k
    }

    /**
     * Prints the Faucet's holdings (assets held in each token account)
     */
    it('          Get Faucet Data', async () => await getFaucetData(true))

    it('          Request Airdrop', async () => {
        // const mintRequest = new PublicKey(
        //     'rumwqxXmjKAmSdkfkc5qDpHTpETYJRyXY22DWYUmWDt' // Place your own asset token address here
        // )
        const mintRequest = assets[0].address
        await requestAirdrop(program, payer, faucetAddress, mintRequest, 30, 9)
    })
})
