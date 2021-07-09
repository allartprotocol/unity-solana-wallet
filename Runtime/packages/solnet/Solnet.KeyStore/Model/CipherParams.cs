namespace Solnet.KeyStore.Model
{
    [System.Serializable]
    public class CipherParams
    {
        public CipherParams()
        {
        }

        public CipherParams(byte[] iv)
        {
            Iv = iv.ToHex();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string Iv { get; set; }
    }
}