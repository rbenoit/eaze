using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Web.Scrapper.Service
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // Run as console app if first argument is "console"; supply remaining arguments to start
            if (args != null && args.Length > 0 && string.Equals(args[0], "console", StringComparison.InvariantCultureIgnoreCase))
            {
                JobProcessorService service = new JobProcessorService();

                service.Start((args.Length > 1) ? args.Skip(1).ToArray() : new string[] { });

                Console.WriteLine("Hit enter key to quit running...");
                Console.ReadLine();

                service.Stop();
            }
            // Run as windows service
            else
            {
                using (var service = new JobProcessorService())
                    ServiceBase.Run(service);
            }           
        }       
    }
}
