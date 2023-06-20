use anchor_lang::prelude::*;
use crate::{GameDataAccount, ChestVaultAccount, GameActionHistory};
pub use crate::errors::SevenSeasError;
use anchor_spl::{
    token::{Mint, Token, TokenAccount},
};
use anchor_lang::prelude::Account;

pub fn initialize(_ctx: Context<InitializeAccounts>) -> Result<()> {
    msg!("Initialized!");
    Ok(())
}

#[derive(Accounts)]
pub struct InitializeAccounts<'info> {
    #[account(mut)]
    pub signer: Signer<'info>,
    // We must specify the space in order to initialize an account.
    // First 8 bytes are default account discriminator,
    #[account(
        init,
        payer = signer, 
        seeds = [b"level"],
        bump,
        space = 10240
    )]
    pub new_game_data_account: AccountLoader<'info, GameDataAccount>,
    // This is the PDA in which we will deposit the reward SOl and
    // from where we send it back to the first player reaching the chest.
    #[account(
        init,
        seeds = [b"chestVault"],
        bump,
        payer = signer,
        space = 8
    )]
    pub chest_vault: Box<Account<'info, ChestVaultAccount>>,
    // These are used so that the clients can animate certain actions in the game.
    #[account(
        init,
        seeds = [b"gameActions"],
        bump,
        payer = signer,
        space = 4096
    )]
    pub game_actions: Box<Account<'info, GameActionHistory>>,
    /// CHECK: Derived PDAs
    #[account(
        init,
        payer = signer,
        seeds=[b"token_account_owner_pda".as_ref()],
        bump,
        space = 8
    )]
    token_account_owner_pda: AccountInfo<'info>,

    #[account(
        init,
        payer = signer,
        seeds=[b"token_vault".as_ref(), mint_of_token_being_sent.key().as_ref()],
        token::mint=mint_of_token_being_sent,
        token::authority=token_account_owner_pda,
        bump
    )]
    vault_token_account: Account<'info, TokenAccount>,
    pub mint_of_token_being_sent: Account<'info, Mint>,    
    pub system_program: Program<'info, System>,
    pub token_program: Program<'info, Token>,
}
