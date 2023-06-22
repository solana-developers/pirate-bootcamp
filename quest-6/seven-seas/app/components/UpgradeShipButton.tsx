import { useState } from "react"
import { Button } from "@chakra-ui/react"
import { useConnection, useWallet } from "@solana/wallet-adapter-react"
import { useProgram } from "@/contexts/ProgramContext"
import { useAccounts } from "@/contexts/AccountsContext"
import { goldTokenMint, tokenVault } from "@/utils/constants"

// Still need to figure out how to display the next upgrade cost
// and disable the button if the player doesn't have enough gold
const UpgradeShipButton = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { connection } = useConnection()
  const [isLoading, setIsLoading] = useState(false)

  // Program from context
  const { program } = useProgram()

  // Accounts from context
  const { playerGoldTokenAccount, playerShipPDA } = useAccounts()

  const handleClick = async () => {
    setIsLoading(true)

    try {
      const tx = await program!.methods
        .upgradeShip()
        .accounts({
          newShip: playerShipPDA!,
          signer: publicKey!,
          nftAccount: publicKey!,
          vaultTokenAccount: tokenVault,
          mintOfTokenBeingSent: goldTokenMint,
          playerTokenAccount: playerGoldTokenAccount!,
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
      Upgrade Ship
    </Button>
  )
}

export default UpgradeShipButton
