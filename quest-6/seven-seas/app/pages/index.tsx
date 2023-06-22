import { Box, Flex, Spacer, VStack } from "@chakra-ui/react"
import WalletMultiButton from "@/components/WalletMultiButton"
import ResetButton from "@/components/ResetButton"
import GameBoard from "@/components/GameBoard"
import ShipActionButtons from "@/components/ShipActionButtons"
import ShipSetupButtons from "@/components/ShipSetupButtons"
import { useWallet } from "@solana/wallet-adapter-react"

export default function Home() {
  const { publicKey } = useWallet()

  return (
    <Box>
      <Flex px={4} py={4}>
        <Spacer />
        <WalletMultiButton />
      </Flex>

      <VStack>
        <GameBoard />
        {publicKey && (
          <>
            <Box height="5px" />
            <ShipActionButtons />
            <Box height="5px" />
            <ShipSetupButtons />
            <Box height="5px" />
            <ResetButton />
            {/* <InitializeButton /> */}
            <Box height="5px" />
          </>
        )}
      </VStack>
    </Box>
  )
}
