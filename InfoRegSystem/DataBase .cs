using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace InfoRegSystem
{
    public class ProjectDatabase
    {
        // Static SqlConnection instance
        public static SqlConnection con;

        // Method to initialize the connection if not already initialized
        public static void InitializeConnection()
        {
            if (con == null)
            {
                con = new SqlConnection(@"Data Source=DESKTOP-NF4HS4J;Initial Catalog=librarydb;Integrated Security=True;Encrypt=False");
            }
        }

        // Method to open the connection safely
        public static void Open()
        {
            InitializeConnection(); // Ensure connection is initialized
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }
        }

        // Method to close the connection safely
        public static void Close()
        {
            if (con != null && con.State != System.Data.ConnectionState.Closed)
            {
                con.Close();
            }
        }
    }
}

