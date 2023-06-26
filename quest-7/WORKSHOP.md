# Workshop

## Step 1 - Setup

Create project directory

```bash
mkdir solana-pay-pirates
cd solana-pay-pirates
```

Initialize npm

```bash
npm init -y
```

Create api directory with index file

```bash
mkdir api
touch api/index.js
```

## Step 2 - Local dev

Install vercel and ngrok locally

```bash
npm i -D vercel ngrok
```

Scaffold server

```js
// api/index.js
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

  return response.status(200).json({});
}
```

Start local dev server

```bash
npx vercel dev
```

Navigate to <http://localhost:3000/api>

In another terminal, start ngrok

```bash
npx ngrok http 3000
```

> With ngrok running, you can now test your local server from the internet.
>
> Navigate to the ngrok url in your browser and you should see the same response as before.

## Step 3 - 🎉 Congratulate yourself and the person sitting next to you!

## Step 4 - Setup Solana Pay QR Code

Create a QR code

<https://qr-code-styling.com/>

The data should be: `solana:<link>`

> Where \<link\> is your ngrok url
>
> Scan the QR code via your mobile wallet -- what happens? what do your logs say?

## Step 5 - Conform to the Solana Pay spec

### Step 5.1 - The GET response

Create a function to handle the GET request

```js
/**
 * @param {VercelResponse} response
 */
function handleGet(response) {
  return response.status(200).json({
    label: 'Chutulu Fire!',
    icon: 'https://github.com/solana-developers/pirate-bootcamp/blob/main/assets/kraken-1.png?raw=true',
  });
}
```

Update the handler to use the function

```diff
export default async function handler(request, response) {
  console.log('handling request', request.method);
+  if (request.method === 'GET') {
-    return response.status(200).json({});
+    return handleGet(response);
+  } else {
+    return response.status(405).json({ error: 'Method not allowed' });
+  }
}
```

### Step 5.2 - The POST response

Create a function to handle the POST request

```js
/**
 * @param {VercelRequest} request
 * @param {VercelResponse} response
 */
async function handlePost(request, response) {
  console.log('account', request.body .account);

  return response.status(200).json({
    transaction: 'TODO',
    message: 'Chutulu Fire!',
  });
}
```

Update the handler to use the function

```diff
export default async function handler(request, response) {
  console.log('handling request', request.method);

  if (request.method === 'GET') {
    return handleGet(response);
-  } else {
+  } else if (request.method === 'POST') {
+    return handlePost(request, response);
  } else {
    return response.status(405).json({ error: 'Method not allowed' });
  }
}
```

### Step 5.2.1 - The POST response -- continued

Let's scaffold our `handlePost` function

```diff
+ import { PublicKey } from '@solana/web3.js';

async function handlePost(request, response) {
-  console.log('account', request.body .account);
+ const player = new PublicKey(request.body.account);
+
+ const chutuluIx = await createChutuluIx(player);
+
+ const transaction = await prepareTx(chutuluIx);

  return response.status(200).json({
-    transaction: 'TODO',
+    transaction,
    message: 'Chutulu Fire!',
  });
}
```

Install `@solana/web3.js`

```bash
npm i @solana/web3.js
```

Create `createChutuluIx` function

```js
/**
 * @typedef {import('@solana/spl-token').Account} Account
 *
 * @param {PublicKey} player
 * @returns {Promise<TransactionInstruction>}
 */
async function createChutuluIx(player) {
  // get player's GOLD token account

  // get accounts for chutuluIX

  // return the instruction
}
```

Get the player's GOLD token account

```diff
async function createChutuluIx(player) {
  // get player's GOLD token account
+ const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
+   connection,
+   payer,
+   GOLD_TOKEN_MINT,
+   player,
+ );


  // get accounts for chutuluIX

  // return the instruction
}
```

Install new deps

```bash
npm i @solana/spl-token dotenv bs58
```

Update imports

```diff
- import { PublicKey } from '@solana/web3.js';
+ import { PublicKey, Keypair, Connection, TransactionInstruction } from '@solana/web3.js';

+ import { getOrCreateAssociatedTokenAccount } from '@solana/spl-token';
+ import bs58 from 'bs58';
+ import dotenv from 'dotenv';
```

Define connection, payer and GOLD_TOKEN_MINT variables

```diff
+ dotenv.config();

+ const connection = new Connection('https://api.devnet.solana.com', 'confirmed');
+ const payer = Keypair.fromSecretKey(bs58.decode(process.env.PAYER));
+ const GOLD_TOKEN_MINT = new PublicKey('goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3');
```

🏁 At this point, your `createChutuluIx` should look like the below:

```js
import { PublicKey, Keypair, Connection, TransactionInstruction } from '@solana/web3.js';
import { getOrCreateAssociatedTokenAccount } from '@solana/spl-token';
import dotenv from 'dotenv';

dotenv.config();

const connection = new Connection('https://api.devnet.solana.com', 'confirmed');
const payer = Keypair.fromSecretKey(bs58.decode(process.env.PAYER));
const GOLD_TOKEN_MINT = new PublicKey('goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3');

// ...

/**
 * @typedef {import('@solana/spl-token').Account} Account
 *
 * @param {PublicKey} player
 * @returns {Promise<TransactionInstruction>}
 */
async function createChutuluIx(player) {
  // get player's GOLD token account
  const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
    connection,
    payer,
    GOLD_TOKEN_MINT,
    player,
  );

  // get accounts for chutuluIX

  // return the instruction
}
```

Get all the accounts needed for the chutulu instruction

