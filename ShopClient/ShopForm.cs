using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopClient
{
    public partial class ShopForm : Form 
    {
        private readonly ShopServerComm _serverComm;
        public ShopForm(ShopServerComm serverComm)
        {
            InitializeComponent();
            _serverComm = serverComm;

            lblUser.Text = $"Currently logged in as: {_serverComm.CurrentUser}";
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await RefreshLists();
        }

        private async Task RefreshLists()
        {
            try
            {
                //display products
                string productsResponse = await _serverComm.GetProductsAsync();
                if (productsResponse.StartsWith("PRODUCTS:"))
                {
                    string[] products = productsResponse.Substring(9).Split('|');
                    lstProducts.Items.Clear();
                    foreach (string product in products)
                    {
                        lstProducts.Items.Add(product);
                    }
                }

                //display orders
                string ordersResponse = await _serverComm.GetOrdersAsync();
                if (ordersResponse.StartsWith("ORDERS:"))
                {
                    string[] orders = ordersResponse.Substring(7).Split('|');
                    lstOrders.Items.Clear();
                    foreach (string order in orders)
                    {
                        lstOrders.Items.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error refreshing lists: " + ex.Message;
            }
        }

        private async void btnPurchase_Click(object sender, EventArgs e)
        {
            if (lstProducts.SelectedItem == null)
            {
                lblStatus.Text = "Please select a product first!";
                return;
            }

            //debug line
            //Console.WriteLine($"Selected item: {lstProducts.SelectedItem}");

            string selectedProduct = lstProducts.SelectedItem.ToString().Split(',')[0];

            //debug line
            //Console.WriteLine($"Extracted product name: {selectedProduct}");

            btnPurchase.Enabled = false;

            try
            {
                string response = await _serverComm.PurchaseAsync(selectedProduct);
                //debug line
                //Console.WriteLine($"Server response: {response}");

                if (response == "DONE")
                {
                    lblStatus.Text = "Purchase successful!";
                    await RefreshLists();
                }
                else
                {
                    lblStatus.Text = response == "NOT_AVAILABLE" ?
                        "Product not available" : "Invalid product";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error making purchase: " + ex.Message;
            }
            finally
            {
                btnPurchase.Enabled = true;
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            btnRefresh.Enabled = false;
            await RefreshLists();
            btnRefresh.Enabled = true;
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                await _serverComm.DisconnectAsync();
            }
            finally
            {
                Application.Exit();
            }
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            try
            {
                await _serverComm.DisconnectAsync();
            }
            catch (Exception)
            {
                //handle any disconnect errors
            }
            finally
            {
                Application.Exit();  //ensure application fully exits
            }
        }
    }
}
