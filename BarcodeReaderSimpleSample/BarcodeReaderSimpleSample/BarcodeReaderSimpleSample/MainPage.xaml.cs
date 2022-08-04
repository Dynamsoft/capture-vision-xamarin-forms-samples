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

        async void OnCustomRendererButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomRendererPage());
        }
    }
}
