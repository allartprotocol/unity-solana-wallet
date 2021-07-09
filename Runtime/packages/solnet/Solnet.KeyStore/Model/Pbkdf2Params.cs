namespace Solnet.KeyStore.Model
{
    public class Pbkdf2Params : KdfParams
    {
        public int Count { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Prf { get; set; }
    }
}