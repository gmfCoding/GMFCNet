using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GMFCNet;

namespace Example.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Loopback;
            if (args.Length > 0)
            {
                ip = IPAddress.Parse(args[0]);
            }

            Client client = new Client(ip, 5000);

            Console.ReadLine();
        }
    }
}
