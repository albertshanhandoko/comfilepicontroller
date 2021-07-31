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
    public partial class FormOptions : Form
    {
        public FormOptions()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var form = new FormLanguage())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //button_thereshold_maxvalue.Text = FormNumpad_max.numpad_max;
                    Console.WriteLine("test");
                }
            }
            this.Show();
            //FormLanguage Form_language = new FormLanguage();
            //Form_language.Show();
            //Application.Run(Form_language);
            //Form_language.ShowDialog();
            //this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var form = new FormThreshold())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //button_thereshold_maxvalue.Text = FormNumpad_max.numpad_max;
                    Console.WriteLine("test");
                }
            }
            this.Show();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            //using (var form = new FormBIASPassword())
            using (var form = new FormNumpadBPassword())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    //button_thereshold_maxvalue.Text = FormNumpad_max.numpad_max;
                    Console.WriteLine("test");
                }
            }
            this.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
