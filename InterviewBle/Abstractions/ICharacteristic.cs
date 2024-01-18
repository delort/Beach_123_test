﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InterviewBle.Enums;
using InterviewBle.Events;

namespace InterviewBle.Abstractions
{
    public interface ICharacteristic
    {
        /// <summary>
        /// Event gets raised, when the davice notifies a value change on this characteristic.
        /// To start listening, call <see cref="StartUpdatesAsync"/>.
        /// </summary>
        event EventHandler<CharacteristicUpdatedEventArgs> ValueUpdated;

        /// <summary>
        /// Id of the characteristic.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Uuid of the characteristic.
        /// </summary>
        string Uuid { get; }

        /// <summary>
        /// Name of the characteristic.
        /// Returns the name if the <see cref="Id"/> is a id of a standard characteristic.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the last known value of the characteristic.
        /// </summary>
        byte[] Value { get; }

        /// <summary>
        /// Gets <see cref="Value"/> as UTF8 encoded string representation.
        /// </summary>
        string StringValue { get; }

        /// <summary>
        /// Properties of the characteristic.
        /// </summary>
        CharacteristicPropertyType Properties { get; }

        /// <summary>
        /// Specifies how the <see cref="WriteAsync"/> function writes the value.
        /// </summary>
        CharacteristicWriteType WriteType { get; set; }

        /// <summary>
        /// Indicates wheter the characteristic can be read or not.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Indicates wheter the characteristic can be written or not.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Indicates wheter the characteristic supports notify or not.
        /// </summary>
        bool CanUpdate { get; }

        /// <summary>
        /// Returns the parent service. Use this to access the device.
        /// </summary>
        IGattService Service { get; }

        /// <summary>
        /// Reads the characteristic value from the device. The result is also stored inisde the Value property.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A task that represents the asynchronous read operation. The Result property will contain a tuple with the read bytes and the ble result code.</returns>
        /// <exception cref="InvalidOperationException">Thrown if characteristic doesn't support read. See: <see cref="CanRead"/></exception>
        /// <exception cref="CharacteristicReadException">Thrown if the reading of the value failed.</exception>
        Task<(byte[] data, int resultCode)> ReadAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends <paramref name="data"/> as characteristic value to the device.
        /// </summary>
        /// <param name="data">Data that should be written.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// A task that represents the asynchronous write operation. The Task will finish after the value was written. The Result property will the errror
        /// code sent by the BLE device. (0 = successful)
        /// If the characteristic is write with response, the Task will finish if the value has been written. 
        /// If it is write without response, the task will immediately finish with <c>0</c>.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if characteristic doesn't support write. See: <see cref="CanWrite"/></exception>
        /// <exception cref="ArgumentNullException">Thrwon if <paramref name="data"/> is null.</exception>
        Task<int> WriteAsync(byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts listening for notify events on this characteristic.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidOperationException">Thrown if characteristic doesn't support notify. See: <see cref="CanUpdate"/></exception>
        /// <exception cref="Exception">Thrown if an error occurs while starting notifications </exception>
        Task StartUpdatesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops listening for notify events on this characteristic.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="Exception">Thrown if an error occurs while starting notifications </exception>
        Task StopUpdatesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the descriptors of the characteristic.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A task that represents the asynchronous read operation. The Result property will contain a list of descriptors.</returns> 
        Task<IReadOnlyList<IDescriptor>> GetDescriptorsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the first descriptor with the Id <paramref name="id"/>. 
        /// </summary>
        /// <param name="id">The id of the searched descriptor.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// A task that represents the asynchronous read operation. 
        /// The Result property will contain the descriptor with the specified <paramref name="id"/>.
        /// If the descriptor doesn't exist, the Result will be null.
        /// </returns>
        Task<IDescriptor> GetDescriptorAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
