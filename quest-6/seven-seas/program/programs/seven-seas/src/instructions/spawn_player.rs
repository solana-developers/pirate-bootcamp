use anchor_lang::prelude::*;
use crate::{PLAYER_KILL_REWARD, PLAY_GAME_FEE, Ship, ChestVaultAccount, GameDataAccount, CHEST_REWARD};
pub use crate::errors::SevenSeasError;
use anchor_spl::{
    token::{Mint, Token, TokenAccount}, associated_token::AssociatedToken,
};

pub fn spawn_player(ctx: Context<SpawnPlayer>, avatar: Pubkey) -> Result<()> {
    let game = &mut ctx.accounts.game_data_account.load_mut()?;
    let ship = &mut ctx.accounts.ship;
    
    let decimals = ctx.accounts.cannon_mint.decimals;
    ship.cannons = ctx.accounts.cannon_token_account.amount / ((u64::pow(10, decimals as u32) as u64));

    let extra_health = ctx.accounts.rum_token_account.amount / ((u64::pow(10, decimals as u32) as u64));

    msg!("Spawned player! With {} cannons", ship.cannons);

    match game.spawn_player(ctx.accounts.player.to_account_info(), avatar, ship, extra_health) {
        Ok(_val) => {
            let cpi_context = CpiContext::new(
                ctx.accounts.system_program.to_account_info().clone(),
                anchor_lang::system_program::Transfer {
                    from: ctx.accounts.token_account_owner.to_account_info().clone(),
                    to: ctx.accounts.chest_vault.to_account_info().clone(),
                },
            );
            anchor_lang::system_program::transfer(
                cpi_context,
                PLAYER_KILL_REWARD + PLAY_GAME_FEE,
            )?;
        }
        Err(err) => {
            panic!("Error: {}", err);
        }
    }
    match game.spawn_chest(ctx.accounts.player.to_account_info()) {
        Ok(_val) => {
            let cpi_context = CpiContext::new(
                ctx.accounts.system_program.to_account_info().clone(),
                anchor_lang::system_program::Transfer {
                    from: ctx.accounts.token_account_owner.to_account_info().clone(),
                    to: ctx.accounts.chest_vault.to_account_info().clone(),
                },
            );
            anchor_lang::system_program::transfer(cpi_context, CHEST_REWARD)?;
        }
        Err(err) => {
            panic!("Error: {}", err);
        }
    }
    Ok(())
}

#[derive(Accounts)]
pub struct SpawnPlayer<'info> {
    /// CHECK: needs to sign for every move later. Would be better if both are signers
    #[account(mut)]
    pub player: AccountInfo<'info>,
    #[account(mut)]
    pub token_account_owner: Signer<'info>,
    #[account(
        mut,
        seeds = [b"chestVault"],
        bump
    )]
    pub chest_vault: Account<'info, ChestVaultAccount>,
    #[account(mut)]
    pub game_data_account: AccountLoader<'info, GameDataAccount>,
    #[account(
        mut,
        seeds = [b"ship", nft_account.key().as_ref()],
        bump
    )]
    pub ship: Account<'info, Ship>,
    /// CHECK: change to token account later
    pub nft_account: AccountInfo<'info>,
    pub system_program: Program<'info, System>,
    #[account(      
        init_if_needed,
        payer = token_account_owner,
        associated_token::mint = cannon_mint,
        associated_token::authority = token_account_owner      
    )]
    pub cannon_token_account: Account<'info, TokenAccount>,
    pub cannon_mint: Account<'info, Mint>,
    #[account(      
        init_if_needed,
        payer = token_account_owner,
        associated_token::mint = rum_mint,
        associated_token::authority = token_account_owner      
    )]
    pub rum_token_account: Account<'info, TokenAccount>,
    pub rum_mint: Account<'info, Mint>,
    pub token_program: Program<'info, Token>,
    pub associated_token_program: Program<'info, AssociatedToken>,
}
