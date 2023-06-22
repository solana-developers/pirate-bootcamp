import { useEffect, useState } from "react"
import { Box, Flex, Text, VStack } from "@chakra-ui/react"
import { useConnection, useWallet } from "@solana/wallet-adapter-react"
import { useProgram } from "@/contexts/ProgramContext"
import { AccountInfo, PublicKey } from "@solana/web3.js"
import { IdlAccounts, Idl } from "@coral-xyz/anchor"
import { gameDataAccount } from "@/utils/constants"
import {
  StarIcon,
  ArrowBackIcon,
  ArrowDownIcon,
  ArrowForwardIcon,
  ArrowUpIcon,
} from "@chakra-ui/icons"

// Not sure if this does anything
type GameState = IdlAccounts<Idl>["gameDataAccount"]

// Not sure how to implement render "shooting"
const GameBoard = () => {
  const { publicKey } = useWallet()
  const { connection } = useConnection()
  // Program from context
  const { program } = useProgram()

  const [state, setState] = useState<GameState>()

  // Fetch the game state account on load
  useEffect(() => {
    if (!program) return
    const fetchBoard = async () => {
      const gameState = await program!.account.gameDataAccount.fetch(
        gameDataAccount
      )
      console.log(JSON.stringify(gameState, null, 2))
      // console.log(gameState.board)
      setState(gameState)
    }

    fetchBoard()
  }, [program])

  const handleAccountChange = (accountInfo: AccountInfo<Buffer>) => {
    try {
      // deserialize the game state account data
      const data = program?.coder.accounts.decode(
        "gameDataAccount",
        accountInfo.data
      )
      setState(data)
    } catch (error) {
      console.log("Error decoding account data:", error)
    }
  }

  useEffect(() => {
    if (!program) return

    const subscriptionId = connection.onAccountChange(
      gameDataAccount,
      handleAccountChange
    )

    return () => {
      // Unsubscribe from the account change subscription when the component unmounts
      connection.removeAccountChangeListener(subscriptionId)
    }
  }, [program])

  const renderTile = (row: any, colIndex: number, publicKey: PublicKey) => {
    switch (row[colIndex].state) {
      case 0:
        return null
      case 1:
        let color =
          row[colIndex].player == publicKey?.toBase58() ? "red.500" : "black"
        return (
          <Box>
            {row[colIndex].lookDirection === 0 && <ArrowUpIcon color={color} />}
            {row[colIndex].lookDirection === 1 && (
              <ArrowForwardIcon color={color} />
            )}
            {row[colIndex].lookDirection === 2 && (
              <ArrowDownIcon color={color} />
            )}
            {row[colIndex].lookDirection === 3 && (
              <ArrowBackIcon color={color} />
            )}
            <Text>{Number(row[colIndex].health)}</Text>
          </Box>
        )
      case 2:
        return <StarIcon />
      default:
        return null
    }
  }

  return (
    <>
      <Box>
        {state?.board[0].map((_: any, colIndex: number) => (
          <Flex key={colIndex}>
            {state.board.map((row: any[], rowIndex: number) => (
              <Box
                key={rowIndex}
                w="40px"
                h="40px"
                border="1px"
                borderColor="black"
                display="flex"
                alignItems="center"
                justifyContent="center"
              >
                {renderTile(row, colIndex, publicKey!)}
              </Box>
            ))}
          </Flex>
        ))}
      </Box>
    </>
  )
}

export default GameBoard
