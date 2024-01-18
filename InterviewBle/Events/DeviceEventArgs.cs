using System;
using InterviewBle.Abstractions;

namespace InterviewBle.Events
{
    public class DeviceEventArgs : System.EventArgs
    {
        /// <summary>
        /// The device.
        /// </summary>
        public IDevice Device;
    }
}
