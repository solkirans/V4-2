using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class Calibration
{

    private List<CalibrationPoint> list;

    public Calibration()
    {
        list = new List<CalibrationPoint>();
        // TODO: Complete member initialization
    }
/*
    public void ReadFromFile(string file_name)
    {

    }

    public void WriteToFile(string file_name)
    {
        if (File.Exists(file_name))
        {
            File.Delete(file_name);
        }

    }

    private string DataToStringLine(decimal position, CalibrationPoint _CalibrationPoint)
    {
        string str_thckness1 = _CalibrationPoint.thickness1.ToString(CultureInfo.InvariantCulture);
        string str_thckness2 = _CalibrationPoint.thickness2.ToString(CultureInfo.InvariantCulture);
        string str_realThckness1 = _CalibrationPoint.realThickness1.ToString(CultureInfo.InvariantCulture);
        string str_realThckness2 = _CalibrationPoint.realThickness2.ToString(CultureInfo.InvariantCulture);
        string str_global_position = _CalibrationPoint.global_position.ToString(CultureInfo.InvariantCulture);
        string str_slope = _CalibrationPoint.slope.ToString(CultureInfo.InvariantCulture);
        string str_offset = _CalibrationPoint.offset.ToString(CultureInfo.InvariantCulture);
        string line = str_thckness1 + ':' + str_thckness2 + ':' + str_realThckness1 + ':' + str_realThckness2 + ':' + str_global_position  + ':' + str_slope + ':' + str_offset;

        return line;
    }

    private CalibrationPoint StringLineToData(string line)
    {
        
        string[] values = line.Split(':');

        CalibrationPoint _CalibrationPoint = new CalibrationPoint();
        _CalibrationPoint.thickness1 = Convert.ToDecimal(values[1]);
        _CalibrationPoint.thickness2 = Convert.ToDecimal(values[2]);
        _CalibrationPoint.realThickness1 = Convert.ToDecimal(values[3]);
        _CalibrationPoint.realThickness2 = Convert.ToDecimal(values[4]);
        _CalibrationPoint.global_position = Convert.ToDecimal(values[5]);
        _CalibrationPoint.slope = Convert.ToDecimal(values[6]);
        _CalibrationPoint.offset = Convert.ToDecimal(values[7]);

        return _CalibrationPoint;
    }*/
}
