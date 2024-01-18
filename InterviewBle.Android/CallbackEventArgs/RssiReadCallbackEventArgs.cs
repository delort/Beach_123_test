﻿using System;
using Android.Bluetooth;

namespace InterviewBle.Droid.CallbackEventArgs
{
    public class RssiReadCallbackEventArgs : EventArgs
    {
        public Exception Error { get; }
        public int Rssi { get; }

        public RssiReadCallbackEventArgs(Exception error, int rssi)
        {
            Error = error;
            Rssi = rssi;
        }
    }
}