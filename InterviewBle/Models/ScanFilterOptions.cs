using System;
using System.Linq;

namespace InterviewBle.Models
{
    /// <summary>
    /// Pass one or multiple scan filters to filter the scan. Pay attention to which filters are platform specific.
    /// At least one scan filter is required to enable scanning whilst the screen is off in Android.
    /// </summary>
    public class ScanFilterOptions
    {
        /// <summary>
        /// Android/iOS/MacOS. Filter the scan by advertised service ID(s).
        /// </summary>
        public Guid[] ServiceUuids { get; set; } = null;

        /// <summary>
        /// Android only. Filter the scan by service data.
        /// </summary>
        public ServiceDataFilter[] ServiceDataFilters { get; set; } = null;

        /// <summary>
        /// Android only. Filter the scan by device address(es)
        /// </summary>
        public string[] DeviceAddresses { get; set; } = null;

        /// <summary>
        /// Android only. Filter the scan by manufacturer data.
        /// </summary>
        public ManufacturerDataFilter[] ManufacturerDataFilters { get; set; } = null;

        /// <summary>
        /// Android only. Filter the scan by device name(s).
        /// </summary>
        public string[] DeviceNames { get; set; } = null;

        /// <summary>
        /// Indicates whether the options include any filter at all.
        /// </summary>
        public bool HasFilter => HasServiceIds || HasServiceData || HasDeviceAddresses || HasManufacturerIds || HasDeviceNames;

        /// <summary>
        /// Indicates whether the options include a filter on service Ids.
        /// </summary>
        public bool HasServiceIds => ServiceUuids?.Any() == true;
        /// <summary>
        /// Indicates whether the options include a filter on service data.
        /// </summary>
        public bool HasServiceData => ServiceDataFilters?.Any() == true;
        /// <summary>
        /// Indicates whether the options include a filter on device addresses.
        /// </summary>
        public bool HasDeviceAddresses => DeviceAddresses?.Any() == true;
        /// <summary>
        /// Indicates whether the options include a filter on manufacturer data.
        /// </summary>
        public bool HasManufacturerIds => ManufacturerDataFilters?.Any() == true;
        /// <summary>
        /// Indicates whether the options include a filter on device names.
        /// </summary>
        public bool HasDeviceNames => DeviceNames?.Any() == true;
    }
}
