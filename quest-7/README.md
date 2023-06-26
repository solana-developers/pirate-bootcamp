# Solana Pay -- Pirates Edition

## Introduction

You'll need a mobile wallet that supports Transaction Requests to participate in this workshop.

- Phantom (iOS, Android)
- Solflare (iOS, Android)

### Testing w/ Vercel and ngrok

#### Vercel

> You could use any cloud provider that supports serverless functions, but we'll use Vercel for this workshop.

Register for a free account at [Vercel](https://vercel.com/signup)

#### ngrok

Register for a free account at [ngrok](https://ngrok.com/)

ngrok is the fastest way to put your app on the internet. Test your development server without deploying.

Start local development server:

```bash
npx vercel dev
```

In another terminal, start ngrok:

```bash
ngrok http 3000
```

## Resources

- [Workshop Slides](https://docs.google.com/presentation/d/1VA5UeP0fhQLI7jy-dHbN-jZ79XqvrV-kEQhW7FLmdwM/edit?usp=sharing)
- [Workshop guide](./WORKSHOP.md)
- [Solana Pay docs](https://docs.solanapay.com)
- [Solana Pay Github Repo](https://github.com/solana-labs/solana-pay)
