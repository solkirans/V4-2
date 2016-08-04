using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Camber_Measurement_System
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            yazici.initialization();
            yazici.durum_yazici("Started. Software Version 1.4");
            
            yazici.durum_yazici("");
            yazici.durum_yazici("Acik olan Excel uygulamaları kapatılacak.");
            try
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

            Application.Run(new Form1());
        }
    }
}
