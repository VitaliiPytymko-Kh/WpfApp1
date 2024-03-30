using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Add_New_Product.xaml
    /// </summary>
    public partial class AddNewProduct : Window
    {
        public event EventHandler ProductAdded;
        SqlConnection conn = null;
        string cs = "";

        public AddNewProduct()
        {
            InitializeComponent();
            conn = new SqlConnection();
            cs = @"Data Source =(localdb)\MSSQLLocalDB; Initial Catalog = Sklad; Integrated Security =SSPI; ";
            conn.ConnectionString = cs;
            conn.Open();
            FillComboBox();
        }


        private void FillComboBox()
        {
           
            DataTable productTypes = GetUniqueProductTypes();
            ProductTypeComboBox.ItemsSource = productTypes.DefaultView;
        }
        private DataTable GetUniqueProductTypes()
        {
            DataTable productTypes = new DataTable();

            try
            {
                string query = "SELECT DISTINCT Тип FROM Товар";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(productTypes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выполнении запроса: " + ex.Message);
            }

            return productTypes;
        }

        private void OnProductAdded()
        {
            ProductAdded?.Invoke(this, EventArgs.Empty);
        }

        private void AddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string productName = ProductNameTextBox.Text;

                
                string selectedType = ProductTypeComboBox.Text;

               
                if (selectedType == null)
                {
                    MessageBox.Show("Please select a product type.");
                    return;
                }

                
                string query = "INSERT INTO Товар (Название, Тип) VALUES (@ProductName, @ProductType)";

               
                SqlCommand command = new SqlCommand(query, conn);

               
                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@ProductType", selectedType);

             
                int rowsAffected = command.ExecuteNonQuery();

                
                if (rowsAffected > 0)
                {
                    MessageBox.Show("New product added successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to add new product.");
                }
                OnProductAdded();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding new product: " + ex.Message);
            }
        }
      
    }
}
