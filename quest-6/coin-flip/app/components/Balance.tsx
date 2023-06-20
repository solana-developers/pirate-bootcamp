import { useEffect } from "react"
import { Text } from "@chakra-ui/react"
import { LAMPORTS_PER_SOL } from "@solana/web3.js"
import { useBalance } from "@/contexts/BalanceContext"
import { useWallet } from "@solana/wallet-adapter-react"
import { connection, solVaultPDA } from "@/utils/anchor"

const Balance = () => {
  const { publicKey } = useWallet()
  const { vaultBalance, playerBalance } = useBalance()

  useEffect(() => {
    // Request airdrop if the vault balance is less than 1 SOL
    const requestAirdrop = async () => {
      try {
        if (vaultBalance < 1 && vaultBalance !== 0) {
          await connection.requestAirdrop(solVaultPDA, 2 * LAMPORTS_PER_SOL)
        }
      } catch (error) {
        console.log(error)
      }
    }

    requestAirdrop()
  }, [])

  return (
    <>
      {publicKey && (
        <>
          <Text>Vault Balance: {vaultBalance}</Text>
          <Text>Player Balance: {playerBalance}</Text>
        </>
      )}
    </>
  )
}

export default Balance
