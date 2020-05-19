## How to use Xamarin demo

**Purchased license:**

### iOS : add `IDBRServerLicenseVerificationDelegate` interface, and Implement it.

```
public class CaptureUI : IDBRServerLicenseVerificationDelegate
{
    ...
    ...
    DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("", "replace your license here", Self);
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

### Android : add `IDBRServerLicenseVerificationListener` interface, and Implement it.

```
public class MainActivity: IDBRServerLicenseVerificationListener
{
    ...
    ...
    BarcodeReader reader = new BarcodeReader("");
    protected override void OnCreate(Bundle savedInstanceState)	
    {
        base.OnCreate(savedInstanceState);

        reader = new BarcodeReader("");
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
