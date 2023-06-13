import {
    Connection,
    Keypair,
    PublicKey,
    TransactionInstruction,
    VersionedTransaction,
    TransactionMessage,
    AddressLookupTableProgram,
    AddressLookupTableAccount,
} from '@solana/web3.js'
import { sleepSeconds } from '.'

/**
 *
 * Creates an Address Lookup Table
 *
 * @param connection Connection to Solana RPC
 * @param payer Transaction Fee Payer and Lookup Table Authority
 * @returns Address of the Lookup Table
 */
export async function createAddressLookupTable(
    connection: Connection,
    payer: Keypair
): Promise<PublicKey> {
    // You must use `max` for the derivation of the Lookup Table address to work consistently
    let recentSlot = await connection.getSlot('max')
    let [createLookupTableIx, lookupTable] =
        AddressLookupTableProgram.createLookupTable({
            authority: payer.publicKey,
            payer: payer.publicKey,
            recentSlot,
        })
    const tx = await buildTransactionV0(
        connection,
        [createLookupTableIx],
        payer.publicKey,
        [payer]
    )
    await connection.sendTransaction(tx)
    return lookupTable
}

/**
 *
 * Extends an Address Lookup Table by adding new addresses to the table
 *
 * @param connection Connection to Solana RPC
 * @param payer Transaction Fee Payer and Lookup Table Authority
 * @param lookupTable Address of the Lookup Table
 * @param addresses Addresses to add to the Lookup Table
 */
export async function extendAddressLookupTable(
    connection: Connection,
    payer: Keypair,
    lookupTable: PublicKey,
    addresses: PublicKey[]
): Promise<void> {
    let extendLookupTableIx = AddressLookupTableProgram.extendLookupTable({
        addresses,
        authority: payer.publicKey,
        lookupTable,
        payer: payer.publicKey,
    })
    const tx = await buildTransactionV0(
        connection,
        [extendLookupTableIx],
        payer.publicKey,
        [payer]
    )
    await connection.sendTransaction(tx)
}

/**
 *
 * Get an Address Lookup Table account
 *
 * @param connection Connection to Solana RPC
 * @param lookupTablePubkey The address of the Address Lookup Table
 */
export async function getAddressLookupTable(
    connection: Connection,
    lookupTablePubkey: PublicKey
): Promise<AddressLookupTableAccount> {
    return connection
        .getAddressLookupTable(lookupTablePubkey)
        .then((res) => res.value)
}

/**
 *
 * Print the contents of an Address Lookup Table
 *
 * @param connection Connection to Solana RPC
 * @param lookupTablePubkey The address of the Address Lookup Table
 */
export async function printAddressLookupTable(
    connection: Connection,
    lookupTablePubkey: PublicKey
): Promise<void> {
    // Lookup Table fetching can lag if this sleep isn't here
    await sleepSeconds(2)
    const lookupTableAccount = await getAddressLookupTable(
        connection,
        lookupTablePubkey
    )
    console.log(`Lookup Table: ${lookupTablePubkey}`)
    for (let i = 0; i < lookupTableAccount.state.addresses.length; i++) {
        const address = lookupTableAccount.state.addresses[i]
        console.log(
            `   Index: ${i
                .toString()
                .padEnd(2)}  Address: ${address.toBase58()}`
        )
    }
}

/**
 *
 * Builds a transaction using the V0 format
 *
 * @param connection Connection to Solana RPC
 * @param instructions Instructions to send
 * @param payer Transaction Fee Payer
 * @param signers All required signers, in order
 * @returns The transaction v0
 */
export async function buildTransactionV0(
    connection: Connection,
    instructions: TransactionInstruction[],
    payer: PublicKey,
    signers: Keypair[]
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

/**
 *
 * Builds a transaction using the V0 format
 * using an Address Lookup Table
 *
 * @param connection Connection to Solana RPC
 * @param instructions Instructions to send
 * @param payer Transaction Fee Payer
 * @param signers All required signers, in order
 * @param lookupTablePubkey The address of the Address Lookup Table to use
 * @returns The transaction v0
 */
export async function buildTransactionV0WithLookupTable(
    connection: Connection,
    instructions: TransactionInstruction[],
    payer: PublicKey,
    signers: Keypair[],
    lookupTablePubkey: PublicKey
): Promise<VersionedTransaction> {
    // Lookup Table fetching can lag if this sleep isn't here
    await sleepSeconds(2)
    const lookupTableAccount = await getAddressLookupTable(
        connection,
        lookupTablePubkey
    )
    let blockhash = await connection
        .getLatestBlockhash()
        .then((res) => res.blockhash)
    // Compile V0 Message with the Lookup Table
    const messageV0 = new TransactionMessage({
        payerKey: payer,
        recentBlockhash: blockhash,
        instructions,
    }).compileToV0Message([lookupTableAccount])
    const tx = new VersionedTransaction(messageV0)
    signers.forEach((s) => tx.sign([s]))
    return tx
}
