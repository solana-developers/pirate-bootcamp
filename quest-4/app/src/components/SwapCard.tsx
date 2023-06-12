import useAnchorProgram from '@/hooks/useAnchorProgram'
import { notify } from '@/utils/notifications'
import { BN } from '@coral-xyz/anchor'
import {
    TOKEN_PROGRAM_ID,
    getAssociatedTokenAddressSync,
} from '@solana/spl-token'
import { useWallet } from '@solana/wallet-adapter-react'
import { PublicKey } from '@solana/web3.js'
import { useEffect, useState } from 'react'
import { TbArrowsLeftRight } from 'react-icons/tb'

interface Asset {
    name: string
    symbol: string
    uri: string
    balance: number
    mint: PublicKey
    poolTokenAccount: PublicKey
    decimals: number
}

interface TokenSwapProps {
    assets: Asset[]
}

const SwapCard: React.FC<TokenSwapProps> = ({ assets }) => {
    const tokens = assets
    const program = useAnchorProgram()
    const [fromToken, setFromToken] = useState(tokens[0])
    const [toToken, setToToken] = useState(tokens[1])
    const [amount, setAmount] = useState(0)
    const [receiveAmount, setReceiveAmount] = useState(0)
    const wallet = useWallet()

    useEffect(() => {
        // Calculate the receive amount based on the constant product formula
        const r = (toToken.balance * amount) / (fromToken.balance + amount)
        const adjustedR = r / Math.pow(10, toToken.decimals)
        const roundedR = Math.round(adjustedR * 100) / 100
        setReceiveAmount(roundedR)
    }, [amount, fromToken, toToken])

    const handleFlop = () => {
        setFromToken(toToken)
        setToToken(fromToken)
    }

    const swap = async () => {
        if (wallet.publicKey) {
            const LIQUIDITY_POOL_SEED_PREFIX = 'liquidity_pool'
            const poolAddress = PublicKey.findProgramAddressSync(
                [Buffer.from(LIQUIDITY_POOL_SEED_PREFIX)],
                program.programId
            )[0]

            const sig = await program.methods
                .swap(new BN(amount))
                .accounts({
                    pool: poolAddress,
                    receiveMint: toToken.mint,
                    poolReceiveTokenAccount: getAssociatedTokenAddressSync(
                        toToken.mint,
                        poolAddress,
                        true
                    ),
                    payerReceiveTokenAccount: getAssociatedTokenAddressSync(
                        toToken.mint,
                        wallet.publicKey,
                        true
                    ),
                    payMint: fromToken.mint,
                    poolPayTokenAccount: getAssociatedTokenAddressSync(
                        fromToken.mint,
                        poolAddress,
                        true
                    ),
                    payerPayTokenAccount: getAssociatedTokenAddressSync(
                        fromToken.mint,
                        wallet.publicKey
                    ),
                    payer: wallet.publicKey,
                    tokenProgram: TOKEN_PROGRAM_ID,
                })
                .rpc()
            notify({
                type: 'success',
                message: 'Swap successful!',
                txid: sig,
            })
        }
    }
    return (
        <div className="flex flex-row justify-center">
            <div className="p-4 shadow dark:bg-stone-900 dark:border-yellow-950 rounded-lg">
                <div className="flex justify-between items-center mb-4">
                    <select
                        value={fromToken.symbol}
                        onChange={(e) => {
                            const selectedAsset = assets.find(
                                (asset) => asset.symbol === e.target.value
                            )
                            if (selectedAsset) {
                                setFromToken(selectedAsset)
                            }
                        }}
                        className="p-3 rounded-md border mx-2 bg-black text-white"
                    >
                        {assets.map(
                            (asset, index) =>
                                asset.symbol !== toToken.symbol && (
                                    <option key={index} value={asset.symbol}>
                                        {asset.name}
                                    </option>
                                )
                        )}
                    </select>

                    <button
                        onClick={handleFlop}
                        className="p-2 bg-yellow-700 hover:bg-yellow-900 text-white rounded"
                    >
                        <TbArrowsLeftRight />
                    </button>

                    <select
                        value={toToken.symbol}
                        onChange={(e) => {
                            const selectedAsset = assets.find(
                                (asset) => asset.symbol === e.target.value
                            )
                            if (selectedAsset) {
                                setToToken(selectedAsset)
                            }
                        }}
                        className="p-3 rounded-md border mx-2 bg-black text-white"
                    >
                        {assets.map(
                            (asset, index) =>
                                asset.symbol !== fromToken.symbol && (
                                    <option key={index} value={asset.symbol}>
                                        {asset.name}
                                    </option>
                                )
                        )}
                    </select>
                </div>
                <div className="flex justify-between items-center">
                    <div className="flex justify-between items-start space-x-4">
                        <div className="flex flex-col">
                            <label htmlFor="pay">Pay:</label>
                            <input
                                id="pay"
                                className="bg-black rounded-lg p-2"
                                placeholder="Amount"
                                type="number"
                                onChange={(e) =>
                                    setAmount(
                                        Number(e.target.value) *
                                            10 ** fromToken.decimals
                                    )
                                }
                            />
                        </div>
                        <div className="flex flex-col">
                            <label htmlFor="receive">Receive:</label>
                            <div
                                id="receive"
                                className="bg-green-950 text-white rounded-lg p-2"
                            >
                                {receiveAmount}
                            </div>
                        </div>
                    </div>
                </div>
                <button
                    className="w-full bg-yellow-700 hover:bg-yellow-900 h-12 mt-2 rounded-lg"
                    onClick={swap}
                >
                    Swap
                </button>
            </div>
        </div>
    )
}

export default SwapCard
