using ControllerPage.Helper;
using ControllerPage.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ControllerPage
{
    public partial class FormBIAS : Form
    {
        double Paddy_Value;
        double brownrice_value;
        double wheat_value;
        double barley_value;
        double soy_value;
        double corn_value;
        double polishedrice_value;


        bool _runPaddy;
        public FormBIAS()
        {
            InitializeComponent();
            DataInitialization_Bias();
        }


        private void DataInitialization_Bias()
        {
            List<SQL_Data_Config> current_config = Sensor_input_Helper.MySql_Get_DataConfig(Sensor_input_Helper.GetLocalIPAddress());
            var Paddy_var = current_config.Where(config => config.Config_Param == "paddy");
            double Paddy_value = (Paddy_var.Select(p => p.Config_Value).ToArray()).First();

            var Brown_var = current_config.Where(config => config.Config_Param == "brown_rice");
            double Brown_value = (Brown_var.Select(p => p.Config_Value).ToArray()).First();

            var Wheat_var = current_config.Where(config => config.Config_Param == "wheat");
            double Wheat_value = (Wheat_var.Select(p => p.Config_Value).ToArray()).First();

            var Barley_var = current_config.Where(config => config.Config_Param == "barley");
            double Barley_value = (Barley_var.Select(p => p.Config_Value).ToArray()).First();

            var Soy_var = current_config.Where(config => config.Config_Param == "soy");
            double Soy_value = (Soy_var.Select(p => p.Config_Value).ToArray()).First();

            var Corn_var = current_config.Where(config => config.Config_Param == "corn");
            double Corn_value = (Corn_var.Select(p => p.Config_Value).ToArray()).First();

            var Polished_var = current_config.Where(config => config.Config_Param == "polished_rice");
            double Polished_value = (Polished_var.Select(p => p.Config_Value).ToArray()).First();

            textBox_Bias_Paddy.Text = Paddy_value.ToString();
            textBox_bias_Brownrice.Text = Brown_value.ToString();
            textBox_bias_wheat.Text = Wheat_value.ToString();
            textBox_bias_barley.Text = Barley_value.ToString();
            textBox_bias_soy.Text = Soy_value.ToString();
            textBox_bias_corn.Text = Corn_value.ToString();
            textBox_bias_Polished_Rice.Text = Polished_value.ToString();

            //button_plus_bias_paddy.Hol

        }



        private void button_Bias_Apply_click(object sender, EventArgs e)
        {

            //minimum is -15.0 %, maximum is 15.0 %.increment of 0.1 %
            if (double.Parse(textBox_Bias_Paddy.Text) < -15 || double.Parse(textBox_Bias_Paddy.Text) > 15)
            {
                MessageBox.Show("Paddy Range is -15% to 15%");
            }
            else
            {
                //double sql_value_paddy = double.Parse(textBox_Bias_Paddy.Text);
                Sensor_input_Helper.Update_DataConfig(Sensor_input_Helper.GetLocalIPAddress(), "paddy", textBox_Bias_Paddy.Text);
                Sensor_input_Helper.Update_DataConfig(Sensor_input_Helper.GetLocalIPAddress(), "brown_rice", textBox_bias_Brownrice.Text);
                Sensor_input_Helper.Update_DataConfig(Sensor_input_Helper.GetLocalIPAddress(), "wheat", textBox_bias_wheat.Text);
                Sensor_input_Helper.Update_DataConfig(Sensor_input_Helper.GetLocalIPAddress(), "corn", textBox_bias_corn.Text);
                Sensor_input_Helper.Update_DataConfig(Sensor_input_Helper.GetLocalIPAddress(), "soy", textBox_bias_soy.Text);
                Sensor_input_Helper.Update_DataConfig(Sensor_input_Helper.GetLocalIPAddress(), "barley", textBox_bias_barley.Text);
                Sensor_input_Helper.Update_DataConfig(Sensor_input_Helper.GetLocalIPAddress(), "polished_rice", textBox_bias_Polished_Rice.Text);
                


                this.Close();
            }
        }




        private void FormBIAS_Load(object sender, EventArgs e)
        {

        }

        #region Paddy
        private bool _runpaddy = false;

        private void button_plus_bias_paddy_MouseDown(object sender, MouseEventArgs e)
        {
            _runpaddy = true;
            Thread readThread = new Thread(Paddy_Plus_Async);
            readThread.Start();
        }
        private void button_plus_bias_paddy_MouseUp(object sender, MouseEventArgs e)
        {
            _runpaddy = false;
        }
        public void Paddy_Plus_Async()
        {
            //DateTime Start = DateTime.Now();
            int counter_paddy = 0;
            while (_runpaddy)
            {
                counter_paddy++;
                if (counter_paddy <= 6)
                {
                    Thread.Sleep(300);
                    textBox_Bias_Paddy.Invoke((Action)delegate
                    {
                        textBox_Bias_Paddy.Text = (double.Parse(textBox_Bias_Paddy.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_paddy > 5 & counter_paddy <= 10)
                {
                    Thread.Sleep(200);
                    textBox_Bias_Paddy.Invoke((Action)delegate
                    {
                        textBox_Bias_Paddy.Text = (double.Parse(textBox_Bias_Paddy.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_paddy > 10)
                {
                    Thread.Sleep(50);
                    textBox_Bias_Paddy.Invoke((Action)delegate
                    {
                        textBox_Bias_Paddy.Text = (double.Parse(textBox_Bias_Paddy.Text) + 0.1).ToString("0.0");
                    });
                }


                //You actions
            }
        }
        private void button_minus_bias_paddy_MouseDown(object sender, MouseEventArgs e)
        {
            _runpaddy = true;
            Thread readThread = new Thread(Paddy_Minus_Async);
            readThread.Start();
        }
        private void button_minus_bias_paddy_MouseUp(object sender, MouseEventArgs e)
        {
            _runpaddy = false;
        }
        public void Paddy_Minus_Async()
        {
            int counter_paddy = 0;
            while (_runpaddy)
            {
                counter_paddy++;
                if (counter_paddy <= 6)
                {
                    Thread.Sleep(300);
                    textBox_Bias_Paddy.Invoke((Action)delegate
                    {
                        textBox_Bias_Paddy.Text = (double.Parse(textBox_Bias_Paddy.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_paddy > 5 & counter_paddy <= 10)
                {
                    Thread.Sleep(200);
                    textBox_Bias_Paddy.Invoke((Action)delegate
                    {
                        textBox_Bias_Paddy.Text = (double.Parse(textBox_Bias_Paddy.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_paddy > 10)
                {
                    Thread.Sleep(50);
                    textBox_Bias_Paddy.Invoke((Action)delegate
                    {
                        textBox_Bias_Paddy.Text = (double.Parse(textBox_Bias_Paddy.Text) - 0.1).ToString("0.0");
                    });
                }


                //You actions
            }
        }


        #endregion

        #region BrownRice
        private bool _runBrown = false;
        private void btn_plus_brown_MouseDown(object sender, MouseEventArgs e)
        {
            _runBrown = true;
            Thread readThread = new Thread(Brown_Plus_Async);
            readThread.Start();
        }

        private void btn_plus_brown_MouseUp(object sender, MouseEventArgs e)
        {
            _runBrown = false;
        }
        public void Brown_Plus_Async()
        {
            int counter_Brown = 0;
            while (_runBrown)
            {
                counter_Brown++;
                if (counter_Brown <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_Brownrice.Invoke((Action)delegate
                    {
                        textBox_bias_Brownrice.Text = (double.Parse(textBox_bias_Brownrice.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_Brown > 5 & counter_Brown <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_Brownrice.Invoke((Action)delegate
                    {
                        textBox_bias_Brownrice.Text = (double.Parse(textBox_bias_Brownrice.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_Brown > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_Brownrice.Invoke((Action)delegate
                    {
                        textBox_bias_Brownrice.Text = (double.Parse(textBox_bias_Brownrice.Text) + 0.1).ToString("0.0");
                    });
                }


                //You actions
            }
        }


        private void btn_minus_brown_MouseDown(object sender, MouseEventArgs e)
        {
            _runBrown = true;
            Thread readThread = new Thread(Brown_Minus_Async);
            readThread.Start();
        }

        private void btn_minus_brown_MouseUp(object sender, MouseEventArgs e)
        {
            _runBrown = false;
        }
        public void Brown_Minus_Async()
        {
            int counter_Brown = 0;
            while (_runBrown)
            {
                counter_Brown++;
                if (counter_Brown <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_Brownrice.Invoke((Action)delegate
                    {
                        textBox_bias_Brownrice.Text = (double.Parse(textBox_bias_Brownrice.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_Brown > 5 & counter_Brown <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_Brownrice.Invoke((Action)delegate
                    {
                        textBox_bias_Brownrice.Text = (double.Parse(textBox_bias_Brownrice.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_Brown > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_Brownrice.Invoke((Action)delegate
                    {
                        textBox_bias_Brownrice.Text = (double.Parse(textBox_bias_Brownrice.Text) - 0.1).ToString("0.0");
                    });
                }
            }
        }


        #endregion

        #region Wheat
        private bool _runwheat = false;
        private void btn_plus_wheat_MouseDown(object sender, MouseEventArgs e)
        {
            _runwheat = true;
            Thread readThread = new Thread(wheat_Plus_Async);
            readThread.Start();
        }

        private void btn_plus_wheat_MouseUp(object sender, MouseEventArgs e)
        {
            _runwheat = false;
        }
        public void wheat_Plus_Async()
        {
            int counter_wheat = 0;
            while (_runwheat)
            {
                counter_wheat++;
                if (counter_wheat <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_wheat.Invoke((Action)delegate
                    {
                        textBox_bias_wheat.Text = (double.Parse(textBox_bias_wheat.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_wheat > 5 & counter_wheat <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_wheat.Invoke((Action)delegate
                    {
                        textBox_bias_wheat.Text = (double.Parse(textBox_bias_wheat.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_wheat > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_wheat.Invoke((Action)delegate
                    {
                        textBox_bias_wheat.Text = (double.Parse(textBox_bias_wheat.Text) + 0.1).ToString("0.0");
                    });
                }
            }
        }

        private void btn_minus_wheat_MouseDown(object sender, MouseEventArgs e)
        {
            _runwheat = true;
            Thread readThread = new Thread(wheat_Minus_Async);
            readThread.Start();
        }

        private void btn_minus_wheat_MouseUp(object sender, MouseEventArgs e)
        {
            _runwheat = false;
        }
        public void wheat_Minus_Async()
        {
            int counter_wheat = 0;
            while (_runwheat)
            {
                counter_wheat++;
                if (counter_wheat <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_wheat.Invoke((Action)delegate
                    {
                        textBox_bias_wheat.Text = (double.Parse(textBox_bias_wheat.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_wheat > 5 & counter_wheat <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_wheat.Invoke((Action)delegate
                    {
                        textBox_bias_wheat.Text = (double.Parse(textBox_bias_wheat.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_wheat > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_wheat.Invoke((Action)delegate
                    {
                        textBox_bias_wheat.Text = (double.Parse(textBox_bias_wheat.Text) - 0.1).ToString("0.0");
                    });
                }
            }
        }


        #endregion

        #region barley
        private bool _runbarley = false;
        private void btn_plus_barley_MouseDown(object sender, MouseEventArgs e)
        {
            _runbarley = true;
            Thread readThread = new Thread(barley_Plus_Async);
            readThread.Start();
        }

        private void btn_plus_barley_MouseUp(object sender, MouseEventArgs e)
        {
            _runbarley = false;
        }

        public void barley_Plus_Async()
        {
            int counter_barley = 0;
            while (_runbarley)
            {
                counter_barley++;
                if (counter_barley <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_barley.Invoke((Action)delegate
                    {
                        textBox_bias_barley.Text = (double.Parse(textBox_bias_barley.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_barley > 5 & counter_barley <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_barley.Invoke((Action)delegate
                    {
                        textBox_bias_barley.Text = (double.Parse(textBox_bias_barley.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_barley > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_barley.Invoke((Action)delegate
                    {
                        textBox_bias_barley.Text = (double.Parse(textBox_bias_barley.Text) + 0.1).ToString("0.0");
                    });
                }
            }
        }


        private void btn_minus_barley_MouseDown(object sender, MouseEventArgs e)
        {
            _runbarley = true;
            Thread readThread = new Thread(barley_Minus_Async);
            readThread.Start();
        }

        private void btn_minus_barley_MouseUp(object sender, MouseEventArgs e)
        {
            _runbarley = false;
        }

        public void barley_Minus_Async()
        {
            int counter_barley = 0;
            while (_runbarley)
            {
                counter_barley++;
                if (counter_barley <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_barley.Invoke((Action)delegate
                    {
                        textBox_bias_barley.Text = (double.Parse(textBox_bias_barley.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_barley > 5 & counter_barley <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_barley.Invoke((Action)delegate
                    {
                        textBox_bias_barley.Text = (double.Parse(textBox_bias_barley.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_barley > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_barley.Invoke((Action)delegate
                    {
                        textBox_bias_barley.Text = (double.Parse(textBox_bias_barley.Text) - 0.1).ToString("0.0");
                    });
                }
            }
        }



        #endregion

        #region Soy
        private bool _runsoy = false;
        private void btn_plus_soy_MouseDown(object sender, MouseEventArgs e)
        {
            _runsoy = true;
            Thread readThread = new Thread(soy_Plus_Async);
            readThread.Start();
        }

        private void btn_plus_soy_MouseUp(object sender, MouseEventArgs e)
        {
            _runsoy = false;
        }
        public void soy_Plus_Async()
        {
            int counter_soy = 0;
            while (_runsoy)
            {
                counter_soy++;
                if (counter_soy <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_soy.Invoke((Action)delegate
                    {
                        textBox_bias_soy.Text = (double.Parse(textBox_bias_soy.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_soy > 5 & counter_soy <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_soy.Invoke((Action)delegate
                    {
                        textBox_bias_soy.Text = (double.Parse(textBox_bias_soy.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_soy > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_soy.Invoke((Action)delegate
                    {
                        textBox_bias_soy.Text = (double.Parse(textBox_bias_soy.Text) + 0.1).ToString("0.0");
                    });
                }
            }
        }


        private void btn_minus_soy_MouseDown(object sender, MouseEventArgs e)
        {
            _runsoy = true;
            Thread readThread = new Thread(soy_Minus_Async);
            readThread.Start();
        }

        private void btn_minus_soy_MouseUp(object sender, MouseEventArgs e)
        {
            _runsoy = false;
        }


        public void soy_Minus_Async()
        {
            int counter_soy = 0;
            while (_runsoy)
            {
                counter_soy++;
                if (counter_soy <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_soy.Invoke((Action)delegate
                    {
                        textBox_bias_soy.Text = (double.Parse(textBox_bias_soy.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_soy > 5 & counter_soy <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_soy.Invoke((Action)delegate
                    {
                        textBox_bias_soy.Text = (double.Parse(textBox_bias_soy.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_soy > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_soy.Invoke((Action)delegate
                    {
                        textBox_bias_soy.Text = (double.Parse(textBox_bias_soy.Text) - 0.1).ToString("0.0");
                    });
                }
            }
        }


        #endregion

        #region polished
        private bool _runpolished = false;
        private void btn_plus_polished_MouseDown(object sender, MouseEventArgs e)
        {
            _runpolished = true;
            Thread readThread = new Thread(polished_Plus_Async);
            readThread.Start();
        }

        private void btn_plus_polished_MouseUp(object sender, MouseEventArgs e)
        {
            _runpolished = false;
        }

        public void polished_Plus_Async()
        {
            int counter_polished = 0;
            while (_runpolished)
            {
                counter_polished++;
                if (counter_polished <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_Polished_Rice.Invoke((Action)delegate
                    {
                        textBox_bias_Polished_Rice.Text = (double.Parse(textBox_bias_Polished_Rice.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_polished > 5 & counter_polished <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_Polished_Rice.Invoke((Action)delegate
                    {
                        textBox_bias_Polished_Rice.Text = (double.Parse(textBox_bias_Polished_Rice.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_polished > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_Polished_Rice.Invoke((Action)delegate
                    {
                        textBox_bias_Polished_Rice.Text = (double.Parse(textBox_bias_Polished_Rice.Text) + 0.1).ToString("0.0");
                    });
                }
            }
        }

        private void btn_minus_polished_MouseDown(object sender, MouseEventArgs e)
        {
            _runpolished = true;
            Thread readThread = new Thread(polished_Minus_Async);
            readThread.Start();
        }

        private void btn_minus_polished_MouseUp(object sender, MouseEventArgs e)
        {
            _runpolished = false;
        }

        public void polished_Minus_Async()
        {
            int counter_polished = 0;
            while (_runpolished)
            {
                counter_polished++;
                if (counter_polished <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_Polished_Rice.Invoke((Action)delegate
                    {
                        textBox_bias_Polished_Rice.Text = (double.Parse(textBox_bias_Polished_Rice.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_polished > 5 & counter_polished <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_Polished_Rice.Invoke((Action)delegate
                    {
                        textBox_bias_Polished_Rice.Text = (double.Parse(textBox_bias_Polished_Rice.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_polished > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_Polished_Rice.Invoke((Action)delegate
                    {
                        textBox_bias_Polished_Rice.Text = (double.Parse(textBox_bias_Polished_Rice.Text) - 0.1).ToString("0.0");
                    });
                }
            }
        }

        #endregion

        #region Corn
        private bool _runcorn = false;
        private void btn_plus_corn_MouseDown(object sender, MouseEventArgs e)
        {
            _runcorn = true;
            Thread readThread = new Thread(corn_Plus_Async);
            readThread.Start();
        }

        private void btn_plus_corn_MouseUp(object sender, MouseEventArgs e)
        {
            _runcorn = false;
        }


        public void corn_Plus_Async()
        {
            int counter_corn = 0;
            while (_runcorn)
            {
                counter_corn++;
                if (counter_corn <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_corn.Invoke((Action)delegate
                    {
                        textBox_bias_corn.Text = (double.Parse(textBox_bias_corn.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_corn > 5 & counter_corn <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_corn.Invoke((Action)delegate
                    {
                        textBox_bias_corn.Text = (double.Parse(textBox_bias_corn.Text) + 0.1).ToString("0.0");
                    });
                }

                else if (counter_corn > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_corn.Invoke((Action)delegate
                    {
                        textBox_bias_corn.Text = (double.Parse(textBox_bias_corn.Text) + 0.1).ToString("0.0");
                    });
                }
            }
        }

        private void btn_minus_corn_MouseDown(object sender, MouseEventArgs e)
        {
            _runcorn = true;
            Thread readThread = new Thread(corn_Minus_Async);
            readThread.Start();
        }

        private void btn_minus_corn_MouseUp(object sender, MouseEventArgs e)
        {
            _runcorn = false;
        }


        public void corn_Minus_Async()
        {
            int counter_corn = 0;
            while (_runcorn)
            {
                counter_corn++;
                if (counter_corn <= 6)
                {
                    Thread.Sleep(300);
                    textBox_bias_corn.Invoke((Action)delegate
                    {
                        textBox_bias_corn.Text = (double.Parse(textBox_bias_corn.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_corn > 5 & counter_corn <= 10)
                {
                    Thread.Sleep(200);
                    textBox_bias_corn.Invoke((Action)delegate
                    {
                        textBox_bias_corn.Text = (double.Parse(textBox_bias_corn.Text) - 0.1).ToString("0.0");
                    });
                }

                else if (counter_corn > 10)
                {
                    Thread.Sleep(50);
                    textBox_bias_corn.Invoke((Action)delegate
                    {
                        textBox_bias_corn.Text = (double.Parse(textBox_bias_corn.Text) - 0.1).ToString("0.0");
                    });
                }
            }
        }



        #endregion



    }
}
