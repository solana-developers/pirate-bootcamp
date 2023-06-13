import * as borsh from 'borsh'
import { Buffer } from 'buffer'
import {
    AccountMeta,
    AddressLookupTableAccount,
    Connection,
    Keypair,
    PublicKey,
    SystemProgram,
    SYSVAR_RENT_PUBKEY,
    TransactionInstruction,
    TransactionMessage,
    VersionedTransaction,
} from '@solana/web3.js'
import { TOKEN_PROGRAM_ID } from '@solana/spl-token'

/**
 * The address of the Arbitrage Program
 */
export const ARBITRAGE_PROGRAM = new PublicKey('')

/**
 * The address of the Arbitrage Program's Lookup Table
 */
export const ARBITRAGE_LOOKUP_TABLE = new PublicKey('')

/**
 * Get the PDA of the Liquidity Pool for a program
 */
export function getPoolAddress(programId: PublicKey): PublicKey {
    return PublicKey.findProgramAddressSync(
        [Buffer.from('liquidity_pool')],
        programId
    )[0]
}

/**
 * Arbitrage program instructions
 */
class ArbitrageProgramInstruction {
    instruction: number
    swap_1_program_id: Uint8Array
    swap_2_program_id: Uint8Array
    concurrency: number
    temperature: number
    constructor(props: {
        swapProgram1: PublicKey
        swapProgram2: PublicKey
        concurrency: number
        temperature: number
    }) {
        this.instruction = 0
        this.swap_1_program_id = props.swapProgram1.toBuffer()
        this.swap_2_program_id = props.swapProgram2.toBuffer()
        this.concurrency = props.concurrency
        this.temperature = props.temperature
    }
    toBuffer() {
        return Buffer.from(
            borsh.serialize(ArbitrageProgramInstructionSchema, this)
        )
    }
}

const ArbitrageProgramInstructionSchema = new Map([
    [
        ArbitrageProgramInstruction,
        {
            kind: 'struct',
            fields: [
                ['instruction', 'u8'],
                ['swap_1_program_id', [32]],
                ['swap_2_program_id', [32]],
                ['concurrency', 'u8'],
                ['temperature', 'u8'],
            ],
        },
    ],
])

/**
 *
 * "Default" `AccountMeta` (marks as mutable non-signer)
 * Used for Token Accounts to "lock" them on the Sealevel runtime
 *
 * @param pubkey Address of the account
 * @returns `KeyArg`
 */
export function defaultAccountMeta(pubkey: PublicKey): AccountMeta {
    return { pubkey, isSigner: false, isWritable: true }
}

/**
 *
 * Creates the instruction for our Arbitrage Program
 *
 * @param programId Arbitrage program ID
 * @param payer Token payer (the one funding the arb)
 * @param tokenAccountsUser The payer's token accounts
 * @param tokenAccountsSwap1 Swap #1's token accounts
 * @param tokenAccountsSwap2 Swap #2's token accounts
 * @param mints The asset mints
 * @param concurrency How many accounts we're evaluating at once
 * @param swapProgram1 Swap #1 program ID
 * @param swapProgram2 Swap #2 program ID
 * @returns `TransactionInstruction`
 */
export function createArbitrageInstruction(
    payer: PublicKey,
    tokenAccountsUser: PublicKey[],
    tokenAccountsSwap1: PublicKey[],
    tokenAccountsSwap2: PublicKey[],
    mints: PublicKey[],
    concurrency: number,
    temperature: number,
    swapProgram1: PublicKey,
    swapProgram2: PublicKey
): TransactionInstruction {
    let swapPool1 = getPoolAddress(swapProgram1)
    let swapPool2 = getPoolAddress(swapProgram2)
    const data = new ArbitrageProgramInstruction({
        swapProgram1,
        swapProgram2,
        concurrency,
        temperature,
    }).toBuffer()
    let keys: AccountMeta[] = [
        // Payer
        { pubkey: payer, isSigner: true, isWritable: true },
        // Token Program
        { pubkey: TOKEN_PROGRAM_ID, isSigner: false, isWritable: false },
        // Liquidity Pool for Swap #1
        defaultAccountMeta(swapPool1),
        // Liquidity Pool for Swap #2
        defaultAccountMeta(swapPool2),
    ]
    // [Token Accounts for User]
    tokenAccountsUser.forEach((a) => keys.push(defaultAccountMeta(a)))
    // [Token Accounts for Swap #1]
    tokenAccountsSwap1.forEach((a) => keys.push(defaultAccountMeta(a)))
    // [Token Accounts for Swap #2]
    tokenAccountsSwap2.forEach((a) => keys.push(defaultAccountMeta(a)))
    // [Mint Accounts]
    mints.forEach((a) => keys.push(defaultAccountMeta(a)))
    return new TransactionInstruction({
        keys,
        programId: ARBITRAGE_PROGRAM,
        data,
    })
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
): Promise<AddressLookupTableAccount | null> {
    return connection
        .getAddressLookupTable(lookupTablePubkey)
        .then((res) => res.value)
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
    signers: Keypair[]
): Promise<VersionedTransaction> {
    const lookupTableAccount = await getAddressLookupTable(
        connection,
        ARBITRAGE_LOOKUP_TABLE
    )
    if (lookupTableAccount == null) {
        throw `Lookup Table not found for ${ARBITRAGE_LOOKUP_TABLE.toBase58()}`
    }
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
