using System;

public class CalibrationPoint
{
    public float thickness1;
    public float thickness2;
    public float realThickness1;
    public float realThickness2;
    public float slope;
    public float offset;

    public void Calibrate()
	{
        float slope = (thickness1 - thickness2) / (realThickness1 - realThickness2);
        float offset = thickness1 - (slope * realThickness1);
	}
}
