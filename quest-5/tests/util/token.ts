import {
    createAssociatedTokenAccountInstruction,
    createInitializeMintInstruction,
    createMintToInstruction,
    getAssociatedTokenAddressSync,
    MINT_SIZE,
    TOKEN_PROGRAM_ID,
} from '@solana/spl-token'
import { Connection, Keypair, PublicKey, SystemProgram } from '@solana/web3.js'
import { buildTransactionV0 } from './transaction'

/**
 *
 * Returns the real quantity of a `quantity` parameter by
 * increasing the number using the mint's decimal places
 *
 * @param quantity The provided quantity argument
 * @param decimals The decimals of the associated mint
 * @returns The real quantity of a `quantity` parameter
 */
export function toBigIntQuantity(quantity: number, decimals: number): bigint {
    return BigInt(quantity) * BigInt(10) ** BigInt(decimals)
}

/**
 *
 * Returns the nominal quantity of a `quantity` parameter by
 * decreasing the number using the mint's decimal places
 *
 * @param quantity The real quantity of a `quantity` parameter
 * @param decimals The decimals of the associated mint
 * @returns The nominal quantity of a `quantity` parameter
 */
export function fromBigIntQuantity(quantity: bigint, decimals: number): string {
    return (Number(quantity) / 10 ** decimals).toFixed(6)
}

/**
 *
 * Mints an existing SPL token to the local keypair
 *
 * @param connection
 * @param payer The Liquidity Provider (local wallet in `Anchor.toml`)
 * @param mint The asset's mint address
 * @param quantity The quantity to fund of the provided mint
 * @param decimals the decimals of this mint (used to calculate real quantity)
 */
export async function mintExistingTokens(
    connection: Connection,
    payer: Keypair,
    mint: PublicKey,
    quantity: number,
    decimals: number
) {
    const tokenAccount = getAssociatedTokenAddressSync(mint, payer.publicKey)
    const mintToWalletIx = createMintToInstruction(
        mint,
        tokenAccount,
        payer.publicKey,
        toBigIntQuantity(quantity, decimals)
    )
    const tx = await buildTransactionV0(
        connection,
        [mintToWalletIx],
        payer.publicKey,
        [payer]
    )
    await connection.sendTransaction(tx)
}
