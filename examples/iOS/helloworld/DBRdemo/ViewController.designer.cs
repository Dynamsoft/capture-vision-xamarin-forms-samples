// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace DBRdemo
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel label { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView qrimage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton readBtn { get; set; }

        [Action ("OnReadBtnClick:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnReadBtnClick (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (label != null) {
                label.Dispose ();
                label = null;
            }

            if (qrimage != null) {
                qrimage.Dispose ();
                qrimage = null;
            }

            if (readBtn != null) {
                readBtn.Dispose ();
                readBtn = null;
            }
        }
    }
}