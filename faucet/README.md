# Pirate Faucet

This faucet allows bootcamp participants to receive limited airdrops for bootcamp asset tokens in case they have issues with any of the DeFi workshop sessions.

The program should be deployed to `devnet`, and it will leverage a `Ledger` PDA account to ensure no bootcamp participant's particular wallet receives more asset tokens than the designated limit.

Limit `src/state/ledger#L16`:

```rust
/// Max airdrops for any asset
pub const MAX_AIRDROP_AMOUNT: u64 = 40;
```

---

This program is not entirely protected against gameability, and proper steps should be taken to ensure any pools of bootcamp asset tokens are not drained.

---

Administrators can deploy this program to `devnet` and run `anchor run test` to fund the faucet. Then, bootcamp participants can leverage the following components to build the necessary client-side script to request an airdrop from the program:

-   The program instruction call in `tests/instructions/index.ts`
-   Creation of the proper associated token accounts in `tests/util/token.ts`
-   The token asset IDs in `tests/main.test.ts`

> Note: You will also have to paste in the `assets.json` file from the currently active swap program!
