using System;
using UIKit;
using DBRiOS;

namespace DBRdemo
{
    public partial class ViewController : UIViewController
    {
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

        partial void onReadBtnClick(UIButton sender)
        {
            Foundation.NSError error = new Foundation.NSError();
            DynamsoftBarcodeReader barcodeReader = new DynamsoftBarcodeReader("");
            TextResult[] result = barcodeReader.DecodeImage(qrimage.Image, "", out error);
            text.Text = result[0].BarcodeText;
        }

        partial void onResetBtnClick(UIButton sender)
        {
            text.Text = "";
        }
    }
}