using System;
using System.Collections.Generic;
using InterviewBle.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InterviewBle
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceDetails : ContentPage
    {
        private readonly IDevice _connectedDevice;                              
        private readonly List<IGattService> _servicesList = new List<IGattService>();

        public DeviceDetails(IDevice connectedDevice)                              
        {
            InitializeComponent();

            _connectedDevice = connectedDevice;
            bleDevice.Text = "Selected BLE device: " + _connectedDevice.Name;
        }

        protected async override void OnAppearing() 
        {
            base.OnAppearing();

            try
            {
                var servicesListReadOnly = await _connectedDevice.GetServicesAsync();          

                _servicesList.Clear();
                var servicesListStr = new List<String>();
                for (int i = 0; i < servicesListReadOnly.Count; i++)                             
                {
                    _servicesList.Add(servicesListReadOnly[i]);                               
                    servicesListStr.Add(servicesListReadOnly[i].Name + ", UUID: " + servicesListReadOnly[i].Id.ToString());
                }
                foundBleServs.ItemsSource = servicesListStr;                                   
            }
            catch
            {
                await DisplayAlert("Error initializing", $"Error initializing UART GATT service.", "OK");
            }
        } 
    }
}