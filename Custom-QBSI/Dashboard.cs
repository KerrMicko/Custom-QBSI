using Custom_QBSI.Clients.Enclosure;
using Custom_QBSI.Clients.NHC;
using Custom_QBSI.Clients.PBS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Custom_QBSI
{
    public class GlobalVariables
    {
        /* 
         * Current clients:
         * NHC
         * Enclosure 
         * PBS
         */
        public static string client = "NHC";
    }
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            this.Text = "QBSI " + GlobalVariables.client;

            Panel panel = ContainerPanel();
            this.Controls.Add(panel);
        }

        private Panel ContainerPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                //BackColor = Color.Aqua,
            };

            if (GlobalVariables.client == "NHC")
            {
                Dashboard_NHC dashboard_NHC = new Dashboard_NHC();
                Panel panel_Main = dashboard_NHC.MainPanel();
                Panel panel_Sidebar = dashboard_NHC.SidebarPanel();
                Panel panel_Title = dashboard_NHC.TitlePanel();

                panel.Controls.Add(panel_Sidebar);
                panel.Controls.Add(panel_Title);
                panel.Controls.Add(panel_Main);
            }
            else if (GlobalVariables.client == "Enclosure")
            {
                Dashboard_Enclosure dashboard_Enclosure = new Dashboard_Enclosure();
                Panel panel_Main = dashboard_Enclosure.MainPanel();
                Panel panel_Sidebar = dashboard_Enclosure.SidebarPanel();
                Panel panel_Title = dashboard_Enclosure.TitlePanel();

                panel.Controls.Add(panel_Sidebar);
                panel.Controls.Add(panel_Title);
                panel.Controls.Add(panel_Main);
            }
            else if (GlobalVariables.client == "PBS")
            {
                Dashboard_PBS dashboard_PBS = new Dashboard_PBS();
                Panel panel_Main = dashboard_PBS.MainPanel();
                Panel panel_Sidebar = dashboard_PBS.SidebarPanel();
                Panel panel_Title = dashboard_PBS.TitlePanel();

                panel.Controls.Add(panel_Sidebar);
                panel.Controls.Add(panel_Title);
                panel.Controls.Add(panel_Main);
            }
            else
            {
                // Add logic for other clients if needed
                throw new NotImplementedException("Client not implemented: " + GlobalVariables.client);
            }


            return panel;
        }
    }
}
