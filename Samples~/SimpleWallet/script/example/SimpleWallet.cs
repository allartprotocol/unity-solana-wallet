using Solnet.Wallet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.Solana.Example
{
    public class SimpleWallet : WalletBaseComponent
    {
        public static SimpleWallet instance;

        public override void Awake()
        {
            base.Awake();
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
