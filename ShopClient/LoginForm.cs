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
    public partial class LoginForm : Form
    {
        private ShopServerComm _serverComm;
        public LoginForm()
        {
            InitializeComponent();
            txtHostname.Text = "localhost";
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            lblStatus.Text = "Connecting...";

            try
            {
                _serverComm = new ShopServerComm(txtHostname.Text);
                if (await _serverComm.ConnectAsync(txtAccountNo.Text))
                {
                    //if successful, show shopping form
                    var shopForm = new ShopForm(_serverComm);
                    shopForm.Show();
                    this.Hide();
                }
                else
                {
                    lblStatus.Text = "Login failed. Invalid account number.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Could not connect to server.";
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}
