using System;

using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace InfoRegSystem
{
    public partial class BookInfo : Form
    {
        private Dashboard _dashboard;
        public BookInfo(Dashboard dashboard)
        {
            InitializeComponent();
            _dashboard = dashboard;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            SqlCommand cmd = new SqlCommand("INSERT INTO Books (@Title, @Author, @Copies)", ProjectDatabase.con);

            cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
            cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
            cmd.Parameters.AddWithValue("@Copies", int.Parse(txtCopies.Text));

            ProjectDatabase.con.Open();
            cmd.ExecuteNonQuery();
            MessageBox.Show("Data Saved");
            ProjectDatabase.con.Close();

            _dashboard.loadbookslist();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("No record selected. Please provide a valid record for updating.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int selectedID = Convert.ToInt32(bookgridView.CurrentRow.Cells["BookID"].Value);

                ProjectDatabase.con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE Books SET Title = @Title, Author = @Author, Copies = @Copies WHERE BookID = @BookID", ProjectDatabase.con);

                cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
                cmd.Parameters.AddWithValue("@Author", txtAuthor.Text);
                cmd.Parameters.AddWithValue("@Copies", int.Parse(txtCopies.Text));
                cmd.Parameters.AddWithValue("@BookID", selectedID);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data Updated Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Record not found.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                cmd.ExecuteNonQuery();
                ProjectDatabase.con.Close();
                MessageBox.Show("Data Updated");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _dashboard.loadbookslist();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (bookgridView.CurrentRow == null)
                {
                    MessageBox.Show("Please select a valid record to delete.");
                    return;
                }

                int selectedID = Convert.ToInt32(bookgridView.CurrentRow.Cells["BookID"].Value);

                var result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {

                    SqlDataAdapter da = new SqlDataAdapter("SELECT Title,Author,Copies FROM Books", ProjectDatabase.con);
                    DataTable table = new DataTable();
                    da.Fill(table);

                    bookgridView.DataSource = table;

                    ProjectDatabase.con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Books WHERE BookID = @BookID", ProjectDatabase.con);

                    cmd.Parameters.AddWithValue("@BookID", selectedID);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Record not found.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    ProjectDatabase.con.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _dashboard.loadbookslist();
        }
        public void btndatagridview(object sender, EventArgs e)
        {

            SqlDataAdapter da = new SqlDataAdapter("SELECT BookID,Title,Author,Copies FROM Books", ProjectDatabase.con);
            DataTable table = new DataTable();
            da.Fill(table);
            bookgridView.DataSource = table;

            if (bookgridView.Columns["BookID"] != null)
                bookgridView.Columns["BookID"].Visible = false;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #region Helpers
        private void txtCopies_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtBookID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        #endregion

        private void bookgridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = bookgridView.Rows[e.RowIndex];

                    int selectedID = Convert.ToInt32(bookgridView.CurrentRow.Cells["BookID"].Value);

                    bookgridView.Columns["BookID"].Visible = false;
                    txtTitle.Text = row.Cells["title"].Value.ToString();
                    txtAuthor.Text = row.Cells["author"].Value.ToString();
                    txtCopies.Text = row.Cells["copies"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some error occured: " + ex.Message + " - " + ex.Source);
            }
        }
    }
}
