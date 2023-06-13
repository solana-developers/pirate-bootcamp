//! Arbitrage bot errors
#[derive(Clone, Debug, Eq, thiserror::Error, num_derive::FromPrimitive, PartialEq)]
pub enum ArbitrageProgramError {
    /// Invalid list of accounts: Each list of accounts should be the same
    /// length and passed in the following order: user token accounts, swap 1
    /// token accounts, swap 2 token accounts, mints
    #[error("Invalid list of accounts: Each list of accounts should be the same length and passed in the following order: user token accounts, swap 1 token accounts, swap 2 token accounts, mints")]
    InvalidAccountsList,
    /// A token account not belonging to the user, swap #1's Liquidity Pool, or
    /// swap #2's Liquidity Pool was passed into the program
    #[error("A token account not belonging to the user, swap #1's Liquidity Pool, or swap #2's Liquidity Pool was passed into the program")]
    TokenAccountOwnerNotFound,
    /// The user's proposed pay amount resolves to a value for `r` that exceeds
    /// the balance of the pool's token account for the receive asset
    #[error("The amount proposed to pay resolves to a receive amount that is greater than the current liquidity")]
    InvalidSwapNotEnoughLiquidity,
    /// No arbitrage opportunity was detected, so the program will return an
    /// error so that preflight fails
    #[error("No arbitrage opportunity detected")]
    NoArbitrage,
}

impl From<ArbitrageProgramError> for solana_program::program_error::ProgramError {
    fn from(e: ArbitrageProgramError) -> Self {
        solana_program::program_error::ProgramError::Custom(e as u32)
    }
}
impl<T> solana_program::decode_error::DecodeError<T> for ArbitrageProgramError {
    fn type_of() -> &'static str {
        "ArbitrageProgramError"
    }
}

impl solana_program::program_error::PrintProgramError for ArbitrageProgramError {
    fn print<E>(&self)
    where
        E: 'static
            + std::error::Error
            + solana_program::decode_error::DecodeError<E>
            + solana_program::program_error::PrintProgramError
            + num_traits::FromPrimitive,
    {
        match self {
            ArbitrageProgramError::InvalidAccountsList => {
                solana_program::msg!("Invalid list of accounts: Each list of accounts should be the same length and passed in the following order: user token accounts, swap 1 token accounts, swap 2 token accounts, mints")
            }
            ArbitrageProgramError::TokenAccountOwnerNotFound => {
                solana_program::msg!("A token account not belonging to the user, swap #1's Liquidity Pool, or swap #2's Liquidity Pool was passed into the program")
            }
            ArbitrageProgramError::InvalidSwapNotEnoughLiquidity => {
                solana_program::msg!("The amount proposed to pay resolves to a receive amount that is greater than the current liquidity")
            }
            ArbitrageProgramError::NoArbitrage => {
                solana_program::msg!("No arbitrage opportunity detected")
            }
        }
    }
}
