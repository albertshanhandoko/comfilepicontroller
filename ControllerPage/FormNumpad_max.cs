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
    public partial class FormNumpad_max : Form
    {
        FormThreshold FormThereshold_Temp = new FormThreshold();
        //string textBox_numpad_window;
        public FormNumpad_max()
        {
            InitializeComponent();
            Datainitialization_Numpad();
            
        }
        public static string numpad_max;
        private void Datainitialization_Numpad()
        {
            //string textBox_numpad_window = FormThereshold_Temp.textBox_thereshold_temp.Text;


        }
        private void button12_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_del_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text.Remove(textBox_numpad_window_max.Text.Length - 1, 1);
        }
        #region inputnumpad
        private void button_1_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "1";

        }

        private void button_2_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "2";

        }

        private void button_3_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "3";

        }

        private void button_4_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "4";

        }

        private void button_5_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "5";

        }

        private void button_6_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "6";

        }

        private void button_7_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "7";

        }

        private void button_8_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "8";

        }

        private void button_9_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "9";

        }

        private void button_0_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + "0";

        }

        private void button_dot_Click(object sender, EventArgs e)
        {
            textBox_numpad_window_max.Text = textBox_numpad_window_max.Text + ".";

        }

        #endregion
        private void button_ENT_Click(object sender, EventArgs e)
        {
            //FormThereshold_Temp.button_thereshold_maxvalue.Text = textBox_numpad_window_max.Text;
            //this.Close();
            FormThereshold_Temp.button_thereshold_maxvalue.Text = textBox_numpad_window_max.Text;
            numpad_max = textBox_numpad_window_max.Text;
            this.DialogResult = DialogResult.OK;

            this.Close();

        }

        private void FormNumpad_max_Load(object sender, EventArgs e)
        {

        }
    }
}
