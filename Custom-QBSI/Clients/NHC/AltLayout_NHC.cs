using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Custom_QBSI.Clients.NHC.AltDataClass_NHC;

namespace Custom_QBSI.Clients.NHC
{
    public class AltLayout_NHC
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

        public void Layout_SalesInvoice(PrintPageEventArgs e, List<InvoiceData> invoiceData, string note, string vatType, string businessStyle, string signatoryName,bool isEnableExpDateChecked, bool isLessEWTChecked)
        {

            /* Image image = Properties.Resources.NATURE_SI;
            e.Graphics.DrawImage(image, e.PageBounds);*/

            Font font_Data = font_Eight;

            Rectangle rectDate = new Rectangle(670 - 10, 110, 120 - 10, 25);
            Rectangle rectSoldTo = new Rectangle(145, 140, 285, 20);
            Rectangle rectBusinessStyle = new Rectangle(325, 160, 285, 20);
            Rectangle rectTIN = new Rectangle(145, 160, 285, 20);
            Rectangle rectBusinessAdd = new Rectangle(145, 180, 485, 25);


            string refNumber = invoiceData[0].RefNumber.ToString();
            string date = invoiceData[0].TxnDate.ToString("MM/dd/yyyy");
            string invoiceSoldTo = invoiceData[0].CustomerName.ToString();

            string invoiceTin = "";
            string invoiceBusinessStyle = "";
            string invoiceStoreCode = "";

            foreach (var inv in invoiceData)
            {
                invoiceTin = inv.GetCustomField("TIN");
                if (businessStyle == "")
                {
                    invoiceBusinessStyle = inv.GetCustomField("BUSINESS STYLE");
                }
                else
                {
                    invoiceBusinessStyle = businessStyle;
                }
                invoiceStoreCode = inv.GetCustomField("Store Code");
            }

            string invoiceBusinessAdd = invoiceData[0].ShipAddress1.ToString() + invoiceData[0].ShipAddress2.ToString() + invoiceData[0].ShipAddress3.ToString() + invoiceData[0].ShipAddress4.ToString() + invoiceData[0].ShipAddress5.ToString();


            e.Graphics.DrawString(date, font_Data, Brushes.Black, rectDate, sfAlignCenter);
            e.Graphics.DrawString(invoiceSoldTo, font_Data, Brushes.Black, rectSoldTo);
            e.Graphics.DrawString(invoiceBusinessStyle, font_Data, Brushes.Black, rectBusinessStyle);
            e.Graphics.DrawString(invoiceTin, font_Data, Brushes.Black, rectTIN);
            e.Graphics.DrawString(invoiceBusinessAdd, font_Data, Brushes.Black, rectBusinessAdd);


            Rectangle rectPoNo = new Rectangle(145, 205, 285, 15);
            Rectangle rectStoreCode = new Rectangle(145, 220, 285, 15);
            Rectangle rectTerms = new Rectangle(145, 235, 285, 15);

            string invoicePoNo = invoiceData[0].PONumber.ToString();
            string invoiceTerms = invoiceData[0].Terms.ToString();

            e.Graphics.DrawString(invoicePoNo, font_Data, Brushes.Black, rectPoNo);
            e.Graphics.DrawString(invoiceStoreCode, font_Data, Brushes.Black, rectStoreCode);
            e.Graphics.DrawString(invoiceTerms, font_Data, Brushes.Black, rectTerms);


            //MIDDLE TABLE
            int minus1 = 20;
            int minus2 = 10;

            int tab1XStart = 68;
            int tab1YStart = 280;
            int tab1DataHeight = 25;

            int widthItemQuantity = 75;
            int widthItemUOM = 67;
            int widthItemDesc = 340 - minus1;
            int widthItemUnitPrice = 119 - minus2;
            int widthItemAmount = 120 - minus2;

            int xStartItemQty = tab1XStart;
            int xStartItemUOM = tab1XStart + xStartItemQty + 8;
            int xStartItemDesc = tab1XStart + xStartItemUOM;
            int xStartItemUnitPrice = tab1XStart + xStartItemQty + xStartItemUOM + xStartItemDesc + 61 - minus1;
            int xStartItemAmount = tab1XStart + xStartItemUnitPrice + 51 - minus2;

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

            int itemHeight = 0;
            decimal totalAmount = 0;
            decimal vatableSalesTotal = 0;
            decimal zeroRatedSalesTotal = 0;
            decimal vatExemptSalesTotal = 0;

            foreach (var invoice in invoiceData)
            {
                if (vatType == "Inclusive")
                {
                    for (int i = 0; i < invoice.Lines.Count; i++)
                    {
                        var item = invoice.Lines[i];
                        if (item.Description == "Subtotal") continue;

                        bool hasNext = i + 1 < invoice.Lines.Count;
                        var nextItem = hasNext ? invoice.Lines[i + 1] : null;

                        bool isTaxable = invoice.TaxesName == "Vat";
                        bool isDiscountLine = (item.Description != null && item.Description.ToLower().Contains("discount"))
                                              || item.Amount < 0
                                              || item.Rate.ToString().Contains("%");

                        // Adjusted amount (VAT only if not discount)
                        decimal adjustedAmount = isDiscountLine ? item.Amount : (isTaxable ? item.Amount * 1.12m : item.Amount);
                        decimal rateAdjustment = isTaxable ? item.Rate * 0.12m : 0m;
                        decimal unitRateToDraw = isDiscountLine ? item.Rate : item.Rate + rateAdjustment;

                        // Base description with expiry
                        string descText = item.Description;
                        if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.SkuCode))
                            descText = item.SkuCode + " " + descText;
                        if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.ExpirationDate))
                            descText += " (Exp. " + item.ExpirationDate + ")";

