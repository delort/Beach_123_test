using System;
namespace InterviewBle.Events
{
    public class DeviceErrorEventArgs : DeviceEventArgs
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage;
    }
}
