using System;
using InterviewBle.Abstractions;

namespace InterviewBle.Events
{
    public class CharacteristicUpdatedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The characteristic.
        /// </summary>
        public ICharacteristic Characteristic { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CharacteristicUpdatedEventArgs(ICharacteristic characteristic)
        {
            Characteristic = characteristic;
        }
    }
}
