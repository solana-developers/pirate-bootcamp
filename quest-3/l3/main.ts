import * as anchor from "@coral-xyz/anchor";
import { Program } from "@coral-xyz/anchor";
import { L3 } from "./target/types/l3";
import { Connection, Keypair, PublicKey, clusterApiUrl } from "@solana/web3.js"
import { createMint, getOrCreateAssociatedTokenAccount, mintTo } from "@solana/spl-token"
import NodeWallet from "@coral-xyz/anchor/dist/cjs/nodewallet";
//import dotenv
import dotenv from "dotenv";
import idl from "./target/idl/l3.json"
dotenv.config();




async function main() {

    // let provider = new anchor.setProvider("https://api.devnet.solana.com")
    //init provider with https://api.devnet.solana.com
    // let provider = anchor.AnchorProvider.local("https://api.devnet.solana.com")
    // console.log("provider", provider, provider.wallet.publicKey)

    let connection = new Connection(clusterApiUrl("devnet"));
    let pk = process.env.pk
    //string to uint8Array
    pk = pk.replace(/\[|\]/g, ''); // 去掉开头和结尾的方括号
    let uint8array = new Uint8Array(pk.split(',').map(Number));
    // console.log(process.env.pk)

    const keyPair = Keypair.fromSecretKey(new Uint8Array(uint8array))

    const mintKeyPair = Keypair.fromSecretKey(new Uint8Array([
        14, 41, 239, 207, 181, 168, 251, 195, 74, 216, 140,
        92, 126, 223, 119, 173, 42, 44, 80, 89, 198, 239,
        101, 174, 243, 148, 103, 74, 179, 126, 93, 154, 129,
        27, 200, 229, 216, 253, 238, 86, 91, 189, 228, 39,
        85, 150, 160, 184, 94, 148, 167, 184, 196, 123, 29,
        150, 13, 121, 172, 53, 71, 106, 77, 147
    ]))

    let wallet = new NodeWallet(keyPair);

    // console.log(wallet)
    const provider = new anchor.AnchorProvider(connection, wallet, {
        commitment: "processed",
    });
    // console.log("provider", provider, provider.publicKey)

    async function createMintToken() {
        const mint = await createMint(
            connection,
            wallet.payer,
            wallet.publicKey,
            wallet.publicKey,
            9,
            mintKeyPair
        )
        // console.log("Mint is ", mint)
    }

    // await createMintToken()
    const program = new Program(idl as anchor.Idl, new anchor.web3.PublicKey("4vDhf9BpYKzHqvwK1dW162fTuPzCUj9ZSz65hsM8zuFU"), provider)

    // //1. initialize
    // let [vaultAccount] = PublicKey.findProgramAddressSync(
    //     [Buffer.from("vault")],
    //     program.programId
    // )
    // // console.log("vaultAccount", vaultAccount)
    // const tx = await program.methods.initialize()
    //   .accounts({
    //     signer: wallet.publicKey,
    //     tokenVaultAccount: vaultAccount,
    //     mint: mintKeyPair.publicKey,
    //   })
    //   .rpc();
    // console.log("Your Initialize transaction signature", tx);



    // // 2. Satke 
    // let userTokenAccount = await getOrCreateAssociatedTokenAccount(
    //     connection,
    //     wallet.payer,
    //     mintKeyPair.publicKey,
    //     wallet.publicKey
    // )
    // // console.log("userTokenAccount", userTokenAccount)

    // //to our self mint 100 token

    // // let tx = await mintTo(
    // //     connection,
    // //     wallet.payer,
    // //     mintKeyPair.publicKey,
    // //     userTokenAccount.address,
    // //     wallet.payer,
    // //     1e11,
    // // )
    // // console.log("Mint to transaction signature", tx)

    // let [stakeInfo] = PublicKey.findProgramAddressSync(
    //     [Buffer.from("stake_info"), wallet.publicKey.toBuffer()],
    //     program.programId
    // )

    // let [stakeAccount] = PublicKey.findProgramAddressSync(
    //     [Buffer.from("token"), wallet.publicKey.toBuffer()],
    //     program.programId
    // )

    // console.log("stakeInfo", stakeInfo, "stakeAccount", stakeAccount)


    // let tx = await program.methods
    //     .stake(new anchor.BN(1))
    //     // .signers([payer.payer])
    //     .accounts({
    //         signer: wallet.publicKey,
    //         stakeInfoAccount: stakeInfo,
    //         stakeAccount: stakeAccount,
    //         userTokenAccount: userTokenAccount.address,
    //         mint: mintKeyPair.publicKey,
    //     })
    //     .rpc();
    // console.log("Your transaction signature", tx);


    //3. Unstake
    let [vaultAccount] = PublicKey.findProgramAddressSync(
        [Buffer.from("vault")],
        program.programId
    )

    let tx = await mintTo(
        connection,
        wallet.payer,
        mintKeyPair.publicKey,
        vaultAccount,
        wallet.payer,
        1e21
    )
    console.log("Mint to transaction signature", tx)

    let userTokenAccount = await getOrCreateAssociatedTokenAccount(
        connection,
        wallet.payer,
        mintKeyPair.publicKey,
        wallet.publicKey
    )

    let [stakeInfo] = PublicKey.findProgramAddressSync(
        [Buffer.from("stake_info"), wallet.publicKey.toBuffer()],
        program.programId
    )

    let [stakeAccount] = PublicKey.findProgramAddressSync(
        [Buffer.from("token"), wallet.publicKey.toBuffer()],
        program.programId
    )


    tx = await program.methods
        .unstake()
        // .signers([payer.payer])
        .accounts({
            signer: wallet.publicKey,
            stakeInfoAccount: stakeInfo,
            stakeAccount: stakeAccount,
            userTokenAccount: userTokenAccount.address,
            tokenVaultAccount: vaultAccount,
            mint: mintKeyPair.publicKey,
        })
        .rpc();
    console.log("Your unstake transaction signature", tx);




}

main()