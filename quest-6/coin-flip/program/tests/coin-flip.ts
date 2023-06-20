import * as anchor from "@coral-xyz/anchor"
import { Program } from "@coral-xyz/anchor"
import { CoinFlip } from "../target/types/coin_flip"
import * as sbv2 from "@switchboard-xyz/solana.js"
import { NodeOracle } from "@switchboard-xyz/oracle"

describe("coin-flip", () => {
  const provider = anchor.AnchorProvider.env()
  anchor.setProvider(provider)

  const program = anchor.workspace.CoinFlip as Program<CoinFlip>
  const wallet = anchor.workspace.CoinFlip.provider.wallet

  // Keypair used to create new VRF account during setup
  const vrfSecret = anchor.web3.Keypair.generate()
  console.log(`VRF Account: ${vrfSecret.publicKey}`)

  // PDA for Game State Account, this will be authority of the VRF Account
  const [gameStatePDA] = anchor.web3.PublicKey.findProgramAddressSync(
    [Buffer.from("GAME"), wallet.publicKey.toBytes()],
    program.programId
  )
  console.log(`Game State: ${gameStatePDA}`)

  // PDA for Game's Sol Vault Account
  const [solVaultPDA] = anchor.web3.PublicKey.findProgramAddressSync(
    [Buffer.from("VAULT")],
    program.programId
  )
  console.log(`Sol Vault PDA: ${solVaultPDA}`)

  // Create instruction coder using the IDL
  const vrfIxCoder = new anchor.BorshInstructionCoder(program.idl)

  // Callback to consume randomness (the instruction that the oracle CPI's back into our program)
  // This will be stored on the VRF account
  const vrfClientCallback: sbv2.Callback = {
    programId: program.programId,
    accounts: [
      // ensure all accounts in consumeRandomness are populated
      // must match the accounts required in the consumeRandomness instruction in the same order
      { pubkey: wallet.publicKey, isSigner: false, isWritable: true },
      { pubkey: solVaultPDA, isSigner: false, isWritable: true },
      { pubkey: gameStatePDA, isSigner: false, isWritable: true },
      { pubkey: vrfSecret.publicKey, isSigner: false, isWritable: false },
      {
        pubkey: anchor.web3.SystemProgram.programId,
        isSigner: false,
        isWritable: false,
      },
    ],
    ixData: vrfIxCoder.encode("consumeRandomness", ""), // pass any params for instruction here
  }

  let vrfAccount: sbv2.VrfAccount

  // // ========== BEGIN: Use this for devnet ==========
  // let switchboard: {
  //   program: sbv2.SwitchboardProgram
  //   queue: sbv2.QueueAccount
  // }

  // before(async () => {
  //   // use this for devnet
  //   const switchboardProgram = await sbv2.SwitchboardProgram.fromProvider(
  //     provider
  //   )
  //   const [queueAccount, queue] = await sbv2.QueueAccount.load(
  //     switchboardProgram,
  //     "uPeRMdfPmrPqgRWSrjAnAkH78RqAhe5kXoW6vBYRqFX"
  //   )
  //   switchboard = { program: switchboardProgram, queue: queueAccount }
  // })
  // // ========== END: Use this for devnet ==========

  // ========== START: Use this for localnet ==========
  let oracle: NodeOracle
  let switchboard: sbv2.SwitchboardTestContext

  before(async () => {
    try {
      await provider.connection.requestAirdrop(
        solVaultPDA,
        anchor.web3.LAMPORTS_PER_SOL / 10
      )
    } catch (e) {
      console.log(e)
    }

    // use this for localnet
    switchboard = await sbv2.SwitchboardTestContext.loadFromProvider(provider, {
      name: "Test Queue",
      keypair: sbv2.SwitchboardTestContextV2.loadKeypair(
        "~/.keypairs/queue.json"
      ),
      queueSize: 10,
      reward: 0,
      minStake: 0,
      oracleTimeout: 900,
      unpermissionedFeeds: true,
      unpermissionedVrf: true,
      enableBufferRelayers: true,
      oracle: {
        name: "Test Oracle",
        enable: true,
        stakingWalletKeypair: sbv2.SwitchboardTestContextV2.loadKeypair(
          "~/.keypairs/oracleWallet.json"
        ),
      },
    })
    oracle = await NodeOracle.fromReleaseChannel({
      chain: "solana",
      releaseChannel: "testnet",
      network: "devnet", // disables production capabilities like monitoring and alerts
      rpcUrl: switchboard.program.connection.rpcEndpoint,
      oracleKey: switchboard.oracle.publicKey.toBase58(),
      secretPath: switchboard.walletPath,
      silent: true, // set to true to suppress oracle logs in the console
      envVariables: {
        VERBOSE: "1",
        DEBUG: "1",
        DISABLE_NONCE_QUEUE: "1",
        DISABLE_METRICS: "1",
      },
    })
    await oracle.startAndAwait()
  })

  after(async () => {
    oracle?.stop()
  })
  // ========== END: Use this for localnet ==========

  it("Init Player", async () => {
    // Load switchboard queue account
    const queue = await switchboard.queue.loadData()

    // Create Switchboard VRF and Permission account
    // `switchboard.queue.createVrf` does not work in frontend
    // In frontend, must manually build instructions to create VRF and Permission accounts
    // NOTE: A permission account is always required to be created on-chain no matter what. You cant use a queue without one.
    // If a queue has unpermissionedVrfEnabled set to true, then a permission account is required to be created but no special permissions are needed to request randomness
    // If a queue has unpermissionedVrfEnabled set to false, then the queue authority must grant the vrf's permission account PERMIT_VRF_REQUESTS  permission before it can request randomness
    ;[vrfAccount] = await switchboard.queue.createVrf({
      callback: vrfClientCallback, // callback instruction stored on the vrf account (consume randomness)
      authority: gameStatePDA, // set authority to game state PDA
      vrfKeypair: vrfSecret, // new keypair used to create VRF account
      enable: !queue.unpermissionedVrfEnabled, // only set permissions if required ()
    })

    // Initialize player game state
    const tx = await program.methods
      .initialize()
      .accounts({
        player: wallet.publicKey,
        gameState: gameStatePDA,
        vrf: vrfAccount.publicKey,
      })
      .rpc()
    console.log("Your transaction signature", tx)
  })

  it("request_randomness", async () => {
    // Load switchboard queue account
    const queue = await switchboard.queue.loadData()
    // Load VRF account
    const vrf = await vrfAccount.loadData()

    // Derive the existing permission account using the seeds
    const [permissionAccount, permissionBump] = sbv2.PermissionAccount.fromSeed(
      switchboard.program,
      queue.authority,
      switchboard.queue.publicKey,
      vrfAccount.publicKey
    )

    // 0.002 wSOL fee for requesting randomness
    // Note: "getOrCreateWrappedUser" also does not work in frontend, fails with signature verification failed error
    // Must manually build instructions find player's wSOL account and the escrow wSOL account, and use `createSyncNativeInstruction`
    // player's wSOL account is used to pay fund the escrow account (wSOL Associated token account of player's account)
    // escrow account is used to pay the Oracle fee (wSOL Associated token account of VRF account)
    const [payerTokenWallet] =
      await switchboard.program.mint.getOrCreateWrappedUser(
        switchboard.program.walletPubkey,
        { fundUpTo: 0.002 }
      )

    // Request randomness
    const tx = await program.methods
      .requestRandomness(
        permissionBump, // bump of the permission account
        switchboard.program.programState.bump, // bump of the Switchboard program state account
        1 // player guess (heads)
      )
      .accounts({
        player: wallet.publicKey, // player's account
        solVault: solVaultPDA, // game's sol vault account
        gameState: gameStatePDA, // player's game state account
        vrf: vrfAccount.publicKey, // game state account's vrf account
        oracleQueue: switchboard.queue.publicKey, // switchboard queue account
        queueAuthority: queue.authority, // switchboard queue authority
        dataBuffer: queue.dataBuffer, // switchboard queue data buffer
        permission: permissionAccount.publicKey, // vrf account's permission account
        escrow: vrf.escrow, // vrf account's wSOL escrow account
        programState: switchboard.program.programState.publicKey, // switchboard program state account
        switchboardProgram: switchboard.program.programId, // switchboard program id
        payerWallet: payerTokenWallet, // player's wSOL account
        recentBlockhashes: anchor.web3.SYSVAR_RECENT_BLOCKHASHES_PUBKEY, // recent blockhashes account
        tokenProgram: anchor.utils.token.TOKEN_PROGRAM_ID, // token program id
      })
      .rpc()

    console.log("Your transaction signature", tx)

    // Check Sol Vault balance after requesting randomness
    const balance = await provider.connection.getBalance(solVaultPDA)
    console.log(`Sol Vault Balance: ${balance}`)

    // Note: `vrfAccount.nextResult()` does not work in frontend
    // Can use `connection.onAccountChange` to listen for changes to game state account as a workaround
    const result = await vrfAccount.nextResult(
      new anchor.BN(vrf.counter.toNumber() + 1),
      45_000
    )

    if (!result.success) {
      throw new Error(`Failed to get VRF Result: ${result.status}`)
    }

    // Check game state account result after consuming randomness
    const vrfClientState = await program.account.gameState.fetch(gameStatePDA)
    console.log(`VrfClient Result: ${vrfClientState.result.toString(10)}`)

    // Check Sol Vault balance after consuming randomness
    const balance2 = await provider.connection.getBalance(solVaultPDA)
    console.log(`Sol Vault Balance: ${balance2}`)

    // callback program logs
    const callbackTxnMeta = await vrfAccount.getCallbackTransactions()
    console.log(
      JSON.stringify(
        callbackTxnMeta.map((tx) => tx.meta.logMessages),
        undefined,
        2
      )
    )
  })

  // Close game state account to reset for testing
  // Have not implemented closing VRF account, which means some SOL is lost in the VRF account when closing the game state account
  it("close", async () => {
    const tx = await program.methods
      .close()
      .accounts({
        player: wallet.publicKey,
        gameState: gameStatePDA,
      })
      .rpc()
    console.log("Your transaction signature", tx)
  })
})
