use anchor_lang::prelude::*;
use crate::{GameDataAccount, GameActionHistory};
pub use crate::errors::SevenSeasError;
use anchor_spl::{
    token::{Mint, Token, TokenAccount}, associated_token::AssociatedToken,
};

pub fn move_player_v2(ctx: Context<MovePlayer>, direction: u8) -> Result<()> {
    let game = &mut ctx.accounts.game_data_account.load_mut()?;

    match game.move_in_direction(
        direction,
        ctx.accounts.player.to_account_info(),
        ctx.accounts.chest_vault.to_account_info(),
        ctx.accounts.vault_token_account.to_account_info(),
        ctx.accounts.player_token_account.to_account_info(),
        ctx.accounts.token_account_owner_pda.to_account_info(),
        ctx.accounts.token_program.to_account_info(),
        ctx.bumps["token_account_owner_pda"],
        &mut ctx.accounts.game_actions,
    ) {
        Ok(_val) => {}
        Err(err) => {
            panic!("Error: {}", err);
        }
    }
    game.print().unwrap();
    Ok(())
}


#[derive(Accounts)]
pub struct MovePlayer<'info> {
    /// CHECK:
    #[account(mut)]
    pub chest_vault: AccountInfo<'info>,
    #[account(mut)]
    pub game_data_account: AccountLoader<'info, GameDataAccount>,
    #[account(mut)]
    pub player: Signer<'info>,
    /// CHECK:
    pub token_account_owner: AccountInfo<'info>,
    pub system_program: Program<'info, System>,
    #[account(      
        init_if_needed,
        payer = player,
        associated_token::mint = mint_of_token_being_sent,
        associated_token::authority = token_account_owner      
    )]
    pub player_token_account: Account<'info, TokenAccount>,
    #[account(
        init_if_needed,
        payer = player,
        seeds=[b"token_vault".as_ref(), mint_of_token_being_sent.key().as_ref()],
        token::mint=mint_of_token_being_sent,
        token::authority=token_account_owner_pda,
        bump
    )]
    pub vault_token_account: Account<'info, TokenAccount>,
    /// CHECK:
    #[account(
        mut,
        seeds=[b"token_account_owner_pda".as_ref()],
        bump
    )]
    pub token_account_owner_pda: AccountInfo<'info>,    
    pub mint_of_token_being_sent: Account<'info, Mint>,
    pub token_program: Program<'info, Token>,
    pub associated_token_program: Program<'info, AssociatedToken>,
    #[account(mut)]
    pub game_actions: Account<'info, GameActionHistory>,
}
