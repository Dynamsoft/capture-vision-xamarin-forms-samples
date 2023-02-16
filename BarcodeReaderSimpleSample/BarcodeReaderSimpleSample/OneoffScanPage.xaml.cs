using System;
using System.Collections.Generic;

using Xamarin.Forms;
using DCVXamarin;

namespace BarcodeReaderSimpleSample
{
    public partial class OneoffScanPage : ContentPage, IBarcodeResultListener
    {
        public EventHandler<string> Value;

        public OneoffScanPage()
        {
            InitializeComponent();
            App.dbr.AddResultListener(this);
            // Switch barcode decoding template.
            // VIDEO_SINGLE_BARCODE: High performance on processing single barcode.
            // VIDEO_SPEED_FIRST: High processing speed but not good at processing badly printed barcodes or blurry images.
            // VIDEO_READ_RATE_FIRST: High read rate. Able to decode the badly printed or blurry barcodes but takes a bit longer time on each video frame.
            App.dbr.UpdateRuntimeSettings(EnumDBRPresetTemplate.VIDEO_SINGLE_BARCODE);
            App.dbr.SetCameraEnhancer(App.dce);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.dbr.StartScanning();
            App.dce.Open();           
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            App.dbr.StopScanning();
            App.dce.Close();           
        }

        public void BarcodeResultCallback(int frameID, BarcodeResult[] textResults)
        {
            if (textResults != null && textResults.Length > 0)
            {
                if (textResults[0].BarcodeText != null )
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        EventHandler<string> handler = Value;
                        if (handler != null)
                        {
                            string str = "One-off Scan result:\n" + textResults[0].BarcodeText;
                            handler(this, str);
                        }
                        Navigation.PopAsync();
                    });
                }

            }
        }

    }
}
