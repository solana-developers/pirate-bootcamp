import { bundlrStorage, Metaplex, keypairIdentity, toMetaplexFile } from "@metaplex-foundation/js";
import { Connection, Keypair } from "@solana/web3.js";
import fs from 'fs';
import os from 'os';
import { ASSETS } from "./util/const";

/**
 * Script to upload images and JSON files to Arweave using Metaplex's JS
 * SDK so our assets have images!
 * 
 * This should only need to be run once, and then you should
 * update the URI fields in the `ASSETS` array in `tests/util/const.ts`
 */
describe('[Running Setup Script]: Upload Assets', () => {

    const assets = [
        ['Cannon', 'CAN', 'A cannon for defending yer ship!', './assets/cannon.png', 'cannon.png'],
        ['Cannon Ball', 'CANB', 'Cannon balls for yer cannons!', './assets/cannon-ball.png', 'cannon-ball.png'],
        ['Compass', 'COMP', 'A compass to navigate the seven seas!', './assets/compass.png', 'compass.png'],
        ['Fishing Net', 'FISH', 'A fishing net for catching meals for the crew!', './assets/fishing-net.png', 'fishing-net.png'],
        ['Gold', 'GOLD', 'Ahh the finest gold in all of these waters!', './assets/coin1-tp.png', 'coin1-tp.png'],
        ['Grappling Hook', 'GRAP', 'A grappling hook for boarding other ships!', './assets/grappling-hook.png', 'grappling-hook.png'],
        ['Gunpowder', 'GUNP', 'Gunpowder for ye muskets!', './assets/gunpowder.png', 'gunpowder.png'],
        ['Musket', 'MUSK', 'A musket for firing on enemies!', './assets/musket.png', 'musket.png'],
        ['Rum', 'RUM', 'Rum, more rum!', './assets/rum.png', 'rum.png'],
        ['Telescope', 'TELE', 'A telescope for spotting booty amongst the seas!', './assets/telescope.png', 'telescope.png'],
        ['Treasure Map', 'TMAP', 'A map to help ye find long lost treasures!', './assets/treasure-map-1.png', 'treasure-map-1.png'],
    ]

    // Util function to sleep
    const sleepSeconds = async (s: number) =>
        await new Promise((f) => setTimeout(f, s * 1000))
    
    /**
     * 
     * Uploads an asset's image and JSON payload to Arweave
     * 
     * @param name The asset's name
     * @param symbol The symbol for the asset's token
     * @param description The description for the asset's token
     * @param imagePath Path to image on local filesystem
     * @param imageName Name to dub the image
     */
    async function uploadMetadata(
        name: string,
        symbol: string,
        description: string,
        imagePath: string,
        imageName: string,
    ) {
        const metaplex = Metaplex.make(
            new Connection('https://api.devnet.solana.com/', 'confirmed')
        )
            .use(keypairIdentity(
                Keypair.fromSecretKey(
                    Buffer.from(JSON.parse(fs.readFileSync(
                        os.homedir() + '/.config/solana/id.json', 
                        "utf-8"
                    )))
                )
            ))
            .use(bundlrStorage({ address: `https://devnet.bundlr.network` }));
        const { uri } = await metaplex.nfts().uploadMetadata({
            name,
            symbol,
            description,
            image: toMetaplexFile(fs.readFileSync(imagePath), imageName, { contentType: 'image' }),
        });
        console.log(`ASSET: ${name.padEnd(18, ' ')} URI: ${uri}`);
    }

    /**
     * Upload the image & JSON for each listed asset
     */
    for (const a of assets) {
        it(`Uploading Image & JSON for: ${a[0]}`, async () => {
            await uploadMetadata(a[0], a[1], a[2], a[3], a[4])
            sleepSeconds(3)
        })
    }
})
