//! Swap program account state
use anchor_lang::{prelude::*, system_program};
use anchor_spl::token::{transfer, Mint, Token, TokenAccount, Transfer};
use std::ops::{Add, Div, Mul};

use crate::error::SwapProgramError;

/// The `LiquidityPool` state - the inner data of the program-derived address
/// that will be our Liquidity Pool
#[account]
pub struct LiquidityPool {
    pub assets: Vec<Pubkey>,
    pub bump: u8,
}

impl LiquidityPool {
    /// The Liquidity Pool's seed prefix, and in this case the only seed used to
    /// derive it's program-derived address
    pub const SEED_PREFIX: &'static str = "liquidity_pool";

    /// Anchor discriminator + Vec (empty) + u8
    pub const SPACE: usize = 8 + 4 + 1;

    /// Creates a new `LiquidityPool1 state
    pub fn new(bump: u8) -> Self {
        Self {
            assets: vec![],
            bump,
        }
    }
}

/// Trait used to wrap functionality for the Liquidity Pool that can be called
/// on the Liquidity Pool account as it's pulled from an Anchor Context, ie.
/// `Account<'_, LiquidityPool>`
pub trait LiquidityPoolAccount<'info> {
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
    fn process_swap(
        &mut self,
        receive: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
        ),
        pay: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
            u64,
        ),
        authority: &Signer<'info>,
        token_program: &Program<'info, Token>,
    ) -> Result<()>;
}

impl<'info> LiquidityPoolAccount<'info> for Account<'info, LiquidityPool> {
    /// Validates an asset's key is present in the Liquidity Pool's list of mint
    /// addresses, and throws an error if it is not
    fn check_asset_key(&self, key: &Pubkey) -> Result<()> {
        if self.assets.contains(key) {
            Ok(())
        } else {
            Err(SwapProgramError::InvalidAssetKey.into())
        }
    }

    /// Adds an asset to the Liquidity Pool's list of mint addresses if it does
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
    /// size. This is used in this program to reallocate the Liquidity Pool's
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

    /// Funds the Liquidity Pool by transferring assets from the payer's - or
    /// Liquidity Provider's - token account to the Liquidity Pool's token
    /// account
    ///
    /// In this function, the program is also going to add the mint address to
    /// the list of mint addresses stored in the `LiquidityPool` data, if it
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
        process_transfer_to_pool(from, to, amount, authority, token_program)?;
        Ok(())
    }

    /// Processes the swap for the proposed assets
    ///
    /// This function will make sure both requested assets - the one the user is
    /// proposing to pay and the one the user is requesting to receive in
    /// exchange - are present in the `LiquidityPool` data's list of supported
    /// mint addresses
    ///
    /// It will then calculate the amount of the requested "receive" assets
    /// based on the user's proposed amount of asset to pay, using the
    /// constant-product algorithm `r = f(p)`
    ///
    /// Once calculated, it will process both transfers
    fn process_swap(
        &mut self,
        receive: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
        ),
        pay: (
            &Account<'info, Mint>,
            &Account<'info, TokenAccount>,
            &Account<'info, TokenAccount>,
            u64,
        ),
        authority: &Signer<'info>,
        token_program: &Program<'info, Token>,
    ) -> Result<()> {
        // (From, To)
        let (receive_mint, pool_recieve, payer_recieve) = receive;
        self.check_asset_key(&receive_mint.key())?;
        // (From, To)
        let (pay_mint, payer_pay, pool_pay, pay_amount) = pay;
        self.check_asset_key(&pay_mint.key())?;
        // Determine the amount the payer will recieve of the requested asset
        let receive_amount = determine_swap_receive(
            pool_recieve.amount,
            receive_mint.decimals,
            pool_pay.amount,
            pay_mint.decimals,
            pay_amount,
        )?;
        // Process the swap
        if receive_amount == 0 {
            Err(SwapProgramError::InvalidSwapNotEnoughPay.into())
        } else {
            process_transfer_to_pool(payer_pay, pool_pay, pay_amount, authority, token_program)?;
            process_transfer_from_pool(
                pool_recieve,
                payer_recieve,
                receive_amount,
                self,
                token_program,
            )?;
            Ok(())
        }
    }
}

/// Process a transfer from one the payer's token account to the
/// pool's token account using a CPI
fn process_transfer_to_pool<'info>(
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

/// Process a transfer from the pool's token account to the
/// payer's token account using a CPI with signer seeds
fn process_transfer_from_pool<'info>(
    from: &Account<'info, TokenAccount>,
    to: &Account<'info, TokenAccount>,
    amount: u64,
    pool: &Account<'info, LiquidityPool>,
    token_program: &Program<'info, Token>,
) -> Result<()> {
    transfer(
        CpiContext::new_with_signer(
            token_program.to_account_info(),
            Transfer {
                from: from.to_account_info(),
                to: to.to_account_info(),
                authority: pool.to_account_info(),
            },
            &[&[LiquidityPool::SEED_PREFIX.as_bytes(), &[pool.bump]]],
        ),
        amount,
    )
}

/// The constant-product algorithm `f(p)` to determine the allowed amount of the
/// receiving asset that can be returned in exchange for the amount of the paid
/// asset offered
///
/// ```
/// K = a * b * c * d * P * R
/// K = a * b * c * d * (P + p) * (R - r)
///
/// a * b * c * d * P * R = a * b * c * d * (P + p) * (R - r)
/// PR = (P + p) * (R - r)
/// PR = PR - Pr + Rp - pr
/// 0 = 0 - Pr + Rp - pr
/// -Rp = -Pr - pr
/// -Rp = r(-P - p)
/// r = (-Rp) / (-P - p)
/// r = [-1 * Rp] / [-1 * (P + p)]
/// r = Rp / (P + p)
///
/// r = f(p) = (R * p) / (P + p)
/// ```
fn determine_swap_receive(
    pool_recieve_balance: u64,
    receive_decimals: u8,
    pool_pay_balance: u64,
    pay_decimals: u8,
    pay_amount: u64,
) -> Result<u64> {
    // Convert all values to nominal floats using their respective mint decimal
    // places
    let big_r = convert_to_float(pool_recieve_balance, receive_decimals);
    let big_p = convert_to_float(pool_pay_balance, pay_decimals);
    let p = convert_to_float(pay_amount, pay_decimals);
    // Calculate `f(p)` to get `r`
    let bigr_times_p = big_r.mul(p);
    let bigp_plus_p = big_p.add(p);
    let r = bigr_times_p.div(bigp_plus_p);
    // Make sure `r` does not exceed liquidity
    if r > big_r {
        return Err(SwapProgramError::InvalidSwapNotEnoughLiquidity.into());
    }
    // Return the real value of `r`
    Ok(convert_from_float(r, receive_decimals))
}

/// Converts a `u64` value - in this case the balance of a token account - into
/// an `f32` by using the `decimals` value of its associated mint to get the
/// nominal quantity of a mint stored in that token account
///
/// For example, a token account with a balance of 10,500 for a mint with 3
/// decimals would have a nominal balance of 10.5
fn convert_to_float(value: u64, decimals: u8) -> f32 {
    (value as f32).div(f32::powf(10.0, decimals as f32))
}

/// Converts a nominal value - in this case the calculated value `r` - into a
/// `u64` by using the `decimals` value of its associated mint to get the real
/// quantity of the mint that the user will receive
///
/// For example, if `r` is calculated to be 10.5, the real amount of the asset
/// to be received by the user is 10,500
fn convert_from_float(value: f32, decimals: u8) -> u64 {
    value.mul(f32::powf(10.0, decimals as f32)) as u64
}
