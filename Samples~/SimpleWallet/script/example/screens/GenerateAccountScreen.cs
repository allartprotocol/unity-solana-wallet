using System.Collections;
using System.Collections.Generic;
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

        void Start()
        {
            //WalletSeed.GenerateNewMnemonic();//
            mnemonic_txt.text = WalletKeyPair.GenerateNewMnemonic();//"margin toast sheriff air tank liar tuna oyster cake tell trial more rebuild ostrich sick once palace uphold fall faculty clap slam job pitch";
            generate_btn.onClick.AddListener(() =>
            {
                GenerateNewAccount();
            });

            restore_btn.onClick.AddListener(() =>
            {
                manager.ShowScreen(this, "re-generate_screen");
            });

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
            if (!SimpleWallet.instance)
                return;

            if (!SimpleWallet.instance.LoadSavedWallet())
            {
                ShowScreen();
            }
            else
            {
                manager.ShowScreen(this, "wallet_screen");
            }
        }

        public void GenerateNewAccount()
        {
            SimpleWallet.instance.GenerateWalletWithMenmonic(mnemonic_txt.text);
            manager.ShowScreen(this, "wallet_screen");
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
    }
}
