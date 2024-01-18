﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Java.Util;
using InterviewBle.Droid.BroadcastReceivers;
using InterviewBle.Models;
using InterviewBle.Enums;
using Trace = InterviewBle.Helpers.Trace;
using InterviewBle.Droid.Helpers;
using InterviewBle.Abstractions;

namespace InterviewBle.Droid.Services
{
    public class BleAdapter : AdapterBase
    {
        private readonly BluetoothManager _bluetoothManager;
        private readonly BluetoothAdapter _bluetoothAdapter;
        //private readonly Api18BleScanCallback _api18ScanCallback;
        private readonly Api21BleScanCallback _api21ScanCallback;

        private readonly Dictionary<string, TaskCompletionSource<bool>> _bondingTcsForAddress;

        public BleAdapter(BluetoothManager bluetoothManager)
        {
            _bluetoothManager = bluetoothManager;
            _bluetoothAdapter = bluetoothManager.Adapter;

            //bonding
            var bondStatusBroadcastReceiver = new BondBroadcastReceiver(this);
            Application.Context.RegisterReceiver(bondStatusBroadcastReceiver,
                new IntentFilter(BluetoothDevice.ActionBondStateChanged));

            //forward events from broadcast receiver
            bondStatusBroadcastReceiver.BondStateChanged += (s, args) =>
            {
                HandleDeviceBondStateChanged(args);

                if (!_bondingTcsForAddress.TryGetValue(args.Address, out var tcs))
                {
                    return;
                }

                if (args.State == DeviceBondState.Bonding)
                {
                    return;
                }

                if (args.State == DeviceBondState.Bonded)
                {
                    tcs.TrySetResult(true);
                }

                tcs.TrySetException(new Exception("Bonding failed."));
            };

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                _api21ScanCallback = new Api21BleScanCallback(this);
            }
            else
            {
               // _api18ScanCallback = new Api18BleScanCallback(this);
            }
        }

        public override Task BondAsync(IDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (!(device.NativeDevice is BluetoothDevice nativeDevice))
            {
                throw new ArgumentException("Invalid device type");
            }

            if (nativeDevice.BondState == Bond.Bonded)
            {
                return Task.CompletedTask;
            }

            var deviceAddress = nativeDevice.Address!;
            if (_bondingTcsForAddress.TryGetValue(deviceAddress, out var tcs))
            {
                tcs.TrySetException(new Exception("Bonding failed on old try."));
                _bondingTcsForAddress.Remove(deviceAddress);
            }

            var taskCompletionSource = new TaskCompletionSource<bool>();

            _bondingTcsForAddress.Add(nativeDevice.Address!, taskCompletionSource);

            if (!nativeDevice.CreateBond())
            {
                _bondingTcsForAddress.Remove(nativeDevice.Address);
                throw new Exception("Bonding failed");
            }

            return taskCompletionSource.Task;
        }

