import { useEffect, useState } from "react"
import { Button } from "@chakra-ui/react"
import { useConnection, useWallet } from "@solana/wallet-adapter-react"
import { useProgram } from "@/contexts/ProgramContext"
import { useAccounts } from "@/contexts/AccountsContext"

const InitializeShipButton = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { connection } = useConnection()
  const [isLoading, setIsLoading] = useState(false)
  const [isInitialized, setIsInitialized] = useState(false)

  // Program from context
  const { program } = useProgram()

  // Accounts from context
  const { playerShipPDA, playerShipData } = useAccounts()

  const handleClick = async () => {
    setIsLoading(true)

    try {
      const tx = await program!.methods
        .initializeShip()
        .accounts({
          newShip: playerShipPDA!,
          signer: publicKey!,
          nftAccount: publicKey!, // not sure how this account is used
        })
        .transaction()
      const txSig = await sendTransaction(tx, connection)
      console.log(`https://explorer.solana.com/tx/${txSig}?cluster=devnet`)

      await connection.confirmTransaction(txSig, "confirmed")
      setIsInitialized(true)
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
      isDisabled={!publicKey || isInitialized || playerShipData}
    >
      Initialize Ship
    </Button>
  )
}

export default InitializeShipButton
