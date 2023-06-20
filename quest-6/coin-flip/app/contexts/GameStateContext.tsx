import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
} from "react"
import * as anchor from "@coral-xyz/anchor"
import { useWallet } from "@solana/wallet-adapter-react"
import { AccountInfo, PublicKey } from "@solana/web3.js"
import { useBalance } from "@/contexts/BalanceContext"
import { program, connection, GameState } from "@/utils/anchor"

// Define the structure of the GameState context state
type GameStateContextType = {
  gameStatePDA: PublicKey | undefined
  gameStateData: GameState | undefined
  isLoading: boolean
  message: string
}

// Create the context with default values
const GameStateContext = createContext<GameStateContextType>({
  gameStatePDA: undefined,
  gameStateData: undefined,
  isLoading: false,
  message: "Play the game!",
})

// Custom hook to use the GameState context
export const useGameState = () => useContext(GameStateContext)

// Provider component to wrap around components that need access to the context
export const GameStateProvider = ({
  children,
}: {
  children: React.ReactNode
}) => {
  const { publicKey } = useWallet()
  const { fetchBalance } = useBalance()

  // Player's Game state account PDA
  const [gameStatePDA, setGameStatePDA] = useState<PublicKey | undefined>()
  // Player's Game state account data (deserialized account data)
  const [gameStateData, setGameStateData] = useState<any>()
  // Message to display
  const [message, setMessage] = useState("Play the game!")
  // Loading state
  const [isLoading, setIsLoading] = useState(false)

  // Function to reset state variables
  const reset = () => {
    setGameStatePDA(undefined)
    setGameStateData(undefined)
    setIsLoading(false)
    setMessage("Play the game!")
  }

  // Function to fetch the game state
  const fetchGameState = useCallback(async (gameStatePDA: PublicKey) => {
    try {
      const data = await program.account.gameState.fetch(gameStatePDA)
      setGameStateData(data)
    } catch (error) {
      console.log("Error fetching game state:", error)
    }
  }, [])

  // Function to setup game state
  const setup = useCallback(async () => {
    if (!publicKey) {
      reset()
      return
    }

    // Get the player's Game state account PDA
    const [gameStatePDA] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("GAME"), publicKey.toBytes()],
      program.programId
    )
    setGameStatePDA(gameStatePDA)

    await fetchGameState(gameStatePDA)
  }, [publicKey])

  useEffect(() => {
    setup()
  }, [setup])

  // Function to handle account change
  const handleAccountChange = useCallback(
    async (accountInfo: AccountInfo<Buffer>) => {
      let data
      try {
        // deserialize the game state account data
        data = program.coder.accounts.decode("gameState", accountInfo.data)
      } catch (error) {
        // expect error if the game state account does not exist (has not been created or has been closed)
        console.error("Error decoding account data:", error)
      }

      // If the account data is undefined, reset the game state
      if (!data) {
        setGameStateData(undefined)
        setMessage("Play the game!")
        return
      }

      // If result is 0 and guess is 0, the game account has just been initialized
      if (data.result == 0 && data.guess == 0) {
        setGameStateData(data)
      } else if (data.result == 0) {
        // If result is 0 and guess is not 0, request randomness has been called
        setIsLoading(true)
      } else {
        // If result is not 0, consume randomness has been called
        setIsLoading(false)

        // Set the message to display based on the result
        const tossResult = data.result == 1 ? "Heads" : "Tails"
        const winOrLose =
          data.result == data.guess ? "You won 😄" : "You lost 😕"
        setMessage(`${tossResult}! ${winOrLose}`)

        // Fetch balance to update the balances
        await fetchBalance()
      }
    },
    [gameStatePDA, gameStateData]
  )

  useEffect(() => {
    if (!gameStatePDA) return

    // Subscribe to the games state PDA account change
    const subscriptionId = connection.onAccountChange(
      gameStatePDA,
      handleAccountChange
    )

    return () => {
      // Unsubscribe from the account change subscription when the component unmounts
      connection.removeAccountChangeListener(subscriptionId)
    }
  }, [gameStatePDA, gameStateData])

  return (
    <GameStateContext.Provider
      value={{
        gameStatePDA,
        gameStateData,
        isLoading,
        message,
      }}
    >
      {children}
    </GameStateContext.Provider>
  )
}
