import { PublicKey } from "@solana/web3.js"

export const goldTokenMint = new PublicKey(
  "goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3"
)

export const cannonTokenMint = new PublicKey(
  "boomkN8rQpbgGAKcWvR3yyVVkjucNYcq7gTav78NQAG"
)

export const rumTokenMint = new PublicKey(
  "rumwqxXmjKAmSdkfkc5qDpHTpETYJRyXY22DWYUmWDt"
)

export const programId = new PublicKey(
  "2a4NcnkF5zf14JQXHAv39AsRf7jMFj13wKmTL6ZcDQNd"
)

export const [gameDataAccount] = PublicKey.findProgramAddressSync(
  [Buffer.from("level")],
  programId
)

export const [chestVault] = PublicKey.findProgramAddressSync(
  [Buffer.from("chestVault")],
  programId
)

export const [gameActions] = PublicKey.findProgramAddressSync(
  [Buffer.from("gameActions")],
  programId
)

export const [tokenAccountOwnerPda] = PublicKey.findProgramAddressSync(
  [Buffer.from("token_account_owner_pda")],
  programId
)

export const [tokenVault] = PublicKey.findProgramAddressSync(
  [Buffer.from("token_vault"), goldTokenMint.toBuffer()],
  programId
)
