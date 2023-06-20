import { ChakraProvider } from "@chakra-ui/react"
import { ProgramProvider } from "@/contexts/ProgramContext"
import { AccountsProvider } from "@/contexts/AccountsContext"
import WalletContextProvider from "../contexts/WalletContextProvider"
import type { AppProps } from "next/app"

export default function App({ Component, pageProps }: AppProps) {
  return (
    <ChakraProvider>
      <WalletContextProvider>
        <ProgramProvider>
          <AccountsProvider>
            <Component {...pageProps} />
          </AccountsProvider>
        </ProgramProvider>
      </WalletContextProvider>
    </ChakraProvider>
  )
}
