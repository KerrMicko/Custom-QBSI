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

        public static List<TransferInventoryData> GetTransferInventoryBase(QBSessionManager sessionManager, string refNumber)
        {
            List<TransferInventoryData> transfers = new List<TransferInventoryData>();

            LogDataSync($"[TransferInventoryBase] Fetching TransferInventory for RefNumber={refNumber}");

            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

            var tiQuery = requestMsgSet.AppendTransferInventoryQueryRq();
            tiQuery.IncludeLineItems.SetValue(true);

            tiQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(refNumber);
            tiQuery.ORTransferInventoryQuery.TxnFilterNoCurrency.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcStartsWith);

            var responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            if (responseMsgSet?.ResponseList != null)
            {
                for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                {
                    var response = responseMsgSet.ResponseList.GetAt(i);
                    var tiList = response.Detail as ITransferInventoryRetList;
                    if (tiList != null)
                    {
                        LogDataSync($"[TransferInventoryBase] Found {tiList.Count} transfer(s) for RefNumber={refNumber}");
                        for (int j = 0; j < tiList.Count; j++)
                        {
                            var ti = tiList.GetAt(j);
                            var data = new TransferInventoryData
                            {
                                TxnID = ti.TxnID?.GetValue(),
                                RefNumber = ti.RefNumber?.GetValue(),
                                TxnDate = ti.TxnDate?.GetValue() ?? DateTime.MinValue,
                                Lines = new List<TransferInventoryLineData>()
                            };

                            if (ti.TransferInventoryLineRetList != null)
                            {
                                for (int k = 0; k < ti.TransferInventoryLineRetList.Count; k++)
                                {
                                    var line = ti.TransferInventoryLineRetList.GetAt(k);
                                    data.Lines.Add(new TransferInventoryLineData
                                    {
                                        ItemRefListID = line.ItemRef?.ListID?.GetValue(),
                                        QuantityTransfer = line.QuantityTransferred?.GetValue() ?? 0,
                                        SiteListID = ti.ToInventorySiteRef?.ListID?.GetValue()
                                    });

                                    LogDataSync($"[TransferInventoryBase] Line Added: " +
                                                $"Item={line.ItemRef?.ListID?.GetValue()}, " +
                                                $"Qty={line.QuantityTransferred?.GetValue()}, " +
                                                $"Site={ti.ToInventorySiteRef?.ListID?.GetValue()}");
                                }
                            }


                            transfers.Add(data);
                        }
                    }
                }
            }

            return transfers
                .Where(t => string.Equals(t.RefNumber, refNumber, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }



        public static Dictionary<string, ItemData> GetItemsByListIDs(QBSessionManager sessionManager, List<string> itemListIDs)
        {
            Dictionary<string, ItemData> items = new Dictionary<string, ItemData>(StringComparer.OrdinalIgnoreCase);

            if (itemListIDs == null || itemListIDs.Count == 0)
            {
                LogDataSync("[ItemsByListIDs] No Item IDs to query.");
                return items;
            }

            LogDataSync($"[ItemsByListIDs] Querying {itemListIDs.Count} item(s)...");

            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

            foreach (var id in itemListIDs)
            {
                IItemInventoryQuery invQuery = requestMsgSet.AppendItemInventoryQueryRq();
                invQuery.ORListQueryWithOwnerIDAndClass.ListIDList.Add(id);
                invQuery.IncludeRetElementList.Add("ListID");
                invQuery.IncludeRetElementList.Add("UnitOfMeasureSetRef");
                invQuery.IncludeRetElementList.Add("SalesPrice");
                invQuery.IncludeRetElementList.Add("SalesDesc");
            }

            var responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            if (responseMsgSet?.ResponseList != null)
            {
                for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                {
                    var response = responseMsgSet.ResponseList.GetAt(i);
                    var invList = response.Detail as IItemInventoryRetList;
                    if (invList != null)
                    {
                        LogDataSync($"[ItemsByListIDs] Found {invList.Count} item(s).");
                        for (int j = 0; j < invList.Count; j++)
                        {
                            var invItem = invList.GetAt(j);
                            items[invItem.ListID?.GetValue()] = new ItemData
                            {
                                ListID = invItem.ListID?.GetValue(),
                                UnitOfMeasureListID = invItem.UnitOfMeasureSetRef?.ListID?.GetValue(),
                                SalesPrice = invItem.SalesPrice?.GetValue() ?? 0,
                                SalesDesc = invItem.SalesDesc?.GetValue()
                            };
                            LogDataSync($"[ItemsByListIDs] Item Loaded: {invItem.ListID?.GetValue()} - {invItem.SalesDesc?.GetValue()}");
                        }
                    }
                }
            }

            return items;
        }



        public static Dictionary<string, string> GetBaseUnitNamesByListIDs(QBSessionManager sessionManager, List<string> uomListIDs)
        {
            Dictionary<string, string> uomBaseNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (uomListIDs == null || uomListIDs.Count == 0)
            {
                LogDataSync("[UOMByListIDs] No UOM IDs to query.");
                return uomBaseNames;
            }

            LogDataSync($"[UOMByListIDs] Querying {uomListIDs.Count} UOM(s)...");

            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

            foreach (var id in uomListIDs)
            {
                IUnitOfMeasureSetQuery uomQuery = requestMsgSet.AppendUnitOfMeasureSetQueryRq();
                uomQuery.ORListQuery.ListIDList.Add(id);
                uomQuery.IncludeRetElementList.Add("ListID");
                uomQuery.IncludeRetElementList.Add("BaseUnit");
            }

            var responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            if (responseMsgSet?.ResponseList != null)
            {
                for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                {
                    var response = responseMsgSet.ResponseList.GetAt(i);
                    var uomList = response.Detail as IUnitOfMeasureSetRetList;
                    if (uomList != null)
                    {
                        LogDataSync($"[UOMByListIDs] Found {uomList.Count} UOM(s).");
                        for (int j = 0; j < uomList.Count; j++)
                        {
                            var uom = uomList.GetAt(j);
                            if (!string.IsNullOrEmpty(uom.ListID?.GetValue()))
                            {
                                uomBaseNames[uom.ListID.GetValue()] = uom.BaseUnit?.Name?.GetValue();
                                LogDataSync($"[UOMByListIDs] UOM Loaded: {uom.ListID?.GetValue()} - {uom.BaseUnit?.Name?.GetValue()}");
                            }
                        }
                    }
                }
            }

            return uomBaseNames;
        }



        public static Dictionary<string, InventorySiteData> GetInventorySitesByListIDs(QBSessionManager sessionManager, List<string> siteListIDs)
        {
            var sites = new Dictionary<string, InventorySiteData>(StringComparer.OrdinalIgnoreCase);

            if (siteListIDs == null || siteListIDs.Count == 0)
            {
                LogDataSync("[SitesByListIDs] No Site IDs to query.");
                return sites;
            }

            LogDataSync($"[SitesByListIDs] Querying {siteListIDs.Count} site(s)...");

            IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 13, 0);
            requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

            foreach (var id in siteListIDs)
            {
                IInventorySiteQuery siteQuery = requestMsgSet.AppendInventorySiteQueryRq();
                siteQuery.ORInventorySiteQuery.ListIDList.Add(id);
                siteQuery.IncludeRetElementList.Add("ListID");
                siteQuery.IncludeRetElementList.Add("Name");
                siteQuery.IncludeRetElementList.Add("SiteDesc");
                siteQuery.IncludeRetElementList.Add("SiteAddressBlock");
            }

            var responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            if (responseMsgSet?.ResponseList != null)
            {
                for (int i = 0; i < responseMsgSet.ResponseList.Count; i++)
                {
                    var response = responseMsgSet.ResponseList.GetAt(i);
                    var siteList = response.Detail as IInventorySiteRetList;
                    if (siteList != null)
                    {
                        LogDataSync($"[SitesByListIDs] Found {siteList.Count} site(s).");
                        for (int j = 0; j < siteList.Count; j++)
                        {
                            var site = siteList.GetAt(j);
                            if (!string.IsNullOrEmpty(site.ListID?.GetValue()))
                            {
                                var siteData = new InventorySiteData
                                {
                                    ListID = site.ListID?.GetValue(),
                                    Name = site.Name?.GetValue(),
                                    Description = site.SiteDesc?.GetValue(),
                                    SiteAddressBlockAddr1 = site.SiteAddressBlock?.Addr1?.GetValue(),
                                    SiteAddressBlockAddr2 = site.SiteAddressBlock?.Addr2?.GetValue(),
                                    SiteAddressBlockAddr3 = site.SiteAddressBlock?.Addr3?.GetValue(),
                                    SiteAddressBlockAddr4 = site.SiteAddressBlock?.Addr4?.GetValue(),
                                    SiteAddressBlockAddr5 = site.SiteAddressBlock?.Addr5?.GetValue()
                                };

                                sites[site.ListID.GetValue()] = siteData;

                                LogDataSync($"[SitesByListIDs] Site Loaded: {site.ListID?.GetValue()} - {site.Name?.GetValue()} - {site.SiteDesc?.GetValue()} " +
                                $"Addr1={site.SiteAddressBlock?.Addr1?.GetValue()} Addr2={site.SiteAddressBlock?.Addr2?.GetValue()} " +
                                $"Addr3={site.SiteAddressBlock?.Addr3?.GetValue()} Addr4={site.SiteAddressBlock?.Addr4?.GetValue()} " +
                                $"Addr5={site.SiteAddressBlock?.Addr5?.GetValue()}");
                            }
                        }
                    }
                }
            }

            return sites;
        }




        public static void MapTransferLineDetails(List<TransferInventoryData> transfers,
                                          Dictionary<string, ItemData> items,
                                          Dictionary<string, string> uoms,
                                          Dictionary<string, InventorySiteData> sites)
        {
            LogDataSync("[MapTransferLineDetails] Mapping transfer line details...");

            foreach (var transfer in transfers)
            {
                foreach (var line in transfer.Lines)
                {
                    if (line.ItemRefListID != null && items.TryGetValue(line.ItemRefListID, out var item))
                    {
                        line.SalesPrice = item.SalesPrice;
                        line.ItemDescription = item.SalesDesc;

                        if (!string.IsNullOrEmpty(item.UnitOfMeasureListID) &&
                            uoms.TryGetValue(item.UnitOfMeasureListID, out var baseUnit))
                        {
                            line.BaseUnitName = baseUnit;
                        }
                    }

                    if (!string.IsNullOrEmpty(line.SiteListID) &&sites.TryGetValue(line.SiteListID, out var siteData))
                    {
                        line.SiteName = siteData.Name;
                        line.SiteDescription = siteData.Description;

                        // Map the full address
                        line.SiteAddr1 = siteData.SiteAddressBlockAddr1;
                        line.SiteAddr2 = siteData.SiteAddressBlockAddr2;
                        line.SiteAddr3 = siteData.SiteAddressBlockAddr3;
                        line.SiteAddr4 = siteData.SiteAddressBlockAddr4;
                        line.SiteAddr5 = siteData.SiteAddressBlockAddr5;
                    }


                    LogDataSync($"[MapTransferLineDetails] Line mapped: Item={line.ItemRefListID}, Desc={line.ItemDescription}, Price={line.SalesPrice}, UOM={line.BaseUnitName}, Site={line.SiteName}");
                }
            }
        }



        public static List<TransferInventoryData> GetTransferInventoryByRefNumber(string refNumber)
        {
            QBSessionManager sessionManager = new QBSessionManager();
            List<TransferInventoryData> transfers = new List<TransferInventoryData>();

            try
            {
                LogDataSync($"[GetTransferInventoryByRefNumber] Starting process for RefNumber={refNumber}");

                sessionManager.OpenConnection("", "QBSI");
                sessionManager.BeginSession("", ENOpenMode.omDontCare);

                transfers = GetTransferInventoryBase(sessionManager, refNumber);

                var itemIDs = transfers.SelectMany(t => t.Lines)
                                       .Select(l => l.ItemRefListID)
                                       .Where(id => !string.IsNullOrEmpty(id))
                                       .Distinct()
                                       .ToList();

                var items = GetItemsByListIDs(sessionManager, itemIDs);

                var uomIDs = items.Values.Select(i => i.UnitOfMeasureListID)
                                         .Where(id => !string.IsNullOrEmpty(id))
                                         .Distinct()
                                         .ToList();

                var uoms = GetBaseUnitNamesByListIDs(sessionManager, uomIDs);

                var siteIDs = transfers.SelectMany(t => t.Lines)
                                       .Select(l => l.SiteListID)
                                       .Where(id => !string.IsNullOrEmpty(id))
                                       .Distinct()
                                       .ToList();

                var sites = GetInventorySitesByListIDs(sessionManager, siteIDs);

                MapTransferLineDetails(transfers, items, uoms, sites);

                LogDataSync($"[GetTransferInventoryByRefNumber] Completed for RefNumber={refNumber}, Transfer Count={transfers.Count}");
            }
            catch (Exception ex)
            {
                LogDataSync($"[GetTransferInventoryByRefNumber] Error: {ex.Message}");
            }
            finally
            {
                try
                {
                    sessionManager.EndSession();
                    sessionManager.CloseConnection();
                }
                catch { }
            }

            return transfers;
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
