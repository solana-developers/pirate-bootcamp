#!/usr/bin/env bash

idl=~/Documents/GitHub/solumberjack/seven-seas/program/target/idl/seven_seas.json
program=~/Documents/GitHub/solumberjack/seven-seas/program/target/deploy/seven_seas.so
kp=~/Documents/GitHub/solumberjack/seven-seas/program/target/deploy/seven_seas-keypair.json
programId=$(solana address -k $kp)

anchor idl init $programId -f $idl
anchor test --skip-local-validator --skip-deploy --provider.cluster localnet