using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Reflection;
/*using QiHe.CodeLib;
using ExcelLibrary.CompoundDocumentFormat;
using ExcelLibrary.BinaryDrawingFormat;
using ExcelLibrary.BinaryFileFormat;
using ExcelLibrary.SpreadSheet;
 * */

/*
namespace Weighing_machine_Automation
{
    public static class yazici_excel
    {
       public static int yeni_satir_ekle(string NO, string agirlik, string adet, string birim_agirlik, string tarih)
        {
            int hata_var_mi = 0;

            dosya_yarat();

            return hata_var_mi;
        }

        private static int dosya_yarat()
        {
            int hata_var_mi = 0;
            string file = "olcumler.xls";
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("Olcumler");
            worksheet.Cells[0, 1] = new Cell("Deneme1");
            worksheet.Cells[2, 0] = new Cell("Deneme2");
            worksheet.Cells[3, 3] = new Cell("Deneme3");
            worksheet.Cells[2, 2] = new Cell("Deneme4");
            worksheet.Cells[2, 4] = new Cell("Deneme5");
            worksheet.Cells[4, 0] = new Cell("Deneme6");
            worksheet.Cells[5, 1] = new Cell("Deneme7");
            worksheet.Cells.ColumnWidth[0, 1] = 3000;
            workbook.Worksheets.Add(worksheet);
            workbook.Save(file);


            return hata_var_mi;
        }
        
    }
}
*/

namespace Camber_Measurement_System
{
    public static class yazici_excel
    {

        //string[,] hucreler;

        public static void yeni_satir_ekle()
        {

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
            bolge = (Excel.Range)ExcelSayfa.Cells[1, 3];
            bolge.Value2 = "Agirlik";
            bolge = (Excel.Range)ExcelSayfa.Cells[1, 4];
            bolge.Value2 = "Birim Agirlik";
            bolge = (Excel.Range)ExcelSayfa.Cells[1, 5];
            bolge.Value2 = "Tarih / Saat";

            string s_dosyaadi = "olcumler.xlsx";
            if (s_dosyaadi != "")
            {
                ExcelProje.SaveAs(s_dosyaadi, Excel.XlFileFormat.xlWorkbookDefault, Missing, Missing, false, Missing, Excel.XlSaveAsAccessMode.xlNoChange);
                ExcelProje.Close(true, Missing, Missing);
                ExcelUygulama.Quit();
            }
        }


    }
}