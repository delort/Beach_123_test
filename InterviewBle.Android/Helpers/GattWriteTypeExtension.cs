using System;
using Android.Bluetooth;
using InterviewBle.Enums;

namespace InterviewBle.Droid.Helpers
{
    internal static class GattWriteTypeExtension
    {
        public static CharacteristicWriteType ToCharacteristicWriteType(this GattWriteType writeType)
        {
            if (writeType.HasFlag(GattWriteType.NoResponse))
            {
                return CharacteristicWriteType.WithoutResponse;
            }
            return CharacteristicWriteType.WithResponse;
        }
    }
}