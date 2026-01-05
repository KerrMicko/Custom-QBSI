using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Custom_QBSI.Clients.NHC.Dataclass_NHC;

namespace Custom_QBSI.Clients.NHC
{
    public class Queries_NHC
    {
        public List<InvoiceData> GetInvoiceData(string refNumber)
        {
            List<InvoiceData> invoices = new List<InvoiceData>();
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
                {
                    connection.Open();

                    string invoiceQuery = "SELECT CustomFieldTIN, CustomFieldDRNO, CustomFieldJOBORDERNO, CustomFieldPWDNO, CustomerRefFullName, CustomFieldBUSINESSSTYLE, CustomFieldSTORECODE, PONumber, " +
                                            "BillAddressBlockAddr1, BillAddressBlockAddr2, BillAddressBlockAddr3, BillAddressBlockAddr4, BillAddressBlockAddr5, " +
                                            "TxnDate, TermsRefFullName, DueDate, SalesRepRefFullName, Refnumber, ShipAddressBlockAddr1 , ShipAddressBlockAddr2, ShipAddressBlockAddr3, ShipAddressBlockAddr4, ShipAddressBlockAddr5  " +
                                            "FROM Invoice WHERE RefNumber = ?";

                    using (OleDbCommand command = new OleDbCommand(invoiceQuery, connection))
                    {
                        command.Parameters.AddWithValue("?", refNumber);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InvoiceData newInvoiceItem = new InvoiceData
                                {
                                    TINNO = reader["CustomFieldTIN"] != DBNull.Value ? reader["CustomFieldTIN"].ToString() : string.Empty,
                                    PwdNo = reader["CustomFieldPWDNO"] != DBNull.Value ? reader["CustomFieldPWDNO"].ToString() : string.Empty,
                                    DrNo = reader["CustomFieldDRNO"] != DBNull.Value ? reader["CustomFieldDRNO"].ToString() : string.Empty,
                                    JobOrderNo = reader["CustomFieldJOBORDERNO"] != DBNull.Value ? reader["CustomFieldJOBORDERNO"].ToString() : string.Empty,
                                    BusinessStyle = reader["CustomFieldBUSINESSSTYLE"] != DBNull.Value ? reader["CustomFieldBUSINESSSTYLE"].ToString() : string.Empty,
                                    StoreCode = reader["CustomFieldSTORECODE"] != DBNull.Value ? reader["CustomFieldSTORECODE"].ToString() : string.Empty,
                                    CustomerName = reader["CustomerRefFullName"] != DBNull.Value ? reader["CustomerRefFullName"].ToString() : string.Empty,
                                    PONumber = reader["PONumber"] != DBNull.Value ? reader["PONumber"].ToString() : string.Empty,
                                    BillAddress1 = reader["BillAddressBlockAddr1"] != DBNull.Value ? reader["BillAddressBlockAddr1"].ToString() : string.Empty,
                                    BillAddress2 = reader["BillAddressBlockAddr2"] != DBNull.Value ? reader["BillAddressBlockAddr2"].ToString() : string.Empty,
                                    BillAddress3 = reader["BillAddressBlockAddr3"] != DBNull.Value ? reader["BillAddressBlockAddr3"].ToString() : string.Empty,
                                    BillAddress4 = reader["BillAddressBlockAddr4"] != DBNull.Value ? reader["BillAddressBlockAddr4"].ToString() : string.Empty,
                                    BillAddress5 = reader["BillAddressBlockAddr5"] != DBNull.Value ? reader["BillAddressBlockAddr5"].ToString() : string.Empty,
                                    ShipAddress1 = reader["ShipAddressBlockAddr1"] != DBNull.Value ? reader["ShipAddressBlockAddr1"].ToString() : string.Empty,
                                    ShipAddress2 = reader["ShipAddressBlockAddr2"] != DBNull.Value ? reader["ShipAddressBlockAddr2"].ToString() : string.Empty,
                                    ShipAddress3 = reader["ShipAddressBlockAddr3"] != DBNull.Value ? reader["ShipAddressBlockAddr3"].ToString() : string.Empty,
                                    ShipAddress4 = reader["ShipAddressBlockAddr4"] != DBNull.Value ? reader["ShipAddressBlockAddr4"].ToString() : string.Empty,
                                    ShipAddress5 = reader["ShipAddressBlockAddr5"] != DBNull.Value ? reader["ShipAddressBlockAddr5"].ToString() : string.Empty,
                                    //BillCity = reader["BillAddressCity"] != DBNull.Value ? reader["BillAddressCity"].ToString() : string.Empty,
                                    TxnDate = reader["TxnDate"] != DBNull.Value ? Convert.ToDateTime(reader["TxnDate"]).Date : DateTime.MinValue,
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]).Date : DateTime.MinValue,
                                    Terms = reader["TermsRefFullName"] != DBNull.Value ? reader["TermsRefFullName"].ToString() : string.Empty,
                                    RefNumber = reader["RefNumber"] != DBNull.Value ? reader["RefNumber"].ToString() : string.Empty,
                                    Salesrep = reader["SalesRepRefFullName"] != DBNull.Value ? reader["SalesRepRefFullName"].ToString() : string.Empty,

                                    LineItems = new List<InvoiceLineItem>()  // Initialize line items list 
                                };

                                // Additional query to fetch line items for the current invoice
                                string lineItemQuery = "SELECT InvoiceLineQuantity, SalesRepRefFullName, CustomFieldInvoiceLineOther1, InvoiceLineDesc, ItemSalesTaxRefFullName, SalesTaxTotal, BalanceRemaining, InvoiceLineUnitOfMeasure, InvoiceLineSalesTaxCodeRefFullName, InvoiceLineRate, InvoiceLineAmount, InvoiceLineItemRefListID " +
                                                "FROM InvoiceLine WHERE RefNumber = ?";

                                using (OleDbCommand lineItemCommand = new OleDbCommand(lineItemQuery, connection))
                                {
                                    lineItemCommand.Parameters.AddWithValue("?", refNumber);

                                    using (OleDbDataReader lineReader = lineItemCommand.ExecuteReader())
                                    {
                                        while (lineReader.Read())
                                        {
                                            string itemRefListId = lineReader["InvoiceLineItemRefListID"] as string;

                                            InvoiceLineItem lineItem = new InvoiceLineItem
                                            {
                                                Quantity = lineReader["InvoiceLineQuantity"] != DBNull.Value ? Convert.ToDecimal(lineReader["InvoiceLineQuantity"]) : 0,
                                                Description = lineReader["InvoiceLineDesc"] as string,
                                                Tax = lineReader["InvoiceLineSalesTaxCodeRefFullName"] as string,
                                                ExpirationDate = lineReader["CustomFieldInvoiceLineOther1"] as string,
                                                UnitOfMeasure = lineReader["InvoiceLineUnitOfMeasure"] as string,
                                                TaxesName = lineReader["ItemSalesTaxRefFullName"] as string,
                                                Rate = lineReader["InvoiceLineRate"] != DBNull.Value ? Convert.ToDecimal(lineReader["InvoiceLineRate"]) : 0,
                                                Amount = lineReader["InvoiceLineAmount"] != DBNull.Value ? Convert.ToDecimal(lineReader["InvoiceLineAmount"]) : 0,
                                                TotalAmount = lineReader["BalanceRemaining"] != DBNull.Value ? Convert.ToDecimal(lineReader["BalanceRemaining"]) : 0,
                                                SalesTaxTotal = lineReader["SalesTaxTotal"] != DBNull.Value ? Convert.ToDecimal(lineReader["SalesTaxTotal"]) : 0,
                                                Salesrep = lineReader["SalesRepRefFullName"] as string,
                                            };

                                            if (!string.IsNullOrEmpty(itemRefListId))
                                            {
                                                string itemNameQuery = "SELECT FullName FROM Item WHERE ListID = ?";
                                                using (OleDbCommand itemCommand = new OleDbCommand(itemNameQuery, connection))
                                                {
                                                    itemCommand.Parameters.AddWithValue("?", itemRefListId);

                                                    object result = itemCommand.ExecuteScalar();
                                                    if (result != null && result != DBNull.Value)
                                                    {
                                                        lineItem.ItemName = result.ToString();  // Make sure you have ItemName property in your InvoiceLineItem class
                                                    }
                                                }
                                            }

                                            newInvoiceItem.LineItems.Add(lineItem);
                                        }
                                    }
                                }

                                invoices.Add(newInvoiceItem);  // Add the new invoice with line items to the list.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving data from Access database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return invoices;
        }

