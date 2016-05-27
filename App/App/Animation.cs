using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class Animation : UserControl
    {
        int k; ///масштаб

        public Animation()
        {
            k = 400;
            InitializeComponent();
            pictureBox1.Parent = panel1;
        }

        public void Draw(dPoint[] thread, Beater[] beaters, double bendRadius)
        {
            Bitmap mbit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(mbit);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Pen pen = new Pen(Color.Black, 1f);
            Pen redPen = new Pen(Color.Red, 2f);


            try
            {

                /// точки
                for (int i = 0; i < thread.Length; i++)
                {
                    graphics.DrawEllipse(redPen, (int)(k * thread[i].X), (int)(k * thread[i].Y), 1, 1);
                }

                /// отрезки
                for (int i = 1; i < thread.Length; i++)
                {
                    double Xb = 0;
                    double Yb = 0;

                    bool transect = false;

                    for (int j = 0; j < beaters.Length; j++)
                    {
                        if (beaters[j].LastPoint != -1 && i == (beaters[j].LastPoint + 1) && beaters[j].HasTouch == true)
                        {
                            Xb = beaters[j].Edge.X;
                            Yb = beaters[j].Edge.Y;

                            transect = true;
                            break;
                        }
                    }

                    if (transect)
                    {
                        graphics.DrawLine(pen, (int)(k * thread[i - 1].X), (int)(k * thread[i - 1].Y),
                            (int)(k * Xb), (int)(k * Yb));

                        graphics.DrawLine(pen, (int)(k * thread[i].X), (int)(k * thread[i].Y),
                            (int)(k * Xb), (int)(k * Yb));
                    }
                    else
                    {
                        graphics.DrawLine(pen, (int)(k * thread[i - 1].X), (int)(k * thread[i - 1].Y),
                            (int)(k * thread[i].X), (int)(k * thread[i].Y));
                    }
                }

                /// била
                for (int i = 0; i < beaters.Length; i++)
                {
                    graphics.DrawLine(pen, (int)(k * beaters[i].Edge.X), (int)(k * beaters[i].Edge.Y),
                        (int)(k * beaters[i].Center.X), (int)(k * beaters[i].Center.Y));
                }


                double x = beaters[beaters.Length-1].Center.X-
                    beaters[beaters.Length-1].Radius-0.01+bendRadius;

                double y = beaters[beaters.Length-1].Center.Y;

                Point center = new Point((int)x,(int)y);

                x = center.X-bendRadius;
                y= center.Y-bendRadius;

                Point leftup = new Point((int)x, (int)y);

                graphics.DrawEllipse(pen,leftup.X,leftup.Y,(int)(2*bendRadius),(int)(2*bendRadius));

                pictureBox1.Image = mbit;
            }
            catch 
            {
                MessageBox.Show("Ошибка переполнения");
            }
        }

        private void Animation_Load(object sender, EventArgs e)
        {

        }
    }
}
