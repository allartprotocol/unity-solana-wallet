using AllArt.Solana.Example;
using AllArt.Solana.Nft;
using AllArt.Solana.Utility;
using Solnet.Rpc.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletHolder : MonoBehaviour
{
    public Button toggleWallet_btn;
    public GameObject wallet;

    void Start()
    {
        wallet.SetActive(false);
        toggleWallet_btn.onClick.AddListener(() => {
            wallet.SetActive(!wallet.activeSelf);
        });        
    }
}
