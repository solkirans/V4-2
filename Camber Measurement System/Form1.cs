using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

using PCI8102_W32;

namespace Camber_Measurement_System
{
    public partial class Form1 : Form
    {
        public static Form1 _Form1;

        bool tab_control_locked = true;
        int last_tab_index = 0;
        string date_time_format_for_test_folder = "ddnMMnyy-HHnmmnss";
        public static bool closing_flag = false;
        string lang_file = "gui_tr.dat";
        List<string> text_file_list = new List<string>();
        static object Missing = System.Reflection.Missing.Value;
        decimal[] pos_read;
        decimal[] thick_read;

        Thread measurement_session_thread;


        


        public Form1()
        {
            InitializeComponent();
            _Form1 = this;
        }
        
        #region GUISETTINGS

        public string textBox_offset_calib_value
        {
            get { return textBox_offset_calib.Text; }
            set { textBox_offset_calib.Text = value; }
        }

        public string textBox_filter_const_value
        {
            get { return textBox_filter_const.Text; }
            set { textBox_filter_const.Text = value; }
        } 

        public void fit_sizes()
        {

            int tab_width_offset = 14;
            int tab_height_offset = 34;
            //int start_button_width_offset = 90;
            //int stop_button_width_offset = 90;
            //int start_button_height_offset = 132;
            //int stop_button_height_offset = 104;
            int minimum_form_height = 480;
            int minimum_form_width = 640;
            tabControl1.Width = this.Width - tab_width_offset;
            tabControl1.Height = this.Height - tab_height_offset;
            tabControl2.Width = tabControl1.Width - tab_width_offset;
            tabControl2.Height = tabControl1.Height - tab_height_offset-100;
            button_save.Location = new Point(336, tabControl2.Height+50);



            //strt_measure.Location = new Point((this.Width - tab_width_offset - start_button_width_offset), (this.Height - tab_height_offset - start_button_height_offset));
            //stp_measure.Location = new Point((this.Width - tab_width_offset - stop_button_width_offset), (this.Height - tab_height_offset - stop_button_height_offset));
            //chart1.Size.Width = tabControl1.Width - tab_width_offset;
            /*chart1.Location = new Point(6, 6);
            chart1.Size = new Size(tabControl1.Width - tab_width_offset, tabControl1.Height - tab_height_offset);
            chart2.Location = chart1.Location;
            chart2.Size = chart1.Size;*/
            if (this.Height < minimum_form_height)
            {
                this.Height = minimum_form_height;
            }
            if (this.Width < minimum_form_width)
            {
                this.Width = minimum_form_width;
            }
        }

