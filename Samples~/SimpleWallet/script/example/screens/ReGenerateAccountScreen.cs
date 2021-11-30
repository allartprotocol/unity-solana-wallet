using Solnet.Wallet;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace AllArt.Solana.Example
{
    public class ReGenerateAccountScreen : SimpleScreen
    {
        public TMP_InputField mnemonic_txt;
        public Button generate_btn;
        public Button create_btn;

        public TextMeshProUGUI error_txt;

        void Start()
        {
            generate_btn.onClick.AddListener(() =>
            {
                GenerateNewAccount();
            });

            create_btn.onClick.AddListener(() =>
            {
                manager.ShowScreen(this, "generate_screen");
            });
        }

        public void GenerateNewAccount()
        {
            Wallet keypair = SimpleWallet.instance.GenerateWalletWithMenmonic(mnemonic_txt.text);
            if (keypair != null)
            {
                manager.ShowScreen(this, "wallet_screen");
            }
            else
            {
                error_txt.text = "Keywords are not in a valid format.";
            }
        }

        public override void ShowScreen(object data = null)
        {
            base.ShowScreen();

            error_txt.text = "";
            mnemonic_txt.text = "margin toast sheriff air tank liar tuna oyster cake tell trial more rebuild ostrich sick once palace uphold fall faculty clap slam job pitch";//"";

            gameObject.SetActive(true);
        }

        public override void HideScreen()
        {
            base.HideScreen();
            gameObject.SetActive(false);
        }
    }
}
