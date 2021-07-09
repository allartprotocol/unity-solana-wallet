using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.Solana
{
    public class SignatureStatus {
        public string confirmationStatus;
        public long confirmations;
        public object error;
        public long slot;
    }
}