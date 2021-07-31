using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerPage.Constant;
using ControllerPage.Helper;
using ControllerPage.Library;

namespace ControllerPage
{
    public partial class FormFixedTime : Form
    {
        public FormFixedTime()
        {
            InitializeComponent();

            Combobox_timeinterval.Items.Clear();
            List<string> List_TimeInter;
            //Form1 Form_Main_Window = new Form1();

            List_TimeInter = Sensor_input_Helper.Get_List_Running_Time();
            foreach (string TimeInter in List_TimeInter)
            {
                Combobox_timeinterval.Items.Add(TimeInter);
            }

        }
        public static string combobox_selectedItem_WaitingTime;

        public decimal WaitingIntervalselection { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            //this.WaitingIntervalselection = numericUpDown2.Value;
            if (Combobox_timeinterval.SelectedIndex > -1)
            {
                combobox_selectedItem_WaitingTime = Combobox_timeinterval.SelectedItem.ToString();
            }
            else
            {
                combobox_selectedItem_WaitingTime = string.Empty;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void Combobox_NumPerPCS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
