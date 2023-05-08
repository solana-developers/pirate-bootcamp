#!/bin/bash

# This script will deploy all programs

echo "\n\n================================================="
echo "\n             Deploying All Programs"
echo "\n\n================================================="
sleep 4

solana program deploy ../target/deploy/*.so