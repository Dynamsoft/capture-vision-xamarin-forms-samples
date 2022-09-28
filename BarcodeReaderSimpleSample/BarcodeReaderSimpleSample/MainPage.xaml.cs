using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BarcodeReaderSimpleSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void oneoffscanButton_Clicked(object sender, EventArgs e)
        {
            var page = new OneoffScanPage();
            page.Value += (obj, str) =>
            {
                mainPageLabel.Text = str;
            };
            await Navigation.PushAsync(page);
        }

        async void OnCustomRendererButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomRendererPage());
        }
    }
}
