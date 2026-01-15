using QBFC16Lib;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Custom_QBSI.Clients.MET.AltDataclass;

namespace Custom_QBSI.Clients.MET
{
    public class QBDataSync_MET
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

        public async Task FetchCreateAndSaveData(List<string> tableNames, string selectedYear)
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

                            string startDate = $"{{d '{selectedYear}-01-01'}}";
                            string endDate = $"{{d '{selectedYear}-12-31'}}";

                            if (tableName == "Invoice" || tableName == "InvoiceLine" || tableName == "InvoiceLinkedTxn")
                            {
                                odbcCommand.CommandText = $"SELECT * FROM {tableName} WHERE TxnDate >= {startDate} AND TxnDate <= {endDate}";
                            }
                            else
                            {
                                odbcCommand.CommandText = $"SELECT * FROM {tableName}";
                            }

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

        public static List<InvoiceData> GetInvoiceByRefNumber(string refNumber)
        {
            QBSessionManager sessionManager = new QBSessionManager();
            List<InvoiceData> invoices = new List<InvoiceData>();

            try
            {
                string AppName = "QBSI";
                LogDataSync($"Opening QuickBooks session for RefNumber: {refNumber}");

                // Start QuickBooks session
                sessionManager.OpenConnection("", AppName);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);

                // Create request message set
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                // Build InvoiceQuery request
                IInvoiceQuery invoiceQuery = requestMsgSet.AppendInvoiceQueryRq();
                invoiceQuery.IncludeLineItems.SetValue(true);
                invoiceQuery.IncludeLinkedTxns.SetValue(true);

                // --- IMPORTANT: REQUIRED to get Transaction-level Custom Fields ---
                invoiceQuery.OwnerIDList.Add("0");
                // -----------------------------------------------------------------

                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcStartsWith);
                invoiceQuery.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNumber);

                LogDataSync("Sending InvoiceQuery request...");
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IInvoiceRetList invoiceList = response.Detail as IInvoiceRetList;

                if (invoiceList != null && invoiceList.Count > 0)
                {
                    LogDataSync($"Found {invoiceList.Count} invoice(s) for RefNumber: {refNumber}");
                    for (int i = 0; i < invoiceList.Count; i++)
                    {
                        IInvoiceRet qbInvoice = invoiceList.GetAt(i);

                        InvoiceData invoiceData = new InvoiceData
                        {
                            RefNumber = qbInvoice?.RefNumber?.GetValue() ?? string.Empty,
                            TxnDate = qbInvoice?.TxnDate?.GetValue() ?? DateTime.MinValue,
                            CustomerName = qbInvoice?.CustomerRef?.FullName?.GetValue() ?? string.Empty,
                            BalanceRemaining = qbInvoice?.BalanceRemaining?.GetValue() ?? 0,
                            Subtotal = qbInvoice?.Subtotal?.GetValue() ?? 0,
                            TotalAmount = (qbInvoice?.Subtotal?.GetValue() ?? 0) + (qbInvoice?.SalesTaxTotal?.GetValue() ?? 0),
                            Terms = qbInvoice?.TermsRef?.FullName?.GetValue() ?? string.Empty,
                            DueDate = qbInvoice?.DueDate?.GetValue() ?? DateTime.MinValue,
                            PONumber = qbInvoice?.PONumber?.GetValue() ?? string.Empty,
                            TaxesName = qbInvoice?.ItemSalesTaxRef?.FullName?.GetValue() ?? string.Empty,

                            ShipAddress1 = qbInvoice?.ShipAddressBlock?.Addr1?.GetValue() ?? string.Empty,
                            ShipAddress2 = qbInvoice?.ShipAddressBlock?.Addr2?.GetValue() ?? string.Empty,
                            ShipAddress3 = qbInvoice?.ShipAddressBlock?.Addr3?.GetValue() ?? string.Empty,
                            ShipAddress4 = qbInvoice?.ShipAddressBlock?.Addr4?.GetValue() ?? string.Empty,
                            ShipAddress5 = qbInvoice?.ShipAddressBlock?.Addr5?.GetValue() ?? string.Empty,


                        };

                        // --- 1. Get Default Customer Custom Fields (Existing Logic) ---
                        var customerListID = qbInvoice.CustomerRef?.ListID?.GetValue();
                        if (!string.IsNullOrEmpty(customerListID))
                        {
                            // This fetches the "default" values from the Customer list
                            invoiceData.CustomerCustomFields = GetCustomerCustomFields(sessionManager, customerListID);
                            LogDataSync($"Fetched custom fields for customer {customerListID}");
                        }

                        // Ensure the dictionary is initialized so we don't crash on the next step
                        if (invoiceData.CustomerCustomFields == null)
                        {
                            invoiceData.CustomerCustomFields = new Dictionary<string, string>();
                        }

                        // --- 2. Get INVOICE SPECIFIC Custom Fields (The Fix) ---
                        if (qbInvoice.DataExtRetList != null)
                        {
                            for (int k = 0; k < qbInvoice.DataExtRetList.Count; k++)
                            {
                                IDataExtRet dataExt = qbInvoice.DataExtRetList.GetAt(k);
                                string fieldName = dataExt.DataExtName.GetValue();
                                string fieldValue = dataExt.DataExtValue.GetValue();

                                // Using Dictionary syntax:
                                // If "STORE CODE" exists, this overwrites it. 
                                // If it doesn't exist, this adds it.
                                invoiceData.CustomerCustomFields[fieldName] = fieldValue;

                                LogDataSync($"Processed Invoice Custom Field: {fieldName} = {fieldValue}");
                            }
                        }

                        // --- Line Items ---
                        if (qbInvoice.ORInvoiceLineRetList != null)
                        {
                            // Create a local cache to store ListID -> Abbreviation mappings for this invoice
                            Dictionary<string, string> uomCache = new Dictionary<string, string>();

                            for (int j = 0; j < qbInvoice.ORInvoiceLineRetList.Count; j++)
                            {
                                var orLine = qbInvoice.ORInvoiceLineRetList.GetAt(j);
                                if (orLine?.InvoiceLineRet != null)
                                {
                                    var line = orLine.InvoiceLineRet;

                                    // Get the ListID of the Unit of Measure Set used on this line
                                    string uomListID = line.OverrideUOMSetRef?.ListID?.GetValue();
                                    string finalUOM = line.UnitOfMeasure?.GetValue() ?? string.Empty;

                                    // If there is a UOM ListID, fetch the BaseUnitAbbreviation
                                    if (!string.IsNullOrEmpty(uomListID))
                                    {
                                        if (!uomCache.ContainsKey(uomListID))
                                        {
                                            uomCache[uomListID] = GetUOMAbbreviation(sessionManager, uomListID);
                                        }
                                        finalUOM = uomCache[uomListID];
                                    }

                                    var lineData = new InvoiceLineData
                                    {
                                        ItemName = line.ItemRef?.FullName?.GetValue() ?? string.Empty,
                                        Description = line.Desc?.GetValue() ?? string.Empty,
                                        Quantity = line.Quantity?.GetValue() ?? 0,
                                        UnitOfMeasure = finalUOM, // This will now be "kg", "pc", etc.
                                        Rate = (decimal?)(line.ORRate?.Rate?.GetValue()) ?? 0m,
                                        Amount = (decimal?)(line.Amount?.GetValue()) ?? 0m,
                                        TotalAmount = (decimal?)(line.Amount?.GetValue()) ?? 0m,
                                        ExpirationDate = line.Other1?.GetValue() ?? string.Empty,
                                        SkuCode = line.Other2?.GetValue() ?? string.Empty,
                                        Tax = line.SalesTaxCodeRef?.FullName?.GetValue() ?? string.Empty,
                                        SalesTaxTotal = (decimal?)(line.TaxAmount?.GetValue()) ?? 0m,
                                        ServiceDate = line.ServiceDate != null ? line.ServiceDate.GetValue().ToShortDateString() : string.Empty
                                    };

                                    invoiceData.Lines.Add(lineData);
                                }
                            }
                        }

                        invoices.Add(invoiceData);
                    }
                }
                else
                {
                    LogDataSync($"No invoices found for RefNumber: {refNumber}");
                }

                sessionManager.EndSession();
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                LogDataSync($"ERROR while getting invoice {refNumber}: {ex}");
                try
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                catch { }
            }

            return invoices;
        }


        public static List<InvoiceData> GetSalesOrderByRefNumber(string refNumber)
        {
            QBSessionManager sessionManager = new QBSessionManager();
            List<InvoiceData> invoices = new List<InvoiceData>(); // Reusing InvoiceData class for layout

            try
            {
                string AppName = "QBSI";
                sessionManager.OpenConnection("", AppName);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);

                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                // Change: Build SalesOrderQuery instead of InvoiceQuery
                ISalesOrderQuery soQuery = requestMsgSet.AppendSalesOrderQueryRq();
                soQuery.IncludeLineItems.SetValue(true);
                soQuery.OwnerIDList.Add("0");

                soQuery.ORTxnNoAccountQuery.TxnFilterNoAccount.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcStartsWith);
                soQuery.ORTxnNoAccountQuery.TxnFilterNoAccount.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNumber);

                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                ISalesOrderRetList soList = response.Detail as ISalesOrderRetList;

                if (soList != null && soList.Count > 0)
                {
                    for (int i = 0; i < soList.Count; i++)
                    {
                        ISalesOrderRet qbSO = soList.GetAt(i);

                        InvoiceData soData = new InvoiceData
                        {
                            RefNumber = qbSO?.RefNumber?.GetValue() ?? string.Empty,
                            TxnDate = qbSO?.TxnDate?.GetValue() ?? DateTime.MinValue,
                            CustomerName = qbSO?.CustomerRef?.FullName?.GetValue() ?? string.Empty,
                            Subtotal = qbSO?.Subtotal?.GetValue() ?? 0,
                            TotalAmount = qbSO?.TotalAmount?.GetValue() ?? 0,
                            Terms = qbSO?.TermsRef?.FullName?.GetValue() ?? string.Empty,
                            DueDate = qbSO?.DueDate?.GetValue() ?? DateTime.MinValue,
                            PONumber = qbSO?.PONumber?.GetValue() ?? string.Empty,
                            ShipAddress1 = qbSO?.ShipAddressBlock?.Addr1?.GetValue() ?? string.Empty,
                            ShipAddress2 = qbSO?.ShipAddressBlock?.Addr2?.GetValue() ?? string.Empty,
                            ShipAddress3 = qbSO?.ShipAddressBlock?.Addr3?.GetValue() ?? string.Empty,
                            ShipAddress4 = qbSO?.ShipAddressBlock?.Addr4?.GetValue() ?? string.Empty,
                            ShipAddress5 = qbSO?.ShipAddressBlock?.Addr5?.GetValue() ?? string.Empty,
                        };

                        // Custom Fields Logic
                        if (qbSO.DataExtRetList != null)
                        {
                            soData.CustomerCustomFields = new Dictionary<string, string>();
                            for (int k = 0; k < qbSO.DataExtRetList.Count; k++)
                            {
                                IDataExtRet dataExt = qbSO.DataExtRetList.GetAt(k);
                                soData.CustomerCustomFields[dataExt.DataExtName.GetValue()] = dataExt.DataExtValue.GetValue();
                            }
                        }

                        // Line Items Logic
                        if (qbSO.ORSalesOrderLineRetList != null)
                        {
                            Dictionary<string, string> uomCache = new Dictionary<string, string>();
                            for (int j = 0; j < qbSO.ORSalesOrderLineRetList.Count; j++)
                            {
                                var orLine = qbSO.ORSalesOrderLineRetList.GetAt(j);
                                if (orLine?.SalesOrderLineRet != null)
                                {
                                    var line = orLine.SalesOrderLineRet;
                                    string uomListID = line.OverrideUOMSetRef?.ListID?.GetValue();
                                    string finalUOM = line.UnitOfMeasure?.GetValue() ?? string.Empty;

                                    if (!string.IsNullOrEmpty(uomListID))
                                    {
                                        if (!uomCache.ContainsKey(uomListID)) uomCache[uomListID] = GetUOMAbbreviation(sessionManager, uomListID);
                                        finalUOM = uomCache[uomListID];
                                    }

                                    soData.Lines.Add(new InvoiceLineData
                                    {
                                        ItemName = line.ItemRef?.FullName?.GetValue() ?? string.Empty,
                                        Description = line.Desc?.GetValue() ?? string.Empty,
                                        Quantity = line.Quantity?.GetValue() ?? 0,
                                        UnitOfMeasure = finalUOM,
                                        Rate = (decimal?)(line.ORRate?.Rate?.GetValue()) ?? 0m,
                                        Amount = (decimal?)(line.Amount?.GetValue()) ?? 0m,
                                        SkuCode = line.Other1?.GetValue() ?? string.Empty,
                                        ExpirationDate = line.Other2?.GetValue() ?? string.Empty
                                    });
                                }
                            }
                        }
                        invoices.Add(soData);
                    }
                }
                sessionManager.EndSession();
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                LogDataSync($"Sales Order Query Error: {ex.Message}");
            }
            return invoices;
        }

        public static string GetUOMAbbreviation(QBSessionManager sessionManager, string uomListID)
        {
            if (string.IsNullOrEmpty(uomListID)) return string.Empty;

            try
            {
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeStop;

                // Query the UnitOfMeasureSet
                IUnitOfMeasureSetQuery uomQuery = requestMsgSet.AppendUnitOfMeasureSetQueryRq();
                uomQuery.ORListQuery.ListIDList.Add(uomListID);

                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IUnitOfMeasureSetRetList uomList = response.Detail as IUnitOfMeasureSetRetList;

                if (uomList != null && uomList.Count > 0)
                {
                    IUnitOfMeasureSetRet uom = uomList.GetAt(0);
                    // Access the Abbreviation from the BaseUnit aggregate
                    return uom.BaseUnit?.Abbreviation?.GetValue() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Log the error using your existing LogDataSync method
                LogDataSync($"Error fetching UOM Abbreviation for {uomListID}: {ex.Message}");
            }

            return string.Empty;
        }

        private static Dictionary<string, string> GetCustomerCustomFields(QBSessionManager sessionManager, string customerListID)
        {
            var customFields = new Dictionary<string, string>();

            try
            {
                IMsgSetRequest custRequest = sessionManager.CreateMsgSetRequest("US", 13, 0);
                custRequest.Attributes.OnError = ENRqOnError.roeStop;

                ICustomerQuery custQuery = custRequest.AppendCustomerQueryRq();
                custQuery.ORCustomerListQuery.ListIDList.Add(customerListID);
                custQuery.OwnerIDList.Add("0"); // To include custom fields

                IMsgSetResponse custResponse = sessionManager.DoRequests(custRequest);
                IResponse custResp = custResponse.ResponseList.GetAt(0);
                ICustomerRetList custList = custResp.Detail as ICustomerRetList;

                if (custList != null && custList.Count > 0)
                {
                    ICustomerRet customer = custList.GetAt(0);

                    LogDataSync($"Customer {customerListID} has DataExtRetList count: {customer.DataExtRetList?.Count ?? 0}");

                    if (customer.DataExtRetList != null)
                    {
                        for (int i = 0; i < customer.DataExtRetList.Count; i++)
                        {
                            IDataExtRet dataExt = customer.DataExtRetList.GetAt(i);
                            string name = dataExt.DataExtName?.GetValue();
                            string value = dataExt.DataExtValue?.GetValue();

                            if (!string.IsNullOrEmpty(name))
                            {
                                customFields[name] = value;
                                LogDataSync($"Customer {customerListID} - CustomField: {name} = {value}");
                            }
                        }
                    }
                }
                else
                {
                    LogDataSync($"No customer found for ListID: {customerListID}");
                }
            }
            catch (Exception ex)
            {
                LogDataSync($"ERROR fetching custom fields for Customer {customerListID}: {ex}");
            }

            return customFields;
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
