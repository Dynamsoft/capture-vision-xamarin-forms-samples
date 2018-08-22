using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Com.Dynamsoft.Barcode;
using Android.Views;

namespace DBRDroidDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ImageView imgV;
        TextView textV;
        Button btnRead;
        Button btnReset;

        BarcodeReader reader;
        TextResult[] results;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            imgV = (ImageView)FindViewById(Resource.Id.imageView1);
            textV = (TextView)FindViewById(Resource.Id.textView1);
            btnRead = (Button)FindViewById(Resource.Id.button1);
            btnReset = (Button)FindViewById(Resource.Id.button2);

            reader = new BarcodeReader("");

            imgV.SetWillNotCacheDrawing(false);
            btnRead.Click += delegate
            {
                //results = reader.DecodeBufferedImage(Android.Graphics.BitmapFactory.DecodeResource(Resources,Resource.Drawable.QRcode), "");
                imgV.DrawingCacheEnabled = true;
                results = reader.DecodeBufferedImage(imgV.DrawingCache, "");
                if (results != null && results.Length > 0)
                {
                    textV.Text = results[0].BarcodeText;
                }
            };
            btnReset.Click += delegate
            {
                textV.Text = "";
            };
            System.Console.WriteLine("OnCreate");
            System.Diagnostics.Debug.WriteLine("OnCreate1");
        }
    }
}

