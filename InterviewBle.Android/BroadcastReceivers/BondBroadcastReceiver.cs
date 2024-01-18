using System;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using InterviewBle.Droid.Helpers;
using InterviewBle.Droid.Services;
using InterviewBle.Events;

namespace InterviewBle.Droid.BroadcastReceivers
{
    public class BondBroadcastReceiver : BroadcastReceiver
    {
        private readonly BleAdapter _broadcastAdapter;

        public event EventHandler<DeviceBondStateChangedEventArgs> BondStateChanged;

        public BondBroadcastReceiver(BleAdapter adapter)
        {
            _broadcastAdapter = adapter;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (BondStateChanged == null)
            {
                return;
            }

            var extraBondState = (Bond)intent.GetIntExtra(BluetoothDevice.ExtraBondState, (int)Bond.None);

            BluetoothDevice bluetoothDevice;

#if NET6_0_OR_GREATER
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
#else
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
#endif
            {
                bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice, Java.Lang.Class.FromType(typeof(BluetoothDevice)));
            }
            else
            {
                bluetoothDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
            }

            var device = new BleDevice(_broadcastAdapter, bluetoothDevice, null);

            var address = bluetoothDevice?.Address ?? string.Empty;

            var bondState = extraBondState.FromNative();
            BondStateChanged(this, new DeviceBondStateChangedEventArgs() { Address = address, Device = device, State = bondState });
        }
    }
}
