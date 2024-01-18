using System;
using CoreBluetooth;
using CoreFoundation;
using InterviewBle.Abstractions;
using InterviewBle.Enums;
using InterviewBle.iOS.Extensions;
using InterviewBle.Models;

namespace InterviewBle.iOS.Services
{
    public class BleImplementation : BleImplementationBase
    {
        private static string _restorationIdentifier;
        private static bool _showPowerAlert = true;

        private CBCentralManager _centralManager;
        private IBleCentralManagerDelegate _bleCentralManagerDelegate;

        public static void UseRestorationIdentifier(string restorationIdentifier)
        {
            _restorationIdentifier = restorationIdentifier;
        }

        public static void ShowPowerAlert(bool showPowerAlert)
        {
            _showPowerAlert = showPowerAlert;
        }

        protected override void InitializeNative()
        {
            var cmDelegate = new BleCentralManagerDelegate();
            _bleCentralManagerDelegate = cmDelegate;

            var options = CreateInitOptions();

            _centralManager = new CBCentralManager(cmDelegate, DispatchQueue.CurrentQueue, options);
            _bleCentralManagerDelegate.UpdatedState += (s, e) => State = GetState();
        }

        protected override BluetoothState GetInitialStateNative()
        {
            return GetState();
        }

        protected override IBleAdapter CreateNativeAdapter()
        {
            return new Adapter(_centralManager, _bleCentralManagerDelegate);
        }

        private BluetoothState GetState()
        {
            return _centralManager?.State.ToBluetoothState() ?? BluetoothState.Unavailable;
        }

        private CBCentralInitOptions CreateInitOptions()
        {
            return new CBCentralInitOptions
            {
#if __IOS__
                RestoreIdentifier = _restorationIdentifier,
#endif
                ShowPowerAlert = _showPowerAlert
            };
        }
    }
}