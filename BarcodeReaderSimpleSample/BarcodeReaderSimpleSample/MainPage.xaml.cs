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

        // Go to the page of one-off barcode scanning.
        // One-off scan: Scan barcode(s) from the video streaming and stop scanning after barcode results are output.
        // You might use the barcode result(s) to fill-in forms or move to another page.
        // In this sample, we go back to the front page and display the barcode result.
        async void oneoffscanButton_Clicked(object sender, EventArgs e)
        {
            var page = new OneoffScanPage();
            page.Value += (obj, str) =>
            {
                mainPageLabel.Text = str;
            };
            await Navigation.PushAsync(page);
        }
        
        // Go to the page of continuous barcode scanning.
        // Continuous scanning: Scan barcode(s) from the video streaming and output barcode results continuously.
        // You can add configurations like duplicate result filter, result verification, control the barcode decoding interval, et al.
        async void OnCustomRendererButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomRendererPage());
        }
    }
}
