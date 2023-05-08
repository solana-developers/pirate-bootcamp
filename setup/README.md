# Organizing this Bootcamp

One party should be responsible for setting everything up. To do so, you'll need to configure a development environment to run the associated scripts mentioned in this document.

### Environment Setup:
* Install [Rust](https://www.rust-lang.org/tools/install)
* Install [NodeJS](https://www.linode.com/docs/guides/how-to-install-use-node-version-manager-nvm/)
* Install [Yarn](https://classic.yarnpkg.com/lang/en/docs/install/#mac-stable)
* Install [Solana CLI](https://solanacookbook.com/getting-started/installation.html) (v <version>)
* Install [Anchor CLI](https://dev.to/dabit3/comment/1j2ma) (v <version>)

### Bootcamp Setup:

1. Take a look at all of the pieces:
    * This bootcamp is comprised of the following components:
        * NFT & SPL Token assets for trading in-game
        * The High Seas Program, which everyone's ships are deployed with
        * Quest-specific workshop programs or scripts

2. Set up a master keypair for creating assets and deploying programs.
    * You'll want to generate a new Solana keypair using `solana-keygen new` and store that keypair in a safe place.
    * The scripts used for setup are configured to use your local keypair, so be sure to run `solana config set --keypair <your-keypair-path>.json`.
    * This bootcamp is designed for `devnet`, so be sure to run `solana config set -ud`.
    * You'll also need to obtain some `devnet` SOL - you'll need at least <> SOL total to set everything up.

3. Deploy programs and create assets.
    * `cd setup`.
    * ⚠️ Run all scripts from the within the `setup` directory!
    * Build all of the programs using `build.sh`.
    * Deploy all of the programs using `deploy.sh`(<> SOL required).
    * Generate the in-game assets using `create_assets.sh`(<> SOL required).
    * ⚠️ This will mint all in-game assets to your generated keypair!
    * If any scripts fail, you can follow the README's in the associated Rust or JavaScript libraries to deploy/run manually.

4. Read-through and/or watch the workshop sessions.
    * Each "quest" has one or more associated workshop sessions to teach all the required concepts for the quest.
    * You can read through the documentation, resources, and code to understand how to conduct these workshop sessions, or watch the videos provided with each (where applicable).

5. Anchor's Aweigh!
    * That should be all you need to get started!
    * This bootcamp is loosely designed to be conducted by teaching one "quest" per day, but you can adjust it however you see fit.