                        // Measure description height
                        SizeF descSize = e.Graphics.MeasureString(descText, font_Data, widthItemDesc);
                        int rowHeight = Math.Max((int)Math.Ceiling(descSize.Height), tab1DataHeight);

                        // Classification
                        if (invoice.TaxesName == "Zero rated sales")
                            zeroRatedSalesTotal += item.Amount;
                        else if (invoice.TaxesName == "Vat Exempt")
                            vatExemptSalesTotal += item.Amount;
                        else if (!isDiscountLine && isTaxable)
                            vatableSalesTotal += item.Amount;

                        // ---------- Case 1: Discount line ----------
                        if (isDiscountLine)
                        {
                            e.Graphics.DrawString(descText, font_Data, Brushes.Black,new Rectangle(xStartItemDesc, tab1YStart + itemHeight, widthItemDesc, rowHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemAmount, tab1YStart + itemHeight, widthItemAmount, rowHeight), sfAlignCenterRight);

                            totalAmount += adjustedAmount;
                            itemHeight += rowHeight;
                            continue;
                        }

                        // ---------- Case 2: Description-only after Subtotal ----------
                        if (item.Rate == 0 && i > 0 && invoice.Lines[i - 1].Description == "Subtotal")
                        {
                            e.Graphics.DrawString(descText, font_Data, Brushes.Black,new Rectangle(xStartItemDesc, tab1YStart + itemHeight, widthItemDesc, rowHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemAmount, tab1YStart + itemHeight, widthItemAmount, rowHeight), sfAlignCenterRight);

                            totalAmount += adjustedAmount;
                            itemHeight += rowHeight;
                            continue;
                        }

