using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System;
using System.Windows.Input;
using System.Diagnostics;
using GlobalHotKey;
using System.Windows.Forms;


namespace Bubble
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int HOTKEY_ID = 1;
        public const int WM_HOTKEY = 0x0312;
        public const int MOD_ALT = 0x0001;

        private ButtonOrb closeOrb = new ButtonOrb();
        private ButtonOrb minOrb = new ButtonOrb();
        private ButtonOrb winOrb = new ButtonOrb();
        public int base_width = 300;
        public int base_height = 300;
        public struct orbStyle
        {
            public Color color;
            public Image icon;
            public int rotation;
            public int size;
            public Image? icon2;
            public orbStyle(Color color, Image icon, int rotation, int size, Image icon2 = null)
            {
                this.color = color;
                this.icon = icon;
                this.rotation = rotation;
                this.size = size;
                this.icon2 = icon2;
            }
        }

        public Form1()
        {
            InitializeComponent();
            RegisterHotKey(this.Handle, HOTKEY_ID, MOD_ALT, (int)Keys.F1);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.LightBlue;
            this.Opacity = 0.5;
            this.WindowState = FormWindowState.Normal;

            double deltaTheta = Math.Asin(35 / ((double)318)) * 360 / double.Pi;
            orbStyle closestyle = new orbStyle(Color.Red, Image.FromFile("../../../media/Xicon.png"), 45 - (int)deltaTheta, 35);
            orbStyle winStyle = new orbStyle(Color.Gray, Image.FromFile("../../../media/Minicon.png"), 45, 35);
            orbStyle maxStyle = new orbStyle(Color.Gray, Image.FromFile("../../../media/Maxicon.png"), 45 + (int)deltaTheta, 35, Image.FromFile("../../../Media/Winicon.png"));

            this.Load += (s, e) =>
            {
                ApplyCircleForm();
                closeOrb.Show(this, closestyle, () =>
                {
                    this.Close();
                });
                minOrb.Show(this, winStyle, () =>
                {
                    this.WindowState = FormWindowState.Minimized;
                });
                winOrb.Show(this, maxStyle, () =>
                {
                    SwitchForm();
                });

            };
            this.Resize += (s, e) => this.Invalidate(); // Forces repaint on resize


            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 300;
            this.Height = 300;
            this.ShowInTaskbar = true;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            bool registered = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_ALT, (int)Keys.B);
            if (!registered)
            {
                MessageBox.Show("Failed to register hotkey.");
            }
        }

        private void SwitchForm()
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                ApplyWindowForm();
                this.WindowState = FormWindowState.Maximized;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                ApplyCircleForm();
                this.WindowState = FormWindowState.Normal;
            }
        }
        private void ApplyCircleForm()
        {
            this.Opacity = 0.5;
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, base_width, base_height);
            this.Region = new Region(path);
        }

        private void ApplyWindowForm()
        {
            this.BackColor = Color.LightCyan;
            this.Opacity = 1;
            int maxWidth = Screen.GetWorkingArea(this).Width;
            int maxHeight = Screen.GetWorkingArea(this).Height;
            Rectangle rect = new Rectangle(0, 0, maxWidth, maxHeight);
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(rect);
            this.Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (this.WindowState == FormWindowState.Normal)
            {

                int borderSize = 4;
                Rectangle borderRect = new Rectangle(
                            borderSize / 2,
                            borderSize / 2,
                            this.Width - borderSize,
                            this.Height - borderSize
                        );

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    borderRect, Color.Cyan, Color.Blue, LinearGradientMode.ForwardDiagonal))
                {
                    using (Pen pen = new Pen(brush, 8))
                    {
                        e.Graphics.DrawEllipse(pen, borderRect);
                    }
                }
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {

                int headSize = 32;
                Rectangle rect = new Rectangle(0, 32, this.Width - 32, this.Height - 32);
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect, Color.Cyan, Color.Blue, LinearGradientMode.ForwardDiagonal))
                {
                    using (Pen pen = new Pen(brush, 32))
                    {
                        e.Graphics.DrawLine(pen, 0, 0, this.Width, 0);
                    }
                }

            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            double deltaTheta = Math.Asin(35 / ((double)this.Width + 18)) * 360 / double.Pi;
            if (closeOrb != null)
            {

                closeOrb.UpdatePosition(this, this.WindowState == FormWindowState.Maximized, 45 - (int)deltaTheta, 1);
            }
            if (minOrb != null)
            {
                minOrb.UpdatePosition(this, this.WindowState == FormWindowState.Maximized, 45, 2);
            }
            if (winOrb != null)
            {
                winOrb.UpdatePosition(this, this.WindowState == FormWindowState.Maximized, 45 + (int)deltaTheta, 3);
            }
        }

        protected override void WndProc(ref Message m)
        {
            // Check if the message is a hotkey message and if the hotkey ID matches
            //Debug.WriteLine($"message: {m.Msg}");
            if (m.Msg == WM_HOTKEY)
            {
                // Handle the hotkey press
                Debug.WriteLine("Hotkey Pressed");
            }

            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Unregister the hotkey
            UnregisterHotKey(this.Handle, HOTKEY_ID);
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

    }
}
