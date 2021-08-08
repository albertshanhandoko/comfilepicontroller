using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerPage
{
    public partial class FormIPSet : Form
    {

        public static string combobox_selectedItem_ipsetting;

        public FormIPSet()
        {
            InitializeComponent();
            Combobox_ipsetting.Items.Clear();
            for (int i = 1; i <= 10; i++)
            {
                Combobox_ipsetting.Items.Add(i.ToString());
            }

        }

        public decimal IPselection { get; set; }

        private void Combobox_ipsetting_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.Intervalselection = numericUpDown1.Value;
            if (Combobox_ipsetting.SelectedIndex > -1)
            {
                combobox_selectedItem_ipsetting = Combobox_ipsetting.SelectedItem.ToString();
                var existing_ip = GetLocalIPAddress();
                var setting_ip = combobox_selectedItem_ipsetting;
                ProcessStartInfo procStartInfo = new ProcessStartInfo("/usr/bin/sudo", "/bin/sed -i 's/static ip_address=192.168.0." + existing_ip + "/static ip_address=192.168.0." + setting_ip + "/' /etc/dhcpcd.conf");
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                ProcessStartInfo procStartInfo4 = new ProcessStartInfo("/usr/bin/sudo", "/bin/sed -i 's/bind-address = 192.168.0." + existing_ip + "/bind-address = 192.168.0." + setting_ip + "/' /etc/mysql/mariadb.cnf");
                procStartInfo4.RedirectStandardOutput = true;
                procStartInfo4.UseShellExecute = false;
                procStartInfo4.CreateNoWindow = true;

                System.Diagnostics.Process proc4 = new System.Diagnostics.Process();
                proc4.StartInfo = procStartInfo4;
                proc4.Start();

                ProcessStartInfo procStartInfo5 = new ProcessStartInfo("/usr/bin/sudo", "/bin/sed -i 's/bind-address = 192.168.0." + existing_ip + "/bind-address = 192.168.0." + setting_ip + "/' /etc/mysql/my.cnf");
                procStartInfo5.RedirectStandardOutput = true;
                procStartInfo5.UseShellExecute = false;
                procStartInfo5.CreateNoWindow = true;

                System.Diagnostics.Process proc5 = new System.Diagnostics.Process();
                proc5.StartInfo = procStartInfo5;
                proc5.Start();

                ProcessStartInfo procStartInfo2 = new ProcessStartInfo("/usr/bin/sudo", "service dhcpcd restart");
                procStartInfo2.RedirectStandardOutput = true;
                procStartInfo2.UseShellExecute = false;
                procStartInfo2.CreateNoWindow = true;

                System.Diagnostics.Process proc2 = new System.Diagnostics.Process();
                proc2.StartInfo = procStartInfo2;
                proc2.Start();

                ProcessStartInfo procStartInfo3 = new ProcessStartInfo("/usr/bin/sudo", "reboot");
                procStartInfo3.RedirectStandardOutput = true;
                procStartInfo3.UseShellExecute = false;
                procStartInfo3.CreateNoWindow = true;

                System.Diagnostics.Process proc3 = new System.Diagnostics.Process();
                proc3.StartInfo = procStartInfo3;
                proc3.Start();
            }
            else
            {
                combobox_selectedItem_ipsetting = string.Empty;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();

        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString().Substring(ip.ToString().Length - 2).Replace(".", string.Empty);
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}
