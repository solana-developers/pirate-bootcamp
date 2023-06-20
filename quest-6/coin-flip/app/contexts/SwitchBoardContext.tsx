import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
} from "react"
import * as sbv2 from "@switchboard-xyz/solana.js"
import { provider } from "@/utils/anchor"

// Define the structure of the Switchboard context state
type SwitchboardContextType = {
  switchboard: {
    program: sbv2.SwitchboardProgram
    queueAccount: sbv2.QueueAccount
    queueAccountData: sbv2.types.OracleQueueAccountData
  } | null
}

// Create the context with default values
const SwitchboardContext = createContext<SwitchboardContextType>({
  switchboard: null,
})

// Custom hook to use the Switchboard context
export const useSwitchboard = () => useContext(SwitchboardContext)

// Provider component to wrap around components that need access to the context
export const SwitchboardProvider = ({
  children,
}: {
  children: React.ReactNode
}) => {
  // State variable to hold the switchboard values
  const [switchboard, setSwitchboard] = useState<{
    program: sbv2.SwitchboardProgram
    queueAccount: sbv2.QueueAccount
    queueAccountData: sbv2.types.OracleQueueAccountData
  } | null>(null)

  const fetchSwitchboard = useCallback(async () => {
    // Get switchboard program
    const switchboardProgram = await sbv2.SwitchboardProgram.fromProvider(
      provider
    )

    // Get switchboard queue account and queue account data (deserialized account data)
    const [queueAccount, queueAccountData] = await sbv2.QueueAccount.load(
      switchboardProgram,
      "uPeRMdfPmrPqgRWSrjAnAkH78RqAhe5kXoW6vBYRqFX" // DEVNET_PERMISSIONLESS_QUEUE
    )

    setSwitchboard({
      program: switchboardProgram,
      queueAccount: queueAccount,
      queueAccountData: queueAccountData,
    })
  }, [])

  // Effect to fetch switchboard when the component mounts
  useEffect(() => {
    fetchSwitchboard()
  }, [fetchSwitchboard])

  return (
    <SwitchboardContext.Provider
      value={{
        switchboard,
      }}
    >
      {children}
    </SwitchboardContext.Provider>
  )
}
