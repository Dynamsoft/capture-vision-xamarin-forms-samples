## How to set the license in the DBR Xamarin samples

### iOS:
**trial license:**
```
DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("put your trial license here");
```

**full license:**
```
public class CaptureUI : IDBRServerLicenseVerificationDelegate
{
    ...
    ...
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
