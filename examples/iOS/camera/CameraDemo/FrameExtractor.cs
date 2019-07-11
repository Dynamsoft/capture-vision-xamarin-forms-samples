using System;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using DBRiOS;
using Foundation;
using UIKit;

namespace CameraDemo
{
    class FrameExtractor:AVCaptureVideoDataOutputSampleBufferDelegate
    {
        public Action update;
        public string result;
        private DynamsoftBarcodeReader barcodeReader = new DynamsoftBarcodeReader("t0068MgAAABlOih7jbq10NbpqRczC1hLLu/qZsLWkVW0KBu7u/wruw4zcQXlnIMAWCZ/5cnA/JTRvQ4h+syYnev0wYNWAfX8=");
        private iTextResult[] results;
        private NSError error;
        private bool ready = true;
        private DispatchQueue queue = new DispatchQueue("ReadTask",true);
        private NSData data;
        private nint width;
        private nint height;
        private nint bytesPerRow;
        private CVPixelBuffer pixelBuffer;
        private UIImage uiImage;
        private CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
        private CGBitmapContext context;
        private CGImage cgImage;

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            if (ready)
            {
                ready = false;
                pixelBuffer = (CVPixelBuffer)sampleBuffer.GetImageBuffer();
                pixelBuffer.Lock(CVPixelBufferLock.None);

                width = pixelBuffer.Width;
                height = pixelBuffer.Height;
                bytesPerRow = pixelBuffer.BytesPerRow;
                
                context = new CGBitmapContext(pixelBuffer.BaseAddress, width, height, 8, bytesPerRow, colorSpace, CGImageAlphaInfo.PremultipliedFirst);
                cgImage = context.ToImage();
                uiImage = new UIImage(cgImage);
                pixelBuffer.Unlock(CVPixelBufferLock.None);
                pixelBuffer.Dispose();
                queue.DispatchAsync(ReadTask);
            }
            sampleBuffer.Dispose();
        }

        private void ReadTask()
        {
            results = barcodeReader.DecodeImage(uiImage,"",out error);
            if (results != null && results.Length > 0)
            {
                for(int i =0;i<results.Length;i++)
                {
                    if(i == 0)
                        result = "Code[1]: " + results[0].BarcodeText;
                    else
                        result = result + "\n\n" + "Code[" + (i + 1) + "]: " + results[i].BarcodeText;

                }
                //Console.WriteLine(results[0].BarcodeText);
            }
            else
            {
                result = "";
            }
            DispatchQueue.MainQueue.DispatchAsync(update);
            context.Dispose();
            cgImage.Dispose();
            uiImage.Dispose();
            ready = true;
        }
    }
}