        protected override Task StartScanningForDevicesNativeAsync(ScanFilterOptions scanFilterOptions, bool allowDuplicatesKey, CancellationToken scanCancellationToken)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                StartScanningOld(scanFilterOptions?.ServiceUuids);
            }
            else
            {
                StartScanningNew(scanFilterOptions);
            }

            return Task.FromResult(true);
        }

        private void StartScanningOld(Guid[] serviceUuids)
        {
            var hasFilter = serviceUuids?.Any() ?? false;
            UUID[] uuids = null;
            if (hasFilter)
            {
                uuids = serviceUuids.Select(u => UUID.FromString(u.ToString())).ToArray();
            }
            Trace.Message("Adapter < 21: Starting a scan for devices.");
#pragma warning disable 618
            //_bluetoothAdapter.StartLeScan(uuids, _api18ScanCallback);
#pragma warning restore 618
        }

        private void StartScanningNew(ScanFilterOptions scanFilterOptions)
        {
            var hasFilter = scanFilterOptions?.HasFilter == true;
            List<ScanFilter> scanFilters = null;

            if (hasFilter)
            {
                scanFilters = new List<ScanFilter>();
                if (scanFilterOptions.HasServiceIds)
                {
                    foreach (var serviceUuid in scanFilterOptions.ServiceUuids)
                    {
                        var sfb = new ScanFilter.Builder();
                        sfb.SetServiceUuid(ParcelUuid.FromString(serviceUuid.ToString()));
                        scanFilters.Add(sfb.Build());
                    }
                }
                if (scanFilterOptions.HasServiceData)
                {
                    foreach (var serviceDataFilter in scanFilterOptions.ServiceDataFilters)
                    {
                        var sfb = new ScanFilter.Builder();
                        if (serviceDataFilter.ServiceDataMask == null)
                            sfb.SetServiceData(ParcelUuid.FromString(serviceDataFilter.ServiceDataUuid.ToString()), serviceDataFilter.ServiceData);
                        else
                            sfb.SetServiceData(ParcelUuid.FromString(serviceDataFilter.ServiceDataUuid.ToString()), serviceDataFilter.ServiceData, serviceDataFilter.ServiceDataMask);
                        scanFilters.Add(sfb.Build());
                    }
                }
                if (scanFilterOptions.HasManufacturerIds)
                {
                    foreach (var manufacturerDataFilter in scanFilterOptions.ManufacturerDataFilters)
                    {
                        var sfb = new ScanFilter.Builder();
                        if (manufacturerDataFilter.ManufacturerDataMask != null)
                            sfb.SetManufacturerData(manufacturerDataFilter.ManufacturerId, manufacturerDataFilter.ManufacturerData);
                        else
                            sfb.SetManufacturerData(manufacturerDataFilter.ManufacturerId, manufacturerDataFilter.ManufacturerData, manufacturerDataFilter.ManufacturerDataMask);
                        scanFilters.Add(sfb.Build());
                    }
                }
                if (scanFilterOptions.HasDeviceAddresses)
                {
                    foreach (var deviceAddress in scanFilterOptions.DeviceAddresses)
                    {
                        if (BluetoothAdapter.CheckBluetoothAddress(deviceAddress))
                        {
                            var sfb = new ScanFilter.Builder();
                            sfb.SetDeviceAddress(deviceAddress);
                            scanFilters.Add(sfb.Build());
                        }
                        else
                        {
                            Trace.Message($"Device address {deviceAddress} is invalid. The correct format is \"01:02:03:AB:CD:EF\"");
                        }

                    }
                }
                if (scanFilterOptions.HasDeviceNames)
                {
                    foreach (var deviceName in scanFilterOptions.DeviceNames)
                    {
                        var sfb = new ScanFilter.Builder();
                        sfb.SetDeviceName(deviceName);
                        scanFilters.Add(sfb.Build());
                    }
                }

            }

            var ssb = new ScanSettings.Builder();
            ssb.SetScanMode(ScanMode.ToNative());

#if NET6_0_OR_GREATER
            if (OperatingSystem.IsAndroidVersionAtLeast(23))
#else
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
#endif
            {
                // set the match mode on Android 6 and above
                ssb.SetMatchMode(ScanMatchMode.ToNative());

                // If set to agressive, reduce the number of adverts needed before raising the DeviceFound callback
                if (ScanMatchMode.ToNative() == BluetoothScanMatchMode.Aggressive)
                {
                    // Be more agressive when seeking adverts
                    ssb.SetNumOfMatches((int)BluetoothScanMatchNumber.OneAdvertisement);
                    Trace.Message("Using ScanMatchMode Agressive");
                }
            }

#if NET6_0_OR_GREATER
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
#else
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
#endif
            {
                // enable Bluetooth 5 Advertisement Extensions on Android 8.0 and above
                ssb.SetLegacy(false);
            }
            //ssb.SetCallbackType(ScanCallbackType.AllMatches);

            if (_bluetoothAdapter.BluetoothLeScanner != null)
            {
                Trace.Message($"Adapter >=21: Starting a scan for devices. ScanMode: {ScanMode}");
                if (hasFilter)
                {
                    if (scanFilterOptions.HasServiceIds)
                    {
                        Trace.Message($"Service UUID Scan Filters: {string.Join(", ", scanFilterOptions.ServiceUuids)}");
                    }
                    if (scanFilterOptions.HasServiceData)
                    {
                        Trace.Message($"Service Data Scan Filters: {string.Join(", ", scanFilterOptions.ServiceDataFilters.ToString())}");
                    }
                    if (scanFilterOptions.HasManufacturerIds)
                    {
                        Trace.Message($"Manufacturer Id Scan Filters: {string.Join(", ", scanFilterOptions.ManufacturerDataFilters.ToString())}");
                    }
                    if (scanFilterOptions.HasDeviceAddresses)
                    {
                        Trace.Message($"Device Address Scan Filters: {string.Join(", ", scanFilterOptions.DeviceAddresses)}");
                    }
                    if (scanFilterOptions.HasDeviceNames)
                    {
                        Trace.Message($"Device Name Scan Filters: {string.Join(", ", scanFilterOptions.DeviceNames)}");
                    }
                }
                _bluetoothAdapter.BluetoothLeScanner.StartScan(scanFilters, ssb.Build(), _api21ScanCallback);
            }
            else
            {
                Trace.Message("Adapter >= 21: Scan failed. Bluetooth is probably off");
            }
        }

        protected override void StopScanNative()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                Trace.Message("Adapter < 21: Stopping the scan for devices.");
#pragma warning disable 618
             //   _bluetoothAdapter.StopLeScan(_api18ScanCallback);
