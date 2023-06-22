export type IdleGame = {
  "version": "0.1.0",
  "name": "idle_game",
  "instructions": [
    {
      "name": "initialize",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "clockworkProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The Clockwork thread program."
          ]
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true,
          "docs": [
            "The signer who will pay to initialize the program.",
            "(not to be confused with the thread executions)."
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "thread",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Address to assign to the newly created thread."
          ]
        },
        {
          "name": "threadAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The pda that will own and manage the thread."
          ]
        }
      ],
      "args": [
        {
          "name": "threadId",
          "type": "bytes"
        }
      ]
    },
    {
      "name": "tradeWoodForGold",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account.",
            "Seeds: gameData + signerPublicKey"
          ]
        },
        {
          "name": "signer",
          "isMut": false,
          "isSigner": true
        }
      ],
      "args": []
    },
    {
      "name": "upgradeTeeth",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account.",
            "Seeds: gameData + signerPublicKey"
          ]
        },
        {
          "name": "signer",
          "isMut": false,
          "isSigner": true
        }
      ],
      "args": []
    },
    {
      "name": "buyLumberjack",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account.",
            "Seeds: gameData + signerPublicKey"
          ]
        },
        {
          "name": "signer",
          "isMut": false,
          "isSigner": true
        }
      ],
      "args": []
    },
    {
      "name": "onThreadTick",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account."
          ]
        },
        {
          "name": "thread",
          "isMut": false,
          "isSigner": true,
          "docs": [
            "Verify that only this thread can execute the ThreadTick Instruction"
          ]
        },
        {
          "name": "threadAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The Thread Admin",
            "The authority that was used as a seed to derive the thread address",
            "`thread_authority` should equal `thread.thread_authority`"
          ]
        }
      ],
      "args": []
    },
    {
      "name": "reset",
      "accounts": [
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "clockworkProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The Clockwork thread program."
          ]
        },
        {
          "name": "thread",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The thread to reset."
          ]
        },
        {
          "name": "threadAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The pda that owns and manages the thread."
          ]
        },
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Close the gameData account"
          ]
        }
      ],
      "args": []
    }
  ],
  "accounts": [
    {
      "name": "gameData",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "authority",
            "type": "publicKey"
          },
          {
            "name": "wood",
            "type": "u64"
          },
          {
            "name": "lumberjacks",
            "type": "u64"
          },
          {
            "name": "gold",
            "type": "u64"
          },
          {
            "name": "teethUpgrade",
            "type": "u64"
          },
          {
            "name": "updatedAt",
            "type": "i64"
          }
        ]
      }
    }
  ],
  "errors": [
    {
      "code": 6000,
      "name": "NotEnoughWood",
      "msg": "Not enough wood."
    },
    {
      "code": 6001,
      "name": "NotEnoughGold",
      "msg": "Not enough gold."
    }
  ]
};

export const IDL: IdleGame = {
  "version": "0.1.0",
  "name": "idle_game",
  "instructions": [
    {
      "name": "initialize",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false
        },
        {
          "name": "clockworkProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The Clockwork thread program."
          ]
        },
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true,
          "docs": [
            "The signer who will pay to initialize the program.",
            "(not to be confused with the thread executions)."
          ]
        },
        {
          "name": "systemProgram",
          "isMut": false,
          "isSigner": false
        },
        {
          "name": "thread",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Address to assign to the newly created thread."
          ]
        },
        {
          "name": "threadAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The pda that will own and manage the thread."
          ]
        }
      ],
      "args": [
        {
          "name": "threadId",
          "type": "bytes"
        }
      ]
    },
    {
      "name": "tradeWoodForGold",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account.",
            "Seeds: gameData + signerPublicKey"
          ]
        },
        {
          "name": "signer",
          "isMut": false,
          "isSigner": true
        }
      ],
      "args": []
    },
    {
      "name": "upgradeTeeth",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account.",
            "Seeds: gameData + signerPublicKey"
          ]
        },
        {
          "name": "signer",
          "isMut": false,
          "isSigner": true
        }
      ],
      "args": []
    },
    {
      "name": "buyLumberjack",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account.",
            "Seeds: gameData + signerPublicKey"
          ]
        },
        {
          "name": "signer",
          "isMut": false,
          "isSigner": true
        }
      ],
      "args": []
    },
    {
      "name": "onThreadTick",
      "accounts": [
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The Game Data account account."
          ]
        },
        {
          "name": "thread",
          "isMut": false,
          "isSigner": true,
          "docs": [
            "Verify that only this thread can execute the ThreadTick Instruction"
          ]
        },
        {
          "name": "threadAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The Thread Admin",
            "The authority that was used as a seed to derive the thread address",
            "`thread_authority` should equal `thread.thread_authority`"
          ]
        }
      ],
      "args": []
    },
    {
      "name": "reset",
      "accounts": [
        {
          "name": "payer",
          "isMut": true,
          "isSigner": true
        },
        {
          "name": "clockworkProgram",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The Clockwork thread program."
          ]
        },
        {
          "name": "thread",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "The thread to reset."
          ]
        },
        {
          "name": "threadAuthority",
          "isMut": false,
          "isSigner": false,
          "docs": [
            "The pda that owns and manages the thread."
          ]
        },
        {
          "name": "gameData",
          "isMut": true,
          "isSigner": false,
          "docs": [
            "Close the gameData account"
          ]
        }
      ],
      "args": []
    }
  ],
  "accounts": [
    {
      "name": "gameData",
      "type": {
        "kind": "struct",
        "fields": [
          {
            "name": "authority",
            "type": "publicKey"
          },
          {
            "name": "wood",
            "type": "u64"
          },
          {
            "name": "lumberjacks",
            "type": "u64"
          },
          {
            "name": "gold",
            "type": "u64"
          },
          {
            "name": "teethUpgrade",
            "type": "u64"
          },
          {
            "name": "updatedAt",
            "type": "i64"
          }
        ]
      }
    }
  ],
  "errors": [
    {
      "code": 6000,
      "name": "NotEnoughWood",
      "msg": "Not enough wood."
    },
    {
      "code": 6001,
      "name": "NotEnoughGold",
      "msg": "Not enough gold."
    }
  ]
};
