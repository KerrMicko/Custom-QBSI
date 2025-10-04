using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Custom_QBSI.Clients.FP.DataClass_FP;

namespace Custom_QBSI.Clients.FP
{
    public class Dashboard_FP
    {
        private static readonly List<string> tableNames = new List<string> { "Account",
            "Company", "Customer",
            "Invoice", "InvoiceLine", "InvoiceLinkedTxn",
            "Item" };

        readonly string tableName = "FP";

        private PrintDocument printDocument;
        private PrintPreviewControl printPreviewControl;

        private ComboBox comboBox_Forms;

        // Series Number
        int seriesNumber = 1;
        private TextBox textBox_SeriesNumber;

        // Details
        private CheckBox checkBox_EnableExpDate;
        private CheckBox checkBox_LessEWT;
        private TextBox textBox_Note;
        private TextBox textBox_BusinessStyle;
        private TextBox textBox_ACNo;
        private DateTimePicker dateTimePicker_DateIssued;
        private CheckBox checkBox_IncludeDateIssued;
        private TextBox textBox_PWDSignature;
        private RadioButton radioButton_VATInclusive;
        private RadioButton radioButton_VATExclusive;

        // Signatory
        private TextBox textBox_SignatoryPreparedBy;
        private TextBox textBox_SignatoryCheckedBy;
        private TextBox textBox_SignatoryApprovedBy;

        private FlowLayoutPanel panel_Printing;
        private FlowLayoutPanel panel_Details;

        static int sideBarWidth = 250;

        Font font_Label = new Font("Microsoft Sans Serif", 9);

        public Panel MainPanel()
        {
            Panel panel_Container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(sideBarWidth, 50, 0, 0),
            };

            Panel panel_Main = new Panel
            {
                Parent = panel_Container,
                BackColor = Color.LightGray,
                Dock = DockStyle.Fill,
                //Height = 300,
            };
            printPreviewControl = new PrintPreviewControl();
            printPreviewControl.Dock = DockStyle.Fill;
            printPreviewControl.Zoom = 1;
            printPreviewControl.Visible = false;

            FlowLayoutPanel panel_Details = Main_PanelDetails();
            panel_Details.Parent = panel_Container;

            panel_Main.Controls.Add(printPreviewControl);

            return panel_Container;
        }
        private FlowLayoutPanel Main_PanelDetails()
        {
            Queries_FP queries_FP = new Queries_FP();

            int panelDetailsWidth = sideBarWidth + 30;
            int componentWidth = panelDetailsWidth - 20;

            panel_Details = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = panelDetailsWidth,
                Padding = new Padding(5),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = true,
            };

            Label label_NoteText = new Label
            {
                Parent = panel_Details,
                Width = componentWidth,
                Text = "Note:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_Note = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth,
                Font = font_Label,
                Multiline = true,
                Height = 50,
            };

            Label label_BusinessStyle = new Label
            {
                Parent = panel_Details,
                Width = componentWidth,
                Text = "Business Style/Name:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_BusinessStyle = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth,
                Font = font_Label,
            };

            Label label_ACNo = new Label
            {
                Parent = panel_Details,
                Width = componentWidth / 2,
                Text = "AC NO:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            Label label_DateIssue = new Label
            {
                Parent = panel_Details,
                Width = componentWidth / 2 - 5,
                Text = "Date Issued:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_ACNo = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth / 2,
                Font = font_Label,
            };

            dateTimePicker_DateIssued = new DateTimePicker
            {
                Parent = panel_Details,
                Width = componentWidth / 2 - 5,
                Font = font_Label,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
            };

            var detailedFP = queries_FP.RetrieveACNoAndDateIssued();
            if (!string.IsNullOrEmpty(detailedFP.acNo))
            {
                textBox_ACNo.Text = detailedFP.acNo;
            }
            if (detailedFP.dateIssued.HasValue)
            {
                dateTimePicker_DateIssued.Value = detailedFP.dateIssued.Value;
            }

            Label label_PWDSignature = new Label
            {
                Parent = panel_Details,
                Width = componentWidth,
                Text = "SC/PWD Signature:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
                Visible = false,
            };

            textBox_PWDSignature = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth,
                Font = font_Label,
                Visible = false,
            };
            Button button_Save = new Button
            {
                Parent = panel_Details,
                Height = 26,
                Margin = new Padding(0, 10, 0, 10),
                Width = sideBarWidth + 15,
                Text = "SAVE",
                BackColor = Color.Transparent,
            };

