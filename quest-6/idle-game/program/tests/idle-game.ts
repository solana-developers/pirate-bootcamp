import { expect } from "chai";
import { PublicKey, SystemProgram, } from "@solana/web3.js";
import * as anchor from "@project-serum/anchor";
import { Program } from "@project-serum/anchor";
import { IdleGame } from "../target/types/idle_game";
import { print_address, print_thread, waitForThreadExec } from "../../utils/helpers";

// 0️⃣  Import the Clockwork SDK.
import { ClockworkProvider } from "@clockwork-xyz/sdk";

const provider = anchor.AnchorProvider.env();
anchor.setProvider(provider);
const wallet = provider.wallet; 
const program = anchor.workspace.IdleGame as Program<IdleGame>;
const clockworkProvider = ClockworkProvider.fromAnchorProvider(provider);

/*
** Helpers
*/
const fetchCounter = async (counter) => {
    const counterAcc = await program.account.counter.fetch(counter);
    console.log("currentValue: " + counterAcc.currentValue + ", updatedAt: " + counterAcc.updatedAt);
    return counterAcc;
}


/*
** Tests
*/
describe("idle-game", () => {
    print_address("🤖 Counter program", program.programId.toString());
    const [counter] = PublicKey.findProgramAddressSync(
        [anchor.utils.bytes.utf8.encode("counter")], // 👈 make sure it matches on the prog side
        program.programId
    );

    // 1️⃣ Prepare thread address
    const threadId = "counter-" + new Date().getTime() / 1000;
    const [threadAuthority] = PublicKey.findProgramAddressSync(
        [anchor.utils.bytes.utf8.encode("authority")], // 👈 make sure it matches on the prog side
        program.programId
    );
    const [threadAddress, threadBump] = clockworkProvider.getThreadPDA(threadAuthority, threadId)

    it("It increments every 10 seconds", async () => {
        try {
            // 2️⃣ Ask our program to create a thread via CPI
            // and thus become the admin of that thread
            await program.methods
                .initialize(Buffer.from(threadId))
                .accounts({
                    payer: wallet.publicKey,
                    systemProgram: SystemProgram.programId,
                    clockworkProgram: clockworkProvider.threadProgram.programId,
                    thread: threadAddress,
                    threadAuthority: threadAuthority,
                    counter: counter,
                })
                .rpc();
            await print_thread(clockworkProvider, threadAddress);

            console.log("Verifying that Thread increments the counter every 10s")
            for (let i = 1; i < 4; i++) {
                await waitForThreadExec(clockworkProvider, threadAddress);
                const counterAcc = await fetchCounter(counter);
                expect(counterAcc.currentValue.toString()).to.eq(i.toString());
            }
        } catch (e) {
            // ❌
            // 'Program log: Instruction: ThreadCreate',
            // 'Program 11111111111111111111111111111111 invoke [2]',
            // 'Allocate: account Address { address: ..., base: None } already in use'
            //
            // -> If you encounter this error, the thread address you are trying to use is already in use.
            //    You can change the threadId, to generate a new account address.
            // -> OR update the thread with a ThreadUpdate instruction (more on this in future guide)
            console.error(e);
            expect.fail(e);
        }
    })

    // Just some cleanup to reset the test to a clean state
    afterEach(async () => {
        try {
            await program.methods
                .reset()
                .accounts({
                    payer: wallet.publicKey,
                    clockworkProgram: clockworkProvider.threadProgram.programId,
                    counter: counter,
                    thread: threadAddress,
                    threadAuthority: threadAuthority,
                })
                .rpc();
        } catch (e) { }
    })
});