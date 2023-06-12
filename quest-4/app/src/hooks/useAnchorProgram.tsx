import { useEffect, useState } from 'react'
import { AnchorProvider, Idl, Program } from '@coral-xyz/anchor'
import {
    useAnchorWallet,
    useConnection,
    useWallet,
} from '@solana/wallet-adapter-react'
import { SwapProgram } from '@/idl/swap_program'
import idlFile from '../idl/swap_program.json'

export default function useAnchorProgram(): Program<SwapProgram> {
    const { connection } = useConnection()
    const wallet = useAnchorWallet()
    const [program, setProgram] = useState<Program<SwapProgram> | null>(null)

    const idl = idlFile as Idl

    useEffect(() => {
        if (wallet) {
            const provider = new AnchorProvider(connection, wallet, {})
            const programInstance = new Program(
                idl,
                idl.metadata.address,
                provider
            )
            setProgram(programInstance as unknown as Program<SwapProgram>)
        }
    }, [wallet, connection, idl])

    return program as Program<SwapProgram>
}
