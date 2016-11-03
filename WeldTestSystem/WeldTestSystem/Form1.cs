using System;
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
        public Form1()
        {

            string[] strport = SerialPort.GetPortNames();

            if (strport.Length == 0)
            {
                MessageBox.Show("本机没找到端口", "Error");
                return;
               
            }

            InitializeComponent();

          

            panel1.Visible = true;
            panel2.Visible = false;
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


           

            Thread IOthread = new Thread(IOListener);
            IOthread.Start();



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

                do
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

                    Thread.Sleep(50);

                } while (true);


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
            panel1.Visible = false;

            panel2.Visible = true;


        }

        private void button19_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;


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
            panel1.Visible = true;
            panel2.Visible = false;
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
                    port.DataReceived += new SerialDataReceivedEventHandler(ComRec);
                    groupBox3.Visible = true;
                    groupBox4.Visible = true;
                    groupBox5.Visible = true;

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

            }
        }

        private void ComSend(String data)
        {

            Console.WriteLine(data + "\r\n");
            port.WriteLine(data + "\r\n");
            Thread.Sleep(100);


        }

        private void ComRec(object sender, SerialDataReceivedEventArgs e)
        {
            String rec = port.ReadLine();
            Console.WriteLine(rec);
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
            ComSend(textBox6.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            upmove();

        }
        private void upmove()
        {
            String str = "3lr" + textBoxup.Text;
            ComSend(str);
            ComSend("3sp200");
            ComSend("3m");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            forwardmove();

           
        }

        private void forwardmove()
        {
            String str = "2lr" + textBoxforward.Text;
            ComSend(str);
            ComSend("2sp200");
            ComSend("2m");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            backwardmove();
           
        }

        private void backwardmove()
        {
            String str = "2lr-" + textBoxback.Text;
            ComSend(str);
            ComSend("2sp200");
            ComSend("2m");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            downmove();
          
           
        }

        private void downmove()
        {
            String str = "3lr-" + textBoxdown.Text;
            ComSend(str);
            ComSend("3sp200");
            ComSend("3m");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            leftmove();
          
        }

        private void leftmove()
        {
            String str = "1lr-" + textBoxleft.Text;
            ComSend(str);
            ComSend("1sp200");
            ComSend("1m");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            rightmove();
           
        }

        private void rightmove()
        {
            String str = "1lr" + textBoxright.Text;
            ComSend(str);
            ComSend("1sp200");
            ComSend("1m");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            clockmove();
           
        }

        private void clockmove()
        {
            String str = "4LR" + textBoxclock.Text;
            ComSend(str);
            ComSend("4sp200");
            ComSend("4m");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            counterclock();
          

        }

        private void counterclock()
        {
            String str = "4LR-" + textBoxcounterclock.Text;
            ComSend(str);
            ComSend("4sp200");
            ComSend("4m");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            conformbtn();
           
        }

        private void conformbtn()
        {
            ComSend("1HO");
            ComSend("2HO");
            ComSend("3HO");
            ComSend("4HO");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            pausebtn();
          
        }

        private void pausebtn()
        {
            ComSend("1V0");
            ComSend("2V0");
            ComSend("3V0");
            ComSend("4V0");
        }

        private void button13_Click(object sender, EventArgs e)
        {

        }
        private bool deviceisopen = false;
        private void button14_Click(object sender, EventArgs e)
        {
            if (deviceisopen == false)
            {
                ComSend("1EN");
                ComSend("2EN");
                ComSend("3EN");
                ComSend("4EN");

                button14.Text = "关";
                button14.BackColor = Color.Green;

                deviceisopen = true;
            }
            else
            {
                ComSend("1DI");
                ComSend("2DI");
                ComSend("3DI");
                ComSend("4DI");
                button14.Text = "开";
                button14.BackColor = Color.Red;
                deviceisopen = false;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {

            resetbtn();
           
        }

        private void resetbtn()
        {
            ComSend("1LA0");
            ComSend("2LA0");
            ComSend("3LA0");
            ComSend("4LA0");

            ComSend("1SP200");
            ComSend("2SP200");
            ComSend("3SP200");
            ComSend("4SP200");

            ComSend("1NP");
            ComSend("2NP");
            ComSend("3NP");
            ComSend("4NP");

            ComSend("1M");
            ComSend("2M");
            ComSend("3M");
            ComSend("4M");
        }



        private void button18_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lis.Count; i++)
            {

                List<String> vec = new List<String>();
                String str = lis[i];
                String[] arrstr = str.Split(';');
                for (int j = 1; j < arrstr.Length; j++)
                {
                    vec.Add(arrstr[j]);

                }

                for (int k = 0; k < vec.Count; k++)
                {
                    if (vec[k] != "")
                    {
                        ComSend(vec[k]);

                    }
                }


            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void textBoxup_TextChanged(object sender, EventArgs e)
        {

        }



    }
}
