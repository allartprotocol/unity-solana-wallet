using dotnetstandard_bip39;
using dotnetstandard_bip39.System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Solnet.Wallet;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace AllArt.Solana
{
    public class BIP : MonoBehaviour
    {
        private const string InvalidMnemonic = "Invalid mnemonic";
        private const string InvalidEntropy = "Invalid entropy";
        private const string InvalidChecksum = "Invalid mnemonic checksum";

        private string txt;

        public static object Properties { get; private set; }

        private string lPad(string str, string padString, int length)
        {
            while (str.Length < length)
            {
                str = padString + str;
            }

            return str;
        }

        public string MnemonicToEntropy(string mnemonic, BIP39Wordlist wordlistType)
        {
            var wordlist = GetWordlist(wordlistType);
            var words = mnemonic.Normalize(NormalizationForm.FormKD).Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (words.Length % 3 != 0)
            {
                throw new FormatException(InvalidMnemonic);
            }

            var bits = string.Join("", words.Select(word =>
            {
                var index = Array.IndexOf(wordlist, word);
                if (index == -1)
                {
                    throw new FormatException(InvalidMnemonic);
                }

                return lPad(Convert.ToString(index, 2), "0", 11);
            }));

            // split the binary string into ENT/CS
            var dividerIndex = (int)Math.Floor((double)bits.Length / 33) * 32;
            var entropyBits = bits.Substring(0, dividerIndex);
            var checksumBits = bits.Substring(dividerIndex);

            // calculate the checksum and compare
            var entropyBytesMatch = Regex.Matches(entropyBits, "(.{1,8})")
                .OfType<Match>()
                .Select(m => m.Groups[0].Value)
                .ToArray();

            var entropyBytes = entropyBytesMatch
                .Select(bytes => Convert.ToByte(bytes, 2)).ToArray();

            CheckValidEntropy(entropyBytes);


            var newChecksum = DeriveChecksumBits(entropyBytes);

            if (newChecksum != checksumBits)
                throw new Exception(InvalidChecksum);

            var result = BitConverter
                .ToString(entropyBytes)
                .Replace("-", "")
                .ToLower();

            return result;
        }

        public string EntropyToMnemonic(string entropy, BIP39Wordlist wordlistType)
        {
            var wordlist = GetWordlist(wordlistType);

            //How can I do this more efficiently, the multiple substrings I don't like...
            var entropyBytes = Enumerable.Range(0, entropy.Length / 2)
                .Select(x => Convert.ToByte(entropy.Substring(x * 2, 2), 16))
                .ToArray();

            CheckValidEntropy(entropyBytes);

            var entropyBits = BytesToBinary(entropyBytes);
            var checksumBits = DeriveChecksumBits(entropyBytes);

            var bits = entropyBits + checksumBits;

            var chunks = Regex.Matches(bits, "(.{1,11})")
                .OfType<Match>()
                .Select(m => m.Groups[0].Value)
                .ToArray();

            var words = chunks.Select(binary =>
            {
                var index = Convert.ToInt32(binary, 2);
                return wordlist[index];
            });

            var joinedText = String.Join((wordlistType == BIP39Wordlist.Japanese ? "\u3000" : " "), words);
            return joinedText;
        }

        public string GenerateMnemonic(int strength, BIP39Wordlist wordlistType)
        {
            if (strength % 32 != 0)
                throw new NotSupportedException(InvalidEntropy);

            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            byte[] buffer = new byte[strength / 8];
            rngCryptoServiceProvider.GetBytes(buffer);

            var entropyHex = BitConverter.ToString(buffer).Replace("-", "");

            return EntropyToMnemonic(entropyHex, wordlistType);
        }

        private static void CheckValidEntropy(byte[] entropyBytes)
        {
            if (entropyBytes.Length < 16)
                throw new FormatException(InvalidEntropy);

            if (entropyBytes.Length > 32)
                throw new FormatException(InvalidEntropy);

            if (entropyBytes.Length % 4 != 0)
                throw new FormatException(InvalidEntropy);
        }

        private string Salt(string password)
        {
            return "mnemonic" + (!string.IsNullOrEmpty(password) ? password : "");
        }

        private byte[] MnemonicToSeed(string mnemonic, string password)
        {
            var mnemonicBytes = Encoding.UTF8.GetBytes(mnemonic.Normalize(NormalizationForm.FormKD));
            var saltBytes = Encoding.UTF8.GetBytes(Salt(password.Normalize(NormalizationForm.FormKD)));

            var rfc2898DerivedBytes = new Rfc2898DeriveBytesExtended(mnemonicBytes, saltBytes, 2048, HashAlgorithmName.SHA512);
            var key = rfc2898DerivedBytes.GetBytes(64);

            return key;
        }

        public string MnemonicToSeedHex(string mnemonic, string password)
        {
            var key = MnemonicToSeed(mnemonic, password);
            var hex = BitConverter
                .ToString(key)
                .Replace("-", "")
                .ToLower();

            return hex;
        }

        private string DeriveChecksumBits(byte[] checksum)
        {
            var ent = checksum.Length * 8;
            var cs = (int)ent / 32;

            var sha256Provider = new SHA256CryptoServiceProvider();
            var hash = sha256Provider.ComputeHash(checksum);
            string result = BytesToBinary(hash);
            return result.Substring(0, cs);
        }

        private string BytesToBinary(byte[] hash)
        {
            return string.Join("", hash.Select(h => lPad(Convert.ToString(h, 2), "0", 8)));
        }

        public bool ValidateMnemonic(string mnemonic, BIP39Wordlist wordlist)
        {
            try
            {
                MnemonicToEntropy(mnemonic, wordlist);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private string[] GetWordlist(BIP39Wordlist wordlist)
        {
            var wordlists = new Dictionary<string, string>
            {
                {BIP39Wordlist.ChineseSimplified.ToString(), "chinese_simplified"},
                {BIP39Wordlist.ChineseTraditional.ToString(), "chinese_traditional"},
                {BIP39Wordlist.English.ToString(), "english"},
                {BIP39Wordlist.French.ToString(), "french"},
                {BIP39Wordlist.Italian.ToString(), "italian"},
                {BIP39Wordlist.Japanese.ToString(), "japanese"},
                {BIP39Wordlist.Korean.ToString(), "korean"},
                {BIP39Wordlist.Spanish.ToString(), "spanish"}
            };

            var wordListFile = wordlists[wordlist.ToString()];

            //string path = @"C:\Users\DEJAN\OneDrive\Desktop\Wordlists\english.txt";
            //WWW fileWWW = new WWW(path);

            //txt = fileWWW.text;

            //return txt.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            string stringToReturn = PlayerPrefs.GetString(wordListFile);
            return stringToReturn.Split(' ');
            ////------------------
            //TextAsset worldListResultsAsset = Resources.Load<TextAsset>("Wordlists/" + wordListFile);
            //var fileContents = worldListResultsAsset.text;

            //return fileContents.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //// ----------------------
        }
        //IEnumerator ReadFileAsync(string fileURL, Action action = null)
        //{
        //    WWW fileWWW = new WWW(fileURL);
        //    yield return fileWWW;

        //    // now you have your file's text, you can do whatever you want with it.
        //    txt = fileWWW.text;
        //    action?.Invoke();
        //}

        //[DllImport("__Internal")]
        //private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);
        //public void OnFileUpload(string url)
        //{
        //    StartCoroutine(OutputRoutine(url));
        //}
        //private IEnumerator OutputRoutine(string url)
        //{
        //    var loader = new WWW(url);
        //    yield return loader;
        //    //_loadedMnemonics = loader.text;
        //}
    }
}