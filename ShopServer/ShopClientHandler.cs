using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShopServer
{
    public class ShopClientHandler
    {
        private readonly TcpClient _tcpClient;
        private readonly StoreData _storeData;
        private string _currentUserName;    //track current user

        public ShopClientHandler(TcpClient tcpClient, StoreData storeData)
        {
            _tcpClient = tcpClient;
            _storeData = storeData;
            _currentUserName = string.Empty;    //added upon successful connection
        }

        public void HandleClient(object threadInfo)
        {
            using (_tcpClient)
            {
                try
                {
                    NetworkStream stream = _tcpClient.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream);

                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null) break; //disconnect scenario

                        try
                        {
                            //command processing
                            string response = ProcessCommand(line);

                            //send response back to client
                            writer.WriteLine(response);
                            writer.Flush();

                            //log
                            Console.WriteLine($"Received: {line}");
                            Console.WriteLine($"Sent: {response}");
                        }
                        catch (DisconnectException)
                        {
                            Console.WriteLine("Client disconnected gracefully.");
                            break;
                        }
                       
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Unexpected client disconnect.");
                }
                
            }
        }

        private string ProcessCommand(string command)
        {
            //split command up
            string[] parts = command.Split(':');
            string cmd = parts[0];

            switch (cmd)
            {
                case "CONNECT":
                    if (parts.Length == 2 && int.TryParse(parts[1], 
                        out int accountNo))
                    {
                        if (_storeData.ValidateAccount(accountNo, 
                            out string userName))
                        {
                            _currentUserName = userName;
                            return $"CONNECTED: {userName}";
                        }
                    }
                    return "CONNECT_ERROR";

                case "GET_PRODUCTS":
                    return $"PRODUCTS:{_storeData.GetProductList()}";

                case "GET_ORDERS":
                    return $"ORDERS:{_storeData.GetOrdersList()}";

                case "PURCHASE":
                    if (parts.Length == 2)
                    {
                        _storeData.TryPurchase(parts[1], _currentUserName, out string status);
                        return status;
                    }
                    return "NOT_VALID";

                case "DISCONNECT":
                    _currentUserName = string.Empty; //rid username on disconnect
                    throw new DisconnectException("Client requested disconnect.");

                default:
                    return "INVALID_COMMAND";
            }
        }

        public class DisconnectException : Exception
        {
            public DisconnectException(string message) : base(message) { }
        }
    }
}
