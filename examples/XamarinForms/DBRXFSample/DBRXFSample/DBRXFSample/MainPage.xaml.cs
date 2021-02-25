using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;
using DBRXFSample.Interfaces;
using DBRXFSample.ViewModels;
using Xamarin.Forms.Xaml;

namespace DBRXFSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //NavigationPage.SetHasNavigationBar(this, false);
            Viewmodel = new CaptureViewModel();
            BindingContext = Viewmodel;
        }
        protected override void OnAppearing()
        {
            CaptureHandler = App.CurrentCaptureUI;

            Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(t =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Setup();
                });
            });
            base.OnAppearing();
        }

        public void Setup()
        {
            if (CaptureHandler != null)
            {
                // Start the Capture Session.
                if (!CaptureHandler.GetSessionActive())
                {
                    CaptureHandler.StartSession();
                }
            }
            Viewmodel.StartCaptureSequence();
        }
        /// <summary>
        /// The Viewmodel.
        /// </summary>
        private CaptureViewModel Viewmodel { get; }

        /// <summary>
        /// The Handler for the Native CaptureUI control.
        /// </summary>
        private ICaptureUI CaptureHandler { get; set; }

        private bool isflash = false;
        private void flash_Clicked(object sender, EventArgs e)
        {
            CaptureHandler.onClickFlash();
            if (!isflash)
            {
                flash.ImageSource = "flashoff.png";
            }
            else
            {
                flash.ImageSource = "flashon.png";
            }
            isflash = !isflash;
        }
    }
}
