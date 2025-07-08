using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (TcpClient client = new TcpClient("localhost", 2693))
                {
                    NetworkStream stream = client.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream);

                    // Test 1: Connect with valid account
                    Console.WriteLine("Testing CONNECT command...");
                    writer.WriteLine("CONNECT:1001");
                    writer.Flush();
                    Console.WriteLine($"Response: {reader.ReadLine()}");

                    // Test 2: Get Products
                    Console.WriteLine("\nTesting GET_PRODUCTS command...");
                    writer.WriteLine("GET_PRODUCTS");
                    writer.Flush();
                    Console.WriteLine($"Response: {reader.ReadLine()}");

                    // Test 3: Try a purchase
                    Console.WriteLine("\nTesting PURCHASE command...");
                    writer.WriteLine("PURCHASE:Laptop");
                    writer.Flush();
                    Console.WriteLine($"Response: {reader.ReadLine()}");

                    // Test 4: Get Orders
                    Console.WriteLine("\nTesting GET_ORDERS command...");
                    writer.WriteLine("GET_ORDERS");
                    writer.Flush();
                    Console.WriteLine($"Response: {reader.ReadLine()}");

                    Console.WriteLine("\nTesting DISCONNECT command...");
                    Console.ReadKey(true);
                    writer.WriteLine("DISCONNECT");
                    writer.Flush();
                    Console.WriteLine("Disconnected from server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
