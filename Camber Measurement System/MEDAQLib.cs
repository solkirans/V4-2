using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Camber_Measurement_System
{
	class MEDAQLib
	{
		public enum ERR_CODE
		{
			ERR_NOERROR = 0,
			ERR_FUNTION_NOT_SUPPORTED = -1,
			ERR_CANNOT_OPEN = -2,
			ERR_NOT_OPEN = -3,
			ERR_APPLYING_PARAMS = -4,
			ERR_SEND_CMD_TO_SENSOR = -5,
			ERR_CLEARUNG_BUFFER = -6,
			ERR_HW_COMMUNICATION = -7,
			ERR_TIMEOUT_READING_FROM_SENSOR = -8,
			ERR_READING_SENSOR_DATA = -9,
			ERR_INTERFACE_NOT_SUPPORTED = -10,
			ERR_ALREADY_OPEN = -11,
			ERR_CANNOT_CREATE_INTERFACE = -12,
			ERR_NO_SENSORDATA_AVAILABLE = -13,
			ERR_UNKNOWN_SENSOR_COMMAND = -14,
			ERR_UNKNOWN_SENSOR_ANSWER = -15,
			ERR_SENSOR_ANSWER_ERROR = -16,
			ERR_SENSOR_ANSWER_TOO_SHORT = -17,
			ERR_WRONG_PARAMETER = -18,
			ERR_NOMEMORY = -19,
			ERR_NO_ANSWER_RECEIVED = -20,
			ERR_SENSOR_ANSWER_DOES_NOT_MATCH_COMMAND = -21,
			ERR_BAUDRATE_TOO_LOW = -22,
			ERR_OVERFLOW = -23,
			ERR_INSTANCE_NOT_EXTST = -24,
			ERR_NOT_FOUND = -25,
			ERR_WARNING = -26,
			ERR_SENSOR_ANSWER_WARNING = -27,
		}

		public enum ME_SENSOR
		{
			SENSOR_ILR110x_115x = 19, // optoNCDT ILR
			SENSOR_ILR118x = 20, // optoNCDT ILR
			SENSOR_ILR1191 = 21, // optoNCDT ILR
			SENSOR_ILD1302 = 24, // optoNCDT
			SENSOR_ILD1401 = 1, // optoNCDT
			CONTROLLER_ILD1401 = 14, // Demo case for ILD1401
			SENSOR_ILD1402 = 23, // optoNCDT
			SENSOR_ILD1700 = 2, // optoNCDT
			SENSOR_ILD1800 = 3, // optoNCDT
			SENSOR_ILD2000 = 4, // optoNCDT
			SENSOR_ILD2200 = 5, // optoNCDT
			SENSOR_ILD2300 = 29, // optoNCDT
			SENSOR_IFD2400 = 6, // confocalDT
			SENSOR_IFD2401 = 12, // confocalDT
			SENSOR_IFD2430 = 7, // confocalDT
			SENSOR_IFD2431 = 13, // confocalDT
			SENSOR_IFD2451 = 30, // confocalDT
			SENSOR_IFD2471 = 26, // confocalDT
			SENSOR_ODC1202 = 25, // optoCONTROL
			SENSOR_ODC2500 = 8, // optoCONTROL
			SENSOR_ODC2600 = 9, // optoCONTROL
			CONTROLLER_DT3909 = 27, // eddyNCDT
			CONTROLLER_ESC4912 = 17, // eddyNCDT
			SENSOR_DT3100 = 28, // eddyNCDT
			SENSOR_DT6100 = 16, // capaNCDT
			CONTROLLER_DT6200 = 33, // capaNCDT
			CONTROLLER_KSS6380 = 18, // capaNCDT
			CONTROLLER_DT6500 = 15, // capaNCDT
			ENCODER_IF2004 = 10, // PCI card IF2004
			PCI_CARD_IF2008 = 22, // PCI card IF2008
			SENSOR_LLT27xx = 31, // scanCONTROL+gapCONTROL, only for SensorFinder functionality, OpenSensor will fail
			CONTROLLER_CSP2008 = 32, // Universal controller
		};

		// functions provided by MEDAQLib.dll
		[DllImport("MEDAQlib.dll")]
		public static extern UInt32 CreateSensorInstance(ME_SENSOR sensor);
		[DllImport("MEDAQlib.dll")]
		public static extern UInt32 CreateSensorInstByName(String sensorName);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE ReleaseSensorInstance(UInt32 instanceHandle);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetParameterInt(UInt32 instanceHandle, String paramName, Int32 paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetParameterDWORD(UInt32 instanceHandle, String paramName, UInt32 paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetParameterDouble(UInt32 instanceHandle, String paramName, Double paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetParameterString(UInt32 instanceHandle, String paramName, String paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetParameterStringL(UInt32 instanceHandle, String paramName, String paramValue, UInt32 len);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetParameters(UInt32 instanceHandle, String paramList);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE GetParameterInt(UInt32 instanceHandle, String paramName, ref Int32 paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE GetParameterDWORD(UInt32 instanceHandle, String paramName, ref UInt32 paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE GetParameterDouble(UInt32 instanceHandle, String paramName, ref Double paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE GetParameterString(UInt32 instanceHandle, String paramName, StringBuilder paramValue, ref UInt32 maxLen);
		[DllImport("MEDAQlib.dll", EntryPoint = "GetParameterString")]
		public static extern ERR_CODE GetParameterBinary(UInt32 instanceHandle, String paramName, ushort[] paramValue, ref UInt32 maxLen);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE ClearAllParameters(UInt32 instanceHandle);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensor(UInt32 instanceHandle);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE CloseSensor(UInt32 instanceHandle);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SensorCommand(UInt32 instanceHandle);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE DataAvail(UInt32 instanceHandle, ref Int32 avail);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE TransferData(UInt32 instanceHandle, [MarshalAs(UnmanagedType.LPArray)] Int32[] rawData, [MarshalAs(UnmanagedType.LPArray)] Double[] scaledData, Int32 maxValues, ref Int32 read);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE Poll(UInt32 instanceHandle, [MarshalAs(UnmanagedType.LPArray)] Int32[] rawData, [MarshalAs(UnmanagedType.LPArray)] Double[] scaledData, Int32 maxValues);
		[DllImport("MEDAQLib.dll")]
		public static extern ERR_CODE GetError(UInt32 instanceHandle, StringBuilder errText, UInt32 maxLen);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE GetDLLVersion(StringBuilder versionStr, UInt32 maxLen);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE EnableLogging(UInt32 instanceHandle, Int32 enableLogging, Int32 logType, Int32 logLevel, String logFile, Int32 logAppend, Int32 logFlush);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensorRS232(UInt32 instanceHandle, String port);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensorIF2004(UInt32 instanceHandle, Int32 cardInstance, Int32 channelNumber);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensorIF2008(UInt32 instanceHandle, Int32 cardInstance, Int32 channelNumber);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensorTCPIP(UInt32 instanceHandle, String remoteAddr);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensorDriverX_USB(UInt32 instanceHandle, Int32 deviceInstance);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensorUSBIO(UInt32 instanceHandle, Int32 deviceInstance);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE OpenSensorWinUSB(UInt32 instanceHandle, Int32 deviceInstance);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE ExecSCmd(UInt32 instanceHandle, String sensorCommand);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetIntExecSCmd(UInt32 instanceHandle, String sensorCommand, String paramName, Int32 paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetDoubleExecSCmd(UInt32 instanceHandle, String sensorCommand, String paramName, Double paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE SetStringExecSCmd(UInt32 instanceHandle, String sensorCommand, String paramName, String paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE ExecSCmdGetInt(UInt32 instanceHandle, String sensorCommand, String paramName, ref Int32 paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE ExecSCmdGetDouble(UInt32 instanceHandle, String sensorCommand, String paramName, ref Double paramValue);
		[DllImport("MEDAQlib.dll")]
		public static extern ERR_CODE ExecSCmdGetString(UInt32 instanceHandle, String sensorCommand, String paramName, StringBuilder paramValue, ref UInt32 maxLen);
	}
}
