import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
} from "react"
import { LAMPORTS_PER_SOL } from "@solana/web3.js"
import { useWallet } from "@solana/wallet-adapter-react"
import { provider, solVaultPDA } from "@/utils/anchor"

// Define the structure of the Balance context state
type BalanceContextType = {
  vaultBalance: number
  playerBalance: number
  fetchBalance: () => Promise<void>
}

// Create the context with default values
const BalanceContext = createContext<BalanceContextType>({
  vaultBalance: 0,
  playerBalance: 0,
  fetchBalance: async () => {},
})

// Custom hook to use the Balance context
export const useBalance = () => useContext(BalanceContext)

// Provider component to wrap around components that need access to the context
export const BalanceProvider = ({
  children,
}: {
  children: React.ReactNode
}) => {
  const { publicKey } = useWallet()

  // Game SOL Vault balance
  const [vaultBalance, setVaultBalance] = useState(0)
  // Player balance
  const [playerBalance, setPlayerBalance] = useState(0)

  // Fetch balances
  const fetchBalance = useCallback(async () => {
    if (!publicKey) return
    const vaultBalance = await provider.connection.getBalance(solVaultPDA)
    setVaultBalance(parseFloat((vaultBalance / LAMPORTS_PER_SOL).toFixed(1)))

    const playerBalance = await provider.connection.getBalance(publicKey)
    setPlayerBalance(parseFloat((playerBalance / LAMPORTS_PER_SOL).toFixed(1)))
  }, [publicKey])

  // Effect to fetch balance when the component mounts
  useEffect(() => {
    fetchBalance()
  }, [fetchBalance])

  return (
    <BalanceContext.Provider
      value={{
        vaultBalance,
        playerBalance,
        fetchBalance,
      }}
    >
      {children}
    </BalanceContext.Provider>
  )
}
