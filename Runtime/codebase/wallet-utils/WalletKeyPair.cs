
using Solnet.Wallet;
using System;

namespace AllArt.Solana
{
    public static class WalletKeyPair
    {
        public static string derivePath = "m/44'/501'/0'/0'";

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static string GenerateNewMnemonic()
        {
            dotnetstandard_bip39.BIP39 p = new dotnetstandard_bip39.BIP39();
            string mnemonic = p.GenerateMnemonic(256, dotnetstandard_bip39.BIP39Wordlist.English);
            return mnemonic;
        }

        public static byte[] GetBIP39SeedBytes(string seed)
        {
            return StringToByteArrayFastest(MnemonicToSeedHex(seed));
        }

        public static string MnemonicToSeedHex(string seed)
        {

            dotnetstandard_bip39.BIP39 p = new dotnetstandard_bip39.BIP39();
            return p.MnemonicToSeedHex(seed, string.Empty);
        }

        public static byte[] GetBIP32SeedByte(byte[] seed)
        {
            Ed25519Bip32 bip = new Ed25519Bip32(seed);

            (byte[] key, byte[] chain) = bip.DerivePath(derivePath);
            return key;
        }

        public static byte[] GenerateSeedFromMnemonic(string mnemonic)
        {
            return GetBIP39SeedBytes(mnemonic);
        }

        public static Keypair GenerateKeyPairFromMnemonic(string mnemonics)
        {
            byte[] bip39seed = GetBIP39SeedBytes(mnemonics);

            byte[] finalSeed = GetBIP32SeedByte(bip39seed);
            (byte[] privateKey, byte[] publicKey) = Ed25519Extensions.EdKeyPairFromSeed(finalSeed);

            return new Keypair(publicKey, privateKey);
        }

        public static bool CheckMnemonicValidity(string mnemonic)
        {
            string[] mnemonicWords = mnemonic.Split(' ');
            if (mnemonicWords.Length == 12 || mnemonicWords.Length == 24)
                return true;
            return false;
        }

        public static void SaveKeyPair(Keypair keypair)
        {
            //save to playerPrefs for testing purposes
            //make sure to change later for production
        }
    }
}
