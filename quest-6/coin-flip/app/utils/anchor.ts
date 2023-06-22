import {
  Program,
  AnchorProvider,
  Idl,
  setProvider,
  IdlAccounts,
} from "@coral-xyz/anchor"
import { IDL, Vrf } from "../idl/vrf"
import { clusterApiUrl, Connection, Keypair, PublicKey } from "@solana/web3.js"

// Create a connection to the devnet cluster
export const connection = new Connection(clusterApiUrl("devnet"), {
  commitment: "confirmed",
})

// Create a placeholder AnchorWallet to set up AnchorProvider without connecting a wallet
const MockWallet = {
  publicKey: Keypair.generate().publicKey,
  signTransaction: () => Promise.reject(),
  signAllTransactions: () => Promise.reject(),
}

// Create an Anchor provider
export const provider = new AnchorProvider(connection, MockWallet, {})

// Set the provider as the default provider
setProvider(provider)

// Coin flip game program ID
const programId = new PublicKey("FXWi8jVNNcyCARo6JckMFPiqzcMhPo585NirdPvD2hva")

// Create the program interface using the idl, program ID, and provider
export const program = new Program(
  IDL as Idl,
  programId
) as unknown as Program<Vrf>

// SOL vault PDA for game
export const [solVaultPDA] = PublicKey.findProgramAddressSync(
  [Buffer.from("VAULT")],
  program.programId
)

// Game State Type from Idl
export type GameState = IdlAccounts<Idl>["gameState"]
