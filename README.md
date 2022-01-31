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
      {
            try
            {
                Keypair keypair = WalletKeyPair.GenerateKeyPairFromMnemonic(WalletKeyPair.GenerateNewMnemonic());

                toPublicKey = keypair.publicKey;

                RequestResult<ResponseValue<BlockHash>> blockHash = await activeRpcClient.GetRecentBlockHashAsync();

                var transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                    AddInstruction(SystemProgram.CreateAccount(account.GetPublicKey, toPublicKey, ammount,
                    (long)SystemProgram.AccountDataSize, SystemProgram.ProgramId))
                    .Build(new List<Account>() {
                    account,
                    new Account(keypair.privateKeyByte, keypair.publicKeyByte)
                    });

                RequestResult<string> firstSig = await activeRpcClient.SendTransactionAsync(Convert.ToBase64String(transaction));
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }
        }
```
- First create keypair(private key and public key),
- Then create blockHash from activeRpcClient,
- Initialize transaction 
- Send transaction

### Start connection
```C#
        public SolanaRpcClient StartConnection(EClientUrlSource clientUrlSource, string customUrl = "")
        {
            if (!string.IsNullOrEmpty(customUrl))
                this.customUrl = customUrl;

            try
            {
                if (activeRpcClient == null)
                {
                    activeRpcClient = new SolanaRpcClient(GetConnectionURL(clientUrlSource));
                }

                return activeRpcClient;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
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
 {
     password = LoadPlayerPrefs(passwordKey);
     try
     {
         string mnem = mnemonics;
         if (!WalletKeyPair.CheckMnemonicValidity(mnem))
         {
             return null;
             throw new Exception("Mnemonic is in incorect format");
         }

         this.mnemonics = mnemonics;
         string encryptedMnemonics = cypher.Encrypt(this.mnemonics, password);

         wallet = new Wallet(this.mnemonics, BIP39Wordlist.English);
         privateKey = wallet.Account.GetPrivateKey;

         webSocketService.SubscribeToWalletAccountEvents(wallet.Account.GetPublicKey);

         SavePlayerPrefs(mnemonicsKey, this.mnemonics);
         SavePlayerPrefs(encryptedMnemonicsKey, encryptedMnemonics);

         return wallet;
     }
     catch(Exception ex)
     {
         Debug.Log(ex);
         return null;
     }
 }
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
  {
      try
      {
          string encryptedMnemonics = LoadPlayerPrefs(encryptedMnemonicsKey);
          cypher.Decrypt(encryptedMnemonics, password);
          return true;
      }
      catch(Exception ex)
      {
          return false;
      }
  }
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
   {
       AccountInfo result = await AccountUtility.GetAccountData(account, activeRpcClient);
       if (result != null)
           return (double)result.Lamports / 1000000000;
       else
           return 0;
   }
 ```
 - Returns sol amount of forwarded account
 - Call example 
  ```C#
  double sol = await SimpleWallet.instance.GetSolAmmount(SimpleWallet.instance.wallet.GetAccount(0));
 ```
 ### Transfer sol
 ```C#
 public async Task<RequestResult<string>> TransferSol(string toPublicKey, long ammount = 10000000)
 {
     RequestResult<ResponseValue<BlockHash>> blockHash = await activeRpcClient.GetRecentBlockHashAsync();
     Account fromAccount = wallet.GetAccount(0);

     var transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
         AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toPublicKey, ammount)).Build(fromAccount);

     return await activeRpcClient.SendTransactionAsync(Convert.ToBase64String(transaction));
        }
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
{
    RequestResult<ResponseValue<BlockHash>> blockHash = await activeRpcClient.GetRecentBlockHashAsync();
    RequestResult<ulong> rentExemptionAmmount = await activeRpcClient.GetMinimumBalanceForRentExemptionAsync(SystemProgram.AccountDataSize);
    TokenAccount[] lortAccounts = await GetOwnedTokenAccounts(toWalletAccount, tokenMint, "");
    byte[] transaction;
    if (lortAccounts != null && lortAccounts.Length > 0)
    {
        transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
            AddInstruction(TokenProgram.Transfer(sourceTokenAccount,
            lortAccounts[0].pubkey,
            ammount,
            sourceAccountOwner.GetPublicKey))
            .Build(sourceAccountOwner);
    }
    else
    {
        Keypair newAccKeypair = WalletKeyPair.GenerateKeyPairFromMnemonic(WalletKeyPair.GenerateNewMnemonic());
        transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
            AddInstruction(
            SystemProgram.CreateAccount(
                sourceAccountOwner.GetPublicKey,
                newAccKeypair.publicKey,
                (long)rentExemptionAmmount.Result,
                SystemProgram.AccountDataSize,
                TokenProgram.ProgramId)).
                AddInstruction(
                TokenProgram.InitializeAccount(
                   newAccKeypair.publicKey,
                   tokenMint,
                   toWalletAccount)).
                AddInstruction(TokenProgram.Transfer(sourceTokenAccount,
                    newAccKeypair.publicKey,
                    ammount,
                    sourceAccountOwner.GetPublicKey))
                .Build(new List<Account>()
                {
                    sourceAccountOwner,
                    new Account(newAccKeypair.privateKeyByte,
                    newAccKeypair.publicKeyByte)
                });
    }

    return await activeRpcClient.SendTransactionAsync(Convert.ToBase64String(transaction));
}
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
{
   var result = await activeRpcClient.RequestAirdropAsync(account.GetPublicKey, ammount);
   return result.Result;
}
```
- Send 1 sol to our wallet (this is for testing).
- Call example
```C#
airdrop_btn.onClick.AddListener(async () => {
            await SimpleWallet.instance.RequestAirdrop(SimpleWallet.instance.wallet.GetAccount(0));
        });
