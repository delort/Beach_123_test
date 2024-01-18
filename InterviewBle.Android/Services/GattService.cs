using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using InterviewBle.Abstractions;
using InterviewBle.Models;

namespace InterviewBle.Droid.Services
{
    public class GattService : ServiceBase<BluetoothGattService>
    {
        private readonly BluetoothGatt _gatt;
        private readonly IGattCallback _gattCallback;

        public override Guid Id => Guid.ParseExact(NativeService.Uuid.ToString(), "d");
        public override bool IsPrimary => NativeService.Type == GattServiceType.Primary;

        public GattService(BluetoothGattService nativeService, BluetoothGatt gatt, IGattCallback gattCallback, IDevice device)
            : base(device, nativeService)
        {
            _gatt = gatt;
            _gattCallback = gattCallback;
        }

        protected override Task<IList<ICharacteristic>> GetCharacteristicsNativeAsync()
        {
            return Task.FromResult<IList<ICharacteristic>>(
                NativeService.Characteristics.Select(characteristic => new Characteristic(characteristic, _gatt, _gattCallback, this))
                .Cast<ICharacteristic>().ToList());
        }
    }
}