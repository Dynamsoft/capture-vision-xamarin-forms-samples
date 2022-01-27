## How to set the license in the DBR Xamarin samples

### iOS:
**trial license:**
When using a long alpha-numeric trial license key, `isinitWithLicenseKey` should be set to false and the reader object should be initialized at the start as such:
```
DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("put your trial license here");
```

**full license for 7.x:**
If using DBR v7.x, the `licenseKey` variable should be set as the short 8-digit full license key. `isinitWithLicenseKey` should be set to true if this is the case.
```csharp
public class CaptureOutput : IDBRServerLicenseVerificationDelegate
{
    ...
    public void InitLicenseKey(){
        //Declare inside the function
        DynamsoftBarcodeReader reader = new DynamsoftBarcodeReader("", "full license here", Self);
    }
    ...
    void IDBRServerLicenseVerificationDelegate.Error(bool isSuccess, NSError error)
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
When using DBR v8.x, `isinitWithLicenseKey` should be set to false so that the `InitLicenseKey` method is triggered. 
```csharp
public class CaptureOutput : IDMLTSLicenseVerificationDelegate
{
    ...
    DynamsoftBarcodeReader reader;
    
    //Declare inside the function
    public void InitLicenseKey() {
        iDMLTSConnectionParameters parameters = new iDMLTSConnectionParameters();
        parameters.HandshakeCode = "******";
        //parameters.OrganizationID = "******"; // This parameter can be used instead of HandshakeCode to set the license when using dbr v8.4 and above.
        //parameters.SessionPassword = "******";

        reader = new DynamsoftBarcodeReader(parameters, Self);
    }
    ...
    void IDMLTSLicenseVerificationDelegate.Error(bool isSuccess, NSError error)
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
