//! Processes an attempt to arbitrage trade
use solana_program::{
    account_info::{next_account_info, AccountInfo},
    entrypoint::ProgramResult,
    pubkey::Pubkey,
};

use crate::arb::{try_arbitrage, TryArbitrageArgs};
use crate::partial_state::{PartialMintState, PartialTokenAccountState};
use crate::util::check_pool_address;

/// Processes program inputs to search for an arbitrage opportunity between two
/// swap programs
///
/// Note: accounts must be provided in a very specific order:
/// * Payer
/// * Token Program
/// * Swap #1 Liquidity Pool
/// * Swap #2 Liquidity Pool
/// * [Token Accounts for User]
/// * [Token Accounts for Swap #1]
/// * [Token Accounts for Swap #2]
/// * [Mint Accounts]
pub fn process_arbitrage(
    accounts: &[AccountInfo],
    swap_1_program_id: &Pubkey,
    swap_2_program_id: &Pubkey,
    concurrency: u8,
    temperature: u8,
) -> ProgramResult {
    // Load the first few "fixed" accounts provided
    let accounts_iter = &mut accounts.iter();
    let payer = next_account_info(accounts_iter)?;
    let token_program = next_account_info(accounts_iter)?;
    let system_program = next_account_info(accounts_iter)?;
    let associated_token_program = next_account_info(accounts_iter)?;
    let swap_1_program = next_account_info(accounts_iter)?;
    let swap_2_program = next_account_info(accounts_iter)?;
    let swap_1_pool = next_account_info(accounts_iter)?;
    let swap_2_pool = next_account_info(accounts_iter)?;

    // Ensure each pool address follows the correct derivation from its
    // corresponding program ID
    check_pool_address(swap_1_program_id, swap_1_pool.key)?;
    check_pool_address(swap_2_program_id, swap_2_pool.key)?;

    // Read the provided user's token accounts
    let token_accounts_user = {
        let mut accts = vec![];
        for _x in 0..concurrency {
            accts.push(PartialTokenAccountState::try_deserialize(
                next_account_info(accounts_iter)?,
                payer.key,
            )?);
        }
        accts
    };

    // Read the provided token accounts for Swap Program #1
    let token_accounts_swap_1 = {
        let mut accts = vec![];
        for _x in 0..concurrency {
            accts.push(PartialTokenAccountState::try_deserialize(
                next_account_info(accounts_iter)?,
                swap_1_pool.key,
            )?);
        }
        accts
    };

    // Read the provided token accounts for Swap Program #2
    let token_accounts_swap_2 = {
        let mut accts = vec![];
        for _x in 0..concurrency {
            accts.push(PartialTokenAccountState::try_deserialize(
                next_account_info(accounts_iter)?,
                swap_2_pool.key,
            )?);
        }
        accts
    };

    // Read the provided mint accounts for the assets to evaluate all combinations
    let mints = {
        let mut accts = vec![];
        for _x in 0..concurrency {
            accts.push(PartialMintState::try_deserialize(next_account_info(
                accounts_iter,
            )?)?);
        }
        accts
    };

    // Check if there is an arbitrage opportunity between the two pools, and
    // execute the trade if there is one
    try_arbitrage(TryArbitrageArgs {
        token_accounts_user,
        token_accounts_swap_1,
        token_accounts_swap_2,
        mints,
        payer,
        token_program,
        system_program,
        associated_token_program,
        swap_1_program,
        swap_2_program,
        swap_1_pool,
        swap_2_pool,
        temperature,
    })
}
