using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using AVFoundation;
using CoreFoundation;
using DBRiOS;
using CoreVideo;
using CoreMedia;

namespace DBRXFSample.iOS.CaptureView
{
    class CaptureOutput : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        public DynamsoftBarcodeReader reader;
        public Action update;
        private bool ready = true;
        private DispatchQueue queue = new DispatchQueue("ReadTask", true);
        private NSError errorr;
        private nint bpr;
        private nint width;
        private nint height;
        private NSData buffer;
        public string result = "";
        private iTextResult[] results;

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            if (ready)
            {
                ready = false;
                CVPixelBuffer cVPixelBuffer = (CVPixelBuffer)sampleBuffer.GetImageBuffer();

                cVPixelBuffer.Lock(CVPixelBufferLock.ReadOnly);
                nint dataSize = cVPixelBuffer.DataSize;
                width = cVPixelBuffer.Width;
                height = cVPixelBuffer.Height;
                IntPtr baseAddress = cVPixelBuffer.BaseAddress;
                bpr = cVPixelBuffer.BytesPerRow;
                cVPixelBuffer.Unlock(CVPixelBufferLock.ReadOnly);
                buffer = NSData.FromBytes(baseAddress, (nuint)dataSize);
                cVPixelBuffer.Dispose();
                queue.DispatchAsync(ReadTask);
            }
            sampleBuffer.Dispose();
        }

        private void ReadTask()
        {
            
            results = reader.DecodeBuffer(buffer,
                                        width,
                                        height,
                                        bpr,
                                        EnumImagePixelFormat.Argb8888,
                                        "", out errorr);
                                        
            if (results != null && results.Length > 0)
            {
                for (int i = 0; i < results.Length; i++)
                {
                    if (i == 0)
                        result = "Code[1]: " + results[0].BarcodeText;
                    else
                        result = result + "\n\n" + "Code[" + (i + 1) + "]: " + results[i].BarcodeText;
                }
            }
            else
            {
                result = "";
            }
            
            DispatchQueue.MainQueue.DispatchAsync(update);
            ready = true;
        }
    }
}