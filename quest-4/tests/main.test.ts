import * as anchor from '@coral-xyz/anchor'
import { PublicKey } from '@solana/web3.js'
import { SwapProgram } from '../target/types/swap_program'
import assetsConfig from './util/assets.json'
import { createPool, fundPool, swap } from './instructions'
import {
    calculateChangeInK,
    calculateK,
    fetchPool,
    fetchPoolTokenAccounts,
} from './util/swap'
import {
    logChangeInK,
    logK,
    logPool,
    logPostSwap,
    logPreSwap,
} from './util/log'
import { mintExistingTokens } from './util/token'
import { ASSETS } from './util/const'

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
    const assets = assetsConfig.assets.map((o) => {
        return {
            name: o.name,
            quantity: o.quantity,
            decimals: o.decimals,
            address: new PublicKey(o.address),
        }
    })
    const maxAssetIndex = assetsConfig.assets.length - 1

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
     * Fund the Liquidity Pool if we only just created it now
     */
    for (const asset of assets) {
        it(`          Fund Pool with: ${asset.name}`, async () => {
            await mintExistingTokens(
                provider.connection,
                payer,
                asset.address,
                asset.quantity,
                asset.decimals
            )
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

    /**
     *
     * Attempt a swap with our swap program
     *
     * @param receive
     * @param pay
     * @param payAmount
     */
    async function trySwap(
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
        payAmount: number
    ) {
        await mintExistingTokens(
            provider.connection,
            payer,
            pay.address,
            payAmount,
            pay.decimals
        )
        await sleepSeconds(2)
        const initialK = await getPoolData(false)
        await logPreSwap(
            provider.connection,
            payer.publicKey,
            poolAddress,
            receive,
            pay,
            payAmount
        )
        await swap(
            program,
            payer,
            poolAddress,
            receive.address,
            pay.address,
            payAmount,
            pay.decimals
        )
        await sleepSeconds(2)
        await logPostSwap(
            provider.connection,
            payer.publicKey,
            poolAddress,
            receive,
            pay
        )
        const resultingK = await getPoolData(false)
        logChangeInK(calculateChangeInK(initialK, resultingK))
    }

    /**
     * Runs 10 random swap tests
     */
    for (let x = 0; x < 10; x++) {
        it('          Try Swap', async () => {
            const receiveAssetIndex = getRandomInt(maxAssetIndex)
            // Pay asset can't be the same as receive asset
            let payAssetIndex = getRandomInt(maxAssetIndex)
            while (payAssetIndex === receiveAssetIndex) {
                payAssetIndex = getRandomInt(maxAssetIndex)
            }
            // Pay amount can't be zero
            let payAmount = getRandomInt(ASSETS[payAssetIndex][5])
            while (payAmount === 0) {
                payAmount = getRandomInt(ASSETS[payAssetIndex][5])
            }
            await trySwap(
                assets[receiveAssetIndex],
                assets[payAssetIndex],
                payAmount
            )
        })
    }
})
