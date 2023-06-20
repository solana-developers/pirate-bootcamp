export type Vrf = {
  version: "0.1.0"
  name: "vrf"
  instructions: [
    {
      name: "initialize"
      accounts: [
        {
          name: "player"
          isMut: true
          isSigner: true
        },
        {
          name: "gameState"
          isMut: true
          isSigner: false
        },
        {
          name: "vrf"
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
      name: "requestRandomness"
      accounts: [
        {
          name: "player"
          isMut: true
          isSigner: true
        },
        {
          name: "solVault"
          isMut: true
          isSigner: false
        },
        {
          name: "gameState"
          isMut: true
          isSigner: false
        },
        {
          name: "vrf"
          isMut: true
          isSigner: false
        },
        {
          name: "oracleQueue"
          isMut: true
          isSigner: false
        },
        {
          name: "queueAuthority"
          isMut: true
          isSigner: false
        },
        {
          name: "dataBuffer"
          isMut: true
          isSigner: false
          docs: ["CHECK"]
        },
        {
          name: "permission"
          isMut: true
          isSigner: false
        },
        {
          name: "escrow"
          isMut: true
          isSigner: false
        },
        {
          name: "programState"
          isMut: true
          isSigner: false
        },
        {
          name: "switchboardProgram"
          isMut: false
          isSigner: false
        },
        {
          name: "payerWallet"
          isMut: true
          isSigner: false
        },
        {
          name: "recentBlockhashes"
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
      args: [
        {
          name: "permissionBump"
          type: "u8"
        },
        {
          name: "switchboardStateBump"
          type: "u8"
        },
        {
          name: "guess"
          type: "u8"
        }
      ]
    },
    {
      name: "consumeRandomness"
      accounts: [
        {
          name: "player"
          isMut: false
          isSigner: false
        },
        {
          name: "solVault"
          isMut: true
          isSigner: false
        },
        {
          name: "gameState"
          isMut: true
          isSigner: false
        },
        {
          name: "vrf"
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
      name: "close"
      accounts: [
        {
          name: "player"
          isMut: true
          isSigner: true
        },
        {
          name: "gameState"
          isMut: true
          isSigner: false
        }
      ]
      args: []
    }
  ]
  accounts: [
    {
      name: "gameState"
      type: {
        kind: "struct"
        fields: [
          {
            name: "guess"
            type: "u8"
          },
          {
            name: "bump"
            type: "u8"
          },
          {
            name: "maxResult"
            type: "u64"
          },
          {
            name: "resultBuffer"
            type: {
              array: ["u8", 32]
            }
          },
          {
            name: "result"
            type: "u128"
          },
          {
            name: "timestamp"
            type: "i64"
          },
          {
            name: "vrf"
            type: "publicKey"
          }
        ]
      }
    }
  ]
  errors: [
    {
      code: 6000
      name: "InvalidVrfAuthorityError"
      msg: "Switchboard VRF Account's authority should be set to the client's state pubkey"
    },
    {
      code: 6001
      name: "InvalidVrfAccount"
      msg: "Invalid VRF account provided."
    }
  ]
}

export const IDL: Vrf = {
  version: "0.1.0",
  name: "vrf",
  instructions: [
    {
      name: "initialize",
      accounts: [
        {
          name: "player",
          isMut: true,
          isSigner: true,
        },
        {
          name: "gameState",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vrf",
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
      name: "requestRandomness",
      accounts: [
        {
          name: "player",
          isMut: true,
          isSigner: true,
        },
        {
          name: "solVault",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameState",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vrf",
          isMut: true,
          isSigner: false,
        },
        {
          name: "oracleQueue",
          isMut: true,
          isSigner: false,
        },
        {
          name: "queueAuthority",
          isMut: true,
          isSigner: false,
        },
        {
          name: "dataBuffer",
          isMut: true,
          isSigner: false,
          docs: ["CHECK"],
        },
        {
          name: "permission",
          isMut: true,
          isSigner: false,
        },
        {
          name: "escrow",
          isMut: true,
          isSigner: false,
        },
        {
          name: "programState",
          isMut: true,
          isSigner: false,
        },
        {
          name: "switchboardProgram",
          isMut: false,
          isSigner: false,
        },
        {
          name: "payerWallet",
          isMut: true,
          isSigner: false,
        },
        {
          name: "recentBlockhashes",
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
      args: [
        {
          name: "permissionBump",
          type: "u8",
        },
        {
          name: "switchboardStateBump",
          type: "u8",
        },
        {
          name: "guess",
          type: "u8",
        },
      ],
    },
    {
      name: "consumeRandomness",
      accounts: [
        {
          name: "player",
          isMut: false,
          isSigner: false,
        },
        {
          name: "solVault",
          isMut: true,
          isSigner: false,
        },
        {
          name: "gameState",
          isMut: true,
          isSigner: false,
        },
        {
          name: "vrf",
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
      name: "close",
      accounts: [
        {
          name: "player",
          isMut: true,
          isSigner: true,
        },
        {
          name: "gameState",
          isMut: true,
          isSigner: false,
        },
      ],
      args: [],
    },
  ],
  accounts: [
    {
      name: "gameState",
      type: {
        kind: "struct",
        fields: [
          {
            name: "guess",
            type: "u8",
          },
          {
            name: "bump",
            type: "u8",
          },
          {
            name: "maxResult",
            type: "u64",
          },
          {
            name: "resultBuffer",
            type: {
              array: ["u8", 32],
            },
          },
          {
            name: "result",
            type: "u128",
          },
          {
            name: "timestamp",
            type: "i64",
          },
          {
            name: "vrf",
            type: "publicKey",
          },
        ],
      },
    },
  ],
  errors: [
    {
      code: 6000,
      name: "InvalidVrfAuthorityError",
      msg: "Switchboard VRF Account's authority should be set to the client's state pubkey",
    },
    {
      code: 6001,
      name: "InvalidVrfAccount",
      msg: "Invalid VRF account provided.",
    },
  ],
}
