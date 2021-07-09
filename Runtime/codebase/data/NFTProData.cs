using System;

namespace AllArt.Solana
{
    public class NFTProData
    {
        public string title;
        public string description;
        public string[] licences;
        public string artist;
        public string camm;
        public string lortMint;
        public string fileHash;

        public static NFTProData DeserializeNFTProData(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);

            NFTProData swapData = new NFTProData();

            ObjectToByte.DecodeUTF8StringFromByte(data, 0, 20, out swapData.title);
            ObjectToByte.DecodeUTF8StringFromByte(data, 20, 64, out swapData.description);

            swapData.licences = new string[20];
            for (int i = 84, ind = 0; i < 20 * 32 + 84; i += 32, ind++)
            {
                ObjectToByte.DecodeBase58StringFromByte(data, i, 32, out string key);
                swapData.licences[ind] = key;
            }

            ObjectToByte.DecodeBase58StringFromByte(data, 724, 32, out swapData.artist);
            ObjectToByte.DecodeBase58StringFromByte(data, 756, 32, out swapData.camm);
            ObjectToByte.DecodeBase58StringFromByte(data, 788, 32, out swapData.lortMint);

            return swapData;
        }
    }
}
