//! Arbitrage opportunity spotting and trade placement
use solana_program::{
    account_info::AccountInfo, entrypoint::ProgramResult, instruction::Instruction, msg,
    program::invoke, pubkey::Pubkey,
};

use crate::{
    error::ArbitrageProgramError,
    partial_state::{ArbitrageMintInfo, ArbitrageTokenAccountInfo},
    swap::determine_swap_receive,
    util::{ArbitrageEvaluateOption, ToAccountMeta},
};

/// Args for the `try_arbitrage` algorithm
pub struct TryArbitrageArgs<'a, 'b> {
    pub token_accounts_user: Vec<ArbitrageTokenAccountInfo<'a, 'b>>,
    pub token_accounts_swap_1: Vec<ArbitrageTokenAccountInfo<'a, 'b>>,
    pub token_accounts_swap_2: Vec<ArbitrageTokenAccountInfo<'a, 'b>>,
    pub mints: Vec<ArbitrageMintInfo<'a, 'b>>,
    pub payer: &'a AccountInfo<'b>,
    pub token_program: &'a AccountInfo<'b>,
    pub system_program: &'a AccountInfo<'b>,
    pub associated_token_program: &'a AccountInfo<'b>,
    pub swap_1_program: &'a AccountInfo<'b>,
    pub swap_2_program: &'a AccountInfo<'b>,
    pub swap_1_pool: &'a AccountInfo<'b>,
    pub swap_2_pool: &'a AccountInfo<'b>,
    pub temperature: u8,
}

/// Checks to see if there is an arbitrage opportunity between the two pools,
/// and executes the trade if there is one
pub fn try_arbitrage(args: TryArbitrageArgs<'_, '_>) -> ProgramResult {
    msg!("Swap #1 Pool: {}", args.swap_1_pool.key);
    msg!("Swap #2 Pool: {}", args.swap_2_pool.key);
    let mints_len = args.mints.len();
    for i in 0..mints_len {
        // Load each token account and the mint for the asset we want to drive arbitrage
        // with
        let user_i = args.token_accounts_user.get(i).ok_or_arb_err()?;
        let swap_1_i = args.token_accounts_swap_1.get(i).ok_or_arb_err()?;
        let swap_2_i = args.token_accounts_swap_2.get(i).ok_or_arb_err()?;
        let mint_i = args.mints.get(i).ok_or_arb_err()?;
        for j in (i + 1)..mints_len {
            // Load each token account and the mint for the asset we are investigating
            // arbitrage trading against
            let user_j = args.token_accounts_user.get(j).ok_or_arb_err()?;
            let swap_1_j = args.token_accounts_swap_1.get(j).ok_or_arb_err()?;
            let swap_2_j = args.token_accounts_swap_2.get(j).ok_or_arb_err()?;
            let mint_j = args.mints.get(j).ok_or_arb_err()?;
            // Calculate how much of each asset we can expect to receive for our proposed
            // asset we would pay
            let r_swap_1 =
                determine_swap_receive(swap_1_j.3, mint_j.1, swap_1_i.3, mint_i.1, user_i.3)?;
            let r_swap_2 =
                determine_swap_receive(swap_2_j.3, mint_j.1, swap_2_i.3, mint_i.1, user_i.3)?;
            if r_swap_1 == 0 || r_swap_1 > swap_1_j.3 || r_swap_2 == 0 || r_swap_2 > swap_2_j.3 {
                continue;
            }
            // Evaluate the arbitrage check
            if let Some(trade) = check_for_arbitrage(r_swap_1, r_swap_2, args.temperature) {
                // If we have a trade, place it
                msg!("PLACING TRADE!");
                return match trade {
                    // Buy on Swap #1 and sell on Swap #2
                    Buy::Swap1 => {
                        msg!("Buy on Swap #1 and sell on Swap #2");
                        invoke_arbitrage(
                            (
                                *args.swap_1_program.key,
                                &[
                                    args.swap_1_pool.to_owned(),
                                    mint_j.0.to_owned(),
                                    swap_1_j.0.to_owned(),
                                    user_j.0.to_owned(),
                                    mint_i.0.to_owned(),
                                    swap_1_i.0.to_owned(),
                                    user_i.0.to_owned(),
                                    args.payer.to_owned(),
                                    args.token_program.to_owned(),
                                    args.system_program.to_owned(),
                                    args.associated_token_program.to_owned(),
                                ],
                                user_i.3,
                            ),
                            (
                                *args.swap_2_program.key,
                                &[
                                    args.swap_2_pool.to_owned(),
                                    mint_i.0.to_owned(),
                                    swap_2_i.0.to_owned(),
                                    user_i.0.to_owned(),
                                    mint_j.0.to_owned(),
                                    swap_2_j.0.to_owned(),
                                    user_j.0.to_owned(),
                                    args.payer.to_owned(),
                                    args.token_program.to_owned(),
                                    args.system_program.to_owned(),
                                    args.associated_token_program.to_owned(),
                                ],
                                r_swap_1,
                            ),
                        )
                    }
                    // Buy on Swap #2 and sell on Swap #1
                    Buy::Swap2 => {
                        msg!("Buy on Swap #2 and sell on Swap #1");
                        invoke_arbitrage(
                            (
                                *args.swap_2_program.key,
                                &[
                                    args.swap_2_pool.to_owned(),
                                    mint_j.0.to_owned(),
                                    swap_2_j.0.to_owned(),
                                    user_j.0.to_owned(),
                                    mint_i.0.to_owned(),
                                    swap_2_i.0.to_owned(),
                                    user_i.0.to_owned(),
                                    args.payer.to_owned(),
                                    args.token_program.to_owned(),
                                    args.system_program.to_owned(),
                                    args.associated_token_program.to_owned(),
                                ],
                                r_swap_2,
                            ),
                            (
                                *args.swap_1_program.key,
                                &[
                                    args.swap_1_pool.to_owned(),
                                    mint_i.0.to_owned(),
                                    swap_1_i.0.to_owned(),
                                    user_i.0.to_owned(),
                                    mint_j.0.to_owned(),
                                    swap_1_j.0.to_owned(),
                                    user_j.0.to_owned(),
                                    args.payer.to_owned(),
                                    args.token_program.to_owned(),
                                    args.system_program.to_owned(),
                                    args.associated_token_program.to_owned(),
                                ],
                                user_j.3,
                            ),
                        )
                    }
                };
            }
        }
    }
    Err(ArbitrageProgramError::NoArbitrage.into())
}

