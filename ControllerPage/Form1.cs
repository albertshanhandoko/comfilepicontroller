using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerPage.Constant;
using ControllerPage.Helper;
using ControllerPage.Library;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.Timers;
using System.IO;
using System.Security.Permissions;
using System.Numerics;
using System.Globalization;
using System.Net.Sockets;

namespace ControllerPage
{
    public partial class Form1 : Form
    {
        SerialPort mySerialPort;
        static string application_name = "GX_MX_001_Controller";

        // Parameter Input
        int delay;
        int TotalInterval;
        int running_time_fixed;
        TimeSpan Time_Dif;
        string ResultGrain;
        string ResultMeasure;
        bool temp_cond;
        bool thereshold_param;
        double therehold_max;
        double thereshold_min;
        List<String> List_Error_code = new List<string> { };
        //System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();
        System.Timers.Timer MyTimer = new System.Timers.Timer();
        System.Timers.Timer Timer_5min_StopCheck = new System.Timers.Timer();
        System.Timers.Timer Timer_5min_StopRunning = new System.Timers.Timer();

        public Thread check_thread;
        public Thread checktemp_thread;
        public Thread start_thread;
        public Thread stop_5min_thread;

        bool bool_check_error = false;
        bool bool_stop_click = false;
        int blink_timer;

        // Parameter Looping Sensor
        double bias_value;
        int current_interval_reset;
        int current_interval;
        int counter_data = 0;
        int counter_data_reset = 0;
        bool start_next_cond;
        bool aggregate_cond;
        bool stat_continue;
        bool fixed_time_timer_stop;
        List<data_measure_2> Data_Measure_Result = new List<data_measure_2> { };
        List<data_measure_2> Data_Avg_Result = new List<data_measure_2> { };
        
        data_measure_2 Data_Measure_Current;
        data_measure_2 Data_Avg_Current;
        int timer_counter = 0;
        float total_current_Average;
        float total_average;
        int finish_measurement = 0;
        DateTime FixedTime_start;
        DateTime FixedTime_Finish;
        DateTime FixedTime_Finish_timer;
        DateTime start_5min_check;
        DateTime start_5min_Running;

        //database parameter
        int batch_id;
        bool checkcommand;

        // other
        //Thread check_thread = new Thread(Check_Thread);



        public Form1()
        {
            InitializeComponent();
            data_initiation_input();
            ButtonIPSet.Text = GetLocalIPAddress();

        }

        #region Button_Other
        private void button1_Click(object sender, EventArgs e)
        {
            Sensor_input_Helper.Command_CheckData(mySerialPort);

        }
        private void button2_Click_3(object sender, EventArgs e)
        {
            this.Invalidate();
            this.Refresh();
        }
        private void button3_Click_2(object sender, EventArgs e)
        {
            this.Controls.Clear();// 'removes all the controls on the form
            InitializeComponent();// 'load all the controls again
            //Form1_Load(e, e);// 'Load everything in your form, load event again
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Sensor_input_Helper.Command_Write(mySerialPort, "10192\r");
        }
        private void button5_Click_1(object sender, EventArgs e)
        {
            Sensor_input_Helper.Command_Write(mySerialPort, "22094\r");
        }
        #endregion


