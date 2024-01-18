using System;
using Android.Bluetooth;

namespace InterviewBle.Droid.CallbackEventArgs
{
    public class CharacteristicReadCallbackEventArgs
    {
        public BluetoothGattCharacteristic Characteristic { get; }
        public GattStatus Status { get; }

        public CharacteristicReadCallbackEventArgs(BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            Characteristic = characteristic;
            Status = status;
        }
    }
}
