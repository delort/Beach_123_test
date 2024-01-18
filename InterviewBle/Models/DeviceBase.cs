using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterviewBle.Abstractions;
using InterviewBle.Enums;
using InterviewBle.Helpers;

namespace InterviewBle.Models
{
    /// <summary>
    /// Base class for platform-specific <c>Device</c> classes.
    /// </summary>
    public abstract class DeviceBase<TNativeDevice> : IDevice, ICancellationMaster
    {
        /// <summary>
        /// The adapter that connects to this device.
        /// </summary>
        protected readonly IBleAdapter Adapter;
        private readonly List<IGattService> KnownServices = new List<IGattService>();

        /// <summary>
        /// Id of the device.
        /// </summary>
        public Guid Id { get; protected set; }
        /// <summary>
        /// Advertised Name of the Device.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Last known rssi value in decibals.
        /// Can be updated via <see cref="UpdateRssiAsync()"/>.
        /// </summary>
        public int Rssi { get; protected set; }
        /// <summary>
        /// State of the device.
        /// </summary>
        public DeviceState State => GetState();
        /// <summary>
        /// All the advertisment records.
        /// </summary>
        public IReadOnlyList<AdvertisementRecord> AdvertisementRecords { get; protected set; }
        /// <summary>
        /// The native device.
        /// </summary>
        public TNativeDevice NativeDevice { get; protected set; }

        CancellationTokenSource ICancellationMaster.TokenSource { get; set; } = new CancellationTokenSource();
        object IDevice.NativeDevice => NativeDevice;

        /// <summary>
        /// DeviceBase constructor.
        /// </summary>
        protected DeviceBase(IBleAdapter adapter, TNativeDevice nativeDevice)
        {
            Adapter = adapter;
            NativeDevice = nativeDevice;
        }

        /// <summary>
        /// Gets all services of the device.
        /// </summary>
        public async Task<IReadOnlyList<IGattService>> GetServicesAsync(CancellationToken cancellationToken = default)
        {
            lock (KnownServices)
            {
                if (KnownServices.Any())
                {
                    return KnownServices.ToArray();
                }
            }

            using (var source = this.GetCombinedSource(cancellationToken))
            {
                var services = await GetServicesNativeAsync();

                lock (KnownServices)
                {
                    if (services != null)
                        KnownServices.AddRange(services);

                    return KnownServices.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the first service with the Id <paramref name="id"/>. 
        /// </summary>
        /// <param name="id">The id of the searched service.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IGattService> GetServiceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var services = await GetServicesAsync(cancellationToken);
            return services.ToList().FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Requests a MTU update and fires an "Exchange MTU Request" on the ble stack.
        /// </summary>
        public async Task<int> RequestMtuAsync(int requestValue)
        {
            return await RequestMtuNativeAsync(requestValue);
        }

        /// <summary>
        /// Requests a bluetooth-le connection update request.
        /// </summary>
        public bool UpdateConnectionInterval(ConnectionInterval interval)
        {
            return UpdateConnectionIntervalNative(interval);
        }

        /// <summary>
        /// Updates the rssi value.
        /// </summary>
        public abstract Task<bool> UpdateRssiAsync();

        /// <summary>
        /// Determines the state of the device.
        /// </summary>
        protected abstract DeviceState GetState();
        /// <summary>
        /// Native implementation of <c>GetServicesAsync</c>.
        /// </summary>
        protected abstract Task<IReadOnlyList<IGattService>> GetServicesNativeAsync();
        /// <summary>
        /// Currently not being used anywhere!
        /// </summary>
        protected abstract Task<IGattService> GetServiceNativeAsync(Guid id);
        /// <summary>
        /// Native implementation of <c>RequestMtuAsync</c>.
        /// </summary>
        protected abstract Task<int> RequestMtuNativeAsync(int requestValue);
        /// <summary>
        /// Native implementation of <c>UpdateConnectionInterval</c>.
        /// </summary>
        protected abstract bool UpdateConnectionIntervalNative(ConnectionInterval interval);

        /// <summary>
        /// Convert to string (using the advertised device name).
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Dispose the device.
        /// </summary>
        public virtual void Dispose()
        {
            Adapter.DisconnectDeviceAsync(this);
        }

        /// <summary>
        /// Clear all (known) services.
        /// </summary>
        public void ClearServices()
        {
            this.CancelEverythingAndReInitialize();

            lock (KnownServices)
            {
                foreach (var service in KnownServices)
                {
                    try
                    {
                        service.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Trace.Message("Exception while cleanup of service: {0}", ex.Message);
                    }
                }

                KnownServices.Clear();
            }
        }

        /// <summary>
        /// Equality operator for comparison with other devices.
        /// Checks for equality of the <c>Id</c>.
        /// </summary>
        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            var otherDeviceBase = (DeviceBase<TNativeDevice>)other;
            return Id == otherDeviceBase.Id;
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// (using the hash code of the <c>Id</c>).
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Reflects if the device is connectable.
        /// Only supported if <see cref="SupportsIsConnectable"/> is true.
        /// </summary>
        public abstract bool IsConnectable { get; protected set; }

        /// <summary>
        /// Shows whether the device supports the <see cref="IsConnectable"/>.
        /// </summary>
        public abstract bool SupportsIsConnectable { get; }

        /// <summary>
        /// Gets the <see cref="DeviceBondState"/> of the device.
        /// </summary>
        protected abstract DeviceBondState GetBondState();

        /// <summary>
        /// Gets the <see cref="DeviceBondState"/> of the device.
        /// </summary>
        public DeviceBondState BondState => GetBondState();
    }
}