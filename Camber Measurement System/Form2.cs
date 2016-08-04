using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
namespace Camber_Measurement_System
{
    public partial class Form2 : Form
    {

        string lang_file = "gui_tr.dat";
        List<string> text_file_list = new List<string>();

        /*private void lang_settings()
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

            text_file_list = new List<string>(text_file.Split(new string[] { "\r\n" },
               StringSplitOptions.RemoveEmptyEntries));

            label1.Text = text_file_list[95];
            label2.Text = text_file_list[96];
            button1.Text = text_file_list[97];
            label3.Text = text_file_list[98];
            label4.Text = text_file_list[99];
            button2.Text = text_file_list[100];

            label5.Text = text_file_list[87];
            label6.Text = text_file_list[88];
        }*/

        public Form2()
        {
            InitializeComponent();
        }


        /*public static double StandardDeviation(double[] valueList)
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

        private Int32 sample_size_calculator()
        {
            Int32 sample_size = 512;
            Int32 max_sample_size = Int32.MaxValue/2; 
            // başlangıç sample size'ını belirle
            //double[] data_get = Sensor.get_multiple_data(sample_size, 1, offset_calib);
            // sample size kadar ölçüm al
            //double std1 = StandardDeviation(data_get);
            // standart sapmayı hesapla
            double std_diff = 1;
            // standart sapma değişimini 1 olarak ayarla

            while ((std_diff > 0.01) && (sample_size < max_sample_size))
            // while ( standart sapma değişimi 0.01'den küçükse ve sample_size 10 milyondan küçükse )
            {
                Int32 array_length = sample_size;
                double[] data_get_2 = new double[array_length*2];
                // // mevcut array'in 2 katı kadar yer ayır
                //double[] data_get_3 = Sensor.get_multiple_data(sample_size, 1);
                // // bir o kadar daha sample al
                //data_get.CopyTo(data_get_2, 0);
                //data_get_3.CopyTo(data_get_2, data_get.Length);
                // // başka bir array içinde birleştir.
                double std2 = StandardDeviation(data_get_2);
                // // Standart sapmasını hesapla = std2
                try
                {
                    std_diff = Math.Abs(((std1 - std2) / std1));
                }
                catch (Exception)
                {
                    std_diff = 0;
                }
                
                // // standart sapma değişimi = |(std1-std2)/std1|
                std1 = std2;
                // // std1 = std2
                sample_size = data_get_2.Length;
                // sample size'ı döndür.
            }

            return sample_size;
            
        }

        private Int32 filter_coeff_calculator(Int32 sample_size, Int32 filter_coeff, double required_std)
        {
            double Kp = 3;
            double std = StandardDeviation(Sensor.get_multiple_data(sample_size, filter_coeff));
            while (Math.Abs(required_std) < Math.Abs(std))
            {
                double error = (required_std - std)+1;
                filter_coeff = Convert.ToInt32(Math.Ceiling(filter_coeff * error * Kp));
                std = StandardDeviation(Sensor.get_multiple_data(sample_size, filter_coeff));
            }
            return filter_coeff;
        }

        private void calibration_session(double required_std, double object_thickness)
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
            bolge = (Excel.Range)ExcelSayfa.Cells[1, 2];
            bolge.Value2 = "Filtreli";
            Excel.Range range = ExcelSayfa.get_Range("A1", "B1");
            range.EntireColumn.AutoFit();
            #endregion
            //Int32 sample_size = sample_size_calculator();

            Int32 sample_size = 500;
            // standart sapmayı ölçmek için gerekli sample size'a karar ver
            
            // kalibre edilmemiş standart sapmayı bul
            double[] samples = new double[sample_size];
            samples = Sensor.get_multiple_data(sample_size, 1);
            
            bolge = ExcelSayfa.Range[ExcelSayfa.Cells[2, 1], ExcelSayfa.Cells[1 + sample_size, 1]];
            bolge.Value = samples;
            
            double std = StandardDeviation(samples);
            textBox4.Text = std.ToString("F4", CultureInfo.InvariantCulture);
            
            bolge = (Excel.Range)ExcelSayfa.Cells[sample_size+3, 1];
            bolge.Value2 = "Hesaplanan Std";
            bolge = (Excel.Range)ExcelSayfa.Cells[sample_size + 4, 1];
            bolge.Value = std.ToString("F4", CultureInfo.InvariantCulture);

            // istenen standart sapmaya ulaşmak için gerekli filtre sabitine itere et
            Int32 filter_coeff = 1;
            if (Math.Abs(required_std) < Math.Abs(std))
            {
                filter_coeff = Convert.ToInt32(Math.Ceiling(std/required_std));
                filter_coeff = filter_coeff * filter_coeff;
                filter_coeff = filter_coeff_calculator(sample_size, filter_coeff, required_std);
            }
            double[] samples_2 = new double[sample_size];
            samples_2 = Sensor.get_multiple_data(sample_size, filter_coeff);

            bolge = ExcelSayfa.Range[ExcelSayfa.Cells[2, 2], ExcelSayfa.Cells[1 + sample_size, 2]];
            bolge.Value = samples_2;

            bolge = (Excel.Range)ExcelSayfa.Cells[sample_size + 3, 2];
            bolge.Value2 = "Hesaplanan Filtre";
            bolge = (Excel.Range)ExcelSayfa.Cells[sample_size + 4, 2];
            bolge.Value = filter_coeff.ToString();

            double avg_of_samples = samples_2.Average();
            double offsett_error = object_thickness - avg_of_samples;
            textBox3.Text = offsett_error.ToString("F4", CultureInfo.InvariantCulture);
            offsett_error = offsett_error * -1;
            textBox6.Text = offsett_error.ToString("F4", CultureInfo.InvariantCulture);
            textBox5.Text = Convert.ToString(filter_coeff);

            string format_for_file = "dd MM yy";
            DateTime time = DateTime.Now;              // Use current time
            string tarih = time.ToString(format_for_file);
            string s_dosyaadi = "kalibrasyon" + "_" + tarih + ".xlsx";
            if (s_dosyaadi != "")
            {

                ExcelProje.SaveAs(s_dosyaadi, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, Missing, Excel.XlSaveAsAccessMode.xlNoChange);
                ExcelProje.Close(true, Missing, Missing);
                ExcelUygulama.Quit();
            }

        }
        */
        private void button1_Click(object sender, EventArgs e)
        {
            #region user input controls
            bool textBox_req_acc_check = false;
            bool textBox_obj_thick_check = false;
            decimal d;


            if (textBox_req_acc.Text.Contains('.'))
            {
                textBox_req_acc.Text = textBox_req_acc.Text.Replace('.', ',');
            }

            if (textBox_obj_thick.Text.Contains('.'))
            {
                textBox_obj_thick.Text = textBox_obj_thick.Text.Replace('.', ',');
            }

            if (decimal.TryParse(textBox_req_acc.Text, out d))
            {
                if (Convert.ToDecimal(textBox_req_acc.Text) <= 0)
                {
                    textBox_req_acc.Text = "0,0005";
                }

                if (Convert.ToDecimal(textBox_req_acc.Text) > 0.02M)
                {
                    textBox_req_acc.Text = "0,020";
                }
                textBox_req_acc_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[93]);
            }

