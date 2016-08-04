using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PCI8102_W32;
using System.Timers;
using System.IO;

namespace Camber_Measurement_System
{
    static class motion_head
    {
        
        // Her 10 ms de bir motion satus, position, ve io_status durumlarını oku, sıradışı durumları bildir.
        // hareket ederken, motion satut, position ve io_status durumlarını kontrol et.
        // limit switch ve alarm durumlarında hareketi durdur.
        // alarm var ise alarm flag'ini 1 yap
        // fonksiyonlardan hata dönerse alarm flag'ini 1 yap


        public static int error_flag = 0; // 0 = Ok, 1 = Error
        public static decimal position_pulse = 0;
        public static decimal position_mm = 0;
        //static int stop_read_position = 0;
        static decimal target_pos_pulse = 0;
        static System.Windows.Forms.Timer timer_of_pos = null;
        public static string motion_status = "";
        public static string io_status_str = "";
        public static bool emg_stop_flag = false;


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

        public static decimal[] Pos_to_Pulse(decimal Pos)
        {
            decimal error_code = 0; // 0 = error, 1 = OK
            decimal Pulse;
            decimal[] return_array = new decimal[2] { 0, 0 };
            decimal ratio = 0;

            try
            {
                ratio = Properties.Settings.Default.TDpR / Properties.Settings.Default.SPpR;
                Pulse = Convert.ToDecimal(Convert.ToInt32(Math.Round(Pos / ratio)));
                error_code = 1;
            }
            catch (Exception ex)
            {
                string error_string = text_file_list[46] + "\n\r" + ex;
                yazici.durum_yazici(error_string);
                //MessageBox.Show(error_string);
                throw;
            } 

            return_array[0] = error_code;
            return_array[1] = Pulse;
            return return_array;  
        }

        public static string[] io_status()
        {
            ushort io_status = 0;
            string[] return_array = new string[2] { "Error", "" };
            io_status_str = "";
            Int16 AxisNo = 0;
            try
            {
                motion._8102_get_io_status(AxisNo, ref io_status);
                string binValue = Convert.ToString(io_status, 2);

                /*if ((binValue.Length >= 15) && (binValue[14] == '1'))
                {
                    io_status_str += "Servo-ON output status\n\r";
                    Form1._Form1.servo_status("Servo-ON");
                }
                if ((binValue.Length >= 15) && (binValue[14] == '0'))
                {
                    Form1._Form1.servo_status("Servo-OFF");
                    error_flag = 1;
                }*/
                if ((binValue.Length >= 14) && (binValue[13] == '1'))
                {
                    //io_status_str += "In-Position signal input\n\r";
                }
                if ((binValue.Length >= 13) && (binValue[12] == '1'))
                {
                    //io_status_str += "Slow Down signal input\n\r";
                }
                if ((binValue.Length >= 12) && (binValue[11] == '1'))
                {
                    //io_status_str += "Latch signal input\n\r";
                }
                if ((binValue.Length >= 11) && (binValue[10] == '1'))
                {
                    //io_status_str += "Clear signal\n\r";
                }
                if ((binValue.Length >= 10) && (binValue[9] == '1'))
                {
                    //io_status_str += "Index signal\n\r";
                }
                if ((binValue.Length >= 9) && (binValue[8] == '1'))
                {
                    //io_status_str += "ERC pin output\n\r";
                }
                if ((binValue.Length >= 8) && (binValue[7] == '1'))
                {
                    //io_status_str += "PSC signal input\n\r";
                }
                if ((binValue.Length >= 7) && (binValue[6] == '0'))
                {
                    //io_status_str += "EMG Status\n\r";
                    Form1._Form1.emg_status(text_file_list[47]);
                    //error_flag = 1;
                }
                if ((binValue.Length >= 7) && (binValue[6] == '1'))
                {
                    Form1._Form1.emg_status(text_file_list[48]);
                }
                if ((binValue.Length >= 6) && (binValue[5] == '1'))
                {
                    //io_status_str += "DIR Ouput\n\r";
                }
                if ((binValue.Length >= 5) && (binValue[4] == '1'))
                {
                    //io_status_str += "Origin Switch\n\r";
                }
                if ((binValue.Length >= 4) && (binValue[3] == '0'))
                {
                    //io_status_str += "Negative Limit Switch\n\r";
                    Form1._Form1.MEL_status(text_file_list[47]);
                }
                if ((binValue.Length >= 4) && (binValue[3] == '1'))
                {
                    //io_status_str += "Negative Limit Switch\n\r";
                    Form1._Form1.MEL_status(text_file_list[48]);
                }
                if ((binValue.Length >= 3) && (binValue[2] == '0'))
                {
                    //io_status_str += "Positive Limit Switch\n\r";
                    Form1._Form1.PEL_status(text_file_list[47]);
                }
                if ((binValue.Length >= 3) && (binValue[2] == '1'))
                {
                    Form1._Form1.PEL_status(text_file_list[48]);
                }
                if ((binValue.Length >= 2) && (binValue[1] == '1'))
                {
                    //io_status_str += "Alarm Signal\n\r";
                    Form1._Form1.alarm_status(text_file_list[48]);
                    //error_flag = 1;
                }
                if ((binValue.Length >= 2) && (binValue[1] == '0'))
                {
                    Form1._Form1.alarm_status(text_file_list[47]);
                }
                if ((binValue.Length >= 1) && (binValue[0] == '1'))
                {
                    //io_status_str += "RDY pin input\n\r";
                }
                return_array[0] = "OK!";
                return_array[1] = binValue;
            }
            catch (Exception ex)
            {
                string error_string = "io_status Read Error\n\r" + ex;
                yazici.durum_yazici(error_string);
                //MessageBox.Show(error_string);
                throw;
            }
            

                return return_array;
        }

