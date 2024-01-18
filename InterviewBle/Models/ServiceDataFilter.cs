using System;
namespace InterviewBle.Models
{
    public class ServiceDataFilter
    {
        /// <summary>
        /// The service-data UUID.
        /// </summary>
        public Guid ServiceDataUuid { get; set; }
        /// <summary>
        /// The service data (as a byte array).
        /// </summary>
        public byte[] ServiceData { get; set; } = null;
        /// <summary>
        /// The service-data mask (as a byte array).
        /// </summary>
        public byte[] ServiceDataMask { get; set; } = null;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public ServiceDataFilter(Guid guid, byte[] data = null, byte[] mask = null)
        {
            ServiceDataUuid = guid;
            ServiceData = data ?? Array.Empty<byte>();
            ServiceDataMask = mask;
        }
        /// <summary>
        /// Constructor with UUID as string.
        /// </summary>
        public ServiceDataFilter(string uuid, byte[] data = null, byte[] mask = null) : this(new Guid(uuid), data, mask)
        {
        }
    }
}
