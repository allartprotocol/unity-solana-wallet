using System;
using AllArt.Solana.Utility;

namespace AllArt.Solana.Nft
{
    [System.Serializable]
    public class NFTProData
    {
        public string type;
        public uint version;
        public string mint;
        public string metadata;
        public string title;
        public string description;
        public string creator;
        public string collection;
        public string licence;
        public string licence_title;
        public bool nsfw;
        public string[] tags;

        public static NFTProData DeserializeNFTProData(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);

            NFTProData swapData = new NFTProData();

            int index = 0;

            ObjectToByte.DecodeUTF8StringFromByte(data, 0, 4, out swapData.type);

            index+= 4;
            ObjectToByte.DecodeUIntFromByte(data, index, out swapData.version);
            index += 4;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, 32, out swapData.mint);
            index += 32;

            ObjectToByte.DecodeUTF8StringFromByte(data, index, 32, out swapData.metadata);
            index += 32;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, 20, out swapData.title);
            index += 20;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, 64, out swapData.description);
            index += 64;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, 64, out swapData.creator);
            index += 64;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, 100, out swapData.licence);
            index += 100;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, 20, out swapData.licence_title);
            index += 20;
            swapData.nsfw = BitConverter.ToBoolean(data, index);
            index++;


            swapData.tags = new string[10];
            for (int i = index, ind = 0; i < 10 * 15 + index; i += 15, ind++)
            {
                ObjectToByte.DecodeBase58StringFromByte(data, i, 32, out string key);
                swapData.tags[ind] = key;
            }

            return swapData;
        }
    }
}
