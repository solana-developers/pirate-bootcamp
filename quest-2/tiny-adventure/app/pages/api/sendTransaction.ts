import { Keypair, Transaction } from "@solana/web3.js"
import type { NextApiRequest, NextApiResponse } from "next"
import {
  program,
  globalLevel1GameDataAccount,
  connection,
} from "../../utils/anchor"

const burner = JSON.parse(process.env.BURNER_KEY ?? "") as number[]
const burnerKeypair = Keypair.fromSecretKey(Uint8Array.from(burner))

const messages: Record<string, string> = {
  initialize: "Initializing",
  moveLeft: "Going left",
  moveRight: "Going right",
};

export default async function handler(
  req: NextApiRequest,
  res: NextApiResponse
) {
  let instruction: string | string[] = req.query.instruction || req.body.instruction;

  if (!instruction) {
    return res.status(400).json({ message: "Invalid instruction" });
  }

  if (Array.isArray(instruction)) {
    instruction = instruction[0];
  }

  const message = messages[instruction];

  if (!message) {
    return res.status(400).json({ message: "Invalid instruction" });
  }
  
  let transaction: Transaction

  switch (instruction) {
    case "initialize":
      transaction = await program.methods
        .initialize()
        .accounts({
          newGameDataAccount: globalLevel1GameDataAccount,
          signer: burnerKeypair.publicKey,
        })
        .transaction()
      break
    case "moveRight":
      transaction = await program.methods
        .moveRight()
        .accounts({
          gameDataAccount: globalLevel1GameDataAccount,
        })
        .transaction()
      break
    case "moveLeft":
      transaction = await program.methods
        .moveLeft()
        .accounts({
          gameDataAccount: globalLevel1GameDataAccount,
        })
        .transaction()
      break
    default:
      return res.status(400).json({ message: "Invalid instruction" })
  }

  const txSig = await connection.sendTransaction(transaction, [burnerKeypair])
  console.log("Transaction sent:", txSig)

  const { blockhash, lastValidBlockHeight } =
    await connection.getLatestBlockhash()
  const confirmation = await connection.confirmTransaction({
    blockhash,
    lastValidBlockHeight,
    signature: txSig,
  })

  if (!confirmation) {
    throw new Error("Transaction confirmation failed.")
  }

  if (message) {
    res.status(200).json({ message })
  } else {
    res.status(400).json({ message: "Invalid request body" })
  }
}
