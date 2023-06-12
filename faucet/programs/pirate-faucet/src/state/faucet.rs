//! Swap program account state
use anchor_lang::{prelude::*, system_program};
use anchor_spl::token::{transfer, Mint, Token, TokenAccount, Transfer};

use crate::error::FaucetError;

use super::{Ledger, LedgerAccount};

/// The `Faucet` state - the inner data of the program-derived address
/// that will be our faucet
#[account]
pub struct Faucet {
    pub assets: Vec<Pubkey>,
    pub bump: u8,
}

impl Faucet {
    /// The faucet's seed prefix, and in this case the only seed used to
    /// derive it's program-derived address
    pub const SEED_PREFIX: &'static str = "faucet";

    /// Anchor discriminator + Vec (empty) + u8
    pub const SPACE: usize = 8 + 4 + 1;

    /// Creates a new `Faucet1 state
    pub fn new(bump: u8) -> Self {
        Self {
            assets: vec![],
            bump,
        }
    }
}

/// Trait used to wrap functionality for the faucet that can be called
/// on the faucet account as it's pulled from an Anchor Context, ie.
/// `Account<'_, Faucet>`
pub trait FaucetAccount<'info> {
    fn check_asset_key(&self, key: &Pubkey) -> Result<()>;
    fn add_asset(
        &mut self,
        key: Pubkey,
        payer: &Signer<'info>,
        system_program: &Program<'info, System>,
    ) -> Result<()>;
    fn realloc(
        &mut self,
        space_to_add: usize,
        payer: &Signer<'info>,
        system_program: &Program<'info, System>,
    ) -> Result<()>;
    fn fund(
        &mut self,
        deposit: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
            u64,
        ),
        authority: &Signer<'info>,
        system_program: &Program<'info, System>,
        token_program: &Program<'info, Token>,
    ) -> Result<()>;
    fn process_airdrop(
        &mut self,
        receive: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
            u64,
        ),
        ledger: &mut Account<'info, Ledger>,
        token_program: &Program<'info, Token>,
    ) -> Result<()>;
}

impl<'info> FaucetAccount<'info> for Account<'info, Faucet> {
    /// Validates an asset's key is present in the faucet's list of mint
    /// addresses, and throws an error if it is not
    fn check_asset_key(&self, key: &Pubkey) -> Result<()> {
        if self.assets.contains(key) {
            Ok(())
        } else {
            Err(FaucetError::InvalidAssetKey.into())
        }
    }

    /// Adds an asset to the faucet's list of mint addresses if it does
    /// not already exist in the list
    ///
    /// if the mint address is added, this will require reallocation of the
    /// account's size since the vector will be increasing by one `Pubkey`,
    /// which has a size of 32 bytes
    fn add_asset(
        &mut self,
        key: Pubkey,
        payer: &Signer<'info>,
        system_program: &Program<'info, System>,
    ) -> Result<()> {
        match self.check_asset_key(&key) {
            Ok(()) => (),
            Err(_) => {
                self.realloc(32, payer, system_program)?;
                self.assets.push(key)
            }
        };
        Ok(())
    }

    /// Reallocates the account's size to accommodate for changes in the data
    /// size. This is used in this program to reallocate the faucet's
    /// account when it's vector of mint addresses (`Vec<Pubkey>`) is increased
    /// in size by pushing one `Pubkey` into the vector
    fn realloc(
        &mut self,
        space_to_add: usize,
        payer: &Signer<'info>,
        system_program: &Program<'info, System>,
    ) -> Result<()> {
        let account_info = self.to_account_info();
        let new_account_size = account_info.data_len() + space_to_add;

        // Determine additional rent required
        let lamports_required = (Rent::get()?).minimum_balance(new_account_size);
        let additional_rent_to_fund = lamports_required - account_info.lamports();

        // Perform transfer of additional rent
        system_program::transfer(
            CpiContext::new(
                system_program.to_account_info(),
                system_program::Transfer {
                    from: payer.to_account_info(),
                    to: account_info.clone(),
                },
            ),
            additional_rent_to_fund,
        )?;

        // Reallocate the account
        account_info.realloc(new_account_size, false)?;
        Ok(())
    }

    /// Funds the faucet by transferring assets from the payer's - or
    /// Provider's - token account to the faucet's token
    /// account
    ///
    /// In this function, the program is also going to add the mint address to
    /// the list of mint addresses stored in the `Faucet` data, if it
    /// does not exist, and reallocate the account's size
    fn fund(
        &mut self,
        deposit: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
            u64,
        ),
        authority: &Signer<'info>,
        system_program: &Program<'info, System>,
        token_program: &Program<'info, Token>,
    ) -> Result<()> {
        let (mint, from, to, amount) = deposit;
        self.add_asset(mint.key(), authority, system_program)?;
        process_transfer_to_faucet(from, to, amount, authority, token_program)?;
        Ok(())
    }

    /// Processes the swap for the proposed assets
    ///
    /// This function will make sure both requested assets - the one the user is
    /// proposing to pay and the one the user is requesting to receive in
    /// exchange - are present in the `Faucet` data's list of supported
    /// mint addresses
    ///
    /// It will then calculate the amount of the requested "receive" assets
    /// based on the user's proposed amount of asset to pay, using the
    /// constant-product algorithm `r = f(p)`
    ///
    /// Once calculated, it will process both transfers
    fn process_airdrop(
        &mut self,
        receive: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
            u64,
        ),
        ledger: &mut Account<'info, Ledger>,
        token_program: &Program<'info, Token>,
    ) -> Result<()> {
        let (mint, faucet_token_account, payer_token_account, amount) = receive;
        self.check_asset_key(&mint.key())?;
        // Determine if this person has received this token from the faucet before
        ledger.determine_airdrop_eligible(mint, amount)?;
        // Process the airdrop
        process_transfer_from_faucet(
            faucet_token_account,
            payer_token_account,
            amount,
            self,
            token_program,
        )
    }
}

/// Process a transfer from one the payer's token account to the
/// faucet's token account using a CPI
fn process_transfer_to_faucet<'info>(
    from: &Account<'info, TokenAccount>,
    to: &Account<'info, TokenAccount>,
    amount: u64,
    authority: &Signer<'info>,
    token_program: &Program<'info, Token>,
) -> Result<()> {
    transfer(
        CpiContext::new(
            token_program.to_account_info(),
            Transfer {
                from: from.to_account_info(),
                to: to.to_account_info(),
                authority: authority.to_account_info(),
            },
        ),
        amount,
    )
}

/// Process a transfer from the faucet's token account to the
/// payer's token account using a CPI with signer seeds
fn process_transfer_from_faucet<'info>(
    from: &Account<'info, TokenAccount>,
    to: &Account<'info, TokenAccount>,
    amount: u64,
    faucet: &Account<'info, Faucet>,
    token_program: &Program<'info, Token>,
) -> Result<()> {
    transfer(
        CpiContext::new_with_signer(
            token_program.to_account_info(),
            Transfer {
                from: from.to_account_info(),
                to: to.to_account_info(),
                authority: faucet.to_account_info(),
            },
            &[&[Faucet::SEED_PREFIX.as_bytes(), &[faucet.bump]]],
        ),
        amount,
    )
}
