/** @type {import('next').NextConfig} */
const nextConfig = {
    reactStrictMode: true,
    env: {
        RPC_ENDPOINT: process.env.RPC_ENDPOINT,
    },
    images: {
        remotePatterns: [
            {
                protocol: 'https',
                hostname: 'shdw-drive.genesysgo.net',
                port: '',
                pathname: '/**/*',
            },
            {
                protocol: 'https',
                hostname: 'arweave.net',
                port: '',
                pathname: '/*',
            },
        ],
    },
}

module.exports = nextConfig
