# 💎 Quest 4 - Smuggling, Bargaining, and Upgrading Your Ship

📘 We saw how you can “tokenize” assets using NFTs & tokens, and trade them for Gold.

Now let’s see how we can expand on those concepts to trade (or swap) assets, scale prices based on supply, and create markets for assets!

We’ll explore the fundamental concepts behind a Decentralized Exchange (DEX) as we effectively create an automated market maker.

Build your trading center and see if you can get other pirates to do business in your neck of the woods!

---

### [<img src="https://raw.githubusercontent.com/solana-developers/pirate-bootcamp/main/docs/images/slides-icon.svg" alt="slides" width="20" align="center"/> Presentation Slides](https://docs.google.com/presentation/d/1E15mIvnMg9qvR9RPJnIC9Y4cod-QjBJpjPZ4rQpgEIE/edit?usp=sharing)

### DEXs

Decentralized Exchanges (DEXs) make use of what's called a "Liquidity Pool". This is simply a pool of assets (usually tokens) that are provided by investors (called "Liquidity Providers") and can be exchanged on the platform in a permissionless, peer-to-peer manner.

A common way DEXs manage a liquidity pool is through a **Constant-Product Algorithm**, which involves programmatically ensuring that the product of all assets equals some constant value `K`.

Consider a request to swap `p` number of one asset for some unknown number of another asset `r`:

```
K = a * b * c * d * P * R
K = a * b * c * d * (P + p) * (R - r)

a * b * c * d * P * R = a * b * c * d * (P + p) * (R - r)
PR = (P + p) * (R - r)
PR = PR - Pr + Rp - pr
0 = 0 - Pr + Rp - pr
-Rp = -Pr - pr
-Rp = r(-P - p)
r = (-Rp) / (-P - p)
r = [-1 * Rp] / [-1 * (P + p)]
r = Rp / (P + p)

r = f(p) = (R * p) / (P + p)
```

### About this Repository

This repository is broken up as follows:

-   `app`: The User Interface for interacting with a deployed swap program
-   `programs/swap_program`: The swap program itself
-   `tests`: A series of tests to run on the swap program

If you're following this workshop **on your own**:

1. In a _separate terminal_, start a local validator with `solana-test-validator`. We have multiple tests scripts, so `anchor test` will not work. We must use `anchor run test`.
2. Run `upload-json.test.ts` to upload the images to Arweave's devnet, where they will be available to create token metadata
3. Run `create-assets.test.ts` to create SPL tokens for each asset and mint them to your local keypair
4. Run `main.test.ts` to test the swap program:
    - First this test will fund the Liquidity Pool from your local keypair's minted assets
    - Then it will attempt to load the Liquidity Pool's holdings
    - Lastly, it will run several test swaps on the pool using your local keypair
        - Note: These tests will first mint new assets to your local keypair to swap with

If you're following this workshop **in an existing bootcamp**:

1. In a _separate terminal_, start a local validator with `solana-test-validator`. We have multiple tests scripts, so `anchor test` will not work. We must use `anchor run test`.
2. You will have to request assets be airdropped to your wallet from a bootcamp administrator
    - This is for testing the swap program and for funding your swap program's Liquidity Pool after deployment
    - The mint addresses of each asset are fixed for the bootcamp, so creating your own with `create-assets.test.ts` will create new assets only compatible with your swap program and not with the rest of the bootcamp
3. Deploy your swap program
4. Request your swap program be funded
5. You will start with only Gold as an asset, and you can begin swapping for other assets once the pool is funded

---

### The Swap Program

Our swap program makes use of one Program-Derived Address (PDA) account in which we are storing some custom data. This is our `LiquidityPool`.

You can find the data schema of our `LiquidityPool` account under `src/state.rs`:

```rust
/// The `LiquidityPool` state - the inner data of the program-derived address
/// that will be our Liquidity Pool
#[account]
pub struct LiquidityPool {
    pub assets: Vec<Pubkey>,
    pub bump: u8,
}
```

There's a lot more code in the `src/state.rs` file, but we'll come back to that later on.

For now, you can see we are storing the `bump` - which is a `u8` seed used to create the PDA - and we are also storing a vector of `Pubkey`s - which will represent the mint addresses of every asset supported by our pool.

