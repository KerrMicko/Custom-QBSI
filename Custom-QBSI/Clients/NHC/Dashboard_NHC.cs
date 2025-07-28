using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Custom_QBSI.Clients.NHC
{


    public class Dashboard_NHC
    {
        private PrintDocument printDocument;
        private PrintPreviewControl printPreviewControl;

        private ComboBox comboBox_Forms;
        private FlowLayoutPanel panel_Printing;

        static int sideBarWidth = 250;
        public Panel MainPanel()
        {
            Panel panel_Main = new Panel
            {
                BackColor = Color.LightGray,
                Dock = DockStyle.Fill,
                Padding = new Padding(sideBarWidth, 50, 0, 0),
                //Height = 300,
            };

            printPreviewControl = new PrintPreviewControl();
            printPreviewControl.Dock = DockStyle.Fill;
            printPreviewControl.Zoom = 1;
            printPreviewControl.Visible = false;

            panel_Main.Controls.Add(printPreviewControl);

            return panel_Main;
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
            FlowLayoutPanel panel_Prionting = Panel_Printing();

            // ------------------------------------------
            panel_SideBar.Controls.Add(panel_Forms);
            panel_SideBar.Controls.Add(panel_RefNumber);
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
            button_SearchRefNum.Click += (sender, e) =>
            {
                if (comboBox_Forms.SelectedIndex == 0)
                {
                    MessageBox.Show("Please select a form.", "Notice", MessageBoxButtons.OK);
                }
                else if (comboBox_Forms.SelectedIndex != 0 && textBox_ReferenceNumber.Text != "")
                {
                    string refNumber = textBox_ReferenceNumber.Text;

                    Layout_NHC layout_NHC = new Layout_NHC();
                    PaperSize paperSize = new PaperSize("Custom", 850, 1100);

                    printDocument = new PrintDocument();
                    printDocument.DefaultPageSettings.PaperSize = paperSize;
                    printDocument.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
                    printDocument.PrintPage += (s, ev) =>
                    {
                        layout_NHC.PrintPage_NHC(s, ev, 1);
                    };
                    printPreviewControl.Document = printDocument;
                    printPreviewControl.Visible = true;
                    panel_Printing.Visible = true;
                }
                else
                {
                    MessageBox.Show("Please enter a reference number.", "Notice", MessageBoxButtons.OK);
                }
            };

            return panel_RefNumber;
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
    }
}