            while (textBox_obj_thick.Text.Contains(" "))
            {
                textBox_obj_thick.Text = textBox_obj_thick.Text.Replace(" ","");
            }

            if (textBox_obj_thick.Text == "")
            {
                textBox_obj_thick.Text = "0";
            }

            if (decimal.TryParse(textBox_obj_thick.Text, out d))
            {

                if (Convert.ToDecimal(textBox_obj_thick.Text) < 0)
                {
                    textBox_obj_thick.Text = "0";
                }

                if (Convert.ToDecimal(textBox_obj_thick.Text) > Properties.Settings.Default.maxthickness)
                {
                    textBox_obj_thick.Text = Convert.ToString(Properties.Settings.Default.maxthickness);
                }
                textBox_obj_thick_check = true;
            }
            else
            {
                MessageBox.Show(text_file_list[94]);
            }
            #endregion

            button1.Enabled = false;
            button2.Enabled = false;

            if (textBox_req_acc_check && textBox_obj_thick_check)
            {
                double required_std = Convert.ToDouble(textBox_req_acc.Text) / 4;
                double object_thickness = Convert.ToDouble(textBox_obj_thick.Text);
                //calibration_session(required_std, object_thickness);
            }

            button1.Enabled = true;
            button2.Enabled = true;
        }


        

        private void Form2_Load(object sender, EventArgs e)
        {
            //lang_settings();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //Form1.textBox_offset_calib.Text = textBox6.Text;
            //Form1.textBox_filter_const.Text = textBox5.Text;

            //this.Close();
        }
    }
}
