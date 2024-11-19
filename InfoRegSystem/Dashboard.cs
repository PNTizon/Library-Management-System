using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace InfoRegSystem
{
    public partial class Dashboard : Form
    {
        private frmRegistration frmRegistration;

        public Dashboard()
        {
            InitializeComponent();
            displayBorrow();
            displayMem();

            ProjectDatabase db = new ProjectDatabase();
          
        }
        public Dashboard(frmRegistration frmRegistration) : this()
        {
            this.frmRegistration = frmRegistration;
        }

        private void btnBorrow_Click(object sender, EventArgs e)
        {
            BookInfo bookInfoForm = new BookInfo(this);
            openDashboard(bookInfoForm);
            bookInfoForm.Location = this.Location;
        }
        private void btnReturnBook_Click(object sender, EventArgs e)
        {
            BorrowForm borrowform = new BorrowForm(this);
            openDashboard(borrowform);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmRegistration registrationForm = new frmRegistration();
            registrationForm.Show();
            this.Close();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegistarionFormcs registarionform = new RegistarionFormcs(this);
            openDashboard(registarionform);
            registarionform.Location = this.Location;
        }
        private Form activeForm = null;
        private void openDashboard(Form dashform)
        {
            if (activeForm != null) activeForm.Close();

            activeForm = dashform;
            dashform.TopLevel = false;
            dashform.Dock = DockStyle.Fill;
            pnlDash.Controls.Add(dashform);
            pnlDash.Tag = dashform;
            dashform.BringToFront();
            dashform.Show();
        }
        public void displayMem()
        {
            try
            {
                if (ProjectDatabase.con.State != ConnectionState.Open)
                {
                    ProjectDatabase.con.Open();
                }

                string selectdata = "SELECT COUNT(Id) FROM studentable";
                using (SqlCommand cmd = new SqlCommand(selectdata, ProjectDatabase.con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int count = Convert.ToInt32(reader[0]);
                        lblTotalMem.Text = count.ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (ProjectDatabase.con.State == ConnectionState.Open)
                {
                    ProjectDatabase.con.Close();
                }
            }
        }
        public void displayBorrow()
        {
            try
            {
                ProjectDatabase.Open();

                string query = @"SELECT 
            (SELECT COUNT(Id) FROM borrowtable WHERE BorrowedDate IS NOT NULL AND ReturnDate IS NULL) AS BorrowedCount,
            (SELECT COUNT(Id) FROM borrowtable WHERE ReturnDate IS NOT NULL) AS ReturnedCount";

                using (SqlCommand cmd = new SqlCommand(query, ProjectDatabase.con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int borrowedCount = Convert.ToInt32(reader["BorrowedCount"]);
                        int returnedCount = Convert.ToInt32(reader["ReturnedCount"]);

                        if (lblBorrowBoo != null) lblBorrowBoo.Text = borrowedCount.ToString();
                        if (lblReturnBoo != null) lblReturnBoo.Text = returnedCount.ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ProjectDatabase.Close();
            }
        }
        public void loadbookslist()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT  Title, Author, Copies FROM Books", ProjectDatabase.con);
            DataTable table = new DataTable();
            da.Fill(table);

            dataGridViewBookInfo.DataSource = table;
        }
        public void loadstudentlist()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT  Name,Lastname,Age,Barangay,Email,Phone FROM studentable", ProjectDatabase.con);
            DataTable table = new DataTable();
            da.Fill(table);

            dataGridViewBookInfo.DataSource = table;
        }
        #region Helper
        private void Dashboard_Load(object sender, EventArgs e)
        {
            loadbookslist();
        }
        private void DisplaySearch(object sender, EventArgs e)
        {
            loadstudentlist();
        }
        #endregion
        private void btnSearchStudent_Click(object sender, EventArgs e)
        {
            string searchInput = searchbox.Text;

            ProjectDatabase.con.Open();
            string query = "SELECT * FROM studentable WHERE Id LIKE @searchInput OR Name LIKE @searchInput OR Lastname LIKE @searchInput";

            using (SqlDataAdapter adapter = new SqlDataAdapter(query, ProjectDatabase.con))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@searchInput", $"%{searchInput}%");
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGridViewBookInfo.DataSource = table;
            }
        }

        
    }
}
