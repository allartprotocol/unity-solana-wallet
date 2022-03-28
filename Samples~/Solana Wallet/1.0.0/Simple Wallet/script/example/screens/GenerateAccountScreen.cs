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
    [RequireComponent(typeof(TxtLoader))]
    public class GenerateAccountScreen : SimpleScreen
    {
        public TextMeshProUGUI mnemonic_txt;
        public Button generate_btn;
        public Button restore_btn;
        public Button save_mnemonics_btn;
        public Button back_btn;
        public TMP_InputField password_input_field;
        public TextMeshProUGUI need_password_txt;

        private TxtLoader _txtLoader;
        private string _mnemonicsFileTitle = "Mnemonics";
        private string _privateKeyFileTitle = "PrivateKey";

        void Start()
        {
            _txtLoader = GetComponent<TxtLoader>();
            mnemonic_txt.text = WalletKeyPair.GenerateNewMnemonic();//"margin toast sheriff air tank liar tuna oyster cake tell trial more rebuild ostrich sick once palace uphold fall faculty clap slam job pitch";

            if(generate_btn != null)
            {
                generate_btn.onClick.AddListener(() =>
                {
                    MainThreadDispatcher.Instance().Enqueue(() => { GenerateNewAccount(); });
                });
            }

            if(restore_btn != null)
            {
                restore_btn.onClick.AddListener(() =>
                {
                    manager.ShowScreen(this, "re-generate_screen");
                });
            }

            if(save_mnemonics_btn != null)
            {
                save_mnemonics_btn.onClick.AddListener(() =>
                {
                    _txtLoader.SaveTxt(_mnemonicsFileTitle, mnemonic_txt.text, false);
                });
            }

            if(back_btn != null)
            {
                back_btn.onClick.AddListener(() =>
                {
                    manager.ShowScreen(this, "login_screen");
                });
            }

            _txtLoader.TxtSavedAction += SaveMnemonicsToTxtFile;
        }

        private void OnEnable()
        {
            need_password_txt.gameObject.SetActive(false);
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

        private void SaveMnemonicsToTxtFile(string path, string mnemonics, string fileTitle)
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

                if (path != string.Empty)
                    File.WriteAllText(path, JsonConvert.SerializeObject(mnemonicsModel));
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mnemonicsModel));
                    DownloadFile(gameObject.name, "OnFileDownload", _mnemonicsFileTitle + ".txt", bytes, bytes.Length);
                }
            }
            else if (SimpleWallet.instance.StorageMethodReference == StorageMethod.SimpleTxt)
            {
                if (path != string.Empty)
                    File.WriteAllText(path, mnemonics);
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(mnemonics);
                    DownloadFile(gameObject.name, "OnFileDownload", _mnemonicsFileTitle + ".txt", bytes, bytes.Length);
                }
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
