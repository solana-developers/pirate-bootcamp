import { Connection } from "@solana/web3.js";

export const connection = new Connection(
    "http://localhost:8899",
    "confirmed"
);

export const IDLE_GAME_PROGRAM_ID = "HMz4pAww1UAhwnsE2WFEkSTazKgFf5pwUAnnMxvDbrjf";
