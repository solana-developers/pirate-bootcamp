import { Button, HStack } from "@chakra-ui/react"
import { useCallback, useMemo, useState } from "react"
import {
  LAMPORTS_PER_SOL,
  PublicKey,
  SYSVAR_RECENT_BLOCKHASHES_PUBKEY,
  SystemProgram,
  Transaction,
  TransactionInstruction,
} from "@solana/web3.js"
import {
  createAssociatedTokenAccountInstruction,
  createSyncNativeInstruction,
  getAssociatedTokenAddressSync,
  NATIVE_MINT,
} from "@solana/spl-token"
import * as sbv2 from "@switchboard-xyz/solana.js"
import { TOKEN_PROGRAM_ID } from "@coral-xyz/anchor/dist/cjs/utils/token"
import { useWallet } from "@solana/wallet-adapter-react"
import { useGameState } from "@/contexts/GameStateContext"
import { useSwitchboard } from "@/contexts/SwitchBoardContext"
import { program, connection, solVaultPDA } from "@/utils/anchor"

const CoinTossButtons = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { gameStatePDA, gameStateData, isLoading } = useGameState()
  const { switchboard } = useSwitchboard()
  const [loadingButton, setLoadingButton] = useState(0)

  // The VRF account's associated token account for Wrapped SOL
  // This "escrow" token account is used to pay Oracle fees
  const escrowWrappedSOLTokenAccount = useMemo(() => {
    if (gameStateData) {
      return getAssociatedTokenAddressSync(
        NATIVE_MINT,
        gameStateData.vrf as PublicKey
      )
    }
  }, [gameStateData])

  // The player's associated token account for Wrapped SOL
  // This wSol token account is used to fund the escrow token account
  const playerWrappedSOLTokenAccount = useMemo(
    () => getAssociatedTokenAddressSync(NATIVE_MINT, publicKey!),
    [publicKey]
  )

  // Button click handler
  const handleClick = useCallback(
    async (guess: number) => {
      if (!switchboard || !publicKey || !gameStatePDA) return

      setLoadingButton(guess)

      try {
        // Derive the existing VRF account's permission account using the seeds
        // This was created when the VRF account was initialized
        const [permissionAccount, permissionBump] =
          sbv2.PermissionAccount.fromSeed(
            switchboard.program,
            switchboard.queueAccountData.authority,
            switchboard.queueAccount.publicKey,
            gameStateData!.vrf as PublicKey
          )

        // Prepare array of instructions to send in the transaction
        let instructions: TransactionInstruction[] = []

        // Check if the player's Wrapped SOL token account exists
        const account = await connection.getAccountInfo(
          playerWrappedSOLTokenAccount
        )

        // If player's Wrapped SOL token account doesn't exist, add instruction create it
        if (!account) {
          instructions.push(
            createAssociatedTokenAccountInstruction(
              publicKey, // payer
              playerWrappedSOLTokenAccount, // associated token account address
              publicKey, // owner of token account
              NATIVE_MINT // mint address (Wrapped SOL)
            )
          )
        }

        // Add instruction to transfer 0.002 SOL to the player's Wrapped SOL token account
        // This is the amount needed to pay the Oracle fee, which will be transferred to the escrow token account
        instructions.push(
          SystemProgram.transfer({
            fromPubkey: publicKey,
            toPubkey: playerWrappedSOLTokenAccount,
            lamports: 0.002 * LAMPORTS_PER_SOL,
          })
        )

        // Sync the player's Wrapped SOL token account and SOL balance
        instructions.push(
          createSyncNativeInstruction(playerWrappedSOLTokenAccount)
        )

        // Request randomness instruction
        const instruction = await program.methods
          .requestRandomness(
            permissionBump,
            switchboard.program.programState.bump,
            guess // 1 = heads, 2 = tails
          )
          .accounts({
            solVault: solVaultPDA, // Game SOL Vault
            gameState: gameStatePDA, // Player's game state account
            vrf: gameStateData!.vrf as PublicKey, // game state's VRF account
            oracleQueue: switchboard.queueAccount.publicKey, // switchboard queue account (the queue that the VRF account has permission to)
            queueAuthority: switchboard.queueAccountData.authority, // switchboard queue account's authority
            dataBuffer: switchboard.queueAccountData.dataBuffer, // switchboard queue account's data buffer
            permission: permissionAccount.publicKey, // VRF account's permission account
            escrow: escrowWrappedSOLTokenAccount!, // vrf account's escrow wSol token account
            programState: switchboard.program.programState.publicKey, // switchboard program state account
            switchboardProgram: switchboard.program.programId, // switchboard program
            payerWallet: playerWrappedSOLTokenAccount, // player's wSol token account
            player: publicKey, // player's public key
            recentBlockhashes: SYSVAR_RECENT_BLOCKHASHES_PUBKEY, // recent blockhashes sysvar
            tokenProgram: TOKEN_PROGRAM_ID, // token program
          })
          .instruction()

        // Create a transaction with all the instructions
        const tx = new Transaction().add(...instructions, instruction)

        const txSig = await sendTransaction(tx, connection)
        console.log(`https://explorer.solana.com/tx/${txSig}?cluster=devnet`)
      } catch (error) {
        console.log(error)
      } finally {
        setLoadingButton(0) // Clear the loadingButton regardless of success/failure.
      }
    },
    [publicKey, gameStatePDA, gameStateData, switchboard]
  )

  return (
    <HStack>
      <Button
        width="75px"
        isLoading={loadingButton === 1}
        isDisabled={isLoading || !switchboard}
        onClick={() => handleClick(1)}
      >
        Heads
      </Button>
      <Button
        width="75px"
        isLoading={loadingButton === 2}
        isDisabled={isLoading || !switchboard}
        onClick={() => handleClick(2)}
      >
        Tails
      </Button>
    </HStack>
  )
}

export default CoinTossButtons
