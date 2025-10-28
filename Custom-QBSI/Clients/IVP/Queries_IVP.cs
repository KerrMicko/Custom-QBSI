using QBFC16Lib;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Custom_QBSI.Clients.IVP.DataClass_IVP;

namespace Custom_QBSI.Clients.IVP
{
    public class Queries_IVP
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

                    string invoiceQuery = "SELECT CustomFieldTIN, CustomFieldJOBORDERNO, CustomFieldPWDNO, CustomerRefFullName, CustomFieldBUSINESSSTYLE, CustomFieldSTORECODE, PONumber, " +
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
                                    //DrNo = reader["CustomFieldDRNO"] != DBNull.Value ? reader["CustomFieldDRNO"].ToString() : string.Empty,
                                    JobOrderNo = reader["CustomFieldJOBORDERNO"] != DBNull.Value ? reader["CustomFieldJOBORDERNO"].ToString() : string.Empty,
                                    //SONumber = reader["CustomFieldSONO"] != DBNull.Value ? reader["CustomFieldSONO"].ToString() : string.Empty,
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
                                                Amount = lineReader["InvoiceLineAmount"] != DBNull.Value ? Convert.ToDouble(lineReader["InvoiceLineAmount"]) : 0,
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

        public static List<ReceivePaymentData> GetReceivePaymentData(string refNumber)
        {
            QBSessionManager sessionManager = new QBSessionManager();
            List<ReceivePaymentData> payments = new List<ReceivePaymentData>();

            try
            {
                string AppName = "QBRP";
                LogDataSync($"Opening QuickBooks session for ReceivePayment RefNumber: {refNumber}");

                // Start QuickBooks session
                sessionManager.OpenConnection("", AppName);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);

                // Create request message
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                // Build ReceivePaymentQuery request
                IReceivePaymentQuery paymentQuery = requestMsgSet.AppendReceivePaymentQueryRq();
                paymentQuery.IncludeLineItems.SetValue(true);


                paymentQuery.ORTxnQuery.TxnFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNumber);
                paymentQuery.ORTxnQuery.TxnFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcStartsWith);



                LogDataSync("Sending ReceivePaymentQuery request...");
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IReceivePaymentRetList paymentList = response.Detail as IReceivePaymentRetList;

                if (paymentList != null && paymentList.Count > 0)
                {
                    LogDataSync($"Found {paymentList.Count} ReceivePayment record(s) for RefNumber: {refNumber}");

                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        IReceivePaymentRet qbPayment = paymentList.GetAt(i);

                        ReceivePaymentData payment = new ReceivePaymentData
                        {
                            CustomerName = qbPayment?.CustomerRef?.FullName?.GetValue() ?? string.Empty,
                            RefNumber = qbPayment?.RefNumber?.GetValue() ?? string.Empty,
                            TxnDate = qbPayment?.TxnDate?.GetValue() ?? DateTime.MinValue,
                            TotalAmount = qbPayment?.TotalAmount?.GetValue() ?? 0,
                            LineItems = new List<ReceivePaymentLineItem>()
                        };

                        if (qbPayment.AppliedToTxnRetList != null)
                        {
                            for (int j = 0; j < qbPayment.AppliedToTxnRetList.Count; j++)
                            {
                                IAppliedToTxnRet applyPayment = qbPayment.AppliedToTxnRetList.GetAt(j);

                                ReceivePaymentLineItem lineItem = new ReceivePaymentLineItem
                                {
                                    AppliedToTxnRefNumber = applyPayment?.RefNumber?.GetValue() ?? string.Empty,
                                    AppliedToTxnAmount = applyPayment?.Amount?.GetValue() != null ? (decimal)applyPayment.Amount.GetValue() : 0
                                };

                                payment.LineItems.Add(lineItem);
                            }
                        }


                        payments.Add(payment);
                    }
                }
                else
                {
                    LogDataSync($"No ReceivePayment found for RefNumber: {refNumber}");
                }

                sessionManager.EndSession();
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                LogDataSync($"ERROR while getting ReceivePayment {refNumber}: {ex}");
                try
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                catch { }
            }

            return payments;
        }





        public void UpdateACNoAndDateIssued(string acNo, DateTime dateIssued)
        {
            string connectionString = AccessDatabase.GetAccessConnectionString();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO DetailedIVP (ACNO, DateIssued) VALUES (?, ?)";

                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", acNo);
                    command.Parameters.AddWithValue("?", dateIssued.ToString("MM/dd/yyyy"));
                    // store as string since DateIssued column is Short Text

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record saved successfully!",
                                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to save record.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public (string acNo, DateTime? dateIssued) RetrieveACNoAndDateIssued()
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
            {
                connection.Open();

                string selectQuery = "SELECT TOP 1 ACNO, DateIssued FROM DetailedIVP ORDER BY ID DESC";
                // Get the latest saved record

                using (OleDbCommand selectCommand = new OleDbCommand(selectQuery, connection))
                using (OleDbDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string acNo = reader["ACNO"].ToString();

                        // Safely parse DateIssued (since it's stored as short text)
                        DateTime? dateIssued = null;
                        string dateStr = reader["DateIssued"].ToString();

                        if (!string.IsNullOrWhiteSpace(dateStr) &&
                            DateTime.TryParse(dateStr, out DateTime parsedDate))
                        {
                            dateIssued = parsedDate;
                        }

                        return (acNo, dateIssued);
                    }
                    else
                    {
                        return (string.Empty, null);
                    }
                }
            }
        }

        public void UpdateSignatory(string preparedBy, string checkedBy, string approvedBy)
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
                {
                    connection.Open();

                    string updateQuery = "UPDATE Signatory_IVP SET PreparedBy = ?, CheckedBy = ?, ApprovedBy = ? WHERE ID = 1";
                    using (OleDbCommand updateCommand = new OleDbCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("?", preparedBy);
                        updateCommand.Parameters.AddWithValue("?", checkedBy);
                        updateCommand.Parameters.AddWithValue("?", approvedBy);
                        updateCommand.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                MessageBox.Show("Signatory saved to database.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving signatory to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public (string preparedBy, string checkedBy, string approvedBy) RetrieveSignatory()
        {
            string accessConnectionString = AccessDatabase.GetAccessConnectionString();

            using (OleDbConnection connection = new OleDbConnection(accessConnectionString))
            {
                connection.Open();

                string selectQuery = "SELECT PreparedBy, CheckedBy, ApprovedBy FROM Signatory_IVP WHERE ID = 1";
                using (OleDbCommand selectCommand = new OleDbCommand(selectQuery, connection))
                using (OleDbDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string preparedBy = reader["PreparedBy"].ToString();
                        string checkedBy = reader["CheckedBy"].ToString();
                        string approvedBy = reader["ApprovedBy"].ToString();
                        connection.Close();
                        return (preparedBy, checkedBy, approvedBy);
                    }
                    else
                    {
                        // Return empty strings or handle as needed if no record is found
                        connection.Close();
                        return (string.Empty, string.Empty, string.Empty);
                    }
                }
            }
        }

        private static void LogDataSync(string message)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datasync_log.txt");
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            try
            {
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Fail silently if logging fails
            }
        }
    }
}
