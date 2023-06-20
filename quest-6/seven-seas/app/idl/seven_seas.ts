export type SevenSeas = {
  version: "0.1.0"
  name: "seven_seas"
  instructions: [
    {
      name: "initialize"
      accounts: [
        {
          name: "signer"
          isMut: true
          isSigner: true
        },
        {
          name: "newGameDataAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "chestVault"
          isMut: true
          isSigner: false
        },
        {
          name: "gameActions"
          isMut: true
          isSigner: false
        },
        {
          name: "tokenAccountOwnerPda"
          isMut: true
          isSigner: false
        },
        {
          name: "vaultTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "mintOfTokenBeingSent"
          isMut: false
          isSigner: false
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenProgram"
          isMut: false
          isSigner: false
        }
      ]
      args: []
    },
    {
      name: "initializeShip"
      accounts: [
        {
          name: "signer"
          isMut: true
          isSigner: true
        },
        {
          name: "newShip"
          isMut: true
          isSigner: false
        },
        {
          name: "nftAccount"
          isMut: false
          isSigner: false
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        }
      ]
      args: []
    },
    {
      name: "upgradeShip"
      accounts: [
        {
          name: "signer"
          isMut: true
          isSigner: true
        },
        {
          name: "newShip"
          isMut: true
          isSigner: false
        },
        {
          name: "nftAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "playerTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "vaultTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "mintOfTokenBeingSent"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenProgram"
          isMut: false
          isSigner: false
        }
      ]
      args: []
    },
    {
      name: "reset"
      accounts: [
        {
          name: "signer"
          isMut: true
          isSigner: true
        },
        {
          name: "gameDataAccount"
          isMut: true
          isSigner: false
        }
      ]
      args: []
    },
    {
      name: "startThread"
      accounts: [
        {
          name: "gameDataAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "clockworkProgram"
          isMut: false
          isSigner: false
          docs: ["The Clockwork thread program."]
        },
        {
          name: "payer"
          isMut: true
          isSigner: true
          docs: [
            "The signer who will pay to initialize the program.",
            "(not to be confused with the thread executions)."
          ]
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "thread"
          isMut: true
          isSigner: false
          docs: ["Address to assign to the newly created thread."]
        },
        {
          name: "threadAuthority"
          isMut: false
          isSigner: false
          docs: ["The pda that will own and manage the thread."]
        }
      ]
      args: [
        {
          name: "threadId"
          type: "bytes"
        }
      ]
    },
    {
      name: "pauseThread"
      accounts: [
        {
          name: "payer"
          isMut: true
          isSigner: true
        },
        {
          name: "clockworkProgram"
          isMut: false
          isSigner: false
          docs: ["The Clockwork thread program."]
        },
        {
          name: "thread"
          isMut: true
          isSigner: false
          docs: ["Address to assign to the newly created thread."]
        },
        {
          name: "threadAuthority"
          isMut: false
          isSigner: false
          docs: ["The pda that will own and manage the thread."]
        }
      ]
      args: [
        {
          name: "threadId"
          type: "bytes"
        }
      ]
    },
    {
      name: "resumeThread"
      accounts: [
        {
          name: "payer"
          isMut: true
          isSigner: true
        },
        {
          name: "clockworkProgram"
          isMut: false
          isSigner: false
          docs: ["The Clockwork thread program."]
        },
        {
          name: "thread"
          isMut: true
          isSigner: false
          docs: ["Address to assign to the newly created thread."]
        },
        {
          name: "threadAuthority"
          isMut: false
          isSigner: false
          docs: ["The pda that will own and manage the thread."]
        }
      ]
      args: [
        {
          name: "threadId"
          type: "bytes"
        }
      ]
    },
    {
      name: "onThreadTick"
      accounts: [
        {
          name: "gameData"
          isMut: true
          isSigner: false
        },
        {
          name: "thread"
          isMut: false
          isSigner: true
          docs: [
            "Verify that only this thread can execute the ThreadTick Instruction"
          ]
        },
        {
          name: "threadAuthority"
          isMut: false
          isSigner: false
          docs: [
            "The Thread Admin",
            "The authority that was used as a seed to derive the thread address",
            "`thread_authority` should equal `thread.thread_authority`"
          ]
        }
      ]
      args: []
    },
    {
      name: "spawnPlayer"
      accounts: [
        {
          name: "player"
          isMut: true
          isSigner: false
        },
        {
          name: "tokenAccountOwner"
          isMut: true
          isSigner: true
        },
        {
          name: "chestVault"
          isMut: true
          isSigner: false
        },
        {
          name: "gameDataAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "ship"
          isMut: true
          isSigner: false
        },
        {
          name: "nftAccount"
          isMut: false
          isSigner: false
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "cannonTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "cannonMint"
          isMut: false
          isSigner: false
        },
        {
          name: "rumTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "rumMint"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "associatedTokenProgram"
          isMut: false
          isSigner: false
        }
      ]
      args: [
        {
          name: "avatar"
          type: "publicKey"
        }
      ]
    },
    {
      name: "cthulhu"
      accounts: [
        {
          name: "chestVault"
          isMut: true
          isSigner: false
        },
        {
          name: "gameDataAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "gameActions"
          isMut: true
          isSigner: false
        },
        {
          name: "player"
          isMut: true
          isSigner: true
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenAccountOwner"
          isMut: false
          isSigner: false
        },
        {
          name: "playerTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "vaultTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "tokenAccountOwnerPda"
          isMut: true
          isSigner: false
        },
        {
          name: "mintOfTokenBeingSent"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "associatedTokenProgram"
          isMut: false
          isSigner: false
        }
      ]
      args: [
        {
          name: "blockBump"
          type: "u8"
        }
      ]
    },
    {
      name: "shoot"
      accounts: [
        {
          name: "chestVault"
          isMut: true
          isSigner: false
        },
        {
          name: "gameDataAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "gameActions"
          isMut: true
          isSigner: false
        },
        {
          name: "player"
          isMut: true
          isSigner: true
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenAccountOwner"
          isMut: false
          isSigner: false
        },
        {
          name: "playerTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "vaultTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "tokenAccountOwnerPda"
          isMut: true
          isSigner: false
        },
        {
          name: "mintOfTokenBeingSent"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "associatedTokenProgram"
          isMut: false
          isSigner: false
        }
      ]
      args: [
        {
          name: "blockBump"
          type: "u8"
        }
      ]
    },
    {
      name: "movePlayerV2"
      accounts: [
        {
          name: "chestVault"
          isMut: true
          isSigner: false
        },
        {
          name: "gameDataAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "player"
          isMut: true
          isSigner: true
        },
        {
          name: "tokenAccountOwner"
          isMut: false
          isSigner: false
        },
        {
          name: "systemProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "playerTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "vaultTokenAccount"
          isMut: true
          isSigner: false
        },
        {
          name: "tokenAccountOwnerPda"
          isMut: true
          isSigner: false
        },
        {
          name: "mintOfTokenBeingSent"
          isMut: false
          isSigner: false
        },
        {
          name: "tokenProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "associatedTokenProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "gameActions"
          isMut: true
          isSigner: false
        }
      ]
      args: [
        {
          name: "direction"
          type: "u8"
        },
        {
          name: "blockBump"
          type: "u8"
        }
      ]
    }
  ]
  accounts: [
    {
      name: "gameDataAccount"
      type: {
        kind: "struct"
        fields: [
          {
            name: "board"
            type: {
              array: [
                {
                  array: [
                    {
                      defined: "Tile"
                    },
                    10
                  ]
                },
                10
              ]
            }
          },
          {
            name: "actionId"
            type: "u64"
          }
        ]
      }
    },
    {
      name: "gameActionHistory"
      type: {
        kind: "struct"
        fields: [
          {
            name: "idCounter"
            type: "u64"
          },
          {
            name: "gameActions"
            type: {
              vec: {
                defined: "GameAction"
              }
            }
          }
        ]
      }
    },
    {
      name: "chestVaultAccount"
      type: {
        kind: "struct"
        fields: []
      }
    },
    {
      name: "ship"
      type: {
        kind: "struct"
        fields: [
          {
            name: "health"
            type: "u64"
          },
          {
            name: "kills"
            type: "u16"
          },
          {
            name: "cannons"
            type: "u64"
          },
          {
            name: "upgrades"
            type: "u16"
          },
          {
            name: "xp"
            type: "u16"
          },
          {
            name: "level"
            type: "u16"
          },
          {
            name: "startHealth"
            type: "u64"
          }
        ]
      }
    }
  ]
  types: [
    {
      name: "Tile"
      type: {
        kind: "struct"
        fields: [
          {
            name: "player"
            type: "publicKey"
          },
          {
            name: "state"
            type: "u8"
          },
          {
            name: "health"
            type: "u64"
          },
          {
            name: "damage"
            type: "u64"
          },
          {
            name: "range"
            type: "u16"
          },
          {
            name: "collectReward"
            type: "u64"
          },
          {
            name: "avatar"
            type: "publicKey"
          },
          {
            name: "lookDirection"
            type: "u8"
          },
          {
            name: "shipLevel"
            type: "u16"
          },
          {
            name: "startHealth"
            type: "u64"
          }
        ]
      }
    },
    {
      name: "GameAction"
      type: {
        kind: "struct"
        fields: [
          {
            name: "actionId"
            type: "u64"
          },
          {
            name: "actionType"
            type: "u8"
          },
          {
            name: "player"
            type: "publicKey"
          },
          {
            name: "target"
            type: "publicKey"
          },
          {
            name: "damage"
            type: "u64"
          }
        ]
      }
    }
  ]
  errors: [
    {
      code: 6000
      name: "TileOutOfBounds"
    },
    {
      code: 6001
      name: "BoardIsFull"
    },
    {
      code: 6002
      name: "PlayerAlreadyExists"
    },
    {
      code: 6003
      name: "TriedToMovePlayerThatWasNotOnTheBoard"
    },
    {
      code: 6004
      name: "TriedToShootWithPlayerThatWasNotOnTheBoard"
    },
    {
      code: 6005
      name: "WrongDirectionInput"
    },
    {
      code: 6006
      name: "MaxShipLevelReached"
    },
    {
      code: 6007
      name: "CouldNotFindAShipToAttack"
    }
  ]
}

