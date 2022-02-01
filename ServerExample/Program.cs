using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMFCNet;

namespace Example.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Server server = new Server();
            }
            catch (Exception)
            {

                throw;
            }
            
            Console.ReadLine();
        }
    }
}
