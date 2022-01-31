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
- Save private key in txt file

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
5. Set Client Source (Mainnet/Testnet/Devnet/Custom uri) and Storage Method (Json/Simple txt) on SimpleWallet script in WalletController prefab.
6. If you use custom URI be careful to use WS/WSS instead of HTTP/HTTPS because WebSocket does not work with HTTP / HTTPS.
7. To save mnemonics in JSON format, select the JSON storage method, and if you want to save it as a regular string, select Simple Txt.
8. If you want to use mnemonics saved in JSON format, you must deserialize it first. You have an example in ReGenerateAccountScreen.cs in the ResolveMnemonicsByType method.
9. Import WalletHolder prefab or if you want your design just import wallet prefab and customize the scene like we did with WalletHolder.

## Functionalities description
### Login Screen
- If you have already logged in to your wallet, then your mnemonics are stored and encrypted in memory with your password and you can log in with that password. Otherwise you have to create or restore a wallet.

### Create Wallet Screen
- You now have automatically generated mnemonics and to successfully create a wallet you must enter a password with which the mnemonics will be encrypted. I recommend that you use the Save Mnemonics option and save them to a text file. Then press create button to create a wallet.

### Regenerate Wallet Screen
- If you have saved mnemonics and want to recreate a wallet with it, load them by pressing Load Mnemonics button and generate the password again. Now your wallet is regenerated and the amount of SOL and NFT will be reloaded.

### Wallet Screen
- After you successfully logged in / generated / regenerated a wallet you will automatically be transferred to the wallet screen. Now you are shown SOL balance and your NFT's and you are automatically subscribed to the account via the websocket. This allows you to track changes in your account (automatic refreshing of SOL balance when a balance changes, etc..).

### Recieve Screen
- To facilitate testing, there is an Airdrop option in the Recieve section. Click on the Airdrop button, return to the Wallet Screen and wait a few seconds to see the change in SOL balance.

### Transfer Screen
- To complete the transaction enter the wallet pubkey and the amount you want to send. Then return to the wallet screen and wait a few seconds for the SOL Balance to refresh.

## Introduction to WalletBaseComponent.cs
- This class is located at Packages -> Solana Wallet -> Runtime -> codebase -> WalletBaseComponent.cs

### Create account
```C#
public async void CreateAccount(Account account, string toPublicKey = "", long ammount = 1000)
```
- First create keypair(private key and public key),
- Then create blockHash from activeRpcClient,
- Initialize transaction 
- Send transaction

### Start connection
```C#
public SolanaRpcClient StartConnection(EClientUrlSource clientUrlSource, string customUrl = "")
```
- For starting RPC connection call StartConnection and forward clientSource.
- Function returns new connected RPC client.
- Call example 
 ```C#
 StartConnection(clientSource);
 ```
  
### Generate wallet with mnemonics
```C#
 public Wallet GenerateWalletWithMenmonic(string mnemonics)
 ```
 - First check forwarded mnemonics validity.
 - Encrypt mnemonics with password
 - Create new wallet from mnemonics
 - Subscribe to WebSocket
 - Save mnemonics and encrypted mnemonics in memory
 - Call example
 ```C#
 SimpleWallet.instance.GenerateWalletWithMenmonic(_simpleWallet.LoadPlayerPrefs(_simpleWallet.MnemonicsKey));
 ```
