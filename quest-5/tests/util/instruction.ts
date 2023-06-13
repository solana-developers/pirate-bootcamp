import * as borsh from 'borsh'
import { Buffer } from 'buffer'
import {
    AccountMeta,
    PublicKey,
    SystemProgram,
    SYSVAR_RENT_PUBKEY,
    TransactionInstruction,
} from '@solana/web3.js'
import {
    TOKEN_PROGRAM_ID,
    ASSOCIATED_TOKEN_PROGRAM_ID,
} from '@solana/spl-token'

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
    programId: PublicKey,
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
        // System Program
        { pubkey: SystemProgram.programId, isSigner: false, isWritable: false },
        // Associated Token Program
        {
            pubkey: ASSOCIATED_TOKEN_PROGRAM_ID,
            isSigner: false,
            isWritable: false,
        },
        // Swap #1 Program
        { pubkey: swapProgram1, isSigner: false, isWritable: false },
        // Swap #2 Program
        { pubkey: swapProgram2, isSigner: false, isWritable: false },
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
        programId,
        data,
    })
}
