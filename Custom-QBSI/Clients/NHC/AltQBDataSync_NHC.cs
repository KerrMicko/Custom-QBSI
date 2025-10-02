using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QBFC16Lib;
using static Custom_QBSI.Clients.NHC.AltDataClass_NHC;

namespace Custom_QBSI.Clients.NHC
{
    public class AltQBDataSync_NHC
    {
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

                        var customerListID = qbInvoice.CustomerRef?.ListID?.GetValue();
                        if (!string.IsNullOrEmpty(customerListID))
                        {
                            invoiceData.CustomerCustomFields = GetCustomerCustomFields(sessionManager, customerListID);
                            LogDataSync($"Fetched custom fields for customer {customerListID}");
                        }

                        // --- Line Items ---
                        if (qbInvoice.ORInvoiceLineRetList != null)
                        {
                            for (int j = 0; j < qbInvoice.ORInvoiceLineRetList.Count; j++)
                            {
                                var orLine = qbInvoice.ORInvoiceLineRetList.GetAt(j);
                                if (orLine?.InvoiceLineRet != null)
                                {
                                    var line = orLine.InvoiceLineRet;
                                    var lineData = new InvoiceLineData
                                    {
                                        ItemName = line.ItemRef?.FullName?.GetValue() ?? string.Empty,
                                        Description = line.Desc?.GetValue() ?? string.Empty,
                                        Quantity = line.Quantity?.GetValue() ?? 0,
                                        UnitOfMeasure = line.UnitOfMeasure?.GetValue() ?? string.Empty,
                                        Rate = (decimal?)(line.ORRate?.Rate?.GetValue()) ?? 0m,
                                        Amount = (decimal?)(line.Amount?.GetValue()) ?? 0m,
                                        TotalAmount = (decimal?)(line.Amount?.GetValue()) ?? 0m,
                                        ExpirationDate = line.Other1?.GetValue() ?? string.Empty,
                                        SkuCode = line.Other2?.GetValue() ?? string.Empty,
                                        Tax = line.SalesTaxCodeRef?.FullName?.GetValue() ?? string.Empty,
                                        SalesTaxTotal = (decimal?)(line.TaxAmount?.GetValue()) ?? 0m,
                                        ServiceDate = line.ServiceDate != null? line.ServiceDate.GetValue().ToShortDateString():string.Empty
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

        public static List<TransferInventoryData> GetTransferInventoryByRefNumber(string refNumber)
        {
            QBSessionManager sessionManager = new QBSessionManager();
            List<TransferInventoryData> transfers = new List<TransferInventoryData>();

            try
            {
                string AppName = "QBSI";
                LogDataSync($"Opening QuickBooks session for TransferInventory RefNumber: {refNumber}");

                // --- Open QB session ---
                sessionManager.OpenConnection("", AppName);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                LogDataSync("QuickBooks session opened.");

                // --- Step 1: Query TransferInventory for the specific RefNumber ---
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                ITransferInventoryQuery tiQuery = requestMsgSet.AppendTransferInventoryQueryRq();
                tiQuery.IncludeLineItems.SetValue(true);

                tiQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcStartsWith);
                tiQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNumber);
                LogDataSync($"Query built for RefNumber: {refNumber}");

                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                LogDataSync("TransferInventory response received.");

                if (responseMsgSet?.ResponseList != null && responseMsgSet.ResponseList.Count > 0)
                {
                    LogDataSync($"ResponseList count: {responseMsgSet.ResponseList.Count}");

                    HashSet<string> itemIDs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                    {
                        IResponse response = responseMsgSet.ResponseList.GetAt(i);
                        ITransferInventoryRetList tiList = response.Detail as ITransferInventoryRetList;

                        if (tiList != null)
                        {
                            LogDataSync($"Processing Response {i}, TransferInventory count: {tiList.Count}");

                            for (int j = 0; j < tiList.Count; j++)
                            {
                                ITransferInventoryRet ti = tiList.GetAt(j);
                                TransferInventoryData data = new TransferInventoryData
                                {
                                    TxnID = ti.TxnID?.GetValue(),
                                    TxnDate = ti.TxnDate?.GetValue() ?? DateTime.MinValue,
                                    RefNumber = ti.RefNumber?.GetValue(),
                                    Lines = new List<TransferInventoryLineData>()
                                };

                                if (ti.TransferInventoryLineRetList != null)
                                {
                                    LogDataSync($"TransferInventory has {ti.TransferInventoryLineRetList.Count} lines.");
                                    for (int k = 0; k < ti.TransferInventoryLineRetList.Count; k++)
                                    {
                                        ITransferInventoryLineRet line = ti.TransferInventoryLineRetList.GetAt(k);
                                        string itemListID = line.ItemRef?.ListID?.GetValue();
                                        if (!string.IsNullOrEmpty(itemListID)) itemIDs.Add(itemListID);

                                        data.Lines.Add(new TransferInventoryLineData
                                        {
                                            ItemRefFullNameTransfer = line.ItemRef?.FullName?.GetValue(),
                                            ItemRefListID = itemListID,
                                            QuantityTransfer = line.QuantityTransferred?.GetValue() ?? 0
                                        });

                                        LogDataSync($"Processed line {k} for item: {line.ItemRef?.FullName?.GetValue()}");
                                    }
                                }

                                transfers.Add(data);
                            }
                        }
                    }

                    // --- Step 2: Batch query all item details ---
                    LogDataSync($"Querying {itemIDs.Count} unique items...");
                    Dictionary<string, ItemData> itemDetailsDict = GetItemsByListIDs(sessionManager, itemIDs.ToList());
                    LogDataSync($"Item details retrieved: {itemDetailsDict.Count}");

                    // --- Step 3: Batch query UOM BaseUnits ---
                    HashSet<string> uomIDs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in itemDetailsDict.Values)
                    {
                        if (!string.IsNullOrEmpty(item.UnitOfMeasureListID)) uomIDs.Add(item.UnitOfMeasureListID);
                    }

                    LogDataSync($"Querying {uomIDs.Count} unique UOMs...");
                    Dictionary<string, string> uomBaseNames = GetBaseUnitNamesByListIDs(sessionManager, uomIDs.ToList());
                    LogDataSync($"UOM BaseUnit names retrieved: {uomBaseNames.Count}");

                    // --- Step 4: Map item details and UOM BaseUnits to lines ---
                    foreach (var transfer in transfers)
                    {
                        foreach (var line in transfer.Lines)
                        {
                            if (line.ItemRefListID != null && itemDetailsDict.TryGetValue(line.ItemRefListID, out var item))
                            {
                                line.SalesPrice = item.SalesPrice;
                                if (!string.IsNullOrEmpty(item.UnitOfMeasureListID) && uomBaseNames.TryGetValue(item.UnitOfMeasureListID, out var baseUnit))
                                {
                                    line.BaseUnitName = baseUnit;
                                }
                            }
                        }
                    }

                    LogDataSync($"TransferInventory processing completed. Total transfers: {transfers.Count}");
                }
                else
                {
                    LogDataSync("No TransferInventory found for the specified RefNumber.");
                }
            }
            catch (Exception ex)
            {
                LogDataSync($"Error in GetTransferInventoryByRefNumber: {ex.Message}");
            }
            finally
            {
                sessionManager.EndSession();
                sessionManager.CloseConnection();
                LogDataSync("QuickBooks session closed.");
            }

            return transfers;
        }




        // -------------------------
        // Optimized batch item query
        public static Dictionary<string, ItemData> GetItemsByListIDs(QBSessionManager sessionManager, List<string> itemListIDs)
        {
            Dictionary<string, ItemData> items = new Dictionary<string, ItemData>(StringComparer.OrdinalIgnoreCase);

            if (itemListIDs == null || itemListIDs.Count == 0)
            {
                LogDataSync("No item IDs provided for item query.");
                return items;
            }

            LogDataSync($"Starting batch item query for {itemListIDs.Count} items...");

            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

            foreach (var id in itemListIDs)
            {
                IItemInventoryQuery invQuery = requestMsgSet.AppendItemInventoryQueryRq();
                invQuery.ORListQueryWithOwnerIDAndClass.ListIDList.Add(id);
                invQuery.IncludeRetElementList.Add("ListID");
                invQuery.IncludeRetElementList.Add("UnitOfMeasureSetRef");
                invQuery.IncludeRetElementList.Add("SalesPrice");
                LogDataSync($"Prepared query for item ListID: {id}");
            }

            IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            LogDataSync("Item query response received.");

            if (responseMsgSet?.ResponseList != null)
            {
                for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                {
                    IResponse response = responseMsgSet.ResponseList.GetAt(i);
                    IItemInventoryRetList invList = response.Detail as IItemInventoryRetList;
                    if (invList != null)
                    {
                        for (int j = 0; j < invList.Count; j++)
                        {
                            IItemInventoryRet invItem = invList.GetAt(j);
                            items[invItem.ListID?.GetValue()] = new ItemData
                            {
                                ListID = invItem.ListID?.GetValue(),
                                UnitOfMeasureListID = invItem.UnitOfMeasureSetRef?.ListID?.GetValue(),
                                SalesPrice = invItem.SalesPrice?.GetValue() ?? 0
                            };
                            LogDataSync($"Processed item: {invItem.ListID?.GetValue()}, SalesPrice: {invItem.SalesPrice?.GetValue() ?? 0}");
                        }
                    }
                }
            }

            LogDataSync($"Batch item query completed. Total items retrieved: {items.Count}");
            return items;
        }


        // -------------------------
        // Optimized batch UOM query
        public static Dictionary<string, string> GetBaseUnitNamesByListIDs(QBSessionManager sessionManager, List<string> uomListIDs)
        {
            Dictionary<string, string> uomBaseNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (uomListIDs == null || uomListIDs.Count == 0)
            {
                LogDataSync("No UOM IDs provided for UOM query.");
                return uomBaseNames;
            }

            LogDataSync($"Starting batch UOM query for {uomListIDs.Count} UOMs...");

            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

            foreach (var id in uomListIDs)
            {
                IUnitOfMeasureSetQuery uomQuery = requestMsgSet.AppendUnitOfMeasureSetQueryRq();
                uomQuery.ORListQuery.ListIDList.Add(id);
                uomQuery.IncludeRetElementList.Add("ListID");
                uomQuery.IncludeRetElementList.Add("BaseUnit");
                LogDataSync($"Prepared query for UOM ListID: {id}");
            }

            IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            LogDataSync("UOM query response received.");

            if (responseMsgSet?.ResponseList != null)
            {
                for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                {
                    IResponse response = responseMsgSet.ResponseList.GetAt(i);
                    IUnitOfMeasureSetRetList uomList = response.Detail as IUnitOfMeasureSetRetList;
                    if (uomList != null)
                    {
                        for (int j = 0; j < uomList.Count; j++)
                        {
                            IUnitOfMeasureSetRet uom = uomList.GetAt(j);
                            if (!string.IsNullOrEmpty(uom.ListID?.GetValue()))
                            {
                                uomBaseNames[uom.ListID.GetValue()] = uom.BaseUnit?.Name?.GetValue();
                                LogDataSync($"Processed UOM: {uom.ListID?.GetValue()}, BaseUnit: {uom.BaseUnit?.Name?.GetValue()}");
                            }
                        }
                    }
                }
            }

            LogDataSync($"Batch UOM query completed. Total UOMs retrieved: {uomBaseNames.Count}");
            return uomBaseNames;
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
