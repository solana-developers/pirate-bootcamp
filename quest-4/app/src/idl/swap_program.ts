export type SwapProgram = {
  "version": "0.1.0",
  "name": "swap_program",
  "instructions": [
    {
      "name": "createPool",
      "docs": [
        "Initialize the program by creating the liquidity pool"
      ],
      "accounts": [
        {
          "name": "pool",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Liquidity Pool"
          ],
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "liquidity_pool"
              }
            ]
          }
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true,
          "docs": [
            "Rent payer"
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "System Program: Required for creating the Liquidity Pool"
          ]
        }
      ],
      "args": []
    },
    {
      "name": "fundPool",
      "docs": [
        "Provide liquidity to the pool by funding it with some asset"
      ],
      "accounts": [
        {
          "name": "pool",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Liquidity Pool"
          ],
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "liquidity_pool"
              }
            ]
          }
        },
        {
          "name": "mint",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The mint account for the asset being deposited into the pool"
          ]
        },
        {
          "name": "poolTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Liquidity Pool's token account for the asset being deposited into",
            "the pool"
          ]
        },
        {
          "name": "payerTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The payer's - or Liquidity Provider's - token account for the asset",
            "being deposited into the pool"
          ]
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "System Program: Required for creating the Liquidity Pool's token account",
            "for the asset being deposited into the pool"
          ]
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Token Program: Required for transferring the assets from the Liquidity",
            "Provider's token account into the Liquidity Pool's token account"
          ]
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Associated Token Program: Required for creating the Liquidity Pool's",
            "token account for the asset being deposited into the pool"
          ]
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
      "name": "swap",
      "docs": [
        "Swap assets using the DEX"
      ],
      "accounts": [
        {
          "name": "pool",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Liquidity Pool"
          ],
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "liquidity_pool"
              }
            ]
          }
        },
        {
          "name": "receiveMint",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The mint account for the asset the user is requesting to receive in",
            "exchange"
          ]
        },
        {
          "name": "poolReceiveTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Liquidity Pool's token account for the mint of the asset the user is",
            "requesting to receive in exchange (which will be debited)"
          ]
        },
        {
          "name": "payerReceiveTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The user's token account for the mint of the asset the user is",
            "requesting to receive in exchange (which will be credited)"
          ]
        },
        {
          "name": "payMint",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The mint account for the asset the user is proposing to pay in the swap"
          ]
        },
        {
          "name": "poolPayTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Liquidity Pool's token account for the mint of the asset the user is",
            "proposing to pay in the swap (which will be credited)"
          ]
        },
        {
          "name": "payerPayTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The user's token account for the mint of the asset the user is",
            "proposing to pay in the swap (which will be debited)"
          ]
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true,
          "docs": [
            "The authority requesting to swap (user)"
          ]
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Token Program: Required for transferring the assets between all token",
            "accounts involved in the swap"
          ]
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
          "name": "amountToSwap",
          "type": "u64"
        }
      ]
    }
  ],
  "accounts": [
    {
      "name": "liquidityPool",
      "docs": [
        "The `LiquidityPool` state - the inner data of the program-derived address",
        "that will be our Liquidity Pool"
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
    }
  ]
};

export const IDL: SwapProgram = {
  "version": "0.1.0",
  "name": "swap_program",
  "instructions": [
    {
      "name": "createPool",
      "docs": [
        "Initialize the program by creating the liquidity pool"
      ],
      "accounts": [
        {
          "name": "pool",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Liquidity Pool"
          ],
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "liquidity_pool"
              }
            ]
          }
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true,
          "docs": [
            "Rent payer"
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "System Program: Required for creating the Liquidity Pool"
          ]
        }
      ],
      "args": []
    },
    {
      "name": "fundPool",
      "docs": [
        "Provide liquidity to the pool by funding it with some asset"
      ],
      "accounts": [
        {
          "name": "pool",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Liquidity Pool"
          ],
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "liquidity_pool"
              }
            ]
          }
        },
        {
          "name": "mint",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The mint account for the asset being deposited into the pool"
          ]
        },
        {
          "name": "poolTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Liquidity Pool's token account for the asset being deposited into",
            "the pool"
          ]
        },
        {
          "name": "payerTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The payer's - or Liquidity Provider's - token account for the asset",
            "being deposited into the pool"
          ]
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "System Program: Required for creating the Liquidity Pool's token account",
            "for the asset being deposited into the pool"
          ]
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Token Program: Required for transferring the assets from the Liquidity",
            "Provider's token account into the Liquidity Pool's token account"
          ]
        },
        {
          "name": "associatedTokenProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Associated Token Program: Required for creating the Liquidity Pool's",
            "token account for the asset being deposited into the pool"
          ]
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
      "name": "swap",
      "docs": [
        "Swap assets using the DEX"
      ],
      "accounts": [
        {
          "name": "pool",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Liquidity Pool"
          ],
          "pda": {
            "seeds": [
              {
                "kind": "const",
                "type": "string",
                "value": "liquidity_pool"
              }
            ]
          }
        },
        {
          "name": "receiveMint",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The mint account for the asset the user is requesting to receive in",
            "exchange"
          ]
        },
        {
          "name": "poolReceiveTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Liquidity Pool's token account for the mint of the asset the user is",
            "requesting to receive in exchange (which will be debited)"
          ]
        },
        {
          "name": "payerReceiveTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The user's token account for the mint of the asset the user is",
            "requesting to receive in exchange (which will be credited)"
          ]
        },
        {
          "name": "payMint",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The mint account for the asset the user is proposing to pay in the swap"
          ]
        },
        {
          "name": "poolPayTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Liquidity Pool's token account for the mint of the asset the user is",
            "proposing to pay in the swap (which will be credited)"
          ]
        },
        {
          "name": "payerPayTokenAccount",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The user's token account for the mint of the asset the user is",
            "proposing to pay in the swap (which will be debited)"
          ]
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true,
          "docs": [
            "The authority requesting to swap (user)"
          ]
        },
        {
          "name": "tokenProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "Token Program: Required for transferring the assets between all token",
            "accounts involved in the swap"
          ]
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
          "name": "amountToSwap",
          "type": "u64"
        }
      ]
    }
  ],
  "accounts": [
    {
      "name": "liquidityPool",
      "docs": [
        "The `LiquidityPool` state - the inner data of the program-derived address",
        "that will be our Liquidity Pool"
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
    }
  ]
};
