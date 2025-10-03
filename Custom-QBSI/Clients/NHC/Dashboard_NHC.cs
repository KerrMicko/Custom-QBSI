using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static Custom_QBSI.Clients.NHC.Dataclass_NHC;

namespace Custom_QBSI.Clients.NHC
{
    public class Dashboard_NHC
    {
        private static readonly List<string> tableNames = new List<string> { "Account",
            "Customer",
            "Invoice", "InvoiceLine", "InvoiceLinkedTxn",
            "Item" };

        private PrintDocument printDocument;
        private PrintPreviewControl printPreviewControl;

        private ComboBox comboBox_Forms;
        private DataGridView dataGridView_Lines;

        // Details
        private CheckBox checkBox_EnableExpDate;
        private CheckBox checkBox_LessEWT;
        private TextBox textBox_Note;
        private TextBox textBox_BusinessStyle;
        private TextBox textBox_PWDSignature;
        private RadioButton radioButton_VATInclusive;
        private RadioButton radioButton_VATExclusive;

        private FlowLayoutPanel panel_POTIN;
        private Label label_PONumber;
        private TextBox textBox_PONumber;

        private Label label_TIN;
        private TextBox textBox_TIN;

        private Label label_Address;
        private TextBox textBox_Address;

        private Label label_Terms;
        private TextBox textBox_Terms;

        private Label label_StoreCode;
        private TextBox textBox_StoreCode;

        private Button button_SaveDetails;

        // Signatory
        private TextBox textBox_SignatoryName;

        private FlowLayoutPanel panel_Printing;
        private FlowLayoutPanel panel_Details;

        static int sideBarWidth = 250;
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

            label_Address = new Label
            {
                Parent = panel_Details,
                Width = componentWidth,
                Text = "Address:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
                Visible = false, // hidden by default
            };
            textBox_Address = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth,
                Font = font_Label,
                Multiline = true,
                Height = 50,
                Visible = false, // hidden by default
            };

