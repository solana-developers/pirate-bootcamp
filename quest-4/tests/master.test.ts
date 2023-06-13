import * as anchor from '@coral-xyz/anchor'
import { PublicKey } from '@solana/web3.js'
import { SwapProgram } from '../target/types/swap_program'
import assetsConfig from './util/assets.json'
import { createPool, fundPool, swap } from './instructions'
import { calculateK, fetchPool, fetchPoolTokenAccounts } from './util/swap'
import { logPool } from './util/log'
import { mintExistingTokens } from './util/token'

// Seed prefix for the Liquidity Pool from our program
const LIQUIDITY_POOL_SEED_PREFIX = 'liquidity_pool'

// Util function to sleep
const sleepSeconds = async (s: number) =>
    await new Promise((f) => setTimeout(f, s * 1000))

// Util function for random number below max
function getRandomInt(max: number): number {
    return Math.floor(Math.random() * max)
}

/**
 * Our main unit tests module
 */
describe('[Running Unit Tests]: Swap Program', async () => {
    // Configurations
    const provider = anchor.AnchorProvider.env()
    anchor.setProvider(provider)
    const program = anchor.workspace.SwapProgram as anchor.Program<SwapProgram>
    const payer = (provider.wallet as anchor.Wallet).payer
    const poolAddress = PublicKey.findProgramAddressSync(
        [Buffer.from(LIQUIDITY_POOL_SEED_PREFIX)],
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
    const activeFilter = []
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
                        'goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3'
                    ),
                    mintNew: false,
                }
            } else if (o.name == 'Cannon') {
                return {
                    name: o.name,
                    quantity: o.quantity,
                    decimals: 9,
                    address: new PublicKey(
                        'boomkN8rQpbgGAKcWvR3yyVVkjucNYcq7gTav78NQAG'
                    ),
                    mintNew: false,
                }
            } else if (o.name == 'Rum') {
                return {
                    name: o.name,
                    quantity: o.quantity,
                    decimals: 9,
                    address: new PublicKey(
                        'rumwqxXmjKAmSdkfkc5qDpHTpETYJRyXY22DWYUmWDt'
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

    // Used as a flag to only initialize the Liquidity Pool once
    let programInitialized = false

    /**
     * Check if the Liquidity Pool exists and set the flag
     */
    before('          Check if Pool exists', async () => {
        let poolAccountInfo = await provider.connection.getAccountInfo(
            poolAddress
        )
        if (poolAccountInfo != undefined && poolAccountInfo.lamports != 0) {
            console.log('   Pool already initialized!')
            console.log(`     Address: ${poolAddress.toBase58()}`)
            programInitialized = true
        }
    })

    /**
     * Initialize the Liquidity Pool if it doesn't exist already
     */
    it('          Create Pool', async () => {
        if (!programInitialized) {
            await createPool(program, payer, poolAddress)
        }
    })

    /**
     * Fund the Liquidity Pool
     */
    for (const asset of assets) {
        it(`          Fund Pool with: ${asset.name}`, async () => {
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
            await fundPool(
                program,
                payer,
                poolAddress,
                asset.address,
                asset.quantity,
                asset.decimals
            )
        })
    }

    /**
     *
     * Calculates the Liquidity Pool's holdings (assets held in each token account)
     *
     * @param log A flag provided telling this function whether or not to print to logs
     * @returns The constant-product `K` (Constant-Product Algorithm)
     */
    async function getPoolData(log: boolean): Promise<bigint> {
        const pool = await fetchPool(program, poolAddress)
        const poolTokenAccounts = await fetchPoolTokenAccounts(
            provider.connection,
            poolAddress,
            pool
        )
        const k = calculateK(poolTokenAccounts)
        if (log) {
            await logPool(
                provider.connection,
                poolAddress,
                poolTokenAccounts,
                assets,
                k
            )
        }
        return k
    }

    /**
     * Prints the Liquidity Pool's holdings (assets held in each token account)
     */
    it('          Get Liquidity Pool Data', async () => await getPoolData(true))
})
