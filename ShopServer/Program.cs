using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int PORT = 2693; //IANA states unassigned
            ShopServer server = new ShopServer(PORT);

            Console.WriteLine("Starting server..!");

            // start server on its own thread
            Thread serverThread = new Thread(() => server.Start());
            serverThread.Start();

            Console.WriteLine("Server started. Press any key to stop server.");
            Console.ReadKey(true);

            server.Stop();
        }
    }
}
