import {
  VStack,
  HStack,
  Button,
  Text,
  Box,
  Flex,
  Spacer,
  Heading,
} from "@chakra-ui/react"
import { useEffect, useState } from "react"
import { PublicKey, Transaction } from "@solana/web3.js"
import { useWallet } from "@solana/wallet-adapter-react"
import WalletMultiButton from "@/components/WalletMultiButton"
import {
  program,
  connection,
  globalLevel1GameDataAccount,
} from "@/utils/anchor"

type GameDataAccount = {
  playerPosition: number
}

export default function Home() {
  const { publicKey, sendTransaction } = useWallet()

  const [loadingInitialize, setLoadingInitialize] = useState(false)
  const [loadingRight, setLoadingRight] = useState(false)
  const [loadingLeft, setLoadingLeft] = useState(false)

  const [playerPosition, setPlayerPosition] = useState("........")
  const [message, setMessage] = useState("")
  const [gameDataAccount, setGameDataAccount] =
    useState<GameDataAccount | null>(null)

  const updatePlayerPosition = (position: number) => {
    switch (position) {
      case 0:
        setPlayerPosition("o........")
        setMessage("A journey begins...")
        break
      case 1:
        setPlayerPosition("....o....")
        setMessage("")
        break
      case 2:
        setPlayerPosition("......o..")
        setMessage("")
        break
      case 3:
        setPlayerPosition(".........\\o/")
        setMessage("You have reached the end! Super!")
        break
      default:
        console.log("Invalid player position")
        break
    }
  }

  useEffect(() => {
    if (gameDataAccount && gameDataAccount.playerPosition != null) {
      updatePlayerPosition(gameDataAccount.playerPosition)
    } else {
      console.log("gameDataAccount or playerPosition is null")
    }
  }, [gameDataAccount])

  useEffect(() => {
    fetchData(globalLevel1GameDataAccount)
  }, [])

  async function handleClickGetData() {
    fetchData(globalLevel1GameDataAccount)
  }

  async function handleClickInitialize() {
    if (publicKey) {
      const transaction = program.methods
        .initialize()
        .accounts({
          newGameDataAccount: globalLevel1GameDataAccount,
          signer: publicKey,
        })
        .transaction()

      await sendAndConfirmTransaction(() => transaction, setLoadingInitialize)
    } else {
      try {
        const response = await fetch("/api/sendTransaction", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ instruction: "initialize" }),
        })
        const data = await response.json()
        console.log(data)
      } catch (error) {
        console.error(error)
      }
    }
  }

  async function handleClickRight() {
    if (publicKey) {
      const transaction = program.methods
        .moveRight()
        .accounts({
          gameDataAccount: globalLevel1GameDataAccount,
        })
        .transaction()

      await sendAndConfirmTransaction(() => transaction, setLoadingRight)
    } else {
      try {
        const response = await fetch("/api/sendTransaction", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ instruction: "moveRight" }),
        })
        const data = await response.json()
        console.log(data)
      } catch (error) {
        console.error(error)
      }
    }
  }

  async function handleClickLeft() {
    if (publicKey) {
      const transaction = program.methods
        .moveLeft()
        .accounts({
          gameDataAccount: globalLevel1GameDataAccount,
        })
        .transaction()

      await sendAndConfirmTransaction(() => transaction, setLoadingLeft)
    } else {
      try {
        const response = await fetch("/api/sendTransaction", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ instruction: "moveLeft" }),
        })
        setLoadingLeft(false)
        const data = await response.json()
        console.log(data)
      } catch (error) {
        console.error(error)
      }
    }
  }

  async function sendAndConfirmTransaction(
    transactionBuilder: () => Promise<Transaction>,
    setLoading: (loading: boolean) => void
  ) {
    if (!publicKey || !program || !connection) return

    setLoading(true)

    try {
      const tx = await transactionBuilder()
      const txSig = await sendTransaction(tx, connection)

      const { blockhash, lastValidBlockHeight } =
        await connection.getLatestBlockhash()

      await connection.confirmTransaction({
        blockhash,
        lastValidBlockHeight,
        signature: txSig,
      })

      setLoading(false)
    } catch (error) {
      console.error("Error processing transaction:", error)
      setLoading(false)
    }
  }

  const fetchData = async (pda: PublicKey) => {
    console.log("Fetching GameDataAccount state...")

    try {
      const account = await program.account.gameDataAccount.fetch(pda)
      console.log(JSON.stringify(account, null, 2))
      setGameDataAccount(account)
    } catch (error) {
      console.log(`Error fetching GameDataAccount state: ${error}`)
    }
  }

  useEffect(() => {
    if (!globalLevel1GameDataAccount) return

    const subscriptionId = connection.onAccountChange(
      globalLevel1GameDataAccount,
      (accountInfo) => {
        const decoded = program.coder.accounts.decode(
          "gameDataAccount",
          accountInfo.data
        )
        console.log("New player position via socket", decoded.playerPosition)
        setGameDataAccount(decoded)
      }
    )

    return () => {
      connection.removeAccountChangeListener(subscriptionId)
    }
  }, [connection, globalLevel1GameDataAccount, program])

  return (
    <Box>
      <Flex px={4} py={4}>
        <Spacer />
        <WalletMultiButton />
      </Flex>
      <VStack justifyContent="center" alignItems="center" height="75vh">
        <VStack>
          <Heading fontSize="xl">{message}</Heading>
          <Text fontSize="6xl">{playerPosition}</Text>
          <HStack>
            <Button
              width="100px"
              isLoading={loadingLeft}
              onClick={handleClickLeft}
            >
              Move Left
            </Button>
            <Button width="100px" onClick={handleClickGetData}>
              Get Data
            </Button>
            <Button
              width="100px"
              isLoading={loadingRight}
              onClick={handleClickRight}
            >
              Move Right
            </Button>
          </HStack>
          <Button
            width="100px"
            isLoading={loadingInitialize}
            onClick={handleClickInitialize}
          >
            Initialize
          </Button>
        </VStack>
      </VStack>
    </Box>
  )
}
