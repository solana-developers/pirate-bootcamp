import * as anchor from '@coral-xyz/anchor'
import { Keypair } from '@solana/web3.js'
import fs from 'fs'
import { mintNewTokens } from './util/token'
import { ASSETS } from './util/const'

// Constant to control whether or not metadata is added to the tokens
const METADATA = false

/**
 * Script to create new assets and mint them to the local keypair for testing
 */
describe('[Running Setup Script]: Create Assets', () => {
    const provider = anchor.AnchorProvider.env()
    const payer = (provider.wallet as anchor.Wallet).payer
    anchor.setProvider(provider)

    /**
     * Creates an SPL token for each asset in the list of assets, with the
     * provided configurations
     */
    it('          Creating Assets', async () => {
        let assets_conf = {
            assets: [],
        }

        for (const a of ASSETS) {
            const mintKeypair = Keypair.generate()
            await mintNewTokens(
                provider.connection,
                payer,
                mintKeypair,
                a,
                METADATA
            )
            assets_conf.assets.push({
                name: a[0],
                symbol: a[1],
                description: a[2],
                uri: a[3],
                decimals: a[4],
                quantity: a[5],
                address: mintKeypair.publicKey.toBase58(),
            })
        }

        fs.writeFileSync(
            './tests/util/assets.json',
            JSON.stringify(assets_conf)
        )
    })
})
