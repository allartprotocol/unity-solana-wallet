
﻿namespace AllArt.Solana.Utility
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class ShortVecEncoding
    {
        public static int DecodeLength(byte[] bytes)
        {
            int len = 0;
            int size = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                int elem = bytes[0];
                len |= (elem & 0x7f) << (size * 7);
                size += 1;
                if ((elem & 0x80) == 0)
                {
                    break;
                }
            }
            return len;
        }

        public static void Encodelength(ref List<byte> bytes, int len)
        {
            byte rem_len = (byte)len;
            for (int i = 0; i < bytes.Count; i++)
            {
                int elem = rem_len & 0x7f;
                rem_len >>= 7;
                if (rem_len == 0)
                {
                    bytes.Add((byte)elem);
                    break;
                }
                else
                {
                    elem |= 0x80;
                    bytes.Add((byte)elem);
                }
            }
        }
    }
}
