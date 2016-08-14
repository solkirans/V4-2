using System;

public class CalibrationPoint
{
    public decimal thickness1;
    public decimal thickness2;
    public decimal realThickness1;
    public decimal realThickness2;
    public decimal global_position;
    public decimal slope;
    public decimal offset;

    public CalibrationPoint()
    {

    }
    public void Calibrate()
	{
        decimal slope = (thickness1 - thickness2) / (realThickness1 - realThickness2);
        decimal offset = thickness1 - (slope * realThickness1);
	}
}
