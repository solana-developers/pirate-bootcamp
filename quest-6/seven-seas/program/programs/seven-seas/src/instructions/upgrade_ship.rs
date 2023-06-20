use anchor_lang::prelude::*;
pub use crate::errors::SevenSeasError;

use anchor_spl::{
    token::{Mint, Token, TokenAccount, Transfer},
};
use anchor_lang::prelude::Account;

use crate::{TOKEN_DECIMAL_MULTIPLIER, Ship};

pub fn upgrade_ship(ctx: Context<UpgradeShip>) -> Result<()> {        
    let transfer_instruction = Transfer {
        from: ctx.accounts.player_token_account.to_account_info(),
        to: ctx.accounts.vault_token_account.to_account_info(),
        authority: ctx.accounts.signer.to_account_info(),
    };

    let cpi_ctx = CpiContext::new(
        ctx.accounts.token_program.to_account_info(),
        transfer_instruction
    );

    let cost: u64;
    match ctx.accounts.new_ship.upgrades {
        0 => {
            ctx.accounts.new_ship.health = 50;
            ctx.accounts.new_ship.upgrades = 1;                
            cost = 5;
        },
        1 => {
            ctx.accounts.new_ship.health = 120;
            ctx.accounts.new_ship.upgrades = 2;                
            cost = 200;
        },
        2 => {
            ctx.accounts.new_ship.health = 300;
            ctx.accounts.new_ship.upgrades = 3;                
            cost = 1500;
        },
        3 => {
            ctx.accounts.new_ship.health = 500;
            ctx.accounts.new_ship.upgrades = 4;                
            cost = 25000;
        }
        _ => {
            return Err(SevenSeasError::MaxShipLevelReached.into());
        }  
    }
    anchor_spl::token::transfer(cpi_ctx, cost * TOKEN_DECIMAL_MULTIPLIER)?;           

    msg!("Ship upgraded to level: {}", ctx.accounts.new_ship.upgrades);

    Ok(())
}

#[derive(Accounts)]
pub struct UpgradeShip<'info> {
    #[account(mut)]
    pub signer: Signer<'info>,
    #[account(
        seeds = [b"ship", nft_account.key().as_ref()],
        bump
    )]
    #[account(mut)]
    pub new_ship: Account<'info, Ship>,
    /// CHECK:
    #[account(mut)]
    pub nft_account: AccountInfo<'info>,
    pub system_program: Program<'info, System>,
    #[account( 
        mut,     
        associated_token::mint = mint_of_token_being_sent,
        associated_token::authority = signer      
    )]
    pub player_token_account: Account<'info, TokenAccount>,
    #[account(
        mut,
        seeds=[b"token_vault".as_ref(), mint_of_token_being_sent.key().as_ref()],
        token::mint=mint_of_token_being_sent,
        bump
    )]
    pub vault_token_account: Account<'info, TokenAccount>,
    pub mint_of_token_being_sent: Account<'info, Mint>,
    pub token_program: Program<'info, Token>,
}
