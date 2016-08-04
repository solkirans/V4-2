using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Camber_Measurement_System
{
    static class Sensor
    {

        public static UInt32 iSensor = 0;
        public static string lang_file = "sistem_tr.dat";
        public static List<string> text_file_list = new List<string>();

        public static void load_language()
        {

            string text_file = "";
            try
            {
                using (StreamReader sr = new StreamReader(lang_file, Encoding.Default))
                {
                    text_file = sr.ReadToEnd();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Language file could not be read:" + ex.Message);
                yazici.durum_yazici("Language file could not be read:" + ex.Message);
            }

            //MessageBox.Show(text_file);

            text_file_list = new List<string>(text_file.Split(new string[] { "\r\n" },
                           StringSplitOptions.RemoveEmptyEntries));
        }

        public static double Get_Data(Int32 filter_coeff, decimal offset_calib)
        {
            //bool error_flag = false;
            Int32 EXPECTED_BLOCK_SIZE = 0;
            if (filter_coeff>=1)
            {
                 EXPECTED_BLOCK_SIZE = filter_coeff*3;
            }
            else
            {
             
                
                EXPECTED_BLOCK_SIZE = 1;
                string string_error = " Filter coefficient error: " + Convert.ToString(filter_coeff + "/nFilter coefficient set to" + Convert.ToString(EXPECTED_BLOCK_SIZE));
                yazici.durum_yazici(string_error);
            }
            //EXPECTED_BLOCK_SIZE = filter_coeff*3; // sensör bir sebepten 3 veri gönderiyor, birisi ölçüm değeri, diğerleri sıfır, o yüzden 3 le çarpıyoruz.
            int[] raw_Data = new int[EXPECTED_BLOCK_SIZE];
            //int rawData[EXPECTED_BLOCK_SIZE];
            double[] scaled_Data = new double[EXPECTED_BLOCK_SIZE];
            double measured_data = 0;
            for (int i = 0; i < raw_Data.Length; i++)
            {
                raw_Data[i] = 9;
                scaled_Data[i] = 9;
            }
            // Sleep 10ms, allow other things to happen, ....
            bool bDone = false;
            int try_counter = 0;
            MEDAQLib.ERR_CODE Ret = MEDAQLib.ERR_CODE.ERR_NOERROR;
            while ((!bDone) && (try_counter < 30000))
            {
                Thread.Sleep(1);
                try_counter++;
                // Check whether there's enough data to read in.
                int currentlyAvailable = 0;
                Ret = MEDAQLib.DataAvail(iSensor, ref currentlyAvailable);
                Int32 expectedBlockSize = EXPECTED_BLOCK_SIZE;
                //double[] scaled_Data_trash = new double[currentlyAvailable];
                //int[] raw_Data_trash = new int[currentlyAvailable];
                //int counter_2 = 0; // 10 defa ölçüm alacağız ve bekleyeceğiz. Yoksa ölçümler hatalı geliyor.
                //while (counter_2 < 2)
                //{
                    //counter_2++;
                    //Thread.Sleep(5);
                    if (currentlyAvailable > EXPECTED_BLOCK_SIZE)
                    {
                        bDone = false;

                        Ret = MEDAQLib.Poll(iSensor, raw_Data, scaled_Data, expectedBlockSize);

                        if (Ret == MEDAQLib.ERR_CODE.ERR_NOERROR)
                        {
                            bDone = true;
                        }

                    }
                //}
                
            }
            //Console.WriteLine("Raw Data:");
            //Array.ForEach(raw_Data, x => Console.WriteLine(x));
            measured_data = scaled_Data.Sum();
            measured_data = (measured_data / (EXPECTED_BLOCK_SIZE / 3));
            if ((Convert.ToDecimal(measured_data) > Properties.Settings.Default.maxthickness) || (Convert.ToDecimal(measured_data) < Properties.Settings.Default.minthickness))
            {
                measured_data = 0;
            }
            //Console.WriteLine("Scaled Data:");

            //Array.ForEach(scaled_Data, x => Console.WriteLine(x));
            //Console.WriteLine(Convert.ToString(Ret));
            measured_data = measured_data + Convert.ToDouble(offset_calib);
            return measured_data;
        }

        public static double[] get_multiple_data(Int32 data_count, Int32 filter_coeff, decimal offset_calib)
        {
            double[] measured_values = new double[data_count];

            while (data_count > 0)
            {
                // DÜZELTİLECEK
                measured_values[data_count - 1] = Get_Data(filter_coeff, offset_calib);
                data_count--;
            }

            return measured_values;
        }

        private static string Error(String sError, MEDAQLib.ERR_CODE iErr)
        {
            StringBuilder sbSensorError = new StringBuilder(1024);
            MEDAQLib.GetError(iSensor, sbSensorError, (UInt32)sbSensorError.Capacity);
            string err_Str = "Error in function: " + sError + "\n" + sbSensorError;
            return err_Str;
        }

        private static MEDAQLib.ERR_CODE Open()
        {
            MEDAQLib.ERR_CODE iRet = MEDAQLib.ERR_CODE.ERR_NOERROR;
            // Tell MEDAQLib about interface to be used.
            iRet = MEDAQLib.SetParameterString(iSensor, "IP_Interface", "TCP/IP");
            iRet = MEDAQLib.SetParameterInt(iSensor, "IP_RemoteDataProtocol", 0);
            iRet = MEDAQLib.SetParameterInt(iSensor, "IP_DataPort", 1024);
            iRet = MEDAQLib.SetParameterString(iSensor, "IP_RemoteAddr", Properties.Settings.Default.Sensor_IP);
            if ((iRet = MEDAQLib.SetParameterInt(iSensor, "IP_EnableLogging", 1)) != MEDAQLib.ERR_CODE.ERR_NOERROR)
            {
                string err_str = Error("SetParameterInt (IP_EnableLogging, 1)", iRet);
                //MessageBox.Show(err_str);
                yazici.durum_yazici(err_str);
                return iRet;
            }

            if ((iRet = MEDAQLib.OpenSensorTCPIP(iSensor, Properties.Settings.Default.Sensor_IP)) != MEDAQLib.ERR_CODE.ERR_NOERROR)
            {
                string err_str = Error("OpenSensorTCPIP ( " + Properties.Settings.Default.Sensor_IP + " )", iRet);
                //MessageBox.Show(err_str);
                yazici.durum_yazici(err_str);
                return iRet;
            }
            return MEDAQLib.ERR_CODE.ERR_NOERROR;
        }

        public static int sensor_init()
        {
            int error_code = 0; // 0 = error, 1 = OK
            bool error_flag = false;

            load_language();
            int cntr = 0;
            while ((cntr<5) && (iSensor == 0)) // Try create sensor instances 10 times
            {
                iSensor = MEDAQLib.CreateSensorInstByName("CONTROLLER_CSP2008");
                //cntr++;
            }
            if (iSensor == 0)
            {
                StringBuilder string_of_error = new StringBuilder(1024);
                MEDAQLib.GetError(iSensor, string_of_error, (UInt32)string_of_error.Capacity);
                string error_string = text_file_list[44] + "\n" + Convert.ToString(string_of_error);
                yazici.durum_yazici(error_string);
                MessageBox.Show(error_string, text_file_list[45], MessageBoxButtons.OK, MessageBoxIcon.Error);
                error_flag = true;
            }
            if (error_flag == false)
            {
                //Console.WriteLine("Open sensor...");
                if (Open() != MEDAQLib.ERR_CODE.ERR_NOERROR)
                {
                    
                    StringBuilder string_of_error = new StringBuilder(1024);
                    MEDAQLib.GetError(iSensor, string_of_error, 1024);
                    string error_string = text_file_list[44] + "\n" + Convert.ToString(string_of_error);
                    yazici.durum_yazici(error_string);
                    MessageBox.Show(error_string, text_file_list[45], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //System.Windows.Forms.MessageBox.Show(error_string, System.Windows.Forms.MessageBoxIcon.Error);
                    MEDAQLib.ReleaseSensorInstance(iSensor);
                    error_flag = true;
                }
            }

            if (error_flag == true)
            {
                error_code = 0;
            }
            
            if (error_flag == false)
            {
                error_code = 1;
            }

            return error_code;
        }

        public static int close_sensor()
        {
            int error_code = 0; // 0 = error, 1 = OK
            MEDAQLib.ERR_CODE iRet = MEDAQLib.ERR_CODE.ERR_NOERROR;
            if (iSensor != 0)
            {
                iRet = MEDAQLib.CloseSensor(iSensor);
                if (iRet == MEDAQLib.ERR_CODE.ERR_NOERROR)
                {
                    error_code = 1;
                    MEDAQLib.ReleaseSensorInstance(iSensor);
                    iSensor = 0;
                }
                else
                {
                    string err_str = Error("close_sensor", iRet);
                    MessageBox.Show(err_str);
                    yazici.durum_yazici(err_str);

                }
            }
            return error_code;
        }
    }
}
