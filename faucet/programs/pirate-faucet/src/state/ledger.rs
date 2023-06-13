//! Swap program account state
use anchor_lang::prelude::*;
use anchor_spl::token::Mint;

use crate::error::FaucetError;

/// The `Ledger` state - the inner data of the program-derived address
/// that will be our Ledger
#[derive(Default)]
#[account]
pub struct Ledger {
    amount_received: u64,
}

impl Ledger {
    /// Max airdrops for any asset
    pub const MAX_AIRDROP_AMOUNT: u64 = 40;

    /// The Ledger's seed prefix
    pub const SEED_PREFIX: &'static str = "ledger";

    /// Anchor discriminator + u64
    pub const SPACE: usize = 8 + 8;
}

pub trait LedgerAccount<'info> {
    fn determine_airdrop_eligible(
        &mut self,
        mint: &Account<'info, Mint>,
        amount: u64,
    ) -> Result<()>;
}

impl<'info> LedgerAccount<'info> for Account<'info, Ledger> {
    fn determine_airdrop_eligible(
        &mut self,
        mint: &Account<'info, Mint>,
        amount: u64,
    ) -> Result<()> {
        if self.amount_received + amount > Ledger::MAX_AIRDROP_AMOUNT * mint.decimals as u64 {
            return Err(FaucetError::MaxAmountExceeded.into());
        } else {
            self.amount_received += amount;
        }
        Ok(())
    }
}
