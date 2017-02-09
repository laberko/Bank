using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.Threading;
using BankService;

namespace BankApp
{
    class Program
    {
        static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var appSetting = ConfigurationManager.AppSettings["dataDir"];
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDir, appSetting);
            var fullPath = Path.GetFullPath(path);
            AppDomain.CurrentDomain.SetData("DataDirectory", fullPath);

            var service = new ServiceHost(typeof(WcfService));
            service.Open();
            WcfService.Log("Service state: " + service.State + ". Press Ctrl-C to stop.");
            //keep program running
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };
            QuitEvent.WaitOne();
            service.Close();
            WcfService.Log("Service state: " + service.State + ". Press a key to exit.");
            Console.ReadKey();
        }
    }
}