        public static decimal[] Pulse_to_Pos(decimal Pulse)
        {
            decimal Pos;
            decimal ratio = 0;
            decimal[] return_array = new decimal[2] { 0, 0 };
            int error_code = 0;// 0 = error, 1 = OK
            try
            {
                ratio = Properties.Settings.Default.TDpR / Properties.Settings.Default.SPpR;
                Pos = Convert.ToDecimal(Pulse * ratio);
                error_code = 1;
            }
            catch (Exception ex)
            {
                string error_string = text_file_list[49] + "\n\r" + ex;
                yazici.durum_yazici(error_string);
                MessageBox.Show(error_string);
                error_flag = 1;
                throw;
            }
            return_array[0] = error_code;
            return_array[1] = Pos;
            return return_array;
        }

        static string Error_Code_to_String(Int32 Ret)
        {
            string error_string = "";
            if (Ret == -10000)
            {
                error_string = "Error Card number.";
                error_flag = 1;
            }

            else if (Ret == -10001)
            {
                error_string = "Error operation system version.";
                error_flag = 1;
            }

            else if (Ret == -10002)
            {
                error_string = "Error card’s ID conflict.";
                error_flag = 1;
            }

            else if (Ret == -10200)
            {
                error_string = "Error other process exist.";
                error_flag = 1;
            }

            else if (Ret == -10201)
            {
                error_string = "Error card not found.";
                error_flag = 1;
            }

            else if (Ret == -10202)
            {
                error_string = "Error Open driver failed.";
                error_flag = 1;
            }

            else if (Ret == -10203)
            {
                error_string = "Error ID mapping failed.";
                error_flag = 1;
            }

            else if (Ret == -10205)
            {
                error_string = "Error trigger type.";
                error_flag = 1;
            }

            else if (Ret == -10206)
            {
                error_string = "Error event already enabled.";
                error_flag = 1;
            }

            else if (Ret == -10207)
            {
                error_string = "Error event not enable yet.";
                error_flag = 1;
            }

            else if (Ret == -10208)
            {
                error_string = "Error on board FIFO full.";
                error_flag = 1;
            }

            else if (Ret == -10209)
            {
                error_string = "Error unknown command type.";
                error_flag = 1;
            }

            else if (Ret == -10210)
            {
                error_string = "Error unknow chip type.";
                error_flag = 1;
            }

            else if (Ret == -10211)
            {
                error_string = "Error card not initial.";
                error_flag = 1;
            }

            else if (Ret == -10212)
            {
                error_string = "Error position out of range.";
                error_flag = 1;
            }

            else if (Ret == -10213)
            {
                error_string = "Error motion busy.";
                error_flag = 1;
            }

            else if (Ret == -10214)
            {
                error_string = "Error speed error.";
                error_flag = 1;
            }

            else if (Ret == -10215)
            {
                error_string = "Error slow down point.";
                error_flag = 1;
            }

            else if (Ret == -10216)
            {
                error_string = "Error axis range error.";
                error_flag = 1;
            }

            else if (Ret == -10217)
            {
                error_string = "Error compare parameter error.";
                error_flag = 1;
            }

            else if (Ret == -10218)
            {
                error_string = "Error compare method.";
                error_flag = 1;
            }

            else if (Ret == -10219)
            {
                error_string = "Error axis already stop.";
                error_flag = 1;
            }

            else if (Ret == -10220)
            {
                error_string = "Error axis INT wait failed.";
                error_flag = 1;
            }

            else if (Ret == -10221)
            {
                error_string = "Error user code write failed.";
                error_flag = 1;
            }

            else if (Ret == -10222)
            {
                error_string = "Error array size exceed.";
                error_flag = 1;
            }

            else if (Ret == -10223)
            {
                error_string = "Error factor number.";
                error_flag = 1;
            }

            else if (Ret == -10224)
            {
                error_string = "Error enable range.";
                error_flag = 1;
            }

            else if (Ret == -10225)
            {
                error_string = "Error auto accelerate time.";
                error_flag = 1;
            }

            else if (Ret == -10226)
            {
                error_string = "Error dwell time.";
                error_flag = 1;
            }

            else if (Ret == -10227)
            {
                error_string = "Error dwell distance.";
                error_flag = 1;
            }

            else if (Ret == -10228)
            {
                error_string = "Error new position.";
                error_flag = 1;
            }

            else if (Ret == -10229)
            {
                error_string = "Error motion not in running.";
                error_flag = 1;
            }

            else if (Ret == -10230)
            {
                error_string = "Error velocity change time.";
                error_flag = 1;
            }

            else if (Ret == -10231)
            {
                error_string = "Error speed target.";
                error_flag = 1;
            }

            else if (Ret == -10232)
            {
                error_string = "Error velocity percent.";
                error_flag = 1;
            }

            else if (Ret == -10233)
            {
                error_string = "Error postion change backward.";
                error_flag = 1;
            }

            else if (Ret == -10234)
            {
                error_string = "Error counter number.";
                error_flag = 1;
            }

            return error_string;

        }