Technically, we don't _need_ to store all of the mint addresses in our `LiquidityPool` state, but in our particular program this is simple a design choice, so we can reference these addresses during other instructions of the program.

Speaking of instructions, let's take a look at the instructions offered by our Swap Program:

1. `src/instructions/create_pool.rs`: Creates a new Liquidity Pool

```rust
/// Initialize the program by creating the liquidity pool
pub fn create_pool(ctx: Context<CreatePool>) -> Result<()> {
    // Initialize the new `LiquidityPool` state
    ctx.accounts.pool.set_inner(LiquidityPool::new(
        *ctx.bumps
            .get("pool")
            .expect("Failed to fetch bump for `pool`"),
    ));
    Ok(())
}

#[derive(Accounts)]
pub struct CreatePool<'info> {
    /// Liquidity Pool
    #[account(
        init,
        space = LiquidityPool::SPACE,
        payer = payer,
        seeds = [LiquidityPool::SEED_PREFIX.as_bytes()],
        bump,
    )]
    pub pool: Account<'info, LiquidityPool>,
    /// Rent payer
    #[account(mut)]
    pub payer: Signer<'info>,
    /// System Program: Required for creating the Liquidity Pool
    pub system_program: Program<'info, System>,
}
```

As you can see, this instruction simply initializes our pool and sets the inner data. It will store the bump seed and an empty vector.

We've laid out our seeds for the `LiquidityPool` to be only the `SEED_PREFIX` and nothing else, which means there can only be **one** of these pools for our program. This is again another design choice. If you were to create your own DEX or Swap Program, you might want to support multiple pools, and you can do so!

Here's that `SEED_PREFIX` constant and some other information from `src/state.rs` that we're using in the `create_pool(..)` instruction:

```rust
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
```

2. `src/instructions/fund_pool.rs`: Funds a new Liquidity Pool

```rust
/// Provide liquidity to the pool by funding it with some asset
pub fn fund_pool(ctx: Context<FundPool>, amount: u64) -> Result<()> {
    let pool = &mut ctx.accounts.pool;

    // Deposit: (From, To, amount)
    let deposit = (
        &ctx.accounts.mint,
        &ctx.accounts.payer_token_account,
        &ctx.accounts.pool_token_account,
        amount,
    );

    pool.fund(
        deposit,
        &ctx.accounts.payer,
        &ctx.accounts.system_program,
        &ctx.accounts.token_program,
    )
}

#[derive(Accounts)]
pub struct FundPool<'info> {
    /// Liquidity Pool
    #[account(
        mut,
        seeds = [LiquidityPool::SEED_PREFIX.as_bytes()],
        bump = pool.bump,
    )]
    pub pool: Account<'info, LiquidityPool>,
    /// The mint account for the asset being deposited into the pool
    pub mint: Account<'info, token::Mint>,
    /// The Liquidity Pool's token account for the asset being deposited into
    /// the pool
    #[account(
        init_if_needed,
        payer = payer,
        associated_token::mint = mint,
        associated_token::authority = pool,
    )]
    pub pool_token_account: Account<'info, token::TokenAccount>,
    /// The payer's - or Liquidity Provider's - token account for the asset
    /// being deposited into the pool
    #[account(
        mut,
        associated_token::mint = mint,
        associated_token::authority = payer,
    )]
    pub payer_token_account: Account<'info, token::TokenAccount>,
    // Payer / Liquidity Provider
    #[account(mut)]
    pub payer: Signer<'info>,
    /// System Program: Required for creating the Liquidity Pool's token account
    /// for the asset being deposited into the pool
    pub system_program: Program<'info, System>,
    /// Token Program: Required for transferring the assets from the Liquidity
    /// Provider's token account into the Liquidity Pool's token account
    pub token_program: Program<'info, token::Token>,
    /// Associated Token Program: Required for creating the Liquidity Pool's
    /// token account for the asset being deposited into the pool
    pub associated_token_program: Program<'info, associated_token::AssociatedToken>,
}
```

Here we are funding the pool with some quantity of tokenized assets to do what's called "providing liquidity". All this really means is that we're depositing some quantity of a tokenized asset to make it available for swapping on the DEX.

First let's take a look at the accounts involved, since there appears to be many. They all have corresponding comments, but the key is that we use the **Mint** itself, the **Payer's Token Account** for that mint, the **Pool's Token Account** for that mint (which we create here if it does not exist already), and finally the **Payer** themself.

