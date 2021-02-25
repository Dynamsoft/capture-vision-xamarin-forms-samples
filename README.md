# Xamarin Barcode Reader SDK

Version 8.1.2

The repository aims to help developers build **Android** and **iOS** barcode apps with Dynamsoft Xamarin Barcode Reader SDK.

## License
Get the [trial license](https://www.dynamsoft.com/CustomerPortal/Portal/Triallicense.aspx).

## Contact Us
<support@dynamsoft.com>

## Environment
* Visual Studio 2017 or later

## Installation

- [Xamarin.Dynamsoft.Barcode.Android](https://www.nuget.org/packages/Xamarin.Dynamsoft.Barcode.Android/)

    ```
    PM> Install-Package Xamarin.Dynamsoft.Barcode.Android -Version 8.1.2
    ```

- [Xamarin.Dynamsoft.Barcode.iOS](https://www.nuget.org/packages/Xamarin.Dynamsoft.Barcode.iOS/)

    ```
    PM> Install-Package Xamarin.Dynamsoft.Barcode.iOS -Version 8.1.2
    ```

## HowTo
1. Import examples to **Visual Studio**.
2. Install the barcode SDK via **Package Manager**.
3. Build projects in release mode.

## Examples
- examples
    - Android
        - [helloworld](https://github.com/dynamsoft-dbr/xamarin/tree/master/examples/Android/helloworld)
        - [camera](https://github.com/dynamsoft-dbr/xamarin/tree/master/examples/Android/camera)
    - iOS
        - [helloworld](https://github.com/dynamsoft-dbr/xamarin/tree/master/examples/iOS/helloworld)
        - [camera](https://github.com/dynamsoft-dbr/xamarin/tree/master/examples/iOS/camera)
    - XamarinForms (Xamarin.Forms sample)
        - [DBRXFSSample](https://github.com/dynamsoft-dbr/xamarin/tree/master/examples/XamarinForms/DBRXFSample)
        
## Note
It came to our notice that when using Dynamsoft Barcode Reader SDK 7.0 version, the build of the application would fail. Such an issue could result from a bug in Xamarin platform, which could be viewed via here, https://developercommunity.visualstudio.com/content/problem/595665/systemnotsupportedexception-exception-occurs-when.html.

To solve such an issue, a new version, Commercial Xamarin Android 9.4(d16-2) Preview for Windows+Visual Studio 2019 Preview, has been published by Microsoft, which can be downloaded via here, https://github.com/xamarin/xamarin-android. The new version is compatible with VS2017 and VS2019, but there could be an instability issue using VS2017, so VS2019 is recommended. With the fixed version installed, the build would succeed.
