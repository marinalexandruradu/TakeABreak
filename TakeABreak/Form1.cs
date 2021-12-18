using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Reflection;
using Microsoft.Win32;

namespace TakeABreak
{

    public partial class Form1 : Form
    {
        private static System.Timers.Timer timer1;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run";
            string valueName = "TakeABreak";
            if (Registry.GetValue(keyName, valueName, null) == null)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
            release();
            InitTimer();


        }

        public void InitTimer()
        {
            timer1 = new System.Timers.Timer();
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Tick);
            int RunSeconds = Int32.Parse(textBox1.Text) * 1000;
            timer1.Interval = RunSeconds; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            release();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
             if (e.CloseReason == CloseReason.UserClosing)
            {
                notifyIcon1.Visible = true;
                this.Hide();
                e.Cancel = true;
                
                
            }
        }



        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        const int VK_UP = 0x7A; //F11 Key
        const int VK_DOWN = 0x28;  //down key
        const int VK_LEFT = 0x25;
        const int VK_RIGHT = 0x27;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        int press()
        {

            //Press the key
            keybd_event((byte)VK_UP, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            return 0;

        }
        int release()
        {
            //Release the key
            keybd_event((byte)VK_UP, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            Console.WriteLine("key released");
            return 0;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Control.MousePosition);
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox1 box = new AboutBox1())
            {
                box.ShowDialog(this);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                string DirectoryName = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string ExecutableName = System.AppDomain.CurrentDomain.FriendlyName;
                string WriteInKey = DirectoryName + "\\" + ExecutableName;
                key.SetValue("TakeABreak", WriteInKey);
            }
            if (!checkBox1.Checked) {
                string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (key == null)
                    {

                    }
                    else
                    {
                        key.DeleteValue("TakeABreak");
                    }
                }
            }
        }
    }
}
