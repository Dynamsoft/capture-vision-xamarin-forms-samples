using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Com.Dynamsoft.Dbr;
using Com.Dynamsoft.Camera.View;
using Com.Dynamsoft.Camera.Option;
using Com.Dynamsoft.Camera.Listener;
using DBRXFSample.Droid;
using DBRXFSample.Interfaces;
//using Com.Dynamsoft.Camera.Entity;

[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
[assembly: Xamarin.Forms.ExportRenderer(typeof(DBRXFSample.Controls.CaptureUI), typeof(MainActivity))]
namespace DBRXFSample.Droid
{
    [Activity(Label = "DBRXFSample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IResultListener, ICaptureUI
    {

        private TextView tvResult = null;
        private ImageButton flashBtn;
        private Com.Dynamsoft.Camera.Entity.Camera camera;
        private static BarcodeReader barcodeReader = new BarcodeReader("put your license here");
        private static int previewWidth;
        private static int previewHeight;
        private static YuvImage yuvImage;
        private static int[] stride;
        private static bool isReady = true;
        private static bool fromBack = false;
        private static string result;
        private bool flashOn;
        public static Android.Content.Context context;
        public string resultStr;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CameraView cameraView;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.content_main);
            tvResult = FindViewById<TextView>(Resource.Id.tv_result);
            cameraView = FindViewById<CameraView>(Resource.Id.camera_view);
            tvResult.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;

            context = this;
            camera = new Com.Dynamsoft.Camera.Entity.Camera(this);
            camera.SetBarcodeReader(barcodeReader);//bind barcodereader and camera
            camera.AddCameraView(cameraView);//bind view and camera
            camera.SetEnableBeepSound(true);//enable beepsound
            cameraView.AddOverlay();//enable overlay
            camera.AddResultListener(this);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            App.CurrentCaptureUI = this;
            LoadApplication(new App());
            flashOn = false;
            flashBtn = FindViewById<ImageButton>(Resource.Id.flashBtn);
            flashBtn.Click += delegate
            {
                if (camera == null)
                    return;
                if (!flashOn)
                {
                    camera.TorchDesiredState = TorchState.TorchStateOn;
                    flashOn = true;
                }

                else
                {
                    camera.TorchDesiredState = TorchState.TorchStateOff;
                    flashOn = false;
                }
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            fromBack = false;
            camera.CameraDesireState = CameraState.CameraStateOn;//open camera and start scanning
        }

        protected override void OnPause()
        {
            fromBack = true;
            camera.CameraDesireState = CameraState.CameraStateOff;//close camera and stop scanning
            base.OnPause();
        }

        void IResultListener.OnGetResult(TextResult[] p0, Frame p1)
        {

            RunOnUiThread(() =>
            {
                resultStr = p0[0].BarcodeText;
                tvResult.Text = p0[0].BarcodeText;
            });
        }

        public void StartSession()
        {
        }

        public void StopSession()
        {
        }

        public bool GetSessionActive()
        {
            return false;
        }

        public string GetResults()
        {
            return resultStr;
        }

        public void onClickFlash()
        {
        }
    }
}