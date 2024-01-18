using System;
using Android.Bluetooth;
using InterviewBle.Enums;

namespace InterviewBle.Droid.Helpers
{
    internal static class DeviceBondStateExtension
    {
        public static DeviceBondState FromNative(this Bond bondState)
        {
            switch (bondState)
            {
                case Bond.None:
                    return DeviceBondState.NotBonded;
                case Bond.Bonding:
                    return DeviceBondState.Bonding;
                case Bond.Bonded:
                    return DeviceBondState.Bonded;
                default:
                    return DeviceBondState.NotSupported;
            }
        }

    }
}