
namespace AllArt.Solana
{
    using Org.BouncyCastle.Math;

    public class PublicKey
    {
        public Org.BouncyCastle.Math.BigInteger _bn;
        static PublicKey defaultKey = new PublicKey("11111111111111111111111111111111");

        public PublicKey(string key)
        {
            _bn = new BigInteger(key);
        }

        public PublicKey(PublicKeyData publicKeyData)
        {
            _bn = publicKeyData._bn;
        }

        bool isPublicKeyData(object value)
        {
            return value.GetType() == typeof(PublicKeyData);
        }
    }

    public class PublicKeyInitData : object { }

    public class PublicKeyData
    {
        public BigInteger _bn;
    }
}