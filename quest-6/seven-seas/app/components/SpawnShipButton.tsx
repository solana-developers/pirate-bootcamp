import { useState } from "react"
import { Button } from "@chakra-ui/react"
import { useConnection, useWallet } from "@solana/wallet-adapter-react"
import { useProgram } from "@/contexts/ProgramContext"
import { Keypair } from "@solana/web3.js"
import { useAccounts } from "@/contexts/AccountsContext"
import {
  gameDataAccount,
  chestVault,
  cannonTokenMint,
  rumTokenMint,
} from "@/utils/constants"

const SpawnShipButton = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { connection } = useConnection()
  const [isLoading, setIsLoading] = useState(false)

  // Program from context
  const { program } = useProgram()

  // Accounts from context
  const { playerCannonTokenAccount, playerRumTokenAccount, playerShipPDA } =
    useAccounts()

  const handleClick = async () => {
    setIsLoading(true)

    // Not sure how this is used
    const avatarPubkey = Keypair.generate()

    try {
      const tx = await program!.methods
        .spawnPlayer(avatarPubkey.publicKey)
        .accounts({
          player: publicKey!,
          tokenAccountOwner: publicKey!,
          gameDataAccount: gameDataAccount,
          chestVault: chestVault,
          nftAccount: publicKey!, // not sure how this account is used
          ship: playerShipPDA!,
          cannonTokenAccount: playerCannonTokenAccount!,
          cannonMint: cannonTokenMint,
          rumTokenAccount: playerRumTokenAccount!,
          rumMint: rumTokenMint,
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
      Spawn Ship
    </Button>
  )
}

export default SpawnShipButton
