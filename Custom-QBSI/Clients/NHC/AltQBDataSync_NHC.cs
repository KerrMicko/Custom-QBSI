using System;
using System.Collections.Generic;
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

                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                IInvoiceRetList invoiceList = response.Detail as IInvoiceRetList;

                if (invoiceList != null && invoiceList.Count > 0)
                {
                    for (int i = 0; i < invoiceList.Count; i++)
                    {
                        IInvoiceRet qbInvoice = invoiceList.GetAt(i);

                        InvoiceData invoiceData = new InvoiceData
                        {
                            RefNumber = qbInvoice.RefNumber?.GetValue(),
                            TxnDate = qbInvoice.TxnDate?.GetValue() ?? DateTime.MinValue,
                            CustomerName = qbInvoice.CustomerRef?.FullName?.GetValue(),
                            BalanceRemaining = qbInvoice.BalanceRemaining?.GetValue() ?? 0,
                            Subtotal = qbInvoice.Subtotal?.GetValue() ?? 0,
                            TotalAmount = (qbInvoice.Subtotal?.GetValue() ?? 0) + (qbInvoice.SalesTaxTotal?.GetValue() ?? 0),
                            Terms = qbInvoice.TermsRef?.FullName?.GetValue(),
                            DueDate = qbInvoice.DueDate?.GetValue(),
                            PONumber = qbInvoice.PONumber?.GetValue(),
                            TaxesName = qbInvoice.ItemSalesTaxRef?.FullName?.GetValue(),

                            BillAddress1 = qbInvoice.BillAddressBlock.Addr1.GetValue(),
                            BillAddress2 = qbInvoice.BillAddressBlock?.Addr2?.GetValue(),
                            BillAddress3 = qbInvoice.BillAddressBlock?.Addr3?.GetValue(),
                            BillAddress4 = qbInvoice.BillAddressBlock?.Addr4?.GetValue(),
                            BillAddress5 = qbInvoice.BillAddressBlock?.Addr5?.GetValue(),
                        };

                        var customerListID = qbInvoice.CustomerRef?.ListID?.GetValue();
                        if (!string.IsNullOrEmpty(customerListID))
                        {
                            invoiceData.CustomerCustomFields = GetCustomerCustomFields(sessionManager, customerListID);
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
                                        ItemName = line.ItemRef?.FullName?.GetValue(),
                                        Description = line.Desc?.GetValue(),
                                        Quantity = line.Quantity?.GetValue() ?? 0,
                                        UnitOfMeasure = line.UnitOfMeasure?.GetValue(),
                                        Rate = (decimal)(line.ORRate?.Rate?.GetValue() ?? 0),
                                        Amount = (decimal)(line.Amount?.GetValue() ?? 0),
                                        TotalAmount = (decimal)(line.Amount?.GetValue() ?? 0),
                                        ExpirationDate = line.ExpirationDateForSerialLotNumber?.GetValue(),
                                        Tax = line.SalesTaxCodeRef?.FullName?.GetValue(),
                                        SalesTaxTotal = (decimal)(line.TaxAmount?.GetValue() ?? 0),
                                        ServiceDate = line.ServiceDate?.GetValue().ToShortDateString()
                                    };

                                    invoiceData.Lines.Add(lineData);
                                }
                            }
                        }

                        invoices.Add(invoiceData);
                    }
                }

                sessionManager.EndSession();
                sessionManager.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                sessionManager.EndSession();
                sessionManager.CloseConnection();
            }

            return invoices;
        }

        private static Dictionary<string, string> GetCustomerCustomFields(QBSessionManager sessionManager, string customerListID)
        {
            var customFields = new Dictionary<string, string>();

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

                Console.WriteLine($"Customer {customerListID} has DataExtRetList count: {customer.DataExtRetList?.Count ?? 0}");
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
                        }
                    }
                }
            }

            return customFields;
        }

    }
}
