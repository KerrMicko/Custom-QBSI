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

        public void Layout_CollectionReceipt(PrintPageEventArgs e, List<ReceivePaymentData> payments, string vatType, string businessStyle, bool includeDateIssued, bool isLessEWTChecked, string acNo, DateTime dateIssued, string seriesNumber)
        {
            Image image = Properties.Resources.Collection_Receipt_page_0001;
            e.Graphics.DrawImage(image, e.PageBounds);

            string payee = payments[0].CustomerName;
            double amount = payments[0].TotalAmount;

            string amountInWords = AmountToWordsConverter.Convert(amount);
            string Date = payments[0].TxnDate.ToString("MM/dd/yyyy");


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

                e.Graphics.DrawString(payee, font_Ten, Brushes.Black, new PointF(310, 320));
                e.Graphics.DrawString(Date, font_Ten, Brushes.Black, new PointF(510, 280));
                e.Graphics.DrawString(amount.ToString("N2"), font_Ten, Brushes.Black, new PointF(550, 395));

                RectangleF amountRect = new RectangleF(230, 373, 370, 80);
                e.Graphics.DrawString("              " + formattedAmountInWords, font_Nine, Brushes.Black, amountRect, stringFormat);

                Queries_IVP queries_IVP = new Queries_IVP();
                var signatories = queries_IVP.RetrieveSignatory();

                RectangleF rectPreparedByUnderline = new RectangleF(230, 440, 615, 80);

                e.Graphics.DrawString(signatories.preparedBy, font_Nine, Brushes.Black, rectPreparedByUnderline, sfAlignCenter);

            }
            else
            {
                int offsetX = 20;
                int offsetY = 25;

                e.Graphics.DrawString(payee, font_Ten, Brushes.Black, new PointF(310 - offsetX, 320 - offsetY));
                e.Graphics.DrawString(Date, font_Ten, Brushes.Black, new PointF(510 - offsetX, 280 - offsetY));
                e.Graphics.DrawString(amount.ToString("N2"), font_Ten, Brushes.Black, new PointF(550 - offsetX, 395 - offsetY));

                RectangleF amountRect = new RectangleF(230 - offsetX, 375 - offsetY, 370, 80);
                e.Graphics.DrawString("              " + formattedAmountInWords, font_Nine, Brushes.Black, amountRect, stringFormat);

                Queries_IVP queries_IVP = new Queries_IVP();
                var signatories = queries_IVP.RetrieveSignatory();

                RectangleF rectPreparedByUnderline = new RectangleF(230, 440, 615, 80);

                e.Graphics.DrawString(signatories.preparedBy, font_Nine, Brushes.Black, rectPreparedByUnderline, sfAlignCenter);


            }

        }

    }
}

