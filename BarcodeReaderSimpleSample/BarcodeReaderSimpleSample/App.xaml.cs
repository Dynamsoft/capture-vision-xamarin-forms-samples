using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DCVXamarin;

namespace BarcodeReaderSimpleSample
{
    public partial class App : Application, ILicenseVerificationListener
    {
        public static IDCVCameraEnhancer dce;
        public static IDCVBarcodeReader dbr;

        public App(IDCVCameraEnhancer enhancer, IDCVBarcodeReader reader)
        {
            InitializeComponent();
            dce = enhancer;
            dbr = reader;
            // Initialize license for the Barcode Decoding module of Dynamsoft Capture Vision.
            // The license string here is a time-limited trial license. Note that network connection is required for this license to work.
            // You can also request an extension for your trial license in the customer portal: https://www.dynamsoft.com/customer/license/trialLicense?product=dbr&utm_source=installer&package=xamarin
            dbr.InitLicense("DLS2eyJvcmdhbml6YXRpb25JRCI6IjIwMDAwMSJ9", this);
            MainPage = new NavigationPage(new MainPage());
        }

        public void LicenseVerificationCallback(bool isSuccess, string errorMsg)
        {
            if (!isSuccess)
            {
                System.Console.WriteLine(errorMsg);
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
