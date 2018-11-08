// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace CameraDemo
{
    [Register ("CameraViewController")]
    partial class CameraViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnZoomIn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnZoomOut { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton flashBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel label { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        CameraDemo.PreviewView PreviewView { get; set; }

        [Action ("BtnZoomIn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnZoomIn_TouchUpInside (UIKit.UIButton sender);

        [Action ("BtnZoomOut_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BtnZoomOut_TouchUpInside (UIKit.UIButton sender);

        [Action ("onClickFlahBtn:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void onClickFlahBtn (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnZoomIn != null) {
                btnZoomIn.Dispose ();
                btnZoomIn = null;
            }

            if (btnZoomOut != null) {
                btnZoomOut.Dispose ();
                btnZoomOut = null;
            }

            if (flashBtn != null) {
                flashBtn.Dispose ();
                flashBtn = null;
            }

            if (label != null) {
                label.Dispose ();
                label = null;
            }

            if (PreviewView != null) {
                PreviewView.Dispose ();
                PreviewView = null;
            }
        }
    }
}