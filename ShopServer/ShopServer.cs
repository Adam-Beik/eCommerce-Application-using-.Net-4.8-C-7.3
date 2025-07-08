using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopServer
{
    public class ShopServer
    {
        //port
        private readonly int _port;
        //StoreData object
        private readonly StoreData _storeData;
        //tcp listener
        private TcpListener _listener;
        //check if running
        private bool _isRunning;

        public ShopServer(int port)
        {
            _port = port;
            _storeData = new StoreData();
            _isRunning = false;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _isRunning = true;

            Console.WriteLine($"Server started on port {_port}");

            while (_isRunning)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    Console.WriteLine("New client connected");

                    ShopClientHandler clientHandler = new ShopClientHandler(client, _storeData);
                    ThreadPool.QueueUserWorkItem(clientHandler.HandleClient);
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"SocketException: {e.Message}");
                    if (!_isRunning)
                    {
                        break;
                    }
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
            Console.WriteLine("Server has been stopped.");
        }
    }
}
