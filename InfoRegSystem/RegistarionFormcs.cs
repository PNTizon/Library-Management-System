using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace InfoRegSystem
{
    public partial class RegistarionFormcs : Form
    {
        private Dashboard dashboard;

        public RegistarionFormcs(Dashboard dashboard)
        {
            InitializeComponent();
            this.dashboard = dashboard;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Please select a valid record to delete.");
                    return;
                }

                int selectedID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);

                var result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {

                    ProjectDatabase.con.Open();

                    using (SqlCommand cnn = new SqlCommand("DELETE FROM studentable WHERE ID=@ID", ProjectDatabase.con))
                    {
                        cnn.Parameters.AddWithValue("@ID", selectedID);

                        int rowsAffected = cnn.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Record not found.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    ProjectDatabase.con.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            button2_Click(sender, e);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsValidPhoneNumber(txtPhoneNum.Text))
            {
                if (!IsValidGmailAddress(txtEmail.Text))
                {
                    MessageBox.Show("Please fill the infomation correclty.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                SqlCommand cnn = new SqlCommand("Insert into studentable values (@name,@lastname,@age,@phone,@barangay,@email)", ProjectDatabase.con);

                cnn.Parameters.AddWithValue("@Name", txtName.Text);
                cnn.Parameters.AddWithValue("@Lastname", txtLastname.Text);
                cnn.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                cnn.Parameters.AddWithValue("@Phone", txtPhoneNum.Text);
                cnn.Parameters.AddWithValue("@Barangay", txtBarangay.Text);
                cnn.Parameters.AddWithValue("@Email", txtEmail.Text);

                ProjectDatabase.con.Open();
                cnn.ExecuteNonQuery();
                MessageBox.Show("Data Saved");
                dashboard?.displayMem();
                ProjectDatabase.con.Close();


                button2_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Invalid Phone number.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtLastname.Text))
                {
                    MessageBox.Show("No record selected. Please provide a valid last name for updating.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!IsValidGmailAddress(txtEmail.Text))
                {
                    MessageBox.Show("Please fill in the email information correctly.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int selectedID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);


                ProjectDatabase.con.Open();

                using (SqlCommand cnn = new SqlCommand("UPDATE studentable SET name=@Name, age=@Age, phone=@Phone, email=@Email, barangay=@Barangay, lastname=@Lastname WHERE ID=@ID", ProjectDatabase.con))
                {
                    cnn.Parameters.AddWithValue("@Name", txtName.Text);
                    cnn.Parameters.AddWithValue("@Lastname", txtLastname.Text);
                    cnn.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                    cnn.Parameters.AddWithValue("@Phone", txtPhoneNum.Text);
                    cnn.Parameters.AddWithValue("@Barangay", txtBarangay.Text);
                    cnn.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cnn.Parameters.AddWithValue("@ID", selectedID);

                    int rowsAffected = cnn.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data Updated Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Record not found.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            button2_Click(sender, e);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ProjectDatabase.con.Open();
            using (SqlCommand cnn = new SqlCommand("SELECT ID, Name,LastName, Age, Phone, Barangay, Email FROM studentable", ProjectDatabase.con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cnn);
                DataTable table = new DataTable();
                da.Fill(table);
                dataGridView1.DataSource = table;

                if (dataGridView1.Columns["ID"] != null)
                    dataGridView1.Columns["ID"].Visible = false;
            }
            ProjectDatabase.con.Close();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    int selectedID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);

                    dataGridView1.Columns["Id"].Visible = false;
                    txtName.Text = row.Cells["name"].Value.ToString();
                    txtLastname.Text = row.Cells["lastname"].Value.ToString();
                    txtAge.Text = row.Cells["age"].Value.ToString();
                    txtPhoneNum.Text = row.Cells["phone"].Value.ToString();
                    txtBarangay.Text = row.Cells["barangay"].Value.ToString();
                    txtEmail.Text = row.Cells["email"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Some error occured: " + ex.Message + " - " + ex.Source);
            }
        }
        private void txtPhoneNum_TextChanged(object sender, EventArgs e)
        {
            if (!txtPhoneNum.Text.StartsWith("9")) txtPhoneNum.Text = "9";
            if (txtPhoneNum.Text.Length > 10) txtPhoneNum.Text = txtPhoneNum.Text.Substring(0, 10);
            txtPhoneNum.SelectionStart = txtPhoneNum.Text.Length;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #region Helpers
        private void txtID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtPhoneNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Length >= 10 && phoneNumber.All(char.IsDigit);
        }
        private bool IsValidGmailAddress(string email)
        {
            return email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
        }
        private void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }
        }
        #endregion
        private void RegistarionFormcs_Load(object sender, EventArgs e)
        {
            var countryCodes = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("AFG","+93"),  // Afghanistan
                new Tuple<string, string>("ARM","+374"), // Armenia
                new Tuple<string, string>("AZE","+994"), // Azerbaijan
                new Tuple<string, string>("BGD","+880"), // Bangladesh
                new Tuple<string, string>("BHR","+973"), // Bahrain
                new Tuple<string, string>("BRN","+673"), // Brunei
                new Tuple<string, string>("BTN","+975"), // Bhutan
                new Tuple<string, string>("BLR","+375"), // Belarus
                new Tuple<string, string>("KHM","+855"), // Cambodia
                new Tuple<string, string>("CHN","+86"),  // China
                new Tuple<string, string>("GEO","+995"), // Georgia
                new Tuple<string, string>("IND","+91"),  // India
                new Tuple<string, string>("IDN","+62"),  // Indonesia
                new Tuple<string, string>("IRQ","+964"), // Iraq
                new Tuple<string, string>("IRN","+98"),  // Iran
                new Tuple<string, string>("JOR","+962"), // Jordan
                new Tuple<string, string>("JPN","+81"),  // Japan
                new Tuple<string, string>("KEN","+254"), // Kenya (not part of Asia but keeping it as a placeholder)
                new Tuple<string, string>("KGZ","+996"), // Kyrgyzstan
                new Tuple<string, string>("KOR","+82"),  // South Korea
                new Tuple<string, string>("KWT","+965"), // Kuwait
                new Tuple<string, string>("KAZ","+7"),   // Kazakhstan
                new Tuple<string, string>("LAO","+856"), // Laos
                new Tuple<string, string>("LBN","+961"), // Lebanon
                new Tuple<string, string>("MYS","+60"),  // Malaysia
                new Tuple<string, string>("MDV","+960"), // Maldives
                new Tuple<string, string>("MNG","+976"), // Mongolia
                new Tuple<string, string>("MMR","+95"),  // Myanmar (Burma)
                new Tuple<string, string>("NPL","+977"), // Nepal
                new Tuple<string, string>("OMN","+968"), // Oman
                new Tuple<string, string>("PAK","+92"),  // Pakistan
                new Tuple<string, string>("PHL","+63"),  // Philippines
                new Tuple<string, string>("QAT","+974"), // Qatar
                new Tuple<string, string>("SAU","+966"), // Saudi Arabia
                new Tuple<string, string>("SGP","+65"),  // Singapore
                new Tuple<string, string>("LKA","+94"),  // Sri Lanka
                new Tuple<string, string>("SYR","+963"), // Syria
                new Tuple<string, string>("TJK","+992"), // Tajikistan
                new Tuple<string, string>("THA","+66"),  // Thailand
                new Tuple<string, string>("TUR","+90"),  // Turkey
                new Tuple<string, string>("TKM","+993"), // Turkmenistan
                new Tuple<string, string>("ARE","+971"), // United Arab Emirates
                new Tuple<string, string>("UZB","+998"), // Uzbekistan
                new Tuple<string, string>("VNM","+84"),  // Vietnam
                new Tuple<string, string>("YEM","+967")  // Yemen
            };
            foreach (var country in countryCodes)
            {
                comboBox1.Items.Add($"{country.Item1} {country.Item2}");
            }
        }
    }
}
