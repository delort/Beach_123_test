using System;
using Android.Bluetooth;
using InterviewBle.Enums;

namespace InterviewBle.Droid.Helpers
{
    internal static class CharacteristicWriteTypeExtension
    {
        public static GattWriteType ToNative(this CharacteristicWriteType writeType)
        {
            switch (writeType)
            {
                case CharacteristicWriteType.WithResponse:
                    return GattWriteType.Default;
                case CharacteristicWriteType.WithoutResponse:
                    return GattWriteType.NoResponse;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}