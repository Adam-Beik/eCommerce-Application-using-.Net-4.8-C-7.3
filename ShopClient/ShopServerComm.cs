using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShopClient
{
    public class ShopServerComm
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        private string _hostname;
        private string _currentUser;
        public string CurrentUser => _currentUser;
        private int _port = 2693;

        public ShopServerComm(string hostname)
        {
            _hostname = hostname;
        }

        public async Task<bool> ConnectAsync(string accountNo)
        {
            //connects to the server and authenticates the user with an account number
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_hostname, _port);

                NetworkStream stream = _client.GetStream();
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream);

                //await connect command
                await _writer.WriteLineAsync($"CONNECT:{accountNo}");
                await _writer.FlushAsync();

                //response
                string response = await _reader.ReadLineAsync();
                if(response.StartsWith("CONNECTED:"))
                {
                    _currentUser = response.Substring(10); //store username to display in form
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetProductsAsync()
        {
            await _writer.WriteLineAsync("GET_PRODUCTS");
            await _writer.FlushAsync();
            return await _reader.ReadLineAsync();
        }

        public async Task<string> GetOrdersAsync()
        {
            await _writer.WriteLineAsync("GET_ORDERS");
            await _writer.FlushAsync();
            return await _reader.ReadLineAsync();
        }

        public async Task<string> PurchaseAsync(string productName)
        {
            await _writer.WriteLineAsync($"PURCHASE:{productName}");
            await _writer.FlushAsync();
            return await _reader.ReadLineAsync();
        }

        public async Task DisconnectAsync()
        {
            if (_client != null && _client.Connected)
            {
                await _writer.WriteLineAsync("DISCONNECT");
                await _writer.FlushAsync();
                _client.Close();
            }
        }
    }
}
