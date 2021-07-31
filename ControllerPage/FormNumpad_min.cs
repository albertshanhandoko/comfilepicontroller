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
    public partial class FormNumpad_min : Form
    {
        FormThreshold FormThereshold_Temp = new FormThreshold();
        //string textBox_numpad_window;
        public FormNumpad_min()
        {
            InitializeComponent();
            Datainitialization_Numpad();
            
        }
        public static string numpad_min;

        private void Datainitialization_Numpad()
        {
            //string textBox_numpad_window = FormThereshold_Temp.textBox_thereshold_temp.Text;


        }

        #region nputnumpad 
        private void button12_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_del_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text.Remove(textBox_numpad_window_min.Text.Length - 1, 1);
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "1";

        }

        private void button_2_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "2";

        }

        private void button_3_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "3";

        }

        private void button_4_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "4";

        }

        private void button_5_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "5";

        }

        private void button_6_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "6";

        }

        private void button_7_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "7";

        }

        private void button_8_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "8";

        }

        private void button_9_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "9";

        }

        private void button_0_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + "0";

        }

        private void button_dot_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_min.Text = textBox_numpad_window_min.Text + ".";

        }
#endregion

        private void button_ENT_Click(object sender, EventArgs e)
        {
            FormThereshold_Temp.button_thereshold_minvalue.Text = textBox_numpad_window_min.Text;
            numpad_min = textBox_numpad_window_min.Text;
            this.DialogResult = DialogResult.OK;

            this.Close();
            //this.Close();
        }
    }
}
