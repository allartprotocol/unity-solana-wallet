using AllArt.Solana.Example;
using AllArt.Solana.Nft;
using AllArt.Solana.Utility;
using Solnet.Rpc.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletHolder : MonoBehaviour
{
    public Button toggleWallet_btn;
    public Button subscriptionImage;
    public TextMeshProUGUI subscription_txt;
    public GameObject wallet;

    private Image img;

    void Start()
    {
        img = subscriptionImage.image;
        wallet.SetActive(false);
        toggleWallet_btn.onClick.AddListener(() => {
            wallet.SetActive(!wallet.activeSelf);
        });

        WebSocketActions.WebSocketAccountSubscriptionAction += CheckSubscription;
        WebSocketActions.CloseWebSocketConnectionAction += () => CheckSubscription(false);
    }

    private void CheckSubscription(bool isSubscribed)
    {
        if (isSubscribed)
        {
            MainThreadDispatcher.Instance().Enqueue(() => { img.color = Color.green; });
            MainThreadDispatcher.Instance().Enqueue(() => { subscription_txt.text = "Subscribed"; });
        }
        else
        {
            MainThreadDispatcher.Instance().Enqueue(() => { img.color = Color.red; });
            MainThreadDispatcher.Instance().Enqueue(() => { subscription_txt.text = "Not Subscribed"; });
        }
    }
}
