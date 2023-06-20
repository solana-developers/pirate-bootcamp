#!/usr/bin/env bash

set -ex

idl=~/Documents/GitHub/solumberjack/seven-seas/program/target/idl/seven_seas.json
program=~/Documents/GitHub/solumberjack/seven-seas/program/target/deploy/seven_seas.so
kp=~/Documents/GitHub/solumberjack/seven-seas/program/target/deploy/seven_seas-keypair.json
programId=$(solana address -k $kp)

clockwork localnet \
    --bpf-program $programId \
    --bpf-program $program