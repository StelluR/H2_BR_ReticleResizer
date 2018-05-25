using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace H2TeamChanger
{
    public partial class MainForm : Form
    {

       public const String ProcessName = "halo2";
        Boolean LoadedProcess = false;
        Process[] process;
        Memory memory;

        IntPtr HealthMemAdr = IntPtr.Zero;
        IntPtr HealthMemAdr2 = IntPtr.Zero;
        IntPtr HealthMemAdr3 = IntPtr.Zero;

        int crazy = 0;
        int wire = 0;
        const int Width_Memory_Address = 0x479E70 + 0x7aa750;
        const int Height_Memory_Address = 0x479E70 + 0x7aa752;
        byte Width, Height;
        public static bool running = false;
        public bool t2running = false;
        public static  bool t3running = false;
        int i = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            process = Process.GetProcessesByName(ProcessName);
            UpdateTimer.Start();

            

        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
       
                Thread t2 = new Thread(delegate ()
                {
                    t2running = true;
                    while (!running)
                    {
                        if (CheckIfProcessIsRunning(ProcessName))
                        {
                            process = Process.GetProcessesByName(ProcessName);
                            running = true;



                        }
                        else
                        {
                            Thread.Sleep(10000);
                        }
                    }

                });
                t2.IsBackground = true;
                if (!t2running) t2.Start();
                if (running)
                {
                    try
                    {
                        if (process.Length > 0)
                        {
                            lblGameState.Text = "Running...";
                            lblGameState.ForeColor = System.Drawing.Color.LimeGreen;

                            process = Process.GetProcessesByName(ProcessName);
                            memory = new Memory(process[0]);
                            HealthMemAdr = memory.GetAddress("\"halo2.exe\"+0x479E70 + 0x7aa750");
                            HealthMemAdr2 = memory.GetAddress("\"halo2.exe\"+ 0x479E70 + 0x7aa752");
                            if(i < 3) DisplayValues();
                             i++;
                            if (i > 500) i = 0;





                    }
                }
                    catch (System.IndexOutOfRangeException){
                        running = false;
                        t2.Abort();
                        t2running = false;
                        i = 0;
                    }


                    }
                else
                {
                    lblGameState.Text = "Game is NOT Active!";
                    lblGameState.ForeColor = System.Drawing.Color.Red;
                    textBox1.Text = "?";
                    textBox2.Text = "?";
                }




        }

        public  bool CheckIfProcessIsRunning(string ProcessName)
        {

            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(ProcessName))
                {
                    return true;
                }
            }
            return false;
        }

        private void DisplayValues()
        {

            Width = memory.ReadByte(HealthMemAdr);
            Height = memory.ReadByte(HealthMemAdr2);


            textBox1.Text = Width.ToString();
            textBox2.Text = Height.ToString();

        }

        private void SetWidth(byte team)
        {
            if (process.Length > 0)
            {
                memory.WriteByte(HealthMemAdr, team);
            }
        }


        private void SetHeight(byte value)
        {
            if(process.Length > 0)
            {
                memory.WriteByte(HealthMemAdr2, value);
            }
        }





        #region TeamChangerButtons

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            if(process.Length > 0)
            {
                SetWidth(70);
                SetHeight(70);
                DisplayValues();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(process.Length > 0)
            {
                SetWidth(0);
                SetHeight(0);
                DisplayValues();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int newwidth;
            int newheight;
            if(process.Length > 0)
            {
                if (Int32.TryParse(textBox1.Text, out newwidth))
                {
                    SetWidth((byte)newwidth);
                }
                if (Int32.TryParse(textBox2.Text, out newheight))
                {
                    SetHeight((byte)newheight);
                }
            }

        }

     
    }
}