        public static int Function_Result(Int32 Ret)
        {
            int error = 0; // 0 error, 1 = OK
            if (Ret != 0)
            {
                string error_string = "Function Fail, ErrorCode " + Ret.ToString() + "\n\r" + Error_Code_to_String(Ret);
                yazici.durum_yazici(error_string);
                //MessageBox.Show(error_string);
            }
            else
            {
                error = 1;
            }
            return error;
            
        }

        public static int set_home_position(Int32 position_)
        {
            int error_code = 0; // 0 = error, 1 = OK
            Int16 AxisNo = 0;
            Function_Result(motion._8102_set_position(AxisNo, position_));
            if (error_flag == 0)
            {
                error_code = 1;
                //motion._8102_tv_move(0, 1000, 10000, 1000);

            }
            return error_code;
        }

        public static int motion_init()
        {

            load_language();
            int error_code = 0; // 0 = error, 1 = OK
            int error_code_2 = 0; // 0 = error, 1 = OK
            Int16 Ret = 0;
            Int16 AxisNo = 0;
            UInt16 CardID_InBit = 0;
            Int16 pls_outmode = 3;  //Inerrupt at rising edge, positive direction is 0v
            Int16 pls_iptmode = 0;  //Set 1X A/B mode
            Int16 pls_logic = 0;    //Not inverse direction
            Int16 Src = 0;          //0:External signal feedback ; 1:Command pulse
            Int16 alm_logic = 1;    //0:active LOW ; 1:active HIGH
            Int16 alm_mode = 0;     //0:motor immediately stops ; 1:motor decelerates then stops
            Int16 on_off = 1;       //0:servo on ; 1:servo off
            Int16 Manual_ID = 0;    //0:sequence of PCI slot ; 1:on board DIP switch (SW1) 
            Int16 limit_logic = 0;   // 0: normal open; 1: normal closed
            Int16 limit_mode = 0;   // 0: Stop immediately; 1: Slow down then stop

            error_flag = 0;

            try
            {
                error_code_2 = 1;
                Ret = motion._8102_initial(ref CardID_InBit, Manual_ID);
            }
            catch (Exception ex)
            {

                string message = "Motion Library Initialization Function call error!\n\r" + ex;
                yazici.durum_yazici(message);
            }
            

            if (Ret == 0)  //Card Initialization
            {
                
                try
                {
                    error_code = 0;
                    //Set Motion Parameter
                    error_code += Function_Result(motion._8102_set_pls_outmode(AxisNo, pls_outmode));
                    error_code += Function_Result(motion._8102_set_pls_iptmode(AxisNo, pls_iptmode, pls_logic));
                    error_code += Function_Result(motion._8102_set_feedback_src(AxisNo, Src));
                    error_code += Function_Result(motion._8102_set_alm(AxisNo, alm_logic, alm_mode));
                    error_code += Function_Result(motion._8102_set_servo(AxisNo, on_off));
                    error_code += Function_Result(motion._8102_set_limit_logic(AxisNo, limit_logic));
                    error_code += Function_Result(motion._8102_set_limit_mode(AxisNo, limit_mode));
                    error_code += Function_Result(motion._8102_set_position(AxisNo, 0));
                }
                catch (Exception ex)
                {
                    string message = "Motion Library Parameter Setting Functions call error!\n\r" + ex;
                    yazici.durum_yazici(message);
                }

                if (error_code != 8)
                {
                    string message = text_file_list[51];
                    MessageBox.Show(message, text_file_list[52], MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                if ((error_flag == 0) && (error_code >= 8) && (error_code_2 == 1))
                {
                    try
                    {
                        int period_of_timer = 10;
                        timer_of_pos = new System.Windows.Forms.Timer();
                        timer_of_pos.Interval = period_of_timer;
                        timer_of_pos.Tick += new EventHandler(TimerTick);
                        timer_of_pos.Enabled = true;
                        timer_of_pos.Start();
                    }
                    catch (Exception ex)
                    {

                        string error_string = text_file_list[52] + "\n\r" + ex;
                        yazici.durum_yazici(error_string);
                        MessageBox.Show(error_string);
                        throw;
                    }
                    error_code = 1;
                    //motion._8102_tv_move(0, 1000, 10000, 1000);

                }
                else
                {
                    motion_close();
                }


            }
            else
            {
                motion_close();
                //Function_Result(motion._8102_close());
                //MessageBox.Show("Card Initialization Fail !! ");
            }

            return error_code;
        }

        public static void emg_stop()
        {
            emg_stop_flag = true;
        }

        public static int motion_close()
        {
            Int16 AxisNo = 0;
            Int16 on_off = 1;       //0:servo on ; 1:servo off
            int error_code = 0; // 0 = error, 1 = OK
            int error_code_2 = 0; // 0 = error, 1 = OK
            int error_code_3 = 0; // 0 = error, 1 = OK
            error_flag = 0;
            string message = "motion_head class, motion_close function started";
            yazici.durum_yazici(message);
            try
            {
                error_code = 1;
                Function_Result(motion._8102_emg_stop(AxisNo));
                Function_Result(motion._8102_set_servo(AxisNo, on_off));
            }
            catch (Exception ex)
            {
                
                message = "emg_stop and set_servo functions call error!" + ex;
                yazici.durum_yazici(message);              
                
            }

            try
            {
                error_code_2 = 1;
                Function_Result(motion._8102_close());
                
            }
            catch (Exception ex)
            {
                
                message = "motion_close functions call error!" + ex;
                yazici.durum_yazici(message);

            }
            

            if (error_flag == 0)
            {
                error_code_3 = 1;
            }

            if ((error_code == 1) && (error_code_2 == 1) && (error_code_3 == 1))
            {

            }
            else
            {
                error_code = 1;
                message = "Motion Close is not successful!";
                yazici.durum_yazici(message);
            }

            return error_code;
        }

        public static decimal[] read_position() // mm cinsinden pozisyonu ve hata durumunu döndürür.
        {
            int error_code = 0; // 0 = error, 1 = OK
            decimal[] return_of_read_position = { 0, 0 };
            Double position_double = 0;
            //error_flag = 0;
                Function_Result(motion._8102_get_position(0, ref position_double));
                position_pulse = Convert.ToDecimal(position_double);
                decimal[] return_of_func = Pulse_to_Pos(position_pulse);
                if (return_of_func[0] == 1)
                {
                    //Form1._Form1.update_cur_pos(return_of_func[1]);
                    return_of_read_position[1] = return_of_func[1];
                    if (error_flag == 0)
                    {
                        error_code = 1;
                    }
                }

                return_of_read_position[0] = error_code;
                return return_of_read_position;
        }

        public static int go_position(decimal Dist)
        {
            
            Int16 AxisNo = 0;
            int error_code = 0; // 0 = error, 1 = OK
            Int16 error_of_pos = 0;
            int motion_done_status = 0;
            double MaxPPS =  Convert.ToDouble(Properties.Settings.Default.MaxPPS);
            double StrVel = MaxPPS/10;
            double Tacc = Convert.ToDouble(Properties.Settings.Default.Ta)/1000;
            double Tdec = Convert.ToDouble(Properties.Settings.Default.Td)/1000;
            bool reach = false; // noktaya vardık mı?
            decimal[] return_of_func = Pos_to_Pulse(Dist);
            decimal[] return_of_read_position = read_position();
            int cntr = 0;
            if ((return_of_func[0] == 1) && (emg_stop_flag == false))
            {
                target_pos_pulse = return_of_func[1];
                double SVacc = MaxPPS/4;
                double SVdec = MaxPPS/4;
                Function_Result(motion._8102_start_sr_move(0, Convert.ToDouble(return_of_func[1]), StrVel, MaxPPS, Tacc, Tdec, SVacc, SVdec));
            }
            while (!reach)
            {
                motion_done_status = motion._8102_motion_done(AxisNo);
                cntr++;
                if (motion_done_status == 0)
                {
                    reach = true;
                    motion._8102_get_error_counter(AxisNo,ref error_of_pos);
                    /*if (Math.Abs(error_of_pos) <= 50)
                    {
                        reach = true;
                    }
                    else
                    {
                        Function_Result(motion._8102_start_tr_move(0, Convert.ToDouble(error_of_pos), StrVel, MaxPPS, Tacc, Tdec));
                        
                    }*/
                
                    
                }
                /*if (cntr > 20)
                {
                    MessageBox.Show(Convert.ToString(motion._8102_motion_done(AxisNo)));
                    if (motion._8102_motion_done(AxisNo) == 0)
                    {
                        MessageBox.Show("WHY FUCKING MOTION DONE BUT ALGORITHM STOPS?????");
                    }
                    reach = true;
                }*/
                //Thread.Sleep(10);
                if ((motion_done_status == 17) && (Dist > 0)) // positive switch
                {
                    reach = true;
                }

                else if ((motion_done_status == 18) && (Dist < 0)) // negative switch
                {
                    reach = true;
                }
                //else if (motion_status == "Normal stopped condition")
                //{
                //    reach = true;
                //}
                
            }
            
            if (error_flag == 0)
            {
                error_code = 1;


            }
            else
            {
                motion_close();
            }

            //while 
            return error_code;  
        }

        public static int go_to_home_position()
        {
            int error_code = 0; // 0 = error, 1 = OK


            return error_code;
        }

        static string motion_status_to_string(Int16 status)
        {
            string status_string = "";
            if (status == 0)
            {
                status_string = "Normal stopped condition";
            }
            else if (status == 1)
            {
                status_string = "Waiting for DR";
            }
            else if (status == 2)
            {
                status_string = "Waiting for CSTA input";
            }
            else if (status == 3)
            {
                status_string = "Waiting for an internal synchronous signal";
            }
            else if (status == 4)
            {
                status_string = "Waiting for another axis to stop";
            }
            else if (status == 5)
            {
                status_string = "Waiting for a completion of ERC timer";
            }
            else if (status == 6)
            {
                status_string = "Waiting for a completion of direction change timer";
            }
            else if (status == 7)
            {
                status_string = "Correcting backlash";
            }
            else if (status == 8)
            {
                status_string = "Wait PA/PB";
            }
            else if (status == 9)
            {
                status_string = "At FA speed";
            }
            else if (status == 10)
            {
                status_string = "At FL Speed";
            }
            else if (status == 11)
            {
                status_string = "Accelerating";
            }
            else if (status == 12)
            {
                status_string = "At FH Speed";
            }
            else if (status == 13)
            {
                status_string = "Decelerating";
            }
            else if (status == 14)
            {
                status_string = "Wait INP";
            }
            else if (status == 15)
            {
                status_string = "Others(Controlling Start)";
            }
            else if (status == 16)
            {
                status_string = "SALM";
            }
            else if (status == 17)
            {
                status_string = "SPEL";
            }
            else if (status == 18)
            {
                status_string = "SMEL";
            }
            else if (status == 19)
            {
                status_string = "SEMG";
            }
            else if (status == 20)
            {
                status_string = "SSTP";
            }
            else if (status == 21)
            {
                status_string = "SERC";
            }

            return status_string;
        }

        public static void emergency_stop()
        {
            Int16 AxisNo = 0;
            motion._8102_emg_stop(AxisNo);
        }

        public static void stop_ax()
        {
            Int16 AxisNo = 0;
            double tDec = 0.1;
            motion._8102_sd_stop(AxisNo, tDec);
        }
        /// <summary>
        /// BURAYA ERROR INTERRUPTLARI EKLENECEK
        /// </summary>
        /// <param name="state"></param>

        static void TimerTick(Object myObject, EventArgs myEventArgs)
        {
            if (Form1.closing_flag == false)
            {
                decimal[] return_of_func = { 0, 0 };
                return_of_func = read_position();
                if ((return_of_func[0] == 1))
                {
                    Int16 AxisNo = 0;
                    motion_status = motion_status_to_string(motion._8102_motion_done(AxisNo));
                    Form1._Form1.update_motion_status(motion_status);
                    Form1._Form1.update_cur_pos(return_of_func[1]);
                    // motion status ile pos ana ekrana yazılacak
                }
                else
                {
                    timer_of_pos.Stop();
                    timer_of_pos.Dispose();
                    string message = "Motion Status Read Error!";
                    yazici.durum_yazici(message);


                }
                string[] return_of_func_2 = new string[2] { "", "" };
                return_of_func_2 = io_status();
                if (return_of_func_2[0] == "OK!")
                {
                    //yazici.durum_yazici(return_of_func_2[1]);
                }
                else
                {
                    timer_of_pos.Stop();
                    timer_of_pos.Dispose();
                    string message = "IO Status Read Error!";
                    yazici.durum_yazici(message);
                }
            }
            else
            {
                timer_of_pos.Stop();
                try
                {
                    timer_of_pos.Dispose();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Timer error" + ex);
                }
                Application.Exit();
                
            }
        }
    }
}
