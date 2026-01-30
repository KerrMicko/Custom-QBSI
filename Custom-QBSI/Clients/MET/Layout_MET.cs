using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Custom_QBSI.Clients.MET.AltDataclass;

namespace Custom_QBSI.Clients.MET
{
    public class Layout_MET
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

        public void Layout_SalesInvoice(PrintPageEventArgs e, List<InvoiceData> invoiceData, string note, string vatType, string businessStyle, string signatoryName, bool isEnableExpDateChecked, bool isLessEWTChecked)
        {

            /*Image image = Properties.Resources.MET_Invoice;
            e.Graphics.DrawImage(image, e.PageBounds);*/

            Font font_Data = font_Eight;

            Rectangle rectDate = new Rectangle(430 + 30, 175 - 30, 120, 25);
            Rectangle rectSoldTo = new Rectangle(50 + 30, 220 - 30, 285, 20);
            Rectangle rectTIN = new Rectangle(50 + 30, 245 - 30, 285, 20);
            Rectangle rectBusinessAdd = new Rectangle(50 + 30, 270 - 30, 485, 25);

            string date = invoiceData[0].TxnDate.ToString("MM/dd/yyyy");
            string invoiceSoldTo = invoiceData[0].CustomerName;
            if (invoiceSoldTo.Contains(":"))
            {
                invoiceSoldTo = invoiceSoldTo.Split(':')[0].Trim();
            }


            string invoiceTin = "";

            foreach (var inv in invoiceData)
            {
                invoiceTin = inv.GetCustomField("TIN");
            }

            string invoiceBusinessAdd = invoiceData[0].ShipAddress1.ToString() + invoiceData[0].ShipAddress2.ToString() + invoiceData[0].ShipAddress3.ToString() + invoiceData[0].ShipAddress4.ToString() + invoiceData[0].ShipAddress5.ToString();


            e.Graphics.DrawString(date, font_Data, Brushes.Black, rectDate, sfAlignCenter);
            e.Graphics.DrawString(invoiceSoldTo, font_Data, Brushes.Black, rectSoldTo);
            e.Graphics.DrawString(invoiceTin, font_Data, Brushes.Black, rectTIN);
            e.Graphics.DrawString(invoiceBusinessAdd, font_Data, Brushes.Black, rectBusinessAdd);


            Rectangle rectTerms = new Rectangle(365 + 30, 245 - 30, 285, 15);
            Rectangle rectPO = new Rectangle(365 + 200, 245 - 30, 285, 15);
            Rectangle rectAmountInWords = new Rectangle(50 + 30, 300 - 30, 500, 40);

            string invoiceTerms = invoiceData[0].Terms.ToString();
            string invoicePO = invoiceData[0].PONumber.ToString();
            double totalamount = invoiceData[0].TotalAmount;
            string amountInWords = AmountToWordsConverter.Convert(totalamount);

            e.Graphics.DrawString(invoiceTerms, font_Data, Brushes.Black, rectTerms);
            e.Graphics.DrawString(invoicePO, font_Data, Brushes.Black, rectPO);
            e.Graphics.DrawString(amountInWords, font_Data, Brushes.Black, rectAmountInWords);

            //MIDDLE TABLE
            int minus1 = 20;
            int minus2 = 10;

            int tab1XStart = 20;
            int tab1YStart = 340;
            int tab1DataHeight = 15; //25

            int widthItemQuantity = 60;
            int widthItemDesc = 340 - minus1;
            int widthItemUnitPrice = 90 - minus2;
            int widthItemAmount = 120 - minus2;

            int xStartItemQty = tab1XStart + 330 ;
            int xStartItemDesc = tab1XStart ;
            int xStartItemUnitPrice = tab1XStart + xStartItemQty + xStartItemDesc + 61 - minus1 - 15;
            int xStartItemAmount = tab1XStart + xStartItemUnitPrice + 71 - minus2;

            Rectangle rectItemQuantity = new Rectangle(xStartItemQty, tab1YStart, widthItemQuantity, tab1DataHeight);
            Rectangle rectItemDescription = new Rectangle(xStartItemDesc, tab1YStart, widthItemDesc, tab1DataHeight);
            Rectangle rectItemUnitPrice = new Rectangle(xStartItemUnitPrice, tab1YStart, widthItemUnitPrice, tab1DataHeight);
            Rectangle rectItemAmount = new Rectangle(xStartItemAmount, tab1YStart, widthItemAmount, tab1DataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectItemQuantity);
            e.Graphics.DrawRectangle(Pens.Yellow, rectItemDescription);
            e.Graphics.DrawRectangle(Pens.Orange, rectItemUnitPrice);
            e.Graphics.DrawRectangle(Pens.Pink, rectItemAmount);*/

            int itemHeight = 0;
            decimal totalAmount = 0;
            decimal vatableSalesTotal = 0;
            decimal zeroRatedSalesTotal = 0;
            decimal vatExemptSalesTotal = 0;

            foreach (var invoice in invoiceData)
            {
               
                if (vatType == "Exclusive" || vatType == "Inclusive")
                {
                    for (int i = 0; i < invoice.Lines.Count; i++)
                    {
                        var item = invoice.Lines[i];
                        if (item.Description == "Subtotal") continue;

                        bool hasNext = i + 1 < invoice.Lines.Count;
                        var nextItem = hasNext ? invoice.Lines[i + 1] : null;

                        bool isDiscountLine = (item.Description != null && item.Description.ToLower().Contains("discount"))
                                              || item.Amount < 0
                                              || item.Rate.ToString().Contains("%");

                        decimal adjustedAmount = item.Amount; // Exclusive VAT, amount is always raw
                        decimal unitRateToDraw = item.Rate;

                        // Base description with expiry
                        string descText = item.Description;
                        if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.SkuCode))
                            descText = item.SkuCode + " " + descText;
                        if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.ExpirationDate))
                            descText += " (Exp. " + item.ExpirationDate + ")";

