using CommandLine;
using System;
using XiaomiMiTemperatureAndHumiditySensor.Core;
using System.Linq;

namespace XiaomiMiTemperatureAndHumiditySensor.Console
{
    class Program
    {
        public enum Mode
        {
            FindDevices = 0,
            FindDevicesAndGetDatas = 1,
            FindDevicesAndWatchDatas = 2,
            WatchDevicesAndDatas = 3,
            GetDatasForDevice = 4,
            WatchDatasForDevice = 5,
        }
        public class Options
        {
            [Option('m', "mode", Required = false, HelpText = "Set mode.")]
            public Mode Mode { get; set; }

            [Option('d', "device", Required = false, HelpText = "Set device id (mandatory when using GetDatasForDevice mode)")]
            public string DeviceId { get; set; }
        }

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                   .WithParsedAsync<Options>(async o =>
                   {
                       if (o.Mode == Mode.FindDevices)
                       {
                           var devices = await Discovery.DiscoverDevices();
                           foreach (var device in devices)
                           {
                               System.Console.WriteLine("DEVICE FOUND : ");
                               System.Console.WriteLine($"Name : { device.Name}");
                               System.Console.WriteLine($"Id : { device.Id}");
                               System.Console.WriteLine($"Properties: { string.Join(",", device.Properties.Select(p => $"{p.Key}={p.Value}"))}");
                               System.Console.WriteLine();
                           }
                       }
                       else if (o.Mode == Mode.FindDevicesAndGetDatas)
                       {
                           var devices = await Discovery.DiscoverDevices();
                           foreach (var device in devices)
                           {
                               var datas = await Discovery.GetData(device);
                               System.Console.WriteLine("DEVICE FOUND : ");
                               System.Console.WriteLine($"Name : { device.Name}");
                               System.Console.WriteLine($"Id : { device.Id}");
                               System.Console.WriteLine(datas);
                               System.Console.WriteLine();
                           }
                       }
                       else if (o.Mode == Mode.FindDevicesAndWatchDatas)
                       {
                           var devices = await Discovery.DiscoverDevices();
                           foreach (var device in devices)
                           {
                               await Discovery.Subscribe(device, new Progress<MiDeviceData>(d =>
                               {
                                   System.Console.WriteLine($"Name : { device.Name}");
                                   System.Console.WriteLine($"Id : { device.Id}");
                                   System.Console.WriteLine(d);
                                   System.Console.WriteLine();
                               }));
                           }
                       }
                       else if (o.Mode == Mode.WatchDevicesAndDatas)
                       {
                           //todo : write watchdevice method
                       }
                       else if (o.Mode == Mode.GetDatasForDevice && !string.IsNullOrWhiteSpace(o.DeviceId))
                       {
                           var datas = await Discovery.GetData(o.DeviceId);
                           System.Console.WriteLine(datas);
                           System.Console.WriteLine();
                       }
                       else if (o.Mode == Mode.WatchDatasForDevice && !string.IsNullOrWhiteSpace(o.DeviceId))
                       {
                           //todo : write watchdevice method
                       }
                   });
        }
    }
}
