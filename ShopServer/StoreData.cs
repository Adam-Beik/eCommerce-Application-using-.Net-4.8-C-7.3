using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace ShopServer
{
    public class StoreData
    {
        //threadsafe collections
        private readonly ConcurrentDictionary<string, Product> _products;
        private readonly ConcurrentDictionary<int, Account> _accounts;
        private readonly ConcurrentBag<Order> _orders;
        private readonly object _orderLock = new object();

        public StoreData()
        {
            //initalize random object for product quantity
            Random rnd = new Random();
            //initialize _products dictionary object
            _products = new ConcurrentDictionary<string, Product>();
            //initialize accounts dictionary
            _accounts = new ConcurrentDictionary<int, Account>();
            //initialize orders dictionary
            _orders = new ConcurrentBag<Order>();

            //product entries that add to _products if there's space (1-3 quantity)
            _products.TryAdd("Laptop", new Product("Laptop", rnd.Next(1, 4)));
            _products.TryAdd("Phone", new Product("Phone", rnd.Next(1, 4)));
            _products.TryAdd("Tablet", new Product("Tablet", rnd.Next(1, 4)));
            _products.TryAdd("Watch", new Product("Watch", rnd.Next(1, 4)));
            _products.TryAdd("Headphones", new Product("Headphones", rnd.Next(1, 4)));

            //create 3 accounts to add to _accounts dictionary
            _accounts.TryAdd(1001, new Account(1001, "John Doe"));
            _accounts.TryAdd(1002, new Account(1002, "Jane Smith"));
            _accounts.TryAdd(1003, new Account(1003, "Bob Johnson"));
        }

        //validate account
        public bool ValidateAccount(int accountNumber, out string userName)
        {
            if(_accounts.TryGetValue(accountNumber, out Account account))
            {
                userName = account.UserName;
                return true;
            }
            userName = null;
            return false;
        }

        //get all products with quantities
        public string GetProductList()
        {
            return string.Join("|", _products.Values.Select(product => $"{product.Name},{product.Quantity}"));
        }

        //get orders
        public string GetOrdersList()
        {
            //convert Order object to string: productName,Quantity,userName
            return string.Join("|", _orders.Select(order => $"{order.ProductName},{order.Quantity},{order.UserName}"));
        }

        // method for attempting a purchase
        public bool TryPurchase(string productName, string userName, out string status)
        {
            status = string.Empty;
            Product product;
            if (!_products.TryGetValue(productName, out product))
            {
                status = "NOT_VALID";
                return false;
            }

            if(product.DecreaseQuantity())
            {
                lock (_orderLock) //make order updates safe
                {
                    var existingOrder = _orders.
                        FirstOrDefault(order => order.ProductName == productName
                        && order.UserName == userName);

                    if (existingOrder != null)
                    {
                        existingOrder.IncreaseQuantity();
                    }
                    else
                    {
                        _orders.Add(new Order(productName, userName));
                    }
                }
                status = "DONE";
                return true;
            }

            status = "NOT_AVAILABLE";
            return false;
        }

    }
}