                        // ---------- Case 3: Combined line ----------
                        if (item.Rate == 0 && hasNext && nextItem.Rate == 0 && nextItem.Description != "Subtotal")
                        {
                            decimal combinedAmount = adjustedAmount + (nextItem.Description.ToLower().Contains("discount") ? nextItem.Amount : (isTaxable ? nextItem.Amount * 1.12m : nextItem.Amount));
                            string desc = $"{item.Description} ({nextItem.Description})";
                            if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.SkuCode))
                                desc = item.SkuCode + " " + desc;
                            if (isEnableExpDateChecked && !string.IsNullOrEmpty(item.ExpirationDate))
                                desc += " (Exp. " + item.ExpirationDate + ")";

                            SizeF descSize2 = e.Graphics.MeasureString(desc, font_Data, widthItemDesc);
                            int rowHeight2 = Math.Max((int)Math.Ceiling(descSize2.Height), tab1DataHeight);

                            e.Graphics.DrawString(desc, font_Data, Brushes.Black,new Rectangle(xStartItemDesc, tab1YStart + itemHeight, widthItemDesc, rowHeight2), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemQty, tab1YStart + itemHeight, widthItemQuantity, rowHeight2), sfAlignCenter);
                            e.Graphics.DrawString(item.UnitOfMeasure, font_Data, Brushes.Black,new Rectangle(xStartItemUOM, tab1YStart + itemHeight, widthItemUOM, rowHeight2), sfAlignCenter);
                            e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemUnitPrice, tab1YStart + itemHeight, widthItemUnitPrice, rowHeight2), sfAlignCenterRight);
                            e.Graphics.DrawString(combinedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemAmount, tab1YStart + itemHeight, widthItemAmount, rowHeight2), sfAlignCenterRight);

                            totalAmount += combinedAmount;
                            itemHeight += rowHeight2;
                            i++;
                            continue;
                        }

                        // ---------- Case 4: Regular line ----------
                        e.Graphics.DrawString(descText, font_Data, Brushes.Black,new Rectangle(xStartItemDesc, tab1YStart + itemHeight, widthItemDesc, rowHeight), sfAlignLeftCenter);
                        e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemQty, tab1YStart + itemHeight, widthItemQuantity, rowHeight), sfAlignCenter);
                        e.Graphics.DrawString(item.UnitOfMeasure, font_Data, Brushes.Black,new Rectangle(xStartItemUOM, tab1YStart + itemHeight, widthItemUOM, rowHeight), sfAlignCenter);
                        e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemUnitPrice, tab1YStart + itemHeight, widthItemUnitPrice, rowHeight), sfAlignCenterRight);
                        e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(xStartItemAmount, tab1YStart + itemHeight, widthItemAmount, rowHeight), sfAlignCenterRight);

                        totalAmount += adjustedAmount;
                        itemHeight += rowHeight;
                    }
                }
                else if (vatType == "Exclusive")
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
                            e.Graphics.DrawString(descText, font_Data, Brushes.Black,new Rectangle(rectItemDescription.X, rectItemDescription.Y + itemHeight, rectItemDescription.Width, rowHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemAmount.X, rectItemAmount.Y + itemHeight, rectItemAmount.Width, rowHeight), sfAlignCenterRight);

                            totalAmount += adjustedAmount;
                            itemHeight += rowHeight;
                            continue;
                        }

                        // ---------- Case 2: Description-only ----------
                        if (item.Rate == 0 && i > 0 && invoice.Lines[i - 1].Description == "Subtotal")
                        {
                            e.Graphics.DrawString(descText, font_Data, Brushes.Black,new Rectangle(rectItemDescription.X, rectItemDescription.Y + itemHeight, rectItemDescription.Width, rowHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemAmount.X, rectItemAmount.Y + itemHeight, rectItemAmount.Width, rowHeight), sfAlignCenterRight);

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

                            e.Graphics.DrawString(desc, font_Data, Brushes.Black,new Rectangle(rectItemDescription.X, rectItemDescription.Y + itemHeight, rectItemDescription.Width, rowHeight2), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + itemHeight, rectItemQuantity.Width, rowHeight2), sfAlignCenter);
                            e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + itemHeight, rectItemUnitPrice.Width, rowHeight2), sfAlignCenterRight);
                            e.Graphics.DrawString(combinedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemAmount.X, rectItemAmount.Y + itemHeight, rectItemAmount.Width, rowHeight2), sfAlignCenterRight);

                            totalAmount += combinedAmount;
                            itemHeight += rowHeight2;
                            i++;
                            continue;
                        }

                        // ---------- Case 4: Regular ----------
                        e.Graphics.DrawString(descText, font_Data, Brushes.Black,new Rectangle(rectItemDescription.X, rectItemDescription.Y + itemHeight, rectItemDescription.Width, rowHeight), sfAlignLeftCenter);
                        e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + itemHeight, rectItemQuantity.Width, rowHeight), sfAlignCenter);
                        e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + itemHeight, rectItemUnitPrice.Width, rowHeight), sfAlignCenterRight);
                        e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black,new Rectangle(rectItemAmount.X, rectItemAmount.Y + itemHeight, rectItemAmount.Width, rowHeight), sfAlignCenterRight);

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
                e.Graphics.DrawString("Note: " + invoicenote, font_Data, Brushes.Black, rectNote, sfAlignLeftCenter);
            }







            // LEFT TABLE
            int dataHeight = 18;
            int xStart = 230;
            int yStart = 765 + 17 * 3; // 770
            int width = 170 - 20;

            Rectangle rectVATableSales = new Rectangle(xStart, yStart, width, dataHeight);
            Rectangle rectVATExemptSales = new Rectangle(xStart, yStart + dataHeight, width, dataHeight);
            Rectangle rectZeroRatedSales = new Rectangle(xStart, yStart - 3 + dataHeight * 2, width, dataHeight);
            Rectangle rectVatAmount = new Rectangle(xStart, yStart - 3 + dataHeight * 3, width, dataHeight);

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
                    bool isLineVATable = line.Tax != "Non" || invoice.TaxesName == "Vat";
                    if (isLineVATable)
                    {
                        decimal adjustedAmount = line.Tax != "Non" ? line.Amount * 1.12m : line.Amount;
                        totalVATableAmount += adjustedAmount;
                    }
                }
            }

            // Step 2: compute net and VAT
            decimal amountNetVat = totalVATableAmount / 1.12m;
            decimal totalVAT = totalVATableAmount - amountNetVat;


            if (invoiceData[0].TaxesName == "Vat")
            {
                if (amountNetVat > 0)
                    e.Graphics.DrawString(amountNetVat.ToString("N2"), font_Data, Brushes.Black, rectVATableSales, sfAlignCenterRight);

                if (totalVAT > 0)
                    e.Graphics.DrawString(totalVAT.ToString("N2"), font_Data, Brushes.Black, rectVatAmount, sfAlignCenterRight);
            }
            else
            {
                if (zeroRatedSalesTotal > 0)
                    e.Graphics.DrawString(zeroRatedSalesTotal.ToString("N2"), font_Data, Brushes.Black, rectZeroRatedSales, sfAlignCenterRight);
                else if (vatExemptSalesTotal > 0)
                    e.Graphics.DrawString(vatExemptSalesTotal.ToString("N2"), font_Data, Brushes.Black, rectVATExemptSales, sfAlignCenterRight);
            }


            // RIGHT TABLE RECTANGLES
            int tab2RightDataHeight = 17;
            int tab2RightXStart = 580;
            int tab2RightYStart = 765 + tab2RightDataHeight * 3;
            int tab2RightWidth = 210 - 30;

            Rectangle rectR1TotalSales = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 0, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR2LessVAT = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 1, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR3AmountNetofVAT = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 2, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR4LessDiscount = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 3, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR5AddVAT = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 4, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR6LessWithholdingTax = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 5, tab2RightWidth, tab2RightDataHeight);
            Rectangle rectR7TotalAmountDue = new Rectangle(tab2RightXStart, tab2RightYStart + tab2RightDataHeight * 6, tab2RightWidth, tab2RightDataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectR1TotalSales);
            e.Graphics.DrawRectangle(Pens.Orange, rectR2LessVAT);
            e.Graphics.DrawRectangle(Pens.Yellow, rectR3AmountNetofVAT);
            e.Graphics.DrawRectangle(Pens.Green, rectR4LessDiscount);
            e.Graphics.DrawRectangle(Pens.Blue, rectR5AddVAT);
            e.Graphics.DrawRectangle(Pens.Indigo, rectR6LessWithholdingTax);
            e.Graphics.DrawRectangle(Pens.Violet, rectR7TotalAmountDue);*/

            // -------------------- CALCULATION --------------------

            decimal totalVATableAmount2 = 0;

            foreach (var invoice in invoiceData)
            {
                foreach (var line in invoice.Lines)
                {
                    bool isLineVATable = line.Tax != "Non" || invoice.TaxesName == "Vat";
                    if (isLineVATable)
                    {
                        decimal adjustedAmount = line.Tax != "Non" ? line.Amount * 1.12m : line.Amount;
                        totalVATableAmount2 += adjustedAmount;
                    }
                }
            }

            // Step 2: compute net and VAT
            decimal amountNetVat2 = totalVATableAmount2 / 1.12m;
            decimal totalVAT2 = totalVATableAmount2 - amountNetVat2;
            Console.WriteLine($"Total VATable Amount 2: {totalVATableAmount2}, Net: {amountNetVat2}, VAT: {totalVAT2}");

            // -------------------- R1 Total Sales --------------------
            decimal totalSales = invoiceData.Sum(inv => inv.Lines.Sum(l => l.Amount));
            totalSales += totalVAT2;

            Console.WriteLine($" Total Sales: {totalSales}, Total VATable Amount 2: {totalVATableAmount2}");

            if(vatType == "Inclusive")
            {
                if (totalSales > 0)
                    e.Graphics.DrawString(totalSales.ToString("N2"), font_Data, Brushes.Black, rectR1TotalSales, sfAlignCenterRight);
                if (totalVAT2 > 0)
                    e.Graphics.DrawString(totalVAT2.ToString("N2"), font_Data, Brushes.Black, rectR2LessVAT, sfAlignCenterRight);
            }
            

            // -------------------- R3 Amount Net of VAT --------------------
            if (amountNetVat2 > 0)
                e.Graphics.DrawString(amountNetVat2.ToString("N2"), font_Data, Brushes.Black, rectR3AmountNetofVAT, sfAlignCenterRight);

            // -------------------- R4 Less Discount (blank) --------------------
            // e.Graphics.DrawString("", font_Data, Brushes.Black, rectR4LessDiscount, sfAlignCenterRight);

            // -------------------- R5 Add VAT --------------------
            if (totalVAT2 > 0)
                e.Graphics.DrawString(totalVAT2.ToString("N2"), font_Data, Brushes.Black, rectR5AddVAT, sfAlignCenterRight);

            // -------------------- R6 Less Withholding Tax (EWT 1%) --------------------
            decimal ewtAmount = 0;
            if (isLessEWTChecked)
            {
                ewtAmount = amountNetVat2 * 0.01m;
                e.Graphics.DrawString(ewtAmount.ToString("N2"), font_Data, Brushes.Black, rectR6LessWithholdingTax, sfAlignCenterRight);
            }

            // -------------------- R7 Total Amount Due --------------------
            decimal totalAmountDue = totalVATableAmount2;
            if (isLessEWTChecked)
                totalAmountDue -= ewtAmount;

            if (totalAmountDue > 0)
                e.Graphics.DrawString(totalAmountDue.ToString("N2"), font_EightBold, Brushes.Black, rectR7TotalAmountDue, sfAlignCenterRight);


            // Signatory
            string Signatory = signatoryName;

            Rectangle rectAuthorized = new Rectangle(555, 945 + 40, 230, 18);

            //e.Graphics.DrawRectangle(Pens.Black, rectAuthorized);

            e.Graphics.DrawString(Signatory, font_Data, Brushes.Black, rectAuthorized, sfAlignCenter);

        }

        public void Layout_DeliveryReceipt(PrintPageEventArgs e, List<InvoiceData> invoiceData, string note, string businessStyle, string pwdSignature, bool isEnableExpDateChecked, string signatoryName)
        {
            /*Image image = Properties.Resources.NATURE_DR;
            e.Graphics.DrawImage(image, e.PageBounds);*/

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
            string invoiceBusinessStyle = "";
            string invoiceTin = "";
            string invoiceStoreCode = "";
            foreach (var inv in invoiceData)
            {
                 invoiceTin = inv.GetCustomField("TIN");
                 if (businessStyle == "")
                 {
                    invoiceBusinessStyle = inv.GetCustomField("BUSINESS STYLE");
                 }
                 else
                 {
                       invoiceBusinessStyle = businessStyle;
                 }
                 invoiceStoreCode = inv.GetCustomField("Store Code");
             }
            string invoiceBusinessAdd = invoiceData[0].ShipAddress1.ToString() + invoiceData[0].ShipAddress2.ToString() + invoiceData[0].ShipAddress3.ToString() + invoiceData[0].ShipAddress4.ToString() + invoiceData[0].ShipAddress5.ToString();


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
                foreach (var lineItem in invoice.Lines)
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
            string Signatory = signatoryName;

            Rectangle rectAuthorized = new Rectangle(555, 915, 230, 18);

            //e.Graphics.DrawRectangle(Pens.Black, rectAuthorized);

            e.Graphics.DrawString(Signatory, font_Data, Brushes.Black, rectAuthorized, sfAlignCenter);

        }
    }
}
