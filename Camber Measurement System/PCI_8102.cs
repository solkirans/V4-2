/****************************************************************************/
/*  Copyright (c) 2011, ADLink Technology Inc.  All rights reserved.        */
/*                                                                          */
/*  File Name   :   PCI_8102.cs                                             */
/*                                                                          */
/*  Date        :   2011/6/13                                               */
/*  Programmer  :   Chang-Zhi Lin	                                        */
/****************************************************************************/


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;



namespace PCI8102_W32
{
		public class motion
		{
			
			// Initialization Section 6.3
			[DllImport("8102.dll")]public static extern Int16 _8102_initial(ref System.UInt16 CardID_InBit, System.Int16 Manual_ID);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_close();
            [DllImport("8102.dll")]
            public static extern Int16 _8102_get_version(System.Int16 card_id, ref System.Int16 firmware_ver, ref System.Int32 driver_ver, ref System.Int32 dll_ver);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_set_security_key(System.Int16 card_id, System.UInt16 old_secu_code, System.UInt16 New_secu_code);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_check_security_key(System.Int16 card_id, System.UInt16 secu_code);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_reset_security_key(System.Int16 card_id);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_config_from_file();
			
			
			// Pulse Input/Output Configuration Section 6.4
            [DllImport("8102.dll")]
            public static extern Int16 _8102_set_pls_outmode(System.Int16 AxisNo, System.Int16 pls_outmode);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_set_pls_iptmode(System.Int16 AxisNo, System.Int16 pls_iptmode, System.Int16 pls_logic);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_set_feedback_src(System.Int16 AxisNo, System.Int16 Src);
			
			
			// Velocity mode motion Section 6.5
            [DllImport("8102.dll")]
            public static extern Int16 _8102_tv_move(System.Int16 AxisNo, System.Double StrVel, System.Double MaxVel, System.Double Tacc);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_sv_move(System.Int16 AxisNo, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double SVacc);
            [DllImport("8102.dll")]
            public static extern Int16 _8102_emg_stop(System.Int16 AxisNo);
			[DllImport("8102.dll")]public static extern Int16 _8102_sd_stop(System.Int16 AxisNo, System.Double Tdec);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_current_speed(System.Int16 AxisNo, ref System.Double speed);
			[DllImport("8102.dll")]public static extern Int16 _8102_speed_override(System.Int16 CAxisNo, System.Double NewVelPercent, System.Double Time);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_max_override_speed(System.Int16 AxisNo, System.Double OvrdSpeed, System.Int16 Enable);
			
			
			// Single Axis Position Mode Section 6.6
			[DllImport("8102.dll")]public static extern Int16 _8102_start_tr_move(System.Int16 AxisNo, System.Double Dist, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_ta_move(System.Int16 AxisNo, System.Double Pos, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_sr_move(System.Int16 AxisNo, System.Double Dist, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, System.Double SVacc, System.Double SVdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_sa_move(System.Int16 AxisNo, System.Double Pos, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, System.Double SVacc, System.Double SVdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_move_ratio(System.Int16 AxisNo, System.Double move_ratio);
      [DllImport("8102.dll")]public static extern Int16 _8102_position_override(System.Int16 AxisNo, System.Double NewPos);
      
      
      
      // Linear Interpolated Motion Section 6.7
      [DllImport("8102.dll")]public static extern Int16 _8102_start_tr_move_xy(System.Int16 card_id,  System.Double DistX, System.Double DistY, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_ta_move_xy(System.Int16 card_id,  System.Double PosX, System.Double PosY, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_sr_move_xy(System.Int16 card_id,  System.Double DistX, System.Double DistY, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, System.Double SVacc, System.Double SVdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_sa_move_xy(System.Int16 card_id,  System.Double PosX, System.Double PosY, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, System.Double SVacc, System.Double SVdec);
      
      
      // Circular Interpolation Motion Section 6.8
      [DllImport("8102.dll")]public static extern Int16 _8102_start_tr_arc_xy(System.Int16 card_id, System.Double OffsetCx, System.Double OffsetCy, System.Double OffsetEx, System.Double OffsetEy, System.Int16 CW_CCW, System.Double StrVel,System.Double MaxVel,System.Double Tacc,System.Double Tdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_ta_arc_xy(System.Int16 card_id, System.Double Cx, System.Double Cy, System.Double Ex, System.Double Ey, System.Int16 CW_CCW, System.Double StrVel,System.Double MaxVel,System.Double Tacc,System.Double Tdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_sr_arc_xy(System.Int16 card_id, System.Double OffsetCx, System.Double OffsetCy, System.Double OffsetEx, System.Double OffsetEy, System.Int16 CW_CCW, System.Double StrVel,System.Double MaxVel,System.Double Tacc,System.Double Tdec,System.Double SVacc,System.Double SVdec);
      [DllImport("8102.dll")]public static extern Int16 _8102_start_sa_arc_xy(System.Int16 card_id, System.Double Cx, System.Double Cy, System.Double Ex, System.Double Ey, System.Int16 CW_CCW, System.Double StrVel,System.Double MaxVel,System.Double Tacc,System.Double Tdec,System.Double SVacc,System.Double SVdec);
      
      
      // Home Return Mode Section 6.9
      [DllImport("8102.dll")]public static extern Int16 _8102_set_home_config(System.Int16 AxisNo, System.Int16 home_mode, System.Int16 org_logic, System.Int16 ez_logic, System.Int16 ez_count, System.Int16 erc_out);
      [DllImport("8102.dll")]public static extern Int16 _8102_home_move(System.Int16 AxisNo, System.Double StrVel, System.Double MaxVel, System.Double Tacc);
      [DllImport("8102.dll")]public static extern Int16 _8102_home_search(System.Int16 AxisNo, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double ORGOffset);
      
      
      // Manual Pulser Motion Section 6.10
      [DllImport("8102.dll")]public static extern Int16 _8102_disable_pulser_input(System.Int16 AxisNo, System.UInt16  Disable );
      [DllImport("8102.dll")]public static extern Int16 _8102_pulser_pmove(System.Int16 AxisNo, System.Double Dist, System.Double SpeedLimit);
      [DllImport("8102.dll")]public static extern Int16 _8102_pulser_vmove(System.Int16 AxisNo, System.Double SpeedLimit);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_pulser_ratio(System.Int16 AxisNo, System.Int16 DivF, System.Int16 MultiF);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_pulser_iptmode(System.Int16 AxisNo, System.Int16 InputMode, System.Int16 Inverse);
      
      // Motion Status Section 6.11
      [DllImport("8102.dll")]public static extern Int16 _8102_motion_done(System.Int16 AxisNo);
      
      
      // Motion Interface I/O Section 6.12
      [DllImport("8102.dll")]public static extern Int16 _8102_set_servo(System.Int16 AxisNo, System.Int16 on_off);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_pcs_logic(System.Int16 AxisNo, System.Int16 pcs_logic);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_pcs(System.Int16 AxisNo, System.Int16 enable);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_clr_mode(System.Int16 AxisNo, System.Int16 clr_mode);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_sd(System.Int16 AxisNo, System.Int16 sd_logic, System.Int16 sd_latch, System.Int16 sd_mode);
      [DllImport("8102.dll")]public static extern Int16 _8102_enable_sd(System.Int16 AxisNo, System.Int16 enable);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_inp(System.Int16 AxisNo, System.Int16 inp_enable, System.Int16 inp_logic);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_erc(System.Int16 AxisNo, System.Int16 erc_logic, System.Int16 erc_pulse_width);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_alm(System.Int16 AxisNo, System.Int16 alm_logic, System.Int16 alm_mode);
      [DllImport("8102.dll")]public static extern Int16 _8102_get_io_status(System.Int16 AxisNo, ref System.UInt16 io_sts);
      [DllImport("8102.dll")]public static extern Int16 _8102_set_limit_logic(System.Int16 AxisNo, System.Int16 Logic );
      [DllImport("8102.dll")]public static extern Int16 _8102_set_limit_mode(System.Int16 AxisNo, System.Int16 limit_mode);
      [DllImport("8102.dll")]public static extern Int16 _8102_select_pin23_input(System.Int16 card_id, System.UInt16  Select );
      [DllImport("8102.dll")]public static extern Int16 _8102_select_pin57_input(System.Int16 card_id, System.UInt16  Select );
      
      
      
      // Interrupt Control Section 6.13
      [DllImport("8102.dll")]public static extern Int16 _8102_int_control(System.Int16 card_id, System.Int16 intFlag);
      [DllImport("8102.dll")]public static extern Int16 _8102_wait_error_interrupt(System.Int16 AxisNo, System.Int32 TimeOut_ms );
      [DllImport("8102.dll")]public static extern Int16 _8102_set_motion_int_factor(System.Int16 AxisNo, System.UInt32 int_factor );
      [DllImport("8102.dll")]public static extern Int16 _8102_wait_motion_interrupt(System.Int16 AxisNo, System.Int16 IntFactorBitNo, System.Int32 TimeOut_ms );
      [DllImport("8102.dll")]public static extern Int16 _8102_set_gpio_int_factor(System.Int16 card_id, System.UInt16 int_factor );
      [DllImport("8102.dll")]public static extern Int16 _8102_wait_gpio_interrupt(System.Int16 card_id, System.Int16 IntFactorBitNo, System.Int32 TimeOut_ms );
      
      
      // Position Control and Counters Section 6.14
			[DllImport("8102.dll")]public static extern Int16 _8102_get_position(System.Int16 AxisNo, ref System.Double Pos);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_position(System.Int16 AxisNo, System.Double Pos);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_command(System.Int16 AxisNo, ref System.Int32 Cmd);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_command(System.Int16 AxisNo, System.Int32 Cmd);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_error_counter(System.Int16 AxisNo, ref System.Int16 error);
			[DllImport("8102.dll")]public static extern Int16 _8102_reset_error_counter(System.Int16 AxisNo);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_general_counter(System.Int16 AxisNo, System.Int16 CntSrc, System.Double CntValue);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_general_counter(System.Int16 AxisNo, ref System.Double pos);
			[DllImport("8102.dll")]public static extern Int16 _8102_reset_target_pos(System.Int16 AxisNo, System.Double Pos);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_target_pos(System.Int16 AxisNo, ref System.Double pos);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_res_distance(System.Int16 AxisNo, System.Double Res_Distance);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_res_distance(System.Int16 AxisNo, ref System.Double Res_Distance);
			[DllImport("8102.dll")]public static extern Int16 _8102_clr_counter_select(System.Int16 AxisNo, System.Int16 CounterBit);


			// Position Compare and Latch Section 6.15
			[DllImport("8102.dll")]public static extern Int16 _8102_set_trigger_logic(System.Int16 AxisNo, System.Int16 Logic);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_error_comparator(System.Int16 AxisNo, System.Int16 CmpMethod, System.Int16 CmpAction, System.Int32  Data);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_general_comparator(System.Int16 AxisNo, System.Int16 CmpSrc, System.Int16 CmpMethod, System.Int16 CmpAction, System.Int32 Data);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_trigger_comparator(System.Int16 AxisNo, System.Int16 CmpSrc, System.Int16 CmpMethod, System.Int32 Data);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_latch_source(System.Int16 AxisNo, System.Int16 ltc_src);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_ltc_logic(System.Int16 AxisNo, System.Int16 ltc_logic);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_latch_data(System.Int16 AxisNo, System.Int16 CounterNo, ref System.Double Pos);


			// Continuous Motion Section 6.16
			[DllImport("8102.dll")]public static extern Int16 _8102_set_continuous_move(System.Int16 AxisNo, System.Int16 conti_logic);
			[DllImport("8102.dll")]public static extern Int16 _8102_check_continuous_buffer(System.Int16 AxisNo);
			[DllImport("8102.dll")]public static extern Int16 _8102_dwell_move(System.Int16 AxisNo, System.Double miniSecond);


			// Multiple Axes Simultaneous Operation Section 6.17
			[DllImport("8102.dll")]public static extern Int16 _8102_set_tr_move_all(System.Int16 TotalAxes, ref System.Int16 AxisArray, ref System.Double DistA, ref System.Double StrVelA, ref System.Double MaxVelA, ref System.Double TaccA, ref System.Double TdecA);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_sa_move_all(System.Int16 TotalAx, ref System.Int16 AxisArray, ref System.Double PosA, ref System.Double StrVelA, ref System.Double MaxVelA, ref System.Double TaccA, ref System.Double TdecA, ref System.Double SVaccA, ref System.Double SVdecA);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_ta_move_all(System.Int16 TotalAx, ref System.Int16 AxisArray, ref System.Double PosA, ref System.Double StrVelA, ref System.Double MaxVelA, ref System.Double TaccA, ref System.Double TdecA);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_sr_move_all(System.Int16 TotalAx, ref System.Int16 AxisArray, ref System.Double DistA, ref System.Double StrVelA, ref System.Double MaxVelA, ref System.Double TaccA, ref System.Double TdecA, ref System.Double SVaccA, ref System.Double SVdecA);
			[DllImport("8102.dll")]public static extern Int16 _8102_start_move_all(System.Int16 FirstAxisNo);
			[DllImport("8102.dll")]public static extern Int16 _8102_stop_move_all(System.Int16 FirstAxisNo);
			
			
			
			// General-purposed TTL Output Section 6.18
			[DllImport("8102.dll")]public static extern Int16 _8102_set_gpio_output(System.Int16 card_id, System.UInt16 do_value );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_output(System.Int16 card_id, ref System.UInt16 do_status );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_input(System.Int16 card_id, ref System.UInt16 di_status );
			[DllImport("8102.dll")]public static extern Int16 _8102_set_gpio_output2(System.Int16 card_id, System.UInt16 do_value );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_output2(System.Int16 card_id, ref System.UInt16 do_status );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_input2(System.Int16 card_id, ref System.UInt16 di_status );
			
			// Soft Limit Section 6.19
			[DllImport("8102.dll")]public static extern Int16 _8102_disable_soft_limit(System.Int16 AxisNo);
			[DllImport("8102.dll")]public static extern Int16 _8102_enable_soft_limit(System.Int16 AxisNo, System.Int16 Action);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_soft_limit(System.Int16 AxisNo, System.Int32 Plus_Limit, System.Int32 Neg_Limit);
			
			// Backlash Compensation / Vibratin Suppression Ssection 6.20
			[DllImport("8102.dll")]public static extern Int16 _8102_backlash_comp(System.Int16 AxisNo, System.Int16 CompPulse, System.Int16 Mode);
			[DllImport("8102.dll")]public static extern Int16 _8102_suppress_vibration(System.Int16 AxisNo, System.UInt16 ReverseTime, System.UInt16 ForwardTime);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_fa_speed(System.Int16 AxisNo, System.Double FA_Speed);


			// Speed Profile Calculatin Section 6.21
			[DllImport("8102.dll")]public static extern Int16 _8102_get_tr_move_profile(System.Int16 AxisNo, System.Double Dist, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, ref System.Double pStrVel, ref System.Double pMaxVel, ref System.Double pTacc, ref System.Double pTdec, ref System.Double pTconst );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_ta_move_profile(System.Int16 AxisNo, System.Double Pos, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, ref System.Double pStrVel, ref System.Double pMaxVel, ref System.Double pTacc, ref System.Double pTdec, ref System.Double pTconst );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_sr_move_profile(System.Int16 AxisNo, System.Double Dist, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, System.Double SVacc, System.Double SVdec,ref System.Double pStrVel, ref System.Double pMaxVel, ref System.Double pTacc, ref System.Double pTdec, ref System.Double pSVacc, ref System.Double pSVdec, ref System.Double pTconst);
			[DllImport("8102.dll")]public static extern Int16 _8102_get_sa_move_profile(System.Int16 AxisNo, System.Double Pos, System.Double StrVel, System.Double MaxVel, System.Double Tacc, System.Double Tdec, System.Double SVacc, System.Double SVdec,ref System.Double pStrVel, ref System.Double pMaxVel, ref System.Double pTacc, ref System.Double pTdec, ref System.Double pSVacc, ref System.Double pSVdec, ref System.Double pTconst);

			// New funciton 
			[DllImport("8102.dll")]public static extern Int16 _8102_ring_counter_set(System.Int16 AxisNo, System.Int16 CmpSrc, System.Int16 CmpMethod, System.Int32 Data);
			[DllImport("8102.dll")]public static extern Int16 _8102_set_ring_counter(System.Int16 AxisNo, System.Int32 RingVal );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_ring_counter(System.Int16 AxisNo, ref System.Int32 RingVal );
			  
      // Customized function
      [DllImport("8102.dll")]public static extern Int16 _8102_set_gpio_output_n(System.Int16 card_id, System.UInt16 do_value );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_output_n(System.Int16 card_id, ref System.UInt16 do_status );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_input_n(System.Int16 card_id, ref System.UInt16 di_status );
			[DllImport("8102.dll")]public static extern Int16 _8102_set_gpio_output2_n(System.Int16 card_id, System.UInt16 do_value );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_output2_n(System.Int16 card_id, ref System.UInt16 do_status );
			[DllImport("8102.dll")]public static extern Int16 _8102_get_gpio_input2_n(System.Int16 card_id, ref System.UInt16 di_status );
				
		}
	
}