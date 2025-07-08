using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopServer
{
    public class Order
    {
        public string ProductName { get; }
        public int Quantity { get; private set; }  //starts as 1
        public string UserName { get; }

        public Order(string productName, string userName)
        {
            ProductName = productName;
            Quantity = 1;
            UserName = userName;
        }

        public void IncreaseQuantity()
        {
            Quantity++;
        }
    }
}
