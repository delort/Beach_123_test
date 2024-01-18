using System;
using InterviewBle.Abstractions;
using InterviewBle.Enums;

namespace InterviewBle.Events
{
    public class DeviceBondStateChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The device address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The device.
        /// </summary>
        public IDevice Device { get; set; }

        /// <summary>
        /// The bond state.
        /// </summary>
        public DeviceBondState State { get; set; }
    }
}
