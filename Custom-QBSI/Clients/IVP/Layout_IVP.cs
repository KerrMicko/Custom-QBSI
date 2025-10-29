using Custom_QBSI.Clients.IVP;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Custom_QBSI.Clients.IVP.DataClass_IVP;
using static Custom_QBSI.OtherFunctions;

namespace Custom_QBSI.Clients.IVP
{
    public class Layout_IVP
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
        Font font_Thirteen = new Font("Microsoft Sans Serif", 13, System.Drawing.FontStyle.Regular);
        Font font_ThirteenBold = new Font("Microsoft Sans Serif", 13, System.Drawing.FontStyle.Bold);
        Font font_FourteenBold = new Font("Microsoft Sans Serif", 14, System.Drawing.FontStyle.Bold);
        Font font_SixteenBold = new Font("Microsoft Sans Serif", 16, System.Drawing.FontStyle.Bold);

        StringFormat sfAlignRight = new StringFormat { Alignment = StringAlignment.Far | StringAlignment.Far };
        StringFormat sfAlignCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        StringFormat sfAlignCenterRight = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        StringFormat sfAlignLeftCenter = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        StringFormat sfAlignLeftBottom = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far };

        public void Layout_CollectionReceipt(PrintPageEventArgs e, List<InvoiceData> invoices, string vatType, string businessStyle, bool includeDateIssued, bool isLessEWTChecked, string acNo, DateTime dateIssued, string seriesNumber)
        {
            /*Image image = Properties.Resources.Collection_Receipt_page_0001;
            e.Graphics.DrawImage(image, e.PageBounds);*/

            string payee = invoices[0].CustomerName;
            double TotalAmount = invoices[0].TotalAmount;

            string amountInWords = AmountToWordsConverter.Convert(TotalAmount);
            string Date = invoices[0].TxnDate.ToString("MM/dd/yyyy");


            // 1️⃣ Updated StringFormat
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.Word
            };
            stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            stringFormat.FormatFlags &= ~StringFormatFlags.NoWrap;

            string WrapTextToWidth(Graphics g, string text, Font font, float maxWidth)
            {
                var words = text.Split(' ');
                var sb = new StringBuilder();
                string currentLine = "";

                foreach (var word in words)
                {
                    string testLine = (currentLine.Length == 0) ? word : currentLine + " " + word;
                    var size = g.MeasureString(testLine, font);

                    if (size.Width > (maxWidth - 50))
                    {
                        sb.AppendLine(currentLine + "\n");
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }

                sb.Append(currentLine);
                return sb.ToString();
            }

            string formattedAmountInWords = WrapTextToWidth(e.Graphics, amountInWords, font_Nine, 370);

            if (GlobalVariables.isPrinting)
            {
                e.Graphics.RotateTransform(-90);
                e.Graphics.TranslateTransform(-e.MarginBounds.Height + 180, 0 - 70);

                Rectangle rectpayee = new Rectangle(310, 320, 300, 20);
                Rectangle rectdate = new Rectangle(490, 280, 130, 20);
                Rectangle rectamount = new Rectangle(475, 395, 130, 20);

                //e.Graphics.DrawRectangle(Pens.Red, rectamount);
                //e.Graphics.DrawRectangle(Pens.Blue, rectdate);
                //e.Graphics.DrawRectangle(Pens.Orange, rectpayee);

                e.Graphics.DrawString(payee, font_EightBold, Brushes.Black, rectpayee, sfAlignLeftCenter);
                e.Graphics.DrawString(Date, font_EightBold, Brushes.Black,rectdate , sfAlignCenter);
                e.Graphics.DrawString(TotalAmount.ToString("N2"), font_EightBold, Brushes.Black,rectamount , sfAlignCenterRight);

                int startY = 210;
                int lineHeight = 20;

                string appliedRef = invoices[0].RefNumber;
                string appliedAmt = invoices[0].TotalAmount.ToString("N2");

                Rectangle rectRef = new Rectangle(30, startY, 90, lineHeight);
                Rectangle rectAmt = new Rectangle(105, startY, 90, lineHeight);

                //e.Graphics.DrawRectangle(Pens.Red, rectRef);
                //e.Graphics.DrawRectangle(Pens.Blue, rectAmt);

                e.Graphics.DrawString(appliedRef, font_SevenBold, Brushes.Black, rectRef, sfAlignCenter);
                e.Graphics.DrawString(appliedAmt, font_SevenBold, Brushes.Black, rectAmt, sfAlignCenterRight);

                        
                Rectangle recttotallineamount = new Rectangle(105, startY + 110, 90, lineHeight);

                //e.Graphics.DrawRectangle(Pens.Blue, recttotallineamount);

                e.Graphics.DrawString(TotalAmount.ToString("N2"), font_SevenBold, Brushes.Black, recttotallineamount, sfAlignCenterRight);

                // Amount in words
                Rectangle amountRect = new Rectangle(230, 373, 370, 40);

                //e.Graphics.DrawRectangle(Pens.Blue, amountRect);

                e.Graphics.DrawString("              " + formattedAmountInWords, font_EightBold, Brushes.Black, amountRect, stringFormat);

                Queries_IVP queries_IVP = new Queries_IVP();
                var signatories = queries_IVP.RetrieveSignatory();

                // Prepared by (signature name)
                Rectangle rectPreparedByUnderline = new Rectangle(460, 470, 150, 20);

                //e.Graphics.DrawRectangle(Pens.Blue, rectPreparedByUnderline);

                e.Graphics.DrawString(signatories.preparedBy, font_EightBold, Brushes.Black, rectPreparedByUnderline, sfAlignCenter);
            }
            else
            {
                int offsetX = 20;
                int offsetY = 25;

                // Define rectangles matching the printed layout with offsets
                Rectangle rectPayee = new Rectangle(310 - offsetX, 320 - offsetY, 300, 20);
                Rectangle rectDate = new Rectangle(490 - offsetX, 280 - offsetY, 130, 20);
                Rectangle rectAmount = new Rectangle(475 - offsetX, 395 - offsetY, 130, 20);

                // Optional debug rectangles
                // e.Graphics.DrawRectangle(Pens.Orange, rectPayee);
                // e.Graphics.DrawRectangle(Pens.Blue, rectDate);
                // e.Graphics.DrawRectangle(Pens.Red, rectAmount);

                e.Graphics.DrawString(payee, font_EightBold, Brushes.Black, rectPayee, sfAlignLeftCenter);
                e.Graphics.DrawString(Date, font_EightBold, Brushes.Black, rectDate, sfAlignCenter);
                e.Graphics.DrawString(TotalAmount.ToString("N2"), font_EightBold, Brushes.Black, rectAmount, sfAlignCenterRight);

                // Line items table
                int startY = 210 - offsetY;
                int lineHeight = 20;

                string appliedRef = invoices[0].RefNumber;
                string appliedAmt = invoices[0].TotalAmount.ToString("N2");

                Rectangle rectRef = new Rectangle(30 - offsetX, startY, 90, lineHeight);
                Rectangle rectAmt = new Rectangle(105 - offsetX, startY, 90, lineHeight);

                // Optional debug
                // e.Graphics.DrawRectangle(Pens.Red, rectRef);
                // e.Graphics.DrawRectangle(Pens.Blue, rectAmt);

                e.Graphics.DrawString(appliedRef, font_SevenBold, Brushes.Black, rectRef, sfAlignCenter);
                e.Graphics.DrawString(appliedAmt, font_SevenBold, Brushes.Black, rectAmt, sfAlignCenterRight);

                // Total line amount
                Rectangle rectTotalLineAmount = new Rectangle(105 - offsetX, startY + 110, 90, lineHeight);
                e.Graphics.DrawString(TotalAmount.ToString("N2"), font_SevenBold, Brushes.Black, rectTotalLineAmount, sfAlignCenterRight);

                // Amount in words
                Rectangle amountRect = new Rectangle(230 - offsetX, 373 - offsetY, 370, 40);
                e.Graphics.DrawString("              " + formattedAmountInWords, font_EightBold, Brushes.Black, amountRect, stringFormat);

                Queries_IVP queries_IVP = new Queries_IVP();
                var signatories = queries_IVP.RetrieveSignatory();

                // Prepared by (signature name)
                Rectangle rectPreparedByUnderline = new Rectangle(460 - offsetX, 470 - offsetY, 150, 20);
                e.Graphics.DrawString(signatories.preparedBy, font_EightBold, Brushes.Black, rectPreparedByUnderline, sfAlignCenter);
            }

        }

    }
}

