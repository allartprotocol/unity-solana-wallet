namespace Solnet.KeyStore.Model
{
    public class ScryptParams : KdfParams
    {
        public int N { get; set; }

        public int R { get; set; }

        public int P { get; set; }
    }
}