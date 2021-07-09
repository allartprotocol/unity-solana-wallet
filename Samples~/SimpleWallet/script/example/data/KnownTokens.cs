using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AllArt.Solana.Example
{ 
    [CreateAssetMenu(fileName = "Tokens", menuName = "AllArt/Example/KnownTokensData", order = 1)]
    public class KnownTokens : ScriptableObject
    {
        public KnownToken[] knownTokens;

        public KnownToken GetKnownToken(string pubKey) {
            KnownToken token = knownTokens.FirstOrDefault((e) => e.mint == pubKey);

            if (token != null)
                return token;

            return null;
        }
    }

    [System.Serializable]
    public class KnownToken {
        public string name;
        public Sprite logo;
        public string mint;
    }
}