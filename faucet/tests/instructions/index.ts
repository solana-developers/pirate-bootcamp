import * as anchor from '@coral-xyz/anchor'
import { Keypair, PublicKey } from '@solana/web3.js'
import { getAssociatedTokenAddressSync } from '@solana/spl-token'
import { PirateFaucet } from '../../target/types/pirate_faucet'
import { toBigIntQuantity } from '../util/token'

/**
 *
 * Sends a transaction containing the instruction for the swap program's
 * `create_faucet` instruction
 *
 * @param program The swap program as an `anchor.Program<SwapProgram>`
 * @param payer The Liquidity Provider (local wallet in `Anchor.toml`)
 * @param faucetAddress The address of the Liquidity Faucet program-derived address account
 */
export async function createFaucet(
    program: anchor.Program<PirateFaucet>,
    payer: Keypair,
    faucetAddress: PublicKey
) {
    await program.methods
        .initialize()
        .accounts({
            faucet: faucetAddress,
            payer: payer.publicKey,
            systemProgram: anchor.web3.SystemProgram.programId,
        })
        .signers([payer])
        .rpc()
}

/**
 *
 * Sends a transaction containing the instruction for the swap program's
 * `fund_faucet` instruction
 *
 * @param program The swap program as an `anchor.Program<SwapProgram>`
 * @param payer The Liquidity Provider (local wallet in `Anchor.toml`)
 * @param faucet The address of the Liquidity Faucet program-derived address account
 * @param mint The address of the mint being funded to the Liquidity Faucet
 * @param quantity The quantity to fund of the provided mint
 * @param decimals the decimals of this mint (used to calculate real quantity)
 */
export async function fundFaucet(
    program: anchor.Program<PirateFaucet>,
    payer: Keypair,
    faucet: PublicKey,
    mint: PublicKey,
    quantity: number,
    decimals: number
) {
    await program.methods
        .fund(new anchor.BN(toBigIntQuantity(quantity, decimals).toString()))
        .accounts({
            faucet,
            mint,
            faucetTokenAccount: getAssociatedTokenAddressSync(
                mint,
                faucet,
                true
            ),
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
 * @param faucet The address of the Liquidity Faucet program-derived address account
 * @param receiveMint The address of the mint the user is requesting to receive in exchange
 * @param payMint The address of the mint the user is offering to pay in the swap
 * @param quantity The quantity of the mint the user is offering to pay
 * @param decimals The decimals of the mint the user is offering to pay (used to calculate real quantity)
 */
export async function requestAirdrop(
    program: anchor.Program<PirateFaucet>,
    payer: Keypair,
    faucet: PublicKey,
    mint: PublicKey,
    quantity: number,
    decimals: number
) {
    await program.methods
        .requestAirdrop(
            // new anchor.BN(toBigIntQuantity(quantity, decimals).toString())
            new anchor.BN(quantity)
        )
        .accounts({
            faucet,
            mint,
            faucetTokenAccount: getAssociatedTokenAddressSync(
                mint,
                faucet,
                true
            ),
            payerTokenAccount: getAssociatedTokenAddressSync(
                mint,
                payer.publicKey
            ),
            payer: payer.publicKey,
            tokenProgram: anchor.utils.token.TOKEN_PROGRAM_ID,
        })
        .signers([payer])
        .rpc()
}
