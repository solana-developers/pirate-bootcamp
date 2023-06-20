import { spawn } from "child_process";
import { PublicKey } from "@solana/web3.js";
import { getAccount } from "@solana/spl-token";
//  import { ClockworkProvider, PAYER_PUBKEY } from "@clockwork-xyz/sdk";

const print_address = (label, address) => {
    console.log(`${label}: https://explorer.solana.com/address/${address}?cluster=devnet`);
}

const print_thread = async (clockworkProvider, address) => {
    const threadAccount = await clockworkProvider.getThreadAccount(address);
    console.log("\nThread: ", threadAccount, "\n");
    print_address("🧵 Thread", address);
    console.log("\n")
}

const print_tx = (label, address) => {
    console.log(`${label}: https://explorer.solana.com/tx/${address}?cluster=devnet`);
}

const stream_program_logs = (programId) => {
    const cmd = spawn("solana", ["logs", "-u", "devnet", programId.toString()]);
      cmd.stdout.on("data", data => {
          console.log(`Program Logs: ${data}`);
      });
}

let lastThreadExec = BigInt(0);
const waitForThreadExec = async (clockworkProvider, thread: PublicKey, maxWait: number = 60) => {
    let i = 1;
    while (true) {
        const execContext = (await clockworkProvider.getThreadAccount(thread)).execContext;
        if (execContext) {
            if (lastThreadExec.toString() == "0" || execContext.lastExecAt > lastThreadExec) {
                lastThreadExec = execContext.lastExecAt;
                break;
            }
        }
        if (i == maxWait) throw Error("Timeout");
        i += 1;
        await new Promise((r) => setTimeout(r, i * 1000));
    }
}

export {
    print_address,
    print_thread,
    print_tx,
    stream_program_logs,
    verifyAmount,
    waitForThreadExec,
}