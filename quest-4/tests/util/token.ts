import {
    createCreateMetadataAccountV3Instruction,
    PROGRAM_ID as METADATA_PROGRAM_ID,
} from '@metaplex-foundation/mpl-token-metadata'
import {
    createAssociatedTokenAccountInstruction,
    createInitializeMintInstruction,
    createMintToInstruction,
    getAssociatedTokenAddressSync,
    MINT_SIZE,
    TOKEN_PROGRAM_ID,
} from '@solana/spl-token'
import { Connection, Keypair, PublicKey, SystemProgram } from '@solana/web3.js'
import { buildTransaction } from './transaction'
import { logNewMint } from './log'

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
 * Creates and mints new SPL tokens to the local keypair
 *
 * @param connection Connection to Solana RPC
 * @param payer The Liquidity Provider (local wallet in `Anchor.toml`)
 * @param mintKeypair The generated keypair to be used for the new mint
 * @param asset The associated asset this new mint will represent
 */
export async function mintNewTokens(
    connection: Connection,
    payer: Keypair,
    mintKeypair: Keypair,
    asset: [string, string, string, string, number, number],
    metadata: boolean
) {
    const assetName = asset[0]
    const assetSymbol = asset[1]
    const assetUri = asset[3]
    const decimals = asset[4]
    const quantity = asset[5]

    const tokenAccount = getAssociatedTokenAddressSync(
        mintKeypair.publicKey,
        payer.publicKey
    )

    const createMintAccountIx = SystemProgram.createAccount({
        fromPubkey: payer.publicKey,
        newAccountPubkey: mintKeypair.publicKey,
        lamports: await connection.getMinimumBalanceForRentExemption(MINT_SIZE),
        space: MINT_SIZE,
        programId: TOKEN_PROGRAM_ID,
    })
    const initializeMintIx = createInitializeMintInstruction(
        mintKeypair.publicKey,
        decimals,
        payer.publicKey,
        payer.publicKey
    )
    const createMetadataIx = createCreateMetadataAccountV3Instruction(
        {
            metadata: PublicKey.findProgramAddressSync(
                [
                    Buffer.from('metadata'),
                    METADATA_PROGRAM_ID.toBuffer(),
                    mintKeypair.publicKey.toBuffer(),
                ],
                METADATA_PROGRAM_ID
            )[0],
            mint: mintKeypair.publicKey,
            mintAuthority: payer.publicKey,
            payer: payer.publicKey,
            updateAuthority: payer.publicKey,
        },
        {
            createMetadataAccountArgsV3: {
                data: {
                    name: assetName,
                    symbol: assetSymbol,
                    uri: assetUri,
                    creators: null,
                    sellerFeeBasisPoints: 0,
                    uses: null,
                    collection: null,
                },
                isMutable: false,
                collectionDetails: null,
            },
        }
    )
    const createAssociatedtokenAccountIx =
        createAssociatedTokenAccountInstruction(
            payer.publicKey,
            tokenAccount,
            payer.publicKey,
            mintKeypair.publicKey
        )
    const mintToWalletIx = createMintToInstruction(
        mintKeypair.publicKey,
        tokenAccount,
        payer.publicKey,
        toBigIntQuantity(quantity, decimals)
    )

    const tx = await buildTransaction(
        connection,
        payer.publicKey,
        [payer, mintKeypair],
        metadata
            ? [
                  createMintAccountIx,
                  initializeMintIx,
                  createMetadataIx,
                  createAssociatedtokenAccountIx,
                  mintToWalletIx,
              ]
            : [
                  createMintAccountIx,
                  initializeMintIx,
                  createAssociatedtokenAccountIx,
                  mintToWalletIx,
              ]
    )
    const signature = await connection.sendTransaction(tx)
    logNewMint(
        assetName.toUpperCase(),
        decimals,
        quantity,
        mintKeypair.publicKey,
        signature
    )
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

    const tx = await buildTransaction(
        connection,
        payer.publicKey,
        [payer],
        [mintToWalletIx]
    )
    await connection.sendTransaction(tx)
}
