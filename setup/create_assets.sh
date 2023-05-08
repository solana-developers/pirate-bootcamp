#!/bin/bash

# This script will create all in-game assets (NFTs and SPL Tokens)

echo "\n\n================================================="
echo "\n           Creating All In-Game Assets"
echo "\n\n================================================="
sleep 4

yarn run create-cannons
sleep 2

yarn run create-compasses
sleep 2

yarn run create-fishing-nets
sleep 2

yarn run create-gold
sleep 2

yarn run create-grappling-hooks
sleep 2

yarn run create-gunpowder
sleep 2

yarn run create-kraken
sleep 2

yarn run create-muskets
sleep 2

yarn run create-rum
sleep 2

yarn run create-telescopes
sleep 2

yarn run create-treasure-maps
sleep 2

yarn run create-trophies
sleep 2
