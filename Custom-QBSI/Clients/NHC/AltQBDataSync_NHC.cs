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

                // Start QuickBooks session
                sessionManager.OpenConnection("", AppName);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);

                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                // Build TransferInventory query
                ITransferInventoryQuery tiQuery = requestMsgSet.AppendTransferInventoryQueryRq();
                tiQuery.IncludeLineItems.SetValue(true);

                // Send request
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                if (responseMsgSet?.ResponseList != null && responseMsgSet.ResponseList.Count > 0)
                {
                    for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                    {
                        IResponse response = responseMsgSet.ResponseList.GetAt(i);
                        ITransferInventoryRetList tiList = response.Detail as ITransferInventoryRetList;

                        if (tiList != null && tiList.Count > 0)
                        {
                            for (int j = 0; j < tiList.Count; j++)
                            {
                                ITransferInventoryRet ti = tiList.GetAt(j);

                                // Filter manually by RefNumber
                                if (ti.RefNumber != null && ti.RefNumber.GetValue().Equals(refNumber, StringComparison.OrdinalIgnoreCase))
                                {
                                    TransferInventoryData data = new TransferInventoryData
                                    {
                                        TxnID = ti.TxnID?.GetValue(),
                                        TxnDate = ti.TxnDate?.GetValue() ?? DateTime.MinValue,
                                        RefNumber = ti.RefNumber?.GetValue(),
                                        Lines = new List<TransferInventoryLineData>()
                                    };

                                    if (ti.TransferInventoryLineRetList != null && ti.TransferInventoryLineRetList.Count > 0)
                                    {
                                        // Inside the loop for each TransferInventoryLineRet
                                        for (int k = 0; k < ti.TransferInventoryLineRetList.Count; k++)
                                        {
                                            ITransferInventoryLineRet line = ti.TransferInventoryLineRetList.GetAt(k);

                                            string itemListID = line.ItemRef?.ListID?.GetValue();

                                            // Get item details including UnitOfMeasureListID and SalesPrice
                                            List<ItemData> itemDetails = GetItemByListID(itemListID);
                                            ItemData itemInfo = itemDetails.FirstOrDefault();

                                            // Get BaseUnitName using UnitOfMeasureListID
                                            string baseUnitName = null;
                                            if (!string.IsNullOrEmpty(itemInfo?.UnitOfMeasureListID))
                                            {
                                                baseUnitName = GetBaseUnitNameByUOMListID(itemInfo.UnitOfMeasureListID);
                                            }

                                            TransferInventoryLineData lineData = new TransferInventoryLineData
                                            {
                                                ItemRefFullNameTransfer = line.ItemRef?.FullName?.GetValue(),
                                                ItemRefListID = itemListID,
                                                QuantityTransfer = line.QuantityTransferred?.GetValue() ?? 0,
                                                UnitOfMeasureListID = itemInfo?.UnitOfMeasureListID,
                                                BaseUnitName = baseUnitName,   // <-- added here
                                                SalesPrice = itemInfo?.SalesPrice ?? 0
                                            };

                                            data.Lines.Add(lineData);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"No lines found for RefNumber {ti.RefNumber?.GetValue()}");
                                    }

                                    transfers.Add(data);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("No TransferInventory records found in this response.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No responses returned from QuickBooks.");
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
            }

            return transfers;
        }

        public static List<ItemData> GetItemByListID(string itemListID)
        {
            QBSessionManager sessionManager = new QBSessionManager();
            List<ItemData> items = new List<ItemData>();

            try
            {
                string AppName = "QBSI";
                LogDataSync($"Opening QuickBooks session for Item ListID: {itemListID}");

                sessionManager.OpenConnection("", AppName);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);

                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                // === Inventory Items ===
                IItemInventoryQuery invQuery = requestMsgSet.AppendItemInventoryQueryRq();
                invQuery.IncludeRetElementList.Add("ListID");
                invQuery.IncludeRetElementList.Add("UnitOfMeasureSetRef");
                invQuery.IncludeRetElementList.Add("SalesPrice"); // added SalesPrice

                // Send request
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                if (responseMsgSet?.ResponseList != null && responseMsgSet.ResponseList.Count > 0)
                {
                    IResponse invResponse = responseMsgSet.ResponseList.GetAt(0);
                    IItemInventoryRetList invList = invResponse.Detail as IItemInventoryRetList;

                    if (invList != null && invList.Count > 0)
                    {
                        for (int i = 0; i < invList.Count; i++)
                        {
                            IItemInventoryRet invItem = invList.GetAt(i);

                            if (string.IsNullOrEmpty(itemListID) ||
                                invItem.ListID?.GetValue().Equals(itemListID, StringComparison.OrdinalIgnoreCase) == true)
                            {
                                items.Add(new ItemData
                                {
                                    ListID = invItem.ListID?.GetValue(),
                                    UnitOfMeasureListID = invItem.UnitOfMeasureSetRef?.ListID?.GetValue(),
                                    SalesPrice = invItem.SalesPrice?.GetValue() ?? 0 // added SalesPrice
                                });
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No responses returned from QuickBooks.");
                }
            }
            catch (Exception ex)
            {
                LogDataSync($"Error in GetItemByListID: {ex.Message}");
            }
            finally
            {
                sessionManager.EndSession();
                sessionManager.CloseConnection();
            }

            return items;
        }

        public static string GetBaseUnitNameByUOMListID(string uomListID)
        {
            if (string.IsNullOrEmpty(uomListID)) return null;

            QBSessionManager sessionManager = new QBSessionManager();
            string baseUnitName = null;

            try
            {
                string AppName = "QBSI";
                LogDataSync($"Opening QuickBooks session for UOM ListID: {uomListID}");

                sessionManager.OpenConnection("", AppName);
                sessionManager.BeginSession("", ENOpenMode.omDontCare);

                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                // --- UnitOfMeasureSet Query ---
                IUnitOfMeasureSetQuery uomQuery = requestMsgSet.AppendUnitOfMeasureSetQueryRq();
                uomQuery.IncludeRetElementList.Add("ListID");
                uomQuery.IncludeRetElementList.Add("BaseUnit");

                // Send request
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);

                if (responseMsgSet?.ResponseList != null && responseMsgSet.ResponseList.Count > 0)
                {
                    IResponse response = responseMsgSet.ResponseList.GetAt(0);

                    IUnitOfMeasureSetRetList uomList = response.Detail as IUnitOfMeasureSetRetList;
                    if (uomList != null && uomList.Count > 0)
                    {
                        for (int i = 0; i < uomList.Count; i++)
                        {
                            IUnitOfMeasureSetRet uom = uomList.GetAt(i);
                            if (uom.ListID?.GetValue().Equals(uomListID, StringComparison.OrdinalIgnoreCase) == true)
                            {
                                baseUnitName = uom.BaseUnit?.Name?.GetValue();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogDataSync($"Error in GetBaseUnitNameByUOMListID: {ex.Message}");
            }
            finally
            {
                sessionManager.EndSession();
                sessionManager.CloseConnection();
            }

            return baseUnitName;
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