        #region Button_Func
        private void button_Product_Click(object sender, EventArgs e)
        {
            using (var form = new FormProductselection())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string val = form.Productselection;            //values preserved after close
                    //Do something here with these values
                    string display_val = val.Replace("_", " ");
                    ButtonProduct.Text = display_val;

                }
            }
        }
        private void button_NumInterval_Click(object sender, EventArgs e)
        {
            using (var form = new FormNumberinterval())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ButtonNumInterval.Text = FormNumberinterval.combobox_selectedItem_number_Interval;
                }
            }

        }
        private void button_NumPerPcs_Click(object sender, EventArgs e)
        {
            using (var form = new FormNumberpcsinterval())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ButtonNumPcs.Text = FormNumberpcsinterval.combobox_selectedItem_number_PerPCS;
                }
            }
        }
        private void button_Time_Click(object sender, EventArgs e)
        {
            if (Combobox_Mode.SelectedItem.ToString().ToLower() == "fix time" && Combobox_Mode.SelectedIndex > -1)
            {

                using (var form = new FormFixedTime())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        ButtonWaitingTime.Text = FormFixedTime.combobox_selectedItem_WaitingTime;

                    }
                }

            }
            else
            {
                using (var form = new FormIntervalTime())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        ButtonWaitingTime.Text = FormIntervalTime.combobox_selectedItem_WaitingTime;

                    }
                }

            }


        }
        private void button_Option_Click(object sender, EventArgs e)
        {
            this.Hide();
            //Form2_old F2 = new Form2_old();
            FormOptions FormOption_open = new FormOptions();

            FormOption_open.ShowDialog();
            this.Show();
        }
        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            Sensor_input_Helper.Command_Stop(mySerialPort);
            bool_stop_click = true;
        }
        private void Btn_Check_Click(object sender, EventArgs e)
        {
            if (Combobox_ComPort.SelectedIndex > -1)
            {
                if (Combobox_ComPort.SelectedItem.ToString() == "RS-232")
                {
                    mySerialPort = new SerialPort("/dev/ttyAMA0"); //232
                }
                else if (Combobox_ComPort.SelectedItem.ToString() == "RS-485")
                {
                    mySerialPort = new SerialPort("/dev/ttyS0"); //485
                }
                else
                {
                    mySerialPort = new SerialPort(Combobox_ComPort.Text);
                }
            }
            else
            {
                mySerialPort = new SerialPort("/dev/ttyS0"); //485
            }
            mySerialPort.ReadTimeout = 60 * 1000 * 5;// in miliseconds
            //mySerialPort.ReadBufferSize = 2000000;
            //mySerialPort = new SerialPort("/dev/ttyAMA0"); //232
            if (mySerialPort.IsOpen)
            {
                mySerialPort.Close();
            }

            try
            {
                //Thread check_thread = new Thread(Check_Thread);
                check_thread = new Thread(Check_Thread);
                check_thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sensor Failed to Start");
                Console.WriteLine(ex.Message);
            }
        }
        private void button_start_Click(object sender, EventArgs e)
        {
            data_cleansing();
            bool check_start = Start_Validation();

            if (check_start)
            {

                Temp_TextBox.Text = "";

                string product_text = ButtonProduct.Text.Replace(" ", "_");
                List<SQL_Data_Config> current_config = Sensor_input_Helper.MySql_Get_DataConfig(Sensor_input_Helper.GetLocalIPAddress());
                var Product_var = current_config.Where(config => config.Config_Param == product_text.ToLower());
                double Product_value = (Product_var.Select(p => p.Config_Value).ToArray()).First();

                var TheresholdMax_var = current_config.Where(config => config.Config_Param == "Thereshold_Max");
                therehold_max = (TheresholdMax_var.Select(p => p.Config_Value).ToArray()).First();

                var TheresholdMin_var = current_config.Where(config => config.Config_Param == "Thereshold_Min");
                thereshold_min = (TheresholdMin_var.Select(p => p.Config_Value).ToArray()).First();

                var TheresholdEnable_var = current_config.Where(config => config.Config_Param == "Thereshold_Enable");
                double TheresholdEnable_value = (TheresholdEnable_var.Select(p => p.Config_Value).ToArray()).First();


                if (TheresholdEnable_value == 1)
                {
                    thereshold_param = true;

                }
                else
                {
                    thereshold_param = false;
                }


                //Thereshold_Max Thereshold_Min Thereshold_Enable

                bias_value = Product_value;

                if (Temp_TextBox.Text == "" || String.IsNullOrEmpty(Temp_TextBox.Text))
                {
                    Sensor_input_Helper.Command_CheckTemp(mySerialPort);
                    //string result_temp = "29";
                    string result_temp = CheckTemp();
                }
                else
                {
                    Console.WriteLine("textbox alread filled");
                }
                int jumlahpieces = 0;
                int number;
                if (Int32.TryParse(ButtonNumPcs.Text, out number))
                {
                    jumlahpieces = number;
                }

                Thread.Sleep(500);
                timer_counter = 1;

                batch_id = Sensor_input_Helper.MySql_Insert_Batch(Sensor_input_Helper.GetLocalIPAddress()
                    , ButtonProduct.Text
                    , TotalInterval
                    , delay.ToString()
                    , jumlahpieces
                    , Temp_TextBox.Text)
                    ;



                Console.WriteLine(ResultGrain);
                Console.WriteLine(ResultMeasure);
                Sensor_input_Helper.Command_Stop(mySerialPort);
                Thread.Sleep(2500);
                Console.WriteLine("Stop");
                Sensor_input_Helper.Command_Write(mySerialPort, ResultGrain);
                Thread.Sleep(1000);
                Sensor_input_Helper.Command_Write(mySerialPort, ResultMeasure);

                Console.WriteLine("Start Sequence");
                current_interval = 0;
                current_interval_reset = 0;
                Curr_Interval_TextBox.Text = (current_interval + 1).ToString();

                stat_continue = true;
                Btn_Start.Enabled = false;
                Btn_Stop.Enabled = true;
                Btn_CheckTemp.Enabled = false;
                Btn_Check.Enabled = false;

                Combobox_Mode.Enabled = false;
                Combobox_Mode.Enabled = false;
                textBox_Sensor_Status.Text = "Running";

                Thread readThread;
                if (Combobox_Mode.SelectedItem.ToString().ToLower() == "fix time")
                {
                    FixedTime_start = DateTime.Now;
                    fixed_time_timer_stop = true;
                    start_thread = new Thread(Read_FixedTime_Thread);
                    start_thread.Start();

                    //readThread = new Thread(Read_FixedTime);
                    //readThread.Start();
                }
                else if (Combobox_Mode.SelectedItem.ToString().ToLower() == "fix pieces")
                {
                    //readThread = new Thread(Read_FixedPieces);
                    //readThread.Start();

                    start_thread = new Thread(Read_FixedPieces_Thread);
                    start_thread.Start();

                }
                else if (Combobox_Mode.SelectedItem.ToString().ToLower() == "interval")
                {
                    //readThread = new Thread(Read_Interval);
                    //readThread.Start();
                    start_thread = new Thread(Read_Interval_Thread);
                    start_thread.Start();

                }
                else
                {
                    MessageBox.Show("Wrong Picked on mode");
                }
            }



        }
        private void button_CheckTemp_Click(object sender, EventArgs e)
        {
            Sensor_input_Helper.Command_CheckTemp(mySerialPort);
            string result_temp = CheckTemp();

        }

        #endregion

        #region initial Function
        private void Combobox_Mode_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (Combobox_Mode.SelectedIndex <= -1)
            {
                Console.WriteLine("initial condition");
                //buttonOK.Enabled = true;
            }
            else if (Combobox_Mode.SelectedItem.ToString().ToLower() == "fix time" && Combobox_Mode.SelectedIndex > -1)
            {

                Console.WriteLine("Time mode");
                ButtonProduct.Enabled = true;
                ButtonNumInterval.Enabled = false;
                ButtonNumInterval.Text = string.Empty;
                ButtonNumPcs.Enabled = false;
                ButtonNumPcs.Text = string.Empty;
                ButtonWaitingTime.Enabled = true;
                ButtonWaitingTime.Text = string.Empty;
                textBox9.Text = "Running Time";

            }
            else if (Combobox_Mode.SelectedItem.ToString().ToLower() == "fix pieces" && Combobox_Mode.SelectedIndex > -1)
            {

                Console.WriteLine("Time mode");
                ButtonProduct.Enabled = true;
                ButtonNumInterval.Enabled = false;
                ButtonNumInterval.Text = string.Empty;
                ButtonNumPcs.Enabled = true;
                //ButtonNumPcs.Text = string.Empty;
                ButtonWaitingTime.Enabled = false;
                ButtonWaitingTime.Text = string.Empty;

            }

            else
            {
                ButtonProduct.Enabled = true;
                ButtonNumInterval.Enabled = true;
                ButtonNumPcs.Enabled = true;
                ButtonWaitingTime.Enabled = true;
                ButtonWaitingTime.Text = string.Empty;
                textBox9.Text = "Int. Waiting Time";

                Console.WriteLine("Pieces mode");

            }
        }
        private void data_cleansing()
        {
            // Parameter Di Layar
            /*
            ButtonProduct.Text = "";
            ButtonNumInterval.Text = "";
            ButtonNumPcs.Text = "";
            ButtonWaitingTime.Text = "";
            */
            Curr_Interval_TextBox.Text = "";
            Curr_Kernel_TextBox.Text = "";
            Current_Avg_TextBox.Text = "";
            Curr_Measure_TextBox.Text = "";
            Temp_TextBox.Text = "";

            // Parameter Input
            delay = 0;
            TotalInterval = 0;
            counter_data = 0;
            counter_data_reset = 0;
            ResultGrain = null;
            ResultMeasure = null;
            temp_cond = false;
            //System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();
            System.Timers.Timer MyTimer = new System.Timers.Timer();
            thereshold_param = false;
            blink_timer = 0;

            // Parameter Looping Sensor
            current_interval = 0;
            current_interval_reset = 0;
            start_next_cond = false;
            aggregate_cond = false;
            stat_continue = false;
            Data_Measure_Result = new List<data_measure_2> { };
            Data_Avg_Result = new List<data_measure_2> { };
            Data_Measure_Current = null;
            Data_Avg_Current = null;
            timer_counter = 0;
            total_current_Average = 0;
            total_average = 0;
            finish_measurement = 0;
            fixed_time_timer_stop = false;
            bool_check_error = false;
            bool_stop_click = false;
            //database parameter

            // Button input


        }
        private void data_initiation_input()
        {

            //comboBox_IPAddress.Items.Clear();
            //comboBox_IPAddress.Items.Add(Sensor_input_Helper.GetLocalIPAddress());


            // Comport
            Combobox_ComPort.Items.Clear();
            
            //Combobox_ComPort.Items.Add("COM2");
            Combobox_ComPort.Items.Add("RS-232");
            Combobox_ComPort.Items.Add("RS-485");

            // Controller Mode
            Combobox_Mode.Items.Clear();
            Combobox_Mode.Items.Add("Interval");
            Combobox_Mode.Items.Add("Fix Time");
            Combobox_Mode.Items.Add("Fix Pieces");
            
            // at the start button cannot be clicked
            ButtonProduct.Enabled = false;
            ButtonNumPcs.Enabled = false;
            ButtonNumInterval.Enabled = false;
            ButtonWaitingTime.Enabled = false;
            // diaktifan ketika ganti combobox_moe

            Btn_Start.Enabled = false;
            Btn_Stop.Enabled = false;
            Btn_CheckTemp.Enabled = false;
            // diaktifan ketika click check

            //online and no online
            textBox_Sensor_Status.Text = "Offline";

            // TImer
            MyTimer.Elapsed += new ElapsedEventHandler(MyTimer_Tick);
            MyTimer.Interval = (2000);

            Timer_5min_StopCheck.Elapsed += new ElapsedEventHandler(MyTimer_CheckStop_Tick);
            Timer_5min_StopCheck.Interval = (60000); // testing
            List_Error_code = Sensor_input_Helper.get_List_Error_Code();
            
            textBox_sensornumber.Text = "SENSOR " + (Sensor_input_Helper.GetLocalIPAddress()).Last().ToString();



            //
            string str1 = "1234";
            char[] array1 = str1.ToCharArray();
            string final1 = "";
            foreach (var i in array1)
            {
                string hex1 = String.Format("{0:X}", Convert.ToInt32(i));
                final1 += hex1.Insert(0, "0X") + " ";
            }
            final1 = final1.TrimEnd();
            Console.WriteLine(final1);

            //
            //string checksum = Measure.Substring(5, 2);
            //string test_measure = "125";
            //string checksum = "98";

            string test_measure = "129";
            string checksum = "98";


            Console.WriteLine("Checksum adalah: ", checksum);
            //string str = "123";
            char[] array = test_measure.ToCharArray();
            string final = "";
            //BigInteger x = 0;
            int checksum_measure = 0;
            //BigInteger checksum_measure = 0;
            

            foreach (var i in array)
            {
                string hex = String.Format("{0:X}", Convert.ToInt32(i));
                //final += hex.Insert(0, "0X") + " ";
                //final = hex.Insert(0, "0X") + " ";
                //final += hex;
                //checksum_measure = checksum_measure + BigInteger.Parse(final, NumberStyles.HexNumber);
                //checksum_measure = checksum_measure + Convert.ToInt32(hex, 16);
                checksum_measure = checksum_measure + Convert.ToInt32(hex);


            }
            Console.WriteLine("checksum measure adalah: " + checksum_measure.ToString());
            if (checksum_measure == Convert.ToInt32(checksum))
            {
                Console.WriteLine("uouo");
            }


        }
        private string CheckTemp()
        {
            temp_cond = true;
            string Result_Parsing = "";
            DateTime check_temp_5min_start = DateTime.Now;
            while (temp_cond)
            {
                try
                {
                    Thread.Sleep(4000);// this solves the problem
                    byte[] readBuffer = new byte[mySerialPort.ReadBufferSize];
                    int readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                    string readStr = string.Empty;

                    readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                    readStr = readStr.Trim();
                    Console.WriteLine("ReadStr adalah: " + readStr);

                    string[] charactersToReplace = new string[] { @"r" };
                    foreach (string s in charactersToReplace)
                    {
                        readStr = readStr.Replace(s, "");
                    }

                    char[] delimiter_r = { '\r' };
                    string[] Measures_With_U = readStr.Split(delimiter_r);

                    foreach (string measure in Measures_With_U)
                    {
                        bool isDigitPresent = measure.Any(c => char.IsDigit(c));
                        if (isDigitPresent == true)
                        {
                            Result_Parsing = measure;
                        }

                    }
                    //Result_Parsing = Measures_With_U.Last();

                    if (check_Error(Result_Parsing) || check_5min_error(check_temp_5min_start))
                    {
                        //error_for_button();
                    }
                    else
                    {
                        Result_Parsing = Result_Parsing.Substring(Result_Parsing.Length - 3);
                        Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                    , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));

                        Temp_TextBox.Invoke((Action)delegate
                        {
                            Temp_TextBox.Text = Result_Parsing;
                        });
                        Sensor_input_Helper.Command_Stop(mySerialPort);
                        temp_cond = false;
                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Console.WriteLine(ex);
                    //return "";
                }
            }

            Console.WriteLine("Finsih_Check_Temp");
            return Result_Parsing;
            //Sensor_input_Helper.Command_Stop(mySerialPort);
        }
        private bool Start_Validation()
        {
            bool isvalid = false;
            if (Combobox_Mode.SelectedIndex < 0)
            {
                MessageBox.Show("PLease Enter Controller Mode", application_name);
                isvalid = false;
            }

            else if (Combobox_Mode.SelectedItem.ToString().ToLower() == "fix time" && Combobox_Mode.SelectedIndex >= 0)
            {
                if (ButtonProduct.Text == "")
                {
                    MessageBox.Show("PLease Enter Product", application_name);
                }
                else if (ButtonWaitingTime.Text == "")
                {
                    MessageBox.Show("PLease Enter Running Time", application_name);
                }
                else
                {
                    // Running Time Interval
                    var result = Sensor_input_Helper.GetEnumValueFromDescription<Running_Time>(ButtonWaitingTime.Text);
                    running_time_fixed = ((int)(result)) * 60 / 1000;

                    // Number Grain Maximal
                    ResultGrain = "12598\r";
                    //ResultGrain = "10192\r";// testing only

                    // Product
                    string combox_typemeasure = ButtonProduct.Text;
                    combox_typemeasure = combox_typemeasure.Replace(" ", "_");
                    TypeOfMeasure enum_typemeasure = (TypeOfMeasure)Enum.Parse(typeof(TypeOfMeasure), combox_typemeasure);
                    ResultMeasure = Sensor_input_Helper.GetDescription(enum_typemeasure);

                    isvalid = true;

                    //running_time_fixed = 120;// in seconds. //testing only
                }

            }
            else if (Combobox_Mode.SelectedItem.ToString().ToLower() == "interval" && Combobox_Mode.SelectedIndex >= 0)
            {
                if (ButtonProduct.Text == "")
                {
                    MessageBox.Show("PLease Enter Product", application_name);
                }
                else if (ButtonNumInterval.Text == "")
                {
                    MessageBox.Show("PLease Enter Number Interval", application_name);
                }
                else if (ButtonNumPcs.Text == "")
                {
                    MessageBox.Show("PLease Enter Number of Pieces", application_name);
                }
                else if (ButtonWaitingTime.Text == "")
                {
                    MessageBox.Show("PLease Enter Waiting Time", application_name);
                }
                else
                {

                    // Waiting Time Interval
                    var result = Sensor_input_Helper.GetEnumValueFromDescription<Time_Interval>(ButtonWaitingTime.Text);
                    delay = ((int)(result)) * 60;

                    // Total Interval
                    TotalInterval = int.Parse(ButtonNumInterval.Text.ToString());

                    // Total Number Per Pieces
                    number_grain enum_numgrain = (number_grain)Enum.Parse(typeof(number_grain), ButtonNumPcs.Text);
                    ResultGrain = Sensor_input_Helper.GetDescription(enum_numgrain);

                    // Product
                    string combox_typemeasure = ButtonProduct.Text;
                    combox_typemeasure = combox_typemeasure.Replace(" ", "_");
                    TypeOfMeasure enum_typemeasure = (TypeOfMeasure)Enum.Parse(typeof(TypeOfMeasure), combox_typemeasure);
                    ResultMeasure = Sensor_input_Helper.GetDescription(enum_typemeasure);

                    isvalid = true;

                }

            }
            else if (Combobox_Mode.SelectedItem.ToString().ToLower() == "fix pieces" && Combobox_Mode.SelectedIndex >= 0)
            {
                if (ButtonProduct.Text == "")
                {
                    MessageBox.Show("PLease Enter Product", application_name);
                }
                else if (ButtonNumPcs.Text == "")
                {
                    MessageBox.Show("PLease Enter Number of Pieces", application_name);
                }
                else
                {

                    // No Waiting Time Interval
                    //var result = Sensor_input_Helper.GetEnumValueFromDescription<Time_Interval>(ButtonWaitingTime.Text);
                    //delay = ((int)(result)) * 60;

                    // Total Interval
                    TotalInterval = 1;

                    // Total Number Per Pieces
                    number_grain enum_numgrain = (number_grain)Enum.Parse(typeof(number_grain), ButtonNumPcs.Text);
                    ResultGrain = Sensor_input_Helper.GetDescription(enum_numgrain);

                    // Product
                    string combox_typemeasure = ButtonProduct.Text;
                    combox_typemeasure = combox_typemeasure.Replace(" ", "_");
                    TypeOfMeasure enum_typemeasure = (TypeOfMeasure)Enum.Parse(typeof(TypeOfMeasure), combox_typemeasure);
                    ResultMeasure = Sensor_input_Helper.GetDescription(enum_typemeasure);

                    isvalid = true;
                }

            }


            return isvalid;

        }
        private bool check_5min_error(DateTime start_count)
        {
            bool status_check_5min = false;
            TimeSpan Time_dif_check_5min = start_count - DateTime.Now;
            if (Time_dif_check_5min.TotalSeconds > 60) // aslinya 300, sekrang tssting dlu
            {
                status_check_5min = true;
            }
            return status_check_5min;
        }
        private bool check_db_connection()
        {
            bool check_dbcon = false;
            check_dbcon = Sensor_input_Helper.Check_MySQL_Connect();
            if (check_dbcon == false)
            {
                MessageBox.Show(this, "Database is not connected");
            }
            return check_dbcon;

        }
        private void next_action_button(bool bool_check_error_next)
        {

            MyTimer.Enabled = false;
            MyTimer.Stop();

            if (!bool_check_error_next)
            {
                Btn_Start.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    Btn_Start.Enabled = true;
                });
                Btn_Stop.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    Btn_Stop.Enabled = true;
                });

                Btn_CheckTemp.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    Btn_CheckTemp.Enabled = true;
                });

                Btn_Check.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    Btn_Check.Enabled = true;
                });
                Combobox_Mode.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    Combobox_Mode.Enabled = true;
                });

                Combobox_ComPort.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    Combobox_ComPort.Enabled = true;
                });

                textBox_Sensor_Status.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    textBox_Sensor_Status.Text = "Online";

                });
            }
            else
            {
                Btn_Start.Invoke((Action)delegate
                {
                    Btn_Start.Enabled = false;
                });
                Btn_Stop.Invoke((Action)delegate
                {
                    Btn_Stop.Enabled = false;
                });
                Btn_CheckTemp.Invoke((Action)delegate
                {
                    Btn_CheckTemp.Enabled = false;
                });

                Btn_Check.Invoke((Action)delegate
                {
                    Btn_Check.Enabled = true;
                });
                textBox_Sensor_Status.Invoke((Action)delegate
                {
                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                    textBox_Sensor_Status.Text = "Error";

                });

            }






        }  
        private bool check_Error(string check_string)
        {
            bool_check_error = false;
            foreach (string error in List_Error_code)
            {
                if (check_string == error)
                {
                    bool_check_error = true;
                }
            }

            if (bool_check_error)
            {
                Console.WriteLine("Match Error adalah: " + check_string);
                //Sensor_input_Helper.Update_ErrorCode(Sensor_input_Helper.GetLocalIPAddress(), batch_id_check, check_string);
                Error_Sensor_Controller enum_ErrorCode = (Error_Sensor_Controller)Enum.Parse(typeof(Error_Sensor_Controller), "error" + check_string);
                string Error_Message = Sensor_input_Helper.GetDescription(enum_ErrorCode);
                MessageBox.Show(this, Error_Message, application_name);

            }
            return bool_check_error;

        }
        private bool check_Error_during_measurement(string check_string, int batch_id_check)
        {
            bool_check_error = false;
            List<string> error_during_measurement = new List<string>(new string[] { "020", "021", "element3" });
            foreach (string error in error_during_measurement)
            {
                if (check_string == error)
                {
                    bool_check_error = true;
                }
            }

            if (bool_check_error)
            {
                Console.WriteLine("Match Error adalah: " + check_string);
                Sensor_input_Helper.Update_ErrorCode(Sensor_input_Helper.GetLocalIPAddress(), batch_id_check, check_string);
                Error_Sensor_Controller enum_ErrorCode = (Error_Sensor_Controller)Enum.Parse(typeof(Error_Sensor_Controller), "error" + check_string);
                string Error_Message = Sensor_input_Helper.GetDescription(enum_ErrorCode);
                MessageBox.Show(this, Error_Message, application_name);

            }
            return bool_check_error;

        }

        #endregion


        private List<string> GetWords(string text)
        {
            Regex reg = new Regex("[a-zA-Z0-9]");
            string Word = "";
            char[] ca = text.ToCharArray();
            List<string> characters = new List<string>();
            for (int i = 0; i < ca.Length; i++)
            {
                char c = ca[i];
                if (c > 65535)
                {
                    continue;
                }
                if (char.IsHighSurrogate(c))
                {
                    i++;
                    characters.Add(new string(new[] { c, ca[i] }));
                }
                else
                {
                    if (reg.Match(c.ToString()).Success || c.ToString() == "/")
                    {
                        Word = Word + c.ToString();
                        //characters.Add(new string(new[] { c }));
                    }
                    else if (c.ToString() == " ")
                    {
                        if (Word.Length > 0)
                            characters.Add(Word);
                        Word = "";
                    }
                    else
                    {
                        if (Word.Length > 0)
                            characters.Add(Word);
                        Word = "";
                    }

                }

            }
            return characters;
        }

        #region Timer
        private void MyTimer_CheckStop_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Start MyTimer_CheckStop_Tick");
            TimeSpan Time_dif_check_5min = DateTime.Now - start_5min_check;
           if (Time_dif_check_5min.TotalSeconds > 70) // aslinya 300, sekrang tssting dlu
           //if (Time_dif_check_5min.TotalMinutes > 1) // aslinya 300, sekrang tssting dlu
           {
                MessageBox.Show(this, "Error 030 - no message during checking for 5 mins");
                //error_for_button();
                checkcommand = false;
                Console.WriteLine("Check Thread Aborted");
                Timer_5min_StopCheck.Enabled = false;
                Timer_5min_StopCheck.Stop();
                //mySerialPort.Close();
                //MessageBox.Show(this, "Error 030 - no message during checking for 5 min
                //check_thread.Abort();
                //mySerialPort.ReadTimeout = 1000;
                //KillCheckThread();
                //check_thread.Interrupt();
                //Thread.Sleep(3000);
            }


        }
        private void MyTimer_Tick(object sender, EventArgs e)
        {
            if (blink_timer % 2 == 0)
            {
                Curr_Measure_TextBox.Invoke((Action)delegate
                {
                    Curr_Measure_TextBox.Text = " " + "." + " ";
                });
            }
            else
            {
                Curr_Measure_TextBox.Invoke((Action)delegate
                {
                    Curr_Measure_TextBox.Text = "";
                });
            }
            blink_timer = blink_timer + 1;

            //Console.WriteLine("blink_timer adalah: ", blink_timer.ToString());
            TimeSpan Calibrate = TimeSpan.FromSeconds(20);
            TimeSpan Time_Dif_Timer = DateTime.Now - FixedTime_start;

            if (stat_continue == true
                && Time_Dif_Timer.TotalSeconds - Calibrate.TotalSeconds >= running_time_fixed  // 240 + 100
                && fixed_time_timer_stop == true

                )
            {
                fixed_time_timer_stop = false;
                Sensor_input_Helper.Command_Stop(mySerialPort);
                Thread.Sleep(2000);
            }

        }
        #endregion

        #region Thread

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        private void KillCheckThread()
        {
            check_thread.Abort();
            
        }

        private void stop_check_5min_thread(DateTime start_count)
        {
            MessageBox.Show(this, "Error 030 - no message during checking for 5 mins");
            check_thread.Abort();


            bool status_check_5min = false;
            TimeSpan Time_dif_check_5min = start_count - DateTime.Now;
            if (Time_dif_check_5min.TotalSeconds > 60) // aslinya 300, sekrang tssting dlu
            {
                status_check_5min = true;
            }
            //return status_check_5min;

        }

        private void Check_Thread()
        {
            try
            {
                SensorHelper_2.OpenCon_Port(mySerialPort, 1200);
                Thread.Sleep(30);

                Sensor_input_Helper.Command_Check(mySerialPort);
                Thread.Sleep(12000);

                checkcommand = true;
                string Result_Parsing = "";
                bool check_db = check_db_connection();
                
                while (checkcommand && check_db)
                {
                    //start_5min_check = DateTime.Now;

                    //Timer_5min_StopCheck.Enabled = true;
                    //Timer_5min_StopCheck.Start();
                    
                    Sensor_input_Helper.Command_CheckData(mySerialPort);
                    Thread.Sleep(2000);// this solves the problem
                    string readStr = string.Empty;
                    byte[] readBuffer = new byte[mySerialPort.ReadBufferSize];
                    int readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                    readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                    readStr = readStr.Trim();

                    Console.WriteLine("ReadStr adalah: " + readStr);

                    string[] charactersToReplace = new string[] { @"r" };
                    foreach (string s in charactersToReplace)
                    {
                        readStr = readStr.Replace(s, "");
                    }

                    char[] delimiter_r = { '\r' };
                    string[] Measures_With_U = readStr.Split(delimiter_r);

                    foreach (string measure in Measures_With_U)
                    {
                        bool isDigitPresent = measure.Any(c => char.IsDigit(c));
                        if (isDigitPresent == true)
                        {
                            Result_Parsing = measure;
                        }

                    }
                    //if (check_Error(Result_Parsing) || check_5min_error(start_5min_check))
                    if (check_Error(Result_Parsing) )
                    {
                        //error_for_button();
                        checkcommand = false;
                    }
                    else if (Result_Parsing == "00090")
                    {
                        Console.WriteLine("Sensor Normal");
                        MessageBox.Show(this, "Connection Succeed");
                        checkcommand = false;
                        data_cleansing();
                        //Ready_To_Start_Button();
                    }
                    else
                    {
                        Console.WriteLine("Check not found. This is result parsing : " + Result_Parsing);
                    }
                    //Timer_5min_StopCheck.Enabled = false;
                    //Timer_5min_StopCheck.Stop();
                }
            }
            catch(TimeoutException ex)
            {
                MessageBox.Show(this, "Error 030 - no message during checking for 5 mins");
                //error_for_button();
                checkcommand = false;
                //Console.WriteLine("Check Thread Aborted");
                Console.WriteLine(ex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void Read_Interval_Thread()
        {

            string forever_str;
            string readStr;
            bool Measure_Cond = true;

            byte[] readBuffer = new byte[mySerialPort.ReadBufferSize];
            int readLen;
            string[] charactersToReplace = new string[] { @"\t", @"\n", @"\r", " ", "<CR>", "<LF>" };
            string Result_Parsing;
            bool countingbatch;
            const char STX = '\u0002';
            const char ETX = '\u0003';
            List<string> AllText = new List<string>();
            Data_Measure_Result = new List<data_measure_2> { };
            counter_data = 0;
            //Data_Measure_Result
            DateTime date_start_ReadInterval = DateTime.Now;

            while (stat_continue)
            {
                try
                {
                    readStr = string.Empty;
                    forever_str = string.Empty;
                    aggregate_cond = true;
                    start_next_cond = true;
                    Measure_Cond = true;
                    countingbatch = true;
                    if (timer_counter == 1)
                    {
                        MyTimer.Enabled = true;
                        MyTimer.Start();

                        timer_counter = 0;
                        Console.WriteLine("MyTimerStart");
                    }

                    #region Collect Measurement Value

                    Thread.Sleep(3000);
                    while (Measure_Cond == true)
                    {
                        Thread.Sleep(1000);// this solves the problem
                        readBuffer = new byte[mySerialPort.ReadBufferSize];
                        readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                        //string readStr = string.Empty;
                        Console.WriteLine("ReadStr original adalah: " + Encoding.UTF8.GetString(readBuffer, 0, readLen));
                        forever_str = forever_str + Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        // Data Cleansing

                        if (readStr != "" && readStr != null)
                        {
                            char[] delimiter_r = { '\r' };

                            if (readStr.Any(c => char.IsDigit(c)) && !readStr.Trim().ToLower().Contains("r"))
                            {
                                readStr = readStr.Trim();
                                Console.WriteLine("ReadStr Trim adalah: " + readStr);
                                string[] Measures_With_U = readStr.Split(delimiter_r); // misahin antar nilai
                                List<string> Measure_Results = new List<string>();

                                foreach (var Measure in Measures_With_U)
                                {
                                    
                                    Result_Parsing = GetWords(Measure).FirstOrDefault(); // hilangin ETX dan STX
                                    if (Result_Parsing != "" && Result_Parsing != null)
                                    {
                                        foreach (string s in charactersToReplace)
                                        {
                                            Result_Parsing = Result_Parsing.Replace(s, "");
                                        }
                                    }

                                    if (Result_Parsing != "" && Result_Parsing != null && !Result_Parsing.Trim().ToLower().Contains("r"))
                                    {
                                        // check error
                                        Console.WriteLine("Result_Parsing & Batch_ID adalah: " + Result_Parsing + " " + batch_id.ToString());
                                        if (check_Error_during_measurement(Result_Parsing, batch_id))
                                        {
                                            aggregate_cond = false;
                                            Measure_Cond = false;
                                            countingbatch = false;
                                            bool_check_error = true;
                                            MyTimer.Enabled = false;
                                            MyTimer.Stop();
                                            Console.WriteLine("MyTimerStop");
                                        }
                                        // Finsih check error

                                        Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                            , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));
                                        counter_data_reset = counter_data_reset + 1;
                                        Console.WriteLine("nilai measure adalah: " + Result_Parsing); // ganti jadi
                                        Curr_Kernel_TextBox.Invoke((Action)delegate
                                        {
                                            //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                            Curr_Kernel_TextBox.Text = (counter_data_reset).ToString();
                                        });
                                        if (thereshold_param == true && (double.Parse(Result_Parsing) > therehold_max || double.Parse(Result_Parsing) < thereshold_min))
                                        {
                                            Sensor_input_Helper.Callbeep();
                                        }

                                        //float Result_Parsing_input = float.Parse(Result_Parsing);
                                        readStr = string.Empty;
                                    }
                                }
                                // klo ada measurement. mulai olah
                                // masukin olah data yag lama 
                            }

                            else if (
                                readStr.Trim().ToLower().Contains("r")
                                //&& counter_data_reset > (int.Parse(ButtonNumPcs.Text) / 2)
                                && counter_data_reset > 1
                                && !readStr.Any(c => char.IsDigit(c))
                                && countingbatch == true
                                )
                            {
                                //counter_data = 0;
                                counter_data_reset = 0;
                                Console.WriteLine("Forever_str original adalah: " + forever_str);

                                /*
                                try
                                {
                                    StreamWriter sw = new StreamWriter("C:\\forever_str.txt");
                                    sw.WriteLine(forever_str);
                                    sw.Close();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Exception: " + e.Message);
                                }
                                finally
                                {
                                    Console.WriteLine("Executing finally block.");
                                }
                                */
                                string[] Measures_With_U = forever_str.Split(delimiter_r); // misahin antar nilai

                                foreach (var Measure in Measures_With_U)
                                {
                                    bool test1 = Measure.Any(c => char.IsDigit(c));
                                    bool test2 = !Measure.Trim().ToLower().Contains("r");
                                    bool test3 = Measure.Contains(STX);
                                    bool test4 = Measure.Contains(ETX);

                                    //bool test3 = Measure.Trim().ToLower().Contains("u0002");
                                    //bool test4 = Measure.Trim().ToLower().Contains("u0003");

                                    if (test1 && test2 && test3 && test4)
                                    {

                                    Result_Parsing = GetWords(Measure).FirstOrDefault(); // hilangin ETX dan STX
                                                                                             // Data cleansing
                                        foreach (string s in charactersToReplace)
                                        {
                                            Result_Parsing = Result_Parsing.Replace(s, "");
                                        }
                                        Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                                , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));

                                        Result_Parsing = (double.Parse(Result_Parsing) + bias_value).ToString();

                                        counter_data = Data_Measure_Result.Count;

                                        Data_Measure_Current = new data_measure_2(counter_data + 1
                                            , Result_Parsing
                                            , (DateTime.Now).ToString());
                                        Data_Measure_Result.Add(Data_Measure_Current);

                                        Console.WriteLine("nilai measure forever str parsing result adalah: " + Result_Parsing); // ganti jadi

                                        float Result_Parsing_input = float.Parse(Result_Parsing);
                                        Sensor_input_Helper.MySql_Insert_Measure(batch_id, counter_data + 1, Result_Parsing_input, DateTime.Now, 0);
                                        counter_data_reset = counter_data_reset + 1;
                                        //readStr = string.Empty;

                                    }
                                    else
                                    {
                                        Console.WriteLine("R-nya isinya trash");
                                    }


                                }
                              
                                Curr_Kernel_TextBox.Invoke((Action)delegate
                                {
                                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                    Curr_Kernel_TextBox.Text = counter_data_reset.ToString();
                                });
                                Measure_Cond = false;
                                countingbatch = false;
                            }

                            else
                            {
                                Console.WriteLine("Nilainya Readstr Dari if else null adalah: " + readStr);
                            }

                        }

                        else
                        {
                            Console.WriteLine("Nilainya Readstr null adalah: " + readStr);
                        }
                        //string input = "hello123world";
                        //bool isDigitPresent = input.Any(c => char.IsDigit(c));

                    }

                    #endregion

                    #region Get Aggregate value

                    //start_next_init = 0;
                    //OpenCon_Port_local(mySerialPort, BaudRate);
                    while (aggregate_cond)
                    {
                        Result_Parsing = string.Empty;
                        Console.WriteLine("Start Aggregate_cond");
                        Sensor_input_Helper.Command_MoisturAggregate(mySerialPort);
                        Thread.Sleep(2000);// this solves the problem
                        readBuffer = new byte[mySerialPort.ReadBufferSize];
                        readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                        readStr = string.Empty;
                        readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        readStr = readStr.Trim();

                        Console.WriteLine("ReadStr Average adalah: " + readStr);
                        foreach (string s in charactersToReplace)
                        {
                            Result_Parsing = readStr.Replace(s, "");
                        }

                        //Result_Parsing = GetWords(Result_Parsing).FirstOrDefault();
                        if (Result_Parsing != null)
                        {
                            if (
                                Result_Parsing.Contains("-")
                                && (Result_Parsing.Length) > 4
                                && Result_Parsing.Contains(STX)
                                && Result_Parsing.Contains(ETX)
                                )
                            {
                                MyTimer.Enabled = false;
                                MyTimer.Stop();

                                AllText = GetWords(Result_Parsing);
                                int checkindex;
                                string aggregate_value_string = string.Empty;
                                foreach (string text in AllText)
                                {
                                    if (
                                        text.Length >= 10
                                        //&& text.Length <= 12
                                        && !text.Trim().ToLower().ToString().Contains("r")
                                        )
                                    {
                                        aggregate_value_string = text;
                                    }
                                }

                                Result_Parsing = aggregate_value_string.Substring(5, 3);
                                Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                    , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));
                                Result_Parsing = (double.Parse(Result_Parsing) + bias_value).ToString("0.0");

                                Data_Avg_Result.Add(new data_measure_2(100, Result_Parsing, (DateTime.Now).ToString()));
                                aggregate_cond = false;


                                Curr_Measure_TextBox.Invoke((Action)delegate
                                {
                                    Curr_Measure_TextBox.Text = Result_Parsing;
                                    // Latest Average
                                });
                                total_average = 0;
                                Current_Avg_TextBox.Invoke((Action)delegate
                                {
                                    foreach (data_measure_2 average_val in Data_Avg_Result)
                                    {
                                        total_average = total_average + float.Parse(average_val.Measures);
                                    }

                                    total_current_Average = total_average / Data_Avg_Result.Count();
                                    Current_Avg_TextBox.Text = total_current_Average.ToString("0.0") + "%";
                                    //Final Average
                                });

                                Textbox_Forever.Invoke((Action)delegate
                                {
                                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                    Textbox_Forever.Text = forever_str;
                                });

                                //loat Result_Parsing_input = float.Parse(Result_Parsing);
                                Sensor_input_Helper.MySql_Insert_Measure(batch_id, 1000 + current_interval + 1, float.Parse(Result_Parsing), DateTime.Now, 1);

                                Console.WriteLine("Finish Aggregate");
                                readStr = string.Empty;
                            }
                        }
                        //start_next_init++;
                    }

                    #endregion Finish get aggregate value
                    Console.WriteLine("Finish aggregate region");

                    #region Finish All Measure and close port

                    Console.WriteLine("data_average count adalah: ", Data_Avg_Result.Count().ToString());
                    //Console.WriteLine("data_average count adalah: ", current_interval.ToString());

                    if (Data_Avg_Result.Count() == TotalInterval || bool_check_error == true)
                    {
                        Console.WriteLine("End All Measurement ");
                        stat_continue = false;
                        mySerialPort.DiscardInBuffer();
                        mySerialPort.DiscardOutBuffer();
                        
                        stat_continue = false;
                        start_next_cond = false;
                        aggregate_cond = false;

                        finish_measurement = 1;

                        next_action_button(bool_check_error);

                    }


                    #endregion

                    #region delay start
                    if (start_next_cond == true)
                    {
                        #region Delay start
                        Console.WriteLine("start delay", "start delay");
                        //mySerialPort.Close();
                        Thread.Sleep(delay);
                        Console.WriteLine("Finish delay", "Finish delay");
                        #endregion
                    }
                    #endregion


                    #region Start Next sequence

                    while (start_next_cond)
                    {
                        Sensor_input_Helper.Command_Write(mySerialPort, ResultGrain);
                        Thread.Sleep(1000);
                        Sensor_input_Helper.Command_Write(mySerialPort, ResultMeasure);
                        current_interval++;
                        Curr_Interval_TextBox.Invoke((Action)delegate
                        {
                            Curr_Interval_TextBox.Text = (current_interval + 1).ToString();
                        });

                        start_next_cond = false;
                        blink_timer = 1;
                        timer_counter = 1;
                        counter_data_reset = 0;
                        readStr = string.Empty;


                    }
                    #endregion

                }
                catch (TimeoutException ex)
                {
                    MessageBox.Show(this, "Error 030 - no message during checking for 5 mins");
                    bool error = true;
                    next_action_button(error);

                    mySerialPort.DiscardInBuffer();
                    mySerialPort.DiscardOutBuffer();

                    stat_continue = false;
                    start_next_cond = false;
                    aggregate_cond = false;
                    Console.WriteLine(ex.Message);
                }
                
                catch (Exception ex)
                {
                    //Trace.TraceError(ex.Message);
                    Console.WriteLine(ex.Message);
                    //return "";
                }

            }

            //MessageBox.Show("measurement finsih");
            Console.WriteLine("Measurement Finish");
        }
        public void Read_FixedTime_Thread()
        {
            
            string forever_str= string.Empty; ;
            string readStr;
            bool Measure_Cond = true;
            byte[] readBuffer = new byte[mySerialPort.ReadBufferSize];
            int readLen;
            string[] charactersToReplace = new string[] { @"\t", @"\n", @"\r", " ", "<CR>", "<LF>" };
            string Result_Parsing;
            bool countingbatch;
            const char STX = '\u0002';
            const char ETX = '\u0003';
            List<string> AllText = new List<string>();
            Data_Measure_Result = new List<data_measure_2> { };
            counter_data = 0;
            //Data_Measure_Result
            
            //DateTime date_start_ReadFixedTime = DateTime.Now;
            char[] delimiter_r = { '\r' };

            Console.WriteLine("Running Time Fixed is: " + running_time_fixed.ToString());
            while (stat_continue)
            {
                
                try
                {
                    readStr = string.Empty;
                    aggregate_cond = true;
                    start_next_cond = true;
                    Measure_Cond = true;
                    countingbatch = true;
                    if (timer_counter == 1)
                    {
                        //MyTimer.Elapsed += new ElapsedEventHandler(MyTimer_Tick);
                        //MyTimer.Interval = (2000); // 45 mins
                        MyTimer.Enabled = true;
                        MyTimer.Start();

                        timer_counter = 0;
                        Console.WriteLine("MyTimerStart");
                    }

                    #region Collect Measurement Value

                    Thread.Sleep(3000);
                    while (Measure_Cond == true)
                    {
                        
                        Thread.Sleep(1000);// this solves the problem
                        readBuffer = new byte[mySerialPort.ReadBufferSize];
                        readLen = -1;
                        readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);                        
                        Console.WriteLine("ReadStr original adalah: " + Encoding.UTF8.GetString(readBuffer, 0, readLen));
                        forever_str = forever_str + Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        
                        #region Add Interval
                        if (readStr != "" && readStr != null)
                        {
                            if (readStr.Any(c => char.IsDigit(c)) && !readStr.Trim().ToLower().Contains("r"))
                            {
                                readStr = readStr.Trim();
                                Console.WriteLine("ReadStr Trim adalah: " + readStr);
                                string[] Measures_With_U = readStr.Split(delimiter_r); // misahin antar nilai
                                List<string> Measure_Results = new List<string>();

                                foreach (var Measure in Measures_With_U)
                                {
                                    
                                    Result_Parsing = GetWords(Measure).FirstOrDefault(); // hilangin ETX dan STX
                                    if (Result_Parsing != "" && Result_Parsing != null)
                                    {
                                        foreach (string s in charactersToReplace)
                                        {
                                            Result_Parsing = Result_Parsing.Replace(s, "");
                                        }
                                    }

                                    if (Result_Parsing != "" && Result_Parsing != null && !Result_Parsing.Trim().ToLower().Contains("r"))
                                    {
                                        // check error
                                        Console.WriteLine("Measure & Batch_ID adalah: " + Result_Parsing + " " + batch_id.ToString());
                                        if (check_Error_during_measurement(Result_Parsing, batch_id))
                                        {
                                            aggregate_cond = false;
                                            Measure_Cond = false;
                                            countingbatch = false;
                                            bool_check_error = true;
                                            MyTimer.Enabled = false;
                                            MyTimer.Stop();
                                            Console.WriteLine("Timer Stop");
                                        }
                                        // FInsih check error


                                        Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                            , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));
                                        counter_data_reset = counter_data_reset + 1;
                                        Console.WriteLine("nilai measure adalah: " + Result_Parsing); // ganti jadi
                                        Curr_Kernel_TextBox.Invoke((Action)delegate
                                        {
                                            //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                            Curr_Kernel_TextBox.Text = (counter_data_reset).ToString();
                                        });
                                        if (thereshold_param == true && (double.Parse(Result_Parsing) > therehold_max || double.Parse(Result_Parsing) < thereshold_min))
                                        {
                                            Sensor_input_Helper.Callbeep();
                                        }
                                        //float Result_Parsing_input = float.Parse(Result_Parsing);
                                        readStr = string.Empty;
                                    }
                                }
                                // klo ada measurement. mulai olah
                                // masukin olah data yag lama 
                            }
                            
                            else if(
                                readStr.Trim().ToLower().Contains("r")
                                && counter_data_reset > 8
                                && !readStr.Any(c => char.IsDigit(c))
                                && countingbatch == true
                                )
                            {
                                Sensor_input_Helper.Command_Write(mySerialPort, "12598\r"); // max value

                                Thread.Sleep(1000);
                                Sensor_input_Helper.Command_Write(mySerialPort, ResultMeasure);
                                //start_next_cond = false;
                                blink_timer = 1;
                                timer_counter = 1;
                                //counter_data_reset = 0;
                                readStr = string.Empty;
                            }
                            else
                            {
                                Console.WriteLine("Nilainya Readstr Dari if else null adalah: " + readStr);
                            }

                        }

                        else
                        {
                            Console.WriteLine("Nilainya Readstr null adalah: " + readStr);
                        }

                        #endregion


                        #region finish measurement
                        Time_Dif = DateTime.Now - FixedTime_start;
                        if (Time_Dif.TotalSeconds > running_time_fixed || bool_check_error == true || bool_stop_click == true) // change from time.in seconds
                        {
                            Console.WriteLine("DateTime Now & FixedTime_Start: " + DateTime.Now.ToString() + " & " + FixedTime_start.ToString());
                            Console.WriteLine("Time Dif Total second & Running Time Fixed adalah: " + Time_Dif.TotalSeconds.ToString() + " & " + running_time_fixed.ToString());
                            MyTimer.Enabled = false;
                            MyTimer.Stop();

                            Sensor_input_Helper.Command_Stop(mySerialPort);
                            Thread.Sleep(3000);
                            Sensor_input_Helper.Command_Stop(mySerialPort);
                            Thread.Sleep(3000);
                            Sensor_input_Helper.Command_Stop(mySerialPort);
                            Thread.Sleep(3000);
                            Console.WriteLine("Send Stop for fixed Time");
                            string[] Measures_With_U = forever_str.Split(delimiter_r); // misahin antar nilai
                            counter_data_reset = 0;
                            foreach (var Measure in Measures_With_U)
                            {
                                bool test1 = Measure.Any(c => char.IsDigit(c));
                                bool test2 = !Measure.Trim().ToLower().Contains("r");
                                bool test3 = Measure.Contains(STX);
                                bool test4 = Measure.Contains(ETX);
                                if (test1 && test2 && test3 && test4)
                                {
                                    Result_Parsing = GetWords(Measure).FirstOrDefault(); // hilangin ETX dan STX
                                                                                         // Data cleansing
                                    foreach (string s in charactersToReplace)
                                    {
                                        Result_Parsing = Result_Parsing.Replace(s, "");
                                    }
                                    Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                            , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));

                                    Result_Parsing = (double.Parse(Result_Parsing) + bias_value).ToString("0.0");

                                    counter_data = Data_Measure_Result.Count;

                                    Data_Measure_Current = new data_measure_2(counter_data + 1
                                        , Result_Parsing
                                        , (DateTime.Now).ToString());
                                    Data_Measure_Result.Add(Data_Measure_Current);

                                    Console.WriteLine("nilai measure forever str parsing result adalah: " + Result_Parsing); // ganti jadi

                                    float Result_Parsing_input = float.Parse(Result_Parsing);
                                    Sensor_input_Helper.MySql_Insert_Measure(batch_id, counter_data + 1, Result_Parsing_input, DateTime.Now, 0);
                                    //counter_data_reset = counter_data_reset + 1;
                                    //readStr = string.Empty;
                                    Curr_Kernel_TextBox.Invoke((Action)delegate
                                    {
                                        //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                        Curr_Kernel_TextBox.Text = Data_Measure_Result.Count().ToString();
                                    });
                                }
                                else
                                {
                                    Console.WriteLine("R-nya isinya trash");
                                }
                            }

                            #region Get Aggregate value
                            while (aggregate_cond)
                            {
                                mySerialPort.DiscardOutBuffer();
                                mySerialPort.DiscardInBuffer();
                                readBuffer = new byte[mySerialPort.ReadBufferSize];
                                readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                                readStr = string.Empty;
                                readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                                readStr = readStr.Trim();

                                Console.WriteLine("Start Aggregate_cond");
                                Sensor_input_Helper.Command_MoisturAggregate(mySerialPort);
                                Thread.Sleep(2000);// this solves the problem
                                Result_Parsing = string.Empty;
                                readBuffer = new byte[mySerialPort.ReadBufferSize];
                                readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                                readStr = string.Empty;
                                readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                                readStr = readStr.Trim();

                                Console.WriteLine("ReadStr Average adalah: " + readStr);

                                foreach (string s in charactersToReplace)
                                {
                                    Result_Parsing = readStr.Replace(s, "");
                                }

                                if (Result_Parsing != null)
                                {
                                    Console.WriteLine("Result parsing adalah: " + Result_Parsing);
                                    if (
                                        Result_Parsing.Contains(STX)
                                        && Result_Parsing.Contains(ETX)
                                        && Result_Parsing.Contains("-")
                                        && (Result_Parsing.Length) > 8
                                        && Data_Measure_Result.Count >= 1
                                        )
                                    {
                                        

                                        AllText = GetWords(Result_Parsing);
                                        string aggregate_value_string = string.Empty;
                                        foreach (string text in AllText)
                                        {
                                            if (
                                                text.Length >= 10
                                                && !text.Trim().ToLower().ToString().Contains("r")
                                                )
                                            {
                                                aggregate_value_string = text;
                                            }
                                        }
                                        Console.WriteLine("ReadStr Aggreaget_value_string adalah: " + aggregate_value_string);

                                        Result_Parsing = aggregate_value_string.Substring(5, 3);
                                        Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                            , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));
                                        Result_Parsing = (double.Parse(Result_Parsing) + bias_value).ToString();

                                        Data_Avg_Result.Add(new data_measure_2(100, Result_Parsing, (DateTime.Now).ToString()));
                                        aggregate_cond = false;


                                        Curr_Measure_TextBox.Invoke((Action)delegate
                                        {
                                            Curr_Measure_TextBox.Text = Result_Parsing;
                                        });
                                        total_average = 0;
                                        Current_Avg_TextBox.Invoke((Action)delegate
                                        {
                                            foreach (data_measure_2 average_val in Data_Avg_Result)
                                            {
                                                total_average = total_average + float.Parse(average_val.Measures);
                                            }

                                            total_current_Average = total_average / Data_Avg_Result.Count();
                                            Current_Avg_TextBox.Text = total_current_Average.ToString("0.0") + "%";
                                            //Final Average
                                        });
                                        
                                        //loat Result_Parsing_input = float.Parse(Result_Parsing);
                                        Sensor_input_Helper.MySql_Insert_Measure(batch_id, 1000 + current_interval + 1, float.Parse(Result_Parsing), DateTime.Now, 1);

                                        Console.WriteLine("Finish Aggregate");
                                        readStr = string.Empty;

                                    }

                                    else if (
                                        Data_Measure_Result.Count == 0
                                        || (!Result_Parsing.Contains("-") && (Result_Parsing.Length) > 10)
                                        
                                        )
                                    {
                                        Result_Parsing = "0.0";

                                        Data_Avg_Result.Add(new data_measure_2(100, Result_Parsing, (DateTime.Now).ToString()));
                                        aggregate_cond = false;


                                        Curr_Measure_TextBox.Invoke((Action)delegate
                                        {
                                            Curr_Measure_TextBox.Text = Result_Parsing;
                                        });

                                        Current_Avg_TextBox.Invoke((Action)delegate
                                        {
                                            foreach (data_measure_2 average_val in Data_Avg_Result)
                                            {
                                                total_average = total_average + float.Parse(average_val.Measures);
                                            }

                                            total_current_Average = total_average / Data_Avg_Result.Count();
                                            Current_Avg_TextBox.Text = total_current_Average.ToString("0.0") + "%";
                                            //Final Average
                                        });

                                    }
                                    else
                                    {
                                        Console.WriteLine("Aggreagte empty");
                                    }
                                }
                                //start_next_init++;
                            }

                            #endregion Finish get aggregate value

                            // Stop measurement
                            stat_continue = false;
                            start_next_cond = false;
                            aggregate_cond = false;
                            Measure_Cond = false;

                            if (bool_check_error)
                            {
                                //error_for_button();
                            }
                            else
                            {
                                //Ready_To_Start_Button();
                            }

                            next_action_button(bool_check_error);

                        }

                        #endregion

                    }

                    #endregion


                    #region Start Next sequence

                    while (start_next_cond)
                    {
                        //Sensor_input_Helper.Command_Write(mySerialPort, "12598\r"); // max value
                        Sensor_input_Helper.Command_Write(mySerialPort, "10192\r"); // max value

                        Thread.Sleep(1000);
                        Sensor_input_Helper.Command_Write(mySerialPort, ResultMeasure);
                        current_interval++;
                        
                        
                        //start_next_cond = false;
                        blink_timer = 1;
                        timer_counter = 1;
                        //counter_data_reset = 0;
                        readStr = string.Empty;
                    }
                    #endregion


                }
                catch (TimeoutException ex)
                {
                    
                    
                    MyTimer.Enabled = false;
                    MyTimer.Stop();
                    //mySerialPort.DiscardInBuffer();
                    //mySerialPort.DiscardOutBuffer();
                    stat_continue = false;
                    start_next_cond = false;
                    aggregate_cond = false;
                    Measure_Cond = false;
                    bool error = true;
                    next_action_button(error);
                    MessageBox.Show(this, "Error 030 - no message during checking for 5 mins");
                    Console.WriteLine(ex.Message);

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Console.WriteLine(ex);
                    //return "";
                }

            }

            //MessageBox.Show("measurement finsih");
            Console.WriteLine("Measurement Finish");
        }
        public void Read_FixedPieces_Thread()
        {

            string forever_str;
            string readStr;
            bool Measure_Cond = true;

            byte[] readBuffer = new byte[mySerialPort.ReadBufferSize];
            int readLen;
            string[] charactersToReplace = new string[] { @"\t", @"\n", @"\r", " ", "<CR>", "<LF>" };
            string Result_Parsing;
            bool countingbatch;
            const char STX = '\u0002';
            const char ETX = '\u0003';
            List<string> AllText = new List<string>();
            Data_Measure_Result = new List<data_measure_2> { };
            counter_data = 0;
            DateTime Date_Start_5min_FixedPieces = DateTime.Now;
            //Data_Measure_Result

            while (stat_continue)
            {
                try
                {
                    readStr = string.Empty;
                    forever_str = string.Empty;
                    aggregate_cond = true;
                    start_next_cond = true;
                    Measure_Cond = true;
                    countingbatch = true;
                    if (timer_counter == 1)
                    {
                        MyTimer.Elapsed += new ElapsedEventHandler(MyTimer_Tick);
                        MyTimer.Interval = (2000); // 45 mins
                        MyTimer.Enabled = true;
                        MyTimer.Start();

                        timer_counter = 0;
                    }

                    #region Collect Measurement Value

                    Thread.Sleep(3000);
                    while (Measure_Cond == true)
                    {
                        Thread.Sleep(1000);// this solves the problem
                        readBuffer = new byte[mySerialPort.ReadBufferSize];
                        readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                        //string readStr = string.Empty;
                        Console.WriteLine("ReadStr original adalah: " + Encoding.UTF8.GetString(readBuffer, 0, readLen));
                        forever_str = forever_str + Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        // Data Cleansing

                        if (readStr != "" && readStr != null)
                        {
                            char[] delimiter_r = { '\r' };

                            if (readStr.Any(c => char.IsDigit(c)) && !readStr.Trim().ToLower().Contains("r"))
                            {
                                readStr = readStr.Trim();
                                Console.WriteLine("ReadStr Trim adalah: " + readStr);
                                string[] Measures_With_U = readStr.Split(delimiter_r); // misahin antar nilai
                                List<string> Measure_Results = new List<string>();

                                foreach (var Measure in Measures_With_U)
                                {
                                    Result_Parsing = GetWords(Measure).FirstOrDefault(); // hilangin ETX dan STX
                                    if (Result_Parsing != "" && Result_Parsing != null)
                                    {
                                        foreach (string s in charactersToReplace)
                                        {
                                            Result_Parsing = Result_Parsing.Replace(s, "");
                                        }
                                    }

                                    if (Result_Parsing != "" && Result_Parsing != null && !Result_Parsing.Trim().ToLower().Contains("r"))
                                    {
                                        // check error
                                        Console.WriteLine("Result_Parsing & Batch_ID adalah: " + Result_Parsing + " " + batch_id.ToString());
                                        if (check_Error_during_measurement(Result_Parsing, batch_id) || check_5min_error(Date_Start_5min_FixedPieces))
                                        {
                                            aggregate_cond = false;
                                            Measure_Cond = false;
                                            countingbatch = false;
                                            bool_check_error = true;
                                            MyTimer.Enabled = false;
                                            MyTimer.Stop();

                                        }
                                        // FInsih check error

                                        Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                            , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));
                                        counter_data_reset = counter_data_reset + 1;
                                        Console.WriteLine("nilai measure adalah: " + Result_Parsing); // ganti jadi
                                        Curr_Kernel_TextBox.Invoke((Action)delegate
                                        {
                                            //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                            Curr_Kernel_TextBox.Text = (counter_data_reset).ToString();
                                        });
                                        if (thereshold_param == true && (double.Parse(Result_Parsing) > therehold_max || double.Parse(Result_Parsing) < thereshold_min))
                                        {
                                            Sensor_input_Helper.Callbeep();
                                        }
                                        //float Result_Parsing_input = float.Parse(Result_Parsing);
                                        readStr = string.Empty;
                                    }
                                }
                                // klo ada measurement. mulai olah
                                // masukin olah data yag lama 
                            }

                            else if (
                                readStr.Trim().ToLower().Contains("r")
                                && counter_data_reset > (int.Parse(ButtonNumPcs.Text) / 2)
                                && !readStr.Any(c => char.IsDigit(c))
                                && countingbatch == true
                                )
                            {
                                //counter_data = 0;
                                counter_data_reset = 0;
                                Console.WriteLine("Forever_str original adalah: " + forever_str);

                                try
                                {
                                    //Pass the filepath and filename to the StreamWriter Constructor
                                    StreamWriter sw = new StreamWriter("C:\\forever_str.txt");
                                    //Write a line of text
                                    sw.WriteLine(forever_str);
                                    sw.Close();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Exception: " + e.Message);
                                }
                                finally
                                {
                                    Console.WriteLine("Executing finally block.");
                                }

                                string[] Measures_With_U = forever_str.Split(delimiter_r); // misahin antar nilai

                                foreach (var Measure in Measures_With_U)
                                {
                                    bool test1 = Measure.Any(c => char.IsDigit(c));
                                    bool test2 = !Measure.Trim().ToLower().Contains("r");
                                    bool test3 = Measure.Contains(STX);
                                    bool test4 = Measure.Contains(ETX);

                                    //bool test3 = Measure.Trim().ToLower().Contains("u0002");
                                    //bool test4 = Measure.Trim().ToLower().Contains("u0003");

                                    if (test1 && test2 && test3 && test4)
                                    {
                                        string checksum = (Measure.Substring(5, 2));
                                        Console.WriteLine("nilai checksum adalah: ", checksum);


                                        Result_Parsing = GetWords(Measure).FirstOrDefault(); // hilangin ETX dan STX
                                                                                             // Data cleansing
                                        foreach (string s in charactersToReplace)
                                        {
                                            Result_Parsing = Result_Parsing.Replace(s, "");
                                        }
                                        Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                                , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));

                                        Result_Parsing = (double.Parse(Result_Parsing) + bias_value).ToString();

                                        counter_data = Data_Measure_Result.Count;

                                        Data_Measure_Current = new data_measure_2(counter_data + 1
                                            , Result_Parsing
                                            , (DateTime.Now).ToString());
                                        Data_Measure_Result.Add(Data_Measure_Current);

                                        Console.WriteLine("nilai measure forever str parsing result adalah: " + Result_Parsing); // ganti jadi

                                        float Result_Parsing_input = float.Parse(Result_Parsing);
                                        Sensor_input_Helper.MySql_Insert_Measure(batch_id, counter_data + 1, Result_Parsing_input, DateTime.Now, 0);
                                        counter_data_reset = counter_data_reset + 1;
                                        //readStr = string.Empty;

                                    }
                                    else
                                    {
                                        Console.WriteLine("R-nya isinya trash");
                                    }


                                }
                                // Validasi
                                /*
                                while (counter_data_reset < int.Parse(ButtonNumPcs.Text))
                                {
                                    //integerList[integerList.Count - 1];
                                    Data_Measure_Result.Add(Data_Measure_Result[Data_Measure_Result.Count - 1]);
                                    counter_data = Data_Measure_Result.Count;
                                    counter_data_reset = counter_data_reset + 1;
                                }
                                */
                                //

                                Curr_Kernel_TextBox.Invoke((Action)delegate
                                {
                                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                    Curr_Kernel_TextBox.Text = counter_data_reset.ToString();
                                });
                                Measure_Cond = false;
                                countingbatch = false;
                            }

                            else
                            {
                                Console.WriteLine("Nilainya Readstr Dari if else null adalah: " + readStr);
                            }

                        }

                        else
                        {
                            Console.WriteLine("Nilainya Readstr null adalah: " + readStr);
                        }
                        //string input = "hello123world";
                        //bool isDigitPresent = input.Any(c => char.IsDigit(c));

                    }

                    #endregion

                    #region Get Aggregate value

                    //start_next_init = 0;
                    //OpenCon_Port_local(mySerialPort, BaudRate);
                    while (aggregate_cond)
                    {
                        Result_Parsing = string.Empty;
                        Console.WriteLine("Start Aggregate_cond");
                        Sensor_input_Helper.Command_MoisturAggregate(mySerialPort);
                        Thread.Sleep(2000);// this solves the problem
                        readBuffer = new byte[mySerialPort.ReadBufferSize];
                        readLen = mySerialPort.Read(readBuffer, 0, readBuffer.Length);
                        readStr = string.Empty;
                        readStr = Encoding.UTF8.GetString(readBuffer, 0, readLen);
                        readStr = readStr.Trim();

                        Console.WriteLine("ReadStr Average adalah: " + readStr);
                        foreach (string s in charactersToReplace)
                        {
                            Result_Parsing = readStr.Replace(s, "");
                        }

                        //Result_Parsing = GetWords(Result_Parsing).FirstOrDefault();
                        if (Result_Parsing != null)
                        {
                            if (
                                Result_Parsing.Contains("-")
                                && (Result_Parsing.Length) > 4
                                && Result_Parsing.Contains(STX)
                                && Result_Parsing.Contains(ETX)
                                )
                            {
                                MyTimer.Enabled = false;
                                MyTimer.Stop();

                                AllText = GetWords(Result_Parsing);
                                int checkindex;
                                string aggregate_value_string = string.Empty;
                                foreach (string text in AllText)
                                {
                                    if (
                                        text.Length >= 10
                                        //&& text.Length <= 12
                                        && !text.Trim().ToLower().ToString().Contains("r")
                                        )
                                    {
                                        aggregate_value_string = text;
                                    }
                                }

                                Result_Parsing = aggregate_value_string.Substring(5, 3);
                                Result_Parsing = String.Concat(Result_Parsing.Substring(0, Result_Parsing.Length - 1)
                                    , ".", Result_Parsing.Substring(Result_Parsing.Length - 1, 1));
                                Result_Parsing = (double.Parse(Result_Parsing) + bias_value).ToString();

                                Data_Avg_Result.Add(new data_measure_2(100, Result_Parsing, (DateTime.Now).ToString()));
                                aggregate_cond = false;


                                Curr_Measure_TextBox.Invoke((Action)delegate
                                {
                                    Curr_Measure_TextBox.Text = Result_Parsing;
                                    // Latest Average
                                });
                                total_average = 0;
                                Current_Avg_TextBox.Invoke((Action)delegate
                                {
                                    foreach (data_measure_2 average_val in Data_Avg_Result)
                                    {
                                        total_average = total_average + float.Parse(average_val.Measures);
                                    }

                                    total_current_Average = total_average / Data_Avg_Result.Count();
                                    Current_Avg_TextBox.Text = total_current_Average.ToString("0.0") + "%";
                                    //Final Average
                                });
                                Textbox_Forever.Invoke((Action)delegate
                                {
                                    //Curr_Kernel_TextBox.Text = (counter_data + 1).ToString();
                                    Textbox_Forever.Text = forever_str;
                                });

                                //loat Result_Parsing_input = float.Parse(Result_Parsing);
                                Sensor_input_Helper.MySql_Insert_Measure(batch_id, 1000 + current_interval + 1, float.Parse(Result_Parsing), DateTime.Now, 1);

                                Console.WriteLine("Finish Aggregate");
                                readStr = string.Empty;
                            }
                        }
                        //start_next_init++;
                    }

                    #endregion Finish get aggregate value
                    Console.WriteLine("Finish aggregate region");

                    #region Finish All Measure and close port

                    Console.WriteLine("data_average count adalah: ", Data_Avg_Result.Count().ToString());
                    //Console.WriteLine("data_average count adalah: ", current_interval.ToString());

                    if (Data_Avg_Result.Count() == TotalInterval || bool_check_error == true)
                    {
                        stat_continue = false;
                        mySerialPort.DiscardInBuffer();
                        mySerialPort.DiscardOutBuffer();
                        stat_continue = false;
                        start_next_cond = false;
                        aggregate_cond = false;

                        next_action_button(bool_check_error);
                        finish_measurement = 1;



                    }


                    #endregion

                }
                catch (TimeoutException ex)
                {
                    stat_continue = false;
                    stat_continue = false;
                    start_next_cond = false;
                    aggregate_cond = false;
                    bool error = true;
                    next_action_button(error);
                    MessageBox.Show(this, "Error 030 - no message during checking for 5 mins");
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Console.WriteLine(ex);
                    //return "";
                }

            }

            //MessageBox.Show("measurement finsih");
            Console.WriteLine("Measurement Finish");
        }
        
        #endregion
        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                check_Error("010");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR CODE SALAH");
                Console.WriteLine(ex.Message);
                //  Block of code to handle errors
            }
        }

        private void changeip_Click(object sender, EventArgs e)
        {
            var existing_ip = textBox_sensornumber.Text;
            var setting_ip = ipaddressset.Value;
            ProcessStartInfo procStartInfo = new ProcessStartInfo("/usr/bin/sudo", "/bin/sed -i 's/static ip_address=192.168.0."+ existing_ip + "/static ip_address=192.168.0."+ setting_ip + "/' /etc/dhcpcd.conf");
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            ProcessStartInfo procStartInfo2 = new ProcessStartInfo("/usr/bin/sudo", "service dhcpcd restart");
            procStartInfo2.RedirectStandardOutput = true;
            procStartInfo2.UseShellExecute = false;
            procStartInfo2.CreateNoWindow = true;

            System.Diagnostics.Process proc2 = new System.Diagnostics.Process();
            proc2.StartInfo = procStartInfo2;
            proc2.Start();
            MessageBox.Show("Change Sensor Number Successfull, System will reboot");

            ProcessStartInfo procStartInfo3 = new ProcessStartInfo("/usr/bin/sudo", "reboot");
            procStartInfo3.RedirectStandardOutput = true;
            procStartInfo3.UseShellExecute = false;
            procStartInfo3.CreateNoWindow = true;

            System.Diagnostics.Process proc3 = new System.Diagnostics.Process();
            proc3.StartInfo = procStartInfo3;
            proc3.Start();
        }

        private void textBox_sensornumber_TextChanged(object sender, EventArgs e)
        {

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

        private void button4_Click(object sender, EventArgs e)
        {
            using (var form = new FormIPSet())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ButtonIPSet.Text = FormIPSet.combobox_selectedItem_ipsetting;
                }
            }

        }
    }
}
