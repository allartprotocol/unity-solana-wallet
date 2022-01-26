using Solnet.Wallet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.Solana.Example
{
    public enum StorageMethod { JSON, SimpleTxt }
    public class SimpleWallet : WalletBaseComponent
    {
        public StorageMethod storageMethod;
        
        public static SimpleWallet instance;
        public readonly string storageMethodStateKey = "StorageMethodKey";

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

        private void Start()
        {
            ChangeState(storageMethod.ToString());
            if (PlayerPrefs.HasKey(storageMethodStateKey))
            {
                string storageMethodString = LoadPlayerPrefs(storageMethodStateKey);

                if(storageMethodString != storageMethod.ToString())
                {
                    storageMethodString = storageMethod.ToString();
                    ChangeState(storageMethodString);
                }

                if (storageMethodString == StorageMethod.JSON.ToString())
                    StorageMethodReference = StorageMethod.JSON;
                else if (storageMethodString == StorageMethod.SimpleTxt.ToString())
                    StorageMethodReference = StorageMethod.SimpleTxt;
            }
            else
                StorageMethodReference = StorageMethod.SimpleTxt;          
        }

        private void ChangeState(string state)
        {
            SavePlayerPrefs(storageMethodStateKey, storageMethod.ToString());
        }

        public StorageMethod StorageMethodReference
        {
            get { return storageMethod; }
            set { storageMethod = value; ChangeState(storageMethod.ToString()); }
        }

    }
}
