using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Custom_QBSI.Clients.MET.Dataclass_MET;
using System.Windows.Forms;

namespace Custom_QBSI.Clients.MET
{
    public class Queries_MET
    {
        public void UpdateSignatory(string name)
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Signatory_MET SET CashierName = ? WHERE ID = 1";
                    using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("?", name);
                        //updateCommand.Parameters.AddWithValue("?", approvedBy);
                        updateCommand.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                MessageBox.Show("Name saved to database.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving name to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string RetrieveSignatory()
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
            {
                connection.Open();

                string selectQuery = "SELECT CashierName FROM Signatory_MET WHERE ID = 1";
                using (OleDbCommand selectCommand = new OleDbCommand(selectQuery, connection))
                using (OleDbDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader["CashierName"].ToString();
                        connection.Close();
                        return name;
                    }
                    else
                    {
                        // Return empty strings or handle as needed if no record is found
                        connection.Close();
                        return string.Empty;
                    }
                }
            }
        }


    }
}
