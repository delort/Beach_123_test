using System;
namespace InterviewBle.Models
{
    /// <summary>
    /// A scan filter for manufacturer data (including maufacturer ID and actual data).
    /// Android only.
    /// </summary>
    public class ManufacturerDataFilter
    {
        /// <summary>
        /// The manufacturer Id.
        /// </summary>
        public int ManufacturerId { get; set; }
        /// <summary>
        /// The manufacturer data (as a byte array).
        /// </summary>
        public byte[] ManufacturerData { get; set; } = null;
        /// <summary>
        /// The manufacturer-data mask (as a byte array).
        /// </summary>
        public byte[] ManufacturerDataMask { get; set; } = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ManufacturerDataFilter(int mid, byte[] data = null, byte[] mask = null)
        {
            ManufacturerId = mid;
            ManufacturerData = data ?? Array.Empty<byte>();
            ManufacturerDataMask = mask;
        }
    }
}
