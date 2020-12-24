using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DBRXFSample.Interfaces;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DBRXFSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        /// <summary>
        /// The Capture UI Handler.
        /// </summary>
        public static ICaptureUI CurrentCaptureUI { get; set; }
    }
}
