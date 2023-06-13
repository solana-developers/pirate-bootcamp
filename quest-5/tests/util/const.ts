import { Connection } from '@solana/web3.js'
import { loadKeypairFromFile } from '.'
import os from 'os'

// RPC connection
export const CONNECTION = new Connection('http://localhost:8899', 'confirmed')
// export const CONNECTION = new Connection(
//     'https://api.devnet.solana.com',
//     'confirmed'
// )

// Local keypair
export const PAYER = loadKeypairFromFile(os.homedir + '/.config/solana/id.json')

// Arbitrage program
export const ARBITRAGE_PROGRAM = loadKeypairFromFile(
    './target/deploy/arb_program-keypair.json'
)
