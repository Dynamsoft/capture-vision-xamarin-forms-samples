## How to set the license in the DBR Xamarin samples

### iOS:
**trial license:**
```
DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("put your trial license here");
```

**full license for 7.x:**
```
public class CaptureOutput : IDBRServerLicenseVerificationDelegate
{
    ...
    //Declare inside the function
    DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("", "put your purchased license here", Self);
    ...
    public void Error(bool isSuccess, NSError error)
    {
        if (isSuccess)
        {
            Console.WriteLine("success");
        }
        else {
            Console.WriteLine("error = " + error.UserInfo);
        }
    }
}
```

**full license for 8.x:**
```
public class CaptureOutput : IDMLTSLicenseVerificationDelegate
{
    ...
    DynamsoftBarcodeReader reader;
    
    //Declare inside the function
    public void initLicense() {
        iDMLTSConnectionParameters parameters = new iDMLTSConnectionParameters();
        parameters.organizationID  = "******";
        //parameters.handshakeCode = "******";
        reader = new DynamsoftBarcodeReader(parameters, Self);
    }
    
    ...
    public void Error(bool isSuccess, NSError error)
    {
        if (isSuccess)
        {
            Console.WriteLine("success");
        }
        else {
            Console.WriteLine("error : " + error.UserInfo);
        }
    }
}
```

### Android: 

**trial license:**
```
BarcodeReader reader = new BarcodeReader("put your trial license here");
```

**full license for 7.x:**
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
**full license for 8.x:**
```
public class MainActivity: IDBRLTSLicenseVerificationListener
{
    ...
    ...
    protected override void OnCreate(Bundle savedInstanceState)	
    {
        base.OnCreate(savedInstanceState);

        BarcodeReader reader = new BarcodeReader("");
        MainActivity main = new MainActivity();
        DMLTSConnectionParameters info = new DMLTSConnectionParameters();
        info.organizationID = "********";
        //info.handshakeCode = "******";
        reader.InitLicenseFromLTS(info, main);
    }
    
    ...
    void IDBRLTSLicenseVerificationListener.LTSLicenseVerificationCallback(bool p0, Java.Lang.Exception exception)
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
