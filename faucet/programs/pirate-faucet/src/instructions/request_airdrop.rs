//! Instruction: SwapDia
use anchor_lang::prelude::*;
use anchor_spl::associated_token::AssociatedToken;
use anchor_spl::token;

use crate::error::*;
use crate::state::*;

/// Request an airdrop from the faucet
pub fn request_airdrop(ctx: Context<RequestAirdrop>, amount: u64) -> Result<()> {
    if amount == 0 {
        return Err(FaucetError::InvalidSwapZeroAmount.into());
    }

    let faucet = &mut ctx.accounts.faucet;

    let receive = (
        ctx.accounts.mint.as_ref(),
        ctx.accounts.faucet_token_account.as_ref(),
        ctx.accounts.payer_token_account.as_ref(),
        amount,
    );

    faucet.process_airdrop(
        receive,
        &mut ctx.accounts.ledger,
        &ctx.accounts.token_program,
    )
}

#[derive(Accounts)]
pub struct RequestAirdrop<'info> {
    #[account(
        mut,
        seeds = [Faucet::SEED_PREFIX.as_bytes()],
        bump = faucet.bump,
    )]
    pub faucet: Account<'info, Faucet>,
    pub mint: Box<Account<'info, token::Mint>>,
    #[account(
        mut,
        associated_token::mint = mint,
        associated_token::authority = faucet,
    )]
    pub faucet_token_account: Box<Account<'info, token::TokenAccount>>,
    #[account(
        init_if_needed,
        payer = payer,
        associated_token::mint = mint,
        associated_token::authority = payer,
    )]
    pub payer_token_account: Box<Account<'info, token::TokenAccount>>,
    #[account(
        init_if_needed,
        space = Ledger::SPACE,
        payer = payer,
        seeds = [Ledger::SEED_PREFIX.as_bytes(), payer.key().as_ref(), mint.key().as_ref()],
        bump,
    )]
    pub ledger: Account<'info, Ledger>,
    #[account(mut)]
    pub payer: Signer<'info>,
    pub token_program: Program<'info, token::Token>,
    pub system_program: Program<'info, System>,
    pub associated_token_program: Program<'info, AssociatedToken>,
}
