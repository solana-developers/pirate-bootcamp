import { useState } from "react"
import { Button } from "@chakra-ui/react"
import { useConnection, useWallet } from "@solana/wallet-adapter-react"
import { useProgram } from "@/contexts/ProgramContext"
import {
  gameDataAccount,
  chestVault,
  gameActions,
  tokenAccountOwnerPda,
  goldTokenMint,
  tokenVault,
} from "@/utils/constants"

// Not used by player, only called once by the game creator to initialize the game accounts
const InitializeButton = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { connection } = useConnection()
  const [isLoading, setIsLoading] = useState(false)

  // Program from context
  const { program } = useProgram()

  const handleClick = async () => {
    setIsLoading(true)

    try {
      const tx = await program!.methods
        .initialize()
        .accounts({
          signer: publicKey!,
          newGameDataAccount: gameDataAccount,
          chestVault: chestVault,
          gameActions: gameActions,
          tokenAccountOwnerPda: tokenAccountOwnerPda,
          vaultTokenAccount: tokenVault,
          mintOfTokenBeingSent: goldTokenMint,
        })
        .transaction()
      const txSig = await sendTransaction(tx, connection)
      console.log(`https://explorer.solana.com/tx/${txSig}?cluster=devnet`)
    } catch (error) {
      console.log(error)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Button
      w="150px"
      onClick={handleClick}
      isLoading={isLoading}
      isDisabled={!publicKey}
    >
      Initialize Accounts
    </Button>
  )
}

export default InitializeButton
