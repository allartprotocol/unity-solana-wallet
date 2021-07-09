namespace Solnet.KeyStore.Model
{
    public class KeyStore<TKdfParams> where TKdfParams : KdfParams
    {
        public CryptoInfo<TKdfParams> Crypto { get; set; }

        public string Id { get; set; }

        public string Address { get; set; }

        public int Version { get; set; }
    }
}