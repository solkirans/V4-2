using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace Camber_Measurement_System
{
    static class yazici
    {

        static int flag = 0;
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        static List<string> list_of_strings;
        static Thread writer_thread;
        static int error_flag = 0;

        public static void initialization()
        {
            list_of_strings = new List<string>();
            writer_thread = new Thread((ThreadStart)delegate { list_checker(); });
            writer_thread.IsBackground = true;
            writer_thread.Start();
            error_flag = 0;
        }
        private static void list_checker()
        {

            while (true)
            {

                if ((list_of_strings.Count > 0) && (error_flag == 0))
                {
                    temel_yazici(list_of_strings[0]);
                    list_of_strings.RemoveAt(0);
                }
                Thread.Sleep(100);
            }

        
        }
        private static void temel_yazici(string yazi)
        {
            string durum_dosyasi_adi = Application.StartupPath + "\\Error_log.txt";
            durum_dosyasi_adi = "Error_log.txt";

            string time = Convert.ToString(DateTime.Now);
            try
            {
                if (flag == 0)
                {
                    flag = 1;
                    File.AppendAllText(durum_dosyasi_adi, Environment.NewLine + time + "   " + yazi);
                    flag = 0;
                }

                /*
                using (StreamWriter w = File.AppendText(durum_dosyasi_adi))
                {
                    w.WriteLine(time + "   " + yazi);
                    w.Close();
                }*/

            }
            catch (Exception ex)
            {
                MessageBox.Show("LLof file writing error.\n\r\n\rClose Error Log Files and restart the software." + ex);
                error_flag = 1;
                throw;
            }
        }
        public static int durum_yazici(string yazi)
        {
            int hata_var_mi = 0;
            list_of_strings.Insert(list_of_strings.Count(), yazi);
            return hata_var_mi;
        }

    }
}