```diff
async function createChutuluIx(player) {
  // get player's GOLD token account
  const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
    connection,
    payer,
    GOLD_TOKEN_MINT,
    player,
  );

  // get accounts for chutuluIX
+  // start: get program derived addresses
+  const [level] = PublicKey.findProgramAddressSync([Buffer.from('level')], SEVEN_SEAS_PROGRAM);
+
+  const [chestVault] = PublicKey.findProgramAddressSync(
+    [Buffer.from('chestVault')],
+    SEVEN_SEAS_PROGRAM,
+  );
+
+  const [gameActions] = PublicKey.findProgramAddressSync(
+    [Buffer.from('gameActions')],
+    SEVEN_SEAS_PROGRAM,
+  );
+
+  let [tokenAccountOwnerPda] = await PublicKey.findProgramAddressSync(
+    [Buffer.from('token_account_owner_pda', 'utf8')],
+    SEVEN_SEAS_PROGRAM,
+  );
+
+  let [tokenVault] = await PublicKey.findProgramAddressSync(
+    [Buffer.from('token_vault', 'utf8'), GOLD_TOKEN_MINT.toBuffer()],
+    SEVEN_SEAS_PROGRAM,
+  );
+  // end: get program derived addresses
+
  // return the instruction
}
```

Create the chutulu instruction

```diff
async function createChutuluIx(player) {
  // get player's GOLD token account
  const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
    connection,
    payer,
    GOLD_TOKEN_MINT,
    player,
  );

  // get accounts for chutuluIX
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

  // return the instruction
+  return new TransactionInstruction({
+    programId: SEVEN_SEAS_PROGRAM,
+    keys: [
+      {
+        pubkey: chestVault,
+        isWritable: true,
+        isSigner: false,
+      },
+      {
+        pubkey: level,
+        isWritable: true,
+        isSigner: false,
+      },
+      {
+        pubkey: gameActions,
+        isWritable: true,
+        isSigner: false,
+      },
+      {
+        pubkey: player,
+        isWritable: true,
+        isSigner: true,
+      },
+      {
+        pubkey: SystemProgram.programId,
+        isWritable: false,
+        isSigner: false,
+      },
+      {
+        pubkey: player,
+        isWritable: false,
+        isSigner: false,
+      },
+      {
+        pubkey: playerTokenAccount.address,
+        isWritable: true,
+        isSigner: false,
+      },
+      {
+        pubkey: tokenVault,
+        isWritable: true,
+        isSigner: false,
+      },
+      {
+        pubkey: tokenAccountOwnerPda,
+        isWritable: true,
+        isSigner: false,
+      },
+      {
+        pubkey: GOLD_TOKEN_MINT,
+        isWritable: false,
+        isSigner: false,
+      },
+      {
+        pubkey: TOKEN_PROGRAM_ID,
+        isWritable: false,
+        isSigner: false,
+      },
+      {
+        pubkey: ASSOCIATED_TOKEN_PROGRAM_ID,
+        isWritable: false,
+        isSigner: false,
+      },
+    ],
+    data: Buffer.from(new Uint8Array([84, 206, 8, 255, 98, 163, 218, 19, 1])),
+  });
}
```

Add missing imports

```diff
- import { PublicKey, Keypair, Connection, TransactionInstruction } from '@solana/web3.js';
+ import { PublicKey, Keypair, Connection, Transaction, TransactionInstruction, SystemProgram } from '@solana/web3.js';
- import { getOrCreateAssociatedTokenAccount,  } from '@solana/spl-token';
+ import { getOrCreateAssociatedTokenAccount, ASSOCIATED_TOKEN_PROGRAM_ID, TOKEN_PROGRAM_ID  } from '@solana/spl-token';
import dotenv from 'dotenv';
```

Add missing constant

```diff
const connection = new Connection('https://api.devnet.solana.com', 'confirmed');
const payer = Keypair.fromSecretKey(bs58.decode(process.env.PAYER));
const GOLD_TOKEN_MINT = new PublicKey('goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3');
+ const SEVEN_SEAS_PROGRAM = new PublicKey('2a4NcnkF5zf14JQXHAv39AsRf7jMFj13wKmTL6ZcDQNd');
```

🙏🏾 We're almost there!

Create the `prepareTx` function

```js
/**
 * @param {TransactionInstruction} ix
 */
async function prepareTx(ix) {
  let tx = new Transaction().add(ix);
  tx.recentBlockhash = (await connection.getLatestBlockhash()).blockhash;
  tx.feePayer = payer.publicKey;
}
```

Partially sign the transaction -- why?

```diff
async function prepareTx(ix) {
  let tx = new Transaction().add(ix);
  tx.recentBlockhash = (await connection.getLatestBlockhash()).blockhash;
  tx.feePayer = payer.publicKey;

+ tx.partialSign(payer);
}
```

Do a dance to serialize the transaction and return it

```diff
async function prepareTx(ix) {
  let tx = new Transaction().add(ix);
  tx.recentBlockhash = (await connection.getLatestBlockhash()).blockhash;
  tx.feePayer = payer.publicKey;

  tx.partialSign(payer);

+  tx = Transaction.from(
+    tx.serialize({
+      verifySignatures: false,
+      requireAllSignatures: false,
+    }),
+  );
+
+  const serializedTx = tx.serialize({
+    verifySignatures: false,
+    requireAllSignatures: false,
+  });
+  // end: dance
+
+  return serializedTx.toString('base64');
}
```

🌟 We're all done, scan your QR code and see Chutulu in action!
