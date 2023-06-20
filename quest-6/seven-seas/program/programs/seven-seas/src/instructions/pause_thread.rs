//! Instruction: pause_thread
use anchor_lang::prelude::*;
use crate::{THREAD_AUTHORITY_SEED};
use clockwork_sdk::state::{Thread};

pub fn pause_thread(ctx: Context<PauseThread>, _thread_id: Vec<u8>) -> Result<()> {
    let clockwork_program = &ctx.accounts.clockwork_program;
    let thread = &ctx.accounts.thread;
    let thread_authority = &ctx.accounts.thread_authority;

    // Pause Thread
    let bump = *ctx.bumps.get("thread_authority").unwrap();
    clockwork_sdk::cpi::thread_pause(
        CpiContext::new_with_signer(
            clockwork_program.to_account_info(),
            clockwork_sdk::cpi::ThreadPause {
                thread: thread.to_account_info(),
                authority: thread_authority.to_account_info(),
            },
            &[&[THREAD_AUTHORITY_SEED, &[bump]]],
        )
    )?;

    Ok(())
}

#[derive(Accounts)]
#[instruction(thread_id: Vec<u8>)]
pub struct PauseThread<'info> {
    #[account(mut)]
    pub payer: Signer<'info>,

    /// The Clockwork thread program.
    #[account(address = clockwork_sdk::ID)]
    pub clockwork_program: Program<'info, clockwork_sdk::ThreadProgram>,

    /// Address to assign to the newly created thread.
    /// CHECK: is this the correct account type?
    #[account(mut, address = Thread::pubkey(thread_authority.key(), thread_id))]
    pub thread: AccountInfo<'info>,

    /// The pda that will own and manage the thread.
    #[account(seeds = [THREAD_AUTHORITY_SEED], bump)]
    pub thread_authority: SystemAccount<'info>,
}
