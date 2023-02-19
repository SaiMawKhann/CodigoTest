using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using ZXing;
using ZXing.QrCode;

namespace CodigoTest.Models
{
    public class QRCode
    {
        public void GenerateQRCode(string qrText)
        {
            Byte[] byteArray;
            var width = 250; // width of the Qr Code   
            var height = 250; // height of the Qr Code   
            var margin = 0;
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(qrText);
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    // save to stream as PNG   
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();

                }
            }
        }

        public void GenerateFile(string qrText)
        {
            Byte[] byteArray;
            var width = 250; // width of the Qr Code   
            var height = 250; // height of the Qr Code 
            var margin = 0;
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(qrText);
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image   
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    // save to folder
                    string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                }
            }
        }
        //public void ViewFile()
        //{
            //List<KeyValuePair<string, string>> fileData = new List<KeyValuePair<string, string>>();
            //KeyValuePair<string, string> data;

            ////string[] files = Directory.GetFiles(Server.MapPath("~/qrr"));
            ////foreach (string file in files)
            ////{
            ////    // create a barcode reader instance
            ////    IBarcodeReader reader = new BarcodeReader();
            ////    // load a bitmap
            ////    var barcodeBitmap = (Bitmap)Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("~/qrr") + "/" + Path.GetFileName(file));
            ////    // detect and decode the barcode inside the bitmap
            ////    var result = reader.Decode(barcodeBitmap);
            //    // do something with the result
            //    data = new KeyValuePair<string, string>(result.ToString(), "/qrr/" + Path.GetFileName(file));
            ////    fileData.Add(data);
            //}
      //  }
    }
}
