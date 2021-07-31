using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

using System.IO;

using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel; // CancelEventArgs
using System.Configuration;
using ControllerPage.Constant;
using System.Reflection;
using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ControllerPage.Library;
//using RaspberryIO;
//using MySql.data.Entity;
using System.Data;
using System.Net;
using System.Net.Sockets;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;


namespace ControllerPage.Helper
{
    public class Sensor_input_Helper
    {

        public static string GetLocalIPAddress()
        {
           //return "192.168.0.2";

            var host = Dns.GetHostEntry(Dns.GetHostName());
            
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }


            
         
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static List<String> get_List_Error_Code()
        {
            //List<string> Default_Error_code = Enum.GetValues(typeof(Error_Sensor_Controller)).Cast<Error_Sensor_Controller>().Select(v => v.ToString()).ToList();
            List<String> List_Error_Code = new List<String> {  };
            var Var_List_Error_code = Enum.GetValues(typeof(Error_Sensor_Controller)).Cast<Error_Sensor_Controller>().Select(v => v.ToString()).ToList()
                .Select(s => s.Replace("error", "")).ToList();

            foreach (string error_Code in Var_List_Error_code)
            {
                List_Error_Code.Add(error_Code);
            }
            return List_Error_Code;
        }
        public static void Command_Check(SerialPort mySerialPort)
        {
            //mySerialPort.Write("123");
            //public void Write (byte[] buffer, int offset, int count);
            string data = "00191\r";
            byte[] hexstring = Encoding.ASCII.GetBytes(data);

            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }
        }
        public static void Command_CheckData(SerialPort mySerialPort)
        {
            //mySerialPort.Write("123");
            //public void Write (byte[] buffer, int offset, int count);
            string data = "9119B\r";
            byte[] hexstring = Encoding.ASCII.GetBytes(data);
            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                            // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }
        }
        public static void Command_Stop(SerialPort mySerialPort)
        {
            //mySerialPort.Write("123");
            //public void Write (byte[] buffer, int offset, int count);
            string data = "\r";
            byte[] hexstring = Encoding.ASCII.GetBytes(data);
            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                            // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }
        }

        public static void Command_CheckTemp(SerialPort mySerialPort)
        {
            //mySerialPort.Write("123");
            //public void Write (byte[] buffer, int offset, int count);
            string data = "9239E\r";
            byte[] hexstring = Encoding.ASCII.GetBytes(data);
            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                            // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }
        }


        public static void Command_MoistureMeasure(SerialPort mySerialPort, string input)
        {
            string data = input;
            byte[] hexstring = Encoding.ASCII.GetBytes(data);
            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                            // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }
        }
        public static void Command_NumberofGrain(SerialPort mySerialPort, string input)
        {

            string data = input;
            byte[] hexstring = Encoding.ASCII.GetBytes(data);
            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                            // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }
        }

        public static void Command_Write(SerialPort mySerialPort, string input)
        {
            string data = input;
            byte[] hexstring = Encoding.ASCII.GetBytes(data);
            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                            // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }

        }
        public static void Command_MoisturAggregate(SerialPort mySerialPort)
        {
            //mySerialPort.Write("123");
            //public void Write (byte[] buffer, int offset, int count);
            string data = "9129C\r";
            byte[] hexstring = Encoding.ASCII.GetBytes(data);
            foreach (byte hexval in hexstring)
            {
                byte[] _hexval = new byte[] { hexval };     // need to convert byte 
                                                            // to byte[] to write
                mySerialPort.Write(_hexval, 0, 1);
                //Thread.Sleep(10);
            }
        }
        public static List<string> Get_List_Waiting_Time_Interval()
        {
            var attributes = typeof(Time_Interval).GetMembers()
                .SelectMany(member => member.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>())
                .ToList();

            var result = attributes.Select(x => x.Description);
            List<string> asList = attributes.Select(x => x.Description).ToList();

            return asList;

        }

        public static List<string> Get_List_Running_Time()
        {
            var attributes = typeof(Running_Time).GetMembers()
                .SelectMany(member => member.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>())
                .ToList();

            var result = attributes.Select(x => x.Description);
            List<string> asList = attributes.Select(x => x.Description).ToList();

            return asList;

        }

        public static List<string> Get_List_Number_Grain()
        {
            var attributes = typeof(number_grain).GetMembers()
                .SelectMany(member => member.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>())
                .ToList();

            var result = attributes.Select(x => x.Description);
            List<string> asList = attributes.Select(x => x.Description).ToList();

            return asList;

        }
        public static string GetDescription(Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
        public static T GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(DescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a => ((DescriptionAttribute)a.Att)
                                .Description == description).SingleOrDefault();
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }

