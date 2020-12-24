## How to set the license in the DBR Xamarin samples

### iOS:

**DynamsoftBarcodeCamera:**

```
//if you need to beepsound while decoding, add file path here 
camera = new DynamsoftBarcodeCamera(captureView, "pi.wav")
{
    //true: enable camera gets frames
    //false: disable camera gets frames
    IsEnable = true
};
camera.setEnableBeepSound(true);

//set an interval > 0, the unit is seconds
//During this time, there will be no duplicate barcodes
camera.setDuplicateBarcocdesFilter(1);

//set an interval (int)[0, 5]
//t = 0: Continuous
//t = [1, 5]: wait (t)s after each decode is completed
camera.setContinuousScan(0);
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
