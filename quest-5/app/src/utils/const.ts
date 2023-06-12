import { PublicKey } from '@solana/web3.js';

export const SOL_USD_PRICE_FEED_ID = new PublicKey(
    'ALP8SdU9oARYVLgLR7LrqMNCYBnhtnQz1cj6bwgwQmgj',
);
export const USDC_MINT = new PublicKey(
    '4zMMC9srt5Ri5X14GAgXhaHii3GnPAEERYPJgZJDncDU',
);

export const LOAN_ESCROW_SEED_PREFIX = 'loan_escrow';
export const LOAN_NOTE_MINT_SEED_PREFIX = 'loan_note_mint';

export const REQUIRED_FLOAT_PERCENTAGE = 75;

export type LoanStatus = 'available' | 'claimed' | 'closed';
