import React, { useState, useEffect } from 'react'
import { AiFillCheckCircle } from 'react-icons/ai'
import { GiPirateFlag } from 'react-icons/gi'

const MyComponent = () => {
    const [swap1, setSwap1] = useState('')
    const [swap2, setSwap2] = useState('')
    const [concurrency, setConcurrency] = useState(2)
    const [temperature, setTemperature] = useState(1)
    const [arbitrageRunning, setArbitragerunning] = useState(false)
    const [logs, setLogs] = useState<string[]>([])

    const handleSwap1Change = (e: {
        target: { value: React.SetStateAction<string> }
    }) => {
        setSwap1(e.target.value)
    }

    const handleSwap2Change = (e: {
        target: { value: React.SetStateAction<string> }
    }) => {
        setSwap2(e.target.value)
    }

    const handleConcurrencyChange = (e: { target: { value: string } }) => {
        const value = parseInt(e.target.value)
        setConcurrency(value)
    }

    const handleTemperatureChange = (e: { target: { value: string } }) => {
        const value = parseInt(e.target.value)
        setTemperature(value)
    }

    const logToTerminal = (message: string) => {
        setLogs((prevLogs: string[]) => [...prevLogs, message])
    }

    const handleLaunchArbitrage = () => {
        setArbitragerunning(true)
    }

    const handleStopArbitrage = () => setArbitragerunning(false)

    const executeArbitrage = async () => {
        logToTerminal('Hello')
        await new Promise((resolve) => setTimeout(resolve, 2 * 1000))
    }

    // useEffect(() => {
    //     while (arbitrageRunning) {
    //         executeArbitrage()
    //     }
    // }, [arbitrageRunning])

    return (
        <div className="w-full h-full px-24">
            <div className="flex flow-row flex-1 w-full">
                <div className="flex flow-row w-2/3 p4 rounded-xl bg-slate-400 bg-opacity-40 border border-gray-700">
                    <div className="flex flex-col w-8/12 p-4">
                        <div>
                            <label className="block text-sm font-medium text-lime-400">
                                Swap #1
                            </label>
                            <input
                                type="text"
                                className="mt-1 p-2 block w-full border border-gray-700 rounded focus:ring-blue-500 focus:border-blue-500 bg-slate-400 bg-opacity-40 text-black"
                                value={swap1}
                                onChange={handleSwap1Change}
                            />
                        </div>
                        <div className="mt-2">
                            <label className="block text-sm font-medium text-lime-400">
                                Swap #2
                            </label>
                            <input
                                type="text"
                                className="mt-1 p-2 block w-full border border-gray-700 rounded focus:ring-blue-500 focus:border-blue-500 bg-slate-400 bg-opacity-40 text-black"
                                value={swap2}
                                onChange={handleSwap2Change}
                            />
                        </div>
                    </div>
                    <div className="flex flex-col w-1/12 py-4">
                        <div className="mt-9">
                            <AiFillCheckCircle
                                fill="rgb(163 230 53)"
                                width={'10'}
                            />
                        </div>
                        <div className="mt-14">
                            <AiFillCheckCircle
                                fill="rgb(163 230 53)"
                                width={'10'}
                            />
                        </div>
                    </div>
                    <div className="flex flex-col w-3/12 p-4">
                        <div>
                            <label className="block text-sm font-semibold text-blue-600">
                                Concurrency
                            </label>
                            <input
                                type="number"
                                className="mt-1 p-2 block w-full border border-gray-700 rounded focus:ring-blue-500 focus:border-blue-500 bg-slate-400 bg-opacity-40 text-blue-600"
                                min={2}
                                max={20}
                                value={concurrency}
                                onChange={handleConcurrencyChange}
                            />
                        </div>
                        <div className="mt-2">
                            <label className="block text-sm font-semibold text-blue-600">
                                Temperature
                            </label>
                            <input
                                type="number"
                                className="mt-1 p-2 block w-full border border-gray-700 rounded focus:ring-blue-500 focus:border-blue-500 bg-slate-400 bg-opacity-40 text-blue-600"
                                min={1}
                                max={99}
                                value={temperature}
                                onChange={handleTemperatureChange}
                            />
                        </div>
                    </div>
                </div>
                <div className="flex flex-row flex-1 w-1/3">
                    <div
                        className={`ml-4 w-1/2 h-full text-center rounded-lg border border-slate-500 ${
                            arbitrageRunning
                                ? 'bg-slate-500 bg-opacity-50'
                                : 'bg-lime-500 hover:bg-lime-700'
                        }`}
                    >
                        <button
                            className="w-full h-full p-4 mx-auto my-auto text-center"
                            disabled={arbitrageRunning}
                            onClick={handleLaunchArbitrage}
                        >
                            <div className="w-full mt-4 mb-4 flex flex-col justify-center justify-items-center">
                                <GiPirateFlag
                                    className="mx-auto"
                                    color="rgb(0 0 0)"
                                    size={70}
                                />
                                <p className="mt-3 font-semibold text-black">
                                    Launch Arbitrage
                                </p>
                            </div>
                        </button>
                    </div>
                    <div
                        className={`ml-4 w-1/2 h-full text-center rounded-lg border border-slate-500 ${
                            arbitrageRunning
                                ? 'bg-red-500 hover:bg-red-700'
                                : 'bg-slate-500 bg-opacity-50'
                        }`}
                    >
                        <button
                            className="w-full h-full p-4 mx-auto my-auto text-center"
                            disabled={!arbitrageRunning}
                            onClick={handleStopArbitrage}
                        >
                            <div className="w-full mt-4 mb-4 flex flex-col justify-center justify-items-center">
                                <GiPirateFlag
                                    className="mx-auto"
                                    color="rgb(0 0 0)"
                                    size={70}
                                />
                                <p className="mt-3 font-semibold text-black">
                                    Stop Arbitrage
                                </p>
                            </div>
                        </button>
                    </div>
                </div>
            </div>
            {logs.length > 0 && (
                <div className="p-4 mt-2 rounded-xl bg-slate-400 bg-opacity-60 border border-gray-700 text-black">
                    {logs.map((log, index) => (
                        <div key={index}>{log}</div>
                    ))}
                </div>
            )}
        </div>
    )
}

export default MyComponent
