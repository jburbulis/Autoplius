using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AutopliusReader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                Debug.WriteLine("Started as WindowsService");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new Service1()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                var service = new Service1();
                service.ServiceStart();
                Debug.WriteLine("Started as Console");

                System.Console.WriteLine("Press any key to stop...");
                System.Console.ReadKey(true);
                service.ServiceStop();
            }
           
        }
    }
}
