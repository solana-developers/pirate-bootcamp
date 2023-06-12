import { Keypair } from '@solana/web3.js'
import fs from 'fs'

/**
 *
 * Reads a Keypair JSON file from the local machine
 *
 * @param path Path to keypair JSON file
 * @returns The Solana keypair as a `Keypair` object
 */
export function loadKeypairFromFile(path: string): Keypair {
    return Keypair.fromSecretKey(
        Buffer.from(JSON.parse(fs.readFileSync(path, 'utf-8')))
    )
}

/**
 *
 * Sleeps the process `s` seconds
 *
 * @param s Seconds to sleep
 * @returns Promise - causes the script to sleep
 */
export function sleepSeconds(s: number) {
    return new Promise((resolve) => setTimeout(resolve, s * 1000))
}
