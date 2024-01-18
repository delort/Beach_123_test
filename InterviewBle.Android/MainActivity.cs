﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.Collections.Generic;
using Xamarin.Essentials;
using InterviewBle.Droid.Services;

namespace InterviewBle.Droid
{
    [Activity(Label = "InterviewBle", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var adapter = AndroidBluetoothLE.Current.Adapter;
            LoadApplication(new App(adapter));
            await Permissions.RequestAsync<BLEPermission>();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        // Function below is required to ask for permission to use Bluetooth
        public class BLEPermission : Xamarin.Essentials.Permissions.BasePlatformPermission
        {
            public override (string androidPermission, bool isRuntime)[] RequiredPermissions
            {
                get
                {
                    var permissions = new List<(string androidPermission, bool isRuntime)>();

                    if (Android.OS.Build.VERSION.SdkInt <= Android.OS.BuildVersionCodes.R)
                    {
                        permissions.Add((Android.Manifest.Permission.Bluetooth, true));
                        permissions.Add((Android.Manifest.Permission.BluetoothAdmin, true));
                        permissions.Add((Android.Manifest.Permission.AccessFineLocation, true));
                        permissions.Add((Android.Manifest.Permission.AccessCoarseLocation, true));
                    }
                    else
                    {
                        permissions.Add((Android.Manifest.Permission.BluetoothConnect, true));
                        permissions.Add((Android.Manifest.Permission.BluetoothScan, true));
                    }

                    return permissions.ToArray();
                }
            }
        }
    }
}