            label_Terms = new Label
            {
                Parent = panel_Details,
                Width = componentWidth,
                Text = "Terms:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
                Visible = false, // hidden by default
            };
            textBox_Terms = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth,
                Font = font_Label,
                Visible = false, // hidden by default
            };

            label_StoreCode = new Label
            {
                Parent = panel_Details,
                Width = componentWidth,
                Text = "Store Code:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
                Visible = false, // hidden by default
            };
            textBox_StoreCode = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth,
                Font = font_Label,
                Visible = false, // hidden by default
            };

            panel_POTIN = new FlowLayoutPanel
            {
                Parent = panel_Details,
                Width = 280,           // max width
                Height = 30,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Visible = false // hidden by default
            };

            label_PONumber = new Label
            {
                Parent = panel_POTIN,
                Width = 35,
                Text = "PO#:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label
            };
            textBox_PONumber = new TextBox
            {
                Parent = panel_POTIN,
                Width = 89,
                Font = font_Label
            };

            label_TIN = new Label
            {
                Parent = panel_POTIN,
                Width = 30,
                Text = "TIN:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label
            };
            textBox_TIN = new TextBox
            {
                Parent = panel_POTIN,
                Width = 85,
                Font = font_Label
            };

            checkBox_EnableExpDate = new CheckBox
            {
                Parent = panel_Details,
                Text = "Enable Amount | Item Code | Exp. Date",
                Width = componentWidth,
                Font = font_Label,
                Checked = true,
                Visible = true,
            };

            dataGridView_Lines = new DataGridView
            {
                Parent = panel_Details,
                Width = componentWidth,
                Height = 200,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                Font = font_Label,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ScrollBars = ScrollBars.Vertical,
                Visible = false // hidden initially
            };

            // Columns: ItemDescription, SalesPrice, ExpirationDate
            dataGridView_Lines.Columns.Clear();

            dataGridView_Lines.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Description",
                Name = "Description"
            });
            dataGridView_Lines.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Name = "Price"
            });
            dataGridView_Lines.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Expiration Date",
                Name = "ExpirationDate"
            });

            checkBox_LessEWT = new CheckBox
            {
                Parent = panel_Details,
                Text = "Less EWT (1%) ?",
                Width = componentWidth,
                Font = font_Label,
                Checked = false,
            };

            button_SaveDetails = new Button
            {
                Parent = panel_Details,
                Height = 26,
                Width = componentWidth,
                Text = "SAVE DETAILS",
                BackColor = Color.Transparent,
                Visible = false,
            };
            button_SaveDetails.Click += (sender, e) =>
            {
                string address = textBox_Address.Text;
                string terms = textBox_Terms.Text;
                string storeCode = textBox_StoreCode.Text;
                string poNumber = textBox_PONumber.Text;
                string tin = textBox_TIN.Text;

                Queries_NHC queries_NHC = new Queries_NHC();
                queries_NHC.UpdateSignatory_DR(address, terms, storeCode, poNumber, tin);
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

            ComboBox comboBox_Year = new ComboBox
            {
                Parent = panel_Left,
                Width = 70,
                Margin = new Padding(3, 7, 0, 0),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Microsoft Sans Serif", 10),
            };
            comboBox_Year.Items.AddRange(new string[]
            {
                "2025",
                "2024",
                "2023",
                "2022",
                "2021",
                "2020",
            });
            comboBox_Year.SelectedIndex = 0;

            button_SyncData.Click += async (sender, e) =>
            {
                string selectedYear = comboBox_Year.SelectedItem.ToString();
                try
                {
                    DialogResult result = MessageBox.Show($"Do you want to sync data for Year {selectedYear} from QuickBooks?",
                                         "Sync Confirmation",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        QBDataSync_NHC qbDataSync_NHC = new QBDataSync_NHC();

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

                            await qbDataSync_NHC.FetchCreateAndSaveData(tableNames, selectedYear);

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

            //panel_Left.Parent = panel_Title;
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
            FlowLayoutPanel panel_RefNumber = Panel_RefNumber();
            FlowLayoutPanel panel_Signatory = Panel_Signatory();
            FlowLayoutPanel panel_Prionting = Panel_Printing();

            // ------------------------------------------
            panel_SideBar.Controls.Add(panel_Forms);
            panel_SideBar.Controls.Add(panel_RefNumber);
            panel_SideBar.Controls.Add(panel_Signatory);
            panel_SideBar.Controls.Add(panel_Prionting);

            return panel_SideBar;
        }

        Font font_Label = new Font("Microsoft Sans Serif", 9);

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
                "Delivery Receipt",
            });
            comboBox_Forms.SelectedIndex = 1;
            comboBox_Forms.SelectedIndexChanged += ComboBox_Forms_SelectedIndexChanged;

            return panel_Forms;
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
            button_SearchRefNum.Click += async (sender, e) =>
            {
                try
                {
                    // 1️⃣ Read all UI control values BEFORE Task.Run
                    int selectedFormIndex = comboBox_Forms.SelectedIndex;
                    string refNumber = textBox_ReferenceNumber.Text.Trim();
                    string vatType = radioButton_VATInclusive.Checked ? "Inclusive" : "Exclusive";
                    string note = textBox_Note.Text;
                    string businessStyle = textBox_BusinessStyle.Text;
                    string pwdSignature = textBox_PWDSignature.Text;
                    bool isEnableExpDateChecked = checkBox_EnableExpDate.Checked;
                    bool isLessEWTChecked = checkBox_LessEWT.Checked;
                    string signatoryName = textBox_SignatoryName.Text;
                    string address = textBox_Address.Text;
                    string terms = textBox_Terms.Text;
                    string storeCode = textBox_StoreCode.Text;
                    string poNumber = textBox_PONumber.Text;
                    string tin = textBox_TIN.Text;

                    // 2️⃣ Validate inputs
                    if (selectedFormIndex == 0)
                    {
                        MessageBox.Show("Please select a form.", "Notice", MessageBoxButtons.OK);
                        LogMessage("User did not select a form.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(refNumber))
                    {
                        MessageBox.Show("Please enter a reference number.", "Notice", MessageBoxButtons.OK);
                        LogMessage("Reference number was empty.");
                        return;
                    }

                    LogMessage($"Searching invoice for RefNumber: {refNumber}");

                    // 3️⃣ Create progress form (non-blocking)
                    var progressForm = new Form
                    {
                        StartPosition = FormStartPosition.CenterScreen,
                        Size = new Size(400, 120),
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false,
                        ControlBox = false,
                        Text = "Querying"
                    };

                    var label = new Label
                    {
                        Text = "Querying data from QuickBooks. Please wait...",
                        Dock = DockStyle.Top,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Height = 40
                    };
                    progressForm.Controls.Add(label);

                    var progressBar = new ProgressBar
                    {
                        Minimum = 0,
                        Maximum = 100,
                        Value = 0,
                        Dock = DockStyle.Bottom,
                        Height = 20
                    };
                    progressForm.Controls.Add(progressBar);

                    var timer = new Timer { Interval = 50 };
                    timer.Tick += (s, args) =>
                    {
                        if (progressBar.Value < 95)
                            progressBar.Value += 1;
                    };
                    timer.Start();

                    progressForm.Show(); // Non-blocking

                    // 4️⃣ Run query asynchronously (no UI control access inside Task.Run)
                    var result = await Task.Run(() =>
                    {
                        List<Custom_QBSI.Clients.NHC.AltDataClass_NHC.InvoiceData> invoice = null;
                        List<Custom_QBSI.Clients.NHC.AltDataClass_NHC.TransferInventoryData> transfers = null;

                        if (selectedFormIndex == 1)
                            invoice = AltQBDataSync_NHC.GetInvoiceByRefNumber(refNumber);
                        else if (selectedFormIndex == 2)
                            transfers = AltQBDataSync_NHC.GetTransferInventoryByRefNumber(refNumber);

                        return new { Invoice = invoice, Transfers = transfers };
                    });

                    // 5️⃣ Stop progress and close form
                    timer.Stop();
                    progressBar.Value = 100;
                    await Task.Delay(200);
                    progressForm.Close();

                    // 6️⃣ Check results
                    if (selectedFormIndex == 1 && (result.Invoice == null || result.Invoice.Count == 0))
                    {
                        MessageBox.Show("No invoice found for the given reference number.", "Notice", MessageBoxButtons.OK);
                        LogMessage($"No invoice found for RefNumber: {refNumber}");
                        return;
                    }

                    if (selectedFormIndex == 2 && (result.Transfers == null || result.Transfers.Count == 0))
                    {
                        MessageBox.Show("No transfer records found for the given reference number.", "Notice", MessageBoxButtons.OK);
                        LogMessage($"No transfers found for RefNumber: {refNumber}");
                        return;
                    }

                    // 7️⃣ Prepare print
                    AltLayout_NHC altLayout_NHC = new AltLayout_NHC();
                    PaperSize paperSize = new PaperSize("Custom", 850, 1100);

                    printDocument = new PrintDocument();
                    printDocument.DefaultPageSettings.PaperSize = paperSize;
                    printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;

                    printDocument.PrintPage += (pSender, ev) =>
                    {
                        if (selectedFormIndex == 1)
                            altLayout_NHC.Layout_SalesInvoice(ev, result.Invoice, note, vatType, businessStyle, signatoryName, isEnableExpDateChecked, isLessEWTChecked);
                        else if (selectedFormIndex == 2)
                            altLayout_NHC.Layout_DeliveryReceipt(ev, result.Transfers, note, businessStyle, pwdSignature, address, terms, storeCode, poNumber, tin, isEnableExpDateChecked, signatoryName);
                    };

                    printPreviewControl.Document = printDocument;
                    printPreviewControl.Visible = true;
                    panel_Printing.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogMessage($"ERROR: {ex}");
                }
            };





            return panel_RefNumber;
        }

        private void LogMessage(string message)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_log.txt");
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

            try
            {
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Avoid crashing if logging fails
            }
        }

        private FlowLayoutPanel Panel_Signatory()
        {
            Queries_NHC queries_NHC = new Queries_NHC();

            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 115, // 92 || 68
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

            Label label_Name = new Label
            {
                Parent = panel,
                Width = sideBarWidth - 30,
                Text = "Name:",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
            };

            textBox_SignatoryName = new TextBox
            {
                Parent = panel,
                Width = sideBarWidth - 30, // 190
                Font = font_Label,
            };

            textBox_SignatoryName.Text = queries_NHC.RetrieveSignatory();

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
                queries_NHC.UpdateSignatory(textBox_SignatoryName.Text);
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
                checkBox_EnableExpDate.Visible = true;
                checkBox_LessEWT.Visible = true;

                radioButton_VATInclusive.Visible = true;
                radioButton_VATExclusive.Visible = true;

                // Hide all textboxes
                label_Address.Visible = false;
                textBox_Address.Visible = false;
                label_Terms.Visible = false;
                textBox_Terms.Visible = false;
                label_StoreCode.Visible = false;
                textBox_StoreCode.Visible = false;
                label_PONumber.Visible = false;
                textBox_PONumber.Visible = false;
                label_TIN.Visible = false;
                textBox_TIN.Visible = false;
                panel_POTIN.Visible = false;

                button_SaveDetails.Visible = false;

                // Hide DataGridView
                dataGridView_Lines.Visible = false;
            }
            else if (comboBox_Forms.SelectedIndex == 2)
            {
                checkBox_EnableExpDate.Visible = true;
                checkBox_LessEWT.Visible = false;

                radioButton_VATInclusive.Visible = false;
                radioButton_VATExclusive.Visible = false;

                label_Address.Visible = true;
                textBox_Address.Visible = true;
                label_Terms.Visible = true;
                textBox_Terms.Visible = true;
                label_StoreCode.Visible = true;
                textBox_StoreCode.Visible = true;
                label_PONumber.Visible = true;
                textBox_PONumber.Visible = true;
                label_TIN.Visible = true;
                textBox_TIN.Visible = true;
                panel_POTIN.Visible = true;

                button_SaveDetails.Visible = true;

                // Show DataGridView
                dataGridView_Lines.Visible = true;

                // Optional: populate DataGridView with sample or existing data
                dataGridView_Lines.Rows.Clear();
                dataGridView_Lines.Rows.Add("Sample Item 1", "100.00", "");
                dataGridView_Lines.Rows.Add("Sample Item 2", "250.00", "");

                Queries_NHC queries_NHC = new Queries_NHC();
                var data = queries_NHC.RetrieveSignatory_DR();

                textBox_Address.Text = data[0];
                textBox_Terms.Text = data[1];
                textBox_StoreCode.Text = data[2];
                textBox_PONumber.Text = data[3];
                textBox_TIN.Text = data[4];
            }
            else
            {
                checkBox_EnableExpDate.Visible = false;
                checkBox_LessEWT.Visible = false;
                radioButton_VATInclusive.Visible = false;
                radioButton_VATExclusive.Visible = false;

                label_Address.Visible = false;
                textBox_Address.Visible = false;
                label_Terms.Visible = false;
                textBox_Terms.Visible = false;
                label_StoreCode.Visible = false;
                textBox_StoreCode.Visible = false;
                label_PONumber.Visible = false;
                textBox_PONumber.Visible = false;
                label_TIN.Visible = false;
                textBox_TIN.Visible = false;
                panel_POTIN.Visible = false;

                button_SaveDetails.Visible = false;

                // Hide DataGridView
                dataGridView_Lines.Visible = false;
            }
        }


    }
}
