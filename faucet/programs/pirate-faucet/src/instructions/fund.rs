//! Instruction: InitializePriceData
use anchor_lang::prelude::*;
use anchor_spl::{associated_token, token};

use crate::state::*;

/// Fund tokens to the faucet
pub fn fund(ctx: Context<Fund>, amount: u64) -> Result<()> {
    let faucet = &mut ctx.accounts.faucet;

    let deposit = (
        &ctx.accounts.mint,
        &ctx.accounts.payer_token_account,
        &ctx.accounts.faucet_token_account,
        amount,
    );

    faucet.fund(
        deposit,
        &ctx.accounts.payer,
        &ctx.accounts.system_program,
        &ctx.accounts.token_program,
    )
}

#[derive(Accounts)]
pub struct Fund<'info> {
    #[account(
        mut,
        seeds = [Faucet::SEED_PREFIX.as_bytes()],
        bump = faucet.bump,
    )]
    pub faucet: Account<'info, Faucet>,
    pub mint: Account<'info, token::Mint>,
    #[account(
        init_if_needed,
        payer = payer,
        associated_token::mint = mint,
        associated_token::authority = faucet,
    )]
    pub faucet_token_account: Account<'info, token::TokenAccount>,
    #[account(
        mut,
        associated_token::mint = mint,
        associated_token::authority = payer,
    )]
    pub payer_token_account: Account<'info, token::TokenAccount>,
    #[account(mut)]
    pub payer: Signer<'info>,
    pub system_program: Program<'info, System>,
    pub token_program: Program<'info, token::Token>,
    pub associated_token_program: Program<'info, associated_token::AssociatedToken>,
}
