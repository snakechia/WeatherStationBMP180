using BMP180;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherStationBMP180
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Bmp180Sensor bmp180;
        Bmp180SensorData bmp180reading = new Bmp180SensorData();
        DispatcherTimer timer;
        const string iotHubUri = "<Replace with your iot hub uri>";
        const string deviceKey = "<Replace with your device key>";
        const string devicename = "<Replace with your device name>";
        DeviceClient deviceClient;

        public MainPage()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += Timer_Tick;
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                bmp180 = new Bmp180Sensor();
                await bmp180.InitializeAsync();

                deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(devicename, deviceKey));
                timer.Start();
                getData();
            }
            catch
            { }
        }

        private void Timer_Tick(object sender, object e)
        {
            getData();
        }

        private async void getData()
        {
            try
            {
                bmp180reading = new Bmp180SensorData();
                bmp180reading = await bmp180.GetSensorDataAsync(Bmp180AccuracyMode.UltraHighResolution);

                WeatherData weather = new WeatherData()
                {
                    deviceId = devicename,
                    temperature = bmp180reading.Temperature,
                    pressure = bmp180reading.Pressure
                };

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    temperatureTB.Text = bmp180reading.Temperature.ToString("0.0 °C");
                    pressureTB.Text = bmp180reading.Pressure.ToString("0.0 Pa");
                });

                string jsonstring = JsonConvert.SerializeObject(weather);
                var msg = new Message(Encoding.UTF8.GetBytes(jsonstring));
                await deviceClient.SendEventAsync(msg);
            }
            catch
            { }
        }
        
    }
}
