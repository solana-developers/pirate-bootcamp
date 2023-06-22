pub use crate::errors::SevenSeasError;
use anchor_lang::prelude::*;
pub mod errors;
pub mod state;
pub use state::*;
pub mod instructions;

use anchor_lang::prelude::Account;
use anchor_lang::solana_program::native_token::LAMPORTS_PER_SOL;
use instructions::*;

// This is your program's public key and it will update
// automatically when you build the project.
declare_id!("2a4NcnkF5zf14JQXHAv39AsRf7jMFj13wKmTL6ZcDQNd");

pub const PLAYER_KILL_REWARD: u64 = LAMPORTS_PER_SOL / 20; // 0.05 SOL
pub const CHEST_REWARD: u64 = LAMPORTS_PER_SOL / 20; // 0.05 SOL

//pub const PLAY_GAME_FEE: u64 = LAMPORTS_PER_SOL / 50; // 0.02 SOL
pub const PLAY_GAME_FEE: u64 = 0; // 0.00 SOL

/// Seed for thread_authority PDA.
pub const THREAD_AUTHORITY_SEED: &[u8] = b"authority";

#[program]
pub mod seven_seas {

    use super::*;

    pub fn initialize(_ctx: Context<InitializeAccounts>) -> Result<()> {
        instructions::initialize::initialize(_ctx)
    }

    pub fn initialize_ship(ctx: Context<InitializeShip>) -> Result<()> {
        instructions::initialize_ship::initialize_ship(ctx)
    }

    pub fn upgrade_ship(ctx: Context<UpgradeShip>) -> Result<()> {
        instructions::upgrade_ship::upgrade_ship(ctx)
    }

    pub fn reset(_ctx: Context<Reset>) -> Result<()> {
        _ctx.accounts.game_data_account.load_mut()?.reset()
    }

    pub fn reset_ship(_ctx: Context<ResetShip>) -> Result<()> {
        _ctx.accounts
            .game_data_account
            .load_mut()?
            .reset_ship(_ctx.accounts.signer.key())
    }

    pub fn start_thread(ctx: Context<StartThread>, thread_id: Vec<u8>) -> Result<()> {
        instructions::start_thread(ctx, thread_id)
    }

    pub fn pause_thread(ctx: Context<PauseThread>, thread_id: Vec<u8>) -> Result<()> {
        instructions::pause_thread(ctx, thread_id)
    }

    pub fn resume_thread(ctx: Context<ResumeThread>, thread_id: Vec<u8>) -> Result<()> {
        instructions::resume_thread(ctx, thread_id)
    }

    pub fn on_thread_tick(ctx: Context<ThreadTick>) -> Result<()> {
        let game = &mut ctx.accounts.game_data.load_mut()?;
        game.move_in_direction_by_thread()
    }

    pub fn spawn_player(ctx: Context<SpawnPlayer>, avatar: Pubkey) -> Result<()> {
        instructions::spawn_player(ctx, avatar)
    }

    pub fn cthulhu(ctx: Context<Cthulhu>, _block_bump: u8) -> Result<()> {
        instructions::cthulhu(ctx)
    }

    pub fn shoot(ctx: Context<Shoot>, _block_bump: u8) -> Result<()> {
        instructions::shoot(ctx)
    }

    pub fn move_player_v2(ctx: Context<MovePlayer>, direction: u8, _block_bump: u8) -> Result<()> {
        instructions::move_player_v2(ctx, direction)
    }
}
