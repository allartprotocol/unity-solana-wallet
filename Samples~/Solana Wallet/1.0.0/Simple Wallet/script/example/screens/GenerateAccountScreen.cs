using Newtonsoft.Json;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllArt.Solana.Example
{
    public class GenerateAccountScreen : SimpleScreen
    {
        public TextMeshProUGUI mnemonic_txt;
        public Button generate_btn;
        public Button restore_btn;
        public Button save_mnemonics_btn;
        public TMP_InputField password_input_field;
        public TextMeshProUGUI need_password_txt;

        private string password;
        private string path;
        private string[] paths;

        void Start()
        {
            //WalletSeed.GenerateNewMnemonic();//
            mnemonic_txt.text = WalletKeyPair.GenerateNewMnemonic();//"margin toast sheriff air tank liar tuna oyster cake tell trial more rebuild ostrich sick once palace uphold fall faculty clap slam job pitch";
            generate_btn.onClick.AddListener(() =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => { GenerateNewAccount(); });
                //GenerateNewAccount();
            });

            restore_btn.onClick.AddListener(() =>
            {
                manager.ShowScreen(this, "re-generate_screen");
            });

            save_mnemonics_btn.onClick.AddListener(SaveMnemonicsToTxtFile);

            //if (!SimpleWallet.instance.LoadSavedWallet())
            //{
            //    ShowScreen();
            //}
            //else
            //{
            //    manager.ShowScreen(this, "wallet_screen");
            //}
        }

        private void OnEnable()
        {
            need_password_txt.gameObject.SetActive(false);

            //if (!SimpleWallet.instance)
            //    return;

            //if (!SimpleWallet.instance.LoadSavedWallet())
            //{
            //    ShowScreen();
            //}
            //else
            //{
            //    manager.ShowScreen(this, "wallet_screen");

            //    //UnityMainThreadDispatcher.Instance().Enqueue(() => { WebSocketActions.RequestForAccountSubscriptionSentAction?.Invoke(SimpleWallet.instance.wallet.Account.GetPublicKey); });               
            //}
            mnemonic_txt.text = WalletKeyPair.GenerateNewMnemonic();
        }

        public void GenerateNewAccount()
        {
            if (string.IsNullOrEmpty(password_input_field.text))
            {
                need_password_txt.gameObject.SetActive(true);
                need_password_txt.text = "Need Password!";
                return;
            }

            SimpleWallet.instance.SavePlayerPrefs(SimpleWallet.instance.PasswordKey, password_input_field.text);
            try
            {
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { SimpleWallet.instance.GenerateWalletWithMenmonic(mnemonic_txt.text); });
                
                SimpleWallet.instance.GenerateWalletWithMenmonic(mnemonic_txt.text);
                string mnemonics = mnemonic_txt.text;
                manager.ShowScreen(this, "wallet_screen");
                need_password_txt.gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                password_input_field.gameObject.SetActive(true);
                password_input_field.text = ex.ToString();
            }
        }

        public override void ShowScreen(object data = null)
        {
            base.ShowScreen();
            gameObject.SetActive(true);

        }

        public override void HideScreen()
        {
            base.HideScreen();
            gameObject.SetActive(false);
        }

        private void SaveMnemonicsToTxtFile()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string mnemonic = mnemonic_txt.text;
            if (SimpleWallet.instance.StorageMethodReference == StorageMethod.JSON)
            {
                List<string> mnemonicsList = new List<string>();

                string[] splittedStringArray = mnemonic.Split(' ');
                foreach (string stringInArray in splittedStringArray)
                {
                    mnemonicsList.Add(stringInArray);
                }
                MnemonicsModel mnemonicsModel = new MnemonicsModel
                {
                    Mnemonics = mnemonicsList
                };

                //File.WriteAllText(path, JsonConvert.SerializeObject(mnemonicsModel));
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mnemonicsModel));
                DownloadFile(gameObject.name, "OnFileDownload", "sample.txt", bytes, bytes.Length);
            }
            else if (SimpleWallet.instance.StorageMethodReference == StorageMethod.SimpleTxt)
            {
                //File.WriteAllText(path, mnemonic);
                var bytes = Encoding.UTF8.GetBytes(mnemonic);
                DownloadFile(gameObject.name, "OnFileDownload", "mnemonics.txt", bytes, bytes.Length);
            }
#elif UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE

            paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", false);
            if(paths.Length > 0)
                path = paths[0];

#elif UNITY_ANDROID || UNITY_IPHONE
            string fileType = NativeFilePicker.ConvertExtensionToFileType("txt");
            NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
            {
                if (path == null)
                    Debug.Log("Operation cancelled");
                else
                {
                    this.path = path;
                }
            }, new string[] { fileType });
#endif
            string mnem = mnemonic_txt.text;
            if (SimpleWallet.instance.StorageMethodReference == StorageMethod.JSON)
            {
                List<string> mnemonicsList = new List<string>();

                string[] splittedStringArray = mnem.Split(' ');
                foreach (string stringInArray in splittedStringArray)
                {
                    mnemonicsList.Add(stringInArray);
                }
                MnemonicsModel mnemonicsModel = new MnemonicsModel
                {
                    Mnemonics = mnemonicsList
                };

                File.WriteAllText(path, JsonConvert.SerializeObject(mnemonicsModel));
            }
            else if (SimpleWallet.instance.StorageMethodReference == StorageMethod.SimpleTxt)
            {
                File.WriteAllText(path, mnem);
            }
        }

        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

        // Called from browser
        private void OnFileDownload()
        {

        }
    } 
}
