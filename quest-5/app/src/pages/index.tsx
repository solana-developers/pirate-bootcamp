import Head from 'next/head'
import { HomeView } from '@/views'

export default function Home() {
    return (
        <>
            <Head>
                <title>Arbitrage Pirate</title>
                <meta
                    name="description"
                    content="It's An Arbitrage Pirate's Life for Me"
                />
                <meta
                    name="viewport"
                    content="width=device-width, initial-scale=1"
                />
                <link rel="icon" href="/favicon.ico" />
            </Head>
            <HomeView />
        </>
    )
}