        public void manipulate_go_button(string state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(manipulate_go_button), state);
            }
            else
            {
                if (state == "off")
                    button_go.Enabled = false;
                else
                    button_go.Enabled = true;
            }

        }

        public void manipulate_find_edge_button(string state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(manipulate_find_edge_button), state);
            }
            else
            {
                if (state == "off")
                    button_find_start_of_plate.Enabled = false;
                else
                    button_find_start_of_plate.Enabled = true;
            }

        }

        public void update_cur_pos(decimal pos)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<decimal>(update_cur_pos), pos);
                }
                else
                {
                    this.label15.Text = pos.ToString("N4");
                }
            //}
        }

        public void alarm_status(string status)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string>(alarm_status), status);
                }
                else
                {

                    this.textBox_alarm_status.Text = status;
                }
            //}
        }

        public void emg_status(string status)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string>(emg_status), status);
                }
                else
                {
                    this.textBox_EMG_status.Text = status;
                }
            //}
        }

        public void PEL_status(string status)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string>(PEL_status), status);
                }
                else
                {
                    this.textBox_PEL_status.Text = status;
                }
            //}
        }

        public void MEL_status(string status)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string>(MEL_status), status);
                }
                else
                {
                    this.textBox_MEL_status.Text = status;
                }
            //}
        }

        private void label_adjustment()
        {
            if ((textBox_sensor_status.Text == text_file_list[54]) && (textBox_motion_control_card_status.Text == text_file_list[54]))
            {

                textBox_system_status.Text = text_file_list[56];
                textBox_system_status.Font = new Font(textBox_system_status.Font, FontStyle.Bold);
                Color clr_ = Color.FromKnownColor(KnownColor.YellowGreen);
                textBox_system_status.ForeColor = clr_;
                button_go.Enabled = true;
                button_find_start_of_plate.Enabled = true;
                strt_measure.Enabled = true;
                stp_measure.Enabled = true;
            }

            else
            {
                textBox_system_status.Text = text_file_list[55];
                textBox_system_status.Font = new Font(textBox_system_status.Font, FontStyle.Bold);
                Color clr_ = Color.FromKnownColor(KnownColor.Firebrick);
                textBox_system_status.ForeColor = clr_;
                button_go.Enabled = false;
                button_find_start_of_plate.Enabled = false;
                strt_measure.Enabled = false;
                stp_measure.Enabled = false;
                button_calc_strk_lngth.Enabled = false;
            }
        }

        public void lang_settings()
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

            this.Text = text_file_list[0];
            tabControl1.TabPages[0].Text = text_file_list[1];
            tabControl1.TabPages[1].Text = text_file_list[2];
            groupBox3.Text = text_file_list[3];
            label2.Text = text_file_list[4];
            comboBox_meas_style.Items[0] = text_file_list[5];
            comboBox_meas_style.Items[1] = text_file_list[6];
            label1.Text = text_file_list[7];
            label29.Text = text_file_list[8];
            label8.Text = text_file_list[9];
            label27.Text = text_file_list[10];
            label32.Text = text_file_list[11];
            label40.Text = text_file_list[12];
            label41.Text = text_file_list[13];
            label7.Text = text_file_list[14];
            rfrsh_btn.Text = text_file_list[15];
            strt_measure.Text = text_file_list[16];
            stp_measure.Text = text_file_list[17];
            groupBox1.Text = text_file_list[18];
            label14.Text = text_file_list[19];
            label18.Text = text_file_list[20];
            groupBox2.Text = text_file_list[21];
            label28.Text = text_file_list[22];
            button_go.Text = text_file_list[23];
            button_find_start_of_plate.Text = text_file_list[24];
            groupBox4.Text = text_file_list[25];
            label33.Text = text_file_list[26];
            label34.Text = text_file_list[27];
            label35.Text = text_file_list[28];
            label36.Text = text_file_list[29];
            label37.Text = text_file_list[30];
            label38.Text = text_file_list[31];
            label39.Text = text_file_list[32];
            tabControl2.TabPages[0].Text = text_file_list[33];
            label10.Text = text_file_list[34];
            label16.Text = text_file_list[35];
            label17.Text = text_file_list[36];
            label22.Text = text_file_list[37];
            label30.Text = text_file_list[38];
            label31.Text = text_file_list[39];
            label25.Text = text_file_list[40];
            label24.Text = text_file_list[41];
            label3.Text = text_file_list[42];
            button_calc_strk_lngth.Text = text_file_list[43];
            label6.Text = text_file_list[44];
            button_save.Text = text_file_list[45];
            tabControl2.TabPages[1].Text = text_file_list[46];
            label4.Text = text_file_list[47];
            button_folder_select.Text = text_file_list[48];
            label5.Text = text_file_list[49];
            label26.Text = text_file_list[50];
            label9.Text = text_file_list[51];
            label11.Text = text_file_list[83];
            groupBox8.Text = text_file_list[86];
            label42.Text = text_file_list[87];
            label43.Text = text_file_list[88];
            groupBox5.Text = text_file_list[89];
            button_calc_filter.Text = text_file_list[92];

        }

        public void set_auto_measurement(string status)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string>(set_auto_measurement), status);
                }
                else
                {
                    if (status == "enable")
                    {
                        this.strt_measure.Enabled = true;
                        this.textBox_resolution.Enabled = true;
                        this.comboBox_meas_style.Enabled = true;
                        this.tab_control_locked = false;
                        this.textBox_auto_meas_id.Enabled = true;
                    }
                    else if (status == "disable")
                    {
                        this.strt_measure.Enabled = false;
                        this.textBox_resolution.Enabled = false;
                        this.comboBox_meas_style.Enabled = false;
                        this.tab_control_locked = true;
                        this.textBox_auto_meas_id.Enabled = false;
                    }
                }
            //}
        }

        public void update_cur_thickness(decimal thickness_)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<decimal>(update_cur_thickness), thickness_);
                }
                else
                {
                    decimal new_thickness = decimal.Round(thickness_, 4, MidpointRounding.AwayFromZero);
                    this.label19.Text = thickness_.ToString("N4");
                }
            //}
        }

        public void update_motion_status(string status_)
        {
            //if (closing_flag == false)
            //{
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string>(update_motion_status), status_);
                }
                else
                {
                    this.textBox_Motion_status.Text = status_;
                }
            //}
        }

        public string get_measurement_id()
        {
            //if (closing_flag == false)
            //{
                int text_cmb = 3;
                this.Invoke((MethodInvoker)delegate()
                {
                    text_cmb = comboBox_meas_style.SelectedIndex;
                });
                if (text_cmb == 0)
                {
                    var text = (string)textBox_auto_meas_id.Invoke(new Func<string>(() => textBox_auto_meas_id.Text));
                    return text;
                }
                else
                {
                    var text = (string)textBox_manual_meas_id.Invoke(new Func<string>(() => textBox_manual_meas_id.Text));
                    return text;
                }
            //}
            //else
            //{
            //    return "";
            //}
            
            
        }

        /*public int get_measurement_style()
        {
            //if (closing_flag == false)
            //{
            string text_cmb = "";
            this.Invoke((MethodInvoker)delegate()
            {
                text_cmb = comboBox_meas_style.Text;
            });
            
            int num = (int)textBox_auto_meas_id.Invoke(new Func<string>(() => textBox_auto_meas_id.Text));
            return text;


        }*/

        public decimal get_trg_pos()
        {
            decimal return_val = 0;
            //if (closing_flag == false)
            //{
                decimal d;
                
                if (textBox_manual_distance.Text.Contains('.'))
                {
                    textBox_manual_distance.Text = textBox_manual_distance.Text.Replace('.', ',');
                }

                if (decimal.TryParse(textBox_manual_distance.Text, out d))
                {
                    return_val = Convert.ToDecimal(textBox_manual_distance.Text);
                }
                else
                {
                    MessageBox.Show(text_file_list[53]);
                }
            //}
            return return_val;
            
        }

        #endregion


        private void sensor_check()
        {
            if (Sistem.check_sensor_status() == 1)
            {
                textBox_sensor_status.Text = text_file_list[54];
                textBox_sensor_status.Font = new Font(textBox_sensor_status.Font, FontStyle.Regular);
                Color clr_ = Color.FromKnownColor(KnownColor.YellowGreen);
                textBox_sensor_status.ForeColor = clr_;
            }
            else
            {
                //MessageBox.Show("Sensor initialization failed!");
                textBox_sensor_status.Text = text_file_list[55];
                textBox_sensor_status.Font = new Font(textBox_sensor_status.Font, FontStyle.Bold);
                Color clr_ = Color.FromKnownColor(KnownColor.Firebrick);
                textBox_sensor_status.ForeColor = clr_;
            }
        }

        private void motion_check()
        {
            if (Sistem.check_motor_status() == 1)
            {

                textBox_motion_control_card_status.Text = text_file_list[54];
                textBox_motion_control_card_status.Font = new Font(textBox_motion_control_card_status.Font, FontStyle.Regular);
                Color clr_ = Color.FromKnownColor(KnownColor.YellowGreen);
                textBox_motion_control_card_status.ForeColor = clr_;
            }
            else
            {
                //MessageBox.Show("Motor initialization failed!");
                textBox_motion_control_card_status.Text = text_file_list[55];
                textBox_motion_control_card_status.Font = new Font(textBox_motion_control_card_status.Font, FontStyle.Bold);
                Color clr_ = Color.FromKnownColor(KnownColor.Firebrick);
                textBox_motion_control_card_status.ForeColor = clr_;
                //groupBox2.Visible = false;
            }
        }



        public void settings_writes()
        {
            textBox_sensorIP.Text = Properties.Settings.Default.Sensor_IP;
            //textBox_SmpInt.Text = Convert.ToString(Properties.Settings.Default.Sample_cnt);
            textBox_SPpR.Text = Convert.ToString(Properties.Settings.Default.SPpR);
            textBox_TDpR.Text = Convert.ToString(Properties.Settings.Default.TDpR);
            textBox_acc_time.Text = Convert.ToString(Properties.Settings.Default.Ta);
            textBox_dec_time.Text = Convert.ToString(Properties.Settings.Default.Td);
            textBox_MaxPPS.Text = Convert.ToString(Properties.Settings.Default.MaxPPS);
            textBox_maxthickness.Text = Convert.ToString(Properties.Settings.Default.maxthickness);
            textBox_minthickness.Text = Convert.ToString(Properties.Settings.Default.minthickness);
            textBox_stroke_length.Text = Convert.ToString(Properties.Settings.Default.Strokelength);
            textBox_edge_offset.Text = Convert.ToString(Properties.Settings.Default.offset_from_edge);
            textBox_folder_system_path.Text = Convert.ToString(Properties.Settings.Default.folder_path);
            textBox_main_folder_name.Text = Convert.ToString(Properties.Settings.Default.main_folder_name);
            comboBox_sensorlog_delete_interval.SelectedItem  = Convert.ToString(Properties.Settings.Default.delete_interval);
            textBox_resolution.Text = Convert.ToString(Properties.Settings.Default.point_quantity);
            comboBox_meas_style.SelectedIndex = Properties.Settings.Default.selected_style;
            textBox_filter_const.Text = Convert.ToString(Properties.Settings.Default.filter_coef);
            textBox_offset_calib.Text = Convert.ToString(Properties.Settings.Default.offset_calib);
            if (textBox_main_folder_name.TextLength == 0)
                textBox_full_path.Text = textBox_folder_system_path.Text;
            else
                textBox_full_path.Text = textBox_folder_system_path.Text + textBox_main_folder_name.Text + "\\";
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.Reload();
            }
            catch (Exception ex)
            {
                yazici.durum_yazici("Ayarlar yuklenirken hata olustu.\n\r" + ex);
            }

            Sistem.load_language();
            lang_settings();

            int a = Properties.Settings.Default.point_quantity;
            decimal b = Properties.Settings.Default.Strokelength;
            timer1.Start();
            tab_control_locked = false;
            yazici.durum_yazici("Form1_Load Started.");
            
            sensor_check();
            motion_check();         
            
            //Settings atamaları
            settings_writes();
            //Settings atamaları
            groupBox2.Enabled = false;
            fit_sizes();
            // form is fitted for screen resolution
            
            
            //label_adjustment();

            //Sistem.exceli_yarat("abc","C:\\test\\");
            //Sistem.bombelik_hesapla();
            //Sistem.excele_grafik_ekle();

            

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            fit_sizes();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Location = new Point(0, 0);
        }




        private void tabControl1_Selecting_1(object sender, TabControlCancelEventArgs e)
        {
            if (tab_control_locked == false)
            {

                TabPage current = (sender as TabControl).SelectedTab;
                //MessageBox.Show(Convert.ToString(current.Text));
                if (Convert.ToString(current.Text) == "Raw Result")
                {
                    //MessageBox.Show("RR");
                    //chart1.Series["Series1"].Points.Clear();


                    //chart1.ChartAreas[0].AxisX.Minimum = 0;
                    int length = Sistem.measured_point_list.Count();
                    double last_position = 0;
                    double prev_position = 0;
                    double min_thickness = 20;
                    double max_thickness = 0;
                    //chart1.Series["Series1"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Single;
                    for (int sayac = 0; sayac < length; sayac++)
                    {
                        measured_point _mp = Sistem.measured_point_list.ElementAt(sayac);
                        if (_mp.measured_thickness < min_thickness)
                        {
                            min_thickness = _mp.measured_thickness;
                        }

                        if (_mp.measured_thickness > max_thickness)
                        {
                            max_thickness = _mp.measured_thickness;
                        }

                        //chart1.Series["Series1"].Points.AddXY(_mp.measured_position, _mp.measured_thickness);
                        prev_position = last_position;
                        last_position = _mp.measured_position;
                    }
                    double _position = last_position + (last_position - prev_position);
                    //MessageBox.Show(Convert.ToString(chart1.ChartAreas["ChartArea1"].AxisX.GetType()));
                    //chart1.ChartAreas["ChartArea1"].AxisX.IsStartedFromZero = true;
                    //chart1.ChartAreas["ChartArea1"].AxisX.Maximum = last_position;

                    //chart1.ChartAreas["ChartArea1"].AxisX.Interval = 50;
                    //chart1.ChartAreas["ChartArea1"].AxisX.Interval = Math.Round(((last_position / 50) / 25),0) * 25;
                    /*chart1.ChartAreas["ChartArea1"].AxisY.Maximum = max_thickness;//* 1.02;// last_position + (last_position - prev_position);
                    chart1.ChartAreas["ChartArea1"].AxisY.Minimum = min_thickness;/// 1.02;// last_position + (last_position - prev_position);
                    chart1.ChartAreas["ChartArea1"].AxisX.RoundAxisValues();
                    chart1.ChartAreas["ChartArea1"].AxisX.Title = "Position";
                    chart1.ChartAreas["ChartArea1"].AxisY.Title = "Thickness";
                    chart1.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Arial", 16);
                    chart1.ChartAreas["ChartArea1"].AxisX.TitleForeColor = Color.FromKnownColor(KnownColor.Firebrick);
                    chart1.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Arial", 16);
                    chart1.ChartAreas["ChartArea1"].AxisY.TitleForeColor = Color.FromKnownColor(KnownColor.Firebrick);*/
                }

                if (Convert.ToString(current.Text) == "Processed Results")
                {
                    //MessageBox.Show("PR");
                    //chart2.Series["Series1"].Points.Clear();


                    //chart1.ChartAreas[0].AxisX.Minimum = 0;
                    int length = Sistem.measured_point_list.Count();
                    double last_position = 0;
                    double prev_position = 0;
                    double min_thickness = 20;
                    double max_thickness = 0;
                    //chart2.Series["Series1"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Single;
                    for (int sayac = 0; sayac < length; sayac++)
                    {
                        measured_point _mp = Sistem.measured_point_list.ElementAt(sayac);
                        if (_mp.measured_thickness < min_thickness)
                        {
                            min_thickness = _mp.measured_thickness;
                        }

                        if (_mp.measured_thickness > max_thickness)
                        {
                            max_thickness = _mp.measured_thickness;
                        }

                        //chart2.Series["Series1"].Points.AddXY(_mp.measured_position, _mp.measured_thickness);
                        //chart2.Series["Series2"].Points.AddXY(_mp.measured_position, _mp.measured_thickness);
                        prev_position = last_position;
                        last_position = _mp.measured_position;
                    }
                    double _position = last_position + (last_position - prev_position);
                    //MessageBox.Show(Convert.ToString(chart1.ChartAreas["ChartArea1"].AxisX.GetType()));
                    /*chart2.ChartAreas["ChartArea1"].AxisX.IsStartedFromZero = true;
                    chart2.ChartAreas["ChartArea1"].AxisX.Maximum = last_position;

                    chart2.ChartAreas["ChartArea1"].AxisX.Interval = 50;
                    //chart1.ChartAreas["ChartArea1"].AxisX.Interval = Math.Round(((last_position / 50) / 25),0) * 25;
                    chart2.ChartAreas["ChartArea1"].AxisY.Maximum = max_thickness;//* 1.02;// last_position + (last_position - prev_position);
                    chart2.ChartAreas["ChartArea1"].AxisY.Minimum = min_thickness;/// 1.02;// last_position + (last_position - prev_position);
                    chart2.ChartAreas["ChartArea1"].AxisX.RoundAxisValues();
                    chart2.ChartAreas["ChartArea1"].AxisX.Title = "Position";
                    chart2.ChartAreas["ChartArea1"].AxisY.Title = "Thickness";
                    chart2.ChartAreas["ChartArea1"].AxisX.TitleFont = new Font("Arial", 16);
                    chart2.ChartAreas["ChartArea1"].AxisX.TitleForeColor = Color.FromKnownColor(KnownColor.Firebrick);
                    chart2.ChartAreas["ChartArea1"].AxisY.TitleFont = new Font("Arial", 16);
                    chart2.ChartAreas["ChartArea1"].AxisY.TitleForeColor = Color.FromKnownColor(KnownColor.Firebrick);*/
                }
            }
            else
            {
                tabControl1.SelectTab(last_tab_index);
            }

            

        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            fit_sizes();
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void stp_measure_Click(object sender, EventArgs e)
        {
            yazici.durum_yazici("stp_measure_Click Started.");
            Sistem.stop_measurement_session();
            //yazici.durum_yazici("stp_measure_Click Started.");
            //motion_head.emg_stop();
            //motion_head.emergency_stop();
            //set_auto_measurement("enable");
        }


        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// Start Measure button
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        /// 

        private bool user_input_controls()
        {
            bool auto_measure_id_check = false;
            bool shift_check = false;
            bool operator_check = false;
            bool width_check = false;
            bool rollnumber_check = false;
            bool alloy_check = false;
            bool resolution_check = false;
            bool prd_line_check = false;
            string shift_str = "";
            string prd_line_str = "";
            string operator_str = "";
            string width_str = "";
            string roll_number_str = "";
            string alloy_str = "";
            string measurement_id = "";

            int d_int = 0;

            if (int.TryParse(textBox_resolution.Text, out d_int))
            {
                resolution_check = true;

            }
            else
            {
                MessageBox.Show(text_file_list[57]);
            }


            if (textBox_auto_meas_id.Text.Length >= 6)
            {
                
                auto_measure_id_check = true;
                measurement_id = textBox_auto_meas_id.Text;
                
            }
            else
            {
                MessageBox.Show(text_file_list[58]);
            }

            if (textBox_prd_line.Text.Length >= 1)
            {

                prd_line_check = true;
                prd_line_str = textBox_prd_line.Text;

            }
            else
            {
                MessageBox.Show(text_file_list[84]);
            }

            if (textBox_shift.Text.Length >= 1)
            {
                
                shift_check = true;
                shift_str = textBox_shift.Text;
                
            }
            else
            {
                MessageBox.Show(text_file_list[59]);
            }

            if (textBox_operator.Text.Length >= 1)
            {

                operator_check = true;
                operator_str = textBox_operator.Text;

            }
            else
            {
                MessageBox.Show(text_file_list[60]);
            }

            if (textBox_width.Text.Length >= 1)
            {

                width_check = true;
                width_str = textBox_width.Text;

            }
            else
            {
                MessageBox.Show(text_file_list[61]);
            }

            if (textBox_rollnumber.Text.Length >= 1)
            {

                rollnumber_check = true;
                roll_number_str = textBox_rollnumber.Text;

            }
            else
            {
                MessageBox.Show(text_file_list[62]);
            }

            if (textBox_alloy.Text.Length >= 1)
            {

                alloy_check = true;
                alloy_str = textBox_alloy.Text;

            }
            else
            {
                MessageBox.Show(text_file_list[63]);
            }


            if (auto_measure_id_check && shift_check && operator_check && width_check && rollnumber_check && alloy_check && resolution_check && prd_line_check)
            {
                return true;
            }


            return false;
        }

        private void strt_measure_Click(object sender, EventArgs e)
        {
            
            
            

            //Tuple<decimal[], decimal[]> meas_list_tuple = Excel_Test_Reader("Excel_test.xlsx");

            //pos_read = meas_list_tuple.Item1;
            //thick_read = meas_list_tuple.Item2;

            if (user_input_controls())
            {
                strt_measure.Enabled = false;
                /*comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;*/
                tab_control_locked = true;
                last_tab_index = tabControl1.TabIndex;
                stp_measure.Enabled = true;


                string measurement_id = textBox_auto_meas_id.Text;
                string prd_line_str = textBox_prd_line.Text;
                string shift_str = textBox_shift.Text;
                string operator_str = textBox_operator.Text;
                string width_str = textBox_width.Text;
                string roll_number_str = textBox_rollnumber.Text;
                string alloy_str = textBox_alloy.Text;
                Int32 filter_coeff = Convert.ToInt32(textBox_filter_const.Text);

                Sistem.exceli_kapat();
                yazici.durum_yazici("strt_measure_Click Started." + "ID:" + textBox_auto_meas_id.Text);
                Sistem.exceli_yarat(measurement_id, prd_line_str, textBox_full_path.Text);
                int sample_cnt_2 = Convert.ToInt32(textBox_resolution.Text);
                Properties.Settings.Default.point_quantity = sample_cnt_2;
                measurement_session_thread = new Thread((ThreadStart)delegate { Sistem.start_measurement_session(sample_cnt_2, shift_str, operator_str, width_str, roll_number_str, alloy_str, measurement_id, prd_line_str, filter_coeff); });
                measurement_session_thread.IsBackground = true;
                try
                {
                    measurement_session_thread.Start();
                }
                catch (Exception ex)
                {

                    yazici.durum_yazici("Measurement Start Error: Sample Count:" + sample_cnt_2 + "Exception:" + ex);
                }
            }

        }

        private void button_save_Click(object sender, EventArgs e)
        {

            yazici.durum_yazici("button_save_Click Started.");
            bool TDpR_check = false;
            bool SPpR_check = false;
            bool MaxPPS_check = false;
            bool maxthickness_check = false;
            bool minthickness_check = false;
            bool acc_time_check = false;
            bool dec_time_check = false;
            bool stroke_length_check = false;
            bool edge_offset_check = false;
            bool filter_coef_check = false;
            bool offset_calibration_check = false;

            decimal d;
            int d_int;


            // IP adresi yazılırken '.' yerine ',' yazıldıysa onu düzeltiyoruz.
            if (textBox_sensorIP.Text.Contains(','))
            {
                textBox_sensorIP.Text = textBox_sensorIP.Text.Replace(',', '.');
            }
            // IP adresi düzeltiminin sonu


            // ondalık değerler yazılırken ',' yerine '.' kullanıldıysa onları düzeltiyoruz.
            /*if (textBox_SmpInt.Text.Contains('.'))
            {
                textBox_SmpInt.Text = textBox_SmpInt.Text.Replace('.', ',');
            }
            */
            if (textBox_TDpR.Text.Contains('.'))
            {
                textBox_TDpR.Text = textBox_TDpR.Text.Replace('.', ',');
            }


            if (textBox_acc_time.Text.Contains('.'))
            {
                textBox_acc_time.Text = textBox_acc_time.Text.Replace('.', ',');
            }

            if (textBox_dec_time.Text.Contains('.'))
            {
                textBox_dec_time.Text = textBox_dec_time.Text.Replace('.', ',');
            }

            if (textBox_maxthickness.Text.Contains('.'))
            {
                textBox_maxthickness.Text = textBox_maxthickness.Text.Replace('.', ',');
            }

            if (textBox_minthickness.Text.Contains('.'))
            {
                textBox_minthickness.Text = textBox_minthickness.Text.Replace('.', ',');
            }

            if (textBox_stroke_length.Text.Contains('.'))
            {
                textBox_stroke_length.Text = textBox_stroke_length.Text.Replace('.', ',');
            }

            if (textBox_edge_offset.Text.Contains('.'))
            {
                textBox_edge_offset.Text = textBox_edge_offset.Text.Replace('.', ',');
            }

            if (textBox_offset_calib.Text.Contains('.'))
            {
                textBox_offset_calib.Text = textBox_offset_calib.Text.Replace('.', ',');
            }
            // ondalık değerlerin yazım hataları düzeltildi.



            /*if(int.TryParse(textBox_SmpInt.Text, out d_int))
            {
                SmpInt_check = true;
            }
            else
            {
                MessageBox.Show("Please enter a valid number for Sample Interval.");
            }
            */
            if (decimal.TryParse(textBox_TDpR.Text, out d))
            {
                TDpR_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[64]);
            }

            if (decimal.TryParse(textBox_acc_time.Text, out d))
            {
                acc_time_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[65]);
            }

            if (decimal.TryParse(textBox_dec_time.Text, out d))
            {
                dec_time_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[66]);
            }


            if (decimal.TryParse(textBox_maxthickness.Text, out d))
            {
                maxthickness_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[67]);
            }


            if (decimal.TryParse(textBox_minthickness.Text, out d))
            {
                minthickness_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[68]);
            }

            if (int.TryParse(textBox_SPpR.Text, out d_int))
            {
                SPpR_check = true;
                
            }
            else
            {
                MessageBox.Show(text_file_list[69]);
            }

            if (int.TryParse(textBox_MaxPPS.Text, out d_int))
            {
                MaxPPS_check = true;

            }
            else
            {
                MessageBox.Show(text_file_list[70]);
            }
            if (decimal.TryParse(textBox_stroke_length.Text, out d))
            {
                stroke_length_check = true;

            }
            else
            {
                MessageBox.Show(text_file_list[71]);
            }


            if (decimal.TryParse(textBox_edge_offset.Text, out d))
            {
                edge_offset_check = true;

            }
            else
            {
                MessageBox.Show(text_file_list[72]);
            }



            if (decimal.TryParse(textBox_offset_calib.Text, out d))
            {
                offset_calibration_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[90]);
            }
            if ((int.TryParse(textBox_filter_const.Text, out d_int)) && (Convert.ToInt32(textBox_filter_const.Text) > 0) && (Convert.ToInt32(textBox_filter_const.Text) < 16000))
            {
                    filter_coef_check = true;

            }
            else
            {
                MessageBox.Show(text_file_list[91]);
            }

            if ((filter_coef_check) && (offset_calibration_check) && (edge_offset_check == true) && (TDpR_check == true) && (SPpR_check == true) && (MaxPPS_check == true) && (maxthickness_check == true) && (minthickness_check == true) && (dec_time_check == true) && (acc_time_check == true) && (stroke_length_check == true))
            {
                DialogResult dialogResult = MessageBox.Show(text_file_list[73], text_file_list[74], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Properties.Settings.Default.Sensor_IP = textBox_sensorIP.Text;
                    //Properties.Settings.Default.Sample_cnt = Convert.ToInt32(textBox_SmpInt.Text);
                    Properties.Settings.Default.SPpR = Convert.ToInt32(textBox_SPpR.Text);
                    Properties.Settings.Default.Ta = Convert.ToDecimal(textBox_acc_time.Text);
                    Properties.Settings.Default.Td = Convert.ToDecimal(textBox_dec_time.Text);
                    Properties.Settings.Default.TDpR = Convert.ToDecimal(textBox_TDpR.Text);
                    Properties.Settings.Default.MaxPPS = Convert.ToInt32(textBox_MaxPPS.Text);
                    Properties.Settings.Default.maxthickness = Convert.ToDecimal(textBox_maxthickness.Text);
                    Properties.Settings.Default.minthickness = Convert.ToDecimal(textBox_minthickness.Text);
                    Properties.Settings.Default.Strokelength = Convert.ToDecimal(textBox_stroke_length.Text);
                    Properties.Settings.Default.offset_from_edge = Convert.ToDecimal(textBox_edge_offset.Text);
                    Properties.Settings.Default.folder_path = textBox_folder_system_path.Text;
                    Properties.Settings.Default.main_folder_name = textBox_main_folder_name.Text;
                    Properties.Settings.Default.delete_interval = Convert.ToInt32(comboBox_sensorlog_delete_interval.SelectedItem);
                    Properties.Settings.Default.filter_coef = Convert.ToInt32(textBox_filter_const.Text);
                    Properties.Settings.Default.offset_calib = Convert.ToDecimal(textBox_offset_calib.Text);
                    Properties.Settings.Default.Save();
                }
            }
            
        }

        private void rfrsh_btn_Click(object sender, EventArgs e)
        {
            yazici.durum_yazici("rfrsh_btn_Click Started.");
            Sistem.close_motion();
            Sistem.close_sensor();
            sensor_check();
            motion_check();
            label_adjustment();
            motion_head.emg_stop_flag = false;
        }

        private void button_go_Click(object sender, EventArgs e)
        {
            Sistem.exceli_kapat();
            yazici.durum_yazici("button_go_Click Started.");
            decimal trg_pos = get_trg_pos();
            //Sistem.go_position_and_measure(get_trg_pos());
            Sistem.exceli_yarat(textBox_manual_meas_id.Text, textBox_prd_line.Text, textBox_full_path.Text);
            Sistem.excel_flag = true;
            int filter_coeff = Convert.ToInt32(textBox_filter_const.Text);
            decimal offset_calib = Properties.Settings.Default.offset_calib;
            Thread FirstThread = new Thread((ThreadStart)delegate { Sistem.go_position_and_measure(trg_pos, true, filter_coeff,offset_calib, 5000); });
            FirstThread.IsBackground = true;
            FirstThread.Start();
        }

        private void button_find_start_of_plate_Click(object sender, EventArgs e)
        {
            yazici.durum_yazici("button_find_start_of_plate_Click Started.");
            decimal offset_calib = Properties.Settings.Default.offset_calib;
            DialogResult dialogResult = MessageBox.Show(text_file_list[75], text_file_list[76], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            int filter_coeff = Convert.ToInt32(textBox_filter_const.Text);
            if (dialogResult == DialogResult.Yes)
            {
                Thread FirstThread = new Thread((ThreadStart)delegate { Sistem.find_plate_start("p", true, offset_calib,filter_coeff); });
                FirstThread.IsBackground = true;
                FirstThread.Start();
                //Sistem.find_plate_start("p");

            }
            else if (dialogResult == DialogResult.No)
            {
                Thread FirstThread = new Thread((ThreadStart)delegate { Sistem.find_plate_start("n", true, offset_calib, filter_coeff); });
                FirstThread.IsBackground = true;
                FirstThread.Start();
                //Sistem.find_plate_start("n");

            }
        }

        private void label_thickness_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (comboBox_meas_style.SelectedIndex == 0) // if automatic
            {
                groupBox2.Enabled = false;
            }
            else
            {
                groupBox2.Enabled = true;
            }
            Properties.Settings.Default.selected_style = comboBox_meas_style.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void button_calc_strk_lngth_Click(object sender, EventArgs e)
        {
            button_calc_strk_lngth.Enabled = false;
            button_save.Enabled = false;
            last_tab_index = tabControl1.TabIndex;
            tab_control_locked = true;
            motion_head.go_position(-1000000);
            motion_head.set_home_position(0);
            motion_head.go_position(1000000);
            decimal[] pos = motion_head.read_position();
            if (pos[0] == 1)
            {
                decimal pos_ = pos[1];
                DialogResult dialogResult = MessageBox.Show(text_file_list[77] + pos_ + "mm.\n\r" + text_file_list[78], text_file_list[79], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Properties.Settings.Default.Strokelength = pos_;
                    textBox_stroke_length.Text = Convert.ToString(pos_);

                }
            }
            button_calc_strk_lngth.Enabled = true;
            button_save.Enabled = true;
            tab_control_locked = false;
        }

        private void button_folder_select_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                DateTime time = DateTime.Now;
                string tarih = time.ToString(date_time_format_for_test_folder);
                string full_path = fbd.SelectedPath + "\\test" + tarih;
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(full_path);
                    try
                    {
                        StreamWriter sw = File.CreateText(full_path + "\\test.txt");
                        sw.Close();
                        File.Delete(full_path + "\\test.txt");
                        textBox_folder_system_path.Text = fbd.SelectedPath;

                    }
                    catch (Exception ex2)
                    {
                        string error_str = text_file_list[80] + "\n\n" + text_file_list[81] + "\n\n\n" + ex2;
                        MessageBox.Show(error_str);

                    }
                    di.Delete();
                }
                catch (Exception ex)
                {
                    string error_str = text_file_list[82] + "\n\n" + text_file_list[81] + "\n\n\n" + ex;
                    MessageBox.Show(error_str);
                }

                textBox_full_path.Text = fbd.SelectedPath + textBox_main_folder_name.Text + "\\";
            }
        }

        private void textBox_main_folder_name_TextChanged(object sender, EventArgs e)
        {
            if (textBox_main_folder_name.TextLength == 0)
                textBox_full_path.Text = textBox_folder_system_path.Text;
            else
                textBox_full_path.Text = textBox_folder_system_path.Text + textBox_main_folder_name.Text + "\\";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Sistem.remove_sensor_log_files();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            yazici.durum_yazici("Form1_FormCLosing Started.");
            int d_int = 0;

            if (int.TryParse(textBox_resolution.Text, out d_int))
            {
                Properties.Settings.Default.point_quantity = Convert.ToInt16(textBox_resolution.Text);

            }
            
            
            Properties.Settings.Default.selected_style = comboBox_meas_style.SelectedIndex;


            Properties.Settings.Default.Save();
            closing_flag = true;
            Application.Exit();
            Sistem.exceli_kapat();
            /*int cnt = 0;
            while (cnt < 10)
            {
                cnt++;
                Thread.Sleep(100);
            }
            */
            
            //this.Dispose();
            

            
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }




        private Tuple<decimal[], decimal[]> Excel_Test_Reader(string file_name)
        {
            Excel.Workbook ExcelProje;
            Excel.Worksheet ExcelSayfa;
            Excel.Application ExcelUygulama = new Excel.Application();
            string excel_file = "Excel_test.xlsx";
            string workbookPath = Application.StartupPath + "\\" + file_name;
            ExcelProje = (Excel.Workbook)ExcelUygulama.Workbooks.Open(workbookPath, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing);

            ExcelSayfa = ExcelProje.Worksheets.get_Item(1);
            Excel.Range range = ExcelSayfa.UsedRange;
            var vals = range.Value2;
            Dictionary<decimal, decimal> meas_list = new Dictionary<decimal, decimal>();
            List<decimal> l_of_elem = new List<decimal>();
            System.Collections.IEnumerable enumerable = vals as System.Collections.IEnumerable;
            //int len = enumerable.Count;
            if (enumerable != null)
            {
                foreach (object element in enumerable)
                {
                    l_of_elem.Add(Convert.ToDecimal(element));
                }
            }

            int len = l_of_elem.Count;
            List<decimal> pos = new List<decimal>();
            List<decimal> thick = new List<decimal>();
            for (int i = 0; i < len; i = i + 2)
            {
                pos.Add(l_of_elem[i]);
                thick.Add(l_of_elem[i + 1]);
                //meas_list.Add(pos, thick);
            }

            return Tuple.Create(pos.ToArray(), thick.ToArray());
        }


        public static double StandardDeviation(double[] valueList)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach (double value in valueList)
            {
                double tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }
            return Math.Sqrt(S / (k - 2));
        }

        private void button_calc_filter_Click(object sender, EventArgs e)
        {
            #region excel operations

            Excel.Application ExcelUygulama;
            Excel.Workbook ExcelProje;
            Excel.Worksheet ExcelSayfa;
            object Missing = System.Reflection.Missing.Value;
            Excel.Range ExcelRange;
            ExcelUygulama = new Excel.Application();
            ExcelProje = ExcelUygulama.Workbooks.Add(Missing);
            ExcelSayfa = (Excel.Worksheet)ExcelProje.Worksheets.get_Item(1);
            ExcelRange = ExcelSayfa.UsedRange;
            ExcelSayfa = (Excel.Worksheet)ExcelUygulama.ActiveSheet;

            ExcelUygulama.Visible = false;
            ExcelUygulama.AlertBeforeOverwriting = false;


            Excel.Range bolge = (Excel.Range)ExcelSayfa.Cells[1, 1];
            bolge.Value2 = "Filtresiz";
            Excel.Range range = ExcelSayfa.get_Range("A1", "B1");
            range.EntireColumn.AutoFit();
            #endregion

            Int32 sample_size = 500;
            // standart sapmayı ölçmek için gerekli sample size'a karar ver

            // kalibre edilmemiş standart sapmayı bul

            int d_int = 0;
            int filter_coeff = 1;
            bool filter_coeff_check = false;

            if ((int.TryParse(textBox_filter_const.Text, out d_int)) && (Convert.ToInt32(textBox_filter_const.Text) > 0) && (Convert.ToInt32(textBox_filter_const.Text) < 1000))
            {
                    filter_coeff = Convert.ToInt32(textBox_filter_const.Text);
                    filter_coeff_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[91]);
            }

            if (filter_coeff_check)
            {
                double[] samples = new double[sample_size];
                samples = Sensor.get_multiple_data(sample_size, filter_coeff, 0);

                bolge = ExcelSayfa.Range[ExcelSayfa.Cells[2, 1], ExcelSayfa.Cells[1 + sample_size, 1]];
                //DÜZELTİLECEK
                bolge.Value2 = samples;

                double std = StandardDeviation(samples)*1000;
                string std_str = std.ToString("F4", CultureInfo.InvariantCulture);
                double accuracy = std * 2;
                string accuracy_str = accuracy.ToString("F4", CultureInfo.InvariantCulture);
                double mean = samples.Average();
                string mean_str = mean.ToString("F5", CultureInfo.InvariantCulture);

                MessageBox.Show("Sensör Hassasiyeti: " + accuracy_str + " um" +Environment.NewLine + "Standart Sapma: " +std_str  + " um"+ Environment.NewLine + "Ölçülen Değer: " + mean_str  + " mm");

                bolge = (Excel.Range)ExcelSayfa.Cells[sample_size + 3, 1];
                bolge.Value2 = "Hesaplanan STD";
                bolge = (Excel.Range)ExcelSayfa.Cells[sample_size + 4, 1];
                bolge.Value2 = std.ToString("F5", CultureInfo.InvariantCulture);

                string format_for_file = "dd MM yy";
                DateTime time = DateTime.Now;              // Use current time
                string tarih = time.ToString(format_for_file);
                string s_dosyaadi = "kalibrasyon" + "_" + tarih + ".xlsx";
                string app_path = Application.StartupPath + "\\" + s_dosyaadi;
                if (s_dosyaadi != "")
                {

                    ExcelProje.SaveAs(app_path, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, Missing, Excel.XlSaveAsAccessMode.xlNoChange);
                    ExcelProje.Close(true, Missing, Missing);
                    ExcelUygulama.Quit();
                }

            }

        }
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// Grafik işlemleri
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>

    }
}