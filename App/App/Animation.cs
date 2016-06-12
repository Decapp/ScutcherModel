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

        Pen penBeat;
        Pen whiteBeat ;
        Pen penThread ;
        Pen redThread ;
       
        public Animation()
        {
            k = 430;

            penBeat = new Pen(Color.Black, 3f);
            whiteBeat = new Pen(Color.White, 4f);

             penThread = new Pen(Color.DarkBlue, 1f);
             redThread = new Pen(Color.Red, 1f);

            InitializeComponent();
        }

        public void Draw(dPoint[] thread, Beater[] beaters)
        {
            Bitmap mbit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics graphics = Graphics.FromImage(mbit);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            

            try
            {

                /// точки
                for (int i = 0; i < thread.Length; i++)
                {
                    graphics.DrawEllipse(redThread, (int)(k * thread[i].X), (int)(k * thread[i].Y), 1, 1);
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
                        graphics.DrawLine(penThread, (int)(k * thread[i - 1].X), (int)(k * thread[i - 1].Y),
                            (int)(k * Xb), (int)(k * Yb));

                        graphics.DrawLine(penThread, (int)(k * thread[i].X), (int)(k * thread[i].Y),
                            (int)(k * Xb), (int)(k * Yb));
                    }
                    else
                    {
                        graphics.DrawLine(penThread, (int)(k * thread[i - 1].X), (int)(k * thread[i - 1].Y),
                            (int)(k * thread[i].X), (int)(k * thread[i].Y));
                    }
                }

                /// била
                for (int i = 0; i < beaters.Length; i++)
                {
                    graphics.DrawLine(penBeat, (int)(k * beaters[i].Edge.X), (int)(k * beaters[i].Edge.Y),
                        (int)(k * beaters[i].Center.X), (int)(k * beaters[i].Center.Y));
                }

                ///Центры барабанов

                Point left1 = new Point((int)(k *beaters[0].Center.X) - 6, (int)(k * beaters[0].Center.Y) - 6);
                Point left2 = new Point((int)(k * beaters[0].Center.X) - 3, (int)(k * beaters[0].Center.Y) - 3);

                int width1 = 12;
                int width2 = 6;

                graphics.DrawEllipse(penBeat, left1.X, left1.Y, width1, width1);
                graphics.DrawEllipse(whiteBeat, left2.X, left2.Y, width2, width2);


                left1 = new Point((int)(k * beaters[beaters.Length - 1].Center.X) - 6, (int)(k * beaters[beaters.Length - 1].Center.Y) - 6);
                left2 = new Point((int)(k * beaters[beaters.Length - 1].Center.X) - 3, (int)(k * beaters[beaters.Length - 1].Center.Y) - 3);

                width1 = 12;
                width2 = 6;

                graphics.DrawEllipse(penBeat, left1.X, left1.Y, width1, width1);
                graphics.DrawEllipse(whiteBeat, left2.X, left2.Y, width2, width2);


                pictureBox1.Image = mbit;
            }
            catch
            {
                MessageBox.Show("Ошибка переполнения");
            }
        }

        public void ClampParameters(int type, double location)
        {
            if (type == 1)
                pictureBox2.Image = Properties.Resources.clamp1;
            if (type == 2)
                pictureBox2.Image = Properties.Resources.clamp2;
            if (type == 3)
                pictureBox2.Image = Properties.Resources.clamp3;

            pictureBox2.Location = new Point((int)(location * k - 49), pictureBox2.Location.Y);

            pictureBox1.Image = null;
        }
    }
}
