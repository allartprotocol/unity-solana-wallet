using System;
using UnityEngine;
using AllArt.Solana.Utility;

namespace AllArt.Solana
{
    [System.Serializable]
    public class AccountLayout
    {
        public string mint;
        public string owner;
        public ulong amount;

        public static AccountLayout DeserializeAccountLayout(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);            
            AccountLayout accountLayoutData = new AccountLayout();

            ObjectToByte.DecodeBase58StringFromByte(data, 0, 32, out accountLayoutData.mint);
            ObjectToByte.DecodeBase58StringFromByte(data, 32, 32, out accountLayoutData.owner);
            ObjectToByte.DecodeUlongFromByte(data, 64, out accountLayoutData.amount);

            return accountLayoutData;
        }
    }
}