/// Enum used to tell the algorithm which swap pool is a "buy"
enum Buy {
    /// Buy on Swap #1 and sell on Swap #2
    Swap1,
    /// Buy on Swap #2 and sell on Swap #1
    Swap2,
}

/// Evaluates the percent difference in the calculated values for `r` and
/// determines which pool to buy or sell, if any
fn check_for_arbitrage(r_swap_1: u64, r_swap_2: u64, temperature: u8) -> Option<Buy> {
    // Calculate our appetite for tighter differences in `r` values based on the
    // provided `temperature`
    let threshold = 100.0 - temperature as f64;
    // Calculate the percent difference of `r` for Swap #1 vs. `r` for Swap #2
    let percent_diff = (r_swap_1 as i64 - r_swap_2 as i64) as f64 / r_swap_2 as f64 * 100.0;
    if percent_diff.abs() > threshold {
        if percent_diff > 0.0 {
            // If `r` for Swap #1 is greater than `r` for Swap #2, that means we want to buy
            // from Swap #1
            Some(Buy::Swap1)
        } else {
            // If `r` for Swap #2 is greater than `r` for Swap #1, that means we want to buy
            // from Swap #2
            Some(Buy::Swap2)
        }
    } else {
        None
    }
}

/// Invokes the arbitrage trade by sending a cross-program invocation (CPI)
/// first to the swap program we intend to buy from (receive), and then
/// immediately send another CPI to the swap program we intend to sell to
fn invoke_arbitrage(
    buy: (Pubkey, &[AccountInfo], u64),
    sell: (Pubkey, &[AccountInfo], u64),
) -> ProgramResult {
    let (buy_swap_ix_data, sell_swap_ix_data) = build_ix_datas(buy.2, sell.2);
    let ix_buy = Instruction::new_with_borsh(
        buy.0,
        &buy_swap_ix_data,
        buy.1.iter().map(ToAccountMeta::to_account_meta).collect(),
    );
    let ix_sell = Instruction::new_with_borsh(
        sell.0,
        &sell_swap_ix_data,
        sell.1.iter().map(ToAccountMeta::to_account_meta).collect(),
    );
    msg!("Executing buy ...");
    invoke(&ix_buy, buy.1)?;
    msg!("Executing sell ...");
    invoke(&ix_sell, sell.1)?;
    Ok(())
}

/// Used to build the instruction data for the `swap` instruction
/// on each swap program
fn build_ix_datas(buy_amount: u64, sell_amount: u64) -> ([u8; 16], [u8; 16]) {
    // Initialize both datas
    let mut buy_swap_ix_data = [0u8; 16];
    let mut sell_swap_ix_data = [0u8; 16];
    // Lay out the configs
    let swap_ix_hash = solana_program::hash::hash(b"global:swap");
    let buy_amount_as_bytes: [u8; 8] = buy_amount.to_le_bytes();
    let sell_amount_as_bytes: [u8; 8] = sell_amount.to_le_bytes();
    // Copy in the bytes
    buy_swap_ix_data[..8].copy_from_slice(&swap_ix_hash.to_bytes()[..8]);
    buy_swap_ix_data[8..].copy_from_slice(&buy_amount_as_bytes);
    sell_swap_ix_data[..8].copy_from_slice(&swap_ix_hash.to_bytes()[..8]);
    sell_swap_ix_data[8..].copy_from_slice(&sell_amount_as_bytes);
    (buy_swap_ix_data, sell_swap_ix_data)
}
