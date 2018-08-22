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
        UIKit.UIImageView qrimage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Read { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton Reset { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel text { get; set; }

        [Action ("onReadBtnClick:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void onReadBtnClick (UIKit.UIButton sender);

        [Action ("onResetBtnClick:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void onResetBtnClick (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (qrimage != null) {
                qrimage.Dispose ();
                qrimage = null;
            }

            if (Read != null) {
                Read.Dispose ();
                Read = null;
            }

            if (Reset != null) {
                Reset.Dispose ();
                Reset = null;
            }

            if (text != null) {
                text.Dispose ();
                text = null;
            }
        }
    }
}