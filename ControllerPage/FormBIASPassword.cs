using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControllerPage
{
    public partial class FormBIASPassword : Form
    {
        static string application_name = "GX_MX_001_Controller";
        public FormBIASPassword()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            //string password = "010101";
            string password = "090807";
            string passwordinput =
                numericUpDown1.Value.ToString() + numericUpDown2.Value.ToString() + numericUpDown3.Value.ToString() +
                                numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString() + numericUpDown6.Value.ToString();

            if (password == passwordinput)
            {
                this.Hide();
                FormBIAS Formbias_window = new FormBIAS();
                Formbias_window.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Wrong Password, BIAS not updated", application_name);
                this.Close();
            }

        }
    }
}
