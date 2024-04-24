import * as anchor from "@coral-xyz/anchor";
import { Program } from "@coral-xyz/anchor";
import { L3 } from "../target/types/l3";
import { Connection, Keypair, PublicKey } from "@solana/web3.js"
import { createMint, getOrCreateAssociatedTokenAccount, mintTo } from "@solana/spl-token"

describe("l3", () => {
  // Configure the client to use the local cluster.
  let provider = anchor.AnchorProvider.env()
  // console.log("provider", provider, provider.wallet.publicKey)

  anchor.setProvider(provider);

  const payer = provider.wallet as anchor.Wallet
  // console.log(payer)
  const mintKeyPair = Keypair.fromSecretKey(new Uint8Array([
    14, 41, 239, 207, 181, 168, 251, 195, 74, 216, 140,
    92, 126, 223, 119, 173, 42, 44, 80, 89, 198, 239,
    101, 174, 243, 148, 103, 74, 179, 126, 93, 154, 129,
    27, 200, 229, 216, 253, 238, 86, 91, 189, 228, 39,
    85, 150, 160, 184, 94, 148, 167, 184, 196, 123, 29,
    150, 13, 121, 172, 53, 71, 106, 77, 147
  ]))
  // console.log(mintKeyPair)
  const connection = new Connection('http://127.0.0.1:8899', 'confirmed')

  const program = anchor.workspace.L3 as Program<L3>;


  async function createMintToken() {
    const mint = await createMint(
      connection,
      payer.payer,
      payer.publicKey,
      payer.publicKey,
      9,
      mintKeyPair
    )
    // console.log("Mint is ", mint)
  }

  it("Is initialized!", async () => {
    //   // Add your test here.
    //   const tx = await program.methods.initialize().rpc();
    //   console.log("Your transaction signature", tx);
    // });
    await createMintToken()


    let [vaultAccount] = PublicKey.findProgramAddressSync(
      [Buffer.from("vault")],
      program.programId
    )
    // const tx = await program.methods.initialize().rpc();
    const tx = await program.methods.initialize()
      .accounts({
        signer: payer.publicKey,
        tokenVaultAccount: vaultAccount,
        mint: mintKeyPair.publicKey,
      })
      .rpc();
    console.log("Your transaction signature", tx);
  });


  it("Stake", async () => {
    let userTokenAccount = await getOrCreateAssociatedTokenAccount(
      connection,
      payer.payer,
      mintKeyPair.publicKey,
      payer.publicKey
    )

    await mintTo(
      connection,
      payer.payer,
      mintKeyPair.publicKey,
      userTokenAccount.address,
      payer.payer,
      1e11,
    )

    let [stakeInfo] = PublicKey.findProgramAddressSync(
      [Buffer.from("stake_info"), payer.publicKey.toBuffer()],
      program.programId
    )

    let [stakeAccount] = PublicKey.findProgramAddressSync(
      [Buffer.from("token"), payer.publicKey.toBuffer()],
      program.programId
    )

    // await getOrCreateAssociatedTokenAccount(
    //   connection,
    //   payer.payer,
    //   mintKeyPair.publicKey,
    //   payer.publicKey
    // )

    const tx = await program.methods
      .stake(new anchor.BN(1))
      // .signers([payer.payer])
      .accounts({
        signer: payer.publicKey,
        stakeInfoAccount: stakeInfo,
        stakeAccount: stakeAccount,
        userTokenAccount: userTokenAccount.address,
        mint: mintKeyPair.publicKey,
      })
      .rpc();
    console.log("Your transaction signature", tx);
  })

  
  it("Destake", async () => {

    let [vaultAccount] = PublicKey.findProgramAddressSync(
      [Buffer.from("vault")],
      program.programId
    )

    let userTokenAccount = await getOrCreateAssociatedTokenAccount(
      connection,
      payer.payer,
      mintKeyPair.publicKey,
      payer.publicKey
    )

    let [stakeInfo] = PublicKey.findProgramAddressSync(
      [Buffer.from("stake_info"), payer.publicKey.toBuffer()],
      program.programId
    )

    let [stakeAccount] = PublicKey.findProgramAddressSync(
      [Buffer.from("token"), payer.publicKey.toBuffer()],
      program.programId
    )

    await mintTo(
      connection,
      payer.payer,
      mintKeyPair.publicKey,
      vaultAccount,
      payer.payer,
      1e21
    )



    const tx = await program.methods
      .unstake()
      // .signers([payer.payer])
      .accounts({
        signer: payer.publicKey,
        stakeInfoAccount: stakeInfo,
        stakeAccount: stakeAccount,
        userTokenAccount: userTokenAccount.address,
        tokenVaultAccount: vaultAccount,
        mint: mintKeyPair.publicKey,
      })
      .rpc();
    console.log("Your transaction signature", tx);


  })
});

