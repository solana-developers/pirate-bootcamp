import Head from 'next/head'
import { HomeView } from '@/views';

export default function Home() {
  return (
    <>
      <Head>
        <title>Pirate Swap</title>
        <meta name="description" content="A Pirate's Swap Market" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <HomeView />
    </>
  )
}
