import { useNetworkConfiguration } from '@/contexts/NetworkConfigurationProvider';
import { PublicKey } from '@solana/web3.js';
import { request } from 'https';
import Image from 'next/image';
import { FC, useEffect, useState } from 'react';

interface AssetCardProps {
  name: string;
  symbol: string;
  uri: string;
  decimals: number;
  balance: number;
  mint: PublicKey;
  poolTokenAccount: PublicKey;
}

const LoanCard: FC<AssetCardProps> = (props: AssetCardProps) => {
  const { networkConfiguration } = useNetworkConfiguration();
  const [imagePath, setImagePath] = useState<string>('');

  const nominalBalance = Math.floor(props.balance / Math.pow(10, props.decimals));

  async function getMetadataFromArweave(uri: string) {
    const data = await fetch(uri).then((data) => data.json());
    setImagePath(data.image);
  }

  useEffect(() => {
    getMetadataFromArweave(props.uri);
  });

  return (
    <div className="w-auto pt-4 p-6 border rounded-lg shadow bg-stone-800 border-amber-950 dark:bg-stone-800 dark:border-amber-950">
      <div className="flex flex-row">
        <div className="flex-shrink-0">
          <Image
            className="rounded-full"
            alt={props.name}
            src={imagePath}
            width="100"
            height="100"
          />
        </div>
        <div className="ml-4">
          <p className="font-bold text-gray-400 dark:text-amber-500">{props.name}</p>
          <p className="mt-2 font-semibold text-lg text-gray-700 dark:text-gray-200">{nominalBalance}</p>
          <div className='mt-2'>
          <a
            className="text-xs font-medium text-slate-400"
            target="_blank"
            rel="noopener noreferrer"
            href={`https://explorer.solana.com/address/${props.mint.toBase58()}?cluster=${networkConfiguration}`}
          >
            See on Explorer →
          </a>
        </div>
        </div>
      </div>
    </div>
  );
};

export default LoanCard;
