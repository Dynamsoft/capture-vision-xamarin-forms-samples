
namespace CameraDemo
{
    using AVFoundation;
    using CoreFoundation;
    using CoreGraphics;
    using CoreVideo;
    using Foundation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UIKit;

    public partial class CameraViewController : UIViewController
    {
        //private readonly AVCaptureMetadataOutput metadataOutput = new AVCaptureMetadataOutput();
        private readonly AVCaptureVideoDataOutput videoOutput = new AVCaptureVideoDataOutput();

        private FrameExtractor frameExtractor = new FrameExtractor();

        NSNotificationCenter defaultCenter = new NSNotificationCenter();

        /// <summary>
        /// Communicate with the session and other session objects on this queue.
        /// </summary>
        private readonly DispatchQueue sessionQueue = new DispatchQueue("session queue");

        private readonly AVCaptureSession session = new AVCaptureSession();

        private SessionSetupResult setupResult = SessionSetupResult.Success;

        private AVCaptureDeviceInput videoDeviceInput;

        private AVCaptureDevice device = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);

        private bool isSessionRunning;

        private NSError error;

        private bool flashOn = false;

        // KVO and Notifications
        

        public CameraViewController(IntPtr handle) : base(handle) { }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            

            // Disable UI. The UI is enabled if and only if the session starts running.

            // Add the open barcode gesture recognizer to the region of interest view.

            // Set up the video preview view.
            this.PreviewView.Session = session;

