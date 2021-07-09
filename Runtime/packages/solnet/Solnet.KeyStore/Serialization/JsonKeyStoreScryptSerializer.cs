using System;
using Solnet.KeyStore.Model;
using UnityEngine;

namespace Solnet.KeyStore.Serialization
{
    public static class JsonKeyStoreScryptSerializer
    {
        public static string SerializeScrypt(KeyStore<ScryptParams> scryptKeyStore)
        {
            return JsonUtility.ToJson(scryptKeyStore);
        }

        public static KeyStore<ScryptParams> DeserializeScrypt(string json)
        {
            return JsonUtility.FromJson<KeyStore<ScryptParams>>(json);
        }
    }
}