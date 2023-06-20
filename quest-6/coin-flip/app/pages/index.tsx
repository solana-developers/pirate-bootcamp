import { VStack, Box, Flex, Spacer, Heading, Text } from "@chakra-ui/react"
import { BeatLoader, PacmanLoader } from "react-spinners"
import { PublicKey } from "@solana/web3.js"
import WalletMultiButton from "@/components/WalletMultiButton"
import Balance from "@/components/Balance"
import InitPlayerButton from "@/components/InitPlayerButton"
import CloseButton from "@/components/CloseButton"
import CoinTossButtons from "@/components/CoinTossButtons"
import { useWallet } from "@solana/wallet-adapter-react"
import { useGameState } from "@/contexts/GameStateContext"
import { GameState } from "@/utils/anchor"

interface GameStateProps {
  gameStateData: GameState | undefined
}

interface GameProps extends GameStateProps {
  isLoading: boolean
  message: string
  publicKey: PublicKey | null
}

// Component to display loading state
const LoadingIndicator = () => (
  <VStack>
    <Text>Waiting for Oracle to respond...</Text>
    <PacmanLoader size={24} speedMultiplier={2.5} />
    <BeatLoader size={20} />
  </VStack>
)

// If the player has a game state account, display balances, coin toss buttons, and close button
// Otherwise, display the init player button
const GameState = ({ gameStateData }: GameStateProps) =>
  gameStateData ? (
    <>
      <Balance />
      <CoinTossButtons />
      <CloseButton />
    </>
  ) : (
    <InitPlayerButton />
  )

// Main Game component
const Game = ({ isLoading, message, gameStateData, publicKey }: GameProps) =>
  publicKey ? (
    <>
      {isLoading ? <LoadingIndicator /> : <Text>{message}</Text>}
      <GameState gameStateData={gameStateData} />
    </>
  ) : (
    <Text>Connect your wallet to play</Text>
  )

// Home component contains all other components and provides the structure of the page
const Home = () => {
  const { isLoading, message, gameStateData } = useGameState()
  const { publicKey } = useWallet()

  return (
    <Box>
      <Flex px={4} py={4}>
        <Spacer />
        <WalletMultiButton />
      </Flex>

      <VStack justifyContent="center" alignItems="center" height="75vh">
        <VStack>
          <Heading>Coin Flip</Heading>
          <Game
            isLoading={isLoading}
            message={message}
            gameStateData={gameStateData}
            publicKey={publicKey}
          />
        </VStack>
      </VStack>
    </Box>
  )
}

export default Home
