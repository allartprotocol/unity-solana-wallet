using System;
using AllArt.Solana.Utility;

namespace AllArt.Solana
{
    [System.Serializable]
    public class TokenSwapData
    {
        public byte version;
        public byte isInitialized;
        public byte nonce;
        public ulong ammount;
        public ulong left;
        public string tokenProgramId;
        public string tokenAccountA;
        public string tokenAccountB;
        public string tokenPool;
        public string mintA;
        public string mintB;
        public string feeAccount;
        public string artworkAccount;
        public ulong tradeFeeNumerator;
        public ulong tradeFeeDenominator;
        public ulong ownerTradeFeeNumerator;
        public ulong ownerTradeFeeDenominator;
        public ulong ownerWithdrawFeeNumerator;
        public ulong ownerWithdrawFeeDenominator;
        public ulong hostFeeNumerator;
        public ulong hostFeeDenominator;
        public byte curveType;
        public byte[] curveParameter;

        public static TokenSwapData DeserializeSwapData(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);

            TokenSwapData swapData = new TokenSwapData();

            swapData.version = data[0];
            swapData.isInitialized = data[1];
            swapData.nonce = data[2];
            ObjectToByte.DecodeUlongFromByte(data, 3, out swapData.ammount);
            ObjectToByte.DecodeUlongFromByte(data, 11, out swapData.left);
            ObjectToByte.DecodeBase58StringFromByte(data, 19, 32, out swapData.tokenProgramId);
            ObjectToByte.DecodeBase58StringFromByte(data, 51, 32, out swapData.tokenAccountA);
            ObjectToByte.DecodeBase58StringFromByte(data, 83, 32, out swapData.tokenAccountB);
            ObjectToByte.DecodeBase58StringFromByte(data, 115, 32, out swapData.tokenPool);
            ObjectToByte.DecodeBase58StringFromByte(data, 147, 32, out swapData.mintA);
            ObjectToByte.DecodeBase58StringFromByte(data, 179, 32, out swapData.mintB);
            ObjectToByte.DecodeBase58StringFromByte(data, 211, 32, out swapData.feeAccount);
            ObjectToByte.DecodeBase58StringFromByte(data, 243, 32, out swapData.artworkAccount);
            ObjectToByte.DecodeUlongFromByte(data, 275, out swapData.tradeFeeNumerator);
            ObjectToByte.DecodeUlongFromByte(data, 283, out swapData.tradeFeeDenominator);
            ObjectToByte.DecodeUlongFromByte(data, 291, out swapData.ownerTradeFeeNumerator);
            ObjectToByte.DecodeUlongFromByte(data, 299, out swapData.ownerTradeFeeDenominator);
            ObjectToByte.DecodeUlongFromByte(data, 307, out swapData.ownerWithdrawFeeNumerator);
            ObjectToByte.DecodeUlongFromByte(data, 315, out swapData.ownerWithdrawFeeDenominator);
            swapData.curveType = data[323];
            return swapData;
        }
    }
}
