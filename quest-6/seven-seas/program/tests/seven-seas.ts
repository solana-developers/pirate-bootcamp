import * as anchor from "@coral-xyz/anchor";
import { Program } from "@coral-xyz/anchor";

import { SevenSeas } from "../target/types/seven_seas";
import {
  TOKEN_PROGRAM_ID,
  createMint,
  getMint,
  mintTo,
  getAccount,
  getOrCreateAssociatedTokenAccount,
  ASSOCIATED_TOKEN_PROGRAM_ID,
} from "@solana/spl-token"; 
import fs from "fs";
import { Keypair } from "@solana/web3.js";

import { ClockworkProvider } from "@clockwork-xyz/sdk";
import { publicKey } from "@project-serum/anchor/dist/cjs/utils";

let goldTokenMint = new anchor.web3.PublicKey("goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3");
let cannonTokenMint = new anchor.web3.PublicKey("boomkN8rQpbgGAKcWvR3yyVVkjucNYcq7gTav78NQAG");
let rumTokenMint = new anchor.web3.PublicKey("rumwqxXmjKAmSdkfkc5qDpHTpETYJRyXY22DWYUmWDt");
const threadId = "thread-wind";

describe("seven-seas", () => {
  // Configure the client to use the local cluster.
  const provider = anchor.AnchorProvider.env();
  anchor.setProvider(provider);
  const clockworkProvider = ClockworkProvider.fromAnchorProvider(provider);

  const program = anchor.workspace.SevenSeas as Program<SevenSeas>;
  const player = anchor.web3.Keypair.generate();
  let tokenOwner = new Uint8Array(JSON.parse(fs.readFileSync("ownSX1SCfotCS3TMmkZtnrGPdjsVwf5E9sAG94eNQS2.json").toString()));
  let tokenOwnerKeypair = Keypair.fromSecretKey(tokenOwner);

  console.log("Local signer is: ", player.publicKey.toBase58());

  it("Initialize!", async () => {
    
    let confirmOptions = {
      skipPreflight: true,
    };

    console.log(new Date(), "requesting airdrop");
    let airdropTx = await anchor.getProvider().connection.requestAirdrop(
      player.publicKey,
      5 * anchor.web3.LAMPORTS_PER_SOL
    );
    let res = await anchor.getProvider().connection.confirmTransaction(airdropTx);
    airdropTx = await anchor.getProvider().connection.requestAirdrop(
      tokenOwnerKeypair.publicKey,
      5 * anchor.web3.LAMPORTS_PER_SOL
    );

    res = await anchor.getProvider().connection.confirmTransaction(airdropTx);

    let key = new Uint8Array(JSON.parse(fs.readFileSync("goLdQwNaZToyavwkbuPJzTt5XPNR3H7WQBGenWtzPH3.json").toString()));
    let tokenKeypair = Keypair.fromSecretKey(key);

    const gold_mint = await createMint(
      anchor.getProvider().connection,
      tokenOwnerKeypair,
      tokenOwnerKeypair.publicKey,
      tokenOwnerKeypair.publicKey,
      9, // We are using 9 decimals to match the CLI decimal default exactly, 
      tokenKeypair    
    );

    let cannon_key = new Uint8Array(JSON.parse(fs.readFileSync("boomkN8rQpbgGAKcWvR3yyVVkjucNYcq7gTav78NQAG.json").toString()));
    let cannonTokenKeypair = Keypair.fromSecretKey(cannon_key);

    const cannon_mint = await createMint(
      anchor.getProvider().connection,
      tokenOwnerKeypair,
      tokenOwnerKeypair.publicKey,
      tokenOwnerKeypair.publicKey,
      9, // We are using 9 decimals to match the CLI decimal default exactly, 
      cannonTokenKeypair    
    );
    
    let rum_key = new Uint8Array(JSON.parse(fs.readFileSync("rumwqxXmjKAmSdkfkc5qDpHTpETYJRyXY22DWYUmWDt.json").toString()));
    let rumTokenKeypair = Keypair.fromSecretKey(rum_key);

    const rum_mint = await createMint(
      anchor.getProvider().connection,
      tokenOwnerKeypair,
      tokenOwnerKeypair.publicKey,
      tokenOwnerKeypair.publicKey,
      9, // We are using 9 decimals to match the CLI decimal default exactly, 
      rumTokenKeypair    
    );

    let [tokenAccountOwnerPda, bump] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_account_owner_pda", "utf8")],
      program.programId
    );
    
    let [token_vault, bump2] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_vault", "utf8"), gold_mint.toBuffer()],
      program.programId
    );
    
    console.log("Mint address: " + gold_mint.toBase58());
    
    let mintInfo = await getMint(anchor.getProvider().connection, gold_mint);
    console.log("Mint Supply" + mintInfo.supply.toString());
    const mintDecimals = Math.pow(10, mintInfo.decimals);
    
    const playerGoldTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      gold_mint,
      tokenOwnerKeypair.publicKey
    );

    const playerCannonTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      cannon_mint,
      tokenOwnerKeypair.publicKey
    );

    const playerRumTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      rum_mint,
      tokenOwnerKeypair.publicKey
    );

    console.log("SenderTokeAccount: " + playerGoldTokenAccount.address.toBase58());
    console.log("VaultAccount: " + token_vault);
    console.log("TokenAccountOwnerPda: " + tokenAccountOwnerPda);

    const mintToPlayerResult = await mintTo(
      anchor.getProvider().connection,
      player,
      gold_mint,
      playerGoldTokenAccount.address,
      tokenOwnerKeypair,
      10000000 * mintDecimals // 10000000 Token with 9 decimals
    );
    console.log("mint sig: " + mintToPlayerResult);
    await anchor.getProvider().connection.confirmTransaction(mintToPlayerResult, "confirmed");

    const mintCannonsToPlayerResult = await mintTo(
      anchor.getProvider().connection,
      player,
      cannon_mint,
      playerCannonTokenAccount.address,
      tokenOwnerKeypair,
      10000000 * mintDecimals // 10000000 Token with 9 decimals
    );
    console.log("mint sig: " + mintCannonsToPlayerResult);
    await anchor.getProvider().connection.confirmTransaction(mintCannonsToPlayerResult, "confirmed");

    const mintRumToPlayerResult = await mintTo(
      anchor.getProvider().connection,
      player,
      rum_mint,
      playerRumTokenAccount.address,
      tokenOwnerKeypair,
      10000000 * mintDecimals // 10000000 Token with 9 decimals
    );

    console.log("mint sig: " + mintRumToPlayerResult);
    await anchor.getProvider().connection.confirmTransaction(mintRumToPlayerResult, "confirmed");

    let tokenAccountInfo = await getAccount(anchor.getProvider().connection, playerGoldTokenAccount.address);
      console.log(
        "Owned player gold token amount: " + tokenAccountInfo.amount / BigInt(mintDecimals)
      );

    console.log("Play gold tokens: " + (10000000 * mintDecimals).toString());

    const [level] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("level")],
      program.programId
    );
    
    const [chestVault] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("chestVault")],
      program.programId
    );
    
    const [gameActions] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("gameActions")],
      program.programId
    );
    
    const tx = await program.methods.initialize()
    .accounts({
      signer: player.publicKey,
      newGameDataAccount: level,
      chestVault: chestVault,
      gameActions: gameActions,
      tokenAccountOwnerPda: tokenAccountOwnerPda,
      vaultTokenAccount: token_vault,
      mintOfTokenBeingSent: gold_mint,
      systemProgram: anchor.web3.SystemProgram.programId,      
      tokenProgram: TOKEN_PROGRAM_ID,
    })
    .signers([player])
    .rpc(confirmOptions);
    console.log("Your transaction signature", tx);

    // Now that all accounts are there mint some tokens to the program token vault
    const mintToProgramResult = await mintTo(
      anchor.getProvider().connection,
      player,
      gold_mint,
      token_vault,
      tokenOwnerKeypair,
      10000000 * mintDecimals // 10000000 Token with 9 decimals
    );
    console.log("mint sig: " + mintToProgramResult);
    await anchor.getProvider().connection.confirmTransaction(mintToProgramResult, "confirmed");

    // await StartThread();
    // await PauseThread();
    // await ResumeThread();
  });

  it("Init Ship!", async () => {    
    let confirmOptions = {
      skipPreflight: true,
    };

    console.log(new Date(), "requesting airdrop");
    let airdropTx = await anchor.getProvider().connection.requestAirdrop(
      player.publicKey,
      5 * anchor.web3.LAMPORTS_PER_SOL
    );

    const res = await anchor.getProvider().connection.confirmTransaction(airdropTx);

    const [shipPDA] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("ship"), player.publicKey.toBuffer()],
      program.programId
    );

    let [token_vault, bump2] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_vault", "utf8"), goldTokenMint.toBuffer()],
      program.programId
    );

    const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      goldTokenMint,
      player.publicKey
    );

    let mintInfo = await getMint(anchor.getProvider().connection, goldTokenMint);
    console.log("Mint Supply" + mintInfo.supply.toString());
    const mintDecimals = Math.pow(10, mintInfo.decimals);

    const mintToPlayerResult = await mintTo(
      anchor.getProvider().connection,
      player,
      goldTokenMint,
      playerTokenAccount.address,
      tokenOwnerKeypair,
      1000000 * mintDecimals, // 10000000 Token with 9 decimals
      [],
      confirmOptions
    );
    console.log("mint sig: " + mintToPlayerResult);
    await anchor.getProvider().connection.confirmTransaction(mintToPlayerResult, "confirmed");

    let tokenAccountInfo = await getAccount(anchor.getProvider().connection, playerTokenAccount.address);
      console.log(
        "Owned player token amount: " + tokenAccountInfo.amount / BigInt(mintDecimals)
      );

    console.log("Play tokens: " + (10000000 * mintDecimals).toString());

    let tx = await program.methods.initializeShip()
    .accounts({
      newShip: shipPDA,
      signer: player.publicKey,
      nftAccount: player.publicKey,
      systemProgram: anchor.web3.SystemProgram.programId,
    })
    .signers([player])
    .rpc(confirmOptions);
    console.log("Init ship transaction", tx);
    
    tx = await program.methods.upgradeShip()
    .accounts({
      newShip: shipPDA,
      signer: player.publicKey,
      nftAccount: player.publicKey,
      systemProgram: anchor.web3.SystemProgram.programId,
      vaultTokenAccount: token_vault,
      mintOfTokenBeingSent: goldTokenMint,
      tokenProgram: TOKEN_PROGRAM_ID,
      playerTokenAccount: playerTokenAccount.address,    
    })
    .signers([player])
    .rpc(confirmOptions);
    console.log("Upgrade ship transaction", tx);
        
    tx = await program.methods.upgradeShip()
    .accounts({
      newShip: shipPDA,
      signer: player.publicKey,
      nftAccount: player.publicKey,
      systemProgram: anchor.web3.SystemProgram.programId,
      vaultTokenAccount: token_vault,
      mintOfTokenBeingSent: goldTokenMint,
      tokenProgram: TOKEN_PROGRAM_ID,
      playerTokenAccount: playerTokenAccount.address,    
    })
    .signers([player])
    .rpc(confirmOptions);
    console.log("Upgrade ship transaction", tx);    
  });

  it("Spawn ship!", async () => {

    console.log(new Date(), "requesting airdrop");
    let airdropTx = await anchor.getProvider().connection.requestAirdrop(
      player.publicKey,
      5 * anchor.web3.LAMPORTS_PER_SOL
    );

    const res = await anchor.getProvider().connection.confirmTransaction(airdropTx);

    const [level] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("level")],
      program.programId
    );

    const [chestVault] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("chestVault")],
      program.programId
    );
    const avatarPubkey = anchor.web3.Keypair.generate();
    
    const [shipPDA] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("ship"), player.publicKey.toBuffer()],
      program.programId
    );

    const playerCannonTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      cannonTokenMint,
      player.publicKey
    );

    console.log("player cannon account: " + playerCannonTokenAccount.address.toString());

    const playerRumTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      rumTokenMint,
      player.publicKey
    );
    console.log("player rum account: " + playerRumTokenAccount.address.toString());

    const tx = await program.methods.spawnPlayer(avatarPubkey.publicKey)
    .accounts({
      player: player.publicKey,
      tokenAccountOwner: player.publicKey,
      gameDataAccount: level,
      chestVault: chestVault,
      nftAccount: player.publicKey,
      ship: shipPDA,
      systemProgram: anchor.web3.SystemProgram.programId,
      cannonTokenAccount: playerCannonTokenAccount.address,
      cannonMint: cannonTokenMint,
      rumTokenAccount: playerRumTokenAccount.address,
      rumMint: rumTokenMint,
      tokenProgram: TOKEN_PROGRAM_ID,
      associatedTokenProgram: ASSOCIATED_TOKEN_PROGRAM_ID,
    })
    .signers([player]);
    let result = await tx.rpc();
    console.log("Your transaction signature", result);
  });

  it("Move!", async () => {
    console.log(new Date(), "requesting airdrop");
    let airdropTx = await anchor.getProvider().connection.requestAirdrop(
      player.publicKey,
      5 * anchor.web3.LAMPORTS_PER_SOL
    );

    let confirmOptions = {
      skipPreflight: true,
    };

    const res = await anchor.getProvider().connection.confirmTransaction(airdropTx);

    const [level] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("level")],
      program.programId
    );
    
    const [chestVault] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("chestVault")],
      program.programId
    );

    let [tokenAccountOwnerPda, bump] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_account_owner_pda", "utf8")],
      program.programId
    );
    
    let [token_vault, bump2] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_vault", "utf8"), goldTokenMint.toBuffer()],
      program.programId
    );

    const [gameActions] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("gameActions")],
      program.programId
    );

    const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      goldTokenMint,
      player.publicKey
    );

    const tx = await program.methods.movePlayerV2(2, 2)
    .accounts({
      player: player.publicKey,
      gameDataAccount: level,
      chestVault: chestVault,
      tokenAccountOwner: player.publicKey,
      systemProgram: anchor.web3.SystemProgram.programId,
      tokenAccountOwnerPda: tokenAccountOwnerPda,
      vaultTokenAccount: token_vault,
      playerTokenAccount: playerTokenAccount.address,
      mintOfTokenBeingSent: goldTokenMint,
      tokenProgram: TOKEN_PROGRAM_ID,
      associatedTokenProgram: ASSOCIATED_TOKEN_PROGRAM_ID,
      gameActions: gameActions,
    })
    .signers([player])
    .rpc(confirmOptions);
    console.log("Your transaction signature", tx);
  });

  it("Shoot!", async () => {
    console.log(new Date(), "requesting airdrop");
    let airdropTx = await anchor.getProvider().connection.requestAirdrop(
      player.publicKey,
      5 * anchor.web3.LAMPORTS_PER_SOL
    );

    let confirmOptions = {
      skipPreflight: true,
    };

    const res = await anchor.getProvider().connection.confirmTransaction(airdropTx);

    const [level] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("level")],
      program.programId
    );
    
    const [chestVault] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("chestVault")],
      program.programId
    );

    const [gameActions] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("gameActions")],
      program.programId
    );
    let [tokenAccountOwnerPda, bump] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_account_owner_pda", "utf8")],
      program.programId
    );
    
    let [token_vault, bump2] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_vault", "utf8"), goldTokenMint.toBuffer()],
      program.programId
    );

    const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      goldTokenMint,
      player.publicKey
    );

    const tx = await program.methods.shoot(0)
    .accounts({
      player: player.publicKey,
      tokenAccountOwner: player.publicKey,
      gameDataAccount: level,
      chestVault: chestVault,
      gameActions: gameActions,
      tokenAccountOwnerPda: tokenAccountOwnerPda,
      vaultTokenAccount: token_vault,
      playerTokenAccount: playerTokenAccount.address,
      mintOfTokenBeingSent: goldTokenMint,
      tokenProgram: TOKEN_PROGRAM_ID,
      associatedTokenProgram: ASSOCIATED_TOKEN_PROGRAM_ID,
    })
    .signers([player])
    .rpc(confirmOptions);
    console.log("Your transaction signature", tx);
  });

  it("Chutuluh!", async () => {
    console.log(new Date(), "requesting airdrop");
    let airdropTx = await anchor.getProvider().connection.requestAirdrop(
      player.publicKey,
      5 * anchor.web3.LAMPORTS_PER_SOL
    );
    const res = await anchor.getProvider().connection.confirmTransaction(airdropTx);

    let confirmOptions = {
      skipPreflight: true,
    };

    const [level] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("level")],
      program.programId
    );
    
    const [chestVault] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("chestVault")],
      program.programId
    );

    const [gameActions] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("gameActions")],
      program.programId
    );

    let [tokenAccountOwnerPda, bump] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_account_owner_pda", "utf8")],
      program.programId
    );
    
    let [token_vault, bump2] = await anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("token_vault", "utf8"), goldTokenMint.toBuffer()],
      program.programId
    );

    const playerTokenAccount = await getOrCreateAssociatedTokenAccount(
      anchor.getProvider().connection,
      player,
      goldTokenMint,
      player.publicKey
    );

    const tx = await program.methods.cthulhu(0)
    .accounts({
      player: player.publicKey,
      tokenAccountOwner: player.publicKey,
      gameDataAccount: level,
      chestVault: chestVault,
      gameActions: gameActions,
      tokenAccountOwnerPda: tokenAccountOwnerPda,
      vaultTokenAccount: token_vault,
      playerTokenAccount: playerTokenAccount.address,
      mintOfTokenBeingSent: goldTokenMint,
      tokenProgram: TOKEN_PROGRAM_ID,
      associatedTokenProgram: ASSOCIATED_TOKEN_PROGRAM_ID,
    })
    .signers([player])
    .rpc(confirmOptions);
    console.log("Your transaction signature", tx);
  });

  async function StartThread() {

    const [level] = anchor.web3.PublicKey.findProgramAddressSync(
      [Buffer.from("level")],
      program.programId
    );

    const [threadAuthority] = publicKey.findProgramAddressSync(
        [anchor.utils.bytes.utf8.encode("authority")], // 👈 make sure it matches on the prog side
        program.programId
    );
    const [threadAddress, threadBump] = clockworkProvider.getThreadPDA(threadAuthority, threadId)

    const tx = await program.methods.startThread(Buffer.from(threadId))
    .accounts({
      payer: player.publicKey,
      gameDataAccount: level,
      thread: threadAddress,
      threadAuthority: threadAuthority,
      clockworkProgram: clockworkProvider.threadProgram.programId,
      systemProgram: anchor.web3.SystemProgram.programId,
    })
    .signers([player])
    .rpc();
    console.log("Create thread instruction", tx);
  };

  async function PauseThread() {
    console.log(new Date(), "requesting airdrop");

    const [threadAuthority] = publicKey.findProgramAddressSync(
        [anchor.utils.bytes.utf8.encode("authority")], // 👈 make sure it matches on the prog side
        program.programId
    );
    const [threadAddress, threadBump] = clockworkProvider.getThreadPDA(threadAuthority, threadId)

    const tx = await program.methods.pauseThread(Buffer.from(threadId))
    .accounts({
      payer: player.publicKey,
      thread: threadAddress,
      threadAuthority: threadAuthority,
      clockworkProgram: clockworkProvider.threadProgram.programId,
    })
    .signers([player])
    .rpc();
    console.log("Pause thread instruction", tx);
  };

  async function ResumeThread() {
    const clockworkProvider = ClockworkProvider.fromAnchorProvider(provider);

    const [threadAuthority] = publicKey.findProgramAddressSync(
        [anchor.utils.bytes.utf8.encode("authority")], // 👈 make sure it matches on the prog side
        program.programId
    );
    const [threadAddress, threadBump] = clockworkProvider.getThreadPDA(threadAuthority, threadId)

    const tx = await program.methods.resumeThread(Buffer.from(threadId))
    .accounts({
      payer: player.publicKey,
      thread: threadAddress,
      threadAuthority: threadAuthority,
      clockworkProgram: clockworkProvider.threadProgram.programId,
    })
    .signers([player])
    .rpc();
    console.log("Your transaction signature", tx);
  };
});
