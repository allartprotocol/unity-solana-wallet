using Solnet.KeyStore.Model;
using UnityEngine;

namespace Solnet.KeyStore.Serialization
{
    public static class JsonKeyStorePbkdf2Serializer
    {
        public static string SerialisePbkdf2(KeyStore<Pbkdf2Params> pbkdf2KeyStore)
        {
            return JsonUtility.ToJson(pbkdf2KeyStore);
        }

        public static KeyStore<Pbkdf2Params> DeserializePbkdf2(string json)
        {
            return JsonUtility.FromJson<KeyStore<Pbkdf2Params>>(json);
        }
    }
}