import * as anchor from '@coral-xyz/anchor'
import { Keypair, PublicKey } from '@solana/web3.js'
import { getAssociatedTokenAddressSync } from '@solana/spl-token'
import { SwapProgram } from '../../target/types/swap_program'
import { toBigIntQuantity } from '../util/token'

/**
 *
 * Sends a transaction containing the instruction for the swap program's
 * `create_pool` instruction
 *
 * @param program The swap program as an `anchor.Program<SwapProgram>`
 * @param payer The Liquidity Provider (local wallet in `Anchor.toml`)
 * @param poolAddress The address of the Liquidity Pool program-derived address account
 */
export async function createPool(
    program: anchor.Program<SwapProgram>,
    payer: Keypair,
    poolAddress: PublicKey
) {
    await program.methods
        .createPool()
        .accounts({
            pool: poolAddress,
            payer: payer.publicKey,
            systemProgram: anchor.web3.SystemProgram.programId,
        })
        .signers([payer])
        .rpc()
}

/**
 *
 * Sends a transaction containing the instruction for the swap program's
 * `fund_pool` instruction
 *
 * @param program The swap program as an `anchor.Program<SwapProgram>`
 * @param payer The Liquidity Provider (local wallet in `Anchor.toml`)
 * @param pool The address of the Liquidity Pool program-derived address account
 * @param mint The address of the mint being funded to the Liquidity Pool
 * @param quantity The quantity to fund of the provided mint
 * @param decimals the decimals of this mint (used to calculate real quantity)
 */
export async function fundPool(
    program: anchor.Program<SwapProgram>,
    payer: Keypair,
    pool: PublicKey,
    mint: PublicKey,
    quantity: number,
    decimals: number
) {
    await program.methods
        .fundPool(
            new anchor.BN(toBigIntQuantity(quantity, decimals).toString())
        )
        .accounts({
            pool,
            mint,
            poolTokenAccount: getAssociatedTokenAddressSync(mint, pool, true),
            payerTokenAccount: getAssociatedTokenAddressSync(
                mint,
                payer.publicKey
            ),
            payer: payer.publicKey,
            systemProgram: anchor.web3.SystemProgram.programId,
            tokenProgram: anchor.utils.token.TOKEN_PROGRAM_ID,
            associatedTokenProgram: anchor.utils.token.ASSOCIATED_PROGRAM_ID,
        })
        .signers([payer])
        .rpc()
}

/**
 *
 * Sends a transaction containing the instruction for the swap program's
 * `swap` instruction
 *
 * @param program The swap program as an `anchor.Program<SwapProgram>`
 * @param payer The Liquidity Provider (local wallet in `Anchor.toml`)
 * @param pool The address of the Liquidity Pool program-derived address account
 * @param receiveMint The address of the mint the user is requesting to receive in exchange
 * @param payMint The address of the mint the user is offering to pay in the swap
 * @param quantity The quantity of the mint the user is offering to pay
 * @param decimals The decimals of the mint the user is offering to pay (used to calculate real quantity)
 */
export async function swap(
    program: anchor.Program<SwapProgram>,
    payer: Keypair,
    pool: PublicKey,
    receiveMint: PublicKey,
    payMint: PublicKey,
    quantity: number,
    decimals: number
) {
    await program.methods
        .swap(new anchor.BN(toBigIntQuantity(quantity, decimals).toString()))
        .accounts({
            pool,
            receiveMint,
            poolReceiveTokenAccount: getAssociatedTokenAddressSync(
                receiveMint,
                pool,
                true
            ),
            payerReceiveTokenAccount: getAssociatedTokenAddressSync(
                receiveMint,
                payer.publicKey
            ),
            payMint,
            poolPayTokenAccount: getAssociatedTokenAddressSync(
                payMint,
                pool,
                true
            ),
            payerPayTokenAccount: getAssociatedTokenAddressSync(
                payMint,
                payer.publicKey
            ),
            payer: payer.publicKey,
            tokenProgram: anchor.utils.token.TOKEN_PROGRAM_ID,
        })
        .signers([payer])
        .rpc()
}
