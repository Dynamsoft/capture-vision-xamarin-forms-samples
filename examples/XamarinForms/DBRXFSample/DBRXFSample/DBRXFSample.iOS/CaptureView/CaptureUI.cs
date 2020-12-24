using System;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using DBRXFSample.Interfaces;
using DBRiOS;

[assembly: Dependency(typeof(DBRXFSample.iOS.CaptureView.CaptureUI))]
[assembly: ExportRenderer(typeof(DBRXFSample.Controls.CaptureUI), typeof(DBRXFSample.iOS.CaptureView.CaptureUI))]
namespace DBRXFSample.iOS.CaptureView
{
    public class CaptureUI : ViewRenderer, ICaptureUI, IDBReaderListener//, IDBRServerLicenseVerificationDelegate
    {
        string textResults = "";
        DBCaptureView captureView;
        DynamsoftBarcodeCamera camera;
        DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("put your license here");
        public CaptureUI()
        {
            App.CurrentCaptureUI = this;
        }

        public void barcodeReader(ReaderPackage reader, FramePackage frame)
        {
            //reader.error: DynamsoftBarcodeReader errors
            //FramePackage: information about the frame decoding
            if (reader.barcodeResults.Length > 0)
            {
                textResults = "Value: " + reader.barcodeResults[0].BarcodeText;
            }
        }

        public void StartSession()
        {
            //scan view with a CGRect
            captureView = new DBCaptureView(new CGRect(0f, 0f, NativeView.Bounds.Width, NativeView.Bounds.Height));

            //add overlay with stroke and fill color
            captureView.AddOverlay(UIColor.Green.ColorWithAlpha(0.5f), UIColor.Green.ColorWithAlpha(0.5f));

            UIImage off = new UIImage("flashoff.png");
            UIImage on = new UIImage("flashon.png");
            //add torch with images and its CGRect
            captureView.AddTorch(off, on, new CGRect(NativeView.Bounds.Width / 2 - 25, 50, 50, 50));

            //if you need to beepsound while decoding, add file path here 
            camera = new DynamsoftBarcodeCamera(captureView, "pi.wav")
            {
                IsEnable = true //enable camera gets frames
            };

            camera.StartScanning();

            camera.SetResolution(Resolution.Resolution1080P);

            //bing DynamsoftBarcodeReader instance
            camera.BindReader(reader);

            //results callback
            camera.AddDecodeListener(Self);

            camera.setEnableBeepSound(true);

            //set an interval > 0, the unit is seconds
            //During this time, there will be no duplicate barcodes
            camera.setDuplicateBarcodesFilter(1);

            //set an interval [0, 5]
            //t = 0: Continuous
            //t = [1, 5]: wait (t)s after each decode is completed
            camera.setContinuousScan(0);
            NativeView.AddSubview(captureView);
        }

        public bool GetSessionActive()
        {
            return false;
        }

        public void StopSession()
        {

        }

        public string GetResults()
        {
            return textResults;
        }

        protected override void Dispose(bool disposing)
        {
            camera.StopScanning();
            base.Dispose(disposing);
        }

        void ICaptureUI.onClickFlash()
        {

        }

        //void IDBRServerLicenseVerificationDelegate.Error(bool isSuccess, NSError error)
        //{
        //    if (isSuccess)
        //    {
        //        Console.WriteLine("success");
        //    }
        //    else {
        //        Console.WriteLine("error = " + error);
        //    }
        //}
    }
}