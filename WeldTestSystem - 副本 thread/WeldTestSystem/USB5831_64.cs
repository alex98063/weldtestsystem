using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WeldTestSystem
{
    public partial class  USB5831
    {

        //***********************************************************
        // 用于AD采集的参数结构
        public struct USB5831_PARA_AD 
        {
            public Int32 CheckStsMode;		// 检查状态模式
            public Int32 ADMode;            // AD模式选择(连续采集/分组采集)
            public Int32 FirstChannel;      // 首通道,取值范围为[0, 15]
            public Int32 LastChannel;		// 末通道,取值范围为[0, 15]
            public Int32 Frequency;         // 采集频率,单位为Hz,取值范围为[31, 250000]
            public Int32 InputRange;		// 量程选择
            public Int32 GroupInterval;     // 分组采样时的组间间隔(单位：微秒),取值范围为[1, 32767]
            public Int32 LoopsOfGroup;		// 分组采样时，每组循环次数，取值范围为[1, 255]
            public Int32 Gains;				// 增益控制字
            public Int32 TriggerMode;       // 触发模式选择(软件触发、后触发)
            public Int32 TriggerSource;		// 触发源选择(ART、DTR)
            public Int32 TriggerType;		// 触发类型选择(边沿触发/脉冲触发)
            public Int32 TriggerDir;		// 触发方向选择(正向/负向触发)
            public Int32 TrigWindow;		// 触发灵敏度(1-255)，单位:0.5微秒
            public Int32 GroundingMode;		// 接地方式（单端或双端选择）
            public Int32 ClockSource;		// 时钟源选择(内/外时钟源)
            public Int32 bClockOutput;      // 是否允许本地AD转换时钟输出，=TRUE:允许输出到CN1上的CLKOUT，=FALSE:禁止输出到CN1上的CLKOUT
        }

        // AD硬件参数USB5831_PARA_AD中的CheckStsMode检查状态的模式所使用的选项
        public const Int32 USB5831_CHKSTSMODE_HALF = 0x00; // 查询FIFO半满标志(建议高频率采集时使用)
        public const Int32 USB5831_CHKSTSMODE_NPT = 0x01; // 查询FIFO非空标志(建议高实时采集时使用)

        // AD硬件参数USB5831_PARA_AD中的ADMode工作模式所使用的选项
        public const Int32 USB5831_ADMODE_SEQUENCE = 0x00; // 连续采样
        public const Int32 USB5831_ADMODE_GROUP = 0x01; // 分组采样

        //***********************************************************
        // AD硬件参数USB5831_PARA_AD中的InputRange量程所使用的选项
        public const Int32 USB5831_INPUT_N10000_P10000mV = 0x00; // ±10000mV
        public const Int32 USB5831_INPUT_N5000_P5000mV = 0x01; // ±5000mV
        public const Int32 USB5831_INPUT_N2500_P2500mV = 0x02; // ±2500mV
        public const Int32 USB5831_INPUT_0_P10000mV = 0x03; // 0～10000mV

        //***********************************************************
        // AD参数USB5831_PARA_AD中的Gains使用的硬件增益选项
        public const Int32 USB5831_GAINS_1MULT = 0x00; // 1倍增益(使用AD8251放大器)
        public const Int32 USB5831_GAINS_2MULT = 0x01; // 2倍增益(使用AD8251放大器)
        public const Int32 USB5831_GAINS_4MULT = 0x02; // 4倍增益(使用AD8251放大器)
        public const Int32 USB5831_GAINS_8MULT = 0x03; // 8倍增益(使用AD8251放大器)

        //***********************************************************
        // AD硬件参数USB5831_PARA_AD中的TriggerMode成员变量所使用AD触发模式选项
        public const Int32 USB5831_TRIGMODE_SOFT = 0x00; // 软件触发(属于内触发)
        public const Int32 USB5831_TRIGMODE_POST = 0x01; // 硬件后触发(属于外触发)

        //***********************************************************
        // AD硬件参数USB5831_PARA_AD中的TriggerSource成员变量所使用AD触发源选项
        public const Int32 USB5831_TRIGSOURCE_ATR = 0x00; // ATR触发
        public const Int32 USB5831_TRIGSOURCE_DTR = 0x01; // DTR触发

        // AD硬件参数USB5831_PARA_AD中的TriggerType触发类型所使用的选项
        public const Int32 USB5831_TRIGTYPE_EDGE = 0x00; // 边沿触发
        public const Int32 USB5831_TRIGTYPE_PULSE = 0x01; // 脉冲触发(电平类型)

        //***********************************************************
        // AD硬件参数USB5831_PARA_AD中的TriggerDir触发方向所使用的选项
        public const Int32 USB5831_TRIGDIR_NEGATIVE = 0x00; // 负向触发(低电平/下降沿触发)
        public const Int32 USB5831_TRIGDIR_POSITIVE = 0x01; // 正向触发(高电平/上升沿触发)
        public const Int32 USB5831_TRIGDIR_POSIT_NEGAT = 0x02; // 正负向触发(高/低电平或上升/下降沿触发)

        //***********************************************************
        // AD参数(USB5831_PARA_AD)中的GroundingMode使用的模拟信号接地方式选项
        public const Int32 USB5831_GNDMODE_SE = 0x00;	// 单端方式(SE:Single end)
        public const Int32 USB5831_GNDMODE_DI = 0x01;	// 双端方式(DI:Differential)

        //***********************************************************
        // AD硬件参数USB5831_PARA_AD中的ClockSource时钟源所使用的选项
        public const Int32 USB5831_CLOCKSRC_IN  = 0x00; // 内部时钟定时触发
        public const Int32 USB5831_CLOCKSRC_OUT = 0x01; // 外部时钟定时触发(使用CN1上的CLKIN信号输入)

        //***********************************************************
        // DA输出函数USB5831_WriteDeviceDA的模拟量输出范围参数OutputRange所使用的选项
        public const Int32 USB5831_OUTPUT_0_P5000mV     = 0x00;		// 0～5000mV
        public const Int32 USB5831_OUTPUT_0_P10000mV    = 0x01;		// 0～10000mV
        public const Int32 USB5831_OUTPUT_0_P10800mV    = 0x02;		// 0～10800mV
        public const Int32 USB5831_OUTPUT_N5000_P5000mV = 0x03;		// ±5000mV
        public const Int32 USB5831_OUTPUT_N10000_P10000mV = 0x04;	// ±10000mV
        public const Int32 USB5831_OUTPUT_N10800_P10800mV = 0x05;	// ±10800mV

        //######################## 常规通用函数 #################################
               [DllImport("USB5831_64.DLL")]
        public static extern IntPtr USB5831_CreateDevice(Int32 DeviceLgcID); // 创建设备对象(该函数使用系统内逻辑设备ID）
               [DllImport("USB5831_64.DLL")]
        public static extern IntPtr USB5831_CreateDeviceEx(Int32 DevicePhysID); // 创建设备对象(该函数使用板上物理ID,由拔码开关DID1实现)
               [DllImport("USB5831_64.DLL")]
        public static extern Int32 USB5831_GetDeviceCount(IntPtr hDevice);      // 取得USB5831在系统中的设备数量
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_GetDeviceCurrentID(IntPtr hDevice, ref Int32 DeviceLgcID, ref Int32 DevicePhysID); // 取得当前设备的逻辑ID号和物理ID号
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_ListDeviceDlg(); // 用对话框列表系统当中的所有USB5831设备
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_ResetDevice(IntPtr hDevice);		 // 复位整个USB设备
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_ReleaseDevice(IntPtr hDevice);    // 设备句柄

	    //####################### AD数据读取函数 #################################
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_InitDeviceAD(				// 初始化设备,当返回TRUE后,设备即刻开始传输.
									    IntPtr hDevice ,				// 设备句柄,它应由CreateDevice函数创建
									    ref USB5831_PARA_AD pADPara);  // 硬件参数, 它仅在此函数中决定硬件状态							
            
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_ReadDeviceAD(				// 初始化设备后，即可用此函数读取设备上的AD数据
									    IntPtr hDevice ,				// 设备句柄,它应由CreateDevice函数创建
									    UInt16[] ADBuffer,			// 将用于接受数据的用户缓冲区
									    Int32 nReadSizeWords,		// 读取AD数据的长度(字)  
									    ref Int32 nRetSizeWords);// 实际返回数据的长度(字)
            
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_ReleaseDeviceAD(IntPtr hDevice); // 停止AD采集，释放AD对象所占资源

   	    //################# AD的硬件参数操作函数 ########################	
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_SaveParaAD(IntPtr hDevice, ref USB5831_PARA_AD pADPara); 
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_LoadParaAD(IntPtr hDevice, ref USB5831_PARA_AD pADPara);
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_ResetParaAD(IntPtr hDevice, ref USB5831_PARA_AD pADPara); // 将AD采样参数恢复至出厂默认值

	    //####################### DA数据输出函数 #################################
	    // 适于大多数普通用户，这些接口最简单、最快捷、最可靠，让用户不必知道设备
	    // 低层复杂的硬件控制协议和繁多的软件控制编程，仅用下面一个函数便能轻
	    // 松实现高速、连续的DA数据输出
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_WriteDeviceDA(			// 写DA数据
									    IntPtr hDevice ,			// 设备对象句柄,它由CreateDevice函数创建
									    Int32 OutputRange,		// 输出量程，具体定义请参考上面的常量定义部分
									    Int16 nDAData,			// 输出的DA原始数据[0, 4095]
									    Int32 nDAChannel);		// DA输出通道[0-3](写入4，代表四个通道都启动)

	    //####################### 数字I/O输入输出函数 #################################
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_GetDeviceDI(					// 取得开关量状态     
									    IntPtr hDevice ,				// 设备句柄,它应由CreateDevice函数创建								        
									    Byte[] bDISts);			// 开关输入状态(注意: 必须定义为8个字节元素的数组)
            
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_SetDeviceDO(					// 输出开关量状态
									    IntPtr hDevice ,				// 设备句柄,它应由CreateDevice函数创建								        
									    Byte[] bDOSts);			// 开关输出状态(注意: 必须定义为8个字节元素的数组)

	    //############################################################################
               [DllImport("USB5831_64.DLL")]
        public static extern bool USB5831_GetDevVersion(				// 获取设备固件及程序版本
									    IntPtr hDevice ,				// 设备对象句柄,它由CreateDevice函数创建
									    ref UInt32 pulFmwVersion,		// 固件版本
									    ref UInt32 pulDriverVersion);	// 驱动版本

	    //############################ 线程操作函数 ################################
               [DllImport("USB5831_64.DLL")]
	    public static extern IntPtr USB5831_CreateSystemEvent(); 	// 创建内核系统事件对象
               [DllImport("USB5831_64.DLL")]
	    public static extern bool USB5831_ReleaseSystemEvent(IntPtr hEvent); // 释放内核事件对象


        //#################### 辅助常量 #####################

        public const Int32 USB5831_MAX_AD_CHANNELS = 16;


        // 本卡可以支持的各种FIFO存储器的长度(点)
        public const Int32 FIFO_IDT7202_LENGTH				= 1024;
        public const Int32 FIFO_IDT7203_LENGTH				= 2048;
        public const Int32 FIFO_IDT7204_LENGTH				= 4096;
        public const Int32 FIFO_IDT7205_LENGTH				= 8192;
        public const Int32 FIFO_IDT7206_LENGTH				= 16384;
        public const Int32 FIFO_IDT7207_LENGTH				= 32768;




        
    }
}
