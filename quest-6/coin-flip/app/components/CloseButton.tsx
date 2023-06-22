import { useCallback, useState } from "react"
import { Button } from "@chakra-ui/react"
import { useWallet } from "@solana/wallet-adapter-react"
import { useGameState } from "@/contexts/GameStateContext"
import { program, connection } from "@/utils/anchor"

const CloseButton = () => {
  const { publicKey, sendTransaction } = useWallet()
  const { gameStatePDA } = useGameState()
  const [isLoading, setIsLoading] = useState(false)

  // Close button click handler
  const handleClick = useCallback(async () => {
    if (!publicKey || !gameStatePDA) return

    setIsLoading(true)

    try {
      // Transaction to close the game state account
      const tx = await program.methods
        .close()
        .accounts({
          player: publicKey,
          gameState: gameStatePDA,
        })
        .transaction()

      // Send the transaction
      const txSig = await sendTransaction(tx, connection)
      console.log(`https://explorer.solana.com/tx/${txSig}?cluster=devnet`)
    } catch (error) {
      console.log(error)
    } finally {
      setIsLoading(false)
    }
  }, [publicKey, gameStatePDA])

  return (
    <Button onClick={handleClick} isLoading={isLoading}>
      Close
    </Button>
  )
}

export default CloseButton
