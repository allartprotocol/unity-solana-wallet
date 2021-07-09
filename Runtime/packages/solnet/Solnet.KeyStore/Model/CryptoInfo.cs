namespace Solnet.KeyStore.Model
{
    [System.Serializable]
    public class CryptoInfo<TKdfParams> where TKdfParams : KdfParams
    {
        public CryptoInfo()
        {
        }

        public CryptoInfo(string cipher, byte[] cipherText, byte[] iv, byte[] mac, byte[] salt, TKdfParams kdfParams,
            string kdfType)
        {
            Cipher = cipher;
            CipherText = cipherText.ToHex();
            Mac = mac.ToHex();
            CipherParams = new CipherParams(iv);
            Kdfparams = kdfParams;
            Kdfparams.Salt = salt.ToHex();
            Kdf = kdfType;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Cipher { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public string CipherText { get; set; }

        // ReSharper disable once StringLiteralTypo
        // ReSharper disable once MemberCanBePrivate.Global
        public CipherParams CipherParams { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Kdf { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        public string Mac { get; set; }

        // ReSharper disable once StringLiteralTypo
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once MemberCanBePrivate.Global
        public TKdfParams Kdfparams { get; set; }
    }
}