using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_QBSI.Clients.NHC
{
    public class AltDataClass_NHC
    {
        public class InvoiceData
        {
            public string RefNumber { get; set; }
            public DateTime TxnDate { get; set; }
            public string CustomerName { get; set; }
            public double BalanceRemaining { get; set; }
            public double Subtotal { get; set; }
            public double TotalAmount { get; set; }
            public string Terms { get; set; }
            public DateTime? DueDate { get; set; }
            public string PONumber { get; set; }
            public string TaxesName { get; set; }
            public string ShipAddress1 { get; set; }
            public string ShipAddress2 { get; set; }
            public string ShipAddress3 { get; set; }
            public string ShipAddress4 { get; set; }
            public string ShipAddress5 { get; set; }

            public List<InvoiceLineData> Lines { get; set; } = new List<InvoiceLineData>();
            public Dictionary<string, string> CustomerCustomFields { get; set; } = new Dictionary<string, string>();

            // 🔹 Helper method
            public string GetCustomField(string fieldName)
            {
                if (CustomerCustomFields != null && CustomerCustomFields.TryGetValue(fieldName, out var value))
                {
                    return value;
                }
                return null;
            }
        }

        public class InvoiceLineData
        {
            public string ItemName { get; set; }
            public string Description { get; set; }
            public double Quantity { get; set; }
            public string UnitOfMeasure { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }
            public decimal TotalAmount { get; set; }
            public string ExpirationDate { get; set; }
            public string SkuCode { get; set; }
            public string Tax { get; set; }
            public decimal SalesTaxTotal { get; set; }
            public string ServiceDate { get; set; }

        }
    }
}