With these four accounts (and the various programs), we can conduct the transfer of the tokens from the payer to the pool, however, notice we also include the **Pool** itself in this instruction. This is because we actually are going to add that mint's address to our pool's vector if it doesn't exist already.

Take a look what's happening under the hood when we run the `pool.fund(..)` function, as written in this code from `src/state.rs`:

```rust
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
```

You can see first we add the asset to the vector and then we process the transfer! But what does that look like?

Here's our `add_asset(..)` function, also in `src/state.rs`:

```rust
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
```

As you can see, we're checking to see if the mint's address is already in the vector using `check_asset_key(&key)`, and if the address isn't already in the vector, we're going to add it, but we also use this `realloc` call. What's `realloc`?

On Solana, when you initialize an account and pay rent, you are given a fixed size for that account. So, when we added a `u8` and an empty vector, we got our initial size, which we designated using the constant `SPACE` (shown above), and we create an account for our pool with a size of 13 bytes.

However, whenever we want to add items to the vector, that vector is going to increase in size! That's because we are using a vector instead of an array. In Rust, an array must have a fixed size, so we'd confidently allocate enough space for the maximum size of the array. Since the vector can change in size - and it will do so whenever we add items to it - we instead want to _increase the size of the account_ each time we add a new item.

Increasing the size of a Solana account is called "reallocating" the account's space, and it only costs the additional rent for the added space.

With that being said, we have this function that handles reallocating the account's size and also funding it with any additional rent required for the new size:

```rust
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
```

Once all of that re-sizing (reallocating) is done, we are also going to process the transfer of the asset from the payer to the pool, like so:

```rust
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
```

This one is pretty straightforward: we're just invoking the Token Program from our program - using what's called a Cross-Program Invocation (CPI) and passing it the instructions and accounts necessary to conduct a transfer from the payer's token account to our pool's token account, which of course the payer themselves has to sign to authorize. Instead of signing the CPI itself, the payer just signs the transaction that sends the instruction to _our_ program.

3. `src/instructions/create_pool.rs`: Creates a new Liquidity Pool

```rust
/// Swap assets using the DEX
pub fn swap(ctx: Context<Swap>, amount_to_swap: u64) -> Result<()> {
    // Make sure the amount is not zero
    if amount_to_swap == 0 {
        return Err(SwapProgramError::InvalidSwapZeroAmount.into());
    }

    let pool = &mut ctx.accounts.pool;

    // Receive: The assets the user is requesting to receive in exchange:
    // (Mint, From, To)
    let receive = (
        ctx.accounts.receive_mint.as_ref(),
        ctx.accounts.pool_receive_token_account.as_ref(),
        ctx.accounts.payer_receive_token_account.as_ref(),
    );

    // Pay: The assets the user is proposing to pay in the swap:
    // (Mint, From, To, Amount)
    let pay = (
        ctx.accounts.pay_mint.as_ref(),
        ctx.accounts.payer_pay_token_account.as_ref(),
        ctx.accounts.pool_pay_token_account.as_ref(),
        amount_to_swap,
    );

    pool.process_swap(
        receive,
        pay,
        &ctx.accounts.payer,
        &ctx.accounts.token_program,
    )
}

#[derive(Accounts)]
pub struct Swap<'info> {
    /// Liquidity Pool
    #[account(
        mut,
        seeds = [LiquidityPool::SEED_PREFIX.as_bytes()],
        bump = pool.bump,
    )]
    pub pool: Account<'info, LiquidityPool>,
    /// The mint account for the asset the user is requesting to receive in
    /// exchange
    #[account(
        constraint = !receive_mint.key().eq(&pay_mint.key()) @ SwapProgramError::InvalidSwapMatchingAssets
    )]
    pub receive_mint: Box<Account<'info, token::Mint>>,
    /// The Liquidity Pool's token account for the mint of the asset the user is
    /// requesting to receive in exchange (which will be debited)
    #[account(
        init_if_needed,
        payer = payer,
        associated_token::mint = receive_mint,
        associated_token::authority = pool,
    )]
    pub pool_receive_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The user's token account for the mint of the asset the user is
    /// requesting to receive in exchange (which will be credited)
    #[account(
        mut,
        associated_token::mint = receive_mint,
        associated_token::authority = payer,
    )]
    pub payer_receive_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The mint account for the asset the user is proposing to pay in the swap
    pub pay_mint: Box<Account<'info, token::Mint>>,
    /// The Liquidity Pool's token account for the mint of the asset the user is
    /// proposing to pay in the swap (which will be credited)
    #[account(
        mut,
        associated_token::mint = pay_mint,
        associated_token::authority = pool,
    )]
    pub pool_pay_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The user's token account for the mint of the asset the user is
    /// proposing to pay in the swap (which will be debited)
    #[account(
        mut,
        associated_token::mint = pay_mint,
        associated_token::authority = payer,
    )]
    pub payer_pay_token_account: Box<Account<'info, token::TokenAccount>>,
    /// The authority requesting to swap (user)
    #[account(mut)]
    pub payer: Signer<'info>,
    /// Token Program: Required for transferring the assets between all token
    /// accounts involved in the swap
    pub token_program: Program<'info, token::Token>,
    pub system_program: Program<'info, System>,
    pub associated_token_program: Program<'info, AssociatedToken>,
}

```

