using System;
using InterviewBle.Abstractions;

namespace InterviewBle.iOS.Services
{
    public static class IosBluetoothLE
    {
        static readonly Lazy<IBluetoothLE> Implementation = new Lazy<IBluetoothLE>(CreateImplementation, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static IBluetoothLE Current
        {
            get
            {
                var ret = Implementation.Value;
                return ret;
            }
        }

        static IBluetoothLE CreateImplementation()
        {
            var implementation = new BleImplementation();
            implementation.Initialize();
            return implementation;
        }
    }
}