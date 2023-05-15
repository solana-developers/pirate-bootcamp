# TODO

To set up the bootcamp, we can follow the instructions in the [Setup README](./setup/README.md).

Ideally, we can run through the setup and make sure everything works for anyone who wishes to run this bootcamp "out of the box".

In addition to the procedural setup defined in the above doc, we must hash out the following:

### Quests:

- [ ] Import all finished quests (or "Days") into this monorepo
- [ ] Document all quests & associated workshops
- [ ] Completely fill out each `resources.md` file with any useful resources for the quest (or "Day")

### Ships:

- [ ] Create collection for ship NFTs ?
  - [ ] We may or may not want to do this, since it would require one mint auth
- [ ] Streamline creation of custom ship NFT artwork
  - [ ] Maybe we can get a Foundation Midjourney paid tier going and let people create images with that?
- [ ] Streamline uploading of images to Arweave devnet
  - [ ] See https://github.com/buffalojoec/nyc-bootcamp-swap-program/blob/main/tests/upload-json.test.ts

### Tokens & Assets:

- [ ] Decide on which images will represent which assets
- [ ] Assign a master keypair for creating/minting assets
- [ ] Mint `X` number of assets ahead of time
  - [ ] What is `X` for each asset?

### Programs:

- [ ] Assign a master keypair for deploying programs
- [ ] Deploy programs to devnet
- [ ] Integrate deployed programs with all UIs
