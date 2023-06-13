//! Instruction: InitializePriceData
use anchor_lang::prelude::*;

use crate::state::*;

/// Initialize the faucet
pub fn initialize(ctx: Context<Initialize>) -> Result<()> {
    ctx.accounts.faucet.set_inner(Faucet::new(
        *ctx.bumps
            .get("faucet")
            .expect("Failed to fetch bump for `faucet`"),
    ));
    Ok(())
}

#[derive(Accounts)]
pub struct Initialize<'info> {
    #[account(
        init,
        space = Faucet::SPACE,
        payer = payer,
        seeds = [Faucet::SEED_PREFIX.as_bytes()],
        bump,
    )]
    pub faucet: Account<'info, Faucet>,
    #[account(mut)]
    pub payer: Signer<'info>,
    pub system_program: Program<'info, System>,
}
