using Solnet.Rpc.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AllArt.Solana.Example
{
    public class TokenItem : MonoBehaviour
    {
        public TextMeshProUGUI pub_txt;
        public TextMeshProUGUI ammount_txt;

        public RawImage logo;

        public Button transferButton;

        TokenAccount tokenAccount;
        Nft.Nft nft;
        SimpleScreen parentScreen;

        private void Start()
        {
            logo = GetComponentInChildren<RawImage>();

            transferButton.onClick.AddListener(() =>
            {
                TransferAccount();
            });
        }

        public void InitializeData(TokenAccount tokenAccount, SimpleScreen screen, AllArt.Solana.Nft.Nft nftData = null)
        {
            parentScreen = screen;
            this.tokenAccount = tokenAccount;
            if (nftData != null)
            {
                nft = nftData;
                ammount_txt.text = "";
                pub_txt.text = nftData.metaplexData.data.name;
                logo.texture = nftData.metaplexData.nftImage.file;
            }
            else
            {
                ammount_txt.text = tokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount.ToString();
                logo.gameObject.SetActive(false);
                pub_txt.text = tokenAccount.Account.Data.Parsed.Info.Mint;
            }
        }

        public void TransferAccount()
        {
            if (nft != null)
            {
                parentScreen.manager.ShowScreen(parentScreen, "transfer_screen", nft);
            }
            else
            {
                parentScreen.manager.ShowScreen(parentScreen, "transfer_screen", tokenAccount);
            }
        }
    }
}
