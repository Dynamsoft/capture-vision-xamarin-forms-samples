using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Runtime;
using Android.Graphics;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using Com.Dynamsoft.Barcode;
using System.Collections.Generic;

namespace CameraDemo
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.FullScreen", MainLauncher = true)]
	public class MainActivity : AppCompatActivity, Android.Hardware.Camera.IPreviewCallback, ISurfaceHolderCallback,Android.Support.V4.App.ActivityCompat.IOnRequestPermissionsResultCallback
    {
        private Android.Hardware.Camera camera;
        private SurfaceView surface = null;
        private ImageButton flahBtn;
        private Button zoomInBtn;
        private Button zoomOutBtn;
        private bool flashOn;
        

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);


            tv_text = FindViewById<TextView>(Resource.Id.tv_text);
            surface = FindViewById<SurfaceView>(Resource.Id.sv_surfaceView);
            zoomInBtn = FindViewById<Button>(Resource.Id.btZoomin);
            zoomInBtn.Click += delegate
            {
                if (camera != null)
                {
                    Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
                    int curZoom = parameters.Zoom;
                    if(curZoom+1 <= parameters.MaxZoom)
                    {
                        parameters.Zoom = curZoom + 1;
                        camera.SetParameters(parameters);
                    }
                }
            };
            zoomOutBtn = FindViewById<Button>(Resource.Id.btZoomout);
            zoomOutBtn.Click += delegate
            {
                if (camera != null)
                {
                    Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
                    int curZoom = parameters.Zoom;
                    if (curZoom - 1 >= 0)
                    {
                        parameters.Zoom = curZoom - 1;
                        camera.SetParameters(parameters);
                    }
                }
            };

            var holder = surface.Holder;
            holder.AddCallback(this);

            flashOn = false;
            flahBtn = FindViewById<ImageButton>(Resource.Id.flahBtn);
            flahBtn.Click += delegate
            {
                if (camera == null)
                    return;

                Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
                if (!flashOn)
                {
                    parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
                    flahBtn.SetImageResource(Resource.Drawable.flashoff);
                    flashOn = true;
                }
                else
                {
                    parameters.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
                    flahBtn.SetImageResource(Resource.Drawable.flashon);
                    flashOn = false;
                }
                camera.SetParameters(parameters);
            };
        }
        
        private static BarcodeReader barcodeReader = new BarcodeReader("t0068MgAAABlOih7jbq10NbpqRczC1hLLu/qZsLWkVW0KBu7u/wruw4zcQXlnIMAWCZ/5cnA/JTRvQ4h+syYnev0wYNWAfX8=");
        private static TextView tv_text;
        private static MyHandler myHandler = new MyHandler();
        private static int previewWidth;
        private static int previewHeight;
        private static YuvImage yuvImage;
        private static int[] stride;
        private static bool isReady = true;
        private static bool fromBack = false;
        public const int REQUEST_CAMERA_PERMISSION = 1;

        protected override void OnResume()
        {
            base.OnResume();
            if(fromBack)
            {
                surface.Holder.AddCallback(this);
                fromBack = false;
            }
        }

        protected override void OnPause()
        {
            fromBack = true;
            base.OnPause();
        }

        private void OpenCamera()
        {
            if (CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted)
            {
                RequestCameraPermission();
                return;
            }


            camera = Android.Hardware.Camera.Open();
            Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
            parameters.PictureFormat = ImageFormatType.Jpeg;
            parameters.PreviewFormat = ImageFormatType.Nv21;
            parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousVideo;
            IList<Android.Hardware.Camera.Size> suportedPreviewSizes = parameters.SupportedPreviewSizes;
            int i = 0;
            for (i=0;i<suportedPreviewSizes.Count;i++)
            {
                if (suportedPreviewSizes[i].Width < 1000) break;
            }
            parameters.SetPreviewSize(suportedPreviewSizes[i].Width,suportedPreviewSizes[i].Height);
            camera.SetParameters(parameters);
            camera.SetDisplayOrientation(90);
            camera.SetPreviewCallback(this);
            camera.SetPreviewDisplay(surface.Holder);
            camera.StartPreview();           
            //Get camera width
            previewWidth = parameters.PreviewSize.Width;
            //Get camera height
            previewHeight = parameters.PreviewSize.Height;

            //Resize SurfaceView Size
            float scaledHeight = previewWidth * 1.0f * surface.Width / previewHeight;
            float prevHeight = surface.Height;
            ViewGroup.LayoutParams lp = surface.LayoutParameters;
            lp.Width = surface.Width;
            lp.Height = (int)scaledHeight;
            surface.LayoutParameters = lp;
            surface.Top = (int)((prevHeight - scaledHeight) / 2);
            surface.DrawingCacheEnabled = true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch(requestCode)
            {
                case REQUEST_CAMERA_PERMISSION:
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        OpenCamera();
                    else
                        Toast.MakeText(ApplicationContext, "This App need permission to access camera.", ToastLength.Long).Show();
                    return;
            }
        }

        public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            try
            {
                try
                {
                    
                    yuvImage = new YuvImage(data, ImageFormatType.Nv21,
                            previewWidth, previewHeight, null);
                    
                    stride = yuvImage.GetStrides();
                    if(isReady)
                    {
                        isReady = false;
                        new ReaderTask().Execute();
                    }
                }
                catch (System.Exception exp)
                {
                    Message msg = new Message();
                    msg.What = 0x001;
                    msg.Obj = "except";
                    myHandler.SendMessage(msg);

                }
               

            }
            catch (System.IO.IOException)
            {


            }

        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            //throw new NotImplementedException();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {

            OpenCamera();

        }
        
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            holder.RemoveCallback(this);
            camera.SetPreviewCallback(null);
            camera.StopPreview();
            camera.Release();
            camera = null;

        }

        private void RequestCameraPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this,
                                new string[] { Manifest.Permission.Camera }, MainActivity.REQUEST_CAMERA_PERMISSION);
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera },
                        REQUEST_CAMERA_PERMISSION);
            }
        }

        
        class MyHandler : Handler
        {
            public override void HandleMessage(Message msg)
            {
                isReady = true;
                tv_text.Text = msg.Obj.ToString();
                
            }
        }


        class ReaderTask : AsyncTask
        {
            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                try
                {
                    TextResult[] text = barcodeReader.DecodeBuffer(yuvImage.GetYuvData(), previewWidth, previewHeight, stride[0], EnumImagePixelFormat.IpfNv21, "");

                    Message msg = new Message();
                    msg.What = 0x01;
                    if (text != null && text.Length > 0)
                    {
                        for(int i = 0;i<text.Length;i++)
                        {
                            if (i == 0)
                                msg.Obj = "Code[1]: " + text[0].BarcodeText;
                            else msg.Obj = msg.Obj + "\n\n" + "Code[" + (i + 1) + "]: " + text[i].BarcodeText;
                        }

                    }
                    else msg.Obj = "";

                    myHandler.SendMessage(msg);
                }
                catch (BarcodeReaderException e)
                {
                    e.PrintStackTrace();
                }

                return null;
            }
        }
    }
}

