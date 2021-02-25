using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Com.Dynamsoft.Barcode;
using DBRXFSample.Droid;
using DBRXFSample.Interfaces;
using static Android.Hardware.Camera;

namespace DBRXFSample.Droid
{
    [Activity(Label = "DBRXFSample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ISurfaceHolderCallback, IPreviewCallback, ICaptureUI, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            try
            {
                yuvImage = new YuvImage(data, ImageFormatType.Nv21,
                        previewWidth, previewHeight, null);
                stride = yuvImage.GetStrides();
                try
                {
                    if (isReady)
                    {
                        if (backgroundHandler != null)
                        {
                            isReady = false;
                            Message msg = new Message();
                            msg.What = 100;
                            msg.Obj = yuvImage;
                            backgroundHandler.SendMessage(msg);
                            backgroundHandler.Post(() =>
                            {
                                //tvResult.Text = result;
                            });
                        }
                    }
                }
                catch (BarcodeReaderException e)
                {
                    e.PrintStackTrace();
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
            if (camera != null)
            {
                camera.SetPreviewCallback(null);
                camera.StopPreview();
                camera.Release();
                camera = null;
            }
            if (handlerThread != null)
            {
                handlerThread.QuitSafely();
                handlerThread.Join();
                handlerThread = null;
            }
            backgroundHandler = null;
        }

        private SurfaceView surface = null;
        private Android.Hardware.Camera camera;

        private static BarcodeReader barcodeReader = new BarcodeReader("t0068MgAAAByo0OdFR2KWLO5/rjTOorKni0BLRFwoXKdjNhJVOziu1tC6OG3+qWQpJYRcnSOT6AR+6OJDeXwKTc79buYbtDY=");
        private static MyHandler myHandler = new MyHandler();
        private static int previewWidth;
        private static int previewHeight;
        private static YuvImage yuvImage;
        private static int[] stride;
        private static bool isReady = true;
        private static bool fromBack = false;
        public const int REQUEST_CAMERA_PERMISSION = 1;
        private HandlerThread handlerThread;

        private MyHandler backgroundHandler;
        private static string result;
        private bool flashOn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            flashOn = false;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            surface = new SurfaceView(this);
            layoutParams.Gravity = GravityFlags.CenterHorizontal;
            surface.Holder.AddCallback(this);
            AddContentView(surface, layoutParams);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            App.CurrentCaptureUI = this;
        }


        protected override void OnResume()
        {
            base.OnResume();
            if (fromBack)
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

        private void RequestCameraPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera }, MainActivity.REQUEST_CAMERA_PERMISSION);
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.Camera },
                        REQUEST_CAMERA_PERMISSION);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case REQUEST_CAMERA_PERMISSION:
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        OpenCamera();
                    else
                        Android.Widget.Toast.MakeText(ApplicationContext, "This App need permission to access camera.", Android.Widget.ToastLength.Long).Show();
                    return;
            }
        }

        private void OpenCamera()
        {
            if (CheckSelfPermission(Manifest.Permission.Camera) != Permission.Granted)
            {
                RequestCameraPermission();
                return;
            }

            camera = Open();
            Parameters parameters = camera.GetParameters();
            parameters.PictureFormat = ImageFormatType.Jpeg;
            parameters.PreviewFormat = ImageFormatType.Nv21;
            if (parameters.SupportedFocusModes.Contains(Parameters.FocusModeContinuousVideo))
            {
                parameters.FocusMode = Parameters.FocusModeContinuousVideo;
            }
            IList<Size> suportedPreviewSizes = parameters.SupportedPreviewSizes;
            int i = 0;
            float reqRatio = (float)surface.Height / surface.Width;
            float curRatio, deltaRatio;
            float deltaRatioMin = 10;
            int sizeW = 0;
            int sizeH = 0;
            for (i = 0; i < suportedPreviewSizes.Count; i++)
            {
                curRatio = ((float)suportedPreviewSizes[i].Width) / suportedPreviewSizes[i].Height;
                deltaRatio = Math.Abs(reqRatio - curRatio);
                if (deltaRatio < deltaRatioMin)
                {
                    deltaRatioMin = deltaRatio;
                    sizeW = suportedPreviewSizes[i].Width;
                    sizeH = suportedPreviewSizes[i].Height;
                }
            }
            parameters.SetPreviewSize(sizeW, sizeH);
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
            
            handlerThread = new HandlerThread("background");
            handlerThread.Start();
            backgroundHandler = new MyHandler(Looper.MainLooper);
        }
        public bool GetSessionActive()
        {
            return true;
        }

        public void StartSession()
        {
            //throw new NotImplementedException();
        }

        public void StopSession()
        {
            //throw new NotImplementedException();
        }

        public void onClickFlash()
        {
            if (camera == null)
                return;

            Parameters parameters = camera.GetParameters();
            if (!flashOn)
            {
                parameters.FlashMode = Parameters.FlashModeTorch;
                flashOn = true;
            }
            else
            {
                parameters.FlashMode = Parameters.FlashModeOff;
                flashOn = false;
            }
            camera.SetParameters(parameters);
        }

        public string GetResults()
        {
            return result;
        }

        class MyHandler : Handler
        {
            public MyHandler() : base()
            {
            }
            public MyHandler(Looper looper) : base(looper)
            {
            }
            public override void HandleMessage(Message msg)
            {
                if (msg.What == 100)
                {
                    Message msg1 = new Message();
                    msg1.What = 200;
                    msg1.Obj = "";
                    try
                    {
                        YuvImage image = (YuvImage)msg.Obj;
                        if (image != null)
                        {
                            int[] stridelist = image.GetStrides();
                            TextResult[] text = barcodeReader.DecodeBuffer(image.GetYuvData(), previewWidth, previewHeight, stridelist[0], EnumImagePixelFormat.IpfNv21, "");
                            if (text != null && text.Length > 0)
                            {
                                for (int i = 0; i < text.Length; i++)
                                {
                                    if (i == 0)
                                        msg1.Obj = "Code[1]: " + text[0].BarcodeText;
                                    else
                                        msg1.Obj = msg1.Obj + "\n\n" + "Code[" + (i + 1) + "]: " + text[i].BarcodeText;
                                }
                            }
                        }
                    }
                    catch (BarcodeReaderException e)
                    {
                        msg1.Obj = "";
                        e.PrintStackTrace();
                    }

                    isReady = true;
                    myHandler.SendMessage(msg1);

                }
                else if (msg.What == 200)
                {
                    result = msg.Obj.ToString();
                }
            }
        }
    }
}