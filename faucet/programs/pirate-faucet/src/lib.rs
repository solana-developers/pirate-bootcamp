//! Swap program
#![allow(clippy::result_large_err)]
pub mod error;
pub mod instructions;
pub mod state;

use anchor_lang::prelude::*;
use instructions::*;

declare_id!("4qqkcdDu3porjEyRDpD7ScaDCU7jg8epBPF7HfCSLY7h");

#[program]
pub mod pirate_faucet {
    use super::*;

    /// Initialize the program by creating the liquidity pool
    pub fn initialize(ctx: Context<Initialize>) -> Result<()> {
        instructions::initialize(ctx)
    }

    /// Provide liquidity to the pool by funding it with some asset
    pub fn fund(ctx: Context<Fund>, amount: u64) -> Result<()> {
        instructions::fund(ctx, amount)
    }

    /// Swap assets using the DEX
    pub fn request_airdrop(ctx: Context<RequestAirdrop>, amount: u64) -> Result<()> {
        instructions::request_airdrop(ctx, amount)
    }
}
