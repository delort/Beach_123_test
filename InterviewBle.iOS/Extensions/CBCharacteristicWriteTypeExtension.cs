﻿using System;
using CoreBluetooth;
using InterviewBle.Enums;
using Xamarin.Forms;

namespace InterviewBle.iOS.Extensions
{
    internal static class CBCharacteristicWriteTypeExtension
    {
        public static CharacteristicWriteType ToCharacteristicWriteType(this CBCharacteristicWriteType writeType)
        {
            switch (writeType)
            {
                case CBCharacteristicWriteType.WithoutResponse:
                    return CharacteristicWriteType.WithoutResponse;
                case CBCharacteristicWriteType.WithResponse:
                    return CharacteristicWriteType.WithResponse;
                default:
                    return CharacteristicWriteType.WithResponse;
            }
        }
    }
}

