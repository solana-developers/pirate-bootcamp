export const RPC_ENDPOINT = process.env.RPC_ENDPOINT

export const LOAN_ESCROW_SEED_PREFIX = 'loan_escrow'
export const LOAN_NOTE_MINT_SEED_PREFIX = 'loan_note_mint'

export const REQUIRED_FLOAT_PERCENTAGE = 75

export type LoanStatus = 'available' | 'claimed' | 'closed'