This is our program's third and final instruction, and it's just the instruction you send whenever you want to swap one asset for another.

In our swap program (and most swap programs), we have two assets: the `pay` asset the user is offering to pay, and the `receive` asset the user is requesting to receive in return.

The user is going to select those two assets ahead of time, and pass the necessary token accounts for themselves and the pool in order to process the two transfers:

-   Transfer the `pay` asset from the **payer's** `payer_pay_token_account` to the **pool's** `pool_pay_token_account`
-   Transfer the `receive` asset from the **pool's** `pool_receive_token_account` to the **payer's** `payer_receive_token_account`

We also require the pool account, the mint accounts themselves, and the required programs for these CPIs. Just like in the `fund_pool` instruction, we create any token accounts that don't already exist.

Now here's where we're going to conduct our swap and leverage the Constant-Product Algorithm to determine how much of the `receive` asset a user should receive for the quantity of the `pay` asset they're offering to pay.

Here's the swap being processed, again in `src/state.rs`:

```rust
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
```

The first thing we do is check to make sure both assets' addresses are in our pool's vector of public keys (since if they aren't, there is no provided liquidity available to swap!). Once that checks out, we can go ahead and calculate how much the user is to receive, then process both transfers.

The calculation of how much they will receive is the Constant-Product Algorithm, and it looks like this:

````rust
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
````

Not so bad, right? As you can see, once we've done the arithmetic and derived a concise formula for CPA, we can just code that algorithm into a function and use it like above.

A couple quick items to consider with our implementation of the CPA:

-   We are making sure we use each mint's decimal places to calculate it's total supply in the pool
    -   This allows us to "normalize" the values as we calculate CPA
    -   It also allows us to lose less quantity of an asset when rounding off the decimal places at the end
-   We make sure your calculated `receive` value doesn't overflow the pool's liquidity
    -   If you are offering too much of an asset and the pool can't afford to pay you what it's worth in the `receive` asset, an error is thrown!

And once we've calculated this value and all checks pass we can process both transfers via CPI and terminate the program with an `Ok(())`.

That's all there is to it!

### Tests

The tests in this repository are broken up as follows:

1. `upload-json.test.ts`: Uploads images to Arweave

2. `create-assets.test.ts`: Creates & mints new asset tokens

3. `main.test.ts`: straightforward tests of each instruction we just covered from the program.

    - Initialize the Liquidity Pool by creating the `LiquidityPool` PDA
    - Fund the Liquidity Pool with some tokens of varying assets, so we have some diversity
    - Run some random swaps!

4. `master.test.ts`: For bootcamp administrators who would like to initialize a swap program with existing mints using their local keypair as a funder/authority

### UI

The UI is just a wrapper around the `swap` instruction, and shows you an example of how to build a website for your swap program.

Note: This UI is by no means a production-grade build of all of the checks and various security measures a live swap site that wraps a swap program should have, but it should be enough to get you started!

The UI includes some useful components:

-   Wallet connector
-   Asset selection for `pay` and `receive`
-   Swap preview (using Constant-Product Algorithm and the pool's token accounts)
-   Swap execution with wallet signing
