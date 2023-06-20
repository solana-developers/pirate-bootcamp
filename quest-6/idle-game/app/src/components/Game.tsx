import {
  useAnchorWallet,
  useConnection,
  useWallet,
} from "@solana/wallet-adapter-react"
import { FC, useCallback, useEffect, useState } from "react"
import { notify } from "../utils/notifications"
import { AnchorProvider, Program, setProvider } from "@coral-xyz/anchor"
import { IdleGame, IDL } from "../idl/idle_game"
import { IDLE_GAME_PROGRAM_ID } from "utils/anchor"
import { AccountInfo, LAMPORTS_PER_SOL, PublicKey, SystemProgram } from "@solana/web3.js"
import Image from "next/image"
import * as anchor from "@project-serum/anchor"
import { ClockworkProvider } from "@clockwork-xyz/sdk"

type GameData = {
  wood: number
  lumberjacks: number
  gold: number
  teethUpgrade: number
  updatedAt: number
}

type Costs = {
  woodPerTick: number
  lumberjackCost: number
  teethUpgradeCost: number
  goldPerWood: number
}

const WOOD_PER_SELL: number = 10
const GOLD_PER_WOOD_BASE: number = 5
const GOLD_PER_WOOD_TEETH_MULTIPLIER: number = 2

// Upgrade Teeth
const TEETH_UPGRADE_BASE_COST: number = 50
const TEETH_UPGRADE_COST_MULTIPLIER: number = 1

// Buy Lumberjack
const LUMBERJACK_BASE_COST: number = 20
const LUMBERJACK_COST_MULTIPLIER: number = 5

// Thread tick time
const THREAD_TICK_TIME_IN_SECONDS: number = 10

