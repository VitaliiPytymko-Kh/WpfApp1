using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        SqlConnection conn = null;
        string cs = "";
        DataTable table = null;
        SqlDataReader reader = null;
        SqlCommand cmd = null;
        string text = null;
        private DataTable previousQueryResult = null;
        private string currentTableName;


        public MainWindow()
        {
            InitializeComponent();
            conn = new SqlConnection();
            cs = @"Data Source =(localdb)\MSSQLLocalDB; Initial Catalog = Sklad; Integrated Security =SSPI; ";
            conn.ConnectionString = cs;
            conn.Open();

        }
        private void ExecuteQueryAndRefreshDataGrid(string query)
        {
            try
            {
                if (reader != null)
                    reader.Close(); 

                TextBox1.Text = query;

                cmd = new SqlCommand(query, conn);

                table = new DataTable();

                reader = cmd.ExecuteReader();
                int line = 0;

                do
                {
                    while (reader.Read())
                    {
                        if (line == 0)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                table.Columns.Add(reader.GetName(i));
                            }
                            line++;
                        }
                        DataRow row = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }
                        table.Rows.Add(row);
                    }
                } while (reader.NextResult());

                DataView Source = new DataView(table);
                DataGrid1.ItemsSource = Source;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close(); 
            }
        }

        private void ShowProductWithMaxQuantity()
        {
            try
            {
                if (reader != null) reader.Close();

                string query = @"
            SELECT TOP 1 Товар.Название, SUM(Поставки.Количество) AS Общее_количество
            FROM Товар
            JOIN Поставки ON Товар.ТоварID = Поставки.ТоварID
            GROUP BY Товар.ТоварID, Товар.Название
            ORDER BY Общее_количество DESC;";

                ExecuteQueryAndRefreshDataGrid(query);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ShowProductWithMinQuantity()
        {
            try
            {
                if (reader != null) reader.Close();

                string query = @"
            SELECT TOP 1 Товар.Название, SUM(Поставки.Количество) AS Общее_количество
            FROM Товар
            JOIN Поставки ON Товар.ТоварID = Поставки.ТоварID
            GROUP BY Товар.ТоварID, Товар.Название
            ORDER BY Общее_количество ASC;";

                ExecuteQueryAndRefreshDataGrid(query);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ShowProductWithMinCost()
        {
            try
            {
                if (reader != null) reader.Close();

                string query = @"
            SELECT TOP 1 Товар.Название, MIN(Поставки.Себестоимость) AS Минимальная_себестоимость
            FROM Товар
            JOIN Поставки ON Товар.ТоварID = Поставки.ТоварID
            GROUP BY Товар.ТоварID, Товар.Название
            ORDER BY Минимальная_себестоимость ASC;";

                ExecuteQueryAndRefreshDataGrid(query);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ShowProductWithMaxCost()
        {
            try
            {
                if (reader != null) reader.Close();

                string query = @"
            SELECT TOP 1 Товар.Название, MAX(Поставки.Себестоимость) AS Максимальная_себестоимость
            FROM Товар
            JOIN Поставки ON Товар.ТоварID = Поставки.ТоварID
            GROUP BY Товар.ТоварID, Товар.Название
            ORDER BY Максимальная_себестоимость DESC;";

                ExecuteQueryAndRefreshDataGrid(query);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = TextBox1.Text;

                if (string.IsNullOrEmpty(query))
                {
                    System.Windows.MessageBox.Show("Enter a valid SQL query.");
                    return;
                }

                previousQueryResult = table;

                ExecuteQueryAndRefreshDataGrid(query);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.ItemsSource = null;

            TextBlock1.Text = "Enter your request";

            ExecuteQueryAndRefreshDataGrid("SELECT * FROM Товар");

            currentTableName = "Товар";
            try
            {
                int selectedRowId = GetSelectedRowId(DataGrid1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ExecuteQueryAndRefreshDataGrid("SELECT DISTINCT Тип FROM Товар");
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            DataGrid1.ItemsSource = null;

            TextBlock1.Text = "Enter your request";

            ExecuteQueryAndRefreshDataGrid("SELECT * FROM Поставщик");

            currentTableName = "Поставщик";
            try
            {
                int selectedRowId = GetSelectedRowId(DataGrid1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
          
        }
       
        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            ShowProductWithMaxQuantity();
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            ShowProductWithMinQuantity();
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            ShowProductWithMinCost();
        }
        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            ShowProductWithMaxCost();
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            // Отображение всех поставок
            ExecuteQueryAndRefreshDataGrid("SELECT * FROM Поставки");
        }

        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            QueryWindow1 window1 = new();
            window1.ShowDialog();
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
           TextBox1.Clear();

            if (previousQueryResult != null)
            {
                DataView Source = new DataView(previousQueryResult);
                DataGrid1.ItemsSource = Source;
            }
        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddNewProduct addNewProduct = new();
                addNewProduct.ProductAdded += AddNewProduct_ProductAdded;
                addNewProduct.ShowDialog();
                RefreshProductDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding new product:" + ex.Message);
            }
        }

        private void AddNewProduct_ProductAdded(object? sender, EventArgs e)
        {
            RefreshProductDataGrid();
        }

        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string supplierName = AddSupplier.Text;

                if (string.IsNullOrEmpty(supplierName))
                {
                    MessageBox.Show("Please enter a supplier name.");
                    return;
                }

                string query = "INSERT INTO Поставщик (Название) VALUES (@SupplierName)";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@SupplierName", supplierName);

                    if (reader != null && !reader.IsClosed)
                    {
                        reader.Close();
                    }

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("New supplier added successfully.");
                        RefreshSupplierDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add new supplier.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding new supplier: " + ex.Message);
            }
        }

        private void RefreshSupplierDataGrid()
        {
            try
            {
                string query = "SELECT * FROM Поставщик";
                ExecuteQueryAndRefreshDataGrid(query);

            }
            catch(Exception ex) 
            {
                MessageBox.Show("Error refreshing supplier data grid:" + ex.Message);
            }
        }

        private void RefreshProductDataGrid()
        {
            ExecuteQueryAndRefreshDataGrid("SELECT * FROM Товар");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tableName = GetSelectedTableName();
                int selectedId = GetSelectedRowId(DataGrid1);

                if (selectedId == -1)
                {
                    MessageBox.Show("No row selected.");
                    return;
                }

                // Закрываем DataReader, если он открыт
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                if (tableName == "Товар")
                {
                    string query = "DELETE FROM Товар WHERE ТоварID = @ID";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@ID", selectedId);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Row deleted successfully.");
                            RefreshDataGrid(tableName);
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete row.");
                        }
                    }
                }
                else if (tableName == "Поставщик")
                {
                    string query = "DELETE FROM Поставщик WHERE ПоставщикID = @ID";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@ID", selectedId);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Row deleted successfully.");
                            RefreshDataGrid(tableName);
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete row.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unknown table name.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting row: " + ex.Message);
            }
        }


        private void RefreshDataGrid(string tableName)
        {
            try
            {
                string query = "SELECT *FROM " + tableName;
                ExecuteQueryAndRefreshDataGrid(query);
            }
            catch(Exception ex) { MessageBox.Show("Error refreshing data grid:" + ex.Message);  }
        }

        private int GetSelectedRowId(DataGrid dataGrid)
        {
            if (dataGrid.SelectedItem != null)
            {
                DataRowView rowView = (DataRowView)dataGrid.SelectedItem;
                if (currentTableName == "Товар")
                {
                    int selectedRowId = Convert.ToInt32(rowView["ТоварID"]);
                    return selectedRowId;
                }
                else if (currentTableName == "Поставщик")
                {
                    int selectedRowId = Convert.ToInt32(rowView["ПоставщикID"]);
                    return selectedRowId;
                }
                else
                {
                    throw new Exception("Unknown table name.");
                }
            }
            else
            {
                return -1;
               
            }
        }

        private string GetSelectedTableName()
        {
            return currentTableName;
        }
    }
}