use anchor_lang::prelude::*;
use anchor_lang::solana_program::{
    instruction::Instruction, native_token::LAMPORTS_PER_SOL, system_program,
};

use anchor_lang::InstructionData;
use clockwork_sdk::state::{Thread, ThreadAccount};

declare_id!("HMz4pAww1UAhwnsE2WFEkSTazKgFf5pwUAnnMxvDbrjf");

#[account]
pub struct GameData {
    pub authority: Pubkey,
    pub wood: u64,
    pub lumberjacks: u64,
    pub gold: u64,
    pub teeth_upgrade: u64,
    pub updated_at: i64,
}

#[error_code]
pub enum GameErrorCode {
    #[msg("Not enough wood.")]
    NotEnoughWood,
    #[msg("Not enough gold.")]
    NotEnoughGold,
}

// Balancing
// Selling Wood
const WOOD_PER_SELL: u64 = 10;
const GOLD_PER_WOOD_BASE: u64 = 5;
const GOLD_PER_WOOD_TEETH_MULTIPLIER: u64 = 2;

// Upgrade Teeth
const TEETH_UPGRADE_BASE_COST: u64 = 50;
const TEETH_UPGRADE_COST_MULTIPLIER: u64 = 1;

// Buy Lumberjack
const LUMBERJACK_BASE_COST: u64 = 20;
const LUMBERJACK_COST_MULTIPLIER: u64 = 5;

// Thread tick config
const THREAD_TICK_TIME_IN_SECONDS: &str = "10";

/// Seed for thread_authority PDA.
pub const THREAD_AUTHORITY_SEED: &[u8] = b"authority";

/// Seed for deriving the `Counter` account PDA.
pub const SEED_GAME_DATA: &[u8] = b"gameData";

#[program]
pub mod idle_game {
    use super::*;

    pub fn initialize(ctx: Context<Initialize>, thread_id: Vec<u8>) -> Result<()> {
        let system_program = &ctx.accounts.system_program;
        let clockwork_program = &ctx.accounts.clockwork_program;
        let payer = &ctx.accounts.payer;
        let thread = &ctx.accounts.thread;
        let thread_authority = &ctx.accounts.thread_authority;

        // Initialize game data.
        let game_data = &mut ctx.accounts.game_data;
        game_data.lumberjacks = 1;
        game_data.teeth_upgrade = 1;
        game_data.authority = payer.key();
        game_data.updated_at = Clock::get().unwrap().unix_timestamp;

        // 1️⃣ Prepare an instruction to automate.
        //    In this case, we will automate the ThreadTick instruction.
        let target_ix = Instruction {
            program_id: ID,
            accounts: crate::__client_accounts_thread_tick::ThreadTick {
                game_data: game_data.key(),
                thread: thread.key(),
                thread_authority: thread_authority.key(),
            }
            .to_account_metas(Some(true)),
            data: crate::instruction::OnThreadTick {}.data(),
        };

        // 2️⃣ Define a trigger for the thread.
        let trigger = clockwork_sdk::state::Trigger::Cron {
            schedule: format!("*/{} * * * * * *", THREAD_TICK_TIME_IN_SECONDS).into(),
            skippable: true,
        };

        // 3️⃣ Create a Thread via CPI
        let bump = *ctx.bumps.get("thread_authority").unwrap();
        clockwork_sdk::cpi::thread_create(
            CpiContext::new_with_signer(
                clockwork_program.to_account_info(),
                clockwork_sdk::cpi::ThreadCreate {
                    payer: payer.to_account_info(),
                    system_program: system_program.to_account_info(),
                    thread: thread.to_account_info(),
                    authority: thread_authority.to_account_info(),
                },
                &[&[THREAD_AUTHORITY_SEED,payer.key().as_ref() , &[bump]]],
            ),
            LAMPORTS_PER_SOL / 10, // amount (0.1 sol)
            thread_id,              // id
            vec![target_ix.into()], // instructions
            trigger,                // trigger
        )?;

        Ok(())
    }

    pub fn trade_wood_for_gold(ctx: Context<GameAction>) -> Result<()> {
        let game_data_account = &mut ctx.accounts.game_data;
        if game_data_account.wood < WOOD_PER_SELL {
            return err!(GameErrorCode::NotEnoughWood);
        }
        game_data_account.wood = game_data_account.wood.checked_sub(WOOD_PER_SELL).unwrap();

        // Obviously better teeth increase wood quality, so you get more gold for your wood.
        let gold_per_wood: u64 =
            GOLD_PER_WOOD_BASE + (game_data_account.teeth_upgrade * GOLD_PER_WOOD_TEETH_MULTIPLIER);
        game_data_account.gold = game_data_account.gold.checked_add(gold_per_wood).unwrap();
        Ok(())
    }

    pub fn upgrade_teeth(ctx: Context<GameAction>) -> Result<()> {
        let game_data_account = &mut ctx.accounts.game_data;
        let upgrade_cost: u64 = TEETH_UPGRADE_BASE_COST
            + TEETH_UPGRADE_COST_MULTIPLIER * game_data_account.teeth_upgrade;

        if game_data_account.gold < upgrade_cost {
            return err!(GameErrorCode::NotEnoughGold);
        }
        game_data_account.teeth_upgrade = game_data_account.teeth_upgrade.checked_add(1).unwrap();
        game_data_account.gold = game_data_account.gold.checked_sub(upgrade_cost).unwrap();
        Ok(())
    }

