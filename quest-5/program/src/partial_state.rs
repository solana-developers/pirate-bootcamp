//! Bytemuck-powered zero-copy partial deserialization
use bytemuck::{Pod, Zeroable};
use solana_program::{account_info::AccountInfo, msg, program_error::ProgramError, pubkey::Pubkey};
use spl_token_2022::pod::OptionalNonZeroPubkey;

use crate::error::ArbitrageProgramError;

/// The first three fields of the `spl_token::state::Account`
#[repr(C)]
#[derive(Clone, Copy, Debug, Pod, Zeroable)]
pub struct PartialTokenAccountState {
    pub mint: Pubkey,
    pub owner: Pubkey,
    pub amount: u64,
}

/// Custom return type for our arbitrage algorithm:
/// (account, mint, owner, amount)
pub type ArbitrageTokenAccountInfo<'a, 'b> = (&'a AccountInfo<'b>, Pubkey, Pubkey, u64);

impl PartialTokenAccountState {
    /// Attempts to use zero-copy deserialization via Bytemuck to determine if
    /// this account is in fact an associated token account
    ///
    /// If it is, it will also validate the token account's owner against the
    /// provided address, and finally return only the vital information we need
    /// for the rest of the arbitrage program
    pub fn try_deserialize<'a, 'b>(
        account_info: &'a AccountInfo<'b>,
        owner: &'a Pubkey,
    ) -> Result<ArbitrageTokenAccountInfo<'a, 'b>, ProgramError> {
        // Check that the account has enough data to try to deserialize
        if account_info.data_len() < 72 {
            msg!(
                "Data too small. Should be 72. Found len: {}",
                account_info.data_len()
            );
            msg!("Token Account: {}", account_info.key);
            return Err(ArbitrageProgramError::InvalidAccountsList.into());
        }
        // Try to partially deserialize the account data
        match bytemuck::try_from_bytes::<Self>(&account_info.data.borrow()[..72]) {
            // Validate the owner
            Ok(partial_token) => {
                if !partial_token.owner.eq(owner) {
                    msg!("Owner mismatch");
                    msg!("Expected: {}", owner);
                    msg!("Got:      {}", partial_token.owner);
                    msg!("Token Account: {}", account_info.key);
                    return Err(ArbitrageProgramError::InvalidAccountsList.into());
                }
                // Return the vital information
                Ok((
                    account_info,
                    partial_token.mint,
                    partial_token.owner,
                    partial_token.amount,
                ))
            }
            Err(_) => {
                msg!("Failed to deserialize associated token account");
                msg!("Token Account: {}", account_info.key);
                Err(ArbitrageProgramError::InvalidAccountsList.into())
            }
        }
    }
}

/// The first two fields of the `spl_token::state::Mint`
///
/// The third field `decimals` - which is the one we are interested in - cannot
/// be included in this struct since Bytemuck will not allow two integer types
/// of varying size - such as `u64` and `u8`
///
/// However, since `decimals` is a single byte (`u8`), we can simply take the
/// next byte if the data deserializes into this struct properly
#[repr(C)]
#[derive(Clone, Copy, Debug, Pod, Zeroable)]
pub struct PartialMintState {
    pub mint_authority: OptionalNonZeroPubkey,
    pub supply: u64,
}

/// Custom return type for our arbitrage algorithm:
/// (account, decimals)
pub type ArbitrageMintInfo<'a, 'b> = (&'a AccountInfo<'b>, u8);

impl PartialMintState {
    /// Attempts to use zero-copy deserialization via Bytemuck to determine if
    /// this account is in fact a mint account
    ///
    /// If it is, it will return only the vital information we need
    /// for the rest of the arbitrage program
    pub fn try_deserialize<'a, 'b>(
        account_info: &'a AccountInfo<'b>,
    ) -> Result<ArbitrageMintInfo<'a, 'b>, ProgramError> {
        // Check that the account has enough data to try to deserialize
        if account_info.data_len() < 41 {
            msg!(
                "Data too small. Should be 41. Found len: {}",
                account_info.data_len()
            );
            msg!("Mint: {}", account_info.key);
            return Err(ArbitrageProgramError::InvalidAccountsList.into());
        }
        let data = &account_info.data.borrow()[..41];
        // Try to partially deserialize the account data
        match bytemuck::try_from_bytes::<Self>(&data[..40]) {
            Ok(_) => {
                let decimals = match data.get(40) {
                    Some(d) => d,
                    None => {
                        msg!("Could not get decimals");
                        msg!("Mint: {}", account_info.key);
                        return Err(ArbitrageProgramError::InvalidAccountsList.into());
                    }
                };
                // Return the vital information
                Ok((account_info, *decimals))
            }
            Err(_) => {
                msg!("Failed to deserialize mint account");
                msg!("Mint: {}", account_info.key);
                Err(ArbitrageProgramError::InvalidAccountsList.into())
            }
        }
    }
}
