using Custom_QBSI.Clients.NHC;
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
            Dashboard_NHC dashboard_NHC = new Dashboard_NHC();

            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                //BackColor = Color.Aqua,
            };

            Panel panel_Main = dashboard_NHC.MainPanel();
            Panel panel_Sidebar = dashboard_NHC.SidebarPanel();
            Panel panel_Title = dashboard_NHC.TitlePanel();

            panel.Controls.Add(panel_Sidebar);
            panel.Controls.Add(panel_Title);
            panel.Controls.Add(panel_Main);

            return panel;
        }
    }
}