        public static string GetDescriptionFromEnumValue(Enum value)
        {
            DescriptionAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
        
        public void SQL_ConnectDatabase()
        {
            string connetionString;
            SqlConnection cnn;
            connetionString = @"Data Source=DESKTOP-4MMBVA4\SQLEXPRESS;Initial Catalog=Sensor_Result;User ID=sa_admin;Password=P@ssw0rd";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            //MessageBox.Show("Connection Open!");
            //cnn.Close();
        }
        //string myConnectionString = "server=localhost;database=testDB;uid=root;pwd=abc123;";
        public static void MySQL_ConnectDatabase()
        {
            string server = GetLocalIPAddress();
            MySqlConnection connection;
            string database = "sensor_database";
            string user = "root";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;
            
            connectionString = String.Format("server={0};port={1};user id={2}; password={3}" +
                "; database={4}; SslMode={5}", server, port, user, password, database, sslM);

            connection = new MySqlConnection(connectionString);
            
            try
            {
                connection.Open();
                connection.Close();
                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message + connectionString);
            }
        }

        public static bool Check_MySQL_Connect()
        {
            bool checkresult = false;
            string server = GetLocalIPAddress();
            MySqlConnection connection;
            string database = "sensor_database";
            string user = "root";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;

            connectionString = String.Format("server={0};port={1};user id={2}; password={3}" +
                "; database={4}; SslMode={5}", server, port, user, password, database, sslM);

            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                connection.Close();
                checkresult = true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message + connectionString);
            }
            return checkresult;
        }


        public static void MySQL_ConnectDatabase_test(string connection_string_input)
        {
            //string server = "Localhost_test";

            //string server = "103.224.212.219";
            //string server = "192.168.0.6";
            //string server = "raspberrypi";
            
            //string server = "Localhost via UNIX socket";
            MySqlConnection connection;
            string server = GetLocalIPAddress();
            string database = "sensor_database";
            string user = "root";
            //string password = "admin";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;

            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", server, port, user, password, database, sslM);

            /*
            connectionString = String.Format("server={0};port={1};user id={2};password={3}" +
                "; database={4}", server, port, user, password, database);
            */

            connection = new MySqlConnection(connection_string_input);

            try
            {
                connection.Open();
                MessageBox.Show("successful connection");
                connection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message + connectionString);
            }
        }


        public static int MySql_Insert_Batch( string IPAddress_varinput,string product_varinput
            , int total_interval_varinput, string time_interval_varinput, int number_perinterval_varinput, string Temperature_Var_varinput)
        {
            //MySqlConnection connection;
            //string server = "localhost";
            //string server = "localhost";
            string server = GetLocalIPAddress();
            //string server = "192.168.0.6";

            string database = "sensor_database";
            string user = "root";
            //string password = "admin";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;

            //connectionString = String.Format("server={0};port={1};user id={2}; password={3}" +
            //    "; database={4}; SslMode={5}", server, port, user, password, database, sslM);


            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", server, port, user, password, database, sslM);
            
            using (var connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("Insert_Batch", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("IPAddress_var", IPAddress_varinput));
                command.Parameters.Add(new MySqlParameter("Product_var", product_varinput));
                command.Parameters.Add(new MySqlParameter("Total_Interval_var", total_interval_varinput));
                command.Parameters.Add(new MySqlParameter("Time_Interval_var", time_interval_varinput));
                command.Parameters.Add(new MySqlParameter("Number_Per_Interval_var", number_perinterval_varinput));
                command.Parameters.Add(new MySqlParameter("Temperature_Var", Temperature_Var_varinput));
                // Add parameters
                command.Parameters.Add(new MySqlParameter("?Out_Result_Batch_ID", MySqlDbType.VarChar));
                command.Parameters["?Out_Result_Batch_ID"].Direction = ParameterDirection.Output;


                command.Connection.Open();
                var result = command.ExecuteNonQuery();

                //cmd.ExecuteNonQuery();
                //int lastInsertID = Convert.ToInt32(command.Parameters["Result_Batch_ID"].Value);
                //string str_batch = (string)command.Parameters["?Out_Result_Batch_ID"].Value;
                
                //int PG = (int)command.Parameters["?Out_Result_Batch_ID"].Value;

                //int lastInsertID = (int)command.Parameters["?Out_Result_Batch_ID"].Value;
                //int lastInsertID = (int)command.Parameters["?Out_Result_Batch_ID"].Value;

                //int lastInsertID = 0;
                int lastInsertID  = Int32.Parse( (string)command.Parameters["?Out_Result_Batch_ID"].Value);
                //int lastInsertID = Convert.ToInt32(cmd.Parameters["@fileid"].Value);
                command.Connection.Close();

                return lastInsertID;
            }

        }
        public static void MySql_Insert_Measure(int Batch_ID_varinput, int PerBatch_ID_varinput, float measure_result_varinput, DateTime Created_On_varinput, int IsAverage_varinput)
        {
            //string server = "localhost";
            //MySqlConnection connection;
            //string server = "localhost";
            string server = GetLocalIPAddress();

            //string server = "192.168.0.6";
            
            string database = "sensor_database";
            string user = "root";
            //string password = "admin";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            
            string connectionString;

            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", server, port, user, password, database, sslM);


            using (var connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("Insert_Measure", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("Batch_ID_var", Batch_ID_varinput));
                command.Parameters.Add(new MySqlParameter("PerBatchID_var", PerBatch_ID_varinput));
                command.Parameters.Add(new MySqlParameter("measure_result_var", measure_result_varinput));
                command.Parameters.Add(new MySqlParameter("Created_On_var", Created_On_varinput));
                command.Parameters.Add(new MySqlParameter("IsAverage_var", IsAverage_varinput));


                command.Connection.Open();
                var result = command.ExecuteNonQuery();
                command.Connection.Close();
            }

        }
        public static List<SQL_Data_Config> MySql_Get_DataConfig(string IP_Address_varinput)
        {
            //MySqlConnection connection;
            //string server = "localhost";
            //string server = "192.168.0.4";
            //string server = "192.168.0.6";

            string database = "sensor_database";
            string user = "root";
            //string password = "admin";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;
            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", IP_Address_varinput, port, user, password, database, sslM);

            //Sql_Measure_Batch query_batch = new Sql_Measure_Batch();

            List<SQL_Data_Config> List_Data_Config = new List<SQL_Data_Config> { };


            using (var connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("Get_Parameter", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("Parameter_var", "0"));
                command.Connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        SQL_Data_Config data_Config = new SQL_Data_Config(1, "1", 1);
                        data_Config.Config_Id = (Convert.ToInt32(reader["id"]));
                        data_Config.Config_Param = reader["Parameter"].ToString();
                        data_Config.Config_Value = double.Parse(reader["value"].ToString());

                        List_Data_Config.Add(data_Config);
                    }

                }
                command.Connection.Close();


                
            }

            return List_Data_Config;
        }

        public static void Update_DataConfig(string IP_Address_varinput, string parameter, string value)
        {
            string database = "sensor_database";
            string user = "root";
            //string password = "admin";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;
            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", IP_Address_varinput, port, user, password, database, sslM);

            //Sql_Measure_Batch query_batch = new Sql_Measure_Batch();

            List<SQL_Data_Config> List_Data_Config = new List<SQL_Data_Config> { };


            using (var connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("Update_Parameter", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("Parameter_var", parameter));
                command.Parameters.Add(new MySqlParameter("Value_var", value));

                command.Connection.Open();
                command.ExecuteReader();

            }

        }


        public static void Update_ErrorCode(string IP_Address_varinput, int Batch_ID, string Error_Code)
        {
            string database = "sensor_database";
            string user = "root";
            //string password = "admin";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;
            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", IP_Address_varinput, port, user, password, database, sslM);

            using (var connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("Update_Error", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("Batch_id", Batch_ID));
                command.Parameters.Add(new MySqlParameter("Error_Code", Error_Code));

                command.Connection.Open();
                command.ExecuteReader();

            }

        }


        public static void Update_FinishBatch(string IP_Address_varinput, int Batch_ID, string Error_Code)
        {
            string database = "sensor_database";
            string user = "root";
            //string password = "admin";
            string password = "raspberry";
            string port = "3306";
            string sslM = "none";
            string connectionString;
            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}", IP_Address_varinput, port, user, password, database, sslM);

            using (var connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("Update_Finish_Batch", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new MySqlParameter("Batch_id", Batch_ID));

                command.Connection.Open();
                command.ExecuteReader();

            }

        }


        public static void Callbeep()
        {
            /*
            ScriptEngine engine = Python.CreateEngine();
            engine.ExecuteFile(@"/home/pi/Desktop/Final/sound.py");
            */
            string appArgs = "";
            string appPath = @"/home/pi/Desktop/Final/Sound.py";
            Process proc = new Process();
            //ProcessStartInfo si = new ProcessStartInfo(appPath, appArgs);
            ProcessStartInfo si = new ProcessStartInfo(appPath);
            si.FileName = "python";
            si.Arguments = "/home/pi/Desktop/Final/Sound.py";
            si.UseShellExecute = true;     // Required for UAC elevation.
            proc.StartInfo = si;
            proc.Start();
            proc.WaitForExit();

            /*
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "/home/pi/Fio/to/python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
            */
        }

       
    }
}
