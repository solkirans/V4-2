using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using System.Reflection;

namespace Camber_Measurement_System
{
    static class Sistem
    {
        static int resolution = 0;
        static int wait_Time_meas = 15000;
        static int plate_width = 0;
        //static int strategy = 0; // 0 = uniform, 1 = p type, 2 = pd type
        static int nominal_thickness = 0;
        static string message_string = "";
        public static List<measured_point> measured_point_list = new List<measured_point>();


        static Excel.Application ExcelUygulama = new Excel.Application();
        static Excel.Workbook ExcelProje;
        static Excel.Worksheet ExcelSayfa;
        static object Missing = System.Reflection.Missing.Value;
        static Excel.Range ExcelRange;
        static int satir = 1;
        static string format = "dd/MM/yy HH:mm:ss";
        static string format_for_file = "dd MM yy";
        static string format_for_excel = "HH_mm_ss";
        static string tarih = "";
        
        static string dosya_yolu_full = "";
        static bool stop_flag = false;

        public static bool excel_flag = false;// excel'e yazılmasını istediğimiz zaman bu flag'i true yapıcaz;

        public static string lang_file = "sistem_tr.dat";
        public static List<string> text_file_list = new List<string>();

//////////////////////////////////////////////////////////////////////////////////////////

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


        public static void FormatAsTable(Excel.Range SourceRange)
        {
            yazici.durum_yazici("FormatAsTable started.");
            SourceRange.Worksheet.ListObjects.Add(Excel.XlListObjectSourceType.xlSrcRange,
            SourceRange, System.Type.Missing, Excel.XlYesNoGuess.xlYes, System.Type.Missing).Name =
                "Table1";
            SourceRange.Select();
            SourceRange.Worksheet.ListObjects["Table1"].TableStyle = "TableStyleLight1";
            yazici.durum_yazici("FormatAsTable ended.");
        }

        private static string create_required_paths(string work_path)
        {
            yazici.durum_yazici("create_required_paths started.");
            string year = Convert.ToString(DateTime.Now.Year, CultureInfo.InvariantCulture);
            string month = Convert.ToString(DateTime.Now.Month, CultureInfo.InvariantCulture);
            string time = DateTime.Now.ToString(format_for_excel, CultureInfo.InvariantCulture);
            
            bool exists = System.IO.Directory.Exists(work_path);
            if (!exists)
                System.IO.Directory.CreateDirectory(work_path);
            work_path = work_path + year + "\\";
            exists = System.IO.Directory.Exists(work_path);
            if (!exists)
                System.IO.Directory.CreateDirectory(work_path);
            yazici.durum_yazici("create_required_paths ended.");
            return work_path;
        }

        public static string Month_to_string(int month)
        {
            string month_str = "";

            if (month == 1)
            {
                month_str = text_file_list[0];
            }
            else if (month == 2)
            {
                month_str = text_file_list[1];
            }
            else if (month == 3)
            {
                month_str = text_file_list[2];
            }
            else if (month == 4)
            {
                month_str = text_file_list[3];
            }
            else if (month == 5)
            {
                month_str = text_file_list[4];
            }
            else if (month == 6)
            {
                month_str = text_file_list[5];
            }
            else if (month == 7)
            {
                month_str = text_file_list[6];
            }
            else if (month == 8)
            {
                month_str = text_file_list[7];
            }
            else if (month == 9)
            {
                month_str = text_file_list[8];
            }
            else if (month == 10)
            {
                month_str = text_file_list[9];
            }
            else if (month == 11)
            {
                month_str = text_file_list[10];
            }
            else if (month == 12)
            {
                month_str = text_file_list[11];
            }
            return month_str;
        
        }

