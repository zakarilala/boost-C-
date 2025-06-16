using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Simple_CRUD
{
    public partial class Form1 : Form
    {
        private SqlConnection connection;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\ACER\\source\\repos\\Simple CRUD\\PersonDb.mdf\";Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            textBoxId.Visible = false;
            InitializeDatabaseConnection();
            LoadPersonsData();
            ConfigureDataGridView();
        }
        private void InitializeDatabaseConnection()
        {
            connection = new SqlConnection(connectionString);
        }

        private void LoadPersonsData()
        {
            using (SqlConnection loadConnection = new SqlConnection(connectionString))
            {
                try
                {
                    loadConnection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Persons", loadConnection);
                    DataTable newDataTable = new DataTable();
                    adapter.Fill(newDataTable);
                    dataGridViewPersons.DataSource = newDataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur de chargement des donnees: {ex.Message}");
                }
            }
        }

        private void ConfigureDataGridView()
        {
            dataGridViewPersons.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewPersons.MultiSelect = false;
            dataGridViewPersons.ReadOnly = true;
            dataGridViewPersons.RowHeadersVisible = false;
            dataGridViewPersons.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewPersons.SelectionChanged += DataGridViewPersons_SelectionChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridViewPersons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DataGridViewPersons_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewPersons.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewPersons.SelectedRows[0];
                textBoxId.Text = selectedRow.Cells["Id"].Value.ToString();
                textBoxLastName.Text = selectedRow.Cells["LastName"].Value.ToString();
                textBoxFirstName.Text = selectedRow.Cells["FirstName"].Value.ToString();
                textBoxAge.Text = selectedRow.Cells["Age"].Value.ToString();
                textBoxAddress.Text = selectedRow.Cells["Address"].Value?.ToString() ?? string.Empty;
            }
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                try
                {
                    using (SqlConnection cmdConnection = new SqlConnection(connectionString))
                    {
                        cmdConnection.Open();
                        SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Persons (LastName, FirstName, Age, Address) " +
                            "VALUES (@LastName, @FirstName, @Age, @Address)", cmdConnection);

                        cmd.Parameters.AddWithValue("@LastName", textBoxLastName.Text);
                        cmd.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text);
                        cmd.Parameters.AddWithValue("@Age", int.Parse(textBoxAge.Text));
                        cmd.Parameters.AddWithValue("@Address", textBoxAddress.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClearInputs();
                            LoadPersonsData();
                            MessageBox.Show("Person added successfully!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding person: {ex.Message}");
                }
            }

        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBoxLastName.Text))
            {
                MessageBox.Show("Entrer votre NOM.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxFirstName.Text))
            {
                MessageBox.Show("Entrer votre PRENOM.");
                return false;
            }

            if (!int.TryParse(textBoxAge.Text, out int age) || age <= 0)
            {
                MessageBox.Show("Entrer un age valide.");
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            textBoxId.Clear();
            textBoxLastName.Clear();
            textBoxFirstName.Clear();
            textBoxAge.Clear();
            textBoxAddress.Clear();
        }

        private void buttonModify_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxId.Text))
            {
                MessageBox.Show("Selectionner la personne a modifier.");
                return;
            }

            if (ValidateInputs())
            {
                try
                {
                    using (SqlConnection cmdConnection = new SqlConnection(connectionString))
                    {
                        cmdConnection.Open();
                        SqlCommand cmd = new SqlCommand(
                            "UPDATE Persons SET LastName = @LastName, FirstName = @FirstName, " +
                            "Age = @Age, Address = @Address WHERE Id = @Id", cmdConnection);

                        cmd.Parameters.AddWithValue("@LastName", textBoxLastName.Text);
                        cmd.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text);
                        cmd.Parameters.AddWithValue("@Age", int.Parse(textBoxAge.Text));
                        cmd.Parameters.AddWithValue("@Address", textBoxAddress.Text);
                        cmd.Parameters.AddWithValue("@Id", int.Parse(textBoxId.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClearInputs();
                            LoadPersonsData();
                            MessageBox.Show("Mise à jour réussie !");
                        }
                        else
                        {
                            MessageBox.Show("Aucune ligne n'a été mise à jour.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la mise à jour de la personne: {ex.Message}");
                }
            }
        }

        private void buttonDelete_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxId.Text))
            {
                MessageBox.Show("Selectionner une personne à supprimer.");
                return;
            }

            if (MessageBox.Show("Est ce que vous ete sure de suprimer cette personne ?", "Confirm Delete",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection cmdConnection = new SqlConnection(connectionString))
                    {
                        cmdConnection.Open();
                        SqlCommand cmd = new SqlCommand(
                            "DELETE FROM Persons WHERE Id = @Id", cmdConnection);

                        cmd.Parameters.AddWithValue("@Id", int.Parse(textBoxId.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClearInputs();
                            LoadPersonsData();
                            MessageBox.Show("La personne a ete supprimer avec succes !");
                        }
                        else
                        {
                            MessageBox.Show("Aucune ligne n'a été supprimée.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression de la personne: {ex.Message}");
                }
            }
        }

        private void buttonCancel_Click_1(object sender, EventArgs e)
        {
            ClearInputs();
            dataGridViewPersons.ClearSelection();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
