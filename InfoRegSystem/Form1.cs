using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace InfoRegSystem
{
    public partial class frmRegistration : Form
    {
        Dashboard dashboard;
        public frmRegistration(Dashboard dashboard)
        {
            InitializeComponent();
        }
        public frmRegistration()
        {
            InitializeComponent();
            this.AcceptButton = btnLogin;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Validate username and password
            if (!IsValidUsername(username))
            {
                txtError.Text = "Invalid usernamec.";
                return;
            }
           
            // Check if username and password are correct
            if (username == "admin" && password == "12345")
            {
                Dashboard dashboard = new Dashboard();
                dashboard.Location = this.Location;

                dashboard.Show();

                Clear();
                this.Hide();
            }
            else
            {
                txtError.Text = "Invalid username or password!";
            }
        }
        private void Clear()
        {
            txtUsername.Clear();
            txtPassword.Clear();
        }
        private bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, @"^[a-zA-Z0-9]{5,15}$");
        }

    }
}
