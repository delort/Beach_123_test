using System;
using InterviewBle.Abstractions;
using InterviewBle.Enums;
using InterviewBle.Events;

namespace InterviewBle.Models
{
    /// <summary>
    /// Base class for platform-specific BLE implementations.
    /// </summary>
    public abstract class BleImplementationBase : IBluetoothLE
    {
        private readonly Lazy<IBleAdapter> _adapter;
        private BluetoothState _state;

        /// <summary>
        /// Occurs when the state of the Bluetooth adapter changes.
        /// </summary>
        public event EventHandler<BluetoothStateChangedArgs> StateChanged;

        /// <summary>
        /// Indicates whether the device supports BLE.
        /// </summary>
        public bool IsAvailable => _state != BluetoothState.Unavailable;
        /// <summary>
        /// Indicates whether the Bluetooth adapter is turned on.
        /// </summary>
        public bool IsOn => _state == BluetoothState.On;
        /// <summary>
        /// The Bluetooth adapter.
        /// </summary>
        public IBleAdapter Adapter => _adapter.Value;

        /// <summary>
        /// The current state of the Bluetooth adapter.
        /// </summary>
        public BluetoothState State
        {
            get => _state;
            protected set
            {
                if (_state == value)
                    return;

                var oldState = _state;
                _state = value;
                StateChanged?.Invoke(this, new BluetoothStateChangedArgs(oldState, _state));
            }
        }

        /// <summary>
        /// BleImplementationBase constructor.
        /// </summary>
        protected BleImplementationBase()
        {
            _adapter = new Lazy<IBleAdapter>(CreateAdapter, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Initialize the Bluetooth adapter and determine its initial state.
        /// </summary>
        public void Initialize()
        {
            InitializeNative();
            State = GetInitialStateNative();
        }

        private IBleAdapter CreateAdapter()
        {
           /* if (!IsAvailable)
                return new FakeAdapter();*/

            return CreateNativeAdapter();
        }

        /// <summary>
        /// Native implementation of <c>Initialize</c>.
        /// </summary>
        protected abstract void InitializeNative();
        /// <summary>
        /// Get initial state of native adapter.
        /// </summary>
        protected abstract BluetoothState GetInitialStateNative();
        /// <summary>
        /// Create the native adapter.
        /// </summary>
        protected abstract IBleAdapter CreateNativeAdapter();
    }
}