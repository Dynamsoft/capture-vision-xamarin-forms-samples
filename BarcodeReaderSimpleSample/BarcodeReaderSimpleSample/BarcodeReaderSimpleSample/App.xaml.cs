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
