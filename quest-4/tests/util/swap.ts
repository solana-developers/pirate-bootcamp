import * as anchor from '@coral-xyz/anchor'
import {
    Account as TokenAccount,
    getAccount as getTokenAccount,
    getAssociatedTokenAddressSync,
    getMultipleAccounts as getMultipleTokenAccounts,
} from '@solana/spl-token'
import { Connection, PublicKey } from '@solana/web3.js'
import { SwapProgram } from '../../target/types/swap_program'
import { fromBigIntQuantity } from './token'

/**
 *
 * Fetches the Liquidity Pool account and parses its data
 *
 * @param program The swap program as an `anchor.Program<SwapProgram>`
 * @param poolAddress The address of the Liquidity Pool program-derived address account
 * @returns The parsed Liquidity Pool account
 */
export async function fetchPool(
    program: anchor.Program<SwapProgram>,
    poolAddress: PublicKey
): Promise<anchor.IdlTypes<anchor.Idl>['LiquidityPool']> {
    return program.account.liquidityPool.fetch(
        poolAddress
    ) as anchor.IdlTypes<anchor.Idl>['LiquidityPool']
}

/**
 *
 * Fetches all owned token accounts from the Liquidity Pool's stored
 * list of mint addresses
 *
 * @param connection Connection to Solana RPC
 * @param poolAddress The address of the Liquidity Pool program-derived address account
 * @param pool The Liquidity Pool account itself
 * @returns List of token accounts owned by the Liquidity Pool
 */
export async function fetchPoolTokenAccounts(
    connection: Connection,
    poolAddress: PublicKey,
    pool: anchor.IdlTypes<anchor.Idl>['LiquidityPool']
): Promise<TokenAccount[]> {
    const tokenAddresses = pool.assets.map((m) =>
        getAssociatedTokenAddressSync(m, poolAddress, true)
    )
    return getMultipleTokenAccounts(connection, tokenAddresses)
}

/**
 *
 * Fetches the token account balances for the user and the pool for pay & receive assets
 *
 * @param connection Connection to Solana RPC
 * @param owner The user commencing the swap
 * @param pool The address of the Liquidity Pool program-derived address account
 * @param receiveAddress The mint address of the asset they are requesting to receive
 * @param receiveDecimals The mint's decimals for the asset they are requesting to receive
 * @param payAddress The mint address of the asset they are offering to pay
 * @param payDecimals The mint's decimals for the asset they are offering to pay
 * @returns Each balance in `string` format
 */
export async function calculateBalances(
    connection: Connection,
    owner: PublicKey,
    pool: PublicKey,
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
    const receivePoolTokenAccount = await getTokenAccount(
        connection,
        getAssociatedTokenAddressSync(receiveAddress, pool, true)
    )
    const receivePoolBalance = fromBigIntQuantity(
        receivePoolTokenAccount.amount,
        receiveDecimals
    )
    const payPoolTokenAccount = await getTokenAccount(
        connection,
        getAssociatedTokenAddressSync(payAddress, pool, true)
    )
    const payPoolBalance = fromBigIntQuantity(
        payPoolTokenAccount.amount,
        payDecimals
    )
    return [
        receiveUserBalance,
        receivePoolBalance,
        payUserBalance,
        payPoolBalance,
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
