using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;


public static class Calibration
{

    Dictionary<decimal, CalibrationPoint> CalibrationPoints = new Dictionary<decimal, CalibrationPoint>();

	public static Calibration()
	{

	}

    public void ReadFromFile()
    {

    }

    public void WriteToFile()
    {

    }

    private string DataToStringLine(decimal position, CalibrationPoint _CalibrationPoint)
    {
        string str_position = position.ToString(CultureInfo.InvariantCulture);
        string str_thckness1 = _CalibrationPoint.thickness1.ToString(CultureInfo.InvariantCulture);
        string str_thckness2 = _CalibrationPoint.thickness2.ToString(CultureInfo.InvariantCulture);
        string str_realThckness1 = _CalibrationPoint.realThickness1.ToString(CultureInfo.InvariantCulture);
        string str_realThckness2 = _CalibrationPoint.realThickness2.ToString(CultureInfo.InvariantCulture);
        string str_slope = _CalibrationPoint.slope.ToString(CultureInfo.InvariantCulture);
        string str_offset = _CalibrationPoint.offset.ToString(CultureInfo.InvariantCulture);
        string line = str_position + ':' + str_thckness1 + ':' + str_thckness2 + ':' + str_realThckness1 + ':' + str_realThckness2 + ':' + str_slope + ':' + str_offset;

        return line;
    }

    private Tuple<decimal, CalibrationPoint> StringLineToData(string line)
    {
        Tuple<decimal, CalibrationPoint> result = new Tuple<decimal,CalibrationPoint>();
        string[] values = line.Split(':');
        result.Item1 = Convert.ToDecimal(values[0]);

        CalibrationPoint _CalibrationPoint = new CalibrationPoint();
        _CalibrationPoint.thickness1 = Convert.ToDecimal(values[1]);
        _CalibrationPoint.thickness2 = Convert.ToDecimal(values[2]);
        _CalibrationPoint.realThickness1 = Convert.ToDecimal(values[3]);
        _CalibrationPoint.realThickness2 = Convert.ToDecimal(values[4]);
        _CalibrationPoint.slope = Convert.ToDecimal(values[5]);
        _CalibrationPoint.offset = Convert.ToDecimal(values[6]);

        result.Item2 = _CalibrationPoint;

        return result;
    }
}
