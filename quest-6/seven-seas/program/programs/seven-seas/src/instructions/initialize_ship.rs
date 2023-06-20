use anchor_lang::prelude::*;
use crate::Ship;
pub use crate::errors::SevenSeasError;
use anchor_lang::prelude::Account;

pub fn initialize_ship(ctx: Context<InitializeShip>) -> Result<()> {
    msg!("Ship Initialized!");
    ctx.accounts.new_ship.health = 100;
    ctx.accounts.new_ship.start_health = 100;
    ctx.accounts.new_ship.level = 1;
    ctx.accounts.new_ship.upgrades = 0;
    ctx.accounts.new_ship.cannons = 1;
    Ok(())
}

#[derive(Accounts)]
pub struct InitializeShip<'info> {
    #[account(mut)]
    pub signer: Signer<'info>,
    #[account(
        init,
        payer = signer, 
        seeds = [b"ship", nft_account.key().as_ref()],
        bump,
        space = 1024
    )]
    pub new_ship: Account<'info, Ship>,
    /// CHECK:
    pub nft_account: AccountInfo<'info>,
    pub system_program: Program<'info, System>,
}
