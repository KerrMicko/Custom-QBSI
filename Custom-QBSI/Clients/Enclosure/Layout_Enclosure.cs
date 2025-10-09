using Custom_QBSI.Clients.PBS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static Custom_QBSI.Clients.Enclosure.Dataclass_Enclosure;

namespace Custom_QBSI.Clients.Enclosure
{
    public class Layout_Enclosure
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
        public void Layout_SalesInvoice(PrintPageEventArgs e, List<InvoiceData> invoiceData, string vatType, string businessStyle, bool includeDateIssued, bool isLessEWTChecked, string acNo, DateTime dateIssued, string seriesNumber)
        {
            int maxWidth = 750;
            int xStart = 50 - 25;
            int yStart = 40;

            Image logo = Properties.Resources.logo_enclosure;
            e.Graphics.DrawImage(logo, new Rectangle(xStart - 5, 45, 150, 80));

            Font font_Details = font_Eight;
            Font font_Number = font_Nine;
            Font font_Data = font_Eight;

            Rectangle rectCompanyName = new Rectangle(xStart, yStart, maxWidth, 40);
            Rectangle rectCompanyAddress = new Rectangle(xStart, yStart + 40 - 10, maxWidth, 40);
            Rectangle rectCompanyTIN = new Rectangle(xStart, yStart + 65 - 10, maxWidth, 40);

            string companyName = "ENCLOSURE SYSTEMS SPECIALISTS, INC.";
            string companyAddress = "Warehouse 1-B Narra Bldg., 2276 Chino Roces Ave. Ext.,\n" +
                "Magallanes 1232, City of Makati, NCR, Fourth District, Philippines";
            string companyTIN = "VAT REG. TIN: " + "004-661-469-00000";

            //e.Graphics.DrawRectangle(Pens.Black, rectCompanyAddress);
            e.Graphics.DrawString(companyName, font_SixteenBold, Brushes.Black, rectCompanyName, sfAlignCenter);
            e.Graphics.DrawString(companyAddress, font_Ten, Brushes.Black, rectCompanyAddress, sfAlignCenter);
            e.Graphics.DrawString(companyTIN, font_Ten, Brushes.Black, rectCompanyTIN, sfAlignCenter);

            e.Graphics.DrawString("INVOICE", font_ThirteenBold, Brushes.Black, new PointF(xStart, yStart + 100));

            e.Graphics.DrawString("NO. " + seriesNumber, font_Thirteen, Brushes.Red, new PointF(xStart + 650, yStart + 100));

            int yStartDetails = 180;
            int rectHeight = 28;
            int rightWidth = 180;
            int leftWidth = maxWidth - rightWidth;

            // LEFT
            Rectangle rectCustomerName = new Rectangle(xStart, yStartDetails, leftWidth, rectHeight);
            Rectangle rectBusinessAddress = new Rectangle(xStart, yStartDetails + rectHeight, leftWidth, rectHeight);
            Rectangle rectBusinessAddress2 = new Rectangle(xStart, yStartDetails + rectHeight * 2 + 5, leftWidth, rectHeight);

            Rectangle rectCustomerNameData = new Rectangle(xStart + 120, yStartDetails - 5 , leftWidth - 140, rectHeight + 5);
            Rectangle rectBusinessAddressData = new Rectangle(xStart, yStartDetails + rectHeight + 5, leftWidth, rectHeight * 2);
            // RIGHT
            Rectangle rectDate = new Rectangle(xStart + leftWidth, yStartDetails, rightWidth, rectHeight);
            Rectangle rectTIN = new Rectangle(xStart + leftWidth, yStartDetails + rectHeight, rightWidth, rectHeight);
            Rectangle rectTerms = new Rectangle(xStart + leftWidth, yStartDetails + rectHeight * 2, rightWidth, rectHeight);

            Rectangle rectDateData = new Rectangle(xStart + leftWidth + 38, yStartDetails, rightWidth - 38, rectHeight);
            Rectangle rectTINData = new Rectangle(xStart + leftWidth + 30, yStartDetails + rectHeight, rightWidth - 30, rectHeight);
            Rectangle rectTermsData = new Rectangle(xStart + leftWidth + 52, yStartDetails + rectHeight * 2, rightWidth - 52, rectHeight);

            /*e.Graphics.DrawRectangle(Pens.Black, rectCustomerName);
            e.Graphics.DrawRectangle(Pens.Black, rectBusinessAddress);
            e.Graphics.DrawRectangle(Pens.Black, rectBusinessAddress2);
            e.Graphics.DrawRectangle(Pens.Black, rectDate);
            e.Graphics.DrawRectangle(Pens.Black, rectTIN);
            e.Graphics.DrawRectangle(Pens.Black, rectTerms);*/

            /*e.Graphics.DrawRectangle(Pens.Black, rectCustomerNameData);
            e.Graphics.DrawRectangle(Pens.Black, rectBusinessAddressData);
            e.Graphics.DrawRectangle(Pens.Black, rectDateData);
            e.Graphics.DrawRectangle(Pens.Black, rectTINData);
            e.Graphics.DrawRectangle(Pens.Black, rectTermsData);*/

            string refNumber = invoiceData[0].RefNumber.ToString();
            //string customerName = invoiceData[0].CustomerName.ToString();

            string indentedAddress ="                                 "+ $"{invoiceData[0].BillAddress1} {invoiceData[0].BillAddress2} {invoiceData[0].BillAddress3} " +
                         $"{invoiceData[0].BillAddress4} {invoiceData[0].BillAddress5}";




            string date = invoiceData[0].TxnDate.ToString("MM/dd/yyyy");
            string tin = invoiceData[0].TINNO.ToString();
            string terms = invoiceData[0].Terms.ToString();

            string customerName = "";
            if (string.IsNullOrEmpty(businessStyle))
            {
                customerName = invoiceData[0].CustomerName.ToString();
            }
            else
            {
                customerName = businessStyle;
            }

            e.Graphics.DrawString("Customer's Name:", font_Ten, Brushes.Black, rectCustomerName, sfAlignLeftCenter);
            e.Graphics.DrawString(customerName, font_Eight, Brushes.Black, rectCustomerNameData, sfAlignLeftCenter);

            e.Graphics.DrawString("Business Address:", font_Ten, Brushes.Black, rectBusinessAddress, sfAlignLeftCenter);
            e.Graphics.DrawString(indentedAddress, font_Ten, Brushes.Black, rectBusinessAddressData);
            e.Graphics.DrawString("", font_Ten, Brushes.Black, rectBusinessAddress2, sfAlignLeftCenter);

            e.Graphics.DrawString("Date:", font_Ten, Brushes.Black, rectDate, sfAlignLeftCenter);
            e.Graphics.DrawString(date, font_Ten, Brushes.Black, rectDateData, sfAlignLeftCenter);

            e.Graphics.DrawString("TIN:", font_Ten, Brushes.Black, rectTIN, sfAlignLeftCenter);
            e.Graphics.DrawString(tin, font_Ten, Brushes.Black, rectTINData, sfAlignLeftCenter);

            e.Graphics.DrawString("Terms :", font_Ten, Brushes.Black, rectTerms, sfAlignLeftCenter);
            e.Graphics.DrawString(terms, font_Ten, Brushes.Black, rectTermsData, sfAlignLeftCenter);

            // EXTRA FIELDS
            int yStartExtraFields = yStartDetails + rectHeight * 3;
            int widthExtraFields = 250;
            int xAdd = 51;

            Rectangle rectSO = new Rectangle(xStart, yStartExtraFields, widthExtraFields, rectHeight);
            Rectangle rectDR = new Rectangle(xStart + 100 + widthExtraFields, yStartExtraFields, widthExtraFields, rectHeight);
            Rectangle rectPO = new Rectangle(xStart + widthExtraFields * 2 + 70, yStartExtraFields, widthExtraFields, rectHeight);

            Rectangle rectSOData = new Rectangle(xStart + xAdd, yStartExtraFields, widthExtraFields - xAdd, rectHeight);
            Rectangle rectDRData = new Rectangle(xStart + widthExtraFields + xAdd - 5 + 100, yStartExtraFields, widthExtraFields - xAdd, rectHeight);
            Rectangle rectPOData = new Rectangle(xStart + widthExtraFields * 2 + xAdd + 70, yStartExtraFields, widthExtraFields - xAdd, rectHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectSO);
            e.Graphics.DrawRectangle(Pens.Red, rectDR);
            e.Graphics.DrawRectangle(Pens.Red, rectPO);*/

            e.Graphics.DrawString("S.O # :", font_Ten, Brushes.Black, rectPO, sfAlignLeftCenter);//RECTSO
            e.Graphics.DrawString("DR # :", font_Ten, Brushes.Black, rectDR, sfAlignLeftCenter);
            e.Graphics.DrawString("PO # :", font_Ten, Brushes.Black, rectSO, sfAlignLeftCenter);//RECTPO

            string soNumber = invoiceData[0].SONumber;
            string drNumber = invoiceData[0].DrNo;
            string poNumber = invoiceData[0].PONumber;

            e.Graphics.DrawString(soNumber, font_Ten, Brushes.Black, rectPOData, sfAlignLeftCenter);//RECTSODATA
            e.Graphics.DrawString(drNumber, font_Ten, Brushes.Black, rectDRData, sfAlignLeftCenter);
            e.Graphics.DrawString(poNumber, font_Ten, Brushes.Black, rectSOData, sfAlignLeftCenter);//RECTPODATA

            // TABLE
            int yStartTable = yStartExtraFields + rectHeight + 5; // 268
            int tableDataHeight = 26;

            int widthItemDescription = 350;
            int widthItemQuantity = 130;
            int widthItemUnitPrice = 130;
            int widthItemAmount = 140;

            int xStartItemDescription = xStart;
            int xStartItemQuantity = xStart + widthItemDescription;
            int xStartItemUnitPrice = xStart + widthItemDescription + widthItemQuantity;
            int xStartItemUnitAmount = xStart + widthItemDescription + widthItemQuantity + widthItemUnitPrice;

            Rectangle rectItemDescription = new Rectangle(xStartItemDescription, yStartTable, widthItemDescription, tableDataHeight);
            Rectangle rectItemQuantity = new Rectangle(xStartItemQuantity, yStartTable, widthItemQuantity, tableDataHeight);
            Rectangle rectItemUnitPrice = new Rectangle(xStartItemUnitPrice, yStartTable, widthItemUnitPrice, tableDataHeight);
            Rectangle rectItemAmount = new Rectangle(xStartItemUnitAmount, yStartTable, widthItemAmount, tableDataHeight);

            // ITEMS PART
            e.Graphics.DrawString("Item Description/ Nature of Service", font_Nine, Brushes.Black, rectItemDescription, sfAlignCenter);
            e.Graphics.DrawString("Quantity", font_Nine, Brushes.Black, rectItemQuantity, sfAlignCenter);
            e.Graphics.DrawString("Unit Cost/ Price", font_Nine, Brushes.Black, rectItemUnitPrice, sfAlignCenter);
            e.Graphics.DrawString("Amount", font_Nine, Brushes.Black, rectItemAmount, sfAlignCenter);

            int yPosLayout = 0;
            
            for (int i = 0; i < 12; i++)
            {
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemDescription, yStartTable + yPosLayout, widthItemDescription, tableDataHeight));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemQuantity, yStartTable + yPosLayout, widthItemQuantity, tableDataHeight));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemUnitPrice, yStartTable + yPosLayout, widthItemUnitPrice, tableDataHeight));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemUnitAmount, yStartTable + yPosLayout, widthItemAmount, tableDataHeight));
                yPosLayout += tableDataHeight;
            }
            decimal totalAmount = 0;
            decimal vatableSalesTotal = 0;
            decimal zeroRatedSalesTotal = 0;
            decimal vatExemptSalesTotal = 0;

            int yPos = 26;
            foreach (var invoice in invoiceData)
            {
                if (vatType == "Inclusive")
                {
                    for (int i = 0; i < invoice.LineItems.Count; i++)
                    {
                        var item = invoice.LineItems[i];

                        if (item.Description == "Subtotal") continue;

                        bool isTaxable = item.SalesTaxTotal != 0 && item.Tax != "Non" && item.TaxesName != "Out of State";
                        bool hasNext = i + 1 < invoice.LineItems.Count;
                        var nextItem = hasNext ? invoice.LineItems[i + 1] : null;

                        decimal rateAdjustment = isTaxable ? item.Rate * 0.12m : 0m;
                        decimal unitRateToDraw = item.TaxesName != "Out of State" ? item.Rate + rateAdjustment : item.Rate;

                        // ✅ Classification
                        if (item.TaxesName == "Zero Rated Sales")
                            zeroRatedSalesTotal += item.Amount;
                        else if (item.TaxesName == "Vat Exempt")
                            vatExemptSalesTotal += item.Amount;
                        else if (isTaxable)
                            vatableSalesTotal += item.Amount;

                        // ✅ Case 1: Discount line (separate always)
                        if (item.Description.ToLower().Contains("discount") || item.Rate.ToString().Contains("%") || item.Amount < 0)
                        {
                            decimal discountAmount = isTaxable ? item.Amount * 1.12m : item.Amount;

                            e.Graphics.DrawString(item.Description, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + yPos, rectItemQuantity.Width, tableDataHeight), sfAlignCenter);
                            e.Graphics.DrawString(item.Rate.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + yPos, rectItemUnitPrice.Width, tableDataHeight), sfAlignCenterRight);
                            e.Graphics.DrawString(discountAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                            totalAmount += discountAmount;
                            yPos += tableDataHeight;
                            continue;
                        }

                        // ✅ Case 2: Description-only line after a Subtotal
                        if (item.Rate == 0 && i > 0 && invoice.LineItems[i - 1].Description == "Subtotal")
                        {
                            e.Graphics.DrawString(item.Description, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Amount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                            totalAmount += item.Amount;
                            yPos += tableDataHeight;
                            continue;
                        }

                        // ✅ Case 3: Discount follows item with Rate == 0
                        if (item.Rate == 0 && hasNext && nextItem.Rate == 0 && nextItem.Description != "Subtotal")
                        {
                            decimal combinedAmount = item.Amount + nextItem.Amount;
                            if (isTaxable) combinedAmount *= 1.12m;

                            string desc = $"{item.Description} ({nextItem.Description})";

                            e.Graphics.DrawString(desc, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + yPos, rectItemQuantity.Width, tableDataHeight), sfAlignCenter);
                            e.Graphics.DrawString(item.Rate.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + yPos, rectItemUnitPrice.Width, tableDataHeight), sfAlignCenterRight);
                            e.Graphics.DrawString(combinedAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                            totalAmount += combinedAmount;
                            yPos += tableDataHeight;
                            i++; // Skip discount line
                            continue;
                        }

                        // ✅ Case 4: Regular line item
                        decimal adjustedAmount = isTaxable ? item.Amount * 1.12m : item.Amount;

                        e.Graphics.DrawString(item.Description, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                        e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + yPos, rectItemQuantity.Width, tableDataHeight), sfAlignCenter);
                        e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + yPos, rectItemUnitPrice.Width, tableDataHeight), sfAlignCenterRight);
                        e.Graphics.DrawString(adjustedAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                        totalAmount += adjustedAmount;
                        yPos += tableDataHeight;
                    }
                }
                else if (vatType == "Exclusive")
                {
                    for (int i = 0; i < invoice.LineItems.Count; i++)
                    {
                        var item = invoice.LineItems[i];

                        if (item.Description == "Subtotal") continue;

                        bool hasNext = i + 1 < invoice.LineItems.Count;
                        var nextItem = hasNext ? invoice.LineItems[i + 1] : null;
                        decimal unitRateToDraw = item.Rate; // no adjustment in Exclusive

                        // ✅ Classification
                        if (item.TaxesName == "Zero Rated Sales")
                            zeroRatedSalesTotal += item.Amount;
                        else if (item.TaxesName == "Vat Exempt")
                            vatExemptSalesTotal += item.Amount;
                        else if (item.Tax != "Non")
                            vatableSalesTotal += item.Amount;

                        // ✅ Case 1: Discount line (separate always)
                        if (item.Description.ToLower().Contains("discount") || item.Rate.ToString().Contains("%") || item.Amount < 0)
                        {
                            e.Graphics.DrawString(item.Description, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + yPos, rectItemQuantity.Width, tableDataHeight), sfAlignCenter);
                            e.Graphics.DrawString(item.Rate.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + yPos, rectItemUnitPrice.Width, tableDataHeight), sfAlignCenterRight);
                            e.Graphics.DrawString(item.Amount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                            totalAmount += item.Amount;
                            yPos += tableDataHeight;
                            continue;
                        }

                        // ✅ Case 2: Description-only line after a Subtotal
                        if (item.Rate == 0 && i > 0 && invoice.LineItems[i - 1].Description == "Subtotal")
                        {
                            e.Graphics.DrawString(item.Description, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Amount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                            totalAmount += item.Amount;
                            yPos += tableDataHeight;
                            continue;
                        }

                        // ✅ Case 3: Discount follows item with Rate == 0
                        if (item.Rate == 0 && hasNext && nextItem.Rate == 0 && nextItem.Description != "Subtotal")
                        {
                            decimal combinedAmount = item.Amount + nextItem.Amount;

                            string desc = $"{item.Description} ({nextItem.Description})";

                            e.Graphics.DrawString(desc, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                            e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + yPos, rectItemQuantity.Width, tableDataHeight), sfAlignCenter);
                            e.Graphics.DrawString(item.Rate.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + yPos, rectItemUnitPrice.Width, tableDataHeight), sfAlignCenterRight);
                            e.Graphics.DrawString(combinedAmount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                            totalAmount += combinedAmount;
                            yPos += tableDataHeight;
                            i++;
                            continue;
                        }

                        // ✅ Case 4: Regular line item
                        e.Graphics.DrawString(item.Description, font_Data, Brushes.Black, new Rectangle(rectItemDescription.X, rectItemDescription.Y + yPos, rectItemDescription.Width, tableDataHeight), sfAlignLeftCenter);
                        e.Graphics.DrawString(item.Quantity.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemQuantity.X, rectItemQuantity.Y + yPos, rectItemQuantity.Width, tableDataHeight), sfAlignCenter);
                        e.Graphics.DrawString(unitRateToDraw.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemUnitPrice.X, rectItemUnitPrice.Y + yPos, rectItemUnitPrice.Width, tableDataHeight), sfAlignCenterRight);
                        e.Graphics.DrawString(item.Amount.ToString("N2"), font_Data, Brushes.Black, new Rectangle(rectItemAmount.X, rectItemAmount.Y + yPos, rectItemAmount.Width, tableDataHeight), sfAlignCenterRight);

                        totalAmount += item.Amount;
                        yPos += tableDataHeight;
                    }
                }
            }


            // VAT PART
            int widthVATHeader = 100;
            for (int i = 0; i < 5; i++)
            {
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemDescription, yStartTable + yPosLayout, widthItemDescription, tableDataHeight));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemQuantity, yStartTable + yPosLayout, widthItemQuantity, tableDataHeight));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemUnitPrice, yStartTable + yPosLayout, widthItemUnitPrice, tableDataHeight));
                e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemUnitAmount, yStartTable + yPosLayout, widthItemAmount, tableDataHeight));

                if (i > 0)
                {
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartItemDescription, yStartTable + yPosLayout, widthVATHeader, tableDataHeight));
                }

                yPosLayout += tableDataHeight;
            }


            // LEFT
            int xStartLeftVAT = xStart + widthVATHeader;
            int yStartLeftVAT = yStartTable + tableDataHeight * 13;
            int widthLeftVATData = widthItemDescription - widthVATHeader;

            Rectangle rectVATableSales = new Rectangle(xStartLeftVAT, yStartLeftVAT, widthLeftVATData, tableDataHeight);
            Rectangle rectVAT = new Rectangle(xStartLeftVAT, yStartLeftVAT + tableDataHeight, widthLeftVATData, tableDataHeight);
            Rectangle rectZeroRatedSales = new Rectangle(xStartLeftVAT, yStartLeftVAT + tableDataHeight * 2, widthLeftVATData, tableDataHeight);
            Rectangle rectVATExemptSales = new Rectangle(xStartLeftVAT, yStartLeftVAT + tableDataHeight * 3, widthLeftVATData, tableDataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectVATableSales);
            e.Graphics.DrawRectangle(Pens.Orange, rectVAT);
            e.Graphics.DrawRectangle(Pens.Yellow, rectZeroRatedSales);
            e.Graphics.DrawRectangle(Pens.Green, rectVATExemptSales);*/

            e.Graphics.DrawString("VATable Sales", font_Seven, Brushes.Black, new Rectangle(xStartItemDescription, yStartTable + tableDataHeight * 13, widthVATHeader, tableDataHeight), sfAlignCenterRight);
            e.Graphics.DrawString("VAT", font_Seven, Brushes.Black, new Rectangle(xStartItemDescription, yStartTable + tableDataHeight * 14, widthVATHeader, tableDataHeight), sfAlignCenterRight);
            e.Graphics.DrawString("Zero Rated Sales", font_Seven, Brushes.Black, new Rectangle(xStartItemDescription, yStartTable + tableDataHeight * 15, widthVATHeader, tableDataHeight), sfAlignCenterRight);
            e.Graphics.DrawString("VAT-Exempt Sales", font_Seven, Brushes.Black, new Rectangle(xStartItemDescription, yStartTable + tableDataHeight * 16, widthVATHeader, tableDataHeight), sfAlignCenterRight);

            decimal amountNetVat = 0;

            foreach (var invoice in invoiceData)
            {
                foreach (var lineItem in invoice.LineItems)
                {
                    amountNetVat = lineItem.TotalAmount - lineItem.SalesTaxTotal;
                }
            }

            bool isVAT = invoiceData[0].LineItems[0].Tax != "Non" || invoiceData[0].LineItems[0].TaxesName == "Vatable";
            bool hasVAT = invoiceData[0].LineItems[0].SalesTaxTotal > 0;

            if (isVAT && hasVAT)
            {
                if (amountNetVat > 0)
                {
                    e.Graphics.DrawString(amountNetVat.ToString("N2"), font_Data, Brushes.Black, rectVATableSales, sfAlignCenterRight);
                }

                if (hasVAT)
                {
                    e.Graphics.DrawString(invoiceData[0].LineItems[0].SalesTaxTotal.ToString("N2"), font_Data, Brushes.Black, rectVAT, sfAlignCenterRight);
                }
            }
            else
            {
                if (zeroRatedSalesTotal > 0)
                {
                    e.Graphics.DrawString(zeroRatedSalesTotal.ToString("N2"), font_Data, Brushes.Black, rectZeroRatedSales, sfAlignCenterRight);
                }
                else if (vatExemptSalesTotal > 0)
                {
                    e.Graphics.DrawString(vatExemptSalesTotal.ToString("N2"), font_Data, Brushes.Black, rectVATExemptSales, sfAlignCenterRight);
                }
            }

            // RIGHT
            int xStartRightVAT = xStart + widthItemDescription + widthItemQuantity;
            int yStartRightVAT = yStartLeftVAT - tableDataHeight;
            int widthRightVATData = widthItemUnitPrice + widthItemAmount;

            Rectangle rectTotalSales = new Rectangle(xStartRightVAT, yStartRightVAT, widthRightVATData, tableDataHeight);
            Rectangle rectLessVAT = new Rectangle(xStartRightVAT, yStartRightVAT + tableDataHeight, widthRightVATData, tableDataHeight);
            Rectangle rectAmountNetofVAT = new Rectangle(xStartRightVAT, yStartRightVAT + tableDataHeight * 2, widthRightVATData, tableDataHeight);
            Rectangle rectAddVAT = new Rectangle(xStartRightVAT, yStartRightVAT + tableDataHeight * 3, widthRightVATData, tableDataHeight);
            Rectangle rectLessWithholdingTax = new Rectangle(xStartRightVAT, yStartRightVAT + tableDataHeight * 4, widthRightVATData, tableDataHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectTotalSales);
            e.Graphics.DrawRectangle(Pens.Orange, rectLessVAT);
            e.Graphics.DrawRectangle(Pens.Yellow, rectAmountNetofVAT);
            e.Graphics.DrawRectangle(Pens.Green, rectAddVAT);
            e.Graphics.DrawRectangle(Pens.Blue, rectLessWithholdingTax);*/

            e.Graphics.DrawString("Total Sales (VAT Inclusive)", font_Seven, Brushes.Black, new Rectangle(xStartItemQuantity, yStartTable + tableDataHeight * 12, widthItemQuantity, tableDataHeight), sfAlignCenterRight);
            e.Graphics.DrawString("Less: VAT", font_Seven, Brushes.Black, new Rectangle(xStartItemQuantity, yStartTable + tableDataHeight * 13, widthItemQuantity, tableDataHeight), sfAlignCenterRight);
            e.Graphics.DrawString("Amount: Net of VAT", font_Seven, Brushes.Black, new Rectangle(xStartItemQuantity, yStartTable + tableDataHeight * 14, widthItemQuantity, tableDataHeight), sfAlignCenterRight);
            e.Graphics.DrawString("Add: VAT", font_Seven, Brushes.Black, new Rectangle(xStartItemQuantity, yStartTable + tableDataHeight * 15, widthItemQuantity, tableDataHeight), sfAlignCenterRight);
            e.Graphics.DrawString("Less: Witholding Tax", font_Seven, Brushes.Black, new Rectangle(xStartItemQuantity, yStartTable + tableDataHeight * 16, widthItemQuantity, tableDataHeight), sfAlignCenterRight);

            decimal amountNetVat2 = 0;
            foreach (var invoice in invoiceData)
            {
                foreach (var lineItem in invoice.LineItems)
                {
                    amountNetVat2 = lineItem.TotalAmount - lineItem.SalesTaxTotal;
                }
            }

            var firstLineItem = invoiceData[0].LineItems[0];

            if (vatType == "Inclusive")
            {
                if (firstLineItem.TotalAmount > 0)
                    e.Graphics.DrawString(firstLineItem.TotalAmount.ToString("N2"), font_Data, Brushes.Black, rectTotalSales, sfAlignCenterRight);

                if (firstLineItem.SalesTaxTotal > 0)
                    e.Graphics.DrawString(firstLineItem.SalesTaxTotal.ToString("N2"), font_Data, Brushes.Black, rectLessVAT, sfAlignCenterRight);
            }

            // Common for both Inclusive and Exclusive
            if (amountNetVat2 > 0)
                e.Graphics.DrawString(amountNetVat2.ToString("N2"), font_Data, Brushes.Black, rectAmountNetofVAT, sfAlignCenterRight);

            // Discount — static/blank
            //e.Graphics.DrawString("", font_Data, Brushes.Black, rectR4LessDiscount, sfAlignCenterRight);


            if (firstLineItem.SalesTaxTotal > 0)
                e.Graphics.DrawString(firstLineItem.SalesTaxTotal.ToString("N2"), font_Data, Brushes.Black, rectAddVAT, sfAlignCenterRight);

            // NEED EWT 1% button
            decimal ewtAmount = 0;
            if (isLessEWTChecked)
            {
                ewtAmount = amountNetVat2 * 0.01m;
            }

            if (isLessEWTChecked)
            {
                e.Graphics.DrawString(ewtAmount.ToString("N2"), font_Data, Brushes.Black, rectLessWithholdingTax, sfAlignCenterRight);
            }

            // Calculate final Total Amount Due
            decimal totalAmountDue = firstLineItem.TotalAmount;
            if (isLessEWTChecked)
            {
                totalAmountDue -= ewtAmount;
            }

            // TOTAL AMOUNT DUE
            Rectangle rectTotalAmountDue = new Rectangle(xStart, yStartTable + tableDataHeight * 17, maxWidth, tableDataHeight);
            e.Graphics.DrawRectangle(Pens.Black, rectTotalAmountDue);
            e.Graphics.DrawString("TOTAL AMOUNT DUE", font_EightBold, Brushes.Black, new Rectangle(xStartItemQuantity, yStartTable + tableDataHeight * 17, widthItemQuantity, tableDataHeight), sfAlignCenter);

            // Draw Total Amount Due
            if (totalAmountDue > 0)
            {
                e.Graphics.DrawString("P" + totalAmountDue.ToString("N2"), font_EightBold, Brushes.Black, new Rectangle(xStartItemUnitPrice, yStartTable + tableDataHeight * 17, widthItemUnitPrice + widthItemAmount, tableDataHeight), sfAlignCenterRight);
            }

            // AMOUNT IN WORDS
            int xStartMinus = 5;
            int yStartAmountInWords = yStartTable + tableDataHeight * 18 + 3;

            string amountInWords = OtherFunctions.AmountToWordsConverter.Convert((double)totalAmountDue);

            Rectangle rectAmountInWords = new Rectangle(xStart, yStartTable + tableDataHeight * 18, maxWidth, tableDataHeight * 2);
            e.Graphics.DrawRectangle(Pens.Black, rectAmountInWords);
            e.Graphics.DrawString("AMOUNT IN WORDS: " + amountInWords, font_Nine, Brushes.Black, new Rectangle(xStart + xStartMinus, yStartAmountInWords, maxWidth - xStartMinus, tableDataHeight), sfAlignLeftCenter);

            // SIGNATORY
            int yStartSignatory = yStartTable + tableDataHeight * 20;
            int yStartSignatoryHeader = yStartSignatory + 5;
            int yStartSignatoryUnderline = yStartSignatoryHeader + tableDataHeight + 15;
            int yStartSignatoryText = yStartSignatoryUnderline + tableDataHeight;

            int widthPreparedByHeader = 300;
            int widthCheckedByHeader = 250;
            int widthApprovedByHeader = 200;

            int xStartPreparedByHeader = xStart + xStartMinus;
            int xStartCheckedByHeader = xStart + widthPreparedByHeader;
            int xStartApprovedByHeader = xStart + widthPreparedByHeader + widthCheckedByHeader;

            Rectangle rectSignatory = new Rectangle(xStart, yStartSignatory, maxWidth, tableDataHeight * 4 + 10);
            e.Graphics.DrawRectangle(Pens.Black, rectSignatory);

            Rectangle rectPreparedByHeader = new Rectangle(xStartPreparedByHeader + 75, yStartSignatoryHeader, widthPreparedByHeader - xStartMinus, tableDataHeight);
            Rectangle rectPreparedByUnderline = new Rectangle(xStartPreparedByHeader, yStartSignatoryUnderline, widthPreparedByHeader - xStartMinus - 25 - 22, tableDataHeight);

            Rectangle rectCheckedByHeader = new Rectangle(xStartCheckedByHeader + 50, yStartSignatoryHeader, widthCheckedByHeader, tableDataHeight);
            Rectangle rectCheckedByUnderline = new Rectangle(xStartCheckedByHeader, yStartSignatoryUnderline, widthCheckedByHeader - 10 - 28, tableDataHeight);

            Rectangle rectApprovedByHeader = new Rectangle(xStartApprovedByHeader + 50, yStartSignatoryHeader, widthApprovedByHeader, tableDataHeight);
            Rectangle rectApproveedByUnderline = new Rectangle(xStartApprovedByHeader, yStartSignatoryUnderline, widthApprovedByHeader - 2, tableDataHeight);

            /*e.Graphics.DrawRectangle(Pens.Black, rectPreparedByHeader);
            e.Graphics.DrawRectangle(Pens.Red, rectPreparedByUnderline);

            e.Graphics.DrawRectangle(Pens.Black, rectCheckedByHeader);
            e.Graphics.DrawRectangle(Pens.Red, rectCheckedByUnderline);

            e.Graphics.DrawRectangle(Pens.Black, rectApprovedByHeader);
            e.Graphics.DrawRectangle(Pens.Red, rectApproveedByUnderline);*/

            e.Graphics.DrawString("PREPARED BY:", font_Nine, Brushes.Black, rectPreparedByHeader, sfAlignLeftCenter);
            e.Graphics.DrawString("APPROVED BY:", font_Nine, Brushes.Black, rectCheckedByHeader, sfAlignLeftCenter);//checked by
            e.Graphics.DrawString("RECEIVED BY:", font_Nine, Brushes.Black, rectApprovedByHeader, sfAlignLeftCenter);//approved by

            Queries_Enclosure queries_Enclosure = new Queries_Enclosure();
            var signatories = queries_Enclosure.RetrieveSignatory();

            e.Graphics.DrawString(signatories.preparedBy, font_Nine, Brushes.Black, rectPreparedByUnderline, sfAlignCenter);
            e.Graphics.DrawString(signatories.checkedBy, font_Nine, Brushes.Black, rectCheckedByUnderline, sfAlignCenter);
            e.Graphics.DrawString(signatories.approvedBy, font_Nine, Brushes.Black, rectApproveedByUnderline, sfAlignCenter);

            e.Graphics.DrawString("__________________________________", font_Nine, Brushes.Black, rectPreparedByUnderline, sfAlignLeftBottom);
            e.Graphics.DrawString("_____________________________", font_Nine, Brushes.Black, rectCheckedByUnderline, sfAlignLeftBottom);
            e.Graphics.DrawString("___________________________", font_Nine, Brushes.Black, rectApproveedByUnderline, sfAlignLeftBottom);

            //e.Graphics.DrawRectangle(Pens.Black, new Rectangle(xStartPreparedByHeader, yStartSignatoryText, widthPreparedByHeader, tableDataHeight));
            e.Graphics.DrawString("Signature over Printed Name", font_Nine, Brushes.Black, new Rectangle(xStartPreparedByHeader, yStartSignatoryText, rectPreparedByUnderline.Width, tableDataHeight), sfAlignCenter);
            e.Graphics.DrawString("Signature over Printed Name", font_Nine, Brushes.Black, new Rectangle(xStartCheckedByHeader, yStartSignatoryText, rectCheckedByUnderline.Width, tableDataHeight), sfAlignCenter);
            e.Graphics.DrawString("Signature over Printed Name", font_Nine, Brushes.Black, new Rectangle(xStartApprovedByHeader, yStartSignatoryText, rectApproveedByUnderline.Width, tableDataHeight), sfAlignCenter);

            // EXTRA FIELDS FOOTER
            int yStartFooterExtraFields = yStartSignatory + rectHeight * 4 + 8;

            Rectangle rectACNo = new Rectangle(xStart, yStartFooterExtraFields, 250, rectHeight);
            Rectangle rectDateIssued = new Rectangle(xStart, yStartFooterExtraFields - 10 + rectHeight, 300, rectHeight);
            Rectangle rectSeriesRange = new Rectangle(xStart, yStartFooterExtraFields - 20 + rectHeight * 2, 350, rectHeight);

            Rectangle rectACNoData = new Rectangle(xStart + 58 - 20, yStartFooterExtraFields, 250 - 58, rectHeight);
            Rectangle rectDateIssuedData = new Rectangle(xStart + 92 - 18, yStartFooterExtraFields - 10 + rectHeight, 300 - 92, rectHeight);
            Rectangle rectSeriesRangeData = new Rectangle(xStart + 94 - 15, yStartFooterExtraFields - 20 + rectHeight * 2, 350 - 94, rectHeight);

            /*e.Graphics.DrawRectangle(Pens.Red, rectACNo);
            e.Graphics.DrawRectangle(Pens.Red, rectDateIssued);
            e.Graphics.DrawRectangle(Pens.Red, rectSeriesRange);*/

            //string acNo = "12345678901234567890";
            //DateTime dateIssued = DateTime.Now;
            //string seriesRange = "00001-0000000100";

            Font fontExtraFieldsFooter = font_Seven;

            queries_Enclosure = new Queries_Enclosure();
            var detailedEnclosure = queries_Enclosure.RetrieveACNoAndDateIssued();

            e.Graphics.DrawString("AC NO : _______________________", fontExtraFieldsFooter, Brushes.Black, rectACNo, sfAlignLeftCenter);
            e.Graphics.DrawString("Date Issued : ___________________", fontExtraFieldsFooter, Brushes.Black, rectDateIssued, sfAlignLeftCenter);
            e.Graphics.DrawString("Series Range :", fontExtraFieldsFooter, Brushes.Black, rectSeriesRange, sfAlignLeftCenter);

            e.Graphics.DrawString(detailedEnclosure.acNo, fontExtraFieldsFooter, Brushes.Black, rectACNoData, sfAlignLeftCenter);
            if (detailedEnclosure.dateIssued.HasValue)
            {
                string formattedDate = detailedEnclosure.dateIssued.Value.ToString("MM/dd/yyyy");
                e.Graphics.DrawString(formattedDate, fontExtraFieldsFooter, Brushes.Black, rectDateIssuedData, sfAlignLeftCenter);
            }
              
            e.Graphics.DrawString("000001-9999999999", fontExtraFieldsFooter, Brushes.Black, rectSeriesRangeData, sfAlignLeftCenter);
        }
    }
}
