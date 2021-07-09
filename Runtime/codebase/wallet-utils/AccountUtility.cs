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

namespace AllArt.Solana.Utility
{
    public static class AccountUtility
    {
        public static async Task<AccountInfo> GetAccountData(string accountPublicKey, SolanaRpcClient rpcClient)
        {
            RequestResult<ResponseValue<AccountInfo>> result = await rpcClient.GetAccountInfoAsync(accountPublicKey);
            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

        public static async Task<AccountInfo> GetAccountData(Account account, SolanaRpcClient rpcClient)
        {
            RequestResult<ResponseValue<AccountInfo>> result = await rpcClient.GetAccountInfoAsync(account.GetPublicKey);
            if (result.Result != null && result.Result.Value != null)
            {
                return result.Result.Value;
            }
            return null;
        }

        public static async Task<TokenBalance> GetTokenBalance(string tokenPubKey, SolanaRpcClient rpcClient)
        {
            RequestResult<ResponseValue<TokenBalance>> result = await rpcClient.GetTokenAccountBalanceAsync(tokenPubKey);
            if (result.Result != null)
                return result.Result.Value;
            else
                throw new Exception("No balance for this token reveived");
        }

        public static async void CreateAccount(Account account, SolanaRpcClient rpcClient, string toPublicKey = "", long ammount = 1000)
        {
            Keypair k = WalletKeyPair.GenerateKeyPairFromMnemonic(WalletKeyPair.GenerateNewMnemonic());
            toPublicKey = k.publicKey;

            RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                AddInstruction(SystemProgram.CreateAccount(account.GetPublicKey, toPublicKey, ammount, (long)SystemProgram.AccountDataSize, SystemProgram.ProgramId)).Build(new List<Account>() { account, new Account(k.privateKeyByte, k.publicKeyByte) });

            RequestResult<string> firstSig = await rpcClient.SendTransactionAsync(Convert.ToBase64String(tx));
        }

        public static async Task<RequestResult<ResponseValue<TokenBalance>>> GetTokenSupply(string key, SolanaRpcClient rpcClient)
        {
            RequestResult<ResponseValue<TokenBalance>> supply = await rpcClient.GetTokenSupplyAsync(key);
            return supply;
        }
    }
}
