export type PirateFaucet = {
  "version": "0.1.0",
  "name": "pirate_faucet",
  "instructions": [
    {
      "name": "initialize",
      "docs": [
        "Initialize the program by creating the liquidity pool"
      ],
      "accounts": [
        {
          "name": "faucet",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "faucet"
              }
            ]
          }
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "fund",
      "docs": [
        "Provide liquidity to the pool by funding it with some asset"
      ],
      "accounts": [
        {
          "name": "faucet",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "faucet"
              }
            ]
          }
        },
        {
          "name": "mint",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "faucetTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "payerTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "amount",
          "type": "u64"
        }
      ]
    },
    {
      "name": "requestAirdrop",
      "docs": [
        "Swap assets using the DEX"
      ],
      "accounts": [
        {
          "name": "faucet",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "faucet"
              }
            ]
          }
        },
        {
          "name": "mint",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "faucetTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "payerTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "ledger",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "ledger"
              },
              {
                "kind": "account",
                "type": "publicKey",
                "path": "payer"
              },
              {
                "kind": "account",
                "type": "publicKey",
                "account": "Mint",
                "path": "mint"
              }
            ]
          }
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "amount",
          "type": "u64"
        }
      ]
    }
  ],
  "accounts": [
    {
      "name": "faucet",
      "docs": [
        "The `Faucet` state - the inner data of the program-derived address",
        "that will be our faucet"
      ],
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "assets",
            "type": {
              "vec": "publicKey"
            }
          },
          {
            "name": "bump",
            "type": "u8"
          }
        ]
      }
    },
    {
      "name": "ledger",
      "docs": [
        "The `Ledger` state - the inner data of the program-derived address",
        "that will be our Ledger"
      ],
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "amountReceived",
            "type": "u64"
          }
        ]
      }
    }
  ],
  "errors": [
    {
      "code": 6000,
      "name": "InvalidArithmetic",
      "msg": "Math overflow on `u64` value"
    },
    {
      "code": 6001,
      "name": "InvalidAssetKey",
      "msg": "An invalid asset mint address was provided"
    },
    {
      "code": 6002,
      "name": "InvalidSwapNotEnoughPay",
      "msg": "The amount proposed to pay is not great enough for at least 1 returned asset quantity"
    },
    {
      "code": 6003,
      "name": "InvalidSwapNotEnoughLiquidity",
      "msg": "The amount proposed to pay resolves to a receive amount that is greater than the current liquidity"
    },
    {
      "code": 6004,
      "name": "InvalidSwapMatchingAssets",
      "msg": "The asset proposed to pay is the same asset as the requested asset to receive"
    },
    {
      "code": 6005,
      "name": "InvalidSwapZeroAmount",
      "msg": "A user cannot propose to pay 0 of an asset"
    },
    {
      "code": 6006,
      "name": "MaxAmountExceeded",
      "msg": "Maximum air drop amount exceeded"
    }
  ]
};

export const IDL: PirateFaucet = {
  "version": "0.1.0",
  "name": "pirate_faucet",
  "instructions": [
    {
      "name": "initialize",
      "docs": [
        "Initialize the program by creating the liquidity pool"
      ],
      "accounts": [
        {
          "name": "faucet",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "faucet"
              }
            ]
          }
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": []
    },
    {
      "name": "fund",
      "docs": [
        "Provide liquidity to the pool by funding it with some asset"
      ],
      "accounts": [
        {
          "name": "faucet",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "faucet"
              }
            ]
          }
        },
        {
          "name": "mint",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "faucetTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "payerTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "amount",
          "type": "u64"
        }
      ]
    },
    {
      "name": "requestAirdrop",
      "docs": [
        "Swap assets using the DEX"
      ],
      "accounts": [
        {
          "name": "faucet",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "faucet"
              }
            ]
          }
        },
        {
          "name": "mint",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "faucetTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "payerTokenAccount",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "ledger",
          "isMut": true,
          "isSigner": false,
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "ledger"
              },
              {
                "kind": "account",
                "type": "publicKey",
                "path": "payer"
              },
              {
                "kind": "account",
                "type": "publicKey",
                "account": "Mint",
                "path": "mint"
              }
            ]
          }
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false
        }
      ],
      "args": [
        {
          "name": "amount",
          "type": "u64"
        }
      ]
    }
  ],
  "accounts": [
    {
      "name": "faucet",
      "docs": [
        "The `Faucet` state - the inner data of the program-derived address",
        "that will be our faucet"
      ],
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "assets",
            "type": {
              "vec": "publicKey"
            }
          },
          {
            "name": "bump",
            "type": "u8"
          }
        ]
      }
    },
    {
      "name": "ledger",
      "docs": [
        "The `Ledger` state - the inner data of the program-derived address",
        "that will be our Ledger"
      ],
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "amountReceived",
            "type": "u64"
          }
        ]
      }
    }
  ],
  "errors": [
    {
      "code": 6000,
      "name": "InvalidArithmetic",
      "msg": "Math overflow on `u64` value"
    },
    {
      "code": 6001,
      "name": "InvalidAssetKey",
      "msg": "An invalid asset mint address was provided"
    },
    {
      "code": 6002,
      "name": "InvalidSwapNotEnoughPay",
      "msg": "The amount proposed to pay is not great enough for at least 1 returned asset quantity"
    },
    {
      "code": 6003,
      "name": "InvalidSwapNotEnoughLiquidity",
      "msg": "The amount proposed to pay resolves to a receive amount that is greater than the current liquidity"
    },
    {
      "code": 6004,
      "name": "InvalidSwapMatchingAssets",
      "msg": "The asset proposed to pay is the same asset as the requested asset to receive"
    },
    {
      "code": 6005,
      "name": "InvalidSwapZeroAmount",
      "msg": "A user cannot propose to pay 0 of an asset"
    },
    {
      "code": 6006,
      "name": "MaxAmountExceeded",
      "msg": "Maximum air drop amount exceeded"
    }
  ]
};
