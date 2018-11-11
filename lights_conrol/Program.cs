using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lights_control
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);        
            Application.Run(new LightControlContext());
        }
    }


    public class LightControlContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        private static readonly HttpClient client = new HttpClient();

        public LightControlContext()
        {
            ContextMenuStrip cms = new ContextMenuStrip();

            cms.Items.Add(new ToolStripMenuItem("Включить", null, LightsOn));
            cms.Items.Add(new ToolStripMenuItem("Выключить", null, LightsOff));
            cms.Items.Add(new ToolStripMenuItem("Выход", null, Exit));

            trayIcon = new NotifyIcon()
            {
                Text = "Управление светом",
                Icon = Properties.Resources.icon,
                ContextMenuStrip = cms,
                
                Visible = true
            };

            trayIcon.MouseUp += new MouseEventHandler(trayIcon_MouseUp);
        }

        private void trayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(trayIcon, null);
            }
        }

        static async void LightsOn(object sender, EventArgs e)
        {
           await client.PostAsync("https://maker.ifttt.com/trigger/lightson/with/key/d_O1tk6kMsBVHq-YC9biuL", null);
        }

        static async void LightsOff(object sender, EventArgs e)
        {          
            await client.PostAsync("https://maker.ifttt.com/trigger/lightsoff/with/key/d_O1tk6kMsBVHq-YC9biuL", null);
        }

        void Exit(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Выйти из программы и лишиться возможности контролировать освещение?", "Выход",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
                Application.Exit();
            }

            else if (result == DialogResult.No)
            {
                // none
            }
        }
    }
}
