
﻿namespace AllArt.Solana
{
    using Chaos.NaCl;

    public class Keypair
    {
        public Keypair() { }

        public Keypair(byte[] publicKey, byte[] privateKey)
        {
            this.publicKeyByte = publicKey;
            this.privateKeyByte = privateKey;

            this.publicKey = CryptoBytes.Base58Encode(publicKey);
            this.privateKey = CryptoBytes.Base58Encode(privateKey);
        }

        public Keypair(Ed25519Keypair keypair)
        {
            publicKeyByte = keypair.publicKey;
            privateKeyByte = keypair.privateKey;
            this.publicKey = CryptoBytes.Base58Encode(keypair.publicKey);
            this.privateKey = CryptoBytes.Base58Encode(keypair.privateKey);
        }

        public string publicKey;
        public string privateKey;
        public byte[] publicKeyByte;
        public byte[] privateKeyByte;
    }
}