    pub fn buy_lumberjack(ctx: Context<GameAction>) -> Result<()> {
        let game_data_account = &mut ctx.accounts.game_data;
        let lumber_jack_cost: u64 =
            LUMBERJACK_BASE_COST + LUMBERJACK_COST_MULTIPLIER * game_data_account.lumberjacks;

        if game_data_account.gold < lumber_jack_cost {
            return err!(GameErrorCode::NotEnoughGold);
        }

        game_data_account.gold = game_data_account
            .gold
            .checked_sub(lumber_jack_cost)
            .unwrap();
        game_data_account.lumberjacks = game_data_account.lumberjacks.checked_add(1).unwrap();

        Ok(())
    }

    pub fn on_thread_tick(ctx: Context<ThreadTick>) -> Result<()> {
        let game_data = &mut ctx.accounts.game_data;
        game_data.wood = game_data.wood.checked_add(game_data.lumberjacks).unwrap();
        game_data.updated_at = Clock::get().unwrap().unix_timestamp;
        msg!(
            "Wood: {}, updated_at: {}",
            game_data.wood,
            game_data.updated_at
        );
        Ok(())
    }

    pub fn reset(ctx: Context<Reset>) -> Result<()> {
        // Get accounts
        let clockwork_program = &ctx.accounts.clockwork_program;
        let payer = &ctx.accounts.payer;
        let thread = &ctx.accounts.thread;
        let thread_authority = &ctx.accounts.thread_authority;

        // Delete thread via CPI.
        let bump = *ctx.bumps.get("thread_authority").unwrap();
        clockwork_sdk::cpi::thread_delete(CpiContext::new_with_signer(
            clockwork_program.to_account_info(),
            clockwork_sdk::cpi::ThreadDelete {
                authority: thread_authority.to_account_info(),
                close_to: payer.to_account_info(),
                thread: thread.to_account_info(),
            },
            &[&[THREAD_AUTHORITY_SEED,payer.key().as_ref(), &[bump]]],
        ))?;
        Ok(())
    }
}

#[derive(Accounts)]
pub struct ThreadTick<'info> {
    /// The Game Data account account.
    #[account(mut, seeds = [SEED_GAME_DATA, game_data.authority.key().as_ref()], bump)]
    pub game_data: Account<'info, GameData>,

    /// Verify that only this thread can execute the ThreadTick Instruction
    #[account(signer, constraint = thread.authority.eq(&thread_authority.key()))]
    pub thread: Account<'info, Thread>,

    /// The Thread Admin
    /// The authority that was used as a seed to derive the thread address
    /// `thread_authority` should equal `thread.thread_authority`
    #[account(seeds = [THREAD_AUTHORITY_SEED, game_data.authority.key().as_ref()], bump)]
    pub thread_authority: SystemAccount<'info>,
}

#[derive(Accounts)]
pub struct GameAction<'info> {
    /// The Game Data account account.
    /// Seeds: gameData + signerPublicKey
    #[account(mut, seeds = [SEED_GAME_DATA, signer.key().as_ref()], bump)]
    pub game_data: Account<'info, GameData>,

    pub signer: Signer<'info>,
}

#[derive(Accounts)]
#[instruction(thread_id: Vec<u8>)]
pub struct Initialize<'info> {
    #[account(
        init,
        payer = payer,
        seeds = [SEED_GAME_DATA, payer.key().as_ref()],
        bump,
        space = 8 + std::mem::size_of::< GameData > (),
    )]
    pub game_data: Account<'info, GameData>,

    /// The Clockwork thread program.
    #[account(address = clockwork_sdk::ID)]
    pub clockwork_program: Program<'info, clockwork_sdk::ThreadProgram>,

    /// The signer who will pay to initialize the program.
    /// (not to be confused with the thread executions).
    #[account(mut)]
    pub payer: Signer<'info>,

    #[account(address = system_program::ID)]
    pub system_program: Program<'info, System>,

    /// Address to assign to the newly created thread.
    #[account(mut, address = Thread::pubkey(thread_authority.key(), thread_id))]
    pub thread: SystemAccount<'info>,

    /// The pda that will own and manage the thread.
    #[account(seeds = [THREAD_AUTHORITY_SEED, payer.key().as_ref()], bump)]
    pub thread_authority: SystemAccount<'info>,
}

#[derive(Accounts)]
pub struct Reset<'info> {
    #[account(mut)]
    pub payer: Signer<'info>,

    /// The Clockwork thread program.
    #[account(address = clockwork_sdk::ID)]
    pub clockwork_program: Program<'info, clockwork_sdk::ThreadProgram>,

    /// The thread to reset.
    #[account(mut, address = thread.pubkey(), constraint = thread.authority.eq(&thread_authority.key()))]
    pub thread: Account<'info, Thread>,

    /// The pda that owns and manages the thread.
    #[account(seeds = [THREAD_AUTHORITY_SEED, game_data.authority.key().as_ref()], bump)]
    pub thread_authority: SystemAccount<'info>,

    /// Close the gameData account
    #[account(
        mut,
        seeds = [SEED_GAME_DATA, game_data.authority.key().as_ref()],
        bump,
        close = payer
    )]
    pub game_data: Account<'info, GameData>,
}
