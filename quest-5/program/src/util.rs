//! Util functions for arbitrage bot
use solana_program::{
    account_info::AccountInfo, entrypoint::ProgramResult, instruction::AccountMeta,
    program_error::ProgramError, pubkey::Pubkey,
};

use crate::error::ArbitrageProgramError;

// Asserts the pool address provided is in fact derived from the program ID
// provided
pub fn check_pool_address(program_id: &Pubkey, pool: &Pubkey) -> ProgramResult {
    if !Pubkey::find_program_address(&[b"liquidity_pool"], program_id)
        .0
        .eq(pool)
    {
        return Err(solana_program::program_error::ProgramError::InvalidInstructionData);
    }
    Ok(())
}

/// Trait used to unpack `Option<T>` values for smoother algorithm code
pub trait ArbitrageEvaluateOption<T> {
    fn ok_or_arb_err(self) -> Result<T, ProgramError>;
}

impl<T> ArbitrageEvaluateOption<T> for Option<T> {
    fn ok_or_arb_err(self) -> Result<T, ProgramError> {
        self.ok_or(ArbitrageProgramError::InvalidAccountsList.into())
    }
}

/// Trait used to convert from an `AccountInfo` to an `AccountMeta`
pub trait ToAccountMeta {
    fn to_account_meta(&self) -> AccountMeta;
}

impl ToAccountMeta for AccountInfo<'_> {
    fn to_account_meta(&self) -> AccountMeta {
        AccountMeta {
            pubkey: *self.key,
            is_signer: self.is_signer,
            is_writable: self.is_writable,
        }
    }
}
