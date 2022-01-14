using UnityEngine;
using ZXing;
using ZXing.QrCode;


namespace AllArt.Solana.Example
{
    public static class QRGenerator
    {
        private static Color32[] EncodeQRCode(string textForEncoding, int width, int height)
        {
            BarcodeWriter qrWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return qrWriter.Write(textForEncoding);
        }

        public static Texture2D GenerateQRTexture(string text, int width, int height)
        {
            Texture2D qrTexture = new Texture2D(width, height);
            Color32[] color = EncodeQRCode(text, width, height);
            qrTexture.SetPixels32(color);
            qrTexture.Apply();
            return qrTexture;
        }

        public static Result ReadQR(Color32[] colorSpace, int width, int height)
        {
            BarcodeReader reader = new BarcodeReader();
            Result result = reader.Decode(colorSpace, width, height);

            return result;
        }
    }
}