export const IDL: SevenSeas = {
  version: "0.1.0",
  name: "seven_seas",
  instructions: [
    {
      name: "initialize",
      accounts: [
        {
          name: "signer",
          isMut: true,
          isSigner: true,
        },
        {
          name: "newGameDataAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "chestVault",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameActions",
          isMut: true,
          isSigner: false,
        },
        {
          name: "tokenAccountOwnerPda",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vaultTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "mintOfTokenBeingSent",
          isMut: false,
          isSigner: false,
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenProgram",
          isMut: false,
          isSigner: false,
        },
      ],
      args: [],
    },
    {
      name: "initializeShip",
      accounts: [
        {
          name: "signer",
          isMut: true,
          isSigner: true,
        },
        {
          name: "newShip",
          isMut: true,
          isSigner: false,
        },
        {
          name: "nftAccount",
          isMut: false,
          isSigner: false,
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
      ],
      args: [],
    },
    {
      name: "upgradeShip",
      accounts: [
        {
          name: "signer",
          isMut: true,
          isSigner: true,
        },
        {
          name: "newShip",
          isMut: true,
          isSigner: false,
        },
        {
          name: "nftAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "playerTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vaultTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "mintOfTokenBeingSent",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenProgram",
          isMut: false,
          isSigner: false,
        },
      ],
      args: [],
    },
    {
      name: "reset",
      accounts: [
        {
          name: "signer",
          isMut: true,
          isSigner: true,
        },
        {
          name: "gameDataAccount",
          isMut: true,
          isSigner: false,
        },
      ],
      args: [],
    },
    {
      name: "startThread",
      accounts: [
        {
          name: "gameDataAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "clockworkProgram",
          isMut: false,
          isSigner: false,
          docs: ["The Clockwork thread program."],
        },
        {
          name: "payer",
          isMut: true,
          isSigner: true,
          docs: [
            "The signer who will pay to initialize the program.",
            "(not to be confused with the thread executions).",
          ],
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "thread",
          isMut: true,
          isSigner: false,
          docs: ["Address to assign to the newly created thread."],
        },
        {
          name: "threadAuthority",
          isMut: false,
          isSigner: false,
          docs: ["The pda that will own and manage the thread."],
        },
      ],
      args: [
        {
          name: "threadId",
          type: "bytes",
        },
      ],
    },
    {
      name: "pauseThread",
      accounts: [
        {
          name: "payer",
          isMut: true,
          isSigner: true,
        },
        {
          name: "clockworkProgram",
          isMut: false,
          isSigner: false,
          docs: ["The Clockwork thread program."],
        },
        {
          name: "thread",
          isMut: true,
          isSigner: false,
          docs: ["Address to assign to the newly created thread."],
        },
        {
          name: "threadAuthority",
          isMut: false,
          isSigner: false,
          docs: ["The pda that will own and manage the thread."],
        },
      ],
      args: [
        {
          name: "threadId",
          type: "bytes",
        },
      ],
    },
    {
      name: "resumeThread",
      accounts: [
        {
          name: "payer",
          isMut: true,
          isSigner: true,
        },
        {
          name: "clockworkProgram",
          isMut: false,
          isSigner: false,
          docs: ["The Clockwork thread program."],
        },
        {
          name: "thread",
          isMut: true,
          isSigner: false,
          docs: ["Address to assign to the newly created thread."],
        },
        {
          name: "threadAuthority",
          isMut: false,
          isSigner: false,
          docs: ["The pda that will own and manage the thread."],
        },
      ],
      args: [
        {
          name: "threadId",
          type: "bytes",
        },
      ],
    },
    {
      name: "onThreadTick",
      accounts: [
        {
          name: "gameData",
          isMut: true,
          isSigner: false,
        },
        {
          name: "thread",
          isMut: false,
          isSigner: true,
          docs: [
            "Verify that only this thread can execute the ThreadTick Instruction",
          ],
        },
        {
          name: "threadAuthority",
          isMut: false,
          isSigner: false,
          docs: [
            "The Thread Admin",
            "The authority that was used as a seed to derive the thread address",
            "`thread_authority` should equal `thread.thread_authority`",
          ],
        },
      ],
      args: [],
    },
    {
      name: "spawnPlayer",
      accounts: [
        {
          name: "player",
          isMut: true,
          isSigner: false,
        },
        {
          name: "tokenAccountOwner",
          isMut: true,
          isSigner: true,
        },
        {
          name: "chestVault",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameDataAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "ship",
          isMut: true,
          isSigner: false,
        },
        {
          name: "nftAccount",
          isMut: false,
          isSigner: false,
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "cannonTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "cannonMint",
          isMut: false,
          isSigner: false,
        },
        {
          name: "rumTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "rumMint",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "associatedTokenProgram",
          isMut: false,
          isSigner: false,
        },
      ],
      args: [
        {
          name: "avatar",
          type: "publicKey",
        },
      ],
    },
    {
      name: "cthulhu",
      accounts: [
        {
          name: "chestVault",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameDataAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameActions",
          isMut: true,
          isSigner: false,
        },
        {
          name: "player",
          isMut: true,
          isSigner: true,
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenAccountOwner",
          isMut: false,
          isSigner: false,
        },
        {
          name: "playerTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vaultTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "tokenAccountOwnerPda",
          isMut: true,
          isSigner: false,
        },
        {
          name: "mintOfTokenBeingSent",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "associatedTokenProgram",
          isMut: false,
          isSigner: false,
        },
      ],
      args: [
        {
          name: "blockBump",
          type: "u8",
        },
      ],
    },
    {
      name: "shoot",
      accounts: [
        {
          name: "chestVault",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameDataAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameActions",
          isMut: true,
          isSigner: false,
        },
        {
          name: "player",
          isMut: true,
          isSigner: true,
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenAccountOwner",
          isMut: false,
          isSigner: false,
        },
        {
          name: "playerTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vaultTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "tokenAccountOwnerPda",
          isMut: true,
          isSigner: false,
        },
        {
          name: "mintOfTokenBeingSent",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "associatedTokenProgram",
          isMut: false,
          isSigner: false,
        },
      ],
      args: [
        {
          name: "blockBump",
          type: "u8",
        },
      ],
    },
    {
      name: "movePlayerV2",
      accounts: [
        {
          name: "chestVault",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameDataAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "player",
          isMut: true,
          isSigner: true,
        },
        {
          name: "tokenAccountOwner",
          isMut: false,
          isSigner: false,
        },
        {
          name: "systemProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "playerTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vaultTokenAccount",
          isMut: true,
          isSigner: false,
        },
        {
          name: "tokenAccountOwnerPda",
          isMut: true,
          isSigner: false,
        },
        {
          name: "mintOfTokenBeingSent",
          isMut: false,
          isSigner: false,
        },
        {
          name: "tokenProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "associatedTokenProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "gameActions",
          isMut: true,
          isSigner: false,
        },
      ],
      args: [
        {
          name: "direction",
          type: "u8",
        },
        {
          name: "blockBump",
          type: "u8",
        },
      ],
    },
  ],
  accounts: [
    {
      name: "gameDataAccount",
      type: {
        kind: "struct",
        fields: [
          {
            name: "board",
            type: {
              array: [
                {
                  array: [
                    {
                      defined: "Tile",
                    },
                    10,
                  ],
                },
                10,
              ],
            },
          },
          {
            name: "actionId",
            type: "u64",
          },
        ],
      },
    },
    {
      name: "gameActionHistory",
      type: {
        kind: "struct",
        fields: [
          {
            name: "idCounter",
            type: "u64",
          },
          {
            name: "gameActions",
            type: {
              vec: {
                defined: "GameAction",
              },
            },
          },
        ],
      },
    },
    {
      name: "chestVaultAccount",
      type: {
        kind: "struct",
        fields: [],
      },
    },
    {
      name: "ship",
      type: {
        kind: "struct",
        fields: [
          {
            name: "health",
            type: "u64",
          },
          {
            name: "kills",
            type: "u16",
          },
          {
            name: "cannons",
            type: "u64",
          },
          {
            name: "upgrades",
            type: "u16",
          },
          {
            name: "xp",
            type: "u16",
          },
          {
            name: "level",
            type: "u16",
          },
          {
            name: "startHealth",
            type: "u64",
          },
        ],
      },
    },
  ],
  types: [
    {
      name: "Tile",
      type: {
        kind: "struct",
        fields: [
          {
            name: "player",
            type: "publicKey",
          },
          {
            name: "state",
            type: "u8",
          },
          {
            name: "health",
            type: "u64",
          },
          {
            name: "damage",
            type: "u64",
          },
          {
            name: "range",
            type: "u16",
          },
          {
            name: "collectReward",
            type: "u64",
          },
          {
            name: "avatar",
            type: "publicKey",
          },
          {
            name: "lookDirection",
            type: "u8",
          },
          {
            name: "shipLevel",
            type: "u16",
          },
          {
            name: "startHealth",
            type: "u64",
          },
        ],
      },
    },
    {
      name: "GameAction",
      type: {
        kind: "struct",
        fields: [
          {
            name: "actionId",
            type: "u64",
          },
          {
            name: "actionType",
            type: "u8",
          },
          {
            name: "player",
            type: "publicKey",
          },
          {
            name: "target",
            type: "publicKey",
          },
          {
            name: "damage",
            type: "u64",
          },
        ],
      },
    },
  ],
  errors: [
    {
      code: 6000,
      name: "TileOutOfBounds",
    },
    {
      code: 6001,
      name: "BoardIsFull",
    },
    {
      code: 6002,
      name: "PlayerAlreadyExists",
    },
    {
      code: 6003,
      name: "TriedToMovePlayerThatWasNotOnTheBoard",
    },
    {
      code: 6004,
      name: "TriedToShootWithPlayerThatWasNotOnTheBoard",
    },
    {
      code: 6005,
      name: "WrongDirectionInput",
    },
    {
      code: 6006,
      name: "MaxShipLevelReached",
    },
    {
      code: 6007,
      name: "CouldNotFindAShipToAttack",
    },
  ],
}