        public static void exceli_yarat(string meas_id, string prd_line, string work_path)
        {
            satir = 1;
            yazici.durum_yazici("exceli_yarat started.");
            work_path = create_required_paths(work_path);
            string excel_yolu;
            ExcelUygulama.DisplayAlerts = false;
            DateTime time = DateTime.Now;              // Use current time
            tarih = time.ToString(format_for_file);
            tarih = tarih.Replace(' ', '_');
            string month = Month_to_string(Convert.ToInt32(Convert.ToString(DateTime.Now.Month, CultureInfo.InvariantCulture)));
            string excel_name = prd_line + " - " + month + "_" + Convert.ToString(DateTime.Now.Year, CultureInfo.InvariantCulture); 
            excel_yolu = work_path + excel_name+ ".xlsx";
            dosya_yolu_full = excel_yolu;

            ///////////////////////////////////////////////////////////////////////////

            //dosya_yolu_full = Application.StartupPath + "\\26_11_14levha_olcumleri.xlsx";



            ////////////////////////////////////////////////////////////////////////////

            yazici.durum_yazici("Excel dosya yolu: " + excel_yolu + " olarak ayarlandi.");
            yazici.durum_yazici("'exceli_yarat' fonksiyonuna girildi.");
            if (!File.Exists(dosya_yolu_full))
            {
                yazici.durum_yazici("Excel dosya yolu bulunamadı ve yaratılacak.");
                ExcelProje = ExcelUygulama.Workbooks.Add(Missing);
                //ExcelProje = (Excel.Workbook)(ExcelUygulama.Workbooks.Add(Missing));
                ExcelSayfa = (Excel.Worksheet)ExcelProje.Worksheets.get_Item(1);
                ExcelRange = ExcelSayfa.UsedRange;
                ExcelSayfa = (Excel.Worksheet)ExcelUygulama.ActiveSheet;

                int sayfa_Sayisi = ExcelProje.Worksheets.Count;
                int counter_for_same_name = 0;
                foreach (Excel.Worksheet sayfalar in ExcelProje.Worksheets)
                {
                    if ((sayfalar.Name).Contains(meas_id))
                    {
                        counter_for_same_name++;
                    }
                }
                if (counter_for_same_name != 0)
                {
                    ExcelSayfa.Name = meas_id + "__" + Convert.ToString(counter_for_same_name);
                }

                if (counter_for_same_name == 0)
                {
                    ExcelSayfa.Name = meas_id;
                }

                //ExcelUygulama.Visible = true;
                ExcelUygulama.AlertBeforeOverwriting = false;
                ExcelSayfa.Cells[1, 1] = text_file_list[12];
                ExcelSayfa.Cells[1, 2] = text_file_list[13];

                Excel.Range SourceRange = (Excel.Range)ExcelSayfa.get_Range("A1", "B2"); // or whatever range you want here
                FormatAsTable(SourceRange);
                try
                {
                    ExcelProje.SaveAs(dosya_yolu_full, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, Missing, Excel.XlSaveAsAccessMode.xlNoChange);
                    yazici.durum_yazici("Veriler Excel'e başarı ile eklendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(text_file_list[14] + "\n\r\n\r" + ex.Message);
                    yazici.durum_yazici(text_file_list[14] + "\n\r\n\r" + ex.Message);
                }

                yazici.durum_yazici("Excel dosyasi yaratildi ve gereken format verildi.");
                yazici.durum_yazici("exceli_yarat ended.");
            }
            else
            {
                yazici.durum_yazici("Excel dosyasi bulundu.");
                //ExcelProje = ExcelUygulama.Workbooks.Add(dosya_yolu_full);
                ExcelProje = (Excel.Workbook)ExcelUygulama.Workbooks.Open(dosya_yolu_full, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing, Missing);
                int sayfa_Sayisi = ExcelProje.Worksheets.Count;
                int counter_for_same_name = 0;
                foreach (Excel.Worksheet sayfalar in ExcelProje.Worksheets)
                {
                    if ((sayfalar.Name).Contains(meas_id))
                    {
                        counter_for_same_name++;
                    }
                }

                ExcelSayfa = (Excel.Worksheet)ExcelProje.Worksheets.Add(Missing, Missing, Missing, Missing);
                if (counter_for_same_name != 0)
                {
                    ExcelSayfa.Name = meas_id + "__" + Convert.ToString(counter_for_same_name);
                }

                if (counter_for_same_name == 0)
                {
                    ExcelSayfa.Name = meas_id;
                }

                ExcelRange = ExcelSayfa.UsedRange;
                ExcelSayfa = (Excel.Worksheet)ExcelUygulama.ActiveSheet;
                ExcelUygulama.Visible = false;
                ExcelUygulama.AlertBeforeOverwriting = false;


                Excel.Range range = ExcelSayfa.UsedRange;
                satir = range.Rows.Count;

                ExcelSayfa.Cells[1, 1] = text_file_list[12];
                ExcelSayfa.Cells[1, 2] = text_file_list[13];
                range = ExcelSayfa.get_Range("A1", "B1");
                range.EntireColumn.AutoFit();

                Excel.Range SourceRange = (Excel.Range)ExcelSayfa.get_Range("A1", "B2"); // or whatever range you want here
                FormatAsTable(SourceRange);

                try
                {
                    ExcelProje.SaveAs(dosya_yolu_full, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Missing, Missing);
                    yazici.durum_yazici("Excel dosyasina gereken format verildi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(text_file_list[14] + "\n\r\n\r" + ex.Message);
                    yazici.durum_yazici(text_file_list[14] + "\n\r\n\r" + ex.Message);
                }

            }
        }

        public static void excele_veri_ekle(string _olcum_no, decimal _pozisyon, decimal _kalinlik, string _tarih, bool end_of_list)
        {
            //yazici.durum_yazici("excele_veri_ekle started.");
            string dosya_yolu = dosya_yolu_full;
            try
            {
                satir++;

                ExcelSayfa.Cells[satir, 1].NumberFormat = "#.##0,000";
                ExcelSayfa.Cells[satir, 1] = Convert.ToDecimal(_pozisyon);
                ExcelSayfa.Cells[satir, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                ExcelSayfa.Cells[satir, 2].NumberFormat = "#.##0,000";
                ExcelSayfa.Cells[satir, 2] = Convert.ToDecimal(_kalinlik);
                ExcelSayfa.Cells[satir, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                Excel.Range range = ExcelSayfa.get_Range("A2", "B2");
                range.EntireColumn.AutoFit();
                yazici.durum_yazici("Veriler Excel Projesine eklendi.");

            }
            catch (Exception ex)
            {
                MessageBox.Show(text_file_list[15] + "\n\r\n\r" + text_file_list[16] + "\n\r\n\r" + text_file_list[17] + "\n\r\n\r" + ex.Message);
                yazici.durum_yazici(text_file_list[15] + "\n\r\n\r" + text_file_list[16] + "\n\r\n\r" + text_file_list[17] + "\n\r\n\r" + ex.Message);
            }

            ExcelUygulama.AlertBeforeOverwriting = false;
            try
            {
                //excel_kullanimda_mi = 0;
                ExcelProje.SaveAs(dosya_yolu, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Missing, Missing);
            }
            catch (Exception ex)
            {

                MessageBox.Show(text_file_list[15] + "\n\r\n\r" + text_file_list[16] + "\n\r\n\r" + text_file_list[17] + "\n\r\n\r" + ex.Message);
                yazici.durum_yazici(text_file_list[15] + "\n\r\n\r" + text_file_list[16] + "\n\r\n\r" + text_file_list[17] + "\n\r\n\r" + ex.Message);
            }
            //yazici.durum_yazici("excele_veri_ekle ended.");

        }

        public static void excele_grafik_ekle(string shift_str, string operator_str, string width_str, string roll_number_str, string alloy_str, string measurement_id, string prod_line_str)
        {
            yazici.durum_yazici("excele_grafik_ekle started");
            Excel.Range range = ExcelSayfa.UsedRange;
            satir = range.Rows.Count;
            double chart_position_left = ExcelSayfa.get_Range("A:G", System.Type.Missing).Width;
            double chart_position_up = ExcelSayfa.get_Range("A1:A6", System.Type.Missing).Height;
            Excel.ChartObjects xlCharts = (Excel.ChartObjects)ExcelSayfa.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(chart_position_left, chart_position_up, 600, 150);
            Excel.Chart chartPage = myChart.Chart;

            Excel.Range chartRange = ExcelSayfa.get_Range("B1", ("B"+Convert.ToString(satir)));
            chartPage.SetSourceData(chartRange, Missing);
            var series = (Excel.Series)chartPage.SeriesCollection(1);
            series.XValues = ExcelSayfa.get_Range("A2", ("A" + Convert.ToString(satir)));
            chartPage.ChartType = Excel.XlChartType.xlLine;
            //chartPage.Legend.

            ExcelSayfa.Cells[2, 4] = text_file_list[18];
            ExcelSayfa.Cells[3, 4] = text_file_list[19];
            ExcelSayfa.Cells[4, 4] = text_file_list[20];
            ExcelSayfa.Cells[5, 4] = text_file_list[21];
            ExcelSayfa.Cells[6, 4] = text_file_list[22];
            ExcelSayfa.Cells[7, 4] = text_file_list[23];
            ExcelSayfa.Cells[8, 4] = text_file_list[53];
            ExcelSayfa.Cells[9, 4] = text_file_list[54];

            ExcelSayfa.Cells[2, 5].NumberFormat = "@";
            ExcelSayfa.Cells[2, 5] = measurement_id;
            ExcelSayfa.Cells[2, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            ExcelSayfa.Cells[3, 5].NumberFormat = "@";
            ExcelSayfa.Cells[3, 5] = operator_str;
            ExcelSayfa.Cells[3, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            DateTime _time = DateTime.Now;              // Use current time
            string _tarih = _time.ToString(format, CultureInfo.InvariantCulture);
            ExcelSayfa.Cells[4, 5].NumberFormat = "@";
            ExcelSayfa.Cells[4, 5] = _tarih;
            ExcelSayfa.Cells[4, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            ExcelSayfa.Cells[5, 5].NumberFormat = "@";
            ExcelSayfa.Cells[5, 5] = width_str;
            ExcelSayfa.Cells[5, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            ExcelSayfa.Cells[6, 5].NumberFormat = "@";
            ExcelSayfa.Cells[6, 5] = roll_number_str;
            ExcelSayfa.Cells[6, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            ExcelSayfa.Cells[7, 5].NumberFormat = "@";
            ExcelSayfa.Cells[7, 5] = alloy_str;
            ExcelSayfa.Cells[7, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            ExcelSayfa.Cells[8, 5].NumberFormat = "@";
            ExcelSayfa.Cells[8, 5] = shift_str;
            ExcelSayfa.Cells[8, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            ExcelSayfa.Cells[9, 5].NumberFormat = "@";
            ExcelSayfa.Cells[9, 5] = prod_line_str;
            ExcelSayfa.Cells[9, 5].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            range = ExcelSayfa.get_Range("A1", "H7");
            range.EntireColumn.AutoFit();
            ExcelUygulama.Visible = true;
            try
            {
                //excel_kullanimda_mi = 0;
                ExcelProje.SaveAs(dosya_yolu_full, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Missing, Missing);
            }
            catch (Exception ex)
            {
                string msg = text_file_list[24] + "\n\r\n\r" + text_file_list[16] + "\n\r\n\r" + text_file_list[17] + "\n\r\n\r" + ex.Message;
                MessageBox.Show(msg);
                yazici.durum_yazici(msg);
            }
            yazici.durum_yazici("excele_grafik_ekle ended.");
        }

        public static void bombelik_hesapla()
        {
            yazici.durum_yazici("bombelik_hesapla started");
            Excel.Range range = ExcelSayfa.UsedRange;
            satir = range.Rows.Count;

            int k1 = 2;
            int k2 = satir;

            int merkez = ((satir + k1) / 2);
            

            ExcelSayfa.Cells[1, 6] = text_file_list[25];
            ExcelSayfa.Cells[1, 7] = text_file_list[26];
            ExcelSayfa.Cells[1, 8] = text_file_list[27];
            ExcelSayfa.Cells[1, 9] = text_file_list[28];
            ExcelSayfa.Cells[1, 10] = text_file_list[29];
            var val = (ExcelSayfa.Cells[k1, 2] as Excel.Range).Value;
            var val2 =(ExcelSayfa.Cells[k2, 2] as Excel.Range).Value;
            var val3 = (ExcelSayfa.Cells[merkez, 2] as Excel.Range).Value;
            decimal kalinlik1 = Convert.ToDecimal(val);
            decimal kalinlik2 = Convert.ToDecimal(val2);
            decimal merkez_k = Convert.ToDecimal(val3);
            decimal bombelik1 = 0;
            try
            {
                bombelik1 = ((merkez_k - kalinlik1) / merkez_k) * 100;

            }
            catch (Exception ex)
            {
                MessageBox.Show( text_file_list[30] + "\n\r\n\r" + ex.Message);
                yazici.durum_yazici(text_file_list[30] + "\n\r\n\r" + ex.Message);
            }

            decimal bombelik2 = 0;

            try
            {
                bombelik2 = ((merkez_k - kalinlik2) / merkez_k) * 100;

            }
            catch (Exception ex)
            {
                MessageBox.Show(text_file_list[31] + "\n\r\n\r" + ex.Message);
                yazici.durum_yazici(text_file_list[31] + "\n\r\n\r" + ex.Message);
            }

            ExcelSayfa.Cells[2, 6] = bombelik1;
            ExcelSayfa.Cells[2, 6].NumberFormat = "#.##0,000";
            ExcelSayfa.Cells[2, 6].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ExcelSayfa.Cells[2, 7] = bombelik2;
            ExcelSayfa.Cells[2, 7].NumberFormat = "#.##0,000";
            ExcelSayfa.Cells[2, 7].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ExcelSayfa.Cells[2, 8] = kalinlik1;
            ExcelSayfa.Cells[2, 8].NumberFormat = "#.##0,000";
            ExcelSayfa.Cells[2, 8].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ExcelSayfa.Cells[2, 9] = kalinlik2;
            ExcelSayfa.Cells[2, 9].NumberFormat = "#.##0,000";
            ExcelSayfa.Cells[2, 9].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ExcelSayfa.Cells[2, 10] = merkez_k;
            ExcelSayfa.Cells[2, 10].NumberFormat = "#.##0,000";
            ExcelSayfa.Cells[2, 10].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ExcelUygulama.AlertBeforeOverwriting = false;
            range = ExcelSayfa.get_Range("A2", "Z2");
            range.EntireColumn.AutoFit();
            try
            {
                //excel_kullanimda_mi = 0;
                ExcelProje.SaveAs(dosya_yolu_full, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Missing, Missing);
            }
            catch (Exception ex)
            {

                MessageBox.Show(text_file_list[15] + "\n\r\n\r" + text_file_list[16] + "\n\r\n\r" + text_file_list[17] + "\n\r\n\r" + ex.Message);
                yazici.durum_yazici(text_file_list[15] + "\n\r\n\r" + text_file_list[16] + "\n\r\n\r" + text_file_list[17] + "\n\r\n\r" + ex.Message);
            }
            yazici.durum_yazici("bombelik_hesapla Ended");
        }

        public static void exceli_kapat()
        {
            yazici.durum_yazici("exceli_kapat started.");
            try
            {
                ExcelProje.Close(0);
                ExcelUygulama.Quit();
            }
            catch (Exception ex)
            {

                string err_str = "Excel could not closed successfully.\n\n\n" + ex;
                yazici.durum_yazici(err_str);
            }

            /*try
            {
                yazici.durum_yazici("Acik olan Excel dosyalari kapatilmaya calisiliyor.");
                foreach (Process proc in Process.GetProcessesByName("EXCEL"))
                {
                    yazici.durum_yazici("Bir adet Excel dosyasi bulundu ve kapatildi.");
                    proc.Kill();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Açık olan EXCEL uygulamaları kapatılırken hata oluştu.\n\r\n\r" + ex.Message);
                yazici.durum_yazici("Açık olan EXCEL uygulamaları kapatılırken hata oluştu.\n\r\n\r" + ex.Message);
            }
            */

            yazici.durum_yazici("exceli_kapat ended.");
            
        }


//////////////////////////////////////////////////////////////////////////////////////////

        public static int init_system(int _resolution, int _plate_width, int _strategy, int _nominal_thickness)
        {
            int system_status_local = 0; // 0 = not ready, 1 = Ok
            bool error_flag = false;
            yazici.durum_yazici("'Sistem class' constructor run. Values:\n" + "_resolution=" + Convert.ToString(_resolution) + "\n_plate_width=" + Convert.ToString(_plate_width) + "\n_strategy=" + Convert.ToString(_strategy) + "\n_nominal_thickness" + Convert.ToString(_nominal_thickness));

            // Value checking


            // Resolution check
            if ((_resolution > 0) && (_resolution <= 10))
            {
                resolution = _resolution;
            }
            else
            {
                error_flag = true;
                message_string = text_file_list[32] + ": " + Convert.ToString(_resolution);
                MessageBox.Show(message_string);
                yazici.durum_yazici(message_string);

            }


            // Plate width check
            if ((_plate_width > 0) && (_plate_width <= Properties.Settings.Default.Strokelength) && (error_flag == false))
            {
                plate_width = _plate_width;
            }
            else
            {
                error_flag = true;
                message_string = text_file_list[33] + "\n\r" + text_file_list[34] + Convert.ToString(Properties.Settings.Default.Strokelength) + text_file_list[35];
                System.Windows.Forms.MessageBox.Show(message_string);
                yazici.durum_yazici(message_string);
                
            }


            // Thickness check
            if ((_nominal_thickness > 1) && (_nominal_thickness <= 10) && (error_flag == false))
            {
                nominal_thickness = _nominal_thickness;
            }
            else
            {
                error_flag = true;
                message_string = text_file_list[36];
                System.Windows.Forms.MessageBox.Show(message_string);
                yazici.durum_yazici(message_string);

            }

            if (check_motor_status() != 1)
            {
                error_flag = true;
            }

            if (check_sensor_status() != 1)
            {
                error_flag = true;
            }

            if (error_flag == true)
            {
                system_status_local = 0;
            }
            else
            {
                system_status_local = 1;
            }
            yazici.durum_yazici("'Sistem class construtor ended and error flag is:" + Convert.ToString(error_flag));
            return system_status_local;
        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static int check_motor_status()
        {
            yazici.durum_yazici("check_motor_status started.");
            int motor_status = 0; // 0 = error, 1 = Ok
            motor_status = motion_head.motion_init();
            yazici.durum_yazici("check_motor_status ended.");
            return motor_status;
        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static int check_sensor_status()
        {
            yazici.durum_yazici("check_sensor_status started.");
            int sensor_status = 0; // 0 = Error, 1 = Ok
            sensor_status = Sensor.sensor_init();
            yazici.durum_yazici("check_sensor_status ended.");
            return sensor_status;
        }

        public static void remove_sensor_log_files()
        {
            yazici.durum_yazici("remove_sensor_log_files started.");
            string processed_file_name = "";
            string date_time_format_sensor_log = "yyyy-MM-dd_HH-mm-ss";
            string path = Application.StartupPath;
            string[] files = System.IO.Directory.GetFiles(path, "*.txt");

            foreach (string file_name in files)
            {
                if (file_name.Contains("SensorLog_"))
                {
                    
                    processed_file_name = file_name.Replace("SensorLog_", "");
                    processed_file_name = processed_file_name.Replace(path, "");
                    processed_file_name = processed_file_name.Replace(".txt", "");
                    processed_file_name = processed_file_name.Replace("\\", "");
                    DateTime date_of_file;
                    DateTime date_of_now = DateTime.Now;
                    if (DateTime.TryParseExact(processed_file_name,
                           date_time_format_sensor_log,
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None,
                           out date_of_file))
                    {
                        if (date_of_file.AddHours(Properties.Settings.Default.delete_interval) < date_of_now)
                        {
                            try
                            {
                                File.Delete(file_name);
                            }
                            catch (Exception ex)
                            {
                                string err_str = text_file_list[37] + "\n\r" + path + file_name + "\n\n\n" + ex;  
                                MessageBox.Show(err_str);
                            }
                            
                        }
                    }
                }
            }
            yazici.durum_yazici("remove_sensor_log_files ended.");
        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static int go_home_position()
        {
            yazici.durum_yazici("go_home_position started.");
            int positioning_status = 0; // 0 = error, 1 = Ok
            //go home position codes
            yazici.durum_yazici("go_home_position ended.");
            return positioning_status;
        }

//////////////////////////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////////////////////////

        public static decimal[] go_position_and_measure(decimal position, bool insert_to_excel, int filter_coeff, decimal offset_calib, int wait)
       {
            Form1._Form1.manipulate_go_button("off");
            //yazici.durum_yazici("go_position_and_measure started.");
            int error_code_1 = 0; // 0 = error, 1 = Ok
            int error_code_2 = 0; // 0 = error, 1 = Ok
            decimal error_code = 0; // 0 = error, 1 = Ok
            double thickness_ = 0;
            decimal thickness_d = 0;
            decimal[] return_of_func = {error_code,thickness_d};
            try
            {

                motion_head.go_position(position);
                error_code_1 = 1;
            }
            catch (Exception ex)
            {
                string error_string = text_file_list[38] + ex;
                yazici.durum_yazici(error_string);
                MessageBox.Show(error_string);
                throw;
            }
            try
            {
                // BURADA BEKLİYORUZ
                TimeSpan wait_time = new System.TimeSpan(0, 0, 0, 0, wait);

                Thread.Sleep(wait_time);
                thickness_ = Sensor.Get_Data(filter_coeff, offset_calib);
                thickness_d = Convert.ToDecimal(thickness_);
                
                Form1._Form1.update_cur_thickness(thickness_d);
                error_code_2 = 1;
            }
            catch (Exception ex)
            {
                string error_string = text_file_list[39] + ex;
                yazici.durum_yazici(error_string);
                MessageBox.Show(error_string);
            }

            if ((error_code_1 == 1) && (error_code_2 == 1))
            {
                error_code = 1;
            }
            DateTime time = DateTime.Now;              // Use current time
            time = DateTime.Now;
            decimal[] ret_of_read_pos = (motion_head.read_position());
            if (insert_to_excel == true)
            {
                excele_veri_ekle(Form1._Form1.get_measurement_id(), ret_of_read_pos[1], thickness_d, time.ToString(format), false);
            }
            //Form1._Form1.manipulate_go_button("on");
            return_of_func[0] = error_code;
            return_of_func[1] = thickness_d;
            Form1._Form1.manipulate_go_button("on");
            //yazici.durum_yazici("go_position_and_measure ended");
            return return_of_func;
       }

//////////////////////////////////////////////////////////////////////////////////////////

        public static void start_measurement_session(int step_count, string shift_str, string operator_str, string width_str, string roll_number_str, string alloy_str, string measurement_id, string prod_line, Int32 filter_coeff)
        {
            yazici.durum_yazici("start_measurement_session Started.");
            Form1._Form1.set_auto_measurement("disable");
            stop_flag = false;
            //int measurement_status = 0; // 0= error, 1 = ok
            //go home position codes
            decimal length_of_plate = 0;
            decimal offset_from_edges = Properties.Settings.Default.offset_from_edge;
            decimal offset_calib = Properties.Settings.Default.offset_calib;
            decimal step_size = 0;
            step_count = step_count - 1;
            int remaining_step = step_count;
            decimal max_dist = Properties.Settings.Default.Strokelength * 5;
            if (stop_flag == false)
            {
                
                motion_head.go_position(max_dist * (-1));
            }
            if (stop_flag == false)
            {
                Sistem.find_plate_start("p", true, offset_calib , filter_coeff);
            }
            if (stop_flag == false)
            {
                motion_head.go_position(max_dist);
            }
            if (stop_flag == false)
            {
                Sistem.find_plate_start("n", false, offset_calib, filter_coeff);
            }
            decimal[] ret_of_func = motion_head.read_position();
            if (ret_of_func[0] == 1)
            {
                length_of_plate = ret_of_func[1];
                step_size = (length_of_plate - (offset_from_edges * 2)) / step_count;
                go_position_and_measure((offset_from_edges * (-1)), true, filter_coeff, offset_calib, wait_Time_meas);
                
                while ((remaining_step > 0) && (stop_flag == false))
                {

                    try
                    {
                        ret_of_func = go_position_and_measure((step_size * (-1)), true, filter_coeff,offset_calib, wait_Time_meas);
                        remaining_step--;
                        if (ret_of_func[0] == 0)
                        {
                            remaining_step = -1;
                        }
                    }
                    catch (Exception ex)
                    {

                        yazici.durum_yazici("start_measurement_session Error: Step Size" + step_size + "Exception:" + ex);
                    }
                }
                if (stop_flag == true)
                {
                    string str = text_file_list[40];
                    MessageBox.Show(str);
                    yazici.durum_yazici(str);
                }
                
            }
            Form1._Form1.set_auto_measurement("enable");
            yazici.durum_yazici("start_measurement_session Ended, Excel functions will started.");
            bombelik_hesapla();
            excele_grafik_ekle(shift_str, operator_str, width_str, roll_number_str, alloy_str, measurement_id, prod_line);
            //exceli_kapat();
            //return measurement_status;
        }

        public static void stop_measurement_session()
        {
            yazici.durum_yazici("stop_measurement_session started.");
            stop_flag = true;
            motion_head.stop_ax();
            yazici.durum_yazici("stop_measurement_session ended.");
        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static void sinusoidal_curve_fit()
        {

            // equation = (a*sin((b*x)+c))+d
            //int a = 0;
            //int b = 0;
            //int c = 0;
            //int d = 0;
            //double plate_width = find_plate_width();
            //double plate_width = measured_point_list.(measured_point_list.Count());

        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static double find_plate_width()
        {
            yazici.durum_yazici("find_plate_width started.");
            int length_of_list = Sistem.measured_point_list.Count();
            double width_of_plate = 0;
            double delta = 0;
            int sayac = length_of_list - 1;
            measured_point _mp = new measured_point(1, 1);
            while (delta>=0)
            {
                try 
	            {	        
		            _mp = Sistem.measured_point_list.ElementAt(sayac);
	            }
	            catch (Exception)
	            {

	            }
                delta = _mp.measured_position - width_of_plate;
                if (delta >= 0)
                {    
                    width_of_plate = _mp.measured_position;
                }

                sayac = sayac - 1;
            }
            System.Windows.Forms.MessageBox.Show(Convert.ToString(width_of_plate));
            yazici.durum_yazici("find_plate_width ended.");
            return width_of_plate;


        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static int find_plate_start(string Direction, bool reset_with_find, decimal offset_calib ,int filter_coeff) //// BISECTION ALGORITHM
        {
            yazici.durum_yazici("find_plate_start started.");
            bool true_point = false;
            decimal first_step = 100;
            // pozitif tarafta mı, negatif tarafta mı ona göre ilk adımın yönünü ayarlıyoruz.
            if (Direction == "p")
            {
                first_step = 100;
            }
            if (Direction == "n")
            {
                first_step = -100;
            }
            decimal step_size = first_step;
            decimal min_step = ((Properties.Settings.Default.TDpR) / (Properties.Settings.Default.SPpR));
            bool last_point_is_plate = false;
            bool prev_point_is_plate = false;
            int remaining_step = 100; // maximum iterasyon sayısı
            decimal max_thickness_of_plate = Properties.Settings.Default.maxthickness;
            decimal min_thickness_of_plate = Properties.Settings.Default.minthickness;
            decimal[] return_of_func = { 0, 0 };
            int error_code = 0; // 0 = error, 1 = OK


            // bu while döngüsü, doğru nokta bulunana kadar ya da maximum iterasyon sayısı aşılana kadar çalışır.
            // While içerisinde Bisection MEthod algoritması döndürülür.

            while ((true_point == false) && (remaining_step > 0) && (stop_flag == false)) 
            {
                Form1._Form1.manipulate_find_edge_button("off");
                try
                {
                    return_of_func = go_position_and_measure(step_size,false, filter_coeff, offset_calib, 50);
                }
                catch (Exception ex)
                {
                    string error_string = text_file_list[41] + "\n\r" + ex;
                    yazici.durum_yazici(error_string);
                    MessageBox.Show(error_string);
                    remaining_step = 0;
                    throw;
                }
                
                if (return_of_func[0] == 1) // fonksiyon hata vermediyse
                {
                    remaining_step--; // kalan adım sayısını 1 azalt. Bir adet iterasyonu yaptık.

                    if ((return_of_func[1] < max_thickness_of_plate) && (return_of_func[1] > min_thickness_of_plate))
                    { // eğer ölçülen kalınlık, levhanın maksimum ve minimum değerleri arasında kalıyor ise,
                      // yani levha var ise, last_point_is_plate değişkenini true yapıyoruz, yani son ölçüm aldığımız nokta levhaydı diyoruz
                        prev_point_is_plate = last_point_is_plate;
                        last_point_is_plate = true;
                    }
                    else
                    {
                        // ölçülen kalınlık levha kalınlıklarıyla örtüşmüyorsa, son ölçüm noktası levha değildi diyoruz.
                        prev_point_is_plate = last_point_is_plate;
                        last_point_is_plate = false;
                    }

                    if (last_point_is_plate != prev_point_is_plate)
                    {
                        // eğer ölçüm alınan son iki nokta farklıysa, ya levhanın dışından levhaya girdik, ya da levhadan çıktık.
                        // bu durumda, daha küçük adımlarla aksi yönde gitmek gerekir.
                        
                        step_size = (step_size / (-2));
                    }

                    if ((Math.Abs(step_size) <= Math.Abs(min_step)) && (last_point_is_plate == true))
                    {

                        // ulaşılacak en küçük adım boyutuna ulaştık ve ölçüm aldığımız nokta levha ise, bu noktayı levhanın kenarı kabul ediyoruz.
                        //DialogResult dialogResult = MessageBox.Show(text_file_list[42], text_file_list[43], MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //if (dialogResult == DialogResult.Yes)
                        //{
                            int error_code_2 = 0; // 0 = Error, 1 = Ok;
                            remaining_step = 0;
                            if (reset_with_find == true)
                            {
                                motion_head.global_local_position_difference = motion_head.position_mm;
                                error_code_2 = motion_head.set_home_position(0);
                            }
                            else
                            {
                                error_code_2 = 1;
                            }
                            if (error_code_2 == 0)
                            {
                                error_code = 0;
                            }

                        //}
                    }

                }
                else
                {
                    error_code = 0;
                }
                
            }
            Form1._Form1.manipulate_find_edge_button("on");

            yazici.durum_yazici("find_plate_start ended.");
            return error_code;

        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static int close_sensor()
        {
            yazici.durum_yazici("close_sensor started.");
            return Sensor.close_sensor();
        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static int close_motion()
        {
            yazici.durum_yazici("close_motion started.");
            return motion_head.motion_close();
        }
//TESTING ALGORTIHMS
        public static void create_realistic_points()
        {
            
            int nominal_thickness = 5;
            int plate_width = 2200;
            int delta_width = 10;
            int start_of_sin = 0;
            double offset = 0.05;
            double gain = 0.1;
            int end_of_sin = 180;
            double delta_sin = plate_width/10;
            delta_sin = (end_of_sin - start_of_sin)/delta_sin;
            int sayac2 = 0;
            double delta_points = 0;
            double plate_points = 0;
            double thickness_points;
            for (int sayac = 0;sayac<=plate_width;sayac=sayac+delta_width)
            {
                
                delta_points = (gain*Math.Sin( (Math.PI/180) * sayac2*delta_sin));
                thickness_points = nominal_thickness - offset + delta_points;
                plate_points = sayac;
                measured_point mp = new measured_point(thickness_points, plate_points);
                measured_point_list.Add(mp);
                sayac2++;

            }
            
        }

//////////////////////////////////////////////////////////////////////////////////////////

        public static void create_distorted_points()
        {

            int nominal_thickness = 5;
            int plate_width = 2200;
            int delta_width = 10;
            int start_of_sin = 0;
            double offset = 0.05;
            double gain = 0.1;
            int end_of_sin = 180;
            double delta_sin = plate_width / 10;
            delta_sin = (end_of_sin - start_of_sin) / delta_sin;
            int sayac2 = 0;
            double delta_points = 0;
            double plate_points = 0;
            double thickness_points;
            Random r = new Random();
            for (int sayac = 0; sayac <= plate_width; sayac = sayac + delta_width)
            {
                int rInt = r.Next(-100, 100); //for ints
                double rndm_dbl = (double)(rInt / 50000d);
                delta_points = (gain * Math.Sin((Math.PI / 180) * sayac2 * delta_sin));
                thickness_points = nominal_thickness - offset + delta_points + rndm_dbl;
                plate_points = sayac;
                measured_point mp = new measured_point(thickness_points, plate_points);
                measured_point_list.Add(mp);
                sayac2++;

            }

        }

    }
}
