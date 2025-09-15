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

            public List<InvoiceLineData> Lines { get; set; } = new List<InvoiceLineData>();
            public Dictionary<string, string> CustomerCustomFields { get; set; } = new Dictionary<string, string>();
            public Dictionary<string, string> InvoiceCustomFields { get; set; } = new Dictionary<string, string>();
        }

        public class InvoiceLineData
        {
            public string ItemName { get; set; }
            public string Desc { get; set; }
            public double Quantity { get; set; }
            public double Rate { get; set; }
            public double Amount { get; set; }
            public string ServiceDate { get; set; }

        }
    }
}
