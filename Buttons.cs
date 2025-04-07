using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Bubble
{
    

    public class ButtonOrb
    {
        private Form orbForm;
        public void Show(Form parentForm, Form1.orbStyle style, Action onClick = null)
        {
            orbForm = new Form();
            orbForm.FormBorderStyle = FormBorderStyle.None;
            orbForm.BackColor = style.color;
            orbForm.TopMost = true;
            orbForm.ShowInTaskbar = false;
            orbForm.StartPosition = FormStartPosition.Manual;
            orbForm.Owner = parentForm;
            parentForm.Resize += (s, e) => orbForm.Invalidate();

            Size orbSize = new Size(style.size, style.size);
            orbForm.Size = orbSize;

            // Round the orb
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, orbSize.Width, orbSize.Height);
            orbForm.Region = new Region(path);

            // Position relative to parent
            double anglerad = (double)style.rotation * double.Pi / 180;
            double Xpos = parentForm.Width / 2 - orbSize.Width / 2 + (parentForm.Width / 2 + orbForm.Width / 2) * Math.Cos(anglerad);
            double Ypos = parentForm.Height / 2 + orbSize.Height / 2 + (parentForm.Height / 2 + orbForm.Height / 2) * Math.Sin(anglerad);
            Point location = new Point(
                parentForm.Left + (int)Xpos, parentForm.Bottom - (int)Ypos
            );

            orbForm.Location = location;

            orbForm.Click += (s, e) =>
            {
                onClick?.Invoke(); // call the callback if it was given
            };

            orbForm.Paint += (s, e) =>
            {
                int borderSize = 2;
                Rectangle borderRect = new Rectangle(
                    borderSize,
                    borderSize,
                    orbSize.Width - borderSize * 2,
                    orbSize.Height - borderSize * 2
                );

                if (style.icon2 != null && parentForm.WindowState == FormWindowState.Maximized)
                {
                    e.Graphics.DrawImage(style.icon2, borderRect);
                }
                else
                {
                    e.Graphics.DrawImage(style.icon, borderRect);
                }
                using (LinearGradientBrush brush = new LinearGradientBrush(
                        borderRect, Color.FromArgb(120, 0, 255, 255), Color.FromArgb(120, 0, 0, 255), LinearGradientMode.BackwardDiagonal))
                    {
                        using (Pen pen = new Pen(brush, 8))
                        {
                            e.Graphics.DrawEllipse(pen, borderRect);
                        }
                    }
                //draw an image
                
            };

            orbForm.Show();
        }

        public void UpdatePosition(Form parentForm, bool maximised, int angle = 0, int barpos = 0)
        {
            if (maximised)
            {
                orbForm.Size = new Size(35, 35);
                orbForm.Location = new Point(parentForm.Right - barpos*orbForm.Width, parentForm.Top);
                return;
            }
            if (orbForm != null && !orbForm.IsDisposed)
            {
                Size orbSize = new Size(35, 35);
                orbForm.Size = orbSize;
                double anglerad = (double)angle * double.Pi / 180;
                double Xpos = parentForm.Width / 2 - orbSize.Width / 2 + (parentForm.Width / 2 + orbForm.Width / 2) * Math.Cos(anglerad);
                double Ypos = parentForm.Height / 2 + orbSize.Height / 2 + (parentForm.Height / 2 + orbForm.Height / 2) * Math.Sin(anglerad);
                orbForm.Location = new Point(
                    parentForm.Left + (int)Xpos, parentForm.Bottom - (int)Ypos
                );
            }
        }

        public void Close()
        {
            orbForm?.Close();
        }

    }

}
