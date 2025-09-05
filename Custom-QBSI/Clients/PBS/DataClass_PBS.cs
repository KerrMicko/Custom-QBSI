using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_QBSI.Clients.PBS
{
    public class DataClass_PBS
    {
        public class InvoiceData
        {
            public string TINNO { get; set; }
            public string CustomerName { get; set; }
            public string PONumber { get; set; }
            //public string DrNo { get; set; }
            public string PwdNo { get; set; }
            //public string SONumber { get; set; }
            public string BusinessStyle { get; set; }
            public string StoreCode { get; set; }
            public string JobOrderNo { get; set; }
            public string BillAddress1 { get; set; }
            public string BillAddress2 { get; set; }
            public string BillAddress3 { get; set; }
            public string BillAddress4 { get; set; }
            public string BillAddress5 { get; set; }
            public string BillCity { get; set; }
            public string ShipAddress1 { get; set; }
            public string ShipAddress2 { get; set; }
            public string ShipAddress3 { get; set; }
            public string ShipAddress4 { get; set; }
            public string ShipAddress5 { get; set; }

            public DateTime TxnDate { get; set; }
            public DateTime DueDate { get; set; }
            public string Terms { get; set; }
            public string Salesrep { get; set; }
            public string RefNumber { get; set; }

            // New property to hold line items associated with this invoice
            public List<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
        }

        public class InvoiceLineItem
        {
            public decimal Quantity { get; set; } // Change to decimal
            public string Description { get; set; }
            public string Salesrep { get; set; }
            public string ItemName { get; set; }
            public string Tax { get; set; }
            public string UnitOfMeasure { get; set; }
            public string ExpirationDate { get; set; }
            public string TaxesName { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal SalesTaxTotal { get; set; }

        }
    }
}
