using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Com.Dynamsoft.Barcode;
using System;
using DBRAndroidDemo;
using Android.Graphics;
using Java.IO;
using System.IO;
using System.Text;
using Android.Content.Res;

namespace DBRADroidDemo
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
                imgV.DrawingCacheEnabled = true;
                Bitmap bitmap = imgV.DrawingCache;

               
                ByteArrayOutputStream by = new ByteArrayOutputStream();
                MemoryStream baos = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, baos);               
                results = reader.DecodeFileInMemory(baos.ToArray(), "");               
                if (results != null && results.Length > 0)
                {
                    textV.Text = results[0].BarcodeText ;
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

