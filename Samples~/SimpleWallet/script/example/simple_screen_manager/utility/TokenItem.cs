using AllArt.Solana.Example;
using Solnet.Rpc.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllArt.Solana
{

    public class TokenItem : MonoBehaviour
    {
        public TextMeshProUGUI pub_txt;
        public TextMeshProUGUI ammount_txt;

        public Image logo;

        public Button transferButton;

        TokenAccount tokenAccount;
        Screen parentScreen;

        private void Start()
        {
            transferButton.onClick.AddListener(() => {
                TransferAccount();
            });
        }

        public void InitializeData(TokenAccount tokenAccount, Screen screen, KnownToken knownToken = null)
        {
            parentScreen = screen;
            this.tokenAccount = tokenAccount;
            ammount_txt.text = tokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount.ToString();
            if (knownToken != null)
            {
                pub_txt.text = knownToken.name;
                logo.sprite = knownToken.logo;
            }
            else {
                logo.gameObject.SetActive(false);
                pub_txt.text = tokenAccount.Account.Data.Parsed.Info.Mint;// pubkey;
            }
        }

        public void TransferAccount() {
            parentScreen.manager.ShowScreen(parentScreen, "transfer_screen", tokenAccount);
        }
    }
}
