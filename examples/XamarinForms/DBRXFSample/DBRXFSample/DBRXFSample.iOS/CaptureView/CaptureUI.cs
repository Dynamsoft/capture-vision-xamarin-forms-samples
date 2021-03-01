using System;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreVideo;

using AVFoundation;
using Foundation;
using UIKit;
using DBRXFSample.Interfaces;
using CoreFoundation;
using ObjCRuntime;
using DBRiOS;

[assembly: Dependency(typeof(DBRXFSample.iOS.CaptureView.CaptureUI))]
[assembly: ExportRenderer(typeof(DBRXFSample.Controls.CaptureUI), typeof(DBRXFSample.iOS.CaptureView.CaptureUI))]
namespace DBRXFSample.iOS.CaptureView
{
    public class CaptureUI : ViewRenderer, ICaptureUI, IDBRServerLicenseVerificationDelegate//, IDMLTSLicenseVerificationDelegate
    {
        public static bool OnDevice = Runtime.Arch == Arch.DEVICE;
        private CaptureOutput captureOutput = new CaptureOutput();
        private string result = "";
        private bool flashOn = false;
        private NSError error;
        private AVCaptureDevice device = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);
        AVCaptureVideoPreviewLayer videoPreviewLayer = new AVCaptureVideoPreviewLayer();

        private AVCaptureSession captureSession = new AVCaptureSession
        {
            SessionPreset = AVCaptureSession.PresetHigh
        };
        private AVCaptureDeviceInput videoDeviceInput;
        private AVCaptureVideoDataOutput videoDataOutput;
        private UIView liveCameraStream;

        private bool isinitWithLicenseKey = false;
        private string licenseKey = "put your licenseKey here";
        DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("put your license here");

        /// <summary>
        /// Determines if the Capture Session is active.
        /// </summary>
        public bool GetSessionActive()
        {
            return captureSession?.Running ?? false;
        }
        private const AVCaptureVideoOrientation VideoOrientation = AVCaptureVideoOrientation.Portrait;

        /// <summary>
        /// Communicate with the session and other session objects on this queue.
        /// </summary>
        private readonly DispatchQueue sessionQueue = new DispatchQueue("sessionqueue");

        public CaptureUI()
        {
            App.CurrentCaptureUI = this;
        }

        private void InitLicenseKey()
        {
            reader = new DynamsoftBarcodeReader("", licenseKey, Self);

            //dbr 8.x
            //iDMLTSConnectionParameters parameters = new iDMLTSConnectionParameters();
            //parameters.HandshakeCode = "******";
            ////parameters.SessionPassword = "******";
            //reader = new DynamsoftBarcodeReader(parameters, Self);
        }

        void IDBRServerLicenseVerificationDelegate.Error(bool isSuccess, NSError error)
        {
            if (error != null)
            {
                Console.WriteLine("UserInfo:" + error.UserInfo);
            }
        }

        //void IDMLTSLicenseVerificationDelegate.Error(bool isSuccess, NSError error)
        //{
        //    if (isSuccess)
        //    {
        //        Console.WriteLine("UserInfo:" + error.UserInfo);
        //    }
        //}

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (isinitWithLicenseKey) {
                InitLicenseKey();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SetupUserInterface();
            if (OnDevice)
            {
                AuthorizeCameraUse();
                sessionQueue.DispatchAsync(() =>
                {
                    SetupSession();
                });
            }
            else
            {
                var heightScale = (double)9 / 16;
                var vidHeight = NativeView.Frame.Width * heightScale;
                var yPos = (NativeView.Frame.Height / 2) - (vidHeight / 2);

                liveCameraStream.Frame = new CGRect(0f, yPos, NativeView.Bounds.Width, vidHeight);
                liveCameraStream.BackgroundColor = UIColor.Clear;
                liveCameraStream.Add(new UILabel(new CGRect(0f, 0f, NativeView.Bounds.Width, 20)) { Text = "The Emulator does not support Camera Usage.", TextColor = UIColor.White });
            }
        }

        public async void AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        public void SetupSession()
        {
            videoPreviewLayer.Session = captureSession;
            videoPreviewLayer.Frame = liveCameraStream.Bounds;
            liveCameraStream.Layer.AddSublayer(videoPreviewLayer);

            var captureDevice = GetBackCamera();
            ConfigureCameraForDevice(captureDevice);
            NSError err;
            videoDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice, out err);
            videoDataOutput = new AVCaptureVideoDataOutput
            {
                AlwaysDiscardsLateVideoFrames = true
            };
            DispatchQueue queue = new DispatchQueue("dbrcameraQueue");
            if (captureSession.CanAddInput(videoDeviceInput))
            {
                captureSession.AddInput(videoDeviceInput);
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var initialVideoOrientation = AVCaptureVideoOrientation.Portrait;
                    var statusBarOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                    if (statusBarOrientation != UIInterfaceOrientation.Unknown)
                    {
                        AVCaptureVideoOrientation videoOrintation;
                        if (Enum.TryParse(statusBarOrientation.ToString(), out videoOrintation))
                        {
                            initialVideoOrientation = videoOrintation;
                        }
                    }
                    videoPreviewLayer.Connection.VideoOrientation = initialVideoOrientation;
                });
            }
            else if (err != null)
            {
                Console.WriteLine($"Could not create video device input: {err}");
                //this.setupResult = SessionSetupResult.ConfigurationFailed;
                this.captureSession.CommitConfiguration();
                return;
            }
            else
            {
                Console.WriteLine("Could not add video device input to the session");
                //this.setupResult = SessionSetupResult.ConfigurationFailed;
                this.captureSession.CommitConfiguration();
                return;
            }

            if (captureSession.CanAddOutput(videoDataOutput))
            {
                captureSession.AddOutput(videoDataOutput);
                captureOutput.reader = reader;
                captureOutput.update = ResetResults;

                videoDataOutput.SetSampleBufferDelegateQueue(captureOutput, queue);
                videoDataOutput.WeakVideoSettings = new NSDictionary<NSString, NSObject>(CVPixelBuffer.PixelFormatTypeKey, NSNumber.FromInt32((int)CVPixelFormatType.CV32BGRA));
            }
            else
            {
                Console.WriteLine("Could not add metadata output to the session");
                //this.setupResult = SessionSetupResult.ConfigurationFailed;
                captureSession.CommitConfiguration();

                return;
            }
            captureSession.CommitConfiguration();
        }

        public AVCaptureDevice GetBackCamera()
        {
            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);

            foreach (var device in devices)
            {
                if (device.Position == AVCaptureDevicePosition.Back)
                {
                    return device;
                }
            }

            return null;
        }

        public void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.AutoFocusRangeRestrictionSupported)
            {
                device.LockForConfiguration(out error);
                device.AutoFocusRangeRestriction = AVCaptureAutoFocusRangeRestriction.Near;
                device.UnlockForConfiguration();
            }
        }

        private void SetupUserInterface()
        {
            liveCameraStream = new UIView()
            {
                Frame = new CGRect(0f, 0f, NativeView.Bounds.Width, NativeView.Bounds.Height)
            };

            NativeView.Add(liveCameraStream);
        }

        /// <summary>
        /// Starts the Capture Session.
        /// </summary>
        public void StartSession()
        {
            captureSession?.StartRunning();
        }

        /// <summary>
        /// Stops the Capture Session.
        /// </summary>
        public void StopSession()
        {
            captureSession?.StopRunning();
        }

        void ResetResults()
        {
            result = captureOutput.result;
        }

        string ICaptureUI.GetResults()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                result = captureOutput.result;
            });

            return result;
        }

        /// <summary>
        /// Cleanup the Resources.
        /// </summary>
        /// <param name="disposing">Is Disposing</param>
        protected override void Dispose(bool disposing)
        {
            captureSession?.Dispose();
            videoDeviceInput?.Dispose();
            videoDataOutput.Dispose();

            base.Dispose(disposing);
        }

        public void onClickFlash()
        {
            if (!flashOn)
            {
                device.LockForConfiguration(out error);
                device.TorchMode = AVCaptureTorchMode.On;
                device.UnlockForConfiguration();
                flashOn = true;
            }
            else
            {
                device.LockForConfiguration(out error);
                device.TorchMode = AVCaptureTorchMode.Off;
                device.UnlockForConfiguration();
                flashOn = false;
            }
        }
    }
}