                        // Measure description height
                        SizeF descSize = e.Graphics.MeasureString(descText, font_Data, rectItemDescription.Width);
                        int rowHeight = Math.Max((int)Math.Ceiling(descSize.Height), tab1DataHeight);

                        // Classification
                        if (invoice.TaxesName == "Zero rated sales")
                            zeroRatedSalesTotal += item.Amount;
                        else if (invoice.TaxesName == "Vat Exempt")
                            vatExemptSalesTotal += item.Amount;
                        else if (!isDiscountLine && item.Tax != "Non")
                            vatableSalesTotal += item.Amount;

                        // ---------- Case 1: Discount ----------
                        if (isDiscountLine)
                        {
                            // Added -30 to Y to match regular lines and used rowHeight
                            e.Graphics.DrawString(descText, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X + 45, rectItemDescription.Y - 30 + itemHeight, rectItemDescription.Width, rowHeight), sfAlignLeftCenter);

                            // Usually discounts don't have qty/rate, but we ensure the Amount aligns
                            e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X + 45, rectItemAmount.Y - 30 + itemHeight, rectItemAmount.Width, rowHeight), sfAlignCenterRight);

                            totalAmount += adjustedAmount;
                            itemHeight += rowHeight;
                            continue;
                        }

                        // ---------- Case 2: Description-only ----------
                        if (item.Rate == 0 && i > 0 && invoice.Lines[i - 1].Description == "Subtotal")
                        {
                            e.Graphics.DrawString(descText, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X + 45, rectItemDescription.Y - 30 + itemHeight, rectItemDescription.Width, rowHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X + 45, rectItemAmount.Y - 30 + itemHeight, rectItemAmount.Width, rowHeight), sfAlignCenterRight);

                            totalAmount += adjustedAmount;
                            itemHeight += rowHeight;
                            continue;
                        }

                        // ---------- Case 3: Combined line ----------
                        if (item.Rate == 0 && hasNext && nextItem.Rate == 0 && nextItem.Description != "Subtotal")
                        {
                            decimal combinedAmount = adjustedAmount + nextItem.Amount;
                            string desc = $"{item.Description} ({nextItem.Description})";
                            if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.SkuCode))
                                desc = item.SkuCode + " " + desc;
                            if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.ExpirationDate))
                                desc += " (Exp. " + item.ExpirationDate + ")";

                            SizeF descSize2 = e.Graphics.MeasureString(desc, font_Data, rectItemDescription.Width);
                            int rowHeight2 = Math.Max((int)Math.Ceiling(descSize2.Height), tab1DataHeight);

                            e.Graphics.DrawString(desc, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X + 45, rectItemDescription.Y - 30 + itemHeight, rectItemDescription.Width, rowHeight2), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Quantity.ToString(""), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X + 45, rectItemQuantity.Y - 30 + itemHeight, rectItemQuantity.Width, rowHeight2), sfAlignCenter);
                            e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X + 45, rectItemUnitPrice.Y - 30 + itemHeight, rectItemUnitPrice.Width, rowHeight2), sfAlignCenterRight);
                            e.Graphics.DrawString(combinedAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X + 45, rectItemAmount.Y - 30 + itemHeight, rectItemAmount.Width, rowHeight2), sfAlignCenterRight);

                            totalAmount += combinedAmount;
                            itemHeight += rowHeight2;
                            i++;
                            continue;
                        }

                        // ---------- Case 4: Regular ----------
                        e.Graphics.DrawString(descText, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X + 45, rectItemDescription.Y - 30 + itemHeight, rectItemDescription.Width, rowHeight), sfAlignLeftCenter);
                        e.Graphics.DrawString(item.Quantity.ToString(""), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X + 45, rectItemQuantity.Y - 30 + itemHeight, rectItemQuantity.Width, rowHeight), sfAlignCenter);
                        e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X + 45, rectItemUnitPrice.Y - 30 + itemHeight, rectItemUnitPrice.Width, rowHeight), sfAlignCenterRight);
                        e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X + 45 , rectItemAmount.Y - 30 + itemHeight, rectItemAmount.Width, rowHeight), sfAlignCenterRight);

                        totalAmount += adjustedAmount;
                        itemHeight += rowHeight;
                    }
                }

                itemHeight += tab1DataHeight; // Extra spacing per invoice
            }


            string invoicenote = note;

            Rectangle rectNote = new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + itemHeight, rectItemQuantity.Width + 500, tab1DataHeight);

            /*e.Graphics.DrawRectangle(Pens.Yellow, rectNote);*/
            if (!string.IsNullOrEmpty(invoicenote))
            {
                e.Graphics.DrawString("Note: " + invoicenote, font_EightBold, Brushes.Black, rectNote, sfAlignLeftCenter);
            }


            // LEFT TABLE
            int dataHeight = 20;
            int xStart = 130 + 30;
            int yStart = 628 + 12 * 3; // 770
            int width = 170 - 20;

            Rectangle rectVATableSales = new Rectangle(xStart, yStart - 30, width, dataHeight);
            Rectangle rectVatAmount = new Rectangle(xStart, yStart - 30 + dataHeight, width, dataHeight);
            Rectangle rectZeroRatedSales = new Rectangle(xStart, yStart - 30 - 3 + dataHeight * 2, width, dataHeight);
            Rectangle rectVATExemptSales = new Rectangle(xStart, yStart - 30 - 3 + dataHeight * 3, width, dataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectVATableSales);
            e.Graphics.DrawRectangle(Pens.Blue, rectVATExemptSales);
            e.Graphics.DrawRectangle(Pens.Yellow, rectZeroRatedSales);
            e.Graphics.DrawRectangle(Pens.Orange, rectVatAmount);*/


            decimal totalVATableAmount = 0;

            // Step 1: sum adjusted amounts
            foreach (var invoice in invoiceData)
            {
                foreach (var line in invoice.Lines)
                {
                    bool isLineVATable = line.Tax != "Non" || invoice.TaxesName == "Output Tax";
                    if (isLineVATable)
                    {
                        totalVATableAmount += line.Amount;
                    }
                }
            }

            decimal totalVATableAmount2 = totalVATableAmount;



            decimal amountNetVat = totalVATableAmount2 / 1.12m;
            decimal totalVAT = totalVATableAmount2 - amountNetVat;

            if (vatType == "Inclusive")
            {
               if (amountNetVat > 0)
                   e.Graphics.DrawString(amountNetVat.ToString("N2"), font_Data, Brushes.Black, rectVATableSales, sfAlignCenterRight);

               if (totalVAT > 0)
                   e.Graphics.DrawString(totalVAT.ToString("N2"), font_Data, Brushes.Black, rectVatAmount, sfAlignCenterRight);
            }
            else if (vatType == "Exclusive")
            {

                decimal ewtAmount = totalVATableAmount2 * 0.01m;
                if (isLessEWTChecked)
                    totalVATableAmount2 -= ewtAmount;

                decimal totalAmountDueEWT = totalVATableAmount2;

                if (isLessEWTChecked)
                {
                    decimal vatExemptSalesTotal2 = totalAmountDueEWT + ewtAmount;

                    if (amountNetVat > 0)
                        e.Graphics.DrawString(vatExemptSalesTotal2.ToString("N2"), font_Data, Brushes.Black, rectVATExemptSales, sfAlignCenterRight);
                }
                else
                {
                    decimal vatExemptSalesTotal2 = totalAmountDueEWT;

                    if (amountNetVat > 0)
                        e.Graphics.DrawString(vatExemptSalesTotal2.ToString("N2"), font_Data, Brushes.Black, rectVATExemptSales, sfAlignCenterRight);
                }
                
            }



            // RIGHT TABLE RECTANGLES
            int tab2RightDataHeight = 20;
            int tab2RightXStart = 425 + 30;
            int tab2RightYStart = 548 + tab2RightDataHeight * 3;
            int tab2RightWidth = 210 - 30;

            Rectangle rectR1TotalSales = new Rectangle(tab2RightXStart, tab2RightYStart - 25 - 12 + tab2RightDataHeight * 0, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR2LessVAT = new Rectangle(tab2RightXStart, tab2RightYStart - 25 - 12 + tab2RightDataHeight * 1, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR3AmountNetofVAT = new Rectangle(tab2RightXStart, tab2RightYStart - 25 - 12 + tab2RightDataHeight * 2, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR4LessDiscount = new Rectangle(tab2RightXStart, tab2RightYStart - 25 - 12 + tab2RightDataHeight * 3, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR5AddVAT = new Rectangle(tab2RightXStart, tab2RightYStart - 25 - 12 + tab2RightDataHeight * 4, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR6LessWithholdingTax = new Rectangle(tab2RightXStart, tab2RightYStart - 25 - 12 + tab2RightDataHeight * 5, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR7TotalAmountDue = new Rectangle(tab2RightXStart, tab2RightYStart - 25 - 12 + tab2RightDataHeight * 6, tab2RightWidth, tab2RightDataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectR1TotalSales);
            e.Graphics.DrawRectangle(Pens.Orange, rectR2LessVAT);
            e.Graphics.DrawRectangle(Pens.Yellow, rectR3AmountNetofVAT);
            e.Graphics.DrawRectangle(Pens.Green, rectR4LessDiscount);
            e.Graphics.DrawRectangle(Pens.Blue, rectR5AddVAT);
            e.Graphics.DrawRectangle(Pens.Indigo, rectR6LessWithholdingTax);
            e.Graphics.DrawRectangle(Pens.Violet, rectR7TotalAmountDue);*/

            // -------------------- CALCULATION --------------------

            decimal totalSales = 0;

            foreach (var invoice in invoiceData)
            {
                foreach (var line in invoice.Lines)
                {
                    totalSales += line.Amount;
                }
            }

            decimal totalSales2 = totalSales;
            decimal amountNetVat2 = totalSales2 / 1.12m;
            decimal totalVAT2 = totalSales2 - amountNetVat2;

            // -------------------- R1 Total Sales --------------------
            if (vatType == "Inclusive")
            {
                if (totalSales > 0)
                    e.Graphics.DrawString(totalSales2.ToString("N2"), font_Data, Brushes.Black, rectR1TotalSales, sfAlignCenterRight);

                // -------------------- R3 Amount Net of VAT --------------------
                if (amountNetVat2 > 0)
                    e.Graphics.DrawString(amountNetVat2.ToString("N2"), font_Data, Brushes.Black, rectR3AmountNetofVAT, sfAlignCenterRight);


                if (totalVAT2 > 0)
                    e.Graphics.DrawString(totalVAT2.ToString("N2"), font_Data, Brushes.Black, rectR2LessVAT, sfAlignCenterRight);
                // -------------------- R4 Less Discount (blank) --------------------
                // e.Graphics.DrawString("", font_Data, Brushes.Black, rectR4LessDiscount, sfAlignCenterRight);

                // -------------------- R5 Add VAT --------------------
                /*if (totalVAT2 > 0)
                    e.Graphics.DrawString(totalVAT2.ToString("N2"), font_Data, Brushes.Black, rectR5AddVAT, sfAlignCenterRight);*/

                // -------------------- R6 Less Withholding Tax (EWT 1%) --------------------
                decimal ewtAmount = 0;
                if (isLessEWTChecked)
                {
                    ewtAmount = amountNetVat2 * 0.01m;
                    e.Graphics.DrawString(ewtAmount.ToString("N2"), font_Data, Brushes.Black, rectR6LessWithholdingTax, sfAlignCenterRight);
                }

                // -------------------- R7 Total Amount Due --------------------
                decimal totalAmountDue = totalSales2;
                if (isLessEWTChecked)
                    totalAmountDue -= ewtAmount;

                if (totalAmountDue > 0)
                    e.Graphics.DrawString(totalAmountDue.ToString("N2"), font_EightBold, Brushes.Black, rectR7TotalAmountDue, sfAlignCenterRight);
            }
            else if (vatType == "Exclusive")
            {
                decimal ewtAmount = 0;
                if (isLessEWTChecked)
                {
                    ewtAmount = totalSales * 0.01m;
                    e.Graphics.DrawString(ewtAmount.ToString("N2"), font_Data, Brushes.Black, rectR6LessWithholdingTax, sfAlignCenterRight);
                }

                // -------------------- R7 Total Amount Due --------------------
                decimal totalAmountDue = totalSales;
                if (isLessEWTChecked)
                    totalAmountDue -= ewtAmount;

                if (totalAmountDue > 0)
                    e.Graphics.DrawString(totalAmountDue.ToString("N2"), font_EightBold, Brushes.Black, rectR7TotalAmountDue, sfAlignCenterRight);
            }


            


            // Signatory
            string Signatory = signatoryName;

            Rectangle rectAuthorized = new Rectangle(340, 740 + 40, 230, 18);

            //e.Graphics.DrawRectangle(Pens.Black, rectAuthorized);

            e.Graphics.DrawString(Signatory, font_Data, Brushes.Black, rectAuthorized, sfAlignCenter);


        }

        public void Layout_DeliveryReceipt(PrintPageEventArgs e,List<InvoiceData> invoiceData, string note,string businessStyle,string pwdSignature,string address,string terms,string storeCode,string poNumber,string tin,bool isEnableExpDateChecked,bool allowPriceEditing, string customerName,string signatoryName,DataGridView dataGridView = null)
        {
            /*Image image = Properties.Resources.viber_image_2026_01_14_13_36_47_109;
            e.Graphics.DrawImage(image, e.PageBounds);*/

            Font font_Data = font_Eight;

            Rectangle rectDate = new Rectangle(470, 135 + 10, 120, 25);
            Rectangle rectSoldTo = new Rectangle(150, 135 + 10, 285, 20);
            Rectangle rectBusinessStyle = new Rectangle(150, 138 + 10, 205, 20);
            Rectangle rectTIN = new Rectangle(125, 155 + 10, 285, 20);
            Rectangle rectBusinessAdd = new Rectangle(130, 175 + 10, 500, 25);

            string date = invoiceData[0].DueDate?.ToString("MM/dd/yyyy") ?? "";
            string invoiceTin = "";

            foreach (var inv in invoiceData)
            {
                invoiceTin = inv.GetCustomField("TIN");
            }

            string invoiceBusinessStyle = businessStyle;
            string invoiceBusinessAdd = invoiceData[0].ShipAddress1.ToString() + invoiceData[0].ShipAddress2.ToString() + invoiceData[0].ShipAddress3.ToString() + invoiceData[0].ShipAddress4.ToString() + invoiceData[0].ShipAddress5.ToString();
            string invoiceSoldTo = invoiceData[0].CustomerName;
            if (invoiceSoldTo.Contains(":"))
            {
                invoiceSoldTo = invoiceSoldTo.Split(':')[0].Trim();
            }


            e.Graphics.DrawString(invoiceSoldTo, font_Data, Brushes.Black, rectSoldTo);
            e.Graphics.DrawString(date, font_Data, Brushes.Black, rectDate, sfAlignCenter);
            e.Graphics.DrawString(invoiceTin, font_Data, Brushes.Black, rectTIN);
            e.Graphics.DrawString(invoiceBusinessStyle, font_Data, Brushes.Black, rectBusinessStyle);
            e.Graphics.DrawString(invoiceBusinessAdd, font_Data, Brushes.Black, rectBusinessAdd);

            Rectangle rectPoNo = new Rectangle(125, 205 + 10, 285, 15);
            Rectangle rectTerms = new Rectangle(500, 155 + 10, 285, 15);

            /*e.Graphics.DrawRectangle(Pens.Red, rectPoNo);
            e.Graphics.DrawRectangle(Pens.Yellow, rectStoreCode);
            e.Graphics.DrawRectangle(Pens.Pink, rectTerms);*/

            string invoiceTerms = invoiceData[0].Terms.ToString();
            string invoicePoNo = invoiceData[0].PONumber.ToString();

            e.Graphics.DrawString(invoicePoNo, font_Data, Brushes.Black, rectPoNo);
            e.Graphics.DrawString(invoiceTerms, font_Data, Brushes.Black, rectTerms);

            // TABLE MIDDLE CALCULATION
            int tabXStart = 50;
            int tabYStart = 285;

            int tabDataHeight = 23;

            int widthItemNo = 67;
            int widthItemQuantity = 75;
            int widthItemUnit = 45;
            int widthItemDescription = 520;

            int xStartItemQuantity = tabXStart;
            int xStartItemUnit = tabXStart + widthItemQuantity;
            int xStartItemDescription = tabXStart + widthItemNo + widthItemQuantity + widthItemUnit - 67;
            int xStartItemAmount = tabXStart + widthItemNo + widthItemQuantity + widthItemUnit - 67;

            Rectangle rectItemQuantity = new Rectangle(xStartItemQuantity, tabYStart, widthItemQuantity, tabDataHeight);
            Rectangle rectItemUnit = new Rectangle(xStartItemUnit, tabYStart, widthItemUnit, tabDataHeight);
            Rectangle rectItemDescription = new Rectangle(xStartItemDescription, tabYStart, widthItemDescription, tabDataHeight);
            Rectangle rectItemAmount = new Rectangle(xStartItemAmount, tabYStart, widthItemDescription, tabDataHeight);

            int itemHeight = 0;

            foreach (var invoice in invoiceData)
            {
                int lineIndex = 0;
                decimal totalAmount = 0m; // Sum of all REAL transfer amounts

                foreach (var lineItem in invoice.Lines)
                {
                    string expDateEdited = null;
                    string priceEdited = null;

                    // 🔹 Read user edits from DataGridView (if available)
                    if (dataGridView != null && dataGridView.Visible && lineIndex < dataGridView.Rows.Count)
                    {
                        var row = dataGridView.Rows[lineIndex];
                        if (!row.IsNewRow)
                        {
                            expDateEdited = row.Cells["ExpirationDate"]?.Value?.ToString();
                            priceEdited = row.Cells["Price"]?.Value?.ToString();
                        }
                    }

                    // 🔹 Build item description including EXP date
                    string description = lineItem.Description ?? "";
                    string finalDescription = description;

                    if (isEnableExpDateChecked && !string.IsNullOrWhiteSpace(expDateEdited))
                        finalDescription += $" (EXP: {expDateEdited})";
                    else if (isEnableExpDateChecked && !string.IsNullOrWhiteSpace(lineItem.ExpirationDate))
                        finalDescription += $" (EXP: {lineItem.ExpirationDate})";

                    SizeF descSize = e.Graphics.MeasureString(finalDescription, font_Data, widthItemDescription);
                    int rowHeight = Math.Max(tabDataHeight, (int)Math.Ceiling(descSize.Height));

                    // 🔹 Draw Quantity
                    e.Graphics.DrawString(lineItem.Quantity.ToString(), font_Data, Brushes.Black,
                        new Rectangle(xStartItemQuantity + 20, tabYStart - 20 + itemHeight, widthItemQuantity, rowHeight), sfAlignCenter);

                    // 🔹 Draw Unit
                    e.Graphics.DrawString(lineItem.UnitOfMeasure, font_Data, Brushes.Black,
                        new Rectangle(xStartItemUnit + 20, tabYStart - 20 + itemHeight, widthItemUnit, rowHeight), sfAlignCenter);

                    // 🔹 Draw Description (with EXP)
                    e.Graphics.DrawString(finalDescription, font_Data, Brushes.Black,
                        new Rectangle(xStartItemDescription + 20, tabYStart - 20 + itemHeight, widthItemDescription, rowHeight), sfAlignLeftCenter);

                    // 🔹 Handle printing Amount column
                    if (isEnableExpDateChecked)
                    {
                        // Default = correct transfer amount from QuickBooks:
                        decimal itemAmount = (decimal)lineItem.Amount;

                        // If editing is allowed and user typed a new price → override
                        if (allowPriceEditing &&
                            !string.IsNullOrWhiteSpace(priceEdited) &&
                            decimal.TryParse(priceEdited, out decimal edited))
                        {
                            itemAmount = edited;
                        }

                        // Draw amount
                        Rectangle amountRect = new Rectangle(
                            xStartItemAmount - 100 + widthItemDescription - 40,
                            tabYStart - 20 + itemHeight,
                            100,
                            rowHeight
                        );

                        e.Graphics.DrawString(itemAmount.ToString("N2"), font_Data, Brushes.Black, amountRect, sfAlignCenterRight);

                        // Add to total
                        totalAmount += itemAmount;
                    }

                    itemHeight += rowHeight;
                    lineIndex++;
                }

                // 🔹 Print TOTAL
                if (isEnableExpDateChecked)
                {
                    decimal printedTotal = totalAmount;

                    // Check if user edited TOTAL row
                    if (allowPriceEditing && dataGridView != null && dataGridView.Visible)
                    {
                        foreach (DataGridViewRow row in dataGridView.Rows)
                        {
                            if (!row.IsNewRow && row.Cells["Description"]?.Value?.ToString() == "TOTAL")
                            {
                                string editedTotalText = row.Cells["Price"]?.Value?.ToString();
                                if (decimal.TryParse(editedTotalText, out decimal editedTotal))
                                {
                                    printedTotal = editedTotal;
                                }
                                break;
                            }
                        }
                    }

                    // Draw Total Line
                    int totalRowHeight = 25;
                    int totalY = tabYStart + itemHeight + 10;

                    Rectangle totalLabel = new Rectangle(
                        xStartItemAmount - 100 + widthItemDescription - 140,
                        totalY,
                        100,
                        totalRowHeight
                    );

                    Rectangle totalValue = new Rectangle(
                        xStartItemAmount - 100 + widthItemDescription - 40,
                        totalY,
                        100,
                        totalRowHeight
                    );

                    e.Graphics.DrawString("TOTAL:", font_EightBold, Brushes.Black, totalLabel, sfAlignCenterRight);
                    e.Graphics.DrawString(printedTotal.ToString("N2"), font_EightBold, Brushes.Black, totalValue, sfAlignCenterRight);

                    itemHeight += totalRowHeight + 10;
                }
            }





            // Draw Note
            if (!string.IsNullOrEmpty(note))
            {
                Rectangle rectNote = new Rectangle(50, tabYStart + itemHeight, widthItemDescription, tabDataHeight);
                e.Graphics.DrawString("Note: " + note, font_EightBold, Brushes.Black, rectNote, sfAlignLeftCenter);
            }


            // Signatory
            string Signatory = signatoryName;

            Rectangle rectAuthorized = new Rectangle(555, 915, 230, 18);

            //e.Graphics.DrawRectangle(Pens.Black, rectAuthorized);

            e.Graphics.DrawString(Signatory, font_Data, Brushes.Black, rectAuthorized, sfAlignCenter);

        }


        public class AmountToWordsConverter
        {
            private static string[] units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            private static string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            private static string[] tens = { "", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            private static string[] thousandsGroups = { "", " Thousand", " Million", " Billion" };

            public static string Convert(double amount)
            {
                if (amount == 0)
                    return "Zero Pesos Only";

                if (amount < 0)
                    return "Negative amount, cannot convert to words";

                int pesos = (int)Math.Floor(amount);
                int centavos = (int)Math.Round((amount - pesos) * 100);

                string pesoWords = ConvertToWords(pesos);
                string centavoWords = ConvertToWords(centavos);

                string result = "";
                if (centavos > 0)
                {
                    result = pesoWords + " Pesos";
                    //result = pesoWords + " and " + centavos + "/100 Pesos Only"; kanan terrys
                    result += " and " + centavoWords + " Centavos Only";
                }
                else
                {
                    result = pesoWords + " Pesos Only";
                }

                return result;
            }

            private static string ConvertToWords(int number)
            {
                if (number == 0)
                    return "Zero";

                if (number < 0)
                    return "Negative " + ConvertToWords(Math.Abs(number));

                string words = "";

                for (int i = 0; number > 0; i++)
                {
                    if (number % 1000 != 0)
                    {
                        words = ConvertHundreds(number % 1000) + thousandsGroups[i] + " " + words;
                    }
                    number /= 1000;
                }

                return words.Trim();
            }

            private static string ConvertHundreds(int number)
            {
                string words = "";

                if (number >= 100)
                {
                    words += units[number / 100] + " Hundred ";
                    number %= 100;
                }

                if (number >= 10 && number <= 19)
                {
                    words += teens[number - 10] + " ";
                    number = 0;
                }

                if (number >= 20)
                {
                    words += tens[number / 10] + " ";
                    number %= 10;
                }

                if (number >= 1 && number <= 9)
                {
                    words += units[number] + " ";
                }

                return words;
            }
        }
    }
}
