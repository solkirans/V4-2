using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camber_Measurement_System
{
    class measured_point
    {
        public double measured_thickness = 0;
        public double measured_position = 0;
        string message_string;
        public measured_point(double _measured_thickness, double _measured_position)
        {
            //int error_code = 0; // 0 = error, 1 = ok

            if ( _measured_thickness > 0 )
                this.measured_thickness = _measured_thickness;
            else
            {
                this.message_string = "Measured thickness has negative value.\n\rMeasured thickness is:" + Convert.ToString(_measured_thickness);
                System.Windows.Forms.MessageBox.Show(this.message_string);
                yazici.durum_yazici(this.message_string);
            }

            if (_measured_position >= 0)
                this.measured_position = _measured_position;
            else
            {
                this.message_string = "Measured position has negative value.\n\rMeasured position is:" + Convert.ToString(_measured_position);
                System.Windows.Forms.MessageBox.Show(this.message_string);
                yazici.durum_yazici(this.message_string);
            }
        }
    }
}
