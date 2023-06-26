// @ts-check
import {
  ASSOCIATED_TOKEN_PROGRAM_ID,
  TOKEN_PROGRAM_ID,
  getOrCreateAssociatedTokenAccount,
} from '@solana/spl-token';
import {
  Connection,
  Keypair,
  PublicKey,
  SystemProgram,
  Transaction,
  TransactionInstruction,
} from '@solana/web3.js';
import dotenv from 'dotenv';
import { SEVEN_SEAS_PROGRAM, GOLD_TOKEN_MINT } from '../constants';
import { sha256 } from 'js-sha256';

// Load environment variables from .env
dotenv.config();

const connection = new Connection('https://api.devnet.solana.com', 'confirmed');
const payer = Keypair.fromSecretKey(Uint8Array.from(JSON.parse(process.env.PAYER)));

/**
 * @typedef {import('@vercel/node').VercelResponse} VercelResponse
 * @typedef {import('@vercel/node').VercelRequest} VercelRequest
 *
 * @param {VercelRequest} request
 * @param {VercelResponse} response
 * @returns {Promise<VercelResponse>}
 * */
export default async function handler(request, response) {
  console.log('handling request', request.method);

  if (request.method === 'GET') {
    return handleGet(response);
  } else if (request.method === 'POST') {
    return await handlePost(request, response);
  } else {
    return response.status(405).json({ error: 'Method not allowed' });
  }
}

/**
 * @param {VercelResponse} response
 */
function handleGet(response) {
  return response.status(200).json({
    label: 'Chutulu Fire!',
    icon: 'https://github.com/solana-developers/pirate-bootcamp/blob/main/assets/kraken-1.png?raw=true',
  });
}

/**
 * @param {VercelRequest} request
 * @param {VercelResponse} response
 */
async function handlePost(request, response) {
  // get wallet address
  const player = new PublicKey(request.body.account);
  console.log('player', player.toBase58());

  // create chutulu instruction
  const chutuluIX = await createChutuluIx(player);

  // prepare transaction and serialize
  const transaction = await prepareTx(chutuluIX);

  return response.status(200).json({
    transaction,
    message: 'Chutulu Fire!',
  });
}

/**
 * @typedef {import('@solana/spl-token').Account} Account
 *
 * @param {PublicKey} player
 * @returns {Promise<TransactionInstruction>}
 */
async function createChutuluIx(player) {
  // get payer's token account
  const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
    connection,
    payer,
    GOLD_TOKEN_MINT,
    player,
  );

  // start: get program derived addresses
  const [level] = PublicKey.findProgramAddressSync([Buffer.from('level')], SEVEN_SEAS_PROGRAM);

  const [chestVault] = PublicKey.findProgramAddressSync(
    [Buffer.from('chestVault')],
    SEVEN_SEAS_PROGRAM,
  );

  const [gameActions] = PublicKey.findProgramAddressSync(
    [Buffer.from('gameActions')],
    SEVEN_SEAS_PROGRAM,
  );

  let [tokenAccountOwnerPda] = await PublicKey.findProgramAddressSync(
    [Buffer.from('token_account_owner_pda', 'utf8')],
    SEVEN_SEAS_PROGRAM,
  );

  let [tokenVault] = await PublicKey.findProgramAddressSync(
    [Buffer.from('token_vault', 'utf8'), GOLD_TOKEN_MINT.toBuffer()],
    SEVEN_SEAS_PROGRAM,
  );
  // end: get program derived addresses

  const cthulhuAnchorDiscriminator = sha256.digest('global:cthulhu').slice(0, 8);
  const data = Buffer.from([...cthulhuAnchorDiscriminator, 0o1]);

  return new TransactionInstruction({
    programId: SEVEN_SEAS_PROGRAM,
    keys: [
      {
        pubkey: chestVault,
        isWritable: true,
        isSigner: false,
      },
      {
        pubkey: level,
        isWritable: true,
        isSigner: false,
      },
      {
        pubkey: gameActions,
        isWritable: true,
        isSigner: false,
      },
      {
        pubkey: player,
        isWritable: true,
        isSigner: true,
      },
      {
        pubkey: SystemProgram.programId,
        isWritable: false,
        isSigner: false,
      },
      {
        pubkey: player,
        isWritable: false,
        isSigner: false,
      },
      {
        pubkey: playerTokenAccount.address,
        isWritable: true,
        isSigner: false,
      },
      {
        pubkey: tokenVault,
        isWritable: true,
        isSigner: false,
      },
      {
        pubkey: tokenAccountOwnerPda,
        isWritable: true,
        isSigner: false,
      },
      {
        pubkey: GOLD_TOKEN_MINT,
        isWritable: false,
        isSigner: false,
      },
      {
        pubkey: TOKEN_PROGRAM_ID,
        isWritable: false,
        isSigner: false,
      },
      {
        pubkey: ASSOCIATED_TOKEN_PROGRAM_ID,
        isWritable: false,
        isSigner: false,
      },
    ],
    data,
  });
}

/**
 * @param {TransactionInstruction} ix
 */
async function prepareTx(ix) {
  let tx = new Transaction().add(ix);
  tx.recentBlockhash = (await connection.getLatestBlockhash()).blockhash;
  tx.feePayer = payer.publicKey;

  // partial sign transaction
  tx.partialSign(payer);

  // start: do a dance to get around a bug that is/was in web3.js
  tx = Transaction.from(
    tx.serialize({
      verifySignatures: false,
      requireAllSignatures: false,
    }),
  );

  const serializedTx = tx.serialize({
    verifySignatures: false,
    requireAllSignatures: false,
  });
  // end: dance

  return serializedTx.toString('base64');
}
