using Solnet.Rpc.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using SFB;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text;
using System;

namespace AllArt.Solana.Example
{
    [RequireComponent(typeof(TxtLoader))]
    public class WalletScreen : SimpleScreen
    {
        public TextMeshProUGUI lamports;
        public Button refresh_btn;
        public Button send_btn;
        public Button receive_btn;
        public Button logout_btn;
        public Button save_mnemonics_btn;
        public Button save_private_key_btn;

        public List<TokenItem> token_items;

        public KnownTokens knownTokens;
        public SimpleScreenManager parentManager;

        private TxtLoader _txtLoader;
        private CancellationTokenSource stopTask;

        private string _mnemonicsFileTitle = "Mnemonics";
        private string _privateKeyFileTitle = "PrivateKey";

        void Start()
        {
            _txtLoader = GetComponent<TxtLoader>();
            WebSocketActions.WebSocketAccountSubscriptionAction += (bool istrue) => 
            {
                MainThreadDispatcher.Instance().Enqueue(() => { UpdateWalletBalanceDisplay(); });
            };
            WebSocketActions.CloseWebSocketConnectionAction += DisconnectToWebSocket;
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
                manager.ShowScreen(this, "login_screen");
                if(parentManager != null)
                    parentManager.ShowScreen(this, "[Connect_Wallet_Screen]");
            });

            save_private_key_btn.onClick.AddListener(() => 
            {
                //_txtLoader.SaveByteArray(_privateKeyFileTitle, Encoding.ASCII.GetBytes(SimpleWallet.instance.privateKey), false);
                _txtLoader.SaveTxt(_privateKeyFileTitle, SimpleWallet.instance.privateKey, false);
            });

            save_mnemonics_btn.onClick.AddListener(() =>
            {
                _txtLoader.SaveTxt(_mnemonicsFileTitle, SimpleWallet.instance.LoadPlayerPrefs(SimpleWallet.instance.MnemonicsKey), false);
            });

            _txtLoader.TxtSavedAction += SaveMnemonicsOnClick;
            _txtLoader.TxtSavedAction += SavePrivateKeyOnClick;

            stopTask = new CancellationTokenSource();
        }

        private void SavePrivateKeyOnClick(string path, string key, string fileTitle)
        {
            if (!this.gameObject.activeSelf) return;
            if (fileTitle != _privateKeyFileTitle) return;

            //List<string> list = new List<string>();
            //foreach (byte item in key)
            //{
            //    list.Add(item.ToString());
            //}

            if (path != string.Empty)
            {
                File.WriteAllText(path, key);
            }
            else
            {
                //string result = string.Join(Environment.NewLine, list);
                var bytes = Encoding.UTF8.GetBytes(key);
                DownloadFile(gameObject.name, "OnFileDownload", _privateKeyFileTitle + ".txt", bytes, bytes.Length);
            }
        }

        private void SaveMnemonicsOnClick(string path, string mnemonics, string fileTitle)
        {
            if (!this.gameObject.activeSelf) return;
            if (fileTitle != _mnemonicsFileTitle) return;

            if (SimpleWallet.instance.StorageMethodReference == StorageMethod.JSON)
            {
                List<string> mnemonicsList = new List<string>();

                string[] splittedStringArray = mnemonics.Split(' ');
                foreach (string stringInArray in splittedStringArray)
                {
                    mnemonicsList.Add(stringInArray);
                }
                MnemonicsModel mnemonicsModel = new MnemonicsModel
                {
                    Mnemonics = mnemonicsList
                };

                if(path != string.Empty)
                    File.WriteAllText(path, JsonConvert.SerializeObject(mnemonicsModel));
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mnemonicsModel));
                    DownloadFile(gameObject.name, "OnFileDownload", _mnemonicsFileTitle + ".txt", bytes, bytes.Length);
                }
            }
            else if (SimpleWallet.instance.StorageMethodReference == StorageMethod.SimpleTxt)
            {
                if(path != string.Empty)
                    File.WriteAllText(path, mnemonics);
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(mnemonics);
                    DownloadFile(gameObject.name, "OnFileDownload", _mnemonicsFileTitle + ".txt", bytes, bytes.Length);
                }
            }
        }

        private void TransitionToTransfer(object data = null)
        {
            manager.ShowScreen(this, "transfer_screen", data);
        }

        private async void UpdateWalletBalanceDisplay()
        {
            if (SimpleWallet.instance.wallet is null) return;

            double sol = await SimpleWallet.instance.GetSolAmmount(SimpleWallet.instance.wallet.GetAccount(0));
            MainThreadDispatcher.Instance().Enqueue(() => { lamports.text = $"{sol}"; });
        }

        private void DisconnectToWebSocket()
        {
            MainThreadDispatcher.Instance().Enqueue(() => { manager.ShowScreen(this, "login_screen"); });
            MainThreadDispatcher.Instance().Enqueue(() => { SimpleWallet.instance.DeleteWalletAndClearKey(); });
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
                    if (float.Parse(item.Account.Data.Parsed.Info.TokenAmount.Amount) > 0)
                    {
                        Nft.Nft nft = await Nft.Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint, SimpleWallet.instance.activeRpcClient, true);

                        //Task<AllArt.Solana.Nft.Nft> t = Task.Run<AllArt.Solana.Nft.Nft>( async () => {
                        //    return await AllArt.Solana.Nft.Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint, SimpleWallet.instance.activeRpcClient, false);
                        //}, stopTask.Token);

                        //Debug.Log("new");
                        //AllArt.Solana.Nft.Nft nft = t.Result;
                        if (itemIndex >= token_items.Count) return;
                        if (token_items[itemIndex] == null) return;

                        token_items[itemIndex].gameObject.SetActive(true);
                        token_items[itemIndex].InitializeData(item, this, nft);
                        itemIndex++;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (stopTask is null) return;
            stopTask.Cancel();
        }

        void DisableTokenItems()
        {
            foreach (TokenItem token in token_items)
            {
                token.gameObject.SetActive(false);
            }
        }

        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

        // Called from browser
        public void OnFileDownload()
        {
            
        }
    }
}
