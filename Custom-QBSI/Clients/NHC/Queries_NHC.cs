using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Custom_QBSI.Clients.NHC.Dataclass_NHC;
using static Custom_QBSI.Clients.NHC.AccessToDatabase_NHC;

namespace Custom_QBSI.Clients.NHC
{
    public class Queries_NHC
    {
        public List<InvoiceData> GetInvoiceData(string refNumber)
        {
            List<InvoiceData> invoices = new List<InvoiceData>();
            string accessConnectionString = GetAccessConnectionString();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
                {
                    connection.Open();

                    string invoiceQuery = "SELECT CustomFieldTIN, CustomFieldDRNO, CustomFieldJOBORDERNO, CustomFieldPWDNO, CustomerRefFullName, PONumber, " +
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
                                string lineItemQuery = "SELECT InvoiceLineQuantity, SalesRepRefFullName, InvoiceLineDesc, ItemSalesTaxRefFullName, SalesTaxTotal, BalanceRemaining, InvoiceLineUnitOfMeasure, InvoiceLineSalesTaxCodeRefFullName, InvoiceLineRate, InvoiceLineAmount, InvoiceLineItemRefListID " +
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
    }
}