        public void UpdateSignatory(string name)
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Signatory_NHC SET CashierName = ? WHERE ID = 1";
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

                string selectQuery = "SELECT CashierName FROM Signatory_NHC WHERE ID = 1";
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

        /*public void UpdateSignatory_DR(string address, string terms, string storeCode, string po, string tin)
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
                {
                    connection.Open();

                    string updateQuery = @"
                        UPDATE Signatory_NHC 
                        SET 
                            Address = ?, 
                            Terms = ?, 
                            StoreCode = ?, 
                            PONo = ?, 
                            TINNo = ?
                        WHERE ID = 1";

                    using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("?", address ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("?", terms ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("?", storeCode ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("?", po ?? (object)DBNull.Value);
                        updateCommand.Parameters.AddWithValue("?", tin ?? (object)DBNull.Value);

                        updateCommand.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                MessageBox.Show("Details saved to database.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving name to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        public void SaveOrUpdateHistoryDR(string refNumber, string address, string terms, string storeCode, string poNumber, string tin, string businessStyle, string note, string customerName)
        {
            string connString = AccessDatabase.GetAccessConnectionString();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();

                object CheckNull(string value)
                {
                    if (string.IsNullOrWhiteSpace(value)) return DBNull.Value;
                    return value;
                }

                string checkQuery = "SELECT COUNT(*) FROM HistoryDRDetails WHERE [RefNumber] = ?";
                using (OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@RefNumber", refNumber);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // UPDATE Query - Added [CustomerName]
                        string updateQuery = @"UPDATE HistoryDRDetails 
                                       SET [Address] = ?, 
                                           [Terms] = ?, 
                                           [StoreCode] = ?, 
                                           [PONumber] = ?, 
                                           [TIN] = ?, 
                                           [BusinessStyle] = ?, 
                                           [Note] = ?,
                                           [CustomerName] = ?
                                       WHERE [RefNumber] = ?";

                        using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@Address", CheckNull(address));
                            updateCmd.Parameters.AddWithValue("@Terms", CheckNull(terms));
                            updateCmd.Parameters.AddWithValue("@StoreCode", CheckNull(storeCode));
                            updateCmd.Parameters.AddWithValue("@PONumber", CheckNull(poNumber));
                            updateCmd.Parameters.AddWithValue("@TIN", CheckNull(tin));
                            updateCmd.Parameters.AddWithValue("@BusinessStyle", CheckNull(businessStyle));
                            updateCmd.Parameters.AddWithValue("@Note", CheckNull(note));

                            // New Parameter
                            updateCmd.Parameters.AddWithValue("@CustomerName", CheckNull(customerName));

                            // WHERE clause
                            updateCmd.Parameters.AddWithValue("@RefNumber", refNumber);

                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // INSERT Query - Added [CustomerName]
                        string insertQuery = @"INSERT INTO HistoryDRDetails 
                                      ([RefNumber], [Address], [Terms], [StoreCode], [PONumber], [TIN], [BusinessStyle], [Note], [CustomerName]) 
                                      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@RefNumber", refNumber);
                            insertCmd.Parameters.AddWithValue("@Address", CheckNull(address));
                            insertCmd.Parameters.AddWithValue("@Terms", CheckNull(terms));
                            insertCmd.Parameters.AddWithValue("@StoreCode", CheckNull(storeCode));
                            insertCmd.Parameters.AddWithValue("@PONumber", CheckNull(poNumber));
                            insertCmd.Parameters.AddWithValue("@TIN", CheckNull(tin));
                            insertCmd.Parameters.AddWithValue("@BusinessStyle", CheckNull(businessStyle));
                            insertCmd.Parameters.AddWithValue("@Note", CheckNull(note));

                            // New Parameter
                            insertCmd.Parameters.AddWithValue("@CustomerName", CheckNull(customerName));

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public HistoryDRDetails GetHistoryDRDetails(string refNumber)
        {
            string connectionString = AccessDatabase.GetAccessConnectionString();

            if (string.IsNullOrWhiteSpace(refNumber))
                return null;

            HistoryDRDetails details = null;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT Address, Terms, StoreCode, PONumber, TIN, BusinessStyle, Note, CustomerName 
                             FROM HistoryDRDetails
                             WHERE RefNumber = @RefNumber";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RefNumber", refNumber);

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            details = new HistoryDRDetails
                            {
                                Address = reader["Address"]?.ToString(),
                                Terms = reader["Terms"]?.ToString(),
                                StoreCode = reader["StoreCode"]?.ToString(),
                                PONumber = reader["PONumber"]?.ToString(),
                                TIN = reader["TIN"]?.ToString(),
                                BusinessStyle = reader["BusinessStyle"]?.ToString(),
                                Note = reader["Note"]?.ToString(),
                                Cname = reader["CustomerName"]?.ToString()
                            };
                        }
                    }
                }
            }

            return details;
        }


        public string[] RetrieveSignatory_DR()
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
                {
                    connection.Open();

                    string selectQuery = @"
                        SELECT [Address], [Terms], [StoreCode], [PONo], [TINNo]
                        FROM Signatory_NHC 
                        WHERE ID = 1";

                    using (OleDbCommand selectCommand = new OleDbCommand(selectQuery, connection))
                    using (OleDbDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new string[]
                            {
                                reader["Address"]?.ToString(),
                                reader["Terms"]?.ToString(),
                                reader["StoreCode"]?.ToString(),
                                reader["PONo"]?.ToString(),
                                reader["TINNo"]?.ToString()
                            };
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving data from Access database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.ToString());
            }

            return new string[0]; // empty if no record found
        }
    }
}