## How to set the license in the DBR Xamarin samples

### iOS:

**DynamsoftBarcodeCamera:**

```
// Set the file path of the sound if you would like to have a beeping sound when a barcode is scanned successfully.
camera = new DynamsoftBarcodeCamera(captureView, "pi.wav")
{
    //true: open camera to get video frames
    //false: close camera
    IsEnable = true
};
camera.StartScanning();
camera.SetResolution(Resolution.Resolution1080P);
// Bind DynamsoftBarcodeReader instance
camera.BindReader(reader);
 
// Set results callback
camera.AddDecodeListener(Self);
camera.setEnableBeepSound(true);
// Set the intervals (in seconds) to filter duplicate barcodes. 
// The value of setDuplicateBarcodesFilter is an integer and must be greater than 0. If it is set to 10, then no duplicate barcodes will be returned in 10 seconds.
camera.setDuplicateBarcodesFilter(10);

// Set the amount of time to wait before decoding the next frame. The allowed values are (int)[0, 5].
// t = 0: Continuous decoding with no interval
// t = [1, 5]: Wait (t)s after a frame is successfully decoded and before executing the next decoding function.
camera.setContinuousScan(0);

 
...

//barcodes results callback
public void barcodeReader(ReaderPackage reader, FramePackage frame)
{
    //reader.error: DynamsoftBarcodeReader errors
    //FramePackage: information about the decoded frame, including the buffer, Width, Height, Stride, PixelFormat and FrameID.
    if (reader.barcodeResults.Length > 0)
    {
        textResults = "Value: " + reader.barcodeResults[0].BarcodeText;
    }
}

```

**trial license:**
```
DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("put your trial license here");
```

**full license:**
```
public class CaptureOutput : IDBRServerLicenseVerificationDelegate
{
    ...
    //Declare inside the function
    DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("", "put your purchased license here", Self);
    ...
    void IDBRServerLicenseVerificationDelegate.Error(bool isSuccess, NSError error)
    {
        if (isSuccess)
        {
            Console.WriteLine("success");
        }
        else {
            Console.WriteLine("error = " + error);
        }
    }
}
```

### Android: 

```
camera = new Com.Dynamsoft.Camera.Entity.Camera(this);
camera.SetBarcodeReader(barcodeReader); //bind barcodereader and camera
camera.AddCameraView(cameraView);       //bind view and camera
camera.SetEnableBeepSound(true);        //enable beepsound
cameraView.AddOverlay();                //enable overlay
camera.AddResultListener(this);         //set results listener

// Set the intervals (in seconds) to filter duplicate barcodes. 
// The value of setDuplicateBarcodesFilter is an integer and must be greater than 0. If it is set to 10, then no duplicate barcodes will be returned in 10 seconds.
camera.setDuplicateBarcodesFilter(10);

// Set the amount of time to wait before decoding the next frame. The allowed values are (int)[0, 5].
// t = 0: Continuous decoding with no interval
// t = [1, 5]: Wait (t)s after a frame is successfully decoded and before executing the next decoding function.
camera.setContinuousScan(0);

...

void IResultListener.OnGetResult(TextResult[] p0, Frame p1)
{
    RunOnUiThread(() =>
    {
        resultStr = p0[0].BarcodeText;
        tvResult.Text = p0[0].BarcodeText;
    });
}
```

**trial license:**
```
BarcodeReader reader = new BarcodeReader("put your trial license here");
```

**full license:**
```
public class MainActivity: IDBRServerLicenseVerificationListener
{
    ...
    ...
    protected override void OnCreate(Bundle savedInstanceState)	
    {
        base.OnCreate(savedInstanceState);

        BarcodeReader reader = new BarcodeReader("");
        MainActivity main = new MainActivity();
        reader.InitLicenseFromServer("","replace your license key here",main);
    }
    
    ...
    public void LicenseVerificationCallback(bool p0, Exception p1)
    {
        if (p0)
        {
            Console.WriteLine("success");
        }
        else {
            Console.WriteLine("error = " + p1);
        }
    }
}

```
