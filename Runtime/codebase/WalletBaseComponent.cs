using AllArt.Solana.Utility;
using dotnetstandard_bip39;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AllArt.Solana
{
    public class WalletBaseComponent : MonoBehaviour
    {
        #region Connections
        public static string devNetAdress = "https://api.devnet.solana.com";
        public static string testNetAdress = "https://api.testnet.solana.com";
        public static string mainNetAdress = "https://api.mainnet.solana.com";

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
            if (autoConnectOnStartup)
            {
                StartConnection(clientSource);
            }
        }

        string GetConnectionURL(EClientUrlSource clientUrlSource)
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

        #endregion

        public Wallet wallet { get; set; }
        public string mnemonics { get; private set; }

        public async void CreateAccount(Account account, string toPublicKey = "", long ammount = 1000)
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

        public async Task<AccountInfo> GetAccountData(Account account)
        {
            RequestResult<ResponseValue<AccountInfo>> result = await activeRpcClient.GetAccountInfoAsync(account.GetPublicKey);
            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

        public async Task<TokenAccount[]> GetOwnedTokenAccounts(string walletPubKey, string tokenMintPubKey, string tokenProgramPublicKey)
        {
            RequestResult<ResponseValue<TokenAccount[]>> result = await activeRpcClient.GetTokenAccountsByOwnerAsync(walletPubKey, tokenMintPubKey, tokenProgramPublicKey);
            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

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

        public async Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenSupply(string key)
        {
            RequestResult<ResponseValue<TokenBalance>> supply = await activeRpcClient.GetTokenSupplyAsync(key);
            return supply;
        }

        public SolanaRpcClient StartConnection(EClientUrlSource clientUrlSource, string customUrl = "")
        {
            if (!string.IsNullOrEmpty(customUrl))
                this.customUrl = customUrl;

            if (activeRpcClient == null)
            {
                activeRpcClient = new SolanaRpcClient(GetConnectionURL(clientUrlSource));
            }

            return activeRpcClient;
        }

        public Wallet GenerateWalletWithMenmonic(string mnemonics)
        {
            string mnem = mnemonics;
            if (!WalletKeyPair.CheckMnemonicValidity(mnem))
            { 
                return null;
                throw new Exception("Mnemonic is in incorect format");
            }

            this.mnemonics = mnemonics;

            wallet = new Wallet(this.mnemonics, BIP39Wordlist.English);
            PlayerPrefs.SetString("Mnemonics", this.mnemonics);

            return wallet;
        }

        public bool LoadSavedWallet()
        {
            string mnemonicWords = string.Empty;
            if (PlayerPrefs.HasKey("Mnemonics"))
            {
                mnemonicWords = PlayerPrefs.GetString("Mnemonics");
                wallet = new Solnet.Wallet.Wallet(mnemonicWords, BIP39Wordlist.English);
                return true;
            }
            return false;
        }

        public async Task<double> GetSolAmmount(Account account)
        {
            AccountInfo result = await AccountUtility.GetAccountData(account, activeRpcClient);
            if (result != null)
                return (double)result.Lamports / 1000000000;
            else
                return 0;
        }

        public async void TransferSol(Account fromAccount, string toPublicKey, long ammount = 10000000)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await activeRpcClient.GetRecentBlockHashAsync();

            var transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toPublicKey, ammount)).Build(fromAccount);

            RequestResult<string> firstSig = await activeRpcClient.SendTransactionAsync(Convert.ToBase64String(transaction));
        }

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

        public async Task<RequestResult<string>> TransferSol(string toPublicKey, long ammount = 10000000)
        {
            RequestResult<ResponseValue<BlockHash>> blockHash = await activeRpcClient.GetRecentBlockHashAsync();
            Account fromAccount = wallet.GetAccount(0);

            var transaction = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toPublicKey, ammount)).Build(fromAccount);

            return await activeRpcClient.SendTransactionAsync(Convert.ToBase64String(transaction));
        }

        public async Task<string> RequestAirdrop(Account account, ulong ammount = 1000000000)
        {
            var result = await activeRpcClient.RequestAirdropAsync(account.GetPublicKey, ammount);
            return result.Result;
        }

        public async Task<TokenAccount[]> GetOwnedTokenAccounts(Account account)
        {
            RequestResult<ResponseValue<TokenAccount[]>> result = await activeRpcClient.GetTokenAccountsByOwnerAsync(account.GetPublicKey, "", TokenProgram.ProgramId);
            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

        public void DeleteWalletAndClearKey()
        {
            PlayerPrefs.DeleteKey("Mnemonics");
            wallet = null;
        }
    }
}
