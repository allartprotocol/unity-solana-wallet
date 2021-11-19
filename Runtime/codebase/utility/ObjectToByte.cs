using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Merkator.BitCoin;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AllArt.Solana.Utility
{
    public static class ObjectToByte
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static byte[] getBytes(CompiledInstruction str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static CompiledInstruction fromBytes(byte[] arr)
        {
            CompiledInstruction str = new CompiledInstruction();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (CompiledInstruction)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }

        public static void DecodeBase58StringFromByte(byte[] data, int offset, int length, out string decodedData)
        {
            decodedData = "";
            byte[] dataCopy = new byte[length];
            Array.Copy(data, (long)offset, dataCopy, 0, length);

            decodedData = Base58Encoding.Encode(dataCopy);
        }

        public static void DecodeUTF8StringFromByte(byte[] data, int offset, int length, out string decodedData)
        {
            decodedData = "";
            byte[] dataCopy = new byte[length];
            Array.Copy(data, (long)offset, dataCopy, 0, length);

            decodedData = Encoding.UTF8.GetString(dataCopy).Replace("\u0000", "");
        }

        public static void DecodeUlongFromByte(byte[] data, int offset, out ulong decodedData)
        {
            decodedData = 0;
            byte[] dataCopy = new byte[8];
            Array.Copy(data, (long)offset, dataCopy, 0, 8);
            decodedData = BitConverter.ToUInt64(dataCopy, 0);
        }

        public static void DecodeUIntFromByte(byte[] data, int offset, out uint decodedData)
        {
            decodedData = 0;
            byte[] dataCopy = new byte[4];
            Array.Copy(data, (long)offset, dataCopy, 0, 4);
            decodedData = BitConverter.ToUInt32(dataCopy, 0);
        }
    }
}

public static class JtokenExtension
{
    public static T GetValue<T>(this JToken jToken, string key, T defaultValue = default(T))
    {
        dynamic ret = jToken[key];
        if (ret == null) return defaultValue;
        if (ret is JObject) return JsonConvert.DeserializeObject<T>(ret.ToString());
        return (T)ret;
    }
}
