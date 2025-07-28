using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void PrintPage_NHC(object sender, PrintPageEventArgs e, int layoutIndex)
        {
            switch (layoutIndex)
            {
                case 1:
                    Layout_SalesInvoice(e);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Layout_SalesInvoice(PrintPageEventArgs e)
        {

            Image image = Properties.Resources.NATURE_SI;
            e.Graphics.DrawImage(image, e.PageBounds);

            Font font_Data = font_EightBold;

            Rectangle rectSoldTo = new Rectangle(145, 140, 285, 20);
            Rectangle rectTIN = new Rectangle(145, 160, 285, 20);
            Rectangle rectBusinessAdd = new Rectangle(145, 180, 285, 25);


            string invoiceSoldTo = "Mercury Drug Corporation";
            string invoiceTin = "000-388-474-0000";
            string invoiceBusinessAdd = "Bacoor City Habay";


            e.Graphics.DrawString(invoiceSoldTo, font_Data, Brushes.Black, rectSoldTo);
            e.Graphics.DrawString(invoiceTin, font_Data, Brushes.Black, rectTIN);
            e.Graphics.DrawString(invoiceBusinessAdd, font_Data, Brushes.Black, rectBusinessAdd);


            Rectangle rectPoNo = new Rectangle(145, 205, 285, 15);
            Rectangle rectStoreCode = new Rectangle(145, 220, 285, 15);
            Rectangle rectTerms = new Rectangle(145, 235, 285, 15);

            string invoicePoNo = "404642500336";
            string invoiceStoreCode = "0464";
            string invoiceTerms = "30 Days";


            e.Graphics.DrawString(invoicePoNo, font_Data, Brushes.Black, rectPoNo);
            e.Graphics.DrawString(invoiceStoreCode, font_Data, Brushes.Black, rectStoreCode);
            e.Graphics.DrawString(invoiceTerms, font_Data, Brushes.Black, rectTerms);







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
            string Signatory = "Kerr Micko Lanante";

            Rectangle rectAuthorized = new Rectangle(555, 945, 230, 18);

            //e.Graphics.DrawRectangle(Pens.Black, rectAuthorized);

            e.Graphics.DrawString(Signatory, font_Data, Brushes.Black, rectAuthorized, sfAlignCenter);

        }
    }
}
