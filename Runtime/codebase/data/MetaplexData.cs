using System;
using System.Collections.Generic;
using UnityEngine;
using AllArt.Solana.Utility;

namespace AllArt.Solana.Nft
{
    [System.Serializable]
    public class Attributes {
        public string trait_type;
        public string value;
    }

    [System.Serializable]
    public class MetaplexJsonData {
        public string previewUrl;
        public string image;
        public Attributes[] attributes;
    }

    [System.Serializable]
    public class MetaplexData
    {
        public string name;
        public string symbol;
        public string url;
        public int seller_fee_basis_points;
        public CreatorData[] creators;
        public MetaplexJsonData json;
    }

    [System.Serializable]
    public class CreatorData
    {
        public string address;
        public bool verified;
        public int share;
    }

    [System.Serializable]
    public class Metaplex : iNftStandard<Metaplex>
    {
        public string authority;
        public string mint;
        public MetaplexData data;
        public iNftFile<Texture2D> nftImage;

        public Metaplex()
        {
            data = new MetaplexData();
        }

        public Metaplex ParseData(string base64Data)
        {
            byte[] data = Convert.FromBase64String(base64Data);

            int index = 1;
            Metaplex metaplexData = this;

            ObjectToByte.DecodeBase58StringFromByte(data, index, 32, out metaplexData.authority);
            index += 32;
            ObjectToByte.DecodeBase58StringFromByte(data, index, 32, out metaplexData.mint);
            index += 32;

            ObjectToByte.DecodeUIntFromByte(data, index, out uint nameLength);
            index += 4;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, (int)nameLength, out metaplexData.data.name);
            index += (int)nameLength;

            ObjectToByte.DecodeUIntFromByte(data, index, out uint symbolLength);
            index += 4;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, (int)symbolLength, out metaplexData.data.symbol);
            index += (int)symbolLength;

            ObjectToByte.DecodeUIntFromByte(data, index, out uint urlLenght);
            index += 4;
            ObjectToByte.DecodeUTF8StringFromByte(data, index, (int)urlLenght, out metaplexData.data.url);
            index += (int)urlLenght;

            metaplexData.data.seller_fee_basis_points = BitConverter.ToUInt16(data, index);

            index += 3;

            ObjectToByte.DecodeUIntFromByte(data, index, out uint creatorsLenght);
            index += 4;
            List<CreatorData> creators = new List<CreatorData>();

            for (int i = 0; i < creatorsLenght; i++)
            {
                CreatorData creatorData = new CreatorData();
                ObjectToByte.DecodeUTF8StringFromByte(data, index, (int)nameLength, out string creator);
                creatorData.address = creator;
                index += 32;
                creatorData.verified = BitConverter.ToBoolean(data, index++);
                creatorData.share = data[index++];
            }

            metaplexData.data.creators = creators.ToArray();

            return metaplexData;
        }
    }
}
