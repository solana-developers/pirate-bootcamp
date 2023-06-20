import { useState } from "react"
import { Button } from "@chakra-ui/react"
import { useConnection, useWallet } from "@solana/wallet-adapter-react"
import { useProgram } from "@/contexts/ProgramContext"
import { useAccounts } from "@/contexts/AccountsContext"
import {
  gameDataAccount,
  chestVault,
  gameActions,
  tokenAccountOwnerPda,
  goldTokenMint,
  tokenVault,
} from "@/utils/constants"

const ShootButton = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { connection } = useConnection()
  const [isLoading, setIsLoading] = useState(false)

  // Program from context
  const { program } = useProgram()

  // Accounts from context
  const { playerGoldTokenAccount } = useAccounts()

  const handleClick = async () => {
    setIsLoading(true)

    try {
      const tx = await program!.methods
        .shoot(0)
        .accounts({
          player: publicKey!,
          gameDataAccount: gameDataAccount,
          chestVault: chestVault,
          tokenAccountOwner: publicKey!,
          tokenAccountOwnerPda: tokenAccountOwnerPda,
          vaultTokenAccount: tokenVault,
          playerTokenAccount: playerGoldTokenAccount!,
          mintOfTokenBeingSent: goldTokenMint,
          gameActions: gameActions,
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
      w="100px"
      onClick={handleClick}
      isLoading={isLoading}
      isDisabled={!publicKey}
    >
      Shoot
    </Button>
  )
}

export default ShootButton
