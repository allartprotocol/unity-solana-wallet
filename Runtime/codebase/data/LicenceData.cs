using System;
using AllArt.Solana.Utility;

namespace AllArt.Solana
{
    [System.Serializable]
    public class LicenceData
    {
        public string title;
        public string licenceMint;
        public string[] licenceRights;
        public string lortLicenceAccount;
        public uint quantity;
        public uint lorts;
        public string fileHash;

        public static LicenceData DeserializeLicenceData(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);
            LicenceData swapData = new LicenceData();

            ObjectToByte.DecodeUTF8StringFromByte(data, 0, 20, out swapData.title);
            ObjectToByte.DecodeBase58StringFromByte(data, 20, 32, out swapData.licenceMint);

            swapData.licenceRights = new string[20];
            for (int i = 52, ind = 0; i < 20 * 32 + 52; i += 32, ind++)
            {
                ObjectToByte.DecodeBase58StringFromByte(data, i, 32, out string key);
                swapData.licenceRights[ind] = key;
            }

            ObjectToByte.DecodeBase58StringFromByte(data, 692, 32, out swapData.lortLicenceAccount);
            ObjectToByte.DecodeUIntFromByte(data, 724, out swapData.quantity);
            ObjectToByte.DecodeUIntFromByte(data, 728, out swapData.lorts);
            ObjectToByte.DecodeUTF8StringFromByte(data, 732, 46, out swapData.fileHash);

            return swapData;
        }
    }
}
