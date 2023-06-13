# ✨ Quest 5 - It's an Arbitrage Pirate's Life for Me

📘 A real scallywag always looks for the finest opportunities to make a quick buck!

When it comes to goods markets (swaps), there’s no better opportunity than an arbitrage opportunity.

We’re going to learn about how arbitrage works, how to build an arbitrage program, and how we can place arbitrage trades across other pirates’ markets!

---

### [<img src="https://raw.githubusercontent.com/solana-developers/pirate-bootcamp/main/docs/images/slides-icon.svg" alt="slides" width="20" align="center"/> Presentation Slides](https://docs.google.com/presentation/d/1gGMdZU4fH5_VfOEPny1Iaj6iAV_xathA3KlkgTNsl9g/edit?usp=sharing)

### Arbitrage

When we consider a Liquidity Pool in a Decentralized Exchange (DEX), we know that we will get less return for our swap the smaller the liquidity amount is for the asset we're requesting.  
This makes sense considering the concepts of supply and demand: If supply is low and demand is high, prices will rise.

In other words, for any asset we intend to pay `p`, the asset we will receive `r` will depend on the balance of `r` in the pool.  
Less of `r` in the pool = smaller number for `r`. More of `r` in the pool = larger number for `r`.  
The balance of `p` in the pool also matters, but we can assume a large discrepency in `r` values across two pools insinuates a different in balances of `p` as well.

If we take `p` and calculate `r` for two pools and see Pool #1 is offering a much lesser quantity than Pool #2, we can assume:

-   Pool #1 has less of `r` and more of `p` than Pool #2
-   Pool #2 has more of `r` and less of `p` than Pool #1
-   We can acquire `r` on Pool #2 and sell to Pool #1 for our original asset, thus generating arbitrage profit

### Our Bot

Our program (bot) is designed to evaluate all possible asset pairings for the provided accounts and determine if there is in fact an arbitrage opportunity amongst the combinations. If there is, it will place a trade between two swap pools.

One of the benefits of our bot's design is the use of **Simulated Transactions** to ensure **we are only paying for a transaction fee when we know for certain we have a profitable trade**.

This is possible because Solana is designed - by default - to simulate transactions before actually sending them to the network to catch errors. This is called a "preflight" check.

Because Solana makes use of "preflight" checks, we can simply write our arbitrage program to result in an error every time no arbitrage opportunity is present. This way, when our program's simluated "preflight" does _not_ result in an error, we know we have a profitable opportunity and thus can send the actual transaction.

The best part about this architecture is we don't even have to modify our client, only our program!

**Configurations:**

-   `concurrency`: (1 - n, where n = number of assets in swaps) How many assets the bot should evaluate combinations across at one time.  
    For example, if `concurrency` is set to 5, one instruction will tell the bot to evaluate all possible combinations of 5 assets.
-   `temperature`: (0 - 99) How aggressive the model is going to be when identifying trades based on percent difference in simulated "receive" values from both swaps.  
    For example, if `temperature` is set to 80, the bot will only identify a valid trade if the simulated "receive" value (`r`) for one swap differs from the other swap by at least 20%.

### About this Repository

This repository is broken up as follows:

-   `app`: The User Interface for running the arbitrage bot against two provided swap programs
-   `program`: The arbitrage program itself
-   `tests`: A series of tests to run on the arbitrage program

If you're following this workshop **on your own**:

