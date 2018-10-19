using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DBRXFSample
{
    public partial class MainPage : ContentPage
    {
        private bool haveRead = false;
        public MainPage()
        {
            InitializeComponent();
            label.HorizontalTextAlignment = TextAlignment.Center;
        }

        void OnBtnReadClick(object sender, EventArgs args)
        {
            if (haveRead)
            {
                label.Text = "";
                btn.Text = "Read";
                haveRead = false;
            }
            else
            {
                var postMan = DependencyService.Get<IPostMan>();
                label.Text = postMan.getResult();
                btn.Text = "Clear";
                haveRead = true;
            }
        }
    }
    
    public interface IPostMan
    {
        string getResult();
    }
}
