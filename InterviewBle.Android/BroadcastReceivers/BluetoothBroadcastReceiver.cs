using System;
using Android.Bluetooth;
using Android.Content;
using InterviewBle.Droid.Helpers;
using InterviewBle.Enums;

namespace InterviewBle.Droid.BroadcastReceivers
{
    public class BluetoothBroadcastReceiver : BroadcastReceiver
    {
        private readonly Action<BluetoothState> _stateChanged;

        public BluetoothBroadcastReceiver(Action<BluetoothState> stateChangedHandler)
        {
            _stateChanged = stateChangedHandler;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (action != BluetoothAdapter.ActionStateChanged)
                return;

            var state = intent.GetIntExtra(BluetoothAdapter.ExtraState, -1);

            if (state == -1)
            {
                _stateChanged?.Invoke(BluetoothState.Unknown);
                return;
            }

            var btState = (State)state;
            _stateChanged?.Invoke(btState.ToBluetoothState());
        }
    }
}