import { createContext, useContext, useEffect, useState } from "react"
import { useWallet } from "@solana/wallet-adapter-react"
import { useProgram } from "@/contexts/ProgramContext"
import { PublicKey } from "@solana/web3.js"
import { getAssociatedTokenAddressSync } from "@solana/spl-token"
import { goldTokenMint, cannonTokenMint, rumTokenMint } from "@/utils/constants"

// Define the structure of the Accounts context state
// Theses are addresses that will be used to interact with the program
type AccountsContextType = {
  playerGoldTokenAccount: PublicKey | null
  playerCannonTokenAccount: PublicKey | null
  playerRumTokenAccount: PublicKey | null
  playerShipPDA: PublicKey | null
  playerShipData: any
}

// Create the context with default null values
const AccountsContext = createContext<AccountsContextType>({
  playerGoldTokenAccount: null,
  playerCannonTokenAccount: null,
  playerRumTokenAccount: null,
  playerShipPDA: null,
  playerShipData: null,
})

// Custom hook to use the Accounts context
export const useAccounts = () => useContext(AccountsContext)

// Provider component to wrap around components that need access to the context
export const AccountsProvider = ({
  children,
}: {
  children: React.ReactNode
}) => {
  const { publicKey } = useWallet()
  const { program } = useProgram()

  // State variable to hold the account addresses
  const [accounts, setAccounts] = useState<AccountsContextType>({
    playerGoldTokenAccount: null,
    playerCannonTokenAccount: null,
    playerRumTokenAccount: null,
    playerShipPDA: null,
    playerShipData: null,
  })

  // Fetch the account addresses
  useEffect(() => {
    const setup = async () => {
      if (!publicKey || !program) return

      const playerGoldTokenAccount = getAssociatedTokenAddressSync(
        goldTokenMint,
        publicKey!
      )

      const playerCannonTokenAccount = getAssociatedTokenAddressSync(
        cannonTokenMint,
        publicKey!
      )

      const playerRumTokenAccount = getAssociatedTokenAddressSync(
        rumTokenMint,
        publicKey!
      )

      const [playerShipPDA] = PublicKey.findProgramAddressSync(
        [Buffer.from("ship"), publicKey!.toBuffer()],
        program!.programId
      )

      let playerShipData = null

      try {
        playerShipData = await program.account.ship.fetch(playerShipPDA)
      } catch (error) {
        console.log("No ship data found")
      }

      setAccounts({
        playerGoldTokenAccount,
        playerCannonTokenAccount,
        playerRumTokenAccount,
        playerShipPDA,
        playerShipData,
      })
    }

    setup()
  }, [publicKey, program])

  return (
    <AccountsContext.Provider value={accounts}>
      {children}
    </AccountsContext.Provider>
  )
}
