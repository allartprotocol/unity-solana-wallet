using Solnet.Rpc.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllArt.Solana.Example
{
    public class WalletScreen : SimpleScreen
    {
        public TextMeshProUGUI lamports;
        public Button refresh_btn;
        public Button send_btn;
        public Button receive_btn;
        public Button logout_btn;

        public List<TokenItem> token_items;

        public KnownTokens knownTokens;

        CancellationTokenSource stopTask;

        void Start()
        {
            refresh_btn?.onClick.AddListener(() =>
            {
                UpdateWalletBalanceDisplay();
                GetOwnedTokenAccounts();
            });

            send_btn?.onClick.AddListener(() =>
            {
                TransitionToTransfer();
            });

            receive_btn?.onClick.AddListener(() =>
            {
                manager.ShowScreen(this, "receive_screen");
            });

            logout_btn.onClick.AddListener(() =>
            {
                SimpleWallet.instance.DeleteWalletAndClearKey();
                manager.ShowScreen(this, "generate_screen");
            });

            stopTask = new CancellationTokenSource();
        }

        private void TransitionToTransfer(object data = null)
        {
            manager.ShowScreen(this, "transfer_screen", data);
        }

        private async void UpdateWalletBalanceDisplay()
        {
            double sol = await SimpleWallet.instance.GetSolAmmount(SimpleWallet.instance.wallet.GetAccount(0));
            lamports.text = $"{sol}";
        }

        public override void ShowScreen(object data = null)
        {
            base.ShowScreen();
            gameObject.SetActive(true);

            GetOwnedTokenAccounts();
            UpdateWalletBalanceDisplay();
        }

        public override void HideScreen()
        {
            base.HideScreen();
            gameObject.SetActive(false);
        }

        public async void GetOwnedTokenAccounts()
        {
            DisableTokenItems();
            TokenAccount[] result = await SimpleWallet.instance.GetOwnedTokenAccounts(SimpleWallet.instance.wallet.GetAccount(0));

            if (result != null && result.Length > 0)
            {
                int itemIndex = 0;
                foreach (TokenAccount item in result)
                {
                    if (int.Parse(item.Account.Data.Parsed.Info.TokenAmount.Amount) > 0)
                    {
                        Nft.Nft nft = await Nft.Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint, SimpleWallet.instance.activeRpcClient, false);

                        //Task<AllArt.Solana.Nft.Nft> t = Task.Run<AllArt.Solana.Nft.Nft>( async () => {
                        //    return await AllArt.Solana.Nft.Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint, SimpleWallet.instance.activeRpcClient, false);
                        //}, stopTask.Token);

                        //Debug.Log("new");
                        //AllArt.Solana.Nft.Nft nft = t.Result;

                        token_items[itemIndex].gameObject.SetActive(true);
                        token_items[itemIndex].InitializeData(item, this, nft);
                        itemIndex++;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            stopTask.Cancel();
        }

        void DisableTokenItems()
        {
            foreach (TokenItem token in token_items)
            {
                token.gameObject.SetActive(false);
            }
        }
    }
}