### Login check with mnemonics and password
 ```C#
  public bool LoginCheckMnemonicAndPassword(string password)
 ```
 - Try to encrypt decrypted mnemonics with typed password.
 - Return true or false
 - Call example 
  ```C#
  private void LoginChecker()
  {
      if (_simpleWallet.LoginCheckMnemonicAndPassword(_passwordInputField.text))
      {
          SimpleWallet.instance.GenerateWalletWithMenmonic(_simpleWallet.LoadPlayerPrefs(_simpleWallet.MnemonicsKey));
          MainThreadDispatcher.Instance().Enqueue(() => { _simpleWallet.StartWebSocketConnection(); }); 
          manager.ShowScreen(this, "wallet_screen");
          this.gameObject.SetActive(false);
      }
      else
      {
          SwitchButtons("TryAgain");
      }
  }
 ```
 ### Get sol amount
  ```C#
  public async Task<double> GetSolAmmount(Account account)
 ```
 - Returns sol amount of forwarded account
 - Call example 
  ```C#
  double sol = await SimpleWallet.instance.GetSolAmmount(SimpleWallet.instance.wallet.GetAccount(0));
 ```
 ### Transfer sol
 ```C#
 public async Task<RequestResult<string>> TransferSol(string toPublicKey, long ammount = 10000000)
 ```
 - Executes sol transaction from one account to another one for forwarded amount.
 - Call example 
 ```C#
 private async void TransferSol()
 {
     RequestResult<string> result = await SimpleWallet.instance.TransferSol(toPublic_txt.text, long.Parse(ammount_txt.text));
     HandleResponse(result);
 }
 ```
 ### Transfer token
 ```C#
 public async Task<RequestResult<string>> TransferToken(string sourceTokenAccount, string toWalletAccount, Account sourceAccountOwner, string tokenMint, long ammount = 1)
 ```
 - Executes SOL transaction from one account to another one
 - Call example
 ```C#
 private async void TransferToken()
 {
     RequestResult<string> result = await SimpleWallet.instance.TransferToken(
                         transferTokenAccount.pubkey,
                         toPublic_txt.text,
                         SimpleWallet.instance.wallet.GetAccount(0),
                         transferTokenAccount.Account.Data.Parsed.Info.Mint,
                         long.Parse(ammount_txt.text));

     HandleResponse(result);
 }
```
### Request airdrop
```C#
public async Task<string> RequestAirdrop(Account account, ulong ammount = 1000000000)
```
- Send 1 sol to our wallet (this is for testing).
- Call example
```C#
airdrop_btn.onClick.AddListener(async () => {
            await SimpleWallet.instance.RequestAirdrop(SimpleWallet.instance.wallet.GetAccount(0));
        });
```
### Get owned token accounts
```C#
public async Task<TokenAccount[]> GetOwnedTokenAccounts(Account account)
```
- Returns array of tokens on the account
- Call example 
```C#
TokenAccount[] result = await SimpleWallet.instance.GetOwnedTokenAccounts(SimpleWallet.instance.wallet.GetAccount(0));
```
### Delete wallet and clear key
```C#
public void DeleteWalletAndClearKey()
```
- Unsubscribe from WebSocket events
- Delete used wallet

### Start WebSocket connection
```C#
public void StartWebSocketConnection()
```
- Starts WebSocket connection when user is logged in.

## Introduction to WebsocketService.cs
- This class is located at Packages -> Solana Wallet -> Runtime -> UnityWebSocket -> WebSocketService.cs
### For WebSocket to work we must first create a connection calling StartConnection from WebSocketService.cs and forward address :
```C#
public void StartConnection(string address)
```
- In this function we create new WebSocket, then subscribe to events and open WebSocket connection.
- Call example
```C#
 webSocketService.StartConnection(GetWebsocketConnectionURL(clientSource));
```
### To subscribe Account on WebSocket events call function SubscribeToWalletAccountEvents and forward Wallet Pub key :
```C#
 public void SubscribeToWalletAccountEvents(string pubKey)
```
- First set subscriptionTypeReference to know which event we are processing (in this case it is accountSubscribe).
- Then call SendParameter and forward parameter for account subscription.
- Call example 
```C#
 webSocketService.SubscribeToWalletAccountEvents(wallet.Account.GetPublicKey);
```
### To unsubscribe Account from WebSocket events call function UnsubscribeToWalletAccountEvents :
```C#
 public void UnSubscribeToWalletAccountEvents()
```
- First set subscriptionTypeReference to know which event we are processing (in this case it is accountUnsubscribe).
- Then call SendParameter and forward parameter for account unsubscription.
- Call example 
 ```C#
 public void StartWebSocketConnection()
 {
     if (webSocketService.Socket != null) return;

     webSocketService.StartConnection(GetWebsocketConnectionURL(clientSource));
 }
```
### To respond to websocket events we use WebSocket actions that we call in OnMessage function : 
 ```C#
 private void OnMessage(object sender, MessageEventArgs e)
```
- Depending on the SubscriptionTypeReference, we deserialize the message into a model.
- Invoke WebSocketAction
- Then subscribe the desired functionality to the action
```C#
 WebSocketActions.WebSocketAccountSubscriptionAction += CheckSubscription;
```

### To close WebSocket connection call CloseConnection :
```C#
 public void CloseConnection()
 {
     if (_socket == null) return;

     _socket.CloseAsync();
 }
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Solnet/blob/master/LICENSE) file for details
