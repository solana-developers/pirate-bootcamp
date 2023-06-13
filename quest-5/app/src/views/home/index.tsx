import { useWallet } from '@solana/wallet-adapter-react'
import ArbCard from '@/components/ArbCard'
import { FC } from 'react'
import Image from 'next/image'

export const HomeView: FC = () => {
    const wallet = useWallet()

    return (
        <div className="md:hero mx-auto p-4 h-full bg-gradient-to-r from-slate-950 to-sky-950">
            {wallet.connected ? (
                <div className="md:hero-content w-3/4 h-full bg-no-repeat bg-auto bg-top bg-[url('../../public/arb2.jpeg')]">
                    <ArbCard />
                </div>
            ) : (
                <div>
                    <h3 className="mb-4 mt-6 px-16 text-center text-2xl font-bold font-serif text-stone-500">
                        Connect a wallet to start pillaging some ports
                    </h3>
                </div>
            )}
        </div>
    )
}
