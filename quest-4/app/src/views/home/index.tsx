import { useWallet } from '@solana/wallet-adapter-react'
import SwapCard from '@/components/SwapCard'
import LoanCard from '@/components/AssetCard'
import useAnchorProgram from '@/hooks/useAnchorProgram'
import { FC, useEffect, useState } from 'react'
import { Asset, getAssets } from '@/stores/useAssetsStore'
import Image from 'next/image'

export const HomeView: FC = () => {
    const wallet = useWallet()
    const program = useAnchorProgram()
    const [assets, setAssets] = useState<Asset[] | undefined>()

    useEffect(() => {
        const fn = async () => {
            const assets = await getAssets(program)
            setAssets(assets)
        }
        if (program != null) {
            fn()
        }
    }, [program])

    return (
        <div className="md:hero mx-auto p-4">
            <div className="md:hero-content flex flex-col">
                <div className="mt-6">
                    <h1 className="mb-4 text-center text-4xl font-bold font-serif text-yellow-600">
                        Welcome to the port, mate
                    </h1>
                </div>
                {wallet && program ? (
                    <div>
                        {assets && (
                            <div>
                                <div className="flex items-center justify-center space-x-10">
                                    <Image
                                        className="rounded-2xl"
                                        alt="port"
                                        src="/port2.png"
                                        width="350"
                                        height="350"
                                    />
                                    <SwapCard assets={assets} />
                                </div>
                                <div className="grid grid-cols-4 gap-4 mt-4">
                                    {assets.map((asset, i) => (
                                        <LoanCard
                                            key={i}
                                            name={asset.name}
                                            symbol={asset.symbol}
                                            uri={asset.uri}
                                            decimals={asset.decimals}
                                            balance={asset.balance}
                                            mint={asset.mint}
                                            poolTokenAccount={
                                                asset.poolTokenAccount
                                            }
                                        />
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                ) : (
                    <div>
                        <h3 className="mb-4 mt-6 text-center text-2xl font-bold font-serif text-stone-500">
                            Connect a wallet to trade
                        </h3>
                    </div>
                )}
            </div>
        </div>
    )
}
