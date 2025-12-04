using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Custom_QBSI.Clients.NHC.Dataclass_NHC;
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
        private List<AltDataClass_NHC.TransferInventoryData> lastQueriedTransfers;
        private bool allowPriceEditing = true;

        public TextBox textBox_ReferenceNumber;


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

        private Label label_CustomerName;
        private TextBox textBox_CustomerName;

        private Button button_SaveDetails;
        private Button button_PrintNewData;


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

            label_CustomerName = new Label
            {
                Text = "Customer Name:",
                Parent = panel_Details,
                Width = componentWidth,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = font_Label,
                Visible = false,
            };

            // Define the TextBox
            textBox_CustomerName = new TextBox
            {
                Parent = panel_Details,
                Width = componentWidth,
                Visible = false,
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
                Visible = false
            };

            dataGridView_Lines.Columns.Clear();
            dataGridView_Lines.Columns.Add("Description", "Description");
            dataGridView_Lines.Columns.Add("Price", "Price");
            dataGridView_Lines.Columns.Add("ExpirationDate", "Expiration Date");

            button_PrintNewData = new Button
            {
                Parent = panel_Details,
                Height = 26,
                Width = componentWidth,
                Text = "RE-GENERATE DOCUMENT",
                BackColor = Color.Transparent,
                Visible = false,
            };

            button_PrintNewData.Click += button_PrintEdited_Click;

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
                if (textBox_ReferenceNumber == null) return;

                string refNumber = textBox_ReferenceNumber.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(refNumber))
                {
                    MessageBox.Show("Please enter a Reference Number before saving.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Read existing fields
                string address = textBox_Address?.Text ?? "";
                string terms = textBox_Terms?.Text ?? "";
                string storeCode = textBox_StoreCode?.Text ?? "";
                string poNumber = textBox_PONumber?.Text ?? "";
                string tin = textBox_TIN?.Text ?? "";
                string businessStyle = textBox_BusinessStyle?.Text ?? "";
                string note = textBox_Note?.Text ?? "";

                // 🆕 Read the new Customer Name
                // Ensure you have this TextBox created in your UI!
                string customerName = textBox_CustomerName?.Text ?? "";

                try
                {
                    Queries_NHC queries_NHC = new Queries_NHC();

                    // Pass the new customerName at the end
                    queries_NHC.SaveOrUpdateHistoryDR(refNumber, address, terms, storeCode, poNumber, tin, businessStyle, note, customerName);

                    MessageBox.Show("Details saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

            textBox_ReferenceNumber = new TextBox
            {
                Parent = panel_RefNumber,
                Width = sideBarWidth - 30,
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
                    // 1️⃣ Read BASIC control values needed for Validation and Querying
                    int selectedFormIndex = comboBox_Forms.SelectedIndex;
                    string refNumber = textBox_ReferenceNumber.Text.Trim();

                    // Settings (Checkboxes/Radios) can be read here
                    string vatType = radioButton_VATInclusive.Checked ? "Inclusive" : "Exclusive";
                    bool isEnableExpDateChecked = checkBox_EnableExpDate.Checked;
                    bool isLessEWTChecked = checkBox_LessEWT.Checked;

                    // 2️⃣ Validate input
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

                    // 3️⃣ Clear all DR detail fields before loading new data
                    textBox_Address.Text = "";
                    textBox_StoreCode.Text = "";
                    textBox_PONumber.Text = "";
                    textBox_TIN.Text = "";
                    textBox_BusinessStyle.Text = "";
                    textBox_Note.Text = "";
                    textBox_Terms.Text = "";
                    textBox_PWDSignature.Text = "";
                    textBox_CustomerName.Text = "";
                    dataGridView_Lines.Rows.Clear();
                    dataGridView_Lines.Visible = false;
                    button_PrintNewData.Visible = false;

                    // 4️⃣ Load saved DR history ONLY if Delivery Receipt is selected
                    if (selectedFormIndex == 2)
                    {
                        Queries_NHC q = new Queries_NHC();
                        HistoryDRDetails savedDetails = q.GetHistoryDRDetails(refNumber);

                        if (savedDetails != null)
                        {
                            textBox_Address.Text = savedDetails.Address ?? "";
                            textBox_StoreCode.Text = savedDetails.StoreCode ?? "";
                            textBox_PONumber.Text = savedDetails.PONumber ?? "";
                            textBox_TIN.Text = savedDetails.TIN ?? "";
                            textBox_BusinessStyle.Text = savedDetails.BusinessStyle ?? "";
                            textBox_Note.Text = savedDetails.Note ?? "";
                            textBox_Terms.Text = savedDetails.Terms ?? "";
                            textBox_CustomerName.Text = savedDetails.Cname ?? ""; // Ensure this matches your class property

                            LogMessage("Loaded saved DR details from HistoryDRDetails.");
                        }
                        else
                        {
                            LogMessage("No saved DR details found for this RefNumber.");
                        }
                    }

                    // 5️⃣ Create progress form
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

                    progressForm.Show();

                    // 6️⃣ Run QuickBooks query asynchronously
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

                    // Stop progress
                    timer.Stop();
                    progressBar.Value = 100;
                    await Task.Delay(200);
                    progressForm.Close();

                    // Check if data is found
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

                    // Handle Edit Mode (Expiration Dates)
                    if (selectedFormIndex == 2 && isEnableExpDateChecked)
                    {
                        DialogResult editChoice = MessageBox.Show(
                            "How would you like to proceed?\n\n" +
                            "Yes - Edit BOTH Price and Expiration Date\n" +
                            "No - Edit ONLY Expiration Date (Price will remain hidden)\n" +
                            "Cancel - Proceed to print without editing",
                            "Edit Options",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question
                        );

                        if (editChoice != DialogResult.Cancel)
                        {
                            // Logic to enable editing grid
                            dataGridView_Lines.Rows.Clear();
                            dataGridView_Lines.Visible = true;
                            panel_Printing.Visible = false;
                            button_PrintNewData.Visible = true;

                            lastQueriedTransfers = result.Transfers;

                            foreach (DataGridViewColumn col in dataGridView_Lines.Columns)
                                col.ReadOnly = true;

                            allowPriceEditing = (editChoice == DialogResult.Yes);

                            dataGridView_Lines.Columns["ExpirationDate"].ReadOnly = false;
                            dataGridView_Lines.Columns["Price"].ReadOnly = !allowPriceEditing;
                            dataGridView_Lines.Columns["Price"].Visible = allowPriceEditing;

                            foreach (var tran in result.Transfers)
                            {
                                if (tran.Lines != null)
                                {
                                    decimal totalAmount = 0m;
                                    foreach (var line in tran.Lines)
                                    {
                                        string desc = line.ItemDescription ?? "N/A";
                                        string expDate = "";
                                        string price = line.AverageCost.ToString("0.00");

                                        dataGridView_Lines.Rows.Add(desc, price, expDate);

                                        decimal lineTotal = (decimal)line.AverageCost * (decimal)line.QuantityTransfer;
                                        totalAmount += lineTotal;
                                    }

                                    int totalRowIndex = dataGridView_Lines.Rows.Add("TOTAL", totalAmount.ToString("0.00"), "");
                                    DataGridViewRow totalRow = dataGridView_Lines.Rows[totalRowIndex];

                                    totalRow.DefaultCellStyle.Font = new Font(dataGridView_Lines.Font, FontStyle.Bold);
                                    totalRow.DefaultCellStyle.BackColor = Color.LightYellow;
                                    totalRow.ReadOnly = !allowPriceEditing;
                                }
                            }

                            MessageBox.Show(
                                allowPriceEditing
                                    ? "You can now edit Price and Expiration Date before printing."
                                    : "You can now edit Expiration Date only. Price will remain hidden in the re-generated document.",
                                "Edit Mode",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            return; // Stop here so user can edit. They will click "Print" later.
                        }
                    }

                    // 7️⃣ CRITICAL FIX: RE-READ VARIABLES FROM UI HERE
                    // We read these NOW because the History loading (Step 4) might have updated the TextBoxes.
                    // If we read them at the very top, we would be sending empty strings to the printer.
                    string currentNote = textBox_Note.Text;
                    string currentBusinessStyle = textBox_BusinessStyle.Text;
                    string currentPwdSignature = textBox_PWDSignature.Text;
                    string currentSignatoryName = textBox_SignatoryName.Text;
                    string currentAddress = textBox_Address.Text;
                    string currentTerms = textBox_Terms.Text;
                    string currentStoreCode = textBox_StoreCode.Text;
                    string currentPoNumber = textBox_PONumber.Text;
                    string currentTin = textBox_TIN.Text;
                    string currentCustomerName = textBox_CustomerName != null ? textBox_CustomerName.Text : "";

                    // 8️⃣ Generate Print Document
                    AltLayout_NHC altLayout_NHC = new AltLayout_NHC();
                    PaperSize paperSize = new PaperSize("Custom", 850, 1100);

                    printDocument = new PrintDocument();
                    printDocument.DefaultPageSettings.PaperSize = paperSize;
                    printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;

                    printDocument.PrintPage += (pSender, ev) =>
                    {
                        if (selectedFormIndex == 1)
                        {
                            altLayout_NHC.Layout_SalesInvoice(
                                ev,
                                result.Invoice,
                                currentNote,
                                vatType,
                                currentBusinessStyle,
                                currentSignatoryName,
                                isEnableExpDateChecked,
                                isLessEWTChecked
                            );
                        }
                        else if (selectedFormIndex == 2)
                        {
                            altLayout_NHC.Layout_DeliveryReceipt(
                            ev,
                            result.Transfers,
                            currentNote,
                            currentBusinessStyle,
                            currentPwdSignature,
                            currentAddress,
                            currentTerms,
                            currentStoreCode,
                            currentPoNumber,
                            currentTin,
                            isEnableExpDateChecked,
                            allowPriceEditing,
                            currentCustomerName,  
                            currentSignatoryName, 
                            dataGridView_Lines
                            );
                        }
                    };

                    printPreviewControl.Document = printDocument;
                    printPreviewControl.Visible = true;
                    panel_Printing.Visible = true;

                    // Hide editing controls (since we are in standard print mode)
                    dataGridView_Lines.Visible = false;
                    button_PrintNewData.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogMessage($"ERROR: {ex}");
                }
            };


            return panel_RefNumber;
        }

        private void button_PrintEdited_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView_Lines.Rows.Count == 0)
                {
                    MessageBox.Show("No data to print.", "Notice", MessageBoxButtons.OK);
                    return;
                }

                if (lastQueriedTransfers == null || lastQueriedTransfers.Count == 0)
                {
                    MessageBox.Show("No transfer data available. Please search a reference number first.", "Notice", MessageBoxButtons.OK);
                    return;
                }

                // ✅ Gather edited values from DataGridView
                var editedLines = new List<(string Description, decimal Price, string ExpirationDate)>();
                foreach (DataGridViewRow row in dataGridView_Lines.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string desc = row.Cells["Description"].Value?.ToString() ?? "";
                        string priceText = row.Cells["Price"].Value?.ToString() ?? "0";
                        decimal.TryParse(priceText, out decimal price);
                        string expDate = row.Cells["ExpirationDate"].Value?.ToString() ?? "";
                        editedLines.Add((desc, price, expDate));
                    }
                }

                // ✅ Apply edited values to transfer lines
                foreach (var tran in lastQueriedTransfers)
                {
                    foreach (var line in tran.Lines)
                    {
                        var edited = editedLines.FirstOrDefault(x => x.Description == line.ItemDescription);
                        if (edited.Description != null)
                        {
                            line.SalesPrice = (double)edited.Price;
                            line.ExpirationDate = edited.ExpirationDate;
                        }
                    }
                }

                // ✅ Recreate and refresh the print document
                AltLayout_NHC altLayout_NHC = new AltLayout_NHC();
                PaperSize paperSize = new PaperSize("Custom", 850, 1100);

                if (printDocument != null)
                {
                    printDocument.PrintPage -= null;
                    printDocument.Dispose();
                }

                printDocument = new PrintDocument();
                printDocument.DefaultPageSettings.PaperSize = paperSize;
                printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;

                // Inside button_PrintEdited_Click
                printDocument.PrintPage += (pSender, ev) =>
                {
                    altLayout_NHC.Layout_DeliveryReceipt(
                        ev,
                        lastQueriedTransfers,
                        textBox_Note.Text,
                        textBox_BusinessStyle.Text,
                        textBox_PWDSignature.Text,
                        textBox_Address.Text,
                        textBox_Terms.Text,
                        textBox_StoreCode.Text,
                        textBox_PONumber.Text,
                        textBox_TIN.Text,
                        checkBox_EnableExpDate.Checked,
                        allowPriceEditing,
                        textBox_CustomerName.Text,
                        textBox_SignatoryName.Text,
                        dataGridView_Lines
                    );
                };

                // ✅ Keep DataGridView visible so user can edit again
                dataGridView_Lines.Visible = true;
                panel_Printing.Visible = true;
                printPreviewControl.Visible = true;

                // ✅ Force refresh of preview
                printPreviewControl.Document = null;
                printPreviewControl.Document = printDocument;
                printPreviewControl.InvalidatePreview();
                printPreviewControl.Refresh();

                // 🟢 Keep Re-generate button visible for repeated edits
                button_PrintNewData.Visible = true;

                MessageBox.Show("Document regenerated with your latest edits.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while regenerating document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                label_CustomerName.Visible = false;
                textBox_CustomerName.Visible = false;

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

                label_CustomerName.Visible = true;
                textBox_CustomerName.Visible = true;

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
                button_PrintNewData.Visible = true;
                


                button_SaveDetails.Visible = true;

                // Show DataGridView
                dataGridView_Lines.Visible = true;

                Queries_NHC queries_NHC = new Queries_NHC();
                var data = queries_NHC.RetrieveSignatory_DR();

                string GetSafe(string[] arr, int index)
                {
                    return arr != null && index < arr.Length ? arr[index] : "";
                }

                textBox_Address.Text = GetSafe(data, 0);
                textBox_Terms.Text = GetSafe(data, 1);
                textBox_StoreCode.Text = GetSafe(data, 2);
                textBox_PONumber.Text = GetSafe(data, 3);
                textBox_TIN.Text = GetSafe(data, 4);
                textBox_CustomerName.Text = GetSafe(data, 5);
            }
            else
            {
                checkBox_EnableExpDate.Visible = false;
                checkBox_LessEWT.Visible = false;
                radioButton_VATInclusive.Visible = false;
                radioButton_VATExclusive.Visible = false;

                label_CustomerName.Visible = false;
                textBox_CustomerName.Visible = false;
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
