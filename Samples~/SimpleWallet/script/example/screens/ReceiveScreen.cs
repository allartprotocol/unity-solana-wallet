using AllArt.Solana;
using AllArt.Solana.Example;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReceiveScreen : SimpleScreen
{
    public Button airdrop_btn;
    public Button close_btn;

    public TextMeshProUGUI publicKey_txt;
    public RawImage qrCode_img;

    private void Start()
    {
        airdrop_btn.onClick.AddListener(async () => {
            await SimpleWallet.instance.RequestAirdrop(SimpleWallet.instance.wallet.GetAccount(0));
        });

        close_btn?.onClick.AddListener(() =>
        {
            manager.ShowScreen(this, "wallet_screen");
        });
    }

    public override void ShowScreen(object data = null)
    {
        base.ShowScreen();
        gameObject.SetActive(true);

        CheckAndToggleAirdrop();

        GenerateQR();
        publicKey_txt.text = SimpleWallet.instance.wallet.GetAccount(0).GetPublicKey;
    }

    private void CheckAndToggleAirdrop()
    {
        if (SimpleWallet.instance.clientSource != WalletBaseComponent.EClientUrlSource.EMainnet)
            airdrop_btn.gameObject.SetActive(true);
        else
            airdrop_btn.gameObject.SetActive(false);
    }

    private void GenerateQR()
    {
        Texture2D tex = QRGenerator.GenerateQRTexture(SimpleWallet.instance.wallet.GetAccount(0).GetPublicKey, 256, 256);
        qrCode_img.texture = tex;
    }

    public override void HideScreen()
    {
        base.HideScreen();
        gameObject.SetActive(false);
    }
}
