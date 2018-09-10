
namespace CameraDemo
{
    using AVFoundation;
    using CoreAnimation;
    using CoreGraphics;
    using Foundation;
    using ObjCRuntime;
    using System;
    using UIKit;

    [Register("PreviewView")]
    public class PreviewView : UIView, IUIGestureRecognizerDelegate
    {
        private static readonly object layerClassLock = new object();

        private static Class layerClass;
        
        public PreviewView(CGRect frame) : base(frame)
        {
            
        }

        [Export("initWithCoder:")]
        public PreviewView(NSCoder coder) : base(coder)
        {
            
        }

        public static Class LayerClass
        {
            [Export("layerClass")]
            get
            {
                if (layerClass == null)
                {
                    lock (layerClassLock)
                    {
                        if (layerClass == null)
                        {
                            layerClass = new Class(typeof(AVCaptureVideoPreviewLayer));
                        }
                    }
                }

                return layerClass;
            }
        }
        
        // AV capture properties

        public AVCaptureVideoPreviewLayer VideoPreviewLayer => this.Layer as AVCaptureVideoPreviewLayer;

        public AVCaptureSession Session
        {
            get
            {
                return this.VideoPreviewLayer.Session;
            }

            set
            {
                this.VideoPreviewLayer.Session = value;
            }
        }
    }
}