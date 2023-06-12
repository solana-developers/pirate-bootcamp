//! Swap functions copied from the Swap program
use solana_program::program_error::ProgramError;
use std::ops::{Add, Div, Mul};

use crate::error::ArbitrageProgramError;

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
pub fn determine_swap_receive(
    pool_recieve_balance: u64,
    receive_decimals: u8,
    pool_pay_balance: u64,
    pay_decimals: u8,
    pay_amount: u64,
) -> Result<u64, ProgramError> {
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
        return Err(ArbitrageProgramError::InvalidSwapNotEnoughLiquidity.into());
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