export const Game: FC = () => {
  const { connection } = useConnection()
  const { publicKey, sendTransaction } = useWallet()
  const [gameState, setGameState] = useState<GameData | null>(null)
  const [threadAccountBalance, setThreadAccountBalance] = useState<number | null>(null)
  const [nextThreadTick, setNextThreadTick] = useState<number>(THREAD_TICK_TIME_IN_SECONDS)
  const [costs, setCosts] = useState<Costs>({
    woodPerTick: 1,
    lumberjackCost: LUMBERJACK_BASE_COST,
    teethUpgradeCost: TEETH_UPGRADE_BASE_COST,
    goldPerWood: GOLD_PER_WOOD_BASE,
  })
  const [gameDataPDA, setGameDataPDA] = useState<PublicKey | null>(null)
  const [threadId, setThreadId] = useState<string | null>(null)
  const [threadAuthority, setThreadAuthority] = useState<PublicKey>(null)
  const [threadAddress, setThreadAddress] = useState<PublicKey>(null)
  const wallet = useAnchorWallet()

  const provider = new AnchorProvider(connection, wallet, {})
  setProvider(provider)
  const program = new Program<IdleGame>(IDL, IDLE_GAME_PROGRAM_ID, provider)
  const clockworkProvider = ClockworkProvider.fromAnchorProvider(provider)

  useEffect(() => {
    if (gameState === null) {
      return
    }
    // console.log("gameData", JSON.stringify(gameState, null, 2))

    const newCosts = {
      woodPerTick: gameState.lumberjacks,
      lumberjackCost:
        LUMBERJACK_BASE_COST +
        gameState.lumberjacks * LUMBERJACK_COST_MULTIPLIER,
      teethUpgradeCost:
        TEETH_UPGRADE_BASE_COST +
        gameState.teethUpgrade * TEETH_UPGRADE_COST_MULTIPLIER,
      goldPerWood:
        GOLD_PER_WOOD_BASE +
        gameState.teethUpgrade * GOLD_PER_WOOD_TEETH_MULTIPLIER,
    }

    setCosts(newCosts)
  }, [gameState])

  useEffect(() => {
    const interval = setInterval(async () => {
      if (gameState == null || gameState.updatedAt == undefined) {
        return
      }
      const lastLoginTime = gameState.updatedAt
      const slot = await connection.getSlot()
      const timestamp = await connection.getBlockTime(slot)
      let timePassed = timestamp - lastLoginTime

      let nextEnergyIn = Math.floor(THREAD_TICK_TIME_IN_SECONDS - timePassed)
      setNextThreadTick(nextEnergyIn)
    }, 1000)

    return () => clearInterval(interval)
  }, [gameState, nextThreadTick])

  // Update game data PDA and other keys every time the public key changes
  useEffect(() => {
    if (publicKey === null) {
      setGameDataPDA(null)
      setThreadAuthority(null)
      setThreadAddress(null)
      return
    }
    const [pda] = PublicKey.findProgramAddressSync(
      [Buffer.from("gameData", "utf8"), publicKey.toBuffer()],
      new PublicKey(IDLE_GAME_PROGRAM_ID)
    )

    let threadId = "gameData-" + wallet.publicKey.toBase58().substring(0, 6);
    setThreadId(threadId);

    const [threadAuthority, bump] = PublicKey.findProgramAddressSync(
      [anchor.utils.bytes.utf8.encode("authority"), publicKey.toBuffer()], // 👈 make sure it matches on the prog side
      program.programId
    )
    setThreadAuthority(threadAuthority);

    const [threadAddress, threadBump] = clockworkProvider.getThreadPDA(
      threadAuthority,
      threadId
    )
    setThreadAddress(threadAddress);
    setGameDataPDA(pda)
  }, [publicKey])

  // Get game data every time the PDA changes
  useEffect(() => {
    if (!publicKey || !gameDataPDA) {
      setGameState(null)
      return
    }

    program.account.gameData
      .fetch(gameDataPDA)
      .then((data) => {
        setGameState(data)
      })
      .catch((error) => {
        window.alert("No player data found, please init!")
        return
      })

    connection.getAccountInfo(threadAddress).then((data) => {   
      if (data == null) {
        setThreadAccountBalance(0);
      } else {
        setThreadAccountBalance(data.lamports / LAMPORTS_PER_SOL);
      }
    });

    const subscriptionID = connection.onAccountChange(
      gameDataPDA,
      (account) => {
        setGameState(program.coder.accounts.decode("gameData", account.data))
      }
    )

    return () => {
      connection.removeAccountChangeListener(subscriptionID)
    }
  }, [gameDataPDA, publicKey])

  const onInitClick = useCallback(async () => {
    if (!publicKey) {
      return
    }

    console.log("threadAddress", threadAddress.toBase58())
    console.log("threadAuthority", threadAuthority.toBase58())

    try {
      const transaction = await program.methods
        .initialize(Buffer.from(threadId))
        .accounts({
          payer: wallet.publicKey,
          systemProgram: SystemProgram.programId,
          clockworkProgram: clockworkProvider.threadProgram.programId,
          thread: threadAddress,
          threadAuthority: threadAuthority,
          gameData: gameDataPDA,
        })
        .transaction()

      const txSig = await sendTransaction(transaction, connection, {
        skipPreflight: true,
      })
      await connection.confirmTransaction(txSig, "confirmed")

      notify({ type: "success", message: "Chopped tree!", txid: txSig })
    } catch (error: any) {
      logError(error?.message)
    }
  }, [gameDataPDA, connection])

  const onUpgradeTeethClick = useCallback(async () => {
    if (!publicKey) {
      return
    }

    try {
      const transaction = await program.methods
        .upgradeTeeth()
        .accounts({
          gameData: gameDataPDA,
          signer: publicKey,
        })
        .transaction()

      const txSig = await sendTransaction(transaction, connection, {
        skipPreflight: true,
      })
      await connection.confirmTransaction(txSig, "confirmed")
    } catch (error: any) {
      logError(error?.message)
    }
  }, [gameDataPDA, connection])

  const onBuyLumberjackClick = useCallback(async () => {
    if (!publicKey) {
      return
    }

    try {
      const transaction = await program.methods
        .buyLumberjack()
        .accounts({
          gameData: gameDataPDA,
          signer: publicKey,
        })
        .transaction()

      const txSig = await sendTransaction(transaction, connection, {
        skipPreflight: true,
      })
      await connection.confirmTransaction(txSig, "confirmed")
    } catch (error: any) {
      console.log(JSON.stringify(error))
      logError(error?.message)
    }
  }, [gameDataPDA, connection])

  const onTradeWoodForGoldClick = useCallback(async () => {
    if (!publicKey) {
      return
    }

    try {
      const transaction = await program.methods
        .tradeWoodForGold()
        .accounts({
          gameData: gameDataPDA,
          signer: publicKey,
        })
        .transaction()

      const txSig = await sendTransaction(transaction, connection, {
        skipPreflight: true,
      })
      await connection.confirmTransaction(txSig, "confirmed")
    } catch (error: any) {
      logError(error?.message)
    }
  }, [gameDataPDA, connection])

  const onStopThreadClick = useCallback(async () => {
    if (!publicKey) {
      return
    }

    console.log("threadAddress", threadAddress.toBase58())
    console.log("threadAuthority", threadAuthority.toBase58())

    try {
      const transaction = await program.methods
        .reset()
        .accounts({
          payer: publicKey,
          gameData: gameDataPDA,
          clockworkProgram: clockworkProvider.threadProgram.programId,
          threadAuthority: threadAuthority,
          thread: threadAddress,
        })
        .transaction()

      const txSig = await sendTransaction(transaction, connection, {
        skipPreflight: true,
      })
      await connection.confirmTransaction(txSig, "confirmed")
    } catch (error: any) {
      logError(error?.message)
    }
  }, [gameDataPDA, connection])

  function logError(error: string) {
    notify({
      type: "error",
      message: `Error!`,
      description: error,
      txid: "",
    })
    console.log("error", `Error! ${error}`, "")
  }

  return (
    <div>
      <div>
        {gameState && costs && publicKey && (
          <>
            <div className="overflow-auto w-3/5 space-x-3 flex items-center">
              <p className="text-xl">{gameState.gold.toString()}</p>
              <Image
                src="/coinpile.png"
                alt="Energy Icon"
                width={64}
                height={64}
              />

              <p className="text-xl">{gameState.wood.toString()}</p>
              <Image src="/Wood.png" alt="Wood Icon" width={64} height={64} />
              <p className="text-xl">
                {"Next " +
                  costs.woodPerTick +
                  " wood in: " +
                  nextThreadTick.toString()}
              </p>
            </div>
          </>
        )}
        {!publicKey && "Connect to dev net wallet!"}

        {!gameState && publicKey && (
          <button
            className="px-8 m-2 btn animate-pulse bg-gradient-to-br from-indigo-500 to-fuchsia-500 hover:from-white hover:to-purple-300 text-black"
            onClick={onInitClick}
          >
            <span>Init </span>
          </button>
        )}

        {gameState && costs && publicKey && (
          <>
            <div className=" flex flex-row relative group items-center">
              {Array.from({ length: gameState.lumberjacks }, (_, i) => (
                <Image
                  key={i}
                  src="/Beaver.png"
                  alt="Energy Icon"
                  width={64}
                  height={64}
                />
              ))}
            </div>
            <div className="relative group items-center">
              <button
                className="px-8 m-2 btn bg-gradient-to-br from-indigo-500 to-fuchsia-500 hover:from-white hover:to-purple-300 text-black"
                onClick={onUpgradeTeethClick}
              >
                <span>
                  Upgrade Teeth ({gameState.teethUpgrade.toString()}) (
                  {costs.teethUpgradeCost} gold)
                </span>
              </button>

              <button
                className="px-8 m-2 btn bg-gradient-to-br from-indigo-500 to-fuchsia-500 hover:from-white hover:to-purple-300 text-black"
                onClick={onBuyLumberjackClick}
              >
                <span>
                  Buy Lumberjack ({gameState.lumberjacks.toString()}) (
                  {costs.lumberjackCost} gold)
                </span>
              </button>
              <button
                className="px-8 m-2 btn bg-gradient-to-br from-indigo-500 to-fuchsia-500 hover:from-white hover:to-purple-300 text-black"
                onClick={onTradeWoodForGoldClick}
              >
                <span>
                  Trade {WOOD_PER_SELL} Wood for {costs.goldPerWood} Gold
                </span>
              </button>
              <button
                className="px-8 m-2 btn bg-gradient-to-br from-indigo-500 to-fuchsia-500 hover:from-white hover:to-purple-300 text-black"
                onClick={onStopThreadClick}
              >
                <span>
                  Stop Thread
                </span>
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  )
}
