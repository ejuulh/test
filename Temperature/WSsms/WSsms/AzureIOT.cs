using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Client;

namespace WSsms
{
    class AzureIOT
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "GOA-IOT-HUB1.azure-devices.net";
        static string deviceKey = "CNjbCkbdvEdkJU/TP6/Vy9wNT7CYtNNcF1FoS+K6YxY=";

        public void init()
        {
            //Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("ServerRoomMonitor", deviceKey), TransportType.Mqtt);
            deviceClient.ProductInfo = "ServerRoomMonitor-By-Ejh";
            //SendDeviceToCloudMessagesAsync();
            //Console.ReadLine();
        }

        public void SendDeviceToCloudMessagesAsync(double currentTemperature)
        {
            //double minTemperature = 20;
            //double minHumidity = 60;
            //int messageId = 1;
            Random rand = new Random();

            //while (true)
            //{
                
                //double currentHumidity = 0;

                var telemetryDataPoint = new
                {
                    //messageId = messageId++,
                    datetime = DateTime.Now,
                    deviceId = "ServerRoomMonitor",
                    temperature = currentTemperature,
                    //humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
            //message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

            //await 
            deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                //await Task.Delay(5000);
            //}
        }

    }
}
