using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Custom_QBSI
{
    public class AccessDatabase
    {
        public static string GetAccessConnectionString() // Access
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "CustomQBSIDatabase.accdb";
            string resourcePath = Path.Combine(baseDirectory, fileName);
            string accessConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={resourcePath};Persist Security Info=False;";

            return accessConnectionString;
        }

        public static string GetQBConnectionString()
        {
            string qbConnectionString = "DSN=QuickBooks Data;";
            //string qbConnectionString = "DSN=QuickBooks Data;UID=Admin;PWD=YourPassword;";
            return qbConnectionString;
        }

        public async Task FetchCreateAndSaveData(List<string> tableNames)
        {
            string odbcConnectionString = GetQBConnectionString();
            string accessConnectionString = GetAccessConnectionString();
           
            DeleteSpecifiedTablesData(tableNames);

            using (OdbcConnection odbcConnection = new OdbcConnection(odbcConnectionString))
            {
                try
                {
                    await odbcConnection.OpenAsync();

                    using (OleDbConnection accessConnection = new OleDbConnection(accessConnectionString))
                    {
                        await accessConnection.OpenAsync();

                        foreach (var tableName in tableNames)
                        {
                            // Ensure table name is enclosed in square brackets
                            string formattedTableName = $"[{tableName}]";

                            // Drop the table if it exists
                            await TryDropTable(accessConnection, tableName);

                            // Create ODBC command to select data from the QuickBooks Data DSN
                            OdbcCommand odbcCommand = odbcConnection.CreateCommand();
                            odbcCommand.CommandText = $"SELECT * FROM {tableName}";

                            // Execute the ODBC command to fetch the schema and data
                            using (OdbcDataReader reader = (OdbcDataReader)await odbcCommand.ExecuteReaderAsync())
                            {
                                // Create the table in Access if it doesn't exist
                                await CreateTableInAccess(accessConnection, formattedTableName, reader);

                                // Construct the Access SQL command for inserting data
                                OleDbCommand accessCommand = accessConnection.CreateCommand();
                                string columnNames = GetColumnNames(reader);
                                string parameterNames = GetParameterNames(reader);
                                accessCommand.CommandText = $"INSERT INTO {formattedTableName} ({columnNames}) VALUES ({parameterNames})";

                                // Add parameters to the Access command
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    accessCommand.Parameters.Add(new OleDbParameter($"@param{i}", reader.GetFieldType(i)));
                                }

                                // Transfer data row by row
                                while (await reader.ReadAsync())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        accessCommand.Parameters[$"@param{i}"].Value = reader.GetValue(i);
                                    }

                                    try
                                    {
                                        await accessCommand.ExecuteNonQueryAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"An error occurred while inserting data into {formattedTableName}: {ex.Message}");
                                        return;
                                    }
                                }
                            }
                        }
                        accessConnection.Close();
                    }
                    odbcConnection.Close();

                    MessageBox.Show("Data transfer completed successfully.");
                }
                catch (OdbcException ex)
                {
                    MessageBox.Show("ODBC Exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching data: " + ex.Message);
                }
            }
        }

        // Method to create a table in Access
        private async Task CreateTableInAccess(OleDbConnection accessConnection, string tableName, OdbcDataReader reader)
        {
            try
            {
                // Build CREATE TABLE SQL command dynamically based on the schema of OdbcDataReader
                List<string> columnDefinitions = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = $"[{reader.GetName(i)}]";
                    string columnType = GetAccessDataType(reader.GetFieldType(i));
                    columnDefinitions.Add($"{columnName} {columnType}");
                }

                string createTableQuery = $"CREATE TABLE {tableName} ({string.Join(", ", columnDefinitions)})";

                using (OleDbCommand command = new OleDbCommand(createTableQuery, accessConnection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating table {tableName}: {ex.Message}");
            }
        }

        // Helper method to map C# data types to Access data types
        private string GetAccessDataType(Type fieldType)
        {
            if (fieldType == typeof(int))
                return "INT";
            else if (fieldType == typeof(double) || fieldType == typeof(float))
                return "DOUBLE";
            else if (fieldType == typeof(decimal))
                return "DOUBLE";
            else if (fieldType == typeof(DateTime))
                return "DATETIME";
            else if (fieldType == typeof(bool))
                return "YESNO";
            else if (fieldType == typeof(string))
                return "TEXT(255)"; // Adjust length as needed
            else
                return "TEXT(255)"; // Default type
        }

        // Helper method to get column names from the OdbcDataReader
        private string GetColumnNames(OdbcDataReader reader)
        {
            List<string> columnNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames.Add($"[{reader.GetName(i)}]");
            }
            return string.Join(", ", columnNames);
        }

        // Helper method to get parameter names for the SQL command
        private string GetParameterNames(OdbcDataReader reader)
        {
            List<string> parameterNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                parameterNames.Add($"@param{i}");
            }
            return string.Join(", ", parameterNames);
        }

        private async Task TryDropTable(OleDbConnection accessConnection, string tableName)
        {
            try
            {
                // Attempt to drop the table
                string dropTableQuery = $"DROP TABLE [{tableName}]";
                using (OleDbCommand dropCommand = new OleDbCommand(dropTableQuery, accessConnection))
                {
                    await dropCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"Table {tableName} dropped successfully.");
                }
            }
            catch (OleDbException ex)
            {
                // Handle the case where the table does not exist
                if (ex.Errors.Count > 0 && ex.Errors[0].SQLState == "42S02") // Table not found error code
                {
                    Console.WriteLine($"Table {tableName} does not exist. Skipping drop.");
                }
                else
                {
                    Console.WriteLine($"An error occurred while dropping table {tableName}: {ex.Message}");
                }
            }
        }

        private void DeleteSpecifiedTablesData(List<string> tableNames)
        {
            string accessConnectionString = GetAccessConnectionString();

            using (OleDbConnection accessConnection = new OleDbConnection(accessConnectionString))
            {
                accessConnection.Open();

                // Predefined list of table names to delete
                //List<string> tableNames = new List<string> { "Invoice", "InvoiceLine", "InvoiceLinkedTxn" };

                foreach (var tableName in tableNames)
                {
                    // Construct the SQL command to delete all data from the table
                    OleDbCommand accessCommand = accessConnection.CreateCommand();
                    accessCommand.CommandText = $"DELETE FROM [{tableName}]";

                    try
                    {
                        accessCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while deleting data from {tableName}: {ex.Message}");
                        return;
                    }
                }
                accessConnection.Close();
            }

            Console.WriteLine("All data from specified tables has been deleted.");
        }

        // -------------------------------
        public void UpdateManualSeriesNumber(string tableName, int seriesNumber)
        {
            string query = $"UPDATE SeriesNumber_{tableName} SET Series = @SeriesNumber";

            using (OleDbConnection connection = new OleDbConnection(GetAccessConnectionString()))
            {
                try
                {
                    connection.Open();
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SeriesNumber", seriesNumber);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating {tableName} series number: {ex.Message}");
                }
            }
        }

        public int GetSeriesNumberFromDatabase(string tableName)
        {
            string accessConnectionString = GetAccessConnectionString();

            int currentSeries = 1; // Default to 1 if no value is found
            string query = $"SELECT Series FROM SeriesNumber_{tableName}"; // Replace 'SeriesTable' with your actual table name

            using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
            {
                try
                {
                    connection.Open();
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int series))
                        {
                            currentSeries = series;
                        }

                        /*if (result != null)
                        {
                            currentSeries = (int)result;
                        }*/
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching {tableName} series number: {ex.Message}");
                }
            }

            return currentSeries;
        }
    }
}
