//! Swap program errors
use anchor_lang::prelude::*;

#[error_code]
pub enum FaucetError {
    /// Some arithmetic operation has caused the resulting number to be too
    /// large for a `u64` value, thus overflowing it
    #[msg("Math overflow on `u64` value")]
    InvalidArithmetic,

    /// The mint address of the asset provided does not match any mint addresses
    /// stored in the `LiquidityPool` state, thus that asset has not been
    /// provided as liquidity to the pool and cannot be swapped
    #[msg("An invalid asset mint address was provided")]
    InvalidAssetKey,

    /// The amount of the "pay" asset that a user has proposed to pay results,
    /// after calculation of the function `r = f(p)`, in a value for `r` that is
    /// less than 0, thus no assets will be moved and this swap will be rejected
    #[msg("The amount proposed to pay is not great enough for at least 1 returned asset quantity")]
    InvalidSwapNotEnoughPay,

    /// The user's proposed pay amount resolves to a value for `r` that exceeds
    /// the balance of the pool's token account for the receive asset
    #[msg("The amount proposed to pay resolves to a receive amount that is greater than the current liquidity")]
    InvalidSwapNotEnoughLiquidity,

    /// A user has requested a swap "X for X" - where both values for "X" are
    /// the same asset mint, which is not allowed
    #[msg("The asset proposed to pay is the same asset as the requested asset to receive")]
    InvalidSwapMatchingAssets,

    /// The user proposed to pay 0 of an asset
    #[msg("A user cannot propose to pay 0 of an asset")]
    InvalidSwapZeroAmount,

    /// Maximum air drop amount exceeded
    #[msg("Maximum air drop amount exceeded")]
    MaxAmountExceeded,
}
