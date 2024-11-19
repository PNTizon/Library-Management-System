using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace InfoRegSystem
{
    public partial class BorrowForm : Form
    {
        private Dashboard dashboard;

        public BorrowForm()
        {
            InitializeComponent();
        }

        public BorrowForm(Dashboard dashboard)
        {
            InitializeComponent();
            this.dashboard = dashboard;
        }

        private void btnBorrow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtBook.Text))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ProjectDatabase.con.Open();

                //Check if the student exists
                SqlCommand checkStudentCmd = new SqlCommand("SELECT Id FROM studentable WHERE Name = @Name AND Lastname = @Lastname", ProjectDatabase.con);
                checkStudentCmd.Parameters.AddWithValue("@Name",txtName.Text);
                checkStudentCmd.Parameters.AddWithValue("@Lastname",txtLastname.Text);

                object studentIdResult = checkStudentCmd.ExecuteScalar();
                if (studentIdResult == null)
                {
                    MessageBox.Show("The entered student name is not registered in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int studentId = Convert.ToInt32(studentIdResult);

                //Check if book exists
                SqlCommand checkBookCmd = new SqlCommand("SELECT COUNT(1) FROM Books WHERE Title = @Book", ProjectDatabase.con);
                checkBookCmd.Parameters.AddWithValue("@Book", txtBook.Text);
                int bookExists = (int)checkBookCmd.ExecuteScalar();

                if (bookExists == 0)
                {
                    MessageBox.Show("The entered book is not registered in the system.", "Error", MessageBoxButtons.OK);
                    return;
                }
                //Insert into borrow table
                SqlCommand insertCmd = new SqlCommand(
                    "INSERT INTO borrowtable (Name, Book, BorrowedDate, ReturnDate) VALUES (@Name, @Book, @BorrowedDate, NULL)", ProjectDatabase.con);
                insertCmd.Parameters.AddWithValue("@Name", txtName.Text);
                insertCmd.Parameters.AddWithValue("@Lastname",txtLastname.Text );
                insertCmd.Parameters.AddWithValue("@Book", txtBook.Text);
                DateTime borrowedDate = BorrowDate.Value;
                insertCmd.Parameters.AddWithValue("@BorrowedDate", borrowedDate);
                insertCmd.ExecuteNonQuery();

                //DecrementCopies in the Books table
                SqlCommand updateCopiesCmd = new SqlCommand("UPDATE Books SET Copies = Copies - 1 WHERE Title = @Book AND Copies > 0", ProjectDatabase.con);
                updateCopiesCmd.Parameters.AddWithValue("@Book", txtBook.Text);
                updateCopiesCmd.ExecuteNonQuery();

                MessageBox.Show("Book borrowed successfully!");

                dashboard?.displayBorrow();
                dashboard?.loadbookslist();
            }
            catch (Exception ex) 
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ProjectDatabase.con.Close();
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ProjectDatabase.con.Open();

            SqlCommand cnn = new SqlCommand("UPDATE borrowtable SET Book=@Book, Name=@Name,Lastname = @Lastname, ReturnDate = @ReturnDate, BorrowedDate = @BorrowedDate WHERE Id = @Id", ProjectDatabase.con);

            cnn.Parameters.AddWithValue("@Name", txtName.Text);
            cnn.Parameters.AddWithValue("@Book", txtBook.Text);

            // Get the selected Borrow Date from DateTimePicker
            DateTime borrowedDate = BorrowDate.Value;
            cnn.Parameters.AddWithValue("@BorrowedDate", borrowedDate);

            // Get the selected Return Date from DateTimePicker
            DateTime? returnDate = ReturnDate.Checked ? (DateTime?)ReturnDate.Value : null;
            cnn.Parameters.AddWithValue("@ReturnDate", returnDate.HasValue ? (object)returnDate.Value : DBNull.Value);

            cnn.ExecuteNonQuery();
            ProjectDatabase.con.Close();
            MessageBox.Show("Data Updated");

            dashboard?.displayBorrow();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ProjectDatabase.con.Open();

                SqlCommand cnn = new SqlCommand("Delete borrowtable where name=@name", ProjectDatabase.con);

                cnn.ExecuteNonQuery();
                ProjectDatabase.con.Close();
                MessageBox.Show("Data Deleted");

                dashboard?.displayBorrow();
            }
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            SqlCommand cnn = new SqlCommand("Select Name,Book,BorrowedDate,ReturnDate from borrowtable", ProjectDatabase.con);

            SqlDataAdapter da = new SqlDataAdapter(cnn);
            DataTable table = new DataTable();
            da.Fill(table);

            dataGridViewBorrow.DataSource = table;
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            UpdateReturnDate();
            dashboard?.displayBorrow();
        }
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void UpdateReturnDate()
        {
            ProjectDatabase.con.Open();

            SqlCommand cmd = new SqlCommand("UPDATE borrowtable SET ReturnDate = @ReturnDate WHERE Id = @Id", ProjectDatabase.con);
            cmd.Parameters.AddWithValue("@ReturnDate", ReturnDate.Value);  // Get the Return Date from the DateTimePicker

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                SqlCommand updateCopiesCmd = new SqlCommand("UPDATE Books SET Copies = Copies + 1 WHERE Title = @Book", ProjectDatabase.con);
                updateCopiesCmd.Parameters.AddWithValue("@Book", txtBook.Text);
                updateCopiesCmd.ExecuteNonQuery();

                MessageBox.Show("Return date updated successfully.");
                btnDisplay_Click(null, null); // Refresh borrow records
                dashboard?.loadbookslist();
            }
            else
            {
                MessageBox.Show("Error: No record found for the specified ID.");
            }
            ProjectDatabase.con.Close();

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridViewBorrow.Rows[e.RowIndex];

                    txtName.Text = row.Cells["name"].Value.ToString();
                    txtBook.Text = row.Cells["book"].Value.ToString();

                    if (row.Cells["borroweddate"].Value != DBNull.Value)
                    {
                        BorrowDate.Value = Convert.ToDateTime(row.Cells["borroweddate"].Value);
                    }
                    else
                    {
                        BorrowDate.Value = DateTime.Now;
                    }

                    if (row.Cells["returndate"].Value != DBNull.Value)
                    {
                        ReturnDate.Value = Convert.ToDateTime(row.Cells["returndate"].Value);
                    }
                    else
                    {
                        ReturnDate.Value = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some error occurred: " + ex.Message + " - " + ex.Source);
            }
        }
        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
