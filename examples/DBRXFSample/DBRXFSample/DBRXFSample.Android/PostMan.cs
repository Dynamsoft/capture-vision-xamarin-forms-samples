using Android.Content;
using Com.Dynamsoft.Barcode;
using DBRXFSample.Droid;
using Android.Graphics;

[assembly: Xamarin.Forms.Dependency(typeof(PostMan))]
namespace DBRXFSample.Droid
{
    public class PostMan : IPostMan
    {
        BarcodeReader reader = new BarcodeReader("t0068MgAAAEMKwCG/nGtAHejYbWgJH1sqDrUEhjbY2iIPP+rd//VnS2xWkcqSLMF3cxKetujwrYi4MxyyYl2qim4I1KKY3tk=");
        TextResult[] results;
        Bitmap bitmap;
        Context context = Android.App.Application.Context;
        long time = 0;
        public string getResult()
        {
            if(bitmap==null)
                bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.qr);
            time = Java.Lang.JavaSystem.CurrentTimeMillis();
            results = reader.DecodeBufferedImage(bitmap, "");
            time = Java.Lang.JavaSystem.CurrentTimeMillis() - time;
            if (results != null && results.Length > 0)
                return results[0].BarcodeText;
            return "Barcode not found.";
        }
    }
}