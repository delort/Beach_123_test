using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterviewBle.Abstractions;
using InterviewBle.Enums;
using InterviewBle.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InterviewBle
{
    public partial class MainPage : ContentPage
    {
        private readonly IBleAdapter _bluetoothAdapter;                            
        private readonly List<IDevice> _gattServiceDevices = new List<IDevice>();

        public MainPage(IBleAdapter bluetoothAdapter)
        {
            InitializeComponent();
            _bluetoothAdapter = bluetoothAdapter;
            _bluetoothAdapter.ScanTimeout = 30000;

            if (_bluetoothAdapter == null)
            {
                DisplayAlert("Please turn on bluetooth", "bluetooth is turned off", "OK");

            }
            _bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>
            {
                if (foundBleDevice.Device != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
                    _gattServiceDevices.Add(foundBleDevice.Device);
            };
        }
        private async Task<bool> PermissionsGrantedAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            return status == PermissionStatus.Granted;
        }


        private async void SearchBle_Clicked(object sender, EventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchBLeButton.IsEnabled = false);
            BleDevicesUIListView.ItemsSource = null;

            if (!await PermissionsGrantedAsync())
            {
                await DisplayAlert("Permission required", "Application needs location permission", "OK");
                IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchBLeButton.IsEnabled = true);
                return;
            }

            _gattServiceDevices.Clear(); 

            if (!_bluetoothAdapter.IsScanning)                                                             
            {
                await _bluetoothAdapter.StartScanningForDevicesAsync();
            }

            foreach (var device in _bluetoothAdapter.ConnectedDevices)
            {
                _gattServiceDevices.Add(device);
            }

            BleDevicesUIListView.ItemsSource = _gattServiceDevices.ToArray();                                   
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchBLeButton.IsEnabled = true);        
        }
        private async void ConnectToDevice_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchBLeButton.IsEnabled = false);
            IDevice selectedItem = e.Item as IDevice; 

            if (selectedItem.State == DeviceState.Connected)
            {
                await Navigation.PushAsync(new DeviceDetails(selectedItem));
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await _bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);          
                    await Navigation.PushAsync(new DeviceDetails(selectedItem));
                }
                catch
                {
                    await DisplayAlert("Error connecting", $"Error connecting to BLE device: {selectedItem.Name ?? "N/A"}", "Retry");
                }
            }

            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(SearchBLeButton.IsEnabled = true);
        }

        
    }
}
