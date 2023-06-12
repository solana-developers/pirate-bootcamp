import {
    Connection,
    Keypair,
    PublicKey,
    TransactionInstruction,
    TransactionMessage,
    VersionedTransaction,
} from '@solana/web3.js'

/**
 *
 * Builds a Solana `VersionedTransaction` for sending instructions
 *
 * @param connection Connection to Solana RPC
 * @param payer The transaction fee payer (local wallet in `Anchor.toml`)
 * @param signers Any required signers as a list of `Keypair`
 * @param instructions The list of instructions to pack into the transaction
 * @returns The built transaction
 */
export async function buildTransaction(
    connection: Connection,
    payer: PublicKey,
    signers: Keypair[],
    instructions: TransactionInstruction[]
): Promise<VersionedTransaction> {
    let blockhash = await connection
        .getLatestBlockhash()
        .then((res) => res.blockhash)

    const messageV0 = new TransactionMessage({
        payerKey: payer,
        recentBlockhash: blockhash,
        instructions,
    }).compileToV0Message()

    const tx = new VersionedTransaction(messageV0)

    signers.forEach((s) => tx.sign([s]))

    return tx
}
