using DBRXFSample.iOS;
using DBRiOS;
using Foundation;

[assembly:Xamarin.Forms.Dependency(typeof(PostMan))]
namespace DBRXFSample.iOS
{
    class PostMan : IPostMan
    {
        DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("t0068MgAAAEMKwCG/nGtAHejYbWgJH1sqDrUEhjbY2iIPP+rd//VnS2xWkcqSLMF3cxKetujwrYi4MxyyYl2qim4I1KKY3tk=");
        TextResult[] results;
        NSError error;
        public string getResult()
        {
            string path = NSBundle.MainBundle.PathForResource("qr", "PNG");
            UIKit.UIImage image = new UIKit.UIImage(path);
            results = reader.DecodeImage(image, "", out error);
            if(results != null && results.Length > 0)
            {
                return results[0].BarcodeText;
            }
            return "Barcode not found.";
        }
    }
}