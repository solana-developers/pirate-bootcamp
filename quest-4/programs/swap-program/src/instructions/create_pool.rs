//! Instruction: InitializePriceData
use anchor_lang::prelude::*;

use crate::state::*;

/// Initialize the program by creating the liquidity pool
pub fn create_pool(ctx: Context<CreatePool>) -> Result<()> {
    // Initialize the new `LiquidityPool` state
    ctx.accounts.pool.set_inner(LiquidityPool::new(
        *ctx.bumps
            .get("pool")
            .expect("Failed to fetch bump for `pool`"),
    ));
    Ok(())
}

#[derive(Accounts)]
pub struct CreatePool<'info> {
    /// Liquidity Pool
    #[account(
        init,
        space = LiquidityPool::SPACE,
        payer = payer,
        seeds = [LiquidityPool::SEED_PREFIX.as_bytes()],
        bump,
    )]
    pub pool: Account<'info, LiquidityPool>,
    /// Rent payer
    #[account(mut)]
    pub payer: Signer<'info>,
    /// System Program: Required for creating the Liquidity Pool
    pub system_program: Program<'info, System>,
}
