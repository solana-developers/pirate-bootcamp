# Anchor Solana Program

Update `wallet` in the `Anchor.toml` file with the path to your local keypair.
You can use the `solana config get` command to find your default Keypair Path.

```
[provider]
cluster = "Localnet"
wallet = "UPDATE_THIS"
```

Build your Anchor program by running the following command:

```shell
anchor build
```

Once you're build the program, run `anchor keys list`

```
anchor keys list
```

The output will display the program ID generated for your program, which should look similar to this:

```
coin_flip: BUVh5GcKPTkEhiHtw3Nv3eUANnxEALcj7nNqwNTe18va
```

Copy the program ID and update it in the `lib.rs` file:

```
declare_id!("BUVh5GcKPTkEhiHtw3Nv3eUANnxEALcj7nNqwNTe18va");
```

Also, update the program ID in the `Anchor.toml` file:

```
[programs.localnet]
coin_flip = "BUVh5GcKPTkEhiHtw3Nv3eUANnxEALcj7nNqwNTe18va"
```

Install the client dependencies using the following commands:

```shell
yarn install
yarn add ts-mocha
```

Finally, run the tests to ensure everything is working as expected:

```
anchor test

```