            button_Save.Click += (sender, e) =>
            {
                queries_FP.UpdateACNoAndDateIssued(
                  textBox_ACNo.Text,
                  dateTimePicker_DateIssued.Value
                  );
            };

            checkBox_EnableExpDate = new CheckBox
            {
                Parent = panel_Details,
                Text = "Enable Amount | Item Code | Exp. Date",
                Width = componentWidth,
                Font = font_Label,
                Checked = true,
                Visible = false,
            };

            checkBox_IncludeDateIssued = new CheckBox
            {
                Parent = panel_Details,
                Text = "Include Date Issued",
                Width = componentWidth,
                Font = font_Label,
                Checked = false,
                Visible = true,
            };

            checkBox_LessEWT = new CheckBox
            {
                Parent = panel_Details,
                Text = "Less EWT (1%) ?",
                Width = componentWidth,
                Font = font_Label,
                Checked = false,
            };

            radioButton_VATInclusive = new RadioButton
            {
                Parent = panel_Details,
                Text = "VAT Inclusive",
                Width = componentWidth / 2 - 10,
                Font = font_Label,
                Checked = true,
            };

            radioButton_VATExclusive = new RadioButton
            {
                Parent = panel_Details,
                Text = "VAT Exclusive",
                Width = componentWidth / 2 - 10,
                Font = font_Label,
            };