#pragma warning restore 618
            }
            else
            {
                Trace.Message("Adapter >= 21: Stopping the scan for devices.");
                _bluetoothAdapter.BluetoothLeScanner?.StopScan(_api21ScanCallback);
            }
        }

        protected override Task ConnectToDeviceNativeAsync(IDevice device, ConnectParameters connectParameters,
            CancellationToken cancellationToken)
        {
            ((BleDevice)device).Connect(connectParameters, cancellationToken);
            return Task.CompletedTask;
        }

        protected override void DisconnectDeviceNative(IDevice device)
        {
            //make sure everything is disconnected
            ((BleDevice)device).Disconnect();
        }

        public override async Task<IDevice> ConnectToKnownDeviceNativeAsync(Guid deviceGuid, ConnectParameters connectParameters = default(ConnectParameters), CancellationToken cancellationToken = default(CancellationToken))
        {
            var macBytes = deviceGuid.ToByteArray().Skip(10).Take(6).ToArray();
            var nativeDevice = _bluetoothAdapter.GetRemoteDevice(macBytes);
            if (nativeDevice == null)
                throw new Exceptions.DeviceConnectionException(deviceGuid, "", $"[Adapter] Device {deviceGuid} not found.");
            var device = new BleDevice(this, nativeDevice, null);

            await ConnectToDeviceAsync(device, connectParameters, cancellationToken);
            return device;
        }

        public override IReadOnlyList<IDevice> GetSystemConnectedOrPairedDevices(Guid[] services = null)
        {
            if (services != null)
            {
                Trace.Message("Caution: GetSystemConnectedDevices does not take into account the 'services' parameter on Android.");
            }

            //add dualMode type too as they are BLE too ;)
            var connectedDevices = _bluetoothManager.GetConnectedDevices(ProfileType.Gatt).Where(d => d.Type == BluetoothDeviceType.Le || d.Type == BluetoothDeviceType.Dual);

            var bondedDevices = _bluetoothAdapter.BondedDevices.Where(d => d.Type == BluetoothDeviceType.Le || d.Type == BluetoothDeviceType.Dual);

            return connectedDevices.Union(bondedDevices, new DeviceComparer()).Select(d => new BleDevice(this, d, null)).Cast<IDevice>().ToList();
        }

        public override IReadOnlyList<IDevice> GetKnownDevicesByIds(Guid[] ids)
        {
            var devices = GetSystemConnectedOrPairedDevices();
            return devices.Where(item => ids.Contains(item.Id)).ToList();
        }

        protected override IReadOnlyList<IDevice> GetBondedDevices()
        {
            var bondedDevices = _bluetoothAdapter.BondedDevices.Where(d => d.Type == BluetoothDeviceType.Le || d.Type == BluetoothDeviceType.Dual);

            return bondedDevices.Select(d => new BleDevice(this, d, null, 0)).Cast<IDevice>().ToList();
        }

        public override bool SupportsExtendedAdvertising()
        {
#if NET6_0_OR_GREATER
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
#else
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
#endif
            {
                return _bluetoothAdapter.IsLeExtendedAdvertisingSupported;
            }
            else
            {
                return false;
            }
        }

        public override bool SupportsCodedPHY()
        {
#if NET6_0_OR_GREATER
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
#else
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
#endif
            {
                return _bluetoothAdapter.IsLeCodedPhySupported;
            }
            else
            {
                return false;
            }
        }


        private class DeviceComparer : IEqualityComparer<BluetoothDevice>
        {
            public bool Equals(BluetoothDevice x, BluetoothDevice y)
            {
                return x.Address == y.Address;
            }

            public int GetHashCode(BluetoothDevice obj)
            {
                return obj.GetHashCode();
            }
        }


        /*public class Api18BleScanCallback : Object, BluetoothAdapter.ILeScanCallback
        {
            private readonly BleAdapter _adapter;

            public Api18BleScanCallback(BleAdapter adapter)
            {
                _adapter = adapter;
            }

            public void OnLeScan(BluetoothDevice bleDevice, int rssi, byte[] scanRecord)
            {
                Trace.Message("Adapter.LeScanCallback: " + bleDevice.Name);

                _adapter.HandleDiscoveredDevice(new BleDevice(_adapter, bleDevice, null, rssi, scanRecord)); // No IsConnectable!
            }
        }*/

        public class Api21BleScanCallback : ScanCallback
        {
            private readonly BleAdapter _adapter;
            public Api21BleScanCallback(BleAdapter adapter)
            {
                _adapter = adapter;
            }

            public override void OnScanFailed(ScanFailure errorCode)
            {
                Trace.Message("Adapter: Scan failed with code {0}", errorCode);
                base.OnScanFailed(errorCode);
            }

            public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
            {
                base.OnScanResult(callbackType, result);

                
                var device = new BleDevice(_adapter, result.Device, null, result.Rssi, result.ScanRecord.GetBytes(),
#if NET6_0_OR_GREATER
                    OperatingSystem.IsAndroidVersionAtLeast(26)
#else
                    (Build.VERSION.SdkInt >= BuildVersionCodes.O)
#endif
                    ? result.IsConnectable : true
                );

             

                _adapter.HandleDiscoveredDevice(device);
            }
        }
    }
}