```

## Introduction to WebsocketService.cs
- This class is located at Packages -> Solana Wallet -> Runtime -> UnityWebSocket -> WebSocketService.cs
### For WebSocket to work we must first create a connection calling StartConnection from WebSocketService.cs and forward address :
```C#
 public void StartConnection(string address)
 {
     _socket = new WebSocket(address);
     _socket.OnOpen += OnOpen;
     _socket.OnMessage += OnMessage;
     _socket.OnClose += OnClose;
     _socket.OnError += OnError;
     _socket.ConnectAsync();
 }
```
- In this function we create new WebSocket, then subscribe to events and open WebSocket connection.
- Call example
```C#
 webSocketService.StartConnection(GetWebsocketConnectionURL(clientSource));
```
### To subscribe Account on WebSocket events call function SubscribeToWalletAccountEvents and forward Wallet Pub key :
```C#
 public void SubscribeToWalletAccountEvents(string pubKey)
 {
     if (_socket is null) return;

     _subscriptionTypeReference = SubscriptionType.accountSubscribe;
     SendParameter(ReturnSubscribeParameter(pubKey));
 }
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
 {
     if (_socket is null) return;
     if (_subscriptionModel is null) return;

     _subscriptionTypeReference = SubscriptionType.accountUnsubscribe;
     SendParameter(ReturnUnsubscribeParameter());
     _subscriptionModel = null;
 }
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
    {
        switch (_subscriptionTypeReference)
        {
            case SubscriptionType.accountSubscribe:
                try
                {
                    _subscriptionModel = JsonConvert.DeserializeObject<SubscriptionModel>(e.Data);
                    MainThreadDispatcher.Instance().Enqueue(() => { WebSocketActions.WebSocketAccountSubscriptionAction.Invoke(true); });
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                break;
            case SubscriptionType.accountUnsubscribe:
                try
                {
                    _unsubscriptionModel = JsonConvert.DeserializeObject<UnsubsciptionModel>(e.Data);
                    MainThreadDispatcher.Instance().Enqueue(() => { WebSocketActions.WebSocketAccountSubscriptionAction.Invoke(false); });
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                break;
        }
    }
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
