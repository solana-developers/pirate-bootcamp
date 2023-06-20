# Setting Up and Running the Application

Follow the steps below to install dependencies and run the application.

## Install Dependencies

First, you need to install the necessary dependencies. Run the following command:

```
npm install
```

This command installs all the dependencies listed in the `package.json` file.

## Run the Application

After installing the dependencies, you can start the application by running:

```
npm run dev
```

This command starts the application in development mode.

## Using Your Own Coin Flip Program

If you've deployed your own version of the coin flip program to devnet and would like to use it, update the `programId` in the `utils/anchor.ts` file. Replace `YOUR_PROGRAM_ID` with the id of your deployed program.

```
// Coin flip game program ID
const programId = new PublicKey("YOUR_PROGRAM_ID")
```
