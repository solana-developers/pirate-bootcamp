/**
 * Demonstrates how to mint NFTs and store their metadata on chain using the Metaplex MetadataProgram
 */

// import custom helpers for demos
import { payer, connection } from "@/lib/vars";
import { explorerURL, loadPublicKeysFromFile, printConsoleSeparator } from "@/lib/helpers";

import { PublicKey } from "@solana/web3.js";
import { Metaplex, bundlrStorage, keypairIdentity } from "@metaplex-foundation/js";

(async () => {
  //////////////////////////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////

  console.log("Payer address:", payer.publicKey.toBase58());

  //////////////////////////////////////////////////////////////////////////////

  // load the stored PublicKeys for ease of use
  let localKeys = loadPublicKeysFromFile();

  // ensure the desired script was already run
  if (!localKeys?.tokenMint)
    return console.warn("No local keys were found. Please run '3.createTokenWithMetadata.ts'");

  const tokenMint: PublicKey = localKeys.tokenMint;

  console.log("==== Local PublicKeys loaded ====");
  console.log("Token's mint address:", tokenMint.toBase58());
  console.log(explorerURL({ address: tokenMint.toBase58() }));

  //////////////////////////////////////////////////////////////////////////////
  //////////////////////////////////////////////////////////////////////////////

  /**
   * define our ship's JSON metadata
   * checkout: https://nft.storage/ to help store images
   */
  const metadata = {
    name: "The Gradient Pearl",
    symbol: "SHIP",
    description:
      "The Gradient Pearl is a legendary Pirate ship that sails the Seven Seas. Captain Rajovenko leads with a drink can in his hand. ",
    image:
      "https://bafybeic75qqhfytc6xxoze2lo5af2lfhmo2kh4mhirelni2wota633dgqu.ipfs.nftstorage.link/",
  };
  // another ship: "https://bafybeiblld2wlxyivlivnhaqbcixhzxrodjzrycjkitz3kdmzj65gebwxe.ipfs.nftstorage.link/"
  // Captain Rajovenko: "https://bafybeihww4tue5pme3h2udqvkpfbzs5zf4h2pysuoowwofbbk372vvtmja.ipfs.nftstorage.link/"

  /**
   * Use the Metaplex sdk to handle most NFT actions
   */

  // create an instance of Metaplex sdk for use
  const metaplex = Metaplex.make(connection)
    // set our keypair to use, and pay for the transaction
    .use(keypairIdentity(payer))
    // define a storage mechanism to upload with
    .use(bundlrStorage({
      address: "https://devnet.bundlr.network",
      providerUrl: "https://api.devnet.solana.com",
      timeout: 60000
    }));

  // upload the JSON metadata
  const { uri } = await metaplex.nfts().uploadMetadata(metadata);

  console.log(uri);

  // create a new nft using the metaplex sdk
  const { nft, response } = await metaplex.nfts().create({
    uri,
    name: metadata.name,
    symbol: metadata.symbol,

    // `sellerFeeBasisPoints` is the royalty that you can define on nft
    sellerFeeBasisPoints: 500, // Represents 5.00%.

    //
    isMutable: true,
  });

  console.log(nft);

  printConsoleSeparator("NFT created:");
  console.log(explorerURL({ txSignature: response.signature }));

  return;

  /**
   *
   */

  printConsoleSeparator("Find by mint:");

  // you can also use the metaplex sdk to retrieve info about the NFT's mint
  const mintInfo = await metaplex.nfts().findByMint({
    mintAddress: tokenMint,
  });
  console.log(mintInfo);
})();
