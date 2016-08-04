using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Camber_Measurement_System
{
    class Points
    {
        private decimal position;
        private decimal thickness;
        private Dictionary<decimal, decimal> meas_list = new Dictionary<decimal, decimal>();


        public int insert_element(decimal pos, decimal thick)
        {
            int status = 0; // 0 = error, 1 = Ok
            try
            {
                meas_list.Add(pos, thick);
                status = 1;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Error at point insertion!\n\r" + ex);
            }
            return status;
        }
    }


}
