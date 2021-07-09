using Solnet.Rpc.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

namespace AllArt.Solana.Example
{
    public class WalletScreen : Screen
    {
        public TextMeshProUGUI lamports;
        public Button refresh_btn;
        public Button send_btn;
        public Button receive_btn;
        public Button logout_btn;

        public List<TokenItem> token_items;

        public KnownTokens knownTokens;

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
                    KnownToken known = knownTokens.GetKnownToken(item.Account.Data.Parsed.Info.Mint);

                    token_items[itemIndex].gameObject.SetActive(true);
                    token_items[itemIndex].InitializeData(item, this, known);
                    itemIndex++;
                }
            }            
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
