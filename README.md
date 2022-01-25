<div style="text-align:center">

<p>

# Unity-Solana Wallet
The First Open-Source Unity-Solana Wallet with NFT support

The Unity-Solana Wallet is an essential bridge connecting game developers and the Solana blockchain. With Solana’s quick and low-cost transactions, games can start using blockchain technology like never before - in real-time. Thousands of developers will bring millions of players into the Solana ecosystem. This will, in turn, make the Solana projects benefit from an increased number of participants, and cross-protocol interoperability in games will take us beyond the current understanding of DeFi.  
  
Unity-Solana Wallet uses Solnet's implementation .NET SDK, but we had to modify the library to make it Unity compatible with .NET Standard 2.0 and .NET 4.x.
Solnet is Solana's .NET SDK to integrate with the .NET ecosystem.  [Solnet](https://blockmountain.io/Solnet/).

</p>

</div>


## Features
- Create/Backup wallet with mnemonic phrase
- Account handling
- Transaction building
- SOL balance 
- SPL-token balances
- SPL-token transfers
- Basic UI examples 
- WebSocket subscription
- Save and load mnemonics from local txt file

## Dependencies
- Newtonsoft.Json
- Chaos.NaCl.Standard
- Portable.BouncyCastle
- Zxing

## External packages
- Native File Picker
- Standalone File Browser

## Roadmap
- Multiple wallet accounts
- Camera support with QR code scanning for token transfers
- Improved UI for in-game easy integration
- Metaplex NFT / NFT-PRO support with GameObjects 
- Token swaps
- NFT swaps
- One-click in-game currency creator
- Themed UI support
- Metaplex auctions for in-game store items

## Installation

1. Clone this repository outside of the main Unity project
2. Go to Package Manager in your project
3. Click on the plus in the top left corner and select "Add package from disk"
4. Select package.json file from a cloned dir
5. Once the package is installed, in the Package Manager inspector you will have Samples. Click on Import

## Step-by-step instructions
1. If you have an older version of Unity that doesn't have imported Newtonsoft.Json just import it.
2. After importing the wallet Unity will throw unity-plastic error. Just restart Unity.
3. Create a new scene.
4. Import WalletController prefab into your scene.
5. Set Client Source and Storage Method on SimpleWallet script in WalletController prefab.
6. If you use custom URI be careful to use WS/WSS instead of HTTP/HTTPS because WebSocket does not work with HTTP / HTTPS.
7. To save mnemonics in JSON format, select the JSON storage method, and if you want to save it as a regular string, select Simple Txt.
8. If you want to use mnemonics saved in JSON format, you must deserialize it first. You have an example in ReGenerateAccountScreen.cs in the ResolveMnemonicsByType method.
9. Import WalletHolder prefab or if you want your design just import wallet prefab and customize the scene like we did with WalletHolder.

## Functionalities description
### Login Screen
If you have already logged in to your wallet, then your mnemonics are stored and encrypted in memory with your password and you can log in with that password. Otherwise you have to create or restore a wallet.

### Create Wallet Screen
You now have automatically generated mnemonics and to successfully create a wallet you must enter a password with which the mnemonics will be encrypted. I recommend that you use the Save Mnemonics option and save them to a text file. Then press create button to create a wallet.

### Regenerate Wallet Screen
If you have saved mnemonics and want to recreate a wallet with it, load them by pressing Load Mnemonics button and generate the password again. Now your wallet is regenerated and the amount of SOL and NFT will be reloaded.

### Wallet Screen
After you successfully logged in / generated / regenerated a wallet you will automatically be transferred to the wallet screen. Now you are shown SOL balance and your NFT's and you are automatically subscribed to the account via the websocket. This allows you to track changes in your account (automatic refreshing of SOL balance when a balance changes, etc..).

### Recieve Screen
To facilitate testing, there is an Airdfrop option in the Recieve section. Click on the Airdrop button, return to the Wallet Screen and wait a few seconds to see the change in SOL balance.

### Transfer Screen
To complete the transaction enter the wallet pubkey and the amount you want to send. Then return to the wallet screen and wait a few seconds for the SOL Balance to refresh.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Solnet/blob/master/LICENSE) file for details
