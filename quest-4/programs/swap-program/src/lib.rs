//! Swap program
#![allow(clippy::result_large_err)]
pub mod error;
pub mod instructions;
pub mod state;

use anchor_lang::prelude::*;
use instructions::*;

declare_id!("FVCG6YkMbACgskY3ZrwqWFr45ERgJuKrf7C9dRDh6LjX");

#[program]
pub mod swap_program {
    use super::*;

    /// Initialize the program by creating the liquidity pool
    pub fn create_pool(ctx: Context<CreatePool>) -> Result<()> {
        instructions::create_pool(ctx)
    }

    /// Provide liquidity to the pool by funding it with some asset
    pub fn fund_pool(ctx: Context<FundPool>, amount: u64) -> Result<()> {
        instructions::fund_pool(ctx, amount)
    }

    /// Swap assets using the DEX
    pub fn swap(ctx: Context<Swap>, amount_to_swap: u64) -> Result<()> {
        instructions::swap(ctx, amount_to_swap)
    }
}
