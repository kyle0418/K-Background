using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace background
{
    public partial class Form1 : Form
    {
        // 安装钩子
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        // 卸载钩子
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(int idHook);
        // 继续下一个钩子
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        //声明定义
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        static int hKeyboardHook = 0;
        HookProc KeyboardHookProcedure;

        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public extern static void ShowCursor(int status);

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            foreach (Control control in this.Controls)
            {
                if (control is PictureBox)
                {
                    ((PictureBox)control).SizeMode = PictureBoxSizeMode.StretchImage;
                    ((PictureBox)control).Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                    ((PictureBox)control).Click += pictureBox_Click;
                }
            }

            btChoosePic.FlatStyle = FlatStyle.Flat;
            btChoosePic.ForeColor = Color.Transparent;
            btChoosePic.BackColor = Color.Transparent;
            btChoosePic.FlatAppearance.BorderSize = 0;
            btChoosePic.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btChoosePic.FlatAppearance.MouseDownBackColor = Color.Transparent;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            this.BackgroundImage = ((PictureBox)sender).Image;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HookStart();
            pictureBox1.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"/img/a.jpg");
            pictureBox2.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"/img/b.jpg");
            pictureBox3.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"/img/c.jpg");

            Cursortimer.Enabled = true;
            Cursortimer.Interval = 1000;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            HookStop();
        }

        // 安装钩子
        public void HookStart()
        {
            if (hKeyboardHook == 0)
            {
                // 创建HookProc实例
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                //定义全局钩子
                hKeyboardHook = SetWindowsHookEx(13, KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (hKeyboardHook == 0)
                {
                    HookStop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }
        //钩子子程就是钩子所要做的事情。
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //这里可以添加别的功能的代码
            return 1;
        }
        // 卸载钩子
        public void HookStop()
        {
            bool retKeyboard = true;
            if (hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
            }
            if (!(retKeyboard)) throw new Exception("UnhookWindowsHookEx failed.");
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                this.Close();
            }
        }

        bool flag = false;

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            SetPBVisible(flag);
            flag = !flag;
        }

        void SetPBVisible(bool b)
        {
            foreach (Control control in this.Controls)
            {
                if (control is PictureBox)
                {
                    ((PictureBox)control).Visible = b;
                }
            }
        }

        private Point m_Point = new Point();
        private DateTime _DateTime = DateTime.Now;
        private void Cursortimer_Tick(object sender, EventArgs e)
        {
            if (m_Point.X == Cursor.Position.X && m_Point.Y == Cursor.Position.Y)
            {
                ShowCursor(0);
            }
            else
            {
                m_Point = Cursor.Position;
                ShowCursor(1);
            }
        }

        private void btChoosePic_Click(object sender, EventArgs e)
        {
            OpenFileDialog picFileDialog = new OpenFileDialog();
            picFileDialog.Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|" +
                                    "Windows Bitmap(*.bmp)|*.bmp|" +
                                    "Windows Icon(*.ico)|*.ico|" +
                                    "Graphics Interchange Format (*.gif)|(*.gif)|" +
                                    "JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|" +
                                    "Portable Network Graphics (*.png)|*.png|" +
                                    "Tag Image File Format (*.tif)|*.tif;*.tiff";
            if (picFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.BackgroundImage = Image.FromFile(picFileDialog.FileName);
            }
        }
    }
}