            // Check video authorization status. Video access is required and audio
            // access is optional. If audio access is denied, audio is not recorded
            // during movie recording.
            switch (AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video))
            {
                case AVAuthorizationStatus.Authorized:
                    // The user has previously granted access to the camera.
                    break;

                case AVAuthorizationStatus.NotDetermined:
                    // The user has not yet been presented with the option to grant
                    // video access. We suspend the session queue to delay session
                    // setup until the access request has completed.
                    this.sessionQueue.Suspend();
                    AVCaptureDevice.RequestAccessForMediaType(AVMediaType.Video, (granted) =>
                    {
                        if (!granted)
                        {
                            this.setupResult = SessionSetupResult.NotAuthorized;
                        }

                        this.sessionQueue.Resume();
                    });
                    break;

                default:
                    // The user has previously denied access.
                    this.setupResult = SessionSetupResult.NotAuthorized;
                    break;
            }
            
            // Setup the capture session.
            // In general it is not safe to mutate an AVCaptureSession or any of its
            // inputs, outputs, or connections from multiple threads at the same time.
            //
            // Why not do all of this on the main queue?
            // Because AVCaptureSession.StartRunning() is a blocking call which can
            // take a long time. We dispatch session setup to the sessionQueue so
            // that the main queue isn't blocked, which keeps the UI responsive.
            this.sessionQueue.DispatchAsync(this.ConfigureSession);
        }

        partial void onClickFlahBtn(UIButton sender)
        {
            if(!flashOn)
            {
                device.LockForConfiguration(out error);
                device.TorchMode = AVCaptureTorchMode.On;
                device.UnlockForConfiguration();
                flashBtn.SetImage(UIImage.FromBundle("flashoff"),UIControlState.Normal);
                flashOn = true;
            }
            else
            {
                device.LockForConfiguration(out error);
                device.TorchMode = AVCaptureTorchMode.Off;
                device.UnlockForConfiguration();
                flashBtn.SetImage(UIImage.FromBundle("flashon"), UIControlState.Normal);
                flashOn = false;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.sessionQueue.DispatchAsync(() =>
            {
                switch (this.setupResult)
                {
                    case SessionSetupResult.Success:
                        // Only setup observers and start the session running if setup succeeded.
                        this.AddObservers();
                        this.session.StartRunning();
                        this.isSessionRunning = session.Running;

                        break;

                    case SessionSetupResult.NotAuthorized:
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            var message = "CameraDemo doesn't have permission to use the camera, please change privacy settings";
                            var alertController = UIAlertController.Create("CameraDemo", message, UIAlertControllerStyle.Alert);
                            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
                            alertController.AddAction(UIAlertAction.Create("Settings", UIAlertActionStyle.Default, action =>
                            {
                                UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
                            }));

                            this.PresentViewController(alertController, true, null);
                        });
                        break;

                    case SessionSetupResult.ConfigurationFailed:
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            var message = "Unable to capture media";
                            var alertController = UIAlertController.Create("CameraDemo", message, UIAlertControllerStyle.Alert);
                            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

                            this.PresentViewController(alertController, true, null);
                        });
                        break;
                }
            });
        }

        public override void ViewWillDisappear(bool animated)
        {
            this.sessionQueue.DispatchAsync(() =>
            {
                if (this.setupResult == SessionSetupResult.Success)
                {
                    this.session.StopRunning();
                    this.isSessionRunning = this.session.Running;
                    this.RemoveObservers();
                }
            });

            base.ViewWillDisappear(animated);
        }
        
        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            var videoPreviewLayerConnection = this.PreviewView.VideoPreviewLayer.Connection;
            if (videoPreviewLayerConnection != null)
            {
                var deviceOrientation = UIDevice.CurrentDevice.Orientation;
                if (deviceOrientation.IsPortrait() || deviceOrientation.IsLandscape())
                {
                    var newVideoOrientation = this.ConvertOrientation(deviceOrientation);
                    videoPreviewLayerConnection.VideoOrientation = newVideoOrientation;
                }
            }
        }

        private AVCaptureVideoOrientation ConvertOrientation(UIDeviceOrientation deviceOrientation)
        {
            var result = default(AVCaptureVideoOrientation);
            switch (deviceOrientation)
            {
                case UIDeviceOrientation.Portrait:
                    result = AVCaptureVideoOrientation.Portrait;
                    break;
                case UIDeviceOrientation.PortraitUpsideDown:
                    result = AVCaptureVideoOrientation.PortraitUpsideDown;
                    break;
                // TODO: change to logical naming after it will be fixed (map `LandscapeLeft` to `LandscapeLeft`)
                case UIDeviceOrientation.LandscapeLeft:
                    result = AVCaptureVideoOrientation.LandscapeRight;
                    break;
                case UIDeviceOrientation.LandscapeRight:
                    result = AVCaptureVideoOrientation.LandscapeLeft;
                    break;
                default:
                    throw new InvalidProgramException();
            }

            return result;
        }

        #region Session Management
        
        private readonly DispatchQueue videoDataObjectsQueue = new DispatchQueue("Video data objects queue");
        DispatchQueue sampleBufferQueue = new DispatchQueue("SampleBufferQueue");

        private void ConfigureSession()
        {
            if (setupResult == SessionSetupResult.Success)
            {
                this.session.BeginConfiguration();

                // Add video input
                // Choose the back wide angle camera if available, otherwise default to the front wide angle camera
                AVCaptureDevice defaultVideoDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Back) ??
                                                     AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaType.Video, AVCaptureDevicePosition.Front) ??
                                                     null;

                if (defaultVideoDevice == null)
                {
                    Console.WriteLine("Could not get video device");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();
                    return;
                }

                NSError error;
                var videoDeviceInput = AVCaptureDeviceInput.FromDevice(defaultVideoDevice, out error);
                if (this.session.CanAddInput(videoDeviceInput))
                {
                    this.session.AddInput(videoDeviceInput);
                    this.videoDeviceInput = videoDeviceInput;

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        // Why are we dispatching this to the main queue?
                        // Because AVCaptureVideoPreviewLayer is the backing layer for PreviewView and UIView
                        // can only be manipulated on the main thread
                        // Note: As an exception to the above rule, it's not necessary to serialize video orientation changed
                        // on the AVCaptureVideoPreviewLayer's connection with other session manipulation
                        //
                        // Use the status bar orientation as the internal video orientation. Subsequent orientation changes are
                        // handled by CameraViewController.ViewWillTransition(to:with:).

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

                        this.PreviewView.VideoPreviewLayer.Connection.VideoOrientation = initialVideoOrientation;
                    });
                }
                else if (error != null)
                {
                    Console.WriteLine($"Could not create video device input: {error}");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();
                    return;
                }
                else
                {
                    Console.WriteLine("Could not add video device input to the session");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();

                    return;
                }
                
                if (this.session.CanAddOutput(videoOutput))
                {
                    this.session.AddOutput(videoOutput);

                    frameExtractor.update = ResetResult;

                    videoOutput.SetSampleBufferDelegateQueue(frameExtractor, sampleBufferQueue);
                    videoOutput.WeakVideoSettings = new NSDictionary<NSString, NSObject>(CVPixelBuffer.PixelFormatTypeKey, NSNumber.FromInt32((int)CVPixelFormatType.CV32BGRA));

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        //var initialRegionOfInterest = this.PreviewView.VideoPreviewLayer.MapToLayerCoordinates(initialRectOfInterest);
                    });
                }
                else
                {
                    Console.WriteLine("Could not add metadata output to the session");
                    this.setupResult = SessionSetupResult.ConfigurationFailed;
                    this.session.CommitConfiguration();

                    return;
                }

                this.session.CommitConfiguration();
            }
        }

        void ResetResult()
        {
            label.Text = frameExtractor.result;
        }

        #endregion

        #region Presets
        
        private NSString[] AvailableSessionPresets()
        {
            return GetAllSessionPresets().Where(preset => session.CanSetSessionPreset(preset)).ToArray();
        }

        private static IEnumerable<NSString> GetAllSessionPresets()
        {
            yield return AVCaptureSession.PresetPhoto;
            yield return AVCaptureSession.PresetLow;
            yield return AVCaptureSession.PresetMedium;
            yield return AVCaptureSession.PresetHigh;
            yield return AVCaptureSession.Preset352x288;
            yield return AVCaptureSession.Preset640x480;
            yield return AVCaptureSession.Preset1280x720;
            yield return AVCaptureSession.PresetiFrame960x540;
            yield return AVCaptureSession.PresetiFrame1280x720;
            yield return AVCaptureSession.Preset1920x1080;
            yield return AVCaptureSession.Preset3840x2160;
        }

        #endregion

        #region KVO and Notifications

        private NSObject interruptionEndedNotificationToken;

        private NSObject wasInterruptedNotificationToken;

        private NSObject runtimeErrorNotificationToken;

        private IDisposable runningChangeToken;

        private void AddObservers()
        {
            this.runningChangeToken = this.session.AddObserver("running", NSKeyValueObservingOptions.New, this.OnRunningChanged);

            // Observe the previewView's regionOfInterest to update the AVCaptureMetadataOutput's
            // RectOfInterest when the user finishes resizing the region of interest.

            var notificationCenter = NSNotificationCenter.DefaultCenter;

            this.runtimeErrorNotificationToken = notificationCenter.AddObserver(AVCaptureSession.RuntimeErrorNotification, this.OnRuntimeErrorNotification, this.session);

            // A session can only run when the app is full screen. It will be interrupted
            // in a multi-app layout, introduced in iOS 9, see also the documentation of
            // AVCaptureSessionInterruptionReason.Add observers to handle these session
            // interruptions and show a preview is paused message.See the documentation
            // of AVCaptureSessionWasInterruptedNotification for other interruption reasons.
            this.wasInterruptedNotificationToken = notificationCenter.AddObserver(AVCaptureSession.WasInterruptedNotification, this.OnSessionWasInterrupted, this.session);
            this.interruptionEndedNotificationToken = notificationCenter.AddObserver(AVCaptureSession.InterruptionEndedNotification, this.OnSessionInterruptionEnded, this.session);
        }

        private void RemoveObservers()
        {
            this.runningChangeToken?.Dispose();
            this.runtimeErrorNotificationToken?.Dispose();
            this.wasInterruptedNotificationToken?.Dispose();
            this.interruptionEndedNotificationToken?.Dispose();
        }
        

        private void OnRunningChanged(NSObservedChange change)
        {
            var isSessionRunning = ((NSNumber)change.NewValue).BoolValue;

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {

                // After the session stop running, remove the metadata object overlays,
                // if any, so that if the view appears again, the previously displayed
                // metadata object overlays are removed.
                if (!isSessionRunning)
                {
                    
                }

                // When the session starts running, the aspect ration of the video preview may also change if a new session present was applied .
                // To keep the preview view's region of interest within the visible portion of the video preview, the preview view's region of 
                // interest will need to be updates.
                if (isSessionRunning)
                {
                }
            });
        }

        private void OnRuntimeErrorNotification(NSNotification notification)
        {
            var args = new AVCaptureSessionRuntimeErrorEventArgs(notification);
            if (args.Error != null)
            {
                var error = (AVError)(long)args.Error.Code;
                Console.WriteLine($"Capture session runtime error: {error}");

                // Automatically try to restart the session running if media services were
                // reset and the last start running succeeded. Otherwise, enable the user
                // to try to resume the session running.

                if (error == AVError.MediaServicesWereReset)
                {
                    this.sessionQueue.DispatchAsync(() =>
                    {
                        if (this.isSessionRunning)
                        {
                            this.session.StartRunning();
                            this.isSessionRunning = session.Running;
                        }
                    });
                }
            }
        }

        private void OnSessionWasInterrupted(NSNotification notification)
        {
            // In some scenarios we want to enable the user to resume the session running.
            // For example, if music playback is initiated via control center while
            // using AVMetadataRecordPlay, then the user can let AVMetadataRecordPlay resume
            // the session running, which will stop music playback. Note that stopping
            // music playback in control center will not automatically resume the session
            // running. Also note that it is not always possible to resume

            var reasonIntegerValue = ((NSNumber)notification.UserInfo[AVCaptureSession.InterruptionReasonKey]).Int32Value;
            var reason = (AVCaptureSessionInterruptionReason)reasonIntegerValue;
            Console.WriteLine($"Capture session was interrupted with reason {reason}");

           
        }

        private void OnSessionInterruptionEnded(NSNotification notification)
        {
            Console.WriteLine("Capture session interruption ended");
            
        }

        #endregion
        
    }
}