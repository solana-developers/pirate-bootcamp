# ⛴️ Quest 1 - Mint a Pirate Ship NFT

📘 Write the code to mint yourself an NFT that will represent your pirate ship.

Throughout the bootcamp, we’ll customize this NFT, use it to build other things, and ultimately you’ll battle other NFTs on the high seas.

## Workshop Day 1: Solana Basics

## Quick links to learn more

- https://docs.solana.com/developers
- Slides from the workshop:
  https://docs.google.com/presentation/d/1N2-3XMqtoZspSeiPJt3JQChTljCxb-0xJTelP315g1I/edit?usp=sharing

## Tech stack used

- uses TypeScript and NodeJS
- yarn (as the package manager)

## Setup locally

1. Clone this repo to your local system
2. Install the packages via `yarn install`
3. Copy rename the `example.env` file to be named `.env`
4. Update the `RPC_URL` variable to be the cluster URL of a supporting RPC provider

If you have the Solana CLI installed locally: update the `LOCAL_PAYER_JSON_ABSPATH` environment
variable to be the **_absolute path_** of your local testing wallet keypair JSON file.

## Recommended flow to explore this repo

After setting up locally, I recommend exploring the code of the following files (in order):

- [`1.simpleTransaction.ts`](./scripts/1.simpleTransaction.ts)
- [`2.complexTransaction.ts`](./scripts/2.complexTransaction.ts)
- [`3.createTokenWithMetadata.ts`](./scripts/3.createTokenWithMetadata.ts)
- [`4.mintTokens.ts`](./scripts/4.mintTokens.ts)
- [`5.updateMetadata.ts`](./scripts/5.updateMetadata.ts)
- [`6.createNFTs.ts`](./scripts/6.createNFTs.ts)

After reviewing the code in each of these scripts, try running each in order.

> **Note:** Running each of these scripts may save some various bits of data to a `.local_keys`
> folder within this repo for use by the other scripts later in this ordered list. Therefore,
> running them in a different order may result in them not working as written/desired. You have been
> warned :)

### Running the included Scripts

Once setup locally, you will be able to run the scripts included within this repo:

```
yarn demo ./scripts/<script>
```

#### `1.simpleTransaction.ts`

A brief introduction to the Solana web3.js package. Demonstrating how to build and send simple
transactions to the blockchain

#### `2.complexTransaction.ts`

An introduction to more complex transactions using Solana web3.js Demonstrates how to build a more
complex transaction, with multiple instructions.

#### `3.createTokenWithMetadata.ts`

Demonstrates how to create a SPL token and store it's metadata on chain (using the Metaplex MetaData
program)

#### `4.mintTokens.ts`

Demonstrates how to create new SPL tokens (aka "minting tokens") into an existing SPL Token Mint

#### `5.updateMetadata.ts`

Demonstrates how to update the metadata for an SPL token, using the Metaplex MetadataProgram

#### `6.createNFTs.ts`

Demonstrates how to mint NFTs and store their metadata on chain using the Metaplex MetadataProgram