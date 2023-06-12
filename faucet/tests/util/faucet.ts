import * as anchor from '@coral-xyz/anchor'
import {
    Account as TokenAccount,
    getAccount as getTokenAccount,
    getAssociatedTokenAddressSync,
    getMultipleAccounts as getMultipleTokenAccounts,
} from '@solana/spl-token'
import { Connection, PublicKey } from '@solana/web3.js'
import { PirateFaucet } from '../../target/types/pirate_faucet'
import { fromBigIntQuantity } from './token'

/**
 *
 * Fetches the Liquidity Faucet account and parses its data
 *
 * @param program The swap program as an `anchor.Program<SwapProgram>`
 * @param faucetAddress The address of the Liquidity Faucet program-derived address account
 * @returns The parsed Liquidity Faucet account
 */
export async function fetchFaucet(
    program: anchor.Program<SwapProgram>,
    faucetAddress: PublicKey
): Promise<anchor.IdlTypes<anchor.Idl>['LiquidityFaucet']> {
    return program.account.faucet.fetch(
        faucetAddress
    ) as anchor.IdlTypes<anchor.Idl>['LiquidityFaucet']
}

/**
 *
 * Fetches all owned token accounts from the Liquidity Faucet's stored
 * list of mint addresses
 *
 * @param connection Connection to Solana RPC
 * @param faucetAddress The address of the Liquidity Faucet program-derived address account
 * @param faucet The Liquidity Faucet account itself
 * @returns List of token accounts owned by the Liquidity Faucet
 */
export async function fetchFaucetTokenAccounts(
    connection: Connection,
    faucetAddress: PublicKey,
    faucet: anchor.IdlTypes<anchor.Idl>['LiquidityFaucet']
): Promise<TokenAccount[]> {
    const tokenAddresses = faucet.assets.map((m) =>
        getAssociatedTokenAddressSync(m, faucetAddress, true)
    )
    return getMultipleTokenAccounts(connection, tokenAddresses)
}

/**
 *
 * Fetches the token account balances for the user and the faucet for pay & receive assets
 *
 * @param connection Connection to Solana RPC
 * @param owner The user commencing the swap
 * @param faucet The address of the Liquidity Faucet program-derived address account
 * @param receiveAddress The mint address of the asset they are requesting to receive
 * @param receiveDecimals The mint's decimals for the asset they are requesting to receive
 * @param payAddress The mint address of the asset they are offering to pay
 * @param payDecimals The mint's decimals for the asset they are offering to pay
 * @returns Each balance in `string` format
 */
export async function calculateBalances(
    connection: Connection,
    owner: PublicKey,
    faucet: PublicKey,
    receiveAddress: PublicKey,
    receiveDecimals: number,
    payAddress: PublicKey,
    payDecimals: number
): Promise<[string, string, string, string]> {
    const receiveUserTokenAccount = await getTokenAccount(
        connection,
        getAssociatedTokenAddressSync(receiveAddress, owner)
    )
    const receiveUserBalance = fromBigIntQuantity(
        receiveUserTokenAccount.amount,
        receiveDecimals
    )
    const payUserTokenAccount = await getTokenAccount(
        connection,
        getAssociatedTokenAddressSync(payAddress, owner)
    )
    const payUserBalance = fromBigIntQuantity(
        payUserTokenAccount.amount,
        payDecimals
    )
    const receiveFaucetTokenAccount = await getTokenAccount(
        connection,
        getAssociatedTokenAddressSync(receiveAddress, faucet, true)
    )
    const receiveFaucetBalance = fromBigIntQuantity(
        receiveFaucetTokenAccount.amount,
        receiveDecimals
    )
    const payFaucetTokenAccount = await getTokenAccount(
        connection,
        getAssociatedTokenAddressSync(payAddress, faucet, true)
    )
    const payFaucetBalance = fromBigIntQuantity(
        payFaucetTokenAccount.amount,
        payDecimals
    )
    return [
        receiveUserBalance,
        receiveFaucetBalance,
        payUserBalance,
        payFaucetBalance,
    ]
}

/**
 *
 * Calculates the constant-product `K` (Constant-Product Algorithm)
 *
 * @param tokenAccounts
 * @returns `K` as a `bigint`
 */
export function calculateK(tokenAccounts: TokenAccount[]): bigint {
    return tokenAccounts
        .map((a) => a.amount)
        .reduce((product, i) => product * i)
}

/**
 *
 * Calculates `ΔK` ("delta K", or "change in K")
 *
 * @param start The constant-product `K` at the start of the test
 * @param end The constant-product `K` after the test concluded
 * @returns `ΔK` as a `string`
 */
export function calculateChangeInK(start: bigint, end: bigint): string {
    const startNum = Number(start)
    const endNum = Number(end)
    if (startNum === 0) {
        throw new Error('Cannot calculate percent change for a zero value.')
    }
    const change = endNum - startNum
    const percentChange = (change / startNum) * 100
    return percentChange.toFixed(4) + '%'
}
