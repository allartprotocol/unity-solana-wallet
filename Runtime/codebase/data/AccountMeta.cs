
﻿namespace AllArt.Solana
{   
    [System.Serializable]
    public class SignaturePubkeyPair
    {
        public byte[] signature;
        public PublicKey publicKey;
    }

    [System.Serializable]
    public class TransactionCtorFields
    {
        public string recentBlockhash;
        public string nonceInformation;
        public string feePayer;
        public SignaturePubkeyPair signatures;
    }

    [System.Serializable]
    public class CompiledInstruction
    {
        /** Index into the transaction keys array indicating the program account that executes this instruction */
        public int programIdIndex;
        /** Ordered indices into the transaction keys array indicating which accounts to pass to the program */
        public byte[] accounts;
        public byte[] keyIndicesCount;
        public byte[] keyIndices;
        public byte[] dataLength;
        /** The program input data encoded as base 58 */
        public string data;
    };
}