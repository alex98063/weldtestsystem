﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace WeldTestSystem
{
    public partial class Form1 : Form
    {
        int xbase = 8278;
        int ybase = 10347;
        int zbase = 16556;
        int wbase = 112;


        public Form1()
        {
            InitializeComponent();
            string[] strport = SerialPort.GetPortNames();

            if (strport.Length == 0)
            {
                MessageBox.Show("本机没找到端口", "Error");
                return;
               
            }



            panel1.Visible = false;

            panelMain.Visible = true;
           
         
            button1.BackColor = Color.Red;
            button14.BackColor = Color.Red;
            button12.BackColor = Color.Red;
            button3.BackColor = Color.Red;
            button4.BackColor = Color.Red;
            button5.BackColor = Color.Red;
            button6.BackColor = Color.Red;
            button7.BackColor = Color.Red;
            button8.BackColor = Color.Red;
            button9.BackColor = Color.Red;
            button10.BackColor = Color.Red;
            button11.BackColor = Color.Red;
            button13.BackColor = Color.Red;
            button15.BackColor = Color.Red;
            button16.BackColor = Color.Red;



            foreach (string s in strport)
            {
                comboBox1.Items.Add(s);
            }
            comboBox1.SelectedIndex = 0;


            groupBox3.Visible = false;
            groupBox4.Visible = false;
            groupBox5.Visible = false;
            groupBox6.Visible = false;



            for (int i=0;i<listcount;i++)
            {
                xlist.Add(0.0001); 
                ylist.Add(0.0001);
                zlist.Add(0.0001);
                wlist.Add(0.0001);
            }
          

        }

        bool moniflag = false;
        bool drawflag = false;


        List<Double> xlist = new List<Double>();
        List<Double> ylist = new List<Double>();
        List<Double> zlist = new List<Double>();
        List<Double> wlist = new List<Double>();

        String[] posxyzlis = new String[4];


        private void monitorx()
        {


            while (moniflag == true)
            {



              
                ComSend("1pos");
                String a = ComRead2(1);
                posxyzlis[0] = a;

                ComSend("2pos");
                String b = ComRead2(2);
                posxyzlis[1] = b;

                ComSend("3pos");
                String c = ComRead2(3);
                posxyzlis[2] =c;
               

                ComSend("4pos");
                String d = ComRead2(4);
                posxyzlis[3] = d;
               

                if (filewriteflag == true)
                {
                    if (textBoxsavedialog.Text != "")
                    {

                        if (filenamex != "")
                        {
                            savedatethread2(filenamex);
                           
                            count++;
                            OutMoniCount(count);
                            if (count > 9999)
                            {
                                filenamex = textBoxsavedialog.Text + "\\" + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".csv";
                                count = 0;


                            }
                        }
                    }

                }
            }
       
        }



        public static int SelectInputRange()
        {
            Int32 InputRange;       
            InputRange = Convert.ToInt32("1");          
            return InputRange;
        }

        private void startad()
        {
            hDevice = USB5831.USB5831_CreateDevice(DeviceLgcID);
            if (hDevice == (IntPtr)(-1))
            {
                Console.WriteLine("USB5831_CreateDevice Error");
               
                return; // 如果创建设备对象失败，则返回
            }

            InputRange = SelectInputRange();

            ADPara.CheckStsMode = USB5831.USB5831_CHKSTSMODE_HALF;		// 查询FIFO的非空标志
            ADPara.ADMode = USB5831.USB5831_ADMODE_SEQUENCE;		//	选择连续采集模式
            ADPara.FirstChannel = 0;   // 首通道0
            ADPara.LastChannel = 1;   // 末通道3
            ADPara.Frequency = 25; // 采样频率设为25KHz
            ADPara.InputRange = InputRange; // 量程选择
            ADPara.GroupInterval = 1000;	// 组间间隔设为1000微秒
            ADPara.LoopsOfGroup = 1;	// 组内循环次数设为1次
            ADPara.Gains = USB5831.USB5831_GAINS_1MULT;	// 使用1倍增益
            ADPara.TriggerMode = USB5831.USB5831_TRIGMODE_SOFT; // 触发模式选择软件内触发
            ADPara.TriggerSource = USB5831.USB5831_TRIGSOURCE_ATR; // 触发源选择模拟触发ATR
            ADPara.TriggerType = USB5831.USB5831_TRIGTYPE_EDGE; // 触发类型选择边沿触发
            ADPara.TriggerDir = USB5831.USB5831_TRIGDIR_NEGATIVE; // 触发方向选择
            ADPara.TrigWindow = 10; // 触发灵敏度
            ADPara.GroundingMode = USB5831.USB5831_GNDMODE_SE; // 单端方式
            ADPara.ClockSource = USB5831.USB5831_CLOCKSRC_IN;
            ADPara.bClockOutput = 0;

            nChannelCount = ADPara.LastChannel - ADPara.FirstChannel + 1; // 采样通道数

            bReturn = USB5831.USB5831_InitDeviceAD(hDevice, ref ADPara); // 初始化AD
            if (!bReturn)
            {
                Console.WriteLine("USB5831_InitDeviceAD Error");
                USB5831.USB5831_ReleaseDevice(hDevice);   // 释放设备对象
            }

            nReadSizeWords = 2048;
            nRemainder = 2048 % nChannelCount;
            nReadSizeWords = 2048 - nRemainder;   // 读取数据的大小(整个RAM长度64K)
            while (true) 
            {
               
                if (USB5831.USB5831_ReadDeviceAD(hDevice, ADBuffer, nReadSizeWords, ref nRetWords) == false) // 读取AD转换数据
                {
                    Console.WriteLine("ReadDeviceAD Error...");
                    USB5831.USB5831_ReleaseDeviceAD(hDevice); // 释放AD，停止AD数据转换
                }

                int nChannel = ADPara.FirstChannel;
                for (int Index = 0; Index < 2; Index++) // 总共显示64个点的AD数据
                {
                    ADData = (UInt16)(ADBuffer[Index] & 0x1FFF);
                    switch (InputRange) // 根据量程选择，将AD原码按相应公式换算成电压值
                    {
                        case USB5831.USB5831_INPUT_N10000_P10000mV: // ±10V
                            Volt = (float)((20000.00 / 8192) * ADData - 10000.00); // 将AD数据转换为电压值
                            break;
                        case USB5831.USB5831_INPUT_N5000_P5000mV: // ±5V
                            Volt = (float)((10000.00 / 8192) * ADData - 5000.00); // 将AD数据转换为电压值
                            break;
                        case USB5831.USB5831_INPUT_N2500_P2500mV: // ±2.5V
                            Volt = (float)((5000.00 / 8192) * ADData - 2500.00); // 将AD数据转换为电压值
                            break;
                        case USB5831.USB5831_INPUT_0_P10000mV: // 0～10V
                            Volt = (float)((10000.00 / 8192) * ADData); // 将AD数据转换为电压值
                            break;
                    }
                    Console.Write("[AI{0}]={1}\t", nChannel, Volt.ToString("###0.00"));
                    nChannel++; // 通道号递加，准备换算下一个通道的数据
                    if (nChannel > ADPara.LastChannel) // 如果换算到末通道，再回到首通道
                    {
                        Console.WriteLine(""); // 将显示光标位置移到下一项
                        nChannel = ADPara.FirstChannel;
                    }
                } // 多点数据换算显示

                Thread.Sleep(73);
            } // 循环采集

      
        }

        private void startda()
        {
            hDevice = USB5831.USB5831_CreateDevice(DeviceLgcID); // 创建设备对象
            if (hDevice == (IntPtr)(-1))
            {
                Console.WriteLine("USB5831_CreateDevice Error");
                return; // 如果创建设备对象失败，则返回
            }

            OutputRange = SelectOutputRange();
            nDAChannel = Convert.ToInt32("1");//0-1
            Voltage = (float)(Convert.ToDouble("4000"));//mv

            switch (OutputRange)
            {
                case USB5831.USB5831_OUTPUT_0_P5000mV: // 0 - +5V
                    nDAData = (Int16)(Voltage / (5000.00 / 4096));
                    break;
                case USB5831.USB5831_OUTPUT_0_P10000mV: // 0 - +10V
                    nDAData = (Int16)(Voltage / (10000.00 / 4096));
                    break;
                case USB5831.USB5831_OUTPUT_0_P10800mV: // 0 - +10.8V
                    nDAData = (Int16)(Voltage / (10800.00 / 4096));
                    break;
                case USB5831.USB5831_OUTPUT_N5000_P5000mV: // -5V - +5V
                    nDAData = (Int16)(Voltage / (10000.00 / 4096) + 2048);
                    break;
                case USB5831.USB5831_OUTPUT_N10000_P10000mV: // -10V - +10V
                    nDAData = (Int16)(Voltage / (20000.00 / 4096) + 2048);
                    break;
                case USB5831.USB5831_OUTPUT_N10800_P10800mV: // -10.8V - +10.8V
                    nDAData = (Int16)(Voltage / (21600.00 / 4096) + 2048);
                    break;
                default:
                    break;
            }

            if (nDAData < 0)
            {
                nDAData = 0;
            }
            if (nDAData > 4095)
            {
                nDAData = 4095;
            }
            
            bRetStatus = USB5831.USB5831_WriteDeviceDA(hDevice, OutputRange, nDAData, nDAChannel);
            if (!bRetStatus)
            {
                Console.WriteLine("USB5831_WriteDeviceDA error...");
                USB5831.USB5831_ReleaseDevice(hDevice);   // 释放设备对象	
            }

        
        }

        public static int SelectOutputRange()
        {
            Int32 InputRange;
      
            InputRange = Convert.ToInt32("0");
 
            return InputRange;
        }

        private void startdo()
        {

            Byte[] bDOSts = new Byte[8];
            bDOSts[0] = 0;
            bDOSts[1] = 0;
            bDOSts[2] = 0;
            bDOSts[3] = 0;
            bDOSts[4] = 0;
            bDOSts[5] = 0;
            bDOSts[6] = 0;
            bDOSts[7] = 0;

            hDevice = USB5831.USB5831_CreateDevice(DeviceLgcID);
            if (hDevice == (IntPtr)(-1))
            {
                Console.WriteLine("USB5831_CreateDevice Error");

                return; // 如果创建设备对象失败，则返回
            }

            if (!USB5831.USB5831_SetDeviceDO(hDevice, bDOSts)) // 开关量输出
            {
                Console.WriteLine("SetDeviceDO Error...");
                return;
            }
           
        }

        private void IOListener()
        {
            try
            {

               
                    hDevice = USB5831.USB5831_CreateDevice(DeviceLgcID);
                    if (hDevice == (IntPtr)(-1))
                    {
                        Console.WriteLine("USB5831_CreateDevice Error");

                        return; // 如果创建设备对象失败，则返回
                    }
                    else
                    {
                        //  Console.WriteLine("USB5831_CreateDevice Success");

                    }

                    while(ioflag==true)
                    {
                        lock (this)
                        {




                            if (!USB5831.USB5831_GetDeviceDI(hDevice, bDISts)) // 开关量输入
                            {
                                Console.WriteLine("USB5831_GetDeviceDI...");
                                USB5831.USB5831_ReleaseDevice(hDevice);   // 释放设备对象
                                return;
                            }

                            Console.WriteLine("...");

                            if (bDISts[0] == 1)
                            {
                                rightmove();
                                button8.BackColor = Color.Green;

                            }
                            else
                            {
                                button8.BackColor = Color.Red;
                            }
                            if (bDISts[1] == 1)
                            {

                                leftmove();
                                button7.BackColor = Color.Green;
                            }
                            else
                            {

                                button7.BackColor = Color.Red;
                            }
                            if (bDISts[2] == 1)
                            {
                                forwardmove();

                                button5.BackColor = Color.Green;
                            }
                            else
                            {
                                button5.BackColor = Color.Red;
                            }

                            if (bDISts[3] == 1)
                            {

                                backwardmove();

                                button6.BackColor = Color.Green;
                            }
                            else
                            {

                                button6.BackColor = Color.Red;
                            }
                            if (bDISts[4] == 1)
                            {
                                downmove();
                                button4.BackColor = Color.Green;
                            }
                            else
                            {

                                button4.BackColor = Color.Red;
                            }
                            if (bDISts[5] == 1)
                            {
                                upmove();
                                button3.BackColor = Color.Green;
                            }
                            else
                            {

                                button3.BackColor = Color.Red;
                            }
                            if (bDISts[6] == 1)
                            {
                                counterclock();
                                button10.BackColor = Color.Green;
                            }
                            else
                            {

                                button10.BackColor = Color.Red;
                            }
                            if (bDISts[7] == 1)
                            {
                                clockmove();
                                button9.BackColor = Color.Green;
                            }
                            else
                            {

                                button9.BackColor = Color.Red;
                            }
                            if (bDISts[8] == 1)
                            {
                                conformbtn();
                                button11.BackColor = Color.Green;
                            }
                            else
                            {

                                button11.BackColor = Color.Red;
                            }

                            if (bDISts[9] == 1)
                            {
                                pausebtn();
                                button12.BackColor = Color.Green;
                            }
                            else
                            {

                                button12.BackColor = Color.Red;
                            }
                            if (bDISts[10] == 1)
                            {
                                resetbtn();

                                button15.BackColor = Color.Green;
                            }
                            else
                            {

                                button15.BackColor = Color.Red;
                            }
                            if (bDISts[11] == 1)
                            {

                                button16.BackColor = Color.Green;
                            }
                            else
                            {

                                button16.BackColor = Color.Red;
                            }
                            if (bDISts[12] == 1)
                            {
                                clampbtn();

                                button13.BackColor = Color.Green;
                            }
                            else
                            {

                                button13.BackColor = Color.Red;
                            }

                        }

                        Thread.Sleep(51);

                    }

                
                //    
                //   
            }
            catch
            {

            }

        }

        private void clampbtn()
        {
            ComSend("1V0");
            ComSend("2V0");
            ComSend("3V0");
            ComSend("4V0");
        }

     

        static USB5831.USB5831_PARA_AD ADPara; // 硬件参数
        Byte[] bDISts = new Byte[13];
        Byte[] bDOSts = new Byte[13];
        IntPtr hDevice;
        Int32 DeviceLgcID = 0;

        bool bReturn;               // 函数的返回值
        Int32 nReadSizeWords;       // 每次读取AD数据个数
        Int32 nRetWords = 0;            // 实际读取的数据个数
        Int32 nChannelCount = 0;    // 采样通道数
        UInt16[] ADBuffer = new UInt16[32768];  // 接收AD 数据的缓冲区
        UInt16 ADData;
        float Volt = 0.0f;          // 将AD原始数据转换为电压值
        Int32 nRemainder = 0;
        Int32 InputRange = 2;
        Int32 OutputRange = 0;
        Int32 nDAChannel = 2;
        float Voltage = (float)2000;
        Int16 nDAData = 0;
        bool bRetStatus; // 函数的返回值

        private void StartDevice()
        {


            bool bReturn;               // 函数的返回值
            Int32 nReadSizeWords;       // 每次读取AD数据个数
            Int32 nRetWords = 0;            // 实际读取的数据个数
            Int32 nChannelCount = 0;    // 采样通道数
            UInt16[] ADBuffer = new UInt16[32768];  // 接收AD 数据的缓冲区
            UInt16 ADData;
            float Volt = 0.0f;          // 将AD原始数据转换为电压值
            Int32 nRemainder = 0;
            Int32 InputRange = 2;
            Int32 OutputRange = 0;
            Int32 nDAChannel = 2;
            float Voltage = (float)2000;
            Int16 nDAData = 0;
            bool bRetStatus; // 函数的返回值



            hDevice = USB5831.USB5831_CreateDevice(DeviceLgcID);
            if (hDevice == (IntPtr)(-1))
            {
                Console.WriteLine("USB5831_CreateDevice Error");

                return; // 如果创建设备对象失败，则返回
            }
            else
            {
                Console.WriteLine("USB5831_CreateDevice Success");


            }


            switch (OutputRange)
            {
                case USB5831.USB5831_OUTPUT_0_P5000mV: // 0 - +5V
                    nDAData = (Int16)(Voltage / (5000.00 / 4096));
                    break;
                case USB5831.USB5831_OUTPUT_0_P10000mV: // 0 - +10V
                    nDAData = (Int16)(Voltage / (10000.00 / 4096));
                    break;
                case USB5831.USB5831_OUTPUT_0_P10800mV: // 0 - +10.8V
                    nDAData = (Int16)(Voltage / (10800.00 / 4096));
                    break;
                case USB5831.USB5831_OUTPUT_N5000_P5000mV: // -5V - +5V
                    nDAData = (Int16)(Voltage / (10000.00 / 4096) + 2048);
                    break;
                case USB5831.USB5831_OUTPUT_N10000_P10000mV: // -10V - +10V
                    nDAData = (Int16)(Voltage / (20000.00 / 4096) + 2048);
                    break;
                case USB5831.USB5831_OUTPUT_N10800_P10800mV: // -10.8V - +10.8V
                    nDAData = (Int16)(Voltage / (21600.00 / 4096) + 2048);
                    break;
                default:
                    break;
            }

            if (nDAData < 0)
            {
                nDAData = 0;
            }
            if (nDAData > 4095)
            {
                nDAData = 4095;
            }
            Console.WriteLine("nDAData = {0}", nDAData);

            bRetStatus = USB5831.USB5831_WriteDeviceDA(hDevice, OutputRange, nDAData, nDAChannel);
            if (!bRetStatus)
            {
                Console.WriteLine("USB5831_WriteDeviceDA error...");


            }
            else
            {
                Console.WriteLine("USB5831_WriteDeviceDA success...");

            }
            // goto ExitReleaseDevice;

            bDOSts[0] = 1;
            bDOSts[1] = 1;
            bDOSts[2] = 1;
            bDOSts[3] = 1;
            bDOSts[4] = 1;
            bDOSts[5] = 1;
            bDOSts[6] = 1;
            bDOSts[7] = 1;
            bDOSts[8] = 0;
            bDOSts[9] = 0;
            bDOSts[10] = 0;
            bDOSts[11] = 0;
            bDOSts[12] = 0;

            if (!USB5831.USB5831_SetDeviceDO(hDevice, bDOSts)) // 开关量输出
            {
                Console.WriteLine("SetDeviceDO Error...");
                return;
            }

            for (Int32 i = 0; i < 13; i++)
            {
                if (bDOSts[i] == 1)
                    Console.WriteLine("DO{0} = On", i);
                else
                    Console.WriteLine("DO{0} = Off", i);
            }

            if (!USB5831.USB5831_GetDeviceDI(hDevice, bDISts)) // 开关量输入
            {
                Console.WriteLine("USB5831_GetDeviceDI...");
                return;
            }

            for (Int32 i = 0; i < 13; i++)
            {
                if (bDISts[i] == 1)
                    Console.WriteLine("DI{0} = On", i);
                else
                    Console.WriteLine("DI{0} = Off", i);
            }

            //   goto ExitReleaseDevice;

            ADPara.CheckStsMode = USB5831.USB5831_CHKSTSMODE_HALF;		// 查询FIFO的非空标志
            ADPara.ADMode = USB5831.USB5831_ADMODE_SEQUENCE;		//	选择连续采集模式
            ADPara.FirstChannel = 0;   // 首通道0
            ADPara.LastChannel = 15;   // 末通道3
            ADPara.Frequency = 25000; // 采样频率设为25KHz
            ADPara.InputRange = InputRange; // 量程选择
            ADPara.GroupInterval = 100;	// 组间间隔设为1000微秒
            ADPara.LoopsOfGroup = 1;	// 组内循环次数设为1次
            ADPara.Gains = USB5831.USB5831_GAINS_1MULT;	// 使用1倍增益
            ADPara.TriggerMode = USB5831.USB5831_TRIGMODE_SOFT; // 触发模式选择软件内触发
            ADPara.TriggerSource = USB5831.USB5831_TRIGSOURCE_ATR; // 触发源选择模拟触发ATR
            ADPara.TriggerType = USB5831.USB5831_TRIGTYPE_EDGE; // 触发类型选择边沿触发
            ADPara.TriggerDir = USB5831.USB5831_TRIGDIR_NEGATIVE; // 触发方向选择
            ADPara.TrigWindow = 10; // 触发灵敏度
            ADPara.GroundingMode = USB5831.USB5831_GNDMODE_SE; // 单端方式
            ADPara.ClockSource = USB5831.USB5831_CLOCKSRC_IN;
            ADPara.bClockOutput = 0;

            nChannelCount = ADPara.LastChannel - ADPara.FirstChannel + 1; // 采样通道数


            bReturn = USB5831.USB5831_InitDeviceAD(hDevice, ref ADPara); // 初始化AD
            if (!bReturn)
            {
                Console.WriteLine("USB5831_InitDeviceAD Error");

                goto ExitReleaseDevice;
            }



            nReadSizeWords = 2048;
            nRemainder = 2048 % nChannelCount;
            nReadSizeWords = 2048 - nRemainder;   // 读取数据的大小(整个RAM长度64K)


            if (USB5831.USB5831_ReadDeviceAD(hDevice, ADBuffer, nReadSizeWords, ref nRetWords) == false) // 读取AD转换数据
            {
                Console.WriteLine("ReadDeviceAD Error...");

                goto ExitReleaseDeviceAD;
            }

            int nChannel = ADPara.FirstChannel;
            for (int Index = 0; Index < 64; Index++) // 总共显示64个点的AD数据
            {
                ADData = (UInt16)(ADBuffer[Index] & 0x1FFF);
                switch (InputRange) // 根据量程选择，将AD原码按相应公式换算成电压值
                {
                    case USB5831.USB5831_INPUT_N10000_P10000mV: // ±10V
                        Volt = (float)((20000.00 / 8192) * ADData - 10000.00); // 将AD数据转换为电压值
                        break;
                    case USB5831.USB5831_INPUT_N5000_P5000mV: // ±5V
                        Volt = (float)((10000.00 / 8192) * ADData - 5000.00); // 将AD数据转换为电压值
                        break;
                    case USB5831.USB5831_INPUT_N2500_P2500mV: // ±2.5V
                        Volt = (float)((5000.00 / 8192) * ADData - 2500.00); // 将AD数据转换为电压值
                        break;
                    case USB5831.USB5831_INPUT_0_P10000mV: // 0～10V
                        Volt = (float)((10000.00 / 8192) * ADData); // 将AD数据转换为电压值
                        break;
                }
                Console.Write("[AI{0}]={1}\t", nChannel, Volt.ToString("###0.00"));
                nChannel++; // 通道号递加，准备换算下一个通道的数据
                if (nChannel > ADPara.LastChannel) // 如果换算到末通道，再回到首通道
                {
                    Console.WriteLine(""); // 将显示光标位置移到下一项
                    nChannel = ADPara.FirstChannel;
                }
            } // 多点数据换算显示

              ExitReleaseDeviceAD:
            USB5831.USB5831_ReleaseDeviceAD(hDevice); // 释放AD，停止AD数据转换

              ExitReleaseDevice:
            USB5831.USB5831_ReleaseDevice(hDevice);   // 释放设备对象

        }

        private void 预设参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelMain.Visible = false;
            panel1.Visible = true;
       

        }

        private void button19_Click(object sender, EventArgs e)
        {
            panelMain.Visible = true;
          


        }

        List<String> lis = new List<string>();

        private void 读入轨迹文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".csv";
            ofd.Filter = "csv file|*.csv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                label9.Text = ofd.FileName;

                Console.WriteLine(ofd.FileName);

                StreamReader sr = new StreamReader(ofd.FileName, Encoding.Default);


                lis = new List<String>();

                while (sr.Peek() > 0)
                {

                    lis.Add(sr.ReadLine());

                }
                Console.WriteLine(lis.Count);
                for (int i = 0; i < lis.Count; i++)
                {
                    Console.WriteLine(lis[i]);
                }

            }


        }

        private void button20_Click(object sender, EventArgs e)
        {
            panelMain.Visible = true;
          
        }

        SerialPort port;
        private bool ComPortIsOpen = false;

        private void button1_Click(object sender, EventArgs e)
        {
            String comname = comboBox1.SelectedItem.ToString();

            String botelv = comboBox2.SelectedItem.ToString();

            if (ComPortIsOpen == false)
            {

                if (ComOpen(comname, botelv))
                {
                    //MessageBox.Show(comname + "串口已连接！");
                    button1.Text = "断开";
                    button1.BackColor = Color.Green;
                 
                    ComPortIsOpen = true;
                    
                    //port.DataReceived += new SerialDataReceivedEventHandler(ComRec);
                    groupBox3.Visible = true;
                    groupBox4.Visible = true;
                    groupBox5.Visible = true;
                    groupBox6.Visible = true;

                }
                else
                {
                  //  MessageBox.Show("无法打开串口,请检测此串口是否有效或被其他占用！");
                    ComPortIsOpen = false;
                    button1.BackColor = Color.Red;
                    button1.Text = "连接";
                    groupBox3.Visible = false;
                    groupBox4.Visible = false;
                    groupBox5.Visible = false;
                    groupBox6.Visible = false;
                }

            }
            else
            {
                ComClose();
                ComPortIsOpen = false;
                button1.Text = "连接";
                button1.BackColor = Color.Red;
               // MessageBox.Show("串口连接断开！");
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                groupBox5.Visible = false;
                groupBox6.Visible = false;

            }
        }
       

        private void ComSend(String data)
        {

            try
            {
                lock (this)
                {
                    Console.WriteLine(data + "\r\n");
                    port.WriteLine(data + "\r\n");
                }
             //   Thread.Sleep(100);

            }
            catch { Console.WriteLine("send error!"); }
          

        }


        private void ComRead()
        {
            try
            {
                lock (this)
                {
                    String res = port.ReadLine();
                    Console.WriteLine(res);
                }
            }
            catch { Console.WriteLine("read error!"); }


        }

        TimeSpan timevx1, timevx2;
        Int32 posvx1, posvx2;

        TimeSpan timevy1, timevy2;
        Int32 posvy1, posvy2;

        TimeSpan timevz1, timevz2;
        Int32 posvz1, posvz2;

        TimeSpan timevw1, timevw2;
        Int32 posvw1, posvw2;
      

        private String ComRead2(int a)
        {
           
            try
            {
                lock (this)
                {
                    String res = port.ReadLine();
                    Console.WriteLine(res);
                    


                    Int32 pos = Convert.ToInt32(res);

                    if (a == 1)
                    {
                     
                        OutMoniX(pos);
                        timevx2 = DateTime.Now.TimeOfDay;
                        double deltatime=0;
                        
                            Int32 deltapos=0;
                        try
                        {  
                             deltatime = timevx2.Subtract(timevx1).TotalMinutes;
                           
                        }
                           catch { }

                        timevx1 = timevx2;

                            posvx2= pos;
         
                       
                try
                        {
                             deltapos = posvx2 - posvx1;
                      }
                           catch { }

                posvx1 = posvx2;

                try
                {
                    double vv = deltapos / deltatime;
                    OutMoniVX(vv);
                }
                catch { }

                    }
                    if (a == 2)
                    { 
                        
                        
                        OutMoniY(pos);


                        timevy2 = DateTime.Now.TimeOfDay;
                        double deltatime = 0;

                        Int32 deltapos = 0;
                        try
                        {
                            deltatime = timevy2.Subtract(timevy1).TotalMinutes;

                        }
                        catch { }

                        timevy1 = timevy2;

                        posvy2 = pos;


                        try
                        {
                            deltapos = posvy2 - posvy1;
                        }
                        catch { }

                        posvy1 = posvy2;

                        try
                        {
                            double vv= deltapos / deltatime;
                            OutMoniVY(vv);
                        }
                        catch { }
                    
                    
                    
                    }
                    if (a == 3)
                    { OutMoniZ(pos);
                        

                    timevz2 = DateTime.Now.TimeOfDay;
                    double deltatime = 0;

                    Int32 deltapos = 0;
                    try
                    {
                        deltatime = timevz2.Subtract(timevz1).TotalMinutes;

                    }
                    catch { }

                    timevz1 = timevz2;

                    posvz2 = pos;


                    try
                    {
                        deltapos = posvz2 - posvz1;
                    }
                    catch { }

                    posvz1 = posvz2;

                    try
                    {
                        double vv = deltapos / deltatime;
                        OutMoniVZ(vv);
                    }
                    catch { }
                    
                    
                    }
                    if (a == 4)
                    { OutMoniW(pos);

                    timevw2 = DateTime.Now.TimeOfDay;
                    double deltatime = 0;

                    Int32 deltapos = 0;
                    try
                    {
                        deltatime = timevw2.Subtract(timevw1).TotalMinutes;

                    }
                    catch { }

                    timevw1 = timevw2;

                    posvw2 = pos;


                    try
                    {
                        deltapos = posvw2 - posvw1;
                    }
                    catch { }

                    posvw1 = posvw2;

                    try
                    {
                        double vv = deltapos / deltatime;
                        OutMoniVW(vv);
                    }
                    catch { }
                    }

                    return res;
                }
               
            }

            catch { Console.WriteLine("read2 error!"); return null; }     

        }

        static int sleeptime = 300;
        static int listcount = 1000;
        static int xscale = 5;

        public delegate void OutMoniWDelegate(Int32 a);
        public void OutMoniW(Int32 a)
        {

            if (labelW.InvokeRequired)
            {
                OutMoniWDelegate outMoniWDelegate = new OutMoniWDelegate(OutMoniW);
                this.BeginInvoke(outMoniWDelegate, new object[] { a });
                return;
            }
            double b = (double)a / wbase;

            if (wlist.Count > listcount)
            {
                wlist.Add(b);
                wlist.RemoveAt(0);
            }
            else
            {
                wlist.Add(b);
            }

            labelW.Text = b.ToString("0.0");

        }


        public delegate void OutMoniZDelegate(Int32 a);
        public void OutMoniZ(Int32 a)
        {

            if (labelZ.InvokeRequired)
            {
                OutMoniZDelegate outMoniZDelegate = new OutMoniZDelegate(OutMoniZ);
                this.BeginInvoke(outMoniZDelegate, new object[] { a });
                return;
            }
            double b = (double)a / zbase;
            if (zlist.Count > listcount)
            {
                zlist.Add(b);
                zlist.RemoveAt(0);
            }
            else
            {
                zlist.Add(b);
            }
            labelZ.Text = b.ToString("0.0");

        }
      

        public delegate void OutMoniYDelegate(Int32 a);
        public void OutMoniY(Int32 a)
        {

            if (labelY.InvokeRequired)
            {
                OutMoniYDelegate outMoniYDelegate = new OutMoniYDelegate(OutMoniY);
                this.BeginInvoke(outMoniYDelegate, new object[] { a });
                return;
            }
            double b = (double)a / ybase;
            if (ylist.Count > listcount)
            {
                ylist.Add(b);
                ylist.RemoveAt(0);
            }
            else
            {
               ylist.Add(b);
            }
            labelY.Text = b.ToString("0.0");

        }

       

        public delegate void OutMoniXDelegate(Int32 a);
        public void OutMoniX(Int32 a)
        {

            if (labelX.InvokeRequired)
            {
                OutMoniXDelegate outMoniXDelegate = new OutMoniXDelegate(OutMoniX);
                this.BeginInvoke(outMoniXDelegate, new object[] { a });
                return;
            }
            double b = (double)a /xbase;
            if (xlist.Count > listcount)
            {
                xlist.Add(b);
                xlist.RemoveAt(0);
            }
            else
            {
                xlist.Add(b);
            }
        
            labelX.Text = b.ToString("0.0");

        }

        public delegate void OutMoniVXDelegate(double a);
        public void OutMoniVX(double a)
        {

            if (labelvx.InvokeRequired)
            {
                OutMoniVXDelegate outMoniVXDelegate = new OutMoniVXDelegate(OutMoniVX);
                this.BeginInvoke(outMoniVXDelegate, new object[] { a });
                return;
            }
            double b = a / xbase;
            labelvx.Text = b.ToString("0");

        }

        public delegate void OutMoniVYDelegate(double a);
        public void OutMoniVY(double a)
        {

            if (labelvy.InvokeRequired)
            {
                OutMoniVYDelegate outMoniVYDelegate = new OutMoniVYDelegate(OutMoniVY);
                this.BeginInvoke(outMoniVYDelegate, new object[] { a });
                return;
            }
            double b = a / ybase;
            labelvy.Text = b.ToString("0");

        }

        public delegate void OutMoniVZDelegate(double a);
        public void OutMoniVZ(double a)
        {

            if (labelvz.InvokeRequired)
            {
                OutMoniVZDelegate outMoniVZDelegate = new OutMoniVZDelegate(OutMoniVZ);
                this.BeginInvoke(outMoniVZDelegate, new object[] { a });
                return;
            }
            double b = a / zbase;
            labelvz.Text = b.ToString("0");

        }

        public delegate void OutMoniVWDelegate(double a);
        public void OutMoniVW(double a)
        {

            if (labelvw.InvokeRequired)
            {
                OutMoniVWDelegate outMoniVWDelegate = new OutMoniVWDelegate(OutMoniVW);
                this.BeginInvoke(outMoniVWDelegate, new object[] { a });
                return;
            }
            double b = a / wbase;
            labelvw.Text = b.ToString("0");

        }


        public delegate void OutVecdataDelegate(String a);
        public void OutVecdata(String a)
        {

            if (listBox1.InvokeRequired)
            {
                OutVecdataDelegate outVecdataDelegate = new OutVecdataDelegate(OutVecdata);
                this.BeginInvoke(outVecdataDelegate, new object[] { a });
                return;
            }

            listBox1.Items.Add(a);
        }

        public delegate void OutPanelxfreshDelegate();
        public void OutPanelxfresh()
        {

            if (panelXshow.InvokeRequired)
            {
                OutPanelxfreshDelegate outPanelxfreshDelegate = new OutPanelxfreshDelegate(OutPanelxfresh);
                this.BeginInvoke(outPanelxfreshDelegate);
                return;
            }

            
            panelXshow.Refresh();

        }

        public delegate void OutPanelyfreshDelegate();
        public void OutPanelyfresh()
        {

            if (panelYshow.InvokeRequired)
            {
                OutPanelyfreshDelegate outPanelyfreshDelegate = new OutPanelyfreshDelegate(OutPanelyfresh);
                this.BeginInvoke(outPanelyfreshDelegate);
                return;
            }


            panelYshow.Refresh();

        }

        public delegate void OutPanelzfreshDelegate();
        public void OutPanelzfresh()
        {

            if (panelZshow.InvokeRequired)
            {
                OutPanelzfreshDelegate outPanelzfreshDelegate = new OutPanelzfreshDelegate(OutPanelzfresh);
                this.BeginInvoke(outPanelzfreshDelegate);
                return;
            }


            panelZshow.Refresh();

        }

        public delegate void OutPanelwfreshDelegate();
        public void OutPanelwfresh()
        {

            if (panelWshow.InvokeRequired)
            {
                OutPanelwfreshDelegate outPanelwfreshDelegate = new OutPanelwfreshDelegate(OutPanelwfresh);
                this.BeginInvoke(outPanelwfreshDelegate);
                return;
            }


            panelWshow.Refresh();

        }


        public delegate void OutMoniCountDelegate(int a);
        public void OutMoniCount(int a)
        {

            if (labelcount.InvokeRequired)
            {
                OutMoniCountDelegate outMoniCountDelegate = new OutMoniCountDelegate(OutMoniCount);
                this.BeginInvoke(outMoniCountDelegate, new object[] { a });
                return;
            }

            labelcount.Text = a.ToString();

        }


        private void ComRec(object sender, SerialDataReceivedEventArgs e)
        {
            String rec = port.ReadLine();
            Console.WriteLine(rec);

            try
            {
                Int32 posx = Convert.ToInt32(rec);
                OutMoniX(posx);
            }
            catch { }        
            

        }

        private bool ComOpen(String comname, String botelv)
        {
            try
            {

                port = new SerialPort(comname, int.Parse(botelv), Parity.None, 8, StopBits.One);
                port.ReadTimeout = 2000;
                port.WriteTimeout = 2000;

                port.ReadBufferSize = 1024;
                port.WriteBufferSize = 1024;

                port.Open();
                return true;
            }
            catch
            {

                return false;
            }
        }

        private void ComClose()
        {
            port.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            Thread sendtextthread = new Thread(sendtext);
            sendtextthread.Start();
          
        }

        private void sendtext()
        {
            ComSend(textBox6.Text);
           ComRead();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread upmovethread = new Thread(upmove);
            upmovethread.Start();
         

        }
        private void upmove()
        {
            try
            {

                Int32 step = Convert.ToInt32(textBoxup.Text) * zbase;
                String str = "3lr" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("3sp200");
                ComRead();
                ComSend("3m");
                ComRead();
            }
            catch { }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            Thread forwardmovethread = new Thread(forwardmove);
            forwardmovethread.Start();
        

           
        }

        private void forwardmove()
        {
            try
            {

                Int32 step = Convert.ToInt32(textBoxforward.Text) * ybase;
                String str = "2lr" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("2sp200");
                ComRead();
                ComSend("2m");
                ComRead();
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread backwardmovethread = new Thread(backwardmove);
            backwardmovethread.Start();
           
        }

        private void backwardmove()
        {
            try
            {


                Int32 step = Convert.ToInt32(textBoxback.Text) * ybase;
                String str = "2lr-" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("2sp200");
                ComRead();
                ComSend("2m");
                ComRead();
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            Thread downmovethread = new Thread(downmove);
            downmovethread.Start();
           
          
           
        }

        private void downmove()
        {
            try
            {

                Int32 step = Convert.ToInt32(textBoxdown.Text) * zbase;
                String str = "3lr-" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("3sp200");
                ComRead();
                ComSend("3m");
                ComRead();
            }
            catch { }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            Thread leftmovethread = new Thread(leftmove);
            leftmovethread.Start();
          
        }

        private void leftmove()
        {
            try
            {

                Int32 step = Convert.ToInt32(textBoxleft.Text) * xbase;
                String str = "1lr-" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("1sp200");
                ComRead();
                ComSend("1m");
                ComRead();
            }
            catch { }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Thread rightmovethread = new Thread(rightmove);
            rightmovethread.Start();
           
        }

        private void rightmove()
        {
            try
            {

                Int32 step = Convert.ToInt32(textBoxright.Text) * xbase;
                String str = "1lr" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("1sp200");
                ComRead();
                ComSend("1m");
                ComRead();
            }
            catch { }
        }

        private void button9_Click(object sender, EventArgs e)
        {

            Thread clockmovethread = new Thread(clockmove);
            clockmovethread.Start();
           
           
        }

        private void clockmove()
        {
            try
            {

                Int32 step = Convert.ToInt32(textBoxclock.Text) * wbase;
                String str = "4LR" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("4sp200");
                ComRead();
                ComSend("4m");
                ComRead();
            }
            catch { }
        }

        private void button10_Click(object sender, EventArgs e)
        {

            Thread counterclockthread = new Thread(counterclock);
            counterclockthread.Start();
           
          

        }

        private void counterclock()
        {
            try
            {

                Int32 step = Convert.ToInt32(textBoxcounterclock.Text) * wbase;
                String str = "4LR-" + step.ToString();
                ComSend(str);
                ComRead();
                ComSend("4sp200");
                ComRead();
                ComSend("4m");
                ComRead();
            }
            catch { }
        }

        private void button11_Click(object sender, EventArgs e)
        {

          Thread conformthread = new Thread(conformbtn);
          conformthread.Start();
           
           
        }

        private void conformbtn()
        {
            ComSend("1HO");
            ComRead();
            ComSend("2HO");
            ComRead();
            ComSend("3HO");
            ComRead();
            ComSend("4HO");
            ComRead();
        }

        private void button12_Click(object sender, EventArgs e)
        {

            Thread pausethread = new Thread(pausebtn);
            pausethread.Start();
           
          
        }

        private void pausebtn()
        {
            ComSend("1V0");
            ComRead();
            ComSend("2V0");
            ComRead();
            ComSend("3V0");
            ComRead();
            ComSend("4V0");
            ComRead();
        }

        private void button13_Click(object sender, EventArgs e)
        {

        }
        private bool deviceisopen = false;


        private void opensend()
        {
            ComSend("1EN");
            ComRead();
            ComSend("2EN");
            ComRead();
            ComSend("3EN");
            ComRead();
            ComSend("4EN");
            ComRead();
        }

        private void closesend()
        {
            ComSend("1DI");
            ComRead();
            ComSend("2DI");
            ComRead();
            ComSend("3DI");
            ComRead();
            ComSend("4DI");
            ComRead();
        }

        bool ioflag = false;

        private void button14_Click(object sender, EventArgs e)
        {
            if (deviceisopen == false)
            {
                Thread opensendthread = new Thread(opensend);
                opensendthread.Start();

                button14.Text = "关";
                button14.BackColor = Color.Green;

                deviceisopen = true;
                moniflag = true;
                drawflag = true;

                ioflag = true;

                Thread IOthread = new Thread(IOListener);
                IOthread.Start();

                Thread DOthread = new Thread(startdo);
                DOthread.Start();

                Thread DAthread = new Thread(startda);
                DAthread.Start();


                Thread ADthread = new Thread(startad);
                ADthread.Start();

               
              Thread MXthread = new Thread(monitorx);
                MXthread.Start();

                Thread drawxthread = new Thread(drawx);
                drawxthread.Start();
                Thread drawythread = new Thread(drawy);
                drawythread.Start();

                Thread drawzthread = new Thread(drawz);
                drawzthread.Start();

                Thread drawwthread = new Thread(draww);
                drawwthread.Start();

               
            }
            else
            {

                moniflag = false;
                drawflag = false;
                ioflag = false;
                Thread closesendthread = new Thread(closesend);
                closesendthread.Start();

                button14.Text = "开";
                button14.BackColor = Color.Red;
                deviceisopen = false;
               
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {

            Thread restthread = new Thread(resetbtn);
            restthread.Start();
           
           
        }

       

        private void drawx()
        {
          
            while (drawflag==true)
            {
                Thread.Sleep(sleeptime);
                OutPanelxfresh();
            }
        }

        private void drawy()
        {

            while (drawflag == true)
            {
                Thread.Sleep(sleeptime);
                OutPanelyfresh();
            }
        }
        private void drawz()
        {

            while (drawflag == true)
            {
                Thread.Sleep(sleeptime);
                OutPanelzfresh();
            }
        }
        private void draww()
        {

            while (drawflag == true)
            {
                Thread.Sleep(sleeptime);
                OutPanelwfresh();
            }
        }

        private void resetbtn()
        {
            ComSend("1LA0");
            ComRead();
            ComSend("2LA0");
            ComRead();
            ComSend("3LA0");
            ComRead();
            ComSend("4LA0");
            ComRead();

            ComSend("1SP200");
            ComRead();
            ComSend("2SP200");
            ComRead();
            ComSend("3SP200");
            ComRead();
            ComSend("4SP200");
            ComRead();

            ComSend("1NP");
            ComRead();
            ComSend("2NP");
            ComRead();
            ComSend("3NP");
            ComRead();
            ComSend("4NP");
            ComRead();

            ComSend("1M");
            ComRead();
            ComSend("2M");
            ComRead();
            ComSend("3M");
            ComRead();
            ComSend("4M");
            ComRead();
        }

        bool checkflag = false;

        private void spurxyz()
        {
            for (int i = 0; i < lis.Count;i++ )
            {

                List<String> vec = new List<String>();
                String str = lis[i];
                OutVecdata(str);
                String[] arrstr = str.Split(';');
                for (int j = 1; j < arrstr.Length; j++)
                {
                    vec.Add(arrstr[j]);

                }

                for (int k = 0; k < vec.Count - 1; k++)
                {
                    if (vec[k] != "")
                    {
                        
                        ComSend(vec[k]);
                        ComRead();

                    }

                }


                long waittime = long.Parse(vec[vec.Count - 1]);
                Thread.Sleep(new TimeSpan(waittime));

                while (checkflag == false)
                {
                    String str1 = vec[0].Substring(3);
                    String str2 = vec[1].Substring(3);
                        String str3 = vec[2].Substring(3); 
                    if (Math.Abs(Convert.ToInt32(str1) - Convert.ToInt32(posxyzlis[0]))<20 &&
                        Math.Abs(Convert.ToInt32(str2) - Convert.ToInt32(posxyzlis[1])) <20 &&
                        Math.Abs(Convert.ToInt32(str3) - Convert.ToInt32(posxyzlis[3])) < 20)
                    {
                        checkflag = true;
                    }

                }


                checkflag = false;
                

            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
               listBox1.Items.Clear();
               
                Thread spurthread = new Thread(spurxyz);
              
                    spurthread.Start();
               
            

       
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxsavedialog.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        int count = 0;
        bool filewriteflag = false;
        String pathx = "";

        private void savedatethread2(String filename)
        {
            
          
           
            StreamWriter sw = new StreamWriter(filename, true);

            sw.WriteLine(labelX.Text + ";" + labelY.Text + ";" + labelZ.Text + ";" + labelW.Text + ";" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ":" + DateTime.Now.Millisecond.ToString());
               
              sw.Dispose();
           
              
           
        }

        private void savedatethread()
        {

            String filenamex = pathx + "\\" + DateTime.Now.Day.ToString()+DateTime.Now.Hour.ToString()+DateTime.Now.Minute.ToString()+DateTime.Now.Second.ToString() + ".csv"; 
            StreamWriter sw=null;

            while (filewriteflag == true)
            {    sw = new StreamWriter(filenamex, true);
                sw.WriteLine(labelX.Text + ";" + labelY.Text + ";" + labelZ.Text + ";" +labelW.Text +";" + DateTime.Now.ToLongTimeString());
                count++;
                if (count > 9999)
                {
                    filenamex = pathx + "\\" + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".csv";
                    count = 0;
                }
                sw.Dispose();
            }
                
        }
        String filenamex="";
        private void button22_Click(object sender, EventArgs e)
        {

            if (button22.Text == "记录")
            {

                if (textBoxsavedialog.Text != "")
                {
                   

                    if (filewriteflag == false)
                    {
                        pathx = textBoxsavedialog.Text;
                        filewriteflag = true;

                        filenamex = textBoxsavedialog.Text + "\\" + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".csv";

                        button22.Text = "停止";


                        //Thread Savethread = new Thread(savedatethread);
                        //Savethread.Start();
                    }


                }
            }
            else
            {
                filewriteflag = false;
                
                button22.Text = "记录";

            }



        }



        private void panel3_Paint(object sender, PaintEventArgs e)
        {

           
            
            if (drawflag == true)
            {
                XYLinesFactory.DrawXY(panelXshow);
                XYLinesFactory.DrawYLine(panelXshow, -10, 10, 4, "X");
                XYLinesFactory.DrawXLine(panelXshow, listcount, xscale);
                XYLinesFactory.DrawY(panelXshow, -10, 10, listcount, xlist);
            }
            
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            if (drawflag == true)
            {
                XYLinesFactory.DrawXY(panelWshow);
                XYLinesFactory.DrawYLine(panelWshow, -90, 90, 4, "W");
                XYLinesFactory.DrawXLine(panelWshow, listcount, xscale);
                XYLinesFactory.DrawY(panelWshow, -90, 90, listcount, wlist);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            if (drawflag == true)
            {
                XYLinesFactory.DrawXY(panelYshow);
                XYLinesFactory.DrawYLine(panelYshow, -4, 4, 4,"Y");
                XYLinesFactory.DrawXLine(panelYshow, listcount, xscale);
                XYLinesFactory.DrawY(panelYshow, -4, 4, listcount, ylist);
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            if (drawflag == true)
            {
                XYLinesFactory.DrawXY(panelZshow);
                XYLinesFactory.DrawYLine(panelZshow, -10, 10,4, "Z");
                XYLinesFactory.DrawXLine(panelZshow, listcount, xscale);
                XYLinesFactory.DrawY(panelZshow, -10, 10, listcount, zlist);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            panelMain.Visible = true;
            panel1.Visible = false;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            panelMain.Visible = true;
            panel1.Visible = false;
        }

   

     
       

       

      

       
    
      




      
    }
}
