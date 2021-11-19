using AllArt.Solana.Utility;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AllArt.Solana.Nft
{
    [System.Serializable]
    public class NftImage : iNftFile<Texture2D>
    {
        public string name { get; set; }
        public string extension { get; set; }
        public string externalUrl { get; set; }
        public Texture2D file { get; set; }

        ~NftImage() {
            if (file != null)
            {
                GameObject.Destroy(file);
            }
        }

    }

    [System.Serializable]
    public class Nft
    {
        public Metaplex metaplexData;

        public Nft() { }

        public Nft(Metaplex metaplexData)
        {
            this.metaplexData = metaplexData;
        }

        public static async Task<NFTProData> TryGetNftPro(string mint, SolanaRpcClient connection) {
            
            AccountInfo data = await AccountUtility.GetAccountData(mint, connection);

            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(data));

            if (data != null && data.Data != null && data.Data.Count > 0)
            {
                AccountLayout accountlayout = AccountLayout.DeserializeAccountLayout(data.Data[0]);
                Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(accountlayout));
            }

            return null;
        }

        public static async Task<Nft> TryGetNftData(string mint, SolanaRpcClient connection, bool tryUseLocalContent = true)
        {
            Solnet.Wallet.PublicKey metaplexDataPubKey = FindProgramAddress(mint);

            if (metaplexDataPubKey != null)
            {
                AccountInfo data = await AccountUtility.GetAccountData(metaplexDataPubKey.Key, connection);

                if (tryUseLocalContent)
                { 
                    Nft nft = TryLoadNftFromLocal(mint);
                    if (nft != null)
                    {
                        return nft;
                    }
                }

                if (data != null && data.Data != null && data.Data.Count > 0)
                {
                    Metaplex met = new Metaplex().ParseData(data.Data[0]);
                    MetaplexJsonData jsonData = await AllArt.Solana.Utility.FileLoader.LoadFile<MetaplexJsonData>(met.data.url);

                    if (jsonData != null)
                    {
                        met.data.json = jsonData;
                        Texture2D texture = await FileLoader.LoadFile<Texture2D>(met.data.json.image);
                        FileLoader.SaveToPersistenDataPath<Texture2D>(Path.Combine(Application.persistentDataPath, $"{mint}.png"), texture);
                        if (texture)
                        {
                            NftImage nftImage = new NftImage();
                            nftImage.externalUrl = jsonData.image;
                            nftImage.file = texture;
                            met.nftImage = nftImage;
                        }
                    }
                    Nft newNft = new Nft(met);
                    FileLoader.SaveToPersistenDataPath<Nft>(Path.Combine(Application.persistentDataPath, $"{mint}.json"), newNft);
                    return newNft;
                }
            }
            return null;
        }

        public static Nft TryLoadNftFromLocal(string mint)
        {
            Nft local = FileLoader.LoadFileFromLocalPath<Nft>($"{Path.Combine(Application.persistentDataPath, mint)}.json");

            if (local != null)
            {
                Texture2D tex = FileLoader.LoadFileFromLocalPath<Texture2D>($"{Path.Combine(Application.persistentDataPath, mint)}.png");
                if (tex)
                {
                    local.metaplexData.nftImage = new NftImage();
                    local.metaplexData.nftImage.file = tex;
                }
                else
                {
                    return null;
                }
            }

            return local;
        }

        public static Solnet.Wallet.PublicKey CreateAddress(List<byte[]> seed, string programId)
        {
            List<byte> buffer = new List<byte>();

            foreach (byte[] item in seed)
            {
                if (item.Length > 32)
                {
                    throw new Exception("Too long");
                }

                buffer.AddRange(item);
            }

            buffer.AddRange(seed[1]);
            byte[] derive = Encoding.UTF8.GetBytes("ProgramDerivedAddress");
            buffer.AddRange(derive);

            SHA256 sha256 = SHA256.Create();
            byte[] hash1 = sha256.ComputeHash(buffer.ToArray());

            if (hash1.IsOnCurve() != 0)
            {
                throw new Exception("Not on curve");
            }

            Solnet.Wallet.PublicKey publicKey = new Solnet.Wallet.PublicKey(hash1);
            return publicKey;
        }

        public static Solnet.Wallet.PublicKey FindProgramAddress(string mintPublicKey, string programId = "metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s")
        {
            List<byte[]> seeds = new List<byte[]>();

            int nonce = 255;
            seeds.Add(Encoding.UTF8.GetBytes("metadata"));
            seeds.Add(new Solnet.Wallet.PublicKey(programId).KeyBytes);
            seeds.Add(new Solnet.Wallet.PublicKey(mintPublicKey).KeyBytes);
            seeds.Add(new[] { (byte)nonce });

            Solnet.Wallet.PublicKey publicKey = null;

            while (nonce != 0)
            {
                try
                {
                    seeds[3] = new[] { (byte)nonce };
                    publicKey = CreateAddress(seeds, programId);
                    return publicKey;
                }
                catch
                {
                    nonce--;
                    continue;
                }
            }

            return publicKey;
        }

        public static async Task<T> GetMetaplexJsonData<T>(string jsonUrl)
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(jsonUrl);
            response.EnsureSuccessStatusCode();

            try
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.Log(responseBody);
                T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseBody);
                client.Dispose();
                return data;
            }
            catch
            {
                client.Dispose();
                return default;
                throw;
            }
        }

    }
}