1. Clone down the [repository for the swap program](https://github.com/solana-developers/pirate-bootcamp/tree/main/quest-4) and follow the steps in the `README.md` to build & deploy a swap program, add liquidity, and place some random swaps.
2. Take the program's address and paste it into this repository as Swap #1.
3. Repeat Step 1 with a new program address (you should now have two deployed active swap programs).
4. Take the program's address and paste it into this repository as Swap #2.
5. Build & deploy the arbitrage program.
6. Run `yarn run test` to test the arbitrage program.  
   You can adjust the bot's configurations - such as `concurrency` and `temperature` in the test file itself.

If you're following this workshop **in an existing bootcamp**:

> Ideally you will be working with this quest after completing Quest #4 and deploying at least two swap programs amongst your bootcamp peers

1. Deploy your arbitrage program
2. Find two swap programs you'd like to try to arbitrage trade between. Paste their program IDs into the UI or the test script

### The Arbitrage Program

Our arbitrage program is written using the "native" Solana Program crate - which means it's not built using any framework like Anchor or Nautilus.

Our program is designed for one type of instruction - the `TryArbitrage` instruction - and as you can see from our program's processor below, it will fail if you don't send the right data for the `TryArbitrage` instruction to the program.

```rust
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
```

> A quick note on the **processor**
> Solana programs typically have the following components:

-   **Instructions**: Set up the instructions for the program itself and make them available to clients attempting to transact with the program
-   **Processor**: the code that actually implements the business logic for any instruction, often processing the `match` statement that interprets which instruction the program just recieved
-   **State**: any on-chain data for any of the program's owned accounts and supporting methods and associated functions for those structs or enums

Frameworks like Anchor and Nautilus will abstract a lot of this functionality away from you, and simplify your program's components by allowing you to create just instructions and state, while building the processor for you.

With that background context out of the way, let's take a look at our arbitrage program's `processor`, which does three main things:

1. Validates the provided addresses of the two Liquidity Pools
2. Reads in all of the required accounts for searching for arbitrage opportunities
3. Executes the code that will search for an arbitrage opportunity and possibly place a trade

```rust
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
    // of
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
        swap_1_pool,
        swap_2_pool,
        swap_1_program_id,
        swap_2_program_id,
        temperature,
    })
}
```

In our program specifically, we can check that the address provided for a Liquidity Pool is valid because we know the seeds used to create these accounts in [the swap program from the last quest](https://github.com/solana-developers/pirate-bootcamp/tree/main/quest-4). If you remember, there was only one seed:

```rust
const SEED_PREFIX: &'static str = "liquidity_pool";
```

So we can simply run the `Pubkey::find_program_address(..)` function to make sure we get the same address as the one provided, like so:

```rust
//! src/util.rs

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
```

When it comes to reading in the accounts, we are expecting them to be in a very specific order, as defined by the comments above the processor's `process_arbitrage` function:

```rust
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
    ...
```

As you can see, our list should look something like this:

```rust
let account_infos = [
    payer,
    token_program,
    swap_1_pool,
    swap_2_pool,
    user_token_account_1,
    user_token_account_2,
    swap_1_token_account_1,
    swap_1_token_account_2,
    swap_2_token_account_1,
    swap_2_token_account_2,
    mint_1,
    mint_2,
]
```

This would be for an arbitrage instruction that specifies a concurreny of 2. As mentioned above, specifying a concurrency of 2 means we want to check for any and all arbitrage opportunities between two assets, which of course - since we're working with 2 pools - is going to be just one check.

However, if we increase concurency up to say 4, we are instead going to evaluate all possible combinations of 4 assets, which would be 6 possible combinations:

```shell
[0, 1, 2, 3]

[[0, 1], [0, 2], [0, 3], [1, 2], [1, 3], [2, 3]]
```

Notice we are considering one-way combinations only (ie. we look at `[0, 1]` but not `[1, 0]`). This is because our program is simply looking for differences across pools, and depending on whether or not that difference is positive or negative (pool #1 vs. pool #2) will determine whether we should buy or sell, so we are actually evaluating both `[0, 1]` and `[1, 0]` in one check!

And of course our accounts list for concurrency of 4 would instead look like this:

```rust
let account_infos = [
    payer,
    token_program,
    swap_1_pool,
    swap_2_pool,
    user_token_account_1,
    user_token_account_2,
    user_token_account_3,
    user_token_account_4,
    swap_1_token_account_1,
    swap_1_token_account_2,
    swap_1_token_account_3,
    swap_1_token_account_4,
    swap_2_token_account_1,
    swap_2_token_account_2,
    swap_2_token_account_3,
    swap_2_token_account_4,
    mint_1,
    mint_2,
    mint_3,
    mint_4,
]
```

Now, you might be wondering: "Okay, I understand how we're working with concurrency and which accounts are required and in what order, but what in the world is a Partial Token Account State?".

```rust
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
```

Well, our program is making use of certain optimizations to save on **compute units** - which are measures of how much compute your program is using on the Solana virtual machine.

All Solana programs have a maximum compute unit usage of 200k units, and the moment a program reaches that limit, it will halt and return an error.

We are building our arbitrage program to be able to check as many asset pairings as possible without breaking the compute limit, so you will see many small optimizations that can give us more room to work with.

The first optimization we'll cover is **partial deserialization** of an account's inner data, in which we are only going to convert _some_ of the bytes of an account's inner data to it's Rust form and leave the rest alone, which should be less compute-intensive.

Let's see how we do this.

Taking a look at the full schema for an Associated Token Account from the `spl-token` crate, we can find `spl-token::state::Account`:

```rust

```

For the sake of our program, we only care about the first three fields of any Associated Token Account - `mint`, `owner` and `amount`:

```rust
/// The first three fields of the `spl_token::state::Account`
#[repr(C)]
#[derive(Clone, Copy, Debug, Pod, Zeroable)]
pub struct PartialTokenAccountState {
    pub mint: Pubkey,
    pub owner: Pubkey,
    pub amount: u64,
}
```

We are going to use a Rust crate called `bytemuck` to partially deserialize the first few bytes of the account's data into the above data structure, which you can see mimics the first three fields of the actual underlying data stored in the account.

```rust
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
```

⚠️ **Warning:** This will save on compute, but this should not be considered safe. Accounts on Solana can have very similar or identical data and proper precautions should be taken to make sure your program is receiving the proper account and a brute-force attack is not underway.

Just for reference, we also do the same thing for Mint account state, taking only the first two fields:

```rust

```

```rust
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
```

All right, so that takes care of setting up our program to work its magic. But now what happens when we run `try_arbitrage`?

Well, here's our `try_arbitrage` function, but I've omitted the actual act of placing a trade from the below snippet:

```rust
/// Args for the `try_arbitrage` algorithm
pub struct TryArbitrageArgs<'a, 'b> {
    pub token_accounts_user: Vec<ArbitrageTokenAccountInfo<'a, 'b>>,
    pub token_accounts_swap_1: Vec<ArbitrageTokenAccountInfo<'a, 'b>>,
    pub token_accounts_swap_2: Vec<ArbitrageTokenAccountInfo<'a, 'b>>,
    pub mints: Vec<ArbitrageMintInfo<'a, 'b>>,
    pub payer: &'a AccountInfo<'b>,
    pub token_program: &'a AccountInfo<'b>,
    pub swap_1_pool: &'a AccountInfo<'b>,
    pub swap_2_pool: &'a AccountInfo<'b>,
    pub swap_1_program_id: &'a Pubkey,
    pub swap_2_program_id: &'a Pubkey,
    pub temperature: u8,
}

/// Checks to see if there is an arbitrage opportunity between the two pools,
/// and executes the trade if there is one
pub fn try_arbitrage(args: TryArbitrageArgs<'_, '_>) -> ProgramResult {
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
            // Evaluate the arbitrage check
            if let Some(trade) = check_for_arbitrage(r_swap_1, r_swap_2, args.temperature) {
                // If we have a trade, place it
                return match trade {
                    /* Place trade here */
                };
            }
        }
    }
    Err(ArbitrageProgramError::NoArbitrage.into())
}
```

As you can see, this is just a big nested for loop setup, where we're going to check every possible combination of assets. Clearly we're using every single account and argument passed into the program.

When we consider one iteration of the whole thing (for example let's consider `i = 0, j = 1`), we calculate the receive amount for each pool for a given asset that we propose to pay, and for this calculation we are using the token account's full balance.

More specifically, we take asset `i` and offer the max balance of the user's token account for asset `i` and calculate how much of asset `j` we'd receive from either swap pool. The difference in these values is what we use to determine whether or not we have an arbitrage opportunity.

You should remember again from the previous quest where we evaluated the Constant-Product Algorithm to calculate this value for a swap. Well, turns out - if we know that both swaps use this same algorithm - we can implement that algorithm in our arbitrage program as well!

````rust
//! swap.rs

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
````

And how do we determine what percent difference in these `r` values constitutes a valid arbitrage opportunity? Well, that's where the `temperature` value described above comes in.

When we adjust the temperature, we determine how aggressive our arbitrage model should be when it evaluates the difference between two values of `r`. For example, if we set temperature to 80, then anything over a 20% difference (100 - 80) will be identified as a valid arbitrage trade. The higher the temperature, the more aggressive the bot will be.

Here's the function that applies this temperature to two values of `r` and returns whether or not there's a trade available, and also dictates which swap to buy from and which swap to sell to:

```rust
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
```

When that function returns a `Some(Buy)`, we have a trade, and you can see it beinge evaluated in the snippet from `try_arbitrage` from earlier:

```rust
if let Some(trade) = check_for_arbitrage(r_swap_1, r_swap_2, args.temperature) {
    // If we have a trade, place it
    return match trade {
        /* Place trade here */
    };
}
```

Now that we know all of this information, we just have to take a look at the snippet with the "place trade" logic in there - which is simply going to send CPIs to both swap programs!

```rust
// Evaluate the arbitrage check
if let Some(trade) = check_for_arbitrage(r_swap_1, r_swap_2, args.temperature) {
    // If we have a trade, place it
    return match trade {
        // Buy on Swap #1 and sell on Swap #2
        Buy::Swap1 => invoke_arbitrage(
            (
                *args.swap_1_program_id,
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
                ],
                user_i.3,
            ),
            (
                *args.swap_2_program_id,
                &[
                    args.swap_2_pool.to_owned(),
                    mint_i.0.to_owned(),
                    swap_2_i.0.to_owned(),
                    user_i.0.to_owned(),
                    mint_j.0.to_owned(),
                    swap_1_j.0.to_owned(),
                    user_j.0.to_owned(),
                    args.payer.to_owned(),
                    args.token_program.to_owned(),
                ],
                r_swap_1,
            ),
        ),
        // Buy on Swap #2 and sell on Swap #1
        Buy::Swap2 => invoke_arbitrage(
            (
                *args.swap_2_program_id,
                &[
                    args.swap_2_pool.to_owned(),
                    mint_j.0.to_owned(),
                    swap_1_j.0.to_owned(),
                    user_j.0.to_owned(),
                    mint_i.0.to_owned(),
                    swap_1_i.0.to_owned(),
                    user_i.0.to_owned(),
                    args.payer.to_owned(),
                    args.token_program.to_owned(),
                ],
                r_swap_2,
            ),
            (
                *args.swap_1_program_id,
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
                ],
                user_j.3,
            ),
        ),
    };
```

And we've got our CPI calls packed into this nice little function here:

```rust
/// Invokes the arbitrage trade by sending a cross-program invocation (CPI)
/// first to the swap program we intend to buy from (receive), and then
/// immediately send another CPI to the swap program we intend to sell to
fn invoke_arbitrage(
    buy: (Pubkey, &[AccountInfo], u64),
    sell: (Pubkey, &[AccountInfo], u64),
) -> ProgramResult {
    let ix_buy = Instruction::new_with_borsh(
        buy.0,
        &buy.2,
        buy.1.iter().map(ToAccountMeta::to_account_meta).collect(),
    );
    let ix_sell = Instruction::new_with_borsh(
        sell.0,
        &sell.2,
        sell.1.iter().map(ToAccountMeta::to_account_meta).collect(),
    );
    invoke(&ix_buy, buy.1)?;
    invoke(&ix_sell, sell.1)?;
    Ok(())
}
```

Makes sense, right? One last item. Take a look at our snippet for `try_arbitrage` again:

```rust
/// Checks to see if there is an arbitrage opportunity between the two pools,
/// and executes the trade if there is one
pub fn try_arbitrage(args: TryArbitrageArgs<'_, '_>) -> ProgramResult {
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
            // Evaluate the arbitrage check
            if let Some(trade) = check_for_arbitrage(r_swap_1, r_swap_2, args.temperature) {
                // If we have a trade, place it
                return match trade {
                    /* Place trade here */
                };
            }
        }
    }
    Err(ArbitrageProgramError::NoArbitrage.into())
}
```

Notice the very last line: if we never execute an arbitrage trade, we return an error `ArbitrageProgramError::NoArbitrage`. This is because - remember from the description earlier - we want to return an error if no trade is available. This way, we can leverage the preflight simulated transaction to ensure we never pay for a transaction fee unless we have a legitimate trade!

### Address Lookup Tables

When you move over to the client-side of our arbitrage bot, you'll notice we're using a relatively new piece of Solana technology - which allows us to increase concurrency and pack more accounts into a transaction - called an **Address Lookup Table**.

**Address Lookup Tables** are special accounts on Solana that are literally a Hash Map of `<u8, Pubkey>`, where the `u8` is an index starting at 0 and the `Pubkey` is whatever address you put into the table.

The beauty of these accounts is that the transaction v0 (latest transaction model for Solana) supports these lookup tables natively. What does this mean? Well, if you include a lookup table account in your transaction, the transaction itself can refer to the `u8` indices of the accounts instead of their full 32-byte `Pubkey` addresses!

Let's consider that more closely:

-   Say we have 20 accounts we need to include in our transaction, total. That makes 20 \* 32 = 640 bytes
-   Now say we include an address lookup table as well (another 32 bytes). That makes 640 + 32 = 672 bytes
-   However, if all 20 accounts are included in the lookup table, we can swap their 32-byte addresses for their `u8` indices, thus turning our total bytes for our accounts into: 32 + (20 \* 1) = 52

So as you can see we are able to push more accounts into a transaction when we send that single instruction to our arbitrage program. This is important because we know we'll be sending a lot of accounts and we'd rather maximize the program's capability for concurrency than have to reduce concurrency because our transaction size is too large.

### Tests

Our program only has one instruction, so as long as we know of two active swap programs we can arbitrate between and we have the proper tokens to trade, we can simply invoke the arbitrage program with those liquidity pools as the provided program IDs.

Our actual test suite `main.test.ts` will do the following:

1. Gather all token accounts by iterating through the list of know assets
    - Airdrops some asset tokens to the payer if the payer requires some (payer must be mint authority)
2. Create an Address Lookup Table and populate it with all the addresses from:
    - User's token accounts
    - Swap #1's token accounts
    - Swap #2's token accounts
    - Mints
3. Work through every possible one-way combination of the list of assets and send an instruction to our program according to concurrency
    - ie. if concurrency is 5, send instruction every 5 asset pairs

### UI

The UI is pretty straightforward for this demo: it prompts you to paste in the two program IDs for the swap programs you want to arbitrate between and allows you to set the configurations (`concurrency`, `temperature`). Then, once triggered, it will continually search for arbitrage opportunities!
