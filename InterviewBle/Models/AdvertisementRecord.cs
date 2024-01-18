using System;
using InterviewBle.Enums;
using InterviewBle.Extensions;

namespace InterviewBle.Models
{
    public class AdvertisementRecord
    {
        /// <summary>
        /// The type of the advertisement record.
        /// </summary>
        public AdvertisementRecordType Type { get; private set; }
        /// <summary>
        /// The data included in the advertisement record (as a byte array).
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// AdvertisementRecord constructor.
        /// </summary>
        public AdvertisementRecord(AdvertisementRecordType type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        /// <summary>
        /// Returns a string describing the record.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Adv rec [Type {0}; Data {1}]", Type, Data.ToHexString());
        }
    }
}
