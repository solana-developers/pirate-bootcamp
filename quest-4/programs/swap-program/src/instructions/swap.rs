//! Instruction: SwapDia
use anchor_lang::prelude::*;
use anchor_spl::associated_token::AssociatedToken;
use anchor_spl::token;

use crate::error::*;
use crate::state::*;

/// Swap assets using the DEX
pub fn swap(ctx: Context<Swap>, amount_to_swap: u64) -> Result<()> {
    // Make sure the amount is not zero
    if amount_to_swap == 0 {
        return Err(SwapProgramError::InvalidSwapZeroAmount.into());
    }

    let pool = &mut ctx.accounts.pool;

    // Receive: The assets the user is requesting to receive in exchange:
    // (Mint, From, To)
    let receive = (
        ctx.accounts.receive_mint.as_ref(),
        ctx.accounts.pool_receive_token_account.as_ref(),
        ctx.accounts.payer_receive_token_account.as_ref(),
    );

    // Pay: The assets the user is proposing to pay in the swap:
    // (Mint, From, To, Amount)
    let pay = (
        ctx.accounts.pay_mint.as_ref(),
        ctx.accounts.payer_pay_token_account.as_ref(),
        ctx.accounts.pool_pay_token_account.as_ref(),
        amount_to_swap,
    );

    pool.process_swap(
        receive,
        pay,
        &ctx.accounts.payer,
        &ctx.accounts.token_program,
    )
}

#[derive(Accounts)]
pub struct Swap<'info> {
    /// Liquidity Pool
    #[account(
        mut,
        seeds = [LiquidityPool::SEED_PREFIX.as_bytes()],
        bump = pool.bump,
    )]
    pub pool: Account<'info, LiquidityPool>,
    /// The mint account for the asset the user is requesting to receive in
    /// exchange
    #[account(
        constraint = !receive_mint.key().eq(&pay_mint.key()) @ SwapProgramError::InvalidSwapMatchingAssets
    )]
    pub receive_mint: Box<Account<'info, token::Mint>>,
    /// The Liquidity Pool's token account for the mint of the asset the user is
    /// requesting to receive in exchange (which will be debited)
    #[account(
        mut,
        associated_token::mint = receive_mint,
        associated_token::authority = pool,
    )]
    pub pool_receive_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The user's token account for the mint of the asset the user is
    /// requesting to receive in exchange (which will be credited)
    #[account(
        init_if_needed,
        payer = payer,
        associated_token::mint = receive_mint,
        associated_token::authority = payer,
    )]
    pub payer_receive_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The mint account for the asset the user is proposing to pay in the swap
    pub pay_mint: Box<Account<'info, token::Mint>>,
    /// The Liquidity Pool's token account for the mint of the asset the user is
    /// proposing to pay in the swap (which will be credited)
    #[account(
        mut,
        associated_token::mint = pay_mint,
        associated_token::authority = pool,
    )]
    pub pool_pay_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The user's token account for the mint of the asset the user is
    /// proposing to pay in the swap (which will be debited)
    #[account(
        mut,
        associated_token::mint = pay_mint,
        associated_token::authority = payer,
    )]
    pub payer_pay_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The authority requesting to swap (user)
    #[account(mut)]
    pub payer: Signer<'info>,
    /// Token Program: Required for transferring the assets between all token
    /// accounts involved in the swap
    pub token_program: Program<'info, token::Token>,
    pub system_program: Program<'info, System>,
    pub associated_token_program: Program<'info, AssociatedToken>,
}