            return panel_Details;
        }

        public Panel TitlePanel()
        {
            Panel panel_Title = new Panel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(5),
                Height = 50,
                BackColor = Color.FromArgb(51, 160, 90),

            };

            FlowLayoutPanel panel_Left = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.LeftToRight,
                //BackColor = Color.Red,
                Width = 750,
                Padding = new Padding(2),
            };

            Button button_SyncData = new Button
            {
                Parent = panel_Left,
                Text = "Sync Data",
                Font = new Font("Microsoft Sans Serif", 8),
                Width = 90,
                Height = 32,
                BackColor = Color.White,
            };
            button_SyncData.Click += async (sender, e) =>
            {
                //ExportToJSON();
                try
                {
                    DialogResult result = MessageBox.Show("Do you want to sync data from QuickBooks?",
                                         "Sync Confirmation",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        AccessDatabase accessDatabase = new AccessDatabase();

                        using (var progressForm = new Form())
                        {
                            progressForm.StartPosition = FormStartPosition.CenterScreen;
                            progressForm.Size = new System.Drawing.Size(300, 100);
                            progressForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                            progressForm.MaximizeBox = false;
                            progressForm.MinimizeBox = false;
                            progressForm.ControlBox = false;
                            progressForm.Text = "Syncing";

                            var label = new Label
                            {
                                Text = "Syncing data from QuickBooks. Please wait...",
                                Dock = DockStyle.Fill,
                                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                            };

                            progressForm.Controls.Add(label);
                            progressForm.Show();
                            progressForm.BringToFront();

                            await accessDatabase.FetchCreateAndSaveData(tableNames);

                            progressForm.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while syncing data: " + ex.Message);
                }
            };

            Panel panel_Right = new Panel
            {
                Dock = DockStyle.Right,
                //BackColor = Color.Blue,
                Width = 750,
            };

            Label labelTop = new Label
            {
                Parent = panel_Right,
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Regular),
                Dock = DockStyle.Fill,
                //Text = "QUICKBOOKS SALES INVOICE",
                Text = "Q B S I",
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.White,
            };

            panel_Left.Parent = panel_Title;
            panel_Right.Parent = panel_Title;

            return panel_Title;
        }

        public Panel SidebarPanel()
        {
            FlowLayoutPanel panel_SideBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = sideBarWidth,
                Padding = new Padding(2),
                //BackColor = Color.Green,
                BackColor = Color.FromArgb(10, 130, 80),
            };

            // ------------------------------------------
            FlowLayoutPanel panel_Forms = Panel_Forms();
            FlowLayoutPanel panel_SeriesNumber = Panel_SeriesNumber();
            FlowLayoutPanel panel_RefNumber = Panel_RefNumber();
            FlowLayoutPanel panel_Signatory = Panel_Signatory();
            FlowLayoutPanel panel_Printing = Panel_Printing();

            // ------------------------------------------
            panel_SideBar.Controls.Add(panel_Forms);
            panel_SideBar.Controls.Add(panel_SeriesNumber);
            panel_SideBar.Controls.Add(panel_RefNumber);
            panel_SideBar.Controls.Add(panel_Signatory);
            panel_SideBar.Controls.Add(panel_Printing);

            return panel_SideBar;
        }

        private FlowLayoutPanel Panel_Forms()
        {
            FlowLayoutPanel panel_Forms = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 62,
                Width = sideBarWidth - 10,
                BackColor = Color.LightGray,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
            };

            Label label_FormText = new Label
            {
                Parent = panel_Forms,
                Width = sideBarWidth - 10,
                Text = "SELECT FORM:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            comboBox_Forms = new ComboBox
            {
                Parent = panel_Forms,
                Width = sideBarWidth - 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = font_Label,
            };
            comboBox_Forms.Items.AddRange(new string[]
            {
                "",
                "Sales Invoice",
                //"Delivery Receipt",
            });
            comboBox_Forms.SelectedIndex = 1;
            comboBox_Forms.SelectedIndexChanged += ComboBox_Forms_SelectedIndexChanged;

            return panel_Forms;
        }

        private FlowLayoutPanel Panel_SeriesNumber()
        {
            AccessDatabase accessDatabase = new AccessDatabase();
            seriesNumber = accessDatabase.GetSeriesNumberFromDatabase(tableName);

            FlowLayoutPanel panel_SeriesNumber = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 64,
                Width = sideBarWidth - 10,
                BackColor = Color.LightGray,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
            };

            Label label_SeriesNumberText = new Label
            {
                Parent = panel_SeriesNumber,
                Width = sideBarWidth - 30,
                Text = "Current Series Number:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_SeriesNumber = new TextBox
            {
                Parent = panel_SeriesNumber,
                Width = 156,
                Font = new Font("Microsoft Sans Serif", 10),
            };
            textBox_SeriesNumber.Text = FormatSeriesNumber(seriesNumber);
            textBox_SeriesNumber.Leave += TextBox_SeriesNumber_Leave;

            Button button_Decrement = new Button
            {
                Parent = panel_SeriesNumber,
                Height = 28,
                Width = 28,
                Text = "-",
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 1, 0, 0),
                BackColor = Color.Transparent,
            };
            button_Decrement.Click += (sender, e) =>
            {
                if (seriesNumber != 0)
                {
                    seriesNumber--;
                    textBox_SeriesNumber.Text = FormatSeriesNumber(seriesNumber);
                    accessDatabase.UpdateManualSeriesNumber(tableName, seriesNumber);
                }
            };

            Button button_Increment = new Button
            {
                Parent = panel_SeriesNumber,
                Height = 28,
                Width = 28,
                Text = "+",
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(3, 1, 3, 0),
                BackColor = Color.Transparent,
            };
            button_Increment.Click += (sender, e) =>
            {
                seriesNumber++;
                textBox_SeriesNumber.Text = FormatSeriesNumber(seriesNumber);
                accessDatabase.UpdateManualSeriesNumber(tableName, seriesNumber);
            };

            return panel_SeriesNumber;
        }

        private FlowLayoutPanel Panel_RefNumber()
        {
            FlowLayoutPanel panel_RefNumber = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 92,
                Width = sideBarWidth - 10,
                BackColor = Color.LightGray,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
            };

            Label label_RefNumberText = new Label
            {
                Parent = panel_RefNumber,
                Width = sideBarWidth - 30,
                Text = "ENTER REFERENCE NUMBER:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            TextBox textBox_ReferenceNumber = new TextBox
            {
                Parent = panel_RefNumber,
                Width = sideBarWidth - 30, // 190
                Font = font_Label,
            };

            Button button_SearchRefNum = new Button
            {
                Parent = panel_RefNumber,
                Height = 26,
                Width = sideBarWidth - 30,
                Text = "SEARCH",
                BackColor = Color.Transparent,
            };
            button_SearchRefNum.Click += (sender, e) =>
            {
                try
                {
                    if (comboBox_Forms.SelectedIndex == 0)
                    {
                        MessageBox.Show("Please select a form.", "Notice", MessageBoxButtons.OK);
                    }
                    else if (comboBox_Forms.SelectedIndex != 0 && textBox_ReferenceNumber.Text != "")
                    {
                        string refNumber = textBox_ReferenceNumber.Text;

                        string vatType = radioButton_VATInclusive.Checked ? "Inclusive" : "Exclusive";
                        //bool vatType = radioButton_VATInclusive.Checked ? true : false;

                        string note = textBox_Note.Text;
                        string businessStyle = textBox_BusinessStyle.Text;
                        string pwdSignature = textBox_PWDSignature.Text;
                        string acNo = textBox_ACNo.Text;
                        DateTime dateIssued = dateTimePicker_DateIssued.Value;
                        bool includeDateIssued = checkBox_IncludeDateIssued.Checked;
                        bool isEnableExpDateChecked = checkBox_EnableExpDate.Checked;
                        bool isLessEWTChecked = checkBox_LessEWT.Checked;

                        string seriesNumberRef = textBox_SeriesNumber.Text;

                        //string signatoryPreparedBy = textBox_SignatoryPreparedBy.Text;
                        //string signatoryCheckedBy = textBox_SignatoryCheckedBy.Text;
                        //string signatoryApprovedBy = textBox_SignatoryApprovedBy.Text;

                        Queries_FP accessQueries = new Queries_FP();
                        List<InvoiceData> invoice = accessQueries.GetInvoiceData(refNumber);

                        if (invoice.Count == 0)
                        {
                            MessageBox.Show("No invoice found for the given reference number.", "Notice", MessageBoxButtons.OK);
                            return;
                        }

                        Layout_FP layout_FP = new Layout_FP();
                        PaperSize paperSize = new PaperSize("Custom", 850, 1100);

                        printDocument = new PrintDocument();
                        printDocument.DefaultPageSettings.PaperSize = paperSize;
                        printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
                        printDocument.PrintPage += (s, ev) =>
                        {
                            //layout_NHC.PrintPage_NHC(s, ev, invoice, comboBox_Forms.SelectedIndex, note, businessStyle, pwdSignature, isEnableExpDateChecked);
                            //if (comboBox_Forms.SelectedIndex == 1)
                            layout_FP.Layout_SalesInvoice(ev, invoice, vatType, businessStyle, includeDateIssued, isLessEWTChecked, acNo, dateIssued, seriesNumberRef);
                            /*else if (comboBox_Forms.SelectedIndex == 2)
                                layout_Enclosure.Layout_DeliveryReceipt(ev, invoice, note, businessStyle, pwdSignature, isEnableExpDateChecked, signatoryName);*/
                        };
                        printPreviewControl.Document = printDocument;
                        printPreviewControl.Visible = true;
                        panel_Printing.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("Please enter a reference number.", "Notice", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            return panel_RefNumber;
        }

        private FlowLayoutPanel Panel_Signatory()
        {
            Queries_FP queries_FP = new Queries_FP();

            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 215, // 92 || 68
                Width = sideBarWidth - 10,
                BackColor = Color.LightGray,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
            };

            Label label_SignatoryText = new Label
            {
                Parent = panel,
                Width = sideBarWidth - 30,
                Text = "SIGNATORY:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            var signatory = queries_FP.RetrieveSignatory();

            Label label_PreparedBy = new Label
            {
                Parent = panel,
                Width = sideBarWidth - 30,
                Text = "Prepared By:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_SignatoryPreparedBy = new TextBox
            {
                Parent = panel,
                Width = sideBarWidth - 30, // 190
                Font = font_Label,
            };
            textBox_SignatoryPreparedBy.Text = signatory.preparedBy;

            Label label_CheckedBy = new Label
            {
                Parent = panel,
                Width = sideBarWidth - 30,
                Text = "Approved By:", // Checked By
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_SignatoryCheckedBy = new TextBox
            {
                Parent = panel,
                Width = sideBarWidth - 30, // 190
                Font = font_Label,
            };
            textBox_SignatoryCheckedBy.Text = signatory.checkedBy;

            Label label_ApprovedBy = new Label
            {
                Parent = panel,
                Width = sideBarWidth - 30,
                Text = "Received By:",//Approved By
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_SignatoryApprovedBy = new TextBox
            {
                Parent = panel,
                Width = sideBarWidth - 30, // 190
                Font = font_Label,
            };
            textBox_SignatoryApprovedBy.Text = signatory.approvedBy;

            Button button_Save = new Button
            {
                Parent = panel,
                Height = 26,
                Width = sideBarWidth - 30,
                Text = "SAVE",
                BackColor = Color.Transparent,
            };
            button_Save.Click += (sender, e) =>
            {
                queries_FP.UpdateSignatory(
                    textBox_SignatoryPreparedBy.Text,
                    textBox_SignatoryCheckedBy.Text,
                    textBox_SignatoryApprovedBy.Text
                    );
            };

            return panel;
        }

        private FlowLayoutPanel Panel_Printing()
        {
            panel_Printing = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Width = sideBarWidth - 10,
                BackColor = Color.LightGray,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
            };

            Button button_ZoomOut = new Button
            {
                Parent = panel_Printing,
                Text = "Zoom Out",
                Height = 28,
                Width = 108,
                BackColor = Color.Transparent,
            };
            button_ZoomOut.Click += (sender, e) =>
            {
                if (printPreviewControl.Zoom >= 0.1)
                {
                    printPreviewControl.Zoom -= 0.1;
                }
            };

            Button button_ZoomIn = new Button
            {
                Parent = panel_Printing,
                Text = "Zoom In",
                Height = 28,
                Width = 108,
                BackColor = Color.Transparent,
            };
            button_ZoomIn.Click += (sender, e) =>
            {
                printPreviewControl.Zoom += 0.1;
            };

            Button button_Print = new Button
            {
                Parent = panel_Printing,
                Text = "Print",
                Height = 28,
                Width = 222,
                BackColor = Color.Transparent,
            };
            button_Print.Click += (sender, e) =>
            {
                AccessDatabase accessDatabase = new AccessDatabase();
                try
                {
                    PrintDialog printDialog = new PrintDialog
                    {
                        Document = printDocument
                    };

                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        printDialog.Document.Print();
                        printPreviewControl.Visible = false;
                        printPreviewControl.Zoom = 1;
                        panel_Printing.Visible = false;

                        seriesNumber++;
                        textBox_SeriesNumber.Text = FormatSeriesNumber(seriesNumber);
                        accessDatabase.UpdateManualSeriesNumber(tableName, seriesNumber);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while printing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            return panel_Printing;
        }

        private void ComboBox_Forms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Forms.SelectedIndex == 1)
            {
                //panel_Details.Visible = false;
                //panel_Details.Width = 0;
                checkBox_EnableExpDate.Visible = false;
                checkBox_LessEWT.Visible = true;
            }
            else if (comboBox_Forms.SelectedIndex == 2)
            {
                //panel_Details.Visible = true;
                //panel_Details.Width = sideBarWidth + 30;
                checkBox_EnableExpDate.Visible = true;
                checkBox_LessEWT.Visible = false;
            }
            else
            {

            }
        }

        private void TextBox_SeriesNumber_Leave(object sender, EventArgs e)
        {
            string tableName = "FP";
            AccessDatabase accessDatabase = new AccessDatabase();
            accessDatabase.UpdateManualSeriesNumber(tableName, seriesNumber);
        }

        private string FormatSeriesNumber(long seriesNumber)
        {
            /*const long maxSuffix = 99999999999;

            long prefix = (seriesNumber / (maxSuffix + 1)) + 1;
            long suffix = seriesNumber % (maxSuffix + 1);

            return $"{prefix:000000}-{suffix:0000000000}";*/

            //return $"{seriesNumber:00000}-000000000";

            return $"{seriesNumber:000000}";
        }
    }
}
