//! Arbitrage bot between two swap programs!
mod arb;
mod error;
mod partial_state;
mod processor;
mod swap;
mod util;

use borsh::{BorshDeserialize, BorshSerialize};
use solana_program::{
    account_info::AccountInfo, entrypoint, entrypoint::ProgramResult, program_error::ProgramError,
    pubkey::Pubkey,
};

/// The program's instructions
/// This program only takes one instruction
#[derive(BorshSerialize, BorshDeserialize, Debug)]
pub enum ArbitrageProgramInstruction {
    TryArbitrage {
        /// The program ID of the first swap we want to inspect for arbitrage
        swap_1_program_id: Pubkey,
        /// The program ID of the second swap we want to inspect for arbitrage
        swap_2_program_id: Pubkey,
        /// How many assets we are going to evaluate combinations of at one time
        concurrency: u8,
        /// How aggressive the model will be when identifying arbitrage
        /// opportunities
        ///
        /// More specifically, a higher temperature will
        /// mean a smaller percent difference in swap values will trigger a
        /// trade
        temperature: u8,
    },
}

entrypoint!(process);

/// Processor
fn process(_program_id: &Pubkey, accounts: &[AccountInfo], data: &[u8]) -> ProgramResult {
    match ArbitrageProgramInstruction::try_from_slice(data) {
        Ok(ix) => match ix {
            ArbitrageProgramInstruction::TryArbitrage {
                swap_1_program_id,
                swap_2_program_id,
                concurrency,
                temperature,
            } => processor::process_arbitrage(
                accounts,
                &swap_1_program_id,
                &swap_2_program_id,
                concurrency,
                temperature,
            ),
        },
        Err(_) => Err(ProgramError::InvalidInstructionData),
    }
}
