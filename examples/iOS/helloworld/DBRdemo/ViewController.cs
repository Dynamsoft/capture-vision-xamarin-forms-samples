using System;
using UIKit;
using DBRiOS;
using CoreGraphics;

namespace DBRdemo
{
    public partial class ViewController : UIViewController
    {
        private bool haveRead = false;

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

        partial void OnReadBtnClick(UIButton sender)
        {
            if(haveRead)
            {
                label.Text = "";
                haveRead = false;
                readBtn.SetTitle("Read", UIControlState.Normal);
            }
            else
            {
                Foundation.NSError error = new Foundation.NSError();
                DynamsoftBarcodeReader barcodeReader = new DynamsoftBarcodeReader("");
                iTextResult[] result = barcodeReader.DecodeImage(qrimage.Image, "", out error);
                label.Text = result[0].BarcodeText;
                readBtn.SetTitle("Reset", UIControlState.Normal);
            }
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);
        }
    }
}
