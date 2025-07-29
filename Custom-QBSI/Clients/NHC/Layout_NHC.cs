using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Custom_QBSI.Clients.NHC.Dataclass_NHC;

namespace Custom_QBSI.Clients.NHC
{
    public class Layout_NHC
    {
        Font font_Six = new Font("Microsoft Sans Serif", 6, System.Drawing.FontStyle.Regular);
        Font font_Seven = new Font("Microsoft Sans Serif", 7, System.Drawing.FontStyle.Regular);
        Font font_SevenBold = new Font("Microsoft Sans Serif", 7, System.Drawing.FontStyle.Bold);
        Font font_Eight = new Font("Microsoft Sans Serif", 8, System.Drawing.FontStyle.Regular);
        Font font_EightBold = new Font("Microsoft Sans Serif", 8, System.Drawing.FontStyle.Bold);
        Font font_Nine = new Font("Microsoft Sans Serif", 9, System.Drawing.FontStyle.Regular);
        Font font_NineBold = new Font("Microsoft Sans Serif", 9, System.Drawing.FontStyle.Bold);
        Font font_Ten = new Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Regular);
        Font font_TenBold = new Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
        Font font_Eleven = new Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Regular);
        Font font_ElevenBold = new Font("Microsoft Sans Serif", 11, System.Drawing.FontStyle.Bold);
        Font font_TwelveBold = new Font("Microsoft Sans Serif", 12, System.Drawing.FontStyle.Bold);
        Font font_ThirteenBold = new Font("Microsoft Sans Serif", 13, System.Drawing.FontStyle.Bold);
        Font font_FourteenBold = new Font("Microsoft Sans Serif", 14, System.Drawing.FontStyle.Bold);
        Font font_SixteenBold = new Font("Microsoft Sans Serif", 16, System.Drawing.FontStyle.Bold);

        StringFormat sfAlignRight = new StringFormat { Alignment = StringAlignment.Far | StringAlignment.Far };
        StringFormat sfAlignCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat sfAlignCenterRight = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        StringFormat sfAlignLeftCenter = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        StringFormat sfAlignLeftBottom = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far };

        /*public void PrintPage_NHC(object sender, PrintPageEventArgs e, List<InvoiceData> invoiceData, int layoutIndex, string note, string businessStyle, string pwdSignature, bool isEnableExpDateChecked)
        {
            switch (layoutIndex)
            {
                case 1:
                    Layout_SalesInvoice(e, invoiceData);
                    break;
                case 2:
                    Layout_DeliveryReceipt(e, invoiceData, note, businessStyle, pwdSignature, isEnableExpDateChecked);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }*/

        public void Layout_SalesInvoice(PrintPageEventArgs e, List<InvoiceData> invoiceData)
        {

            Image image = Properties.Resources.NATURE_SI;
            e.Graphics.DrawImage(image, e.PageBounds);

            Font font_Data = font_Eight;

            Rectangle rectDate = new Rectangle(670, 110, 120, 25);
            Rectangle rectSoldTo = new Rectangle(145, 140, 285, 20);
            Rectangle rectBusinessStyle = new Rectangle(325, 160, 285, 20);
            Rectangle rectTIN = new Rectangle(145, 160, 285, 20);
            Rectangle rectBusinessAdd = new Rectangle(145, 180, 285, 25);


            string Date = invoiceData[0].TxnDate.ToString("MM/dd/yyyy");
            string invoiceSoldTo = invoiceData[0].CustomerName.ToString();
            string invoiceBusinessStyle = invoiceData[0].BusinessStyle.ToString(); //NEED FOR CUSTOM OR JUST ADD BUSINESS STYLE NAMES INPUT ON PROGRAM
            string invoiceTin = invoiceData[0].TINNO.ToString();
            string invoiceBusinessAdd = invoiceData[0].BillAddress1.ToString() + invoiceData[0].BillAddress2.ToString() + invoiceData[0].BillAddress3.ToString() + invoiceData[0].BillAddress4.ToString() + invoiceData[0].BillAddress5.ToString();


            e.Graphics.DrawString(Date, font_Data, Brushes.Black, rectDate, sfAlignCenter);
            e.Graphics.DrawString(invoiceSoldTo, font_Data, Brushes.Black, rectSoldTo);
            e.Graphics.DrawString(invoiceBusinessStyle, font_Data, Brushes.Black, rectBusinessStyle);
            e.Graphics.DrawString(invoiceTin, font_Data, Brushes.Black, rectTIN);
            e.Graphics.DrawString(invoiceBusinessAdd, font_Data, Brushes.Black, rectBusinessAdd);


            Rectangle rectPoNo = new Rectangle(145, 205, 285, 15);
            Rectangle rectStoreCode = new Rectangle(145, 220, 285, 15);
            Rectangle rectTerms = new Rectangle(145, 235, 285, 15);

            string invoicePoNo = invoiceData[0].PONumber.ToString();
            string invoiceStoreCode = invoiceData[0].StoreCode.ToString(); // CUSTOM STORE CODE
            string invoiceTerms = invoiceData[0].Terms.ToString();

            e.Graphics.DrawString(invoicePoNo, font_Data, Brushes.Black, rectPoNo);
            e.Graphics.DrawString(invoiceStoreCode, font_Data, Brushes.Black, rectStoreCode);
            e.Graphics.DrawString(invoiceTerms, font_Data, Brushes.Black, rectTerms);


            //MIDDLE TABLE

            int tab1XStart = 68;
            int tab1YStart = 280;
            int tab1DataHeight = 26;

            int widthItemQuantity = 75;
            int widthItemUOM = 67;
            int widthItemDesc = 340;
            int widthItemUnitPrice = 119;
            int widthItemAmount = 120;

            int xStartItemQty = tab1XStart;
            int xStartItemUOM = tab1XStart + xStartItemQty + 8;
            int xStartItemDesc = tab1XStart + xStartItemUOM;
            int xStartItemUnitPrice = tab1XStart + xStartItemQty + xStartItemUOM + xStartItemDesc + 61;
            int xStartItemAmount = tab1XStart + xStartItemUnitPrice + 51;


            Rectangle rectItemQuantity = new Rectangle(xStartItemQty, tab1YStart, widthItemQuantity, tab1DataHeight);
            Rectangle rectItemUOM = new Rectangle(xStartItemUOM, tab1YStart, widthItemUOM, tab1DataHeight);
            Rectangle rectItemDescription = new Rectangle(xStartItemDesc, tab1YStart, widthItemDesc, tab1DataHeight);
            Rectangle rectItemUnitPrice = new Rectangle(xStartItemUnitPrice, tab1YStart, widthItemUnitPrice, tab1DataHeight);
            Rectangle rectItemAmount = new Rectangle(xStartItemAmount, tab1YStart, widthItemAmount, tab1DataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectItemQuantity);
            e.Graphics.DrawRectangle(Pens.Blue, rectItemUOM);
            e.Graphics.DrawRectangle(Pens.Yellow, rectItemDescription);
            e.Graphics.DrawRectangle(Pens.Orange, rectItemUnitPrice);
            e.Graphics.DrawRectangle(Pens.Pink, rectItemAmount);*/


            string ItemQuantity = "200";
            string ItemUOM = "cs";
            string ItemDescription = "Capilano Honey UD - 8 x 340g";
            double ItemUnitPrice = 2311.61;
            double ItemAmount = 4623.21;


            e.Graphics.DrawString(ItemQuantity.ToString(), font_Eight, Brushes.Black, rectItemQuantity, sfAlignCenter);
            e.Graphics.DrawString(ItemUOM.ToString(), font_Eight, Brushes.Black, rectItemUOM, sfAlignCenter);
            e.Graphics.DrawString(ItemDescription.ToString(), font_Eight, Brushes.Black, rectItemDescription, sfAlignLeftCenter);
            e.Graphics.DrawString(ItemUnitPrice.ToString("N2"), font_Eight, Brushes.Black, rectItemUnitPrice, sfAlignCenterRight);
            e.Graphics.DrawString(ItemAmount.ToString("N2"), font_Eight, Brushes.Black, rectItemAmount, sfAlignCenterRight);


            //LEFT TABLE
            int tab1LeftDataHeight = 18;
            int tab2LeftDataHeight = 18;

            int tab2LeftXStart = 230;
            int tab2LeftYStart = 770;
            int tab2LeftWidth = 170;


            Rectangle rectVATableSales = new Rectangle(tab2LeftXStart, tab2LeftYStart, tab2LeftWidth, tab1LeftDataHeight);
            Rectangle rectVATExemptSales = new Rectangle(tab2LeftXStart, tab2LeftYStart + tab2LeftDataHeight, tab2LeftWidth, tab1LeftDataHeight);
            Rectangle rectZeroRatedSales = new Rectangle(tab2LeftXStart, tab2LeftYStart - 3 + tab2LeftDataHeight * 2, tab2LeftWidth, tab1LeftDataHeight);
            Rectangle rectVatAmount = new Rectangle(tab2LeftXStart, tab2LeftYStart - 3 + tab2LeftDataHeight * 3, tab2LeftWidth, tab1LeftDataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectVATableSales);
            e.Graphics.DrawRectangle(Pens.Blue, rectVATExemptSales);
            e.Graphics.DrawRectangle(Pens.Yellow, rectZeroRateSales);
            e.Graphics.DrawRectangle(Pens.Orange, rectVatAmount);*/

            double vatAbleSales = 100;
            double vatExempt = 200;
            double zeroRatedSales = 300;
            double vatAmount = 400;


            e.Graphics.DrawString(vatAbleSales.ToString("N2"), font_Data, Brushes.Black, rectVATableSales, sfAlignCenterRight);
            e.Graphics.DrawString(vatExempt.ToString("N2"), font_Data, Brushes.Black, rectVATExemptSales, sfAlignCenterRight);
            e.Graphics.DrawString(zeroRatedSales.ToString("N2"), font_Data, Brushes.Black, rectZeroRatedSales, sfAlignCenterRight);
            e.Graphics.DrawString(vatAmount.ToString("N2"), font_Data, Brushes.Black, rectVatAmount, sfAlignCenterRight);


            // RIGHT TABLE
            int tab2RightDataHeight = 17;

            int tab2RightXStart = 580;
            int tab2RightYStart = 765;
            int tab2RightWidth = 210;

            Rectangle rectR1TotalSales = new Rectangle(tab2RightXStart, tab2RightYStart, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR2LessVAT = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR3AmountNetofVAT = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 2, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR4LessDiscount = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 3, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR5AddVAT = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 4, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR6LessWitholdingTax = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 5, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR7TotalAmountDue = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 6, tab2RightWidth, tab2RightDataHeight);


            /*e.Graphics.DrawRectangle(Pens.Blue, rectR1TotalSales);
            e.Graphics.DrawRectangle(Pens.Red, rectR2LessVAT);
            e.Graphics.DrawRectangle(Pens.Orange, rectR3AmountNetofVAT);
            e.Graphics.DrawRectangle(Pens.Green, rectR4LessDiscount);
            e.Graphics.DrawRectangle(Pens.Yellow, rectR5AddVAT);
            e.Graphics.DrawRectangle(Pens.Violet, rectR6LessWitholdingTax);
            e.Graphics.DrawRectangle(Pens.Pink, rectR7TotalAmountDue);*/

            double totalSales = 500;
            double lessVAT = 600;
            double amountNetOfVAT = 800;
            double lessDiscount = 900;
            double addVAT = 1000;
            double lessWitholdingTax = 1100;
            double totalAmountDue = 1200;

            e.Graphics.DrawString(totalSales.ToString("N2"), font_Data, Brushes.Black, rectR1TotalSales, sfAlignCenterRight);
            e.Graphics.DrawString(lessVAT.ToString("N2"), font_Data, Brushes.Black, rectR2LessVAT, sfAlignCenterRight);
            e.Graphics.DrawString(amountNetOfVAT.ToString("N2"), font_Data, Brushes.Black, rectR3AmountNetofVAT, sfAlignCenterRight);
            e.Graphics.DrawString(lessDiscount.ToString("N2"), font_Data, Brushes.Black, rectR4LessDiscount, sfAlignCenterRight);
            e.Graphics.DrawString(addVAT.ToString("N2"), font_Data, Brushes.Black, rectR5AddVAT, sfAlignCenterRight);
            e.Graphics.DrawString(lessWitholdingTax.ToString("N2"), font_Data, Brushes.Black, rectR6LessWitholdingTax, sfAlignCenterRight);
            e.Graphics.DrawString(totalAmountDue.ToString("N2"), font_Data, Brushes.Black, rectR7TotalAmountDue, sfAlignCenterRight);



            // Signatory
            string Signatory = "Danil Jeus Ampatin";

            Rectangle rectAuthorized = new Rectangle(555, 945, 230, 18);

            //e.Graphics.DrawRectangle(Pens.Black, rectAuthorized);

            e.Graphics.DrawString(Signatory, font_Data, Brushes.Black, rectAuthorized, sfAlignCenter);

        }

        public void Layout_DeliveryReceipt(PrintPageEventArgs e, List<InvoiceData> invoiceData, string note, string businessStyle, string pwdSignature, bool isEnableExpDateChecked)
        {
            Image image = Properties.Resources.NATURE_DR;
            e.Graphics.DrawImage(image, e.PageBounds);

            Font font_Data = font_Eight;

            Rectangle rectDate = new Rectangle(690, 90, 120, 25);
            Rectangle rectSoldTo = new Rectangle(125, 135, 285, 20);
            Rectangle rectBusinessStyle = new Rectangle(615, 138, 205, 20);
            Rectangle rectTIN = new Rectangle(125, 155, 285, 20);
            Rectangle rectBusinessAdd = new Rectangle(125, 175, 285, 25);

            /*e.Graphics.DrawRectangle(Pens.Orange, rectDate);
            e.Graphics.DrawRectangle(Pens.Orange, rectSoldTo);
            e.Graphics.DrawRectangle(Pens.Orange, rectBusinessStyle);
            e.Graphics.DrawRectangle(Pens.Blue, rectTIN);
            e.Graphics.DrawRectangle(Pens.Green, rectBusinessAdd);*/

            string Date = invoiceData[0].TxnDate.ToString("MM/dd/yyyy");
            string invoiceSoldTo = invoiceData[0].CustomerName.ToString();
            string invoiceBusinessStyle = invoiceData[0].BusinessStyle.ToString();
            string invoiceTin = invoiceData[0].TINNO.ToString();
            string invoiceBusinessAdd = invoiceData[0].BillAddress1.ToString() + invoiceData[0].BillAddress2.ToString() + invoiceData[0].BillAddress3.ToString() + invoiceData[0].BillAddress4.ToString() + invoiceData[0].BillAddress5.ToString();


            e.Graphics.DrawString(Date, font_Data, Brushes.Black, rectDate, sfAlignCenter);
            e.Graphics.DrawString(invoiceSoldTo, font_Data, Brushes.Black, rectSoldTo);
            e.Graphics.DrawString(invoiceBusinessStyle, font_Data, Brushes.Black, rectBusinessStyle);
            e.Graphics.DrawString(invoiceTin, font_Data, Brushes.Black, rectTIN);
            e.Graphics.DrawString(invoiceBusinessAdd, font_Data, Brushes.Black, rectBusinessAdd);

            Rectangle rectPoNo = new Rectangle(125, 205, 285, 15);
            Rectangle rectStoreCode = new Rectangle(125, 220, 285, 15);
            Rectangle rectTerms = new Rectangle(125, 235, 285, 15);

            /*e.Graphics.DrawRectangle(Pens.Red, rectPoNo);
            e.Graphics.DrawRectangle(Pens.Yellow, rectStoreCode);
            e.Graphics.DrawRectangle(Pens.Pink, rectTerms);*/

            string invoicePoNo = invoiceData[0].PONumber.ToString();
            string invoiceStoreCode = invoiceData[0].StoreCode.ToString(); // CUSTOM STORE CODE
            string invoiceTerms = invoiceData[0].Terms.ToString();

            e.Graphics.DrawString(invoicePoNo, font_Data, Brushes.Black, rectPoNo);
            e.Graphics.DrawString(invoiceStoreCode, font_Data, Brushes.Black, rectStoreCode);
            e.Graphics.DrawString(invoiceTerms, font_Data, Brushes.Black, rectTerms);

            // TABLE MIDDLE CALCULATION
            int tabXStart = 50;
            int tabYStart = 285;

            int tabDataHeight = 30;

            int widthItemNo = 67;
            int widthItemQuantity = 75;
            int widthItemUnit = 45;
            int widthItemDescription = 640;

            int xStartItemQuantity = tabXStart;
            int xStartItemUnit = tabXStart + widthItemQuantity;
            int xStartItemDescription = tabXStart + widthItemNo + widthItemQuantity + widthItemUnit - 67;
            int xStartItemAmount = tabXStart + widthItemNo + widthItemQuantity + widthItemUnit - 67;

            Rectangle rectItemQuantity = new Rectangle(xStartItemQuantity, tabYStart, widthItemQuantity, tabDataHeight);
            Rectangle rectItemUnit = new Rectangle(xStartItemUnit, tabYStart, widthItemUnit, tabDataHeight);
            Rectangle rectItemDescription = new Rectangle(xStartItemDescription, tabYStart, widthItemDescription, tabDataHeight);
            Rectangle rectItemAmount = new Rectangle(xStartItemAmount, tabYStart, widthItemDescription, tabDataHeight);

            /*e.Graphics.DrawRectangle(Pens.Black, rectItemQuantity);
            e.Graphics.DrawRectangle(Pens.Red, rectItemUnit);
            e.Graphics.DrawRectangle(Pens.Pink, rectItemDescription);
            e.Graphics.DrawRectangle(Pens.Blue, rectItemAmount);*/

            decimal totalAmount = 0;
            int itemHeight = 0;
            int counter = 1;

            foreach (var invoice in invoiceData)
            {
                foreach (var lineItem in invoice.LineItems)
                {
                    e.Graphics.DrawString(lineItem.Quantity.ToString("N2"), font_Data, Brushes.Black, new Rectangle(xStartItemQuantity, tabYStart + itemHeight, widthItemQuantity, tabDataHeight), sfAlignCenter);
                    e.Graphics.DrawString(lineItem.UnitOfMeasure, font_Data, Brushes.Black, new Rectangle(xStartItemUnit, tabYStart + itemHeight, widthItemUnit, tabDataHeight), sfAlignCenter);
                    
                    if (isEnableExpDateChecked)
                    {
                        e.Graphics.DrawString(lineItem.Description + "(Exp." + lineItem.ExpirationDate + ")", font_Data, Brushes.Black, new Rectangle(xStartItemDescription, tabYStart + itemHeight, widthItemDescription, tabDataHeight), sfAlignLeftCenter);
                        e.Graphics.DrawString(lineItem.Amount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(xStartItemAmount, tabYStart + itemHeight, widthItemDescription, tabDataHeight), sfAlignCenterRight);
                        totalAmount += lineItem.Amount;
                    }
                    else
                    {
                        e.Graphics.DrawString(lineItem.Description, font_Data, Brushes.Black, new Rectangle(xStartItemDescription, tabYStart + itemHeight, widthItemDescription, tabDataHeight), sfAlignLeftCenter);
                    }
                    

                    itemHeight += tabDataHeight;
                    counter++;
                }
            }

            Rectangle rectTotalAmount = new Rectangle(xStartItemAmount, tabYStart + itemHeight, widthItemDescription, tabDataHeight);
            Rectangle rectNote = new Rectangle(50, tabYStart + itemHeight, widthItemDescription, tabDataHeight);

            if (isEnableExpDateChecked)
            {
                e.Graphics.DrawString(totalAmount.ToString("N2"), font_Data, Brushes.Black, rectTotalAmount, sfAlignCenterRight);
            }
            e.Graphics.DrawString("Note: " + note, font_Data, Brushes.Black, rectNote, sfAlignLeftCenter);

            // Signatory
            string Signatory = "Danil Jeus Ampatin";

            Rectangle rectAuthorized = new Rectangle(555, 915, 230, 18);

            //e.Graphics.DrawRectangle(Pens.Black, rectAuthorized);

            e.Graphics.DrawString(Signatory, font_Data, Brushes.Black, rectAuthorized, sfAlignCenter);

        }
    }
}
