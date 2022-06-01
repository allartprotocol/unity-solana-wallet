using AllArt.Solana.Utility;
using dotnetstandard_bip39;
using Merkator.BitCoin;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AllArt.Solana
{
    [RequireComponent(typeof(Startup))]
    [RequireComponent(typeof(MainThreadDispatcher))]
    public class WalletBaseComponent : MonoBehaviour
    {
        #region Player Prefs Keys

        private string mnemonicsKey = "Mnemonics";
        private string passwordKey = "Password";
        private string encryptedMnemonicsKey = "EncryptedMnemonics";
        private string privateKeyKey = "PrivateKey";
        #endregion
        #region Connections
        public static string devNetAdress = "https://api.devnet.solana.com";
        public static string testNetAdress = "https://api.testnet.solana.com";
        public static string mainNetAdress = "https://api.mainnet-beta.solana.com";

        public static string webSocketDevNetAdress = "ws://api.devnet.solana.com";
        public static string webSocketTestNetAdress = "ws://api.testnet.solana.com";
        public static string webSocketMainNetAdress = "ws://api.mainnet-beta.solana.com";

        public string customUrl = "http://192.168.0.22:8899";

        public enum EClientUrlSource
        {
            EDevnet,
            EMainnet,
            ETestnet,
            ECustom
        }

        public EClientUrlSource clientSource;
        public bool autoConnectOnStartup = false;

        public SolanaRpcClient activeRpcClient { get; private set; }


        public virtual void Awake()
        {
            webSocketService = new WebSocketService();
            cypher = new Cypher();

            if (autoConnectOnStartup)
            {
                StartConnection(clientSource);
                webSocketService.StartConnection(GetWebsocketConnectionURL(clientSource));
            }
        }

        public void OnDestroy()
        {
            webSocketService.CloseConnection();
        }

        /// <summary>
        /// Returns the url of the desired client source
        /// </summary>
        /// <param name="clientUrlSource"> Desired client source</param>
        /// <returns></returns>
        public string GetConnectionURL(EClientUrlSource clientUrlSource)
        {
            string url = "";
            switch (clientUrlSource)
            {
                case EClientUrlSource.ECustom:
                    url = customUrl;
                    break;
                case EClientUrlSource.EDevnet:
                    url = devNetAdress;
                    break;
                case EClientUrlSource.EMainnet:
                    url = mainNetAdress;
                    break;
                case EClientUrlSource.ETestnet:
                    url = testNetAdress;
                    break;
            }
            return url;
        }

        /// <summary>
        /// Returns the websocket url of the desired client source
        /// </summary>
        /// <param name="clientUrlSource"> Desired client source</param>
        /// <returns></returns>
        public string GetWebsocketConnectionURL(EClientUrlSource clientUrlSource)
        {
            string url = "";
            switch (clientUrlSource)
            {
                case EClientUrlSource.ECustom:
                    url = customUrl;
                    break;
                case EClientUrlSource.EDevnet:
                    url = webSocketDevNetAdress;
                    break;
                case EClientUrlSource.EMainnet:
                    url = webSocketMainNetAdress;
                    break;
                case EClientUrlSource.ETestnet:
                    url = webSocketTestNetAdress;
                    break;
            }
            return url;
        }

        #endregion
        public Wallet wallet { get; set; }
        public string mnemonics { get; private set; }
        public string password { get; private set; }
        public string privateKey { get; private set; }

        [HideInInspector]
        public WebSocketService webSocketService;
        private Cypher cypher;

        /// <summary>
        /// Creates private and public key with mnemonics, then starts RPC connection and creates Account
        /// </summary>
        /// <param name="account">Account to create</param>
        /// <param name="toPublicKey">Public key of Account</param>
        /// <param name="ammount">SOL amount</param>
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
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        /// <summary>
        /// Returns the account data for the forwarded account
        /// </summary>
        /// <param name="account">Forwarded account for which we want to return data</param>
        /// <returns></returns>
        public async Task<AccountInfo> GetAccountData(Account account)
        {
            RequestResult<ResponseValue<AccountInfo>> result = await activeRpcClient.GetAccountInfoAsync(account.GetPublicKey);
            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

        /// <summary>
        /// Returns tokens held by the forwarded account
        /// </summary>
        /// <param name="walletPubKey">Pub key of the wallet for which we want to return tokens</param>
        /// <param name="tokenMintPubKey"></param>
        /// <param name="tokenProgramPublicKey"></param>
        /// <returns></returns>
        public async Task<TokenAccount[]> GetOwnedTokenAccounts(string walletPubKey, string tokenMintPubKey, string tokenProgramPublicKey)
        {
            RequestResult<ResponseValue<TokenAccount[]>> result = await activeRpcClient.GetTokenAccountsByOwnerAsync(walletPubKey, tokenMintPubKey, tokenProgramPublicKey);
            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

        /// <summary>
        /// Returns tokens held by the forwarded account
        /// </summary>
        /// <param name="walletPubKey">Pub key of the wallet for which we want to return tokens</param>
        /// <param name="tokenMintPubKey"></param>
        /// <param name="tokenProgramPublicKey"></param>
        /// <returns></returns>
        public async Task<TokenAccount[]> GetOwnedTokenAccounts(Account account, string tokenMintPubKey, string tokenProgramPublicKey)
        {
            RequestResult<ResponseValue<TokenAccount[]>> result = await activeRpcClient.GetTokenAccountsByOwnerAsync(
                account.GetPublicKey,
                tokenMintPubKey,
                tokenProgramPublicKey);

            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

        /// <summary>
        /// Returns token balance for forwarded token public key
        /// </summary>
        /// <param name="tokenPubKey"> Public key token for which we want to return balance</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TokenBalance> GetTokenBalance(string tokenPubKey)
        {
            RequestResult<ResponseValue<TokenBalance>> result = await activeRpcClient.GetTokenAccountBalanceAsync(tokenPubKey);
            if (result.Result != null)
                return result.Result.Value;
            else
            {
                return null;
                throw new Exception("No balance for this token reveived");
            }
        }

        /// <summary>
        /// Returns token supply for forwarded token public key
        /// </summary>
        /// <param name="tokenPubKey"> Public key token for which we want to return supply</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenSupply(string key)
        {
            RequestResult<ResponseValue<TokenBalance>> supply = await activeRpcClient.GetTokenSupplyAsync(key);
            return supply;
        }

        /// <summary>
        /// Start RPC connection and return new RPC Client 
        /// </summary>
        /// <param name="clientUrlSource">Choosed client source</param>
        /// <param name="customUrl">Custom url for rpc connection</param>
        /// <returns></returns>
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
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a wallet of forwarded mnemonics. Decrypts them with a passed password and stores them in memory, then start Websocket connection to Wallet
        /// </summary>
        /// <param name="mnemonics">Mnemonics by which we generate a wallet</param>
        /// <returns></returns>
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
            catch (Exception ex)
            {
                Debug.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// Recreates a wallet if we have already been logged in and have mnemonics saved in memory
        /// </summary>
        /// <returns></returns>
        public bool LoadSavedWallet()
        {
            string mnemonicWords = string.Empty;
            if (PlayerPrefs.HasKey(mnemonicsKey))
            {
                try
                {
                    mnemonicWords = LoadPlayerPrefs(mnemonicsKey);

                    wallet = new Wallet(mnemonicWords, BIP39Wordlist.English);
                    webSocketService.SubscribeToWalletAccountEvents(wallet.Account.GetPublicKey);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// At each login tries to decrypt encrypted mnemonics with the entered password
        /// </summary>
        /// <param name="password"> Password by which we will try to decrypt the mnemonics</param>
        /// <returns></returns>
        public bool LoginCheckMnemonicAndPassword(string password)
        {
            try
            {
                string encryptedMnemonics = LoadPlayerPrefs(encryptedMnemonicsKey);
                cypher.Decrypt(encryptedMnemonics, password);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns amount of SOL for Account
        /// </summary>
        /// <param name="account">Account for which we want to check the SOL balance</param>
        /// <returns></returns>
        public async Task<double> GetSolAmmount(Account account)
        {
            AccountInfo result = await AccountUtility.GetAccountData(account, activeRpcClient);
            if (result != null)
                return (double)result.Lamports / 1000000000;
            else
                return 0;
        }

        /// <summary>
        /// Executes a SOL transaction from one account to another
        /// </summary>
        /// <param name="fromAccount">The Account from which we perform the transaction</param>
        /// <param name="toPublicKey">The Account on which we perform the transaction</param>
        /// <param name="ammount">Ammount of sol</param>
        public async void TransferSol(Account fromAccount, string toPublicKey, long ammount = 10000000)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await activeRpcClient.GetRecentBlockHashAsync();

            var transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toPublicKey, ammount)).Build(fromAccount);

            RequestResult<string> firstSig = await activeRpcClient.SendTransactionAsync(Convert.ToBase64String(transaction));
        }

        /// <summary>
        /// Executes a token transaction on the desired wallet
        /// </summary>
        /// <param name="sourceTokenAccount">Pub Key of the wallet from which we make the transaction</param>
        /// <param name="toWalletAccount">The Pub Key of the wallet to which we want to make a transaction</param>
        /// <param name="sourceAccountOwner">The Account from which we send tokens</param>
        /// <param name="tokenMint"></param>
        /// <param name="ammount">Ammount of tokens we want to send</param>
        /// <returns></returns>
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

        public async Task<List<TransactionInstructionForJS>> TransferTokenForPhantom(string sourceTokenAccount, string toWalletAccount, string pubKey, string tokenMint, long ammount = 1)
        {
            var createAccount = SystemProgram.CreateAccountForJS(
                        pubKey,
                        toWalletAccount,
                        ammount,
                        SystemProgram.AccountDataSize,
                        TokenProgram.ProgramId);

            var initializeAccount = TokenProgram.InitializeAccountForJS(
                        sourceTokenAccount,
                        tokenMint,
                        toWalletAccount);

            var transfer = TokenProgram.TransferForJS(sourceTokenAccount,
                        toWalletAccount,
                        ammount,
                        pubKey);

            List<TransactionInstructionForJS> newList = new List<TransactionInstructionForJS>();
            newList.Add(createAccount);
            newList.Add(initializeAccount);
            newList.Add(transfer);

            return newList;
        }

        /// <summary>
        /// The key of the account on which we want to execute the transaction
        /// </summary>
        /// <param name="toPublicKey"> Public key of wallet on which we want to execute the transaction </param>
        /// <param name="ammount"> Ammount of sol we want to send</param>
        /// <returns></returns>
        public async Task<RequestResult<string>> TransferSol(string toPublicKey, long ammount = 10000000)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await activeRpcClient.GetRecentBlockHashAsync();
            Account fromAccount = wallet.GetAccount(0);

            var transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toPublicKey, ammount)).Build(fromAccount);

            return await activeRpcClient.SendTransactionAsync(Convert.ToBase64String(transaction));
        }

        /// <summary>
        /// Airdrop sol on wallet
        /// </summary>
        /// <param name="account">Account to which send sol</param>
        /// <param name="ammount">Amount of sol</param>
        /// <returns>Amount of sol</returns>
        public async Task<string> RequestAirdrop(Account account, ulong ammount = 1000000000)
        {
            var result = await activeRpcClient.RequestAirdropAsync(account.GetPublicKey, ammount);
            return result.Result;
        }

        /// <summary>
        /// Returns an array of tokens on the account
        /// </summary>
        /// <param name="account">The account for which we are requesting tokens</param>
        /// <returns>Array of tokens</returns>
        public async Task<TokenAccount[]> GetOwnedTokenAccounts(Account account)
        {
            try
            {
                RequestResult<ResponseValue<TokenAccount[]>> result = await activeRpcClient.GetTokenAccountsByOwnerAsync(account.GetPublicKey, "", TokenProgram.ProgramId);
                if (result.Result != null && result.Result.Value != null)
                {
                    return result.Result.Value;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            return null;
        }

        public async Task<TokenAccount[]> GetOwnedTokenAccountsByPublicKey(string pubKey)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(pubKey);
                string publicKey = Base58Encoding.Encode(bytes);
                RequestResult<ResponseValue<TokenAccount[]>> result = await activeRpcClient.GetTokenAccountsByOwnerAsync(publicKey, "", TokenProgram.ProgramId);
                if (result.Result != null && result.Result.Value != null)
                {
                    return result.Result.Value;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            return null;
        }

        /// <summary>
        /// It disconnects the websocket connection and deletes the wallet we were logged into
        /// </summary>
        public void DeleteWalletAndClearKey()
        {
            webSocketService.UnSubscribeToWalletAccountEvents();
            wallet = null;
        }

        /// <summary>
        /// A function that automatically initiates a websocket connection to the wallet when we log in
        /// </summary>
        public void StartWebSocketConnection()
        {
            if (webSocketService.Socket != null) return;

            webSocketService.StartConnection(GetWebsocketConnectionURL(clientSource));
        }

        #region Data Functions
        public void SavePlayerPrefs(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
#if UNITY_WEBGL
            PlayerPrefs.Save();
#endif
        }

        public string LoadPlayerPrefs(string key)
        {
            return PlayerPrefs.GetString(key);
        }
        #endregion

        #region Getters And Setters
        public string MnemonicsKey => mnemonicsKey;
        public string EncryptedMnemonicsKey => encryptedMnemonicsKey;
        public string PasswordKey => passwordKey;
        public string PrivateKeyKey => privateKeyKey;
        #endregion
    }
}
