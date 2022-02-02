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

        private void Awake()
        {
            logo = GetComponentInChildren<RawImage>();
        }

        private void Start()
        {
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
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { nft = nftData; });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { ammount_txt.text = ""; });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { pub_txt.text = nftData.metaplexData.data.name; });
                nft = nftData;
                ammount_txt.text = "";
                pub_txt.text = nftData.metaplexData.data.name;

                if (logo != null)
                {
                    MainThreadDispatcher.Instance().Enqueue(() => { logo.texture = nftData.metaplexData.nftImage.file; });
                    //logo.texture = nftData.metaplexData.nftImage.file;
                }
            }
            else
            {
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { ammount_txt.text = tokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount.ToString(); });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { logo.gameObject.SetActive(false); });
                //UnityMainThreadDispatcher.Instance().Enqueue(() => { pub_txt.text = tokenAccount.Account.Data.Parsed.Info.Mint; });
                ammount_txt.text = tokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount.ToString();

                if (logo is null) return;

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
