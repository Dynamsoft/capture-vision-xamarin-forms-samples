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

**full license for 8.x and higher versions:**
When using DBR v8.x or higher versions, `isinitWithLicenseKey` should be set to false so that the `InitLicenseKey` method is triggered. 
```csharp
public class CaptureOutput : IDMDLSLicenseVerificationDelegate
{
    ...
    DynamsoftBarcodeReader reader;
    
    //Declare inside the function
    public void InitLicenseKey() {
        iDMDLSConnectionParameters parameters = new iDMDLSConnectionParameters();
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

> Note: In DBR 9, please note that the `iDMDLSConnectionParameters`, `IDMDLSLicenseVerificationDelegate`, `IDBRServerLicenseVerificationDelegate`, `initLicenseFromServer`, and `initLicenseFromDLS` are all officially deprecated but they should still be used in the Xamarin implmentation as the new `initLicense` method is still not included in the Xamarin package. (DBR 9 release notes: [iOS](https://www.dynamsoft.com/barcode-reader/programming/objectivec-swift/release-notes/ios-9.html?ver=latest) / [Android](https://www.dynamsoft.com/barcode-reader/programming/android/release-notes/android-9.html?ver=latest#900-03222022))

### Android: 

**trial license:**
```java
BarcodeReader reader = new BarcodeReader("put your trial license here");
```

**full license for 7.x:**
```java
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
**full license for 8.x and higher versions:**
```java
public class MainActivity: IDBRDLSLicenseVerificationListener
{
    ...
    ...
    protected override void OnCreate(Bundle savedInstanceState)	
    {
        base.OnCreate(savedInstanceState);

        BarcodeReader reader = new BarcodeReader("");
        MainActivity main = new MainActivity();
        DMLTSConnectionParameters info = new DMDLSConnectionParameters();
        info.handshakeCode = "******";
        //info.organizationID = "********";
        reader.InitLicenseFromLTS(info, main);
    }
    
    ...
    void IDBRDLSLicenseVerificationListener.DLSLicenseVerificationCallback(bool p0, Java.Lang.Exception exception)
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
