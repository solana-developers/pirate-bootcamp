import { useCallback, useMemo, useState } from "react"
import { Button } from "@chakra-ui/react"
import { BorshInstructionCoder } from "@coral-xyz/anchor"
import { Keypair, SystemProgram, Transaction } from "@solana/web3.js"
import * as sbv2 from "@switchboard-xyz/solana.js"
import { useWallet } from "@solana/wallet-adapter-react"
import { useGameState } from "@/contexts/GameStateContext"
import { useSwitchboard } from "@/contexts/SwitchBoardContext"
import { program, solVaultPDA, connection } from "@/utils/anchor"

const InitPlayerButton = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { gameStatePDA } = useGameState()
  const { switchboard } = useSwitchboard()
  const [isLoading, setIsLoading] = useState(false)

  // Create a new BorshInstructionCoder instance for CoinFlip program
  const anchorProgramInstructionCoder = useMemo(
    () => new BorshInstructionCoder(program.idl),
    []
  )

  // Init player button click handler
  const handleClick = useCallback(async () => {
    if (!switchboard || !publicKey || !gameStatePDA) return

    setIsLoading(true)

    try {
      // Generate keypair for a new VRF account
      const vrfAccountKeypair = Keypair.generate()

      // Create a callback instruction to store on the VRF account (the consumeRandomness instruction)
      const vrfCallbackInstruction: sbv2.Callback = {
        programId: program.programId,
        accounts: [
          { pubkey: publicKey, isSigner: false, isWritable: true },
          { pubkey: solVaultPDA, isSigner: false, isWritable: true },
          { pubkey: gameStatePDA, isSigner: false, isWritable: true },
          {
            pubkey: vrfAccountKeypair.publicKey,
            isSigner: false,
            isWritable: false,
          },
          {
            pubkey: SystemProgram.programId,
            isSigner: false,
            isWritable: false,
          },
        ],
        ixData: anchorProgramInstructionCoder.encode("consumeRandomness", ""),
      }

      // Intructions to create the VRF account
      const [VrfAccount, createVrfAccountInstruction] =
        await sbv2.VrfAccount.createInstructions(
          switchboard?.program, // switchboard program
          publicKey, // payer
          {
            vrfKeypair: vrfAccountKeypair, // keypair to initialize the new VRF account with
            queueAccount: switchboard.queueAccount, // switchboard queue account
            callback: vrfCallbackInstruction, // callback instruction
            authority: gameStatePDA, // authority of the VRF account (game state PDA)
          }
        )

      // Instructions to create the VRF permission account
      const [PermissionAccount, createPermissionAccountInstructions] =
        sbv2.PermissionAccount.createInstruction(
          switchboard.program, // switchboard program
          publicKey, // payer
          {
            granter: switchboard.queueAccount.publicKey, // the queue account is the granter granting permission to the VRF account
            grantee: vrfAccountKeypair.publicKey, // the VRF account is the grantee being granted permission to access the queue
            authority: switchboard.queueAccountData.authority, //  the queue authority is responsible for vrf accounts access to the queue
          }
        )

      // Instruction to initialize the player's game state account
      const instruction = await program.methods
        .initialize()
        .accounts({
          player: publicKey,
          gameState: gameStatePDA,
          vrf: VrfAccount.publicKey,
        })
        .instruction()

      // Create a transaction with all the instructions
      const tx = new Transaction().add(
        ...createVrfAccountInstruction.ixns,
        ...createPermissionAccountInstructions.ixns,
        instruction
      )

      // Send the transaction, signing with the VRF account keypair (for initialization of the VRF account)
      const txSig = await sendTransaction(tx, connection, {
        signers: [vrfAccountKeypair],
      })

      console.log(`https://explorer.solana.com/tx/${txSig}?cluster=devnet`)
    } catch (error) {
      console.log(error)
    } finally {
      setIsLoading(false) // set loading state back to false
    }
  }, [publicKey, gameStatePDA, switchboard])

  return (
    <Button
      onClick={handleClick}
      isLoading={isLoading || !switchboard}
      loadingText={!switchboard ? "Loading..." : null}
    >
      Init Player
    </Button>
  )
}

export default InitPlayerButton
