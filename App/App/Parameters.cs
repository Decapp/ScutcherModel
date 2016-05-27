using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class Parameters : Form
    {
        Bitmap mbit;
        Graphics graphics;
            
            Pen pen = new Pen(Color.Black, 1f);
            Pen redPen = new Pen(Color.Red, 2f);

            public Parameters()
            {
                InitializeComponent();

                PictureRefresh();

                radioButton8.Checked = true;
            }


        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                label32.Enabled = true;
                textBox17.Enabled = true;
                label31.Enabled = true;

                label33.Enabled = false;
                label34.Enabled = false;
                label29.Enabled = false;
                label30.Enabled = false;
                label35.Enabled = false;
                label38.Enabled = false;
                label37.Enabled = false;
                label36.Enabled = false;
                label29.Enabled = false;

                textBox24.Enabled = false;
                textBox25.Enabled = false;
                textBox26.Enabled = false;
                textBox27.Enabled = false;
            }
            else
            {
                label32.Enabled = false;
                textBox17.Enabled = false;
                label31.Enabled = false;

                label33.Enabled = true;
                label34.Enabled = true;
                label29.Enabled = true;
                label30.Enabled = true;
                label35.Enabled = true;
                label38.Enabled = true;
                label37.Enabled = true;
                label36.Enabled = true;

                textBox24.Enabled = true;
                textBox25.Enabled = true;
                textBox26.Enabled = true;
                textBox27.Enabled = true;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //Нить
            double? threadWeight = 0.001;
            double[] threadWeigthRasp = { 0 };

            double expectedValueLength = 0.6;
            double varianceLength = 0.2;

            double threadYoungModul = 2 * Math.Pow(10, 10);
            double threadCrossSection = 314 * Math.Pow(10, -6);
            double threadPosition = 0.5;
            double threadTopDiameter = 0.0002;
            double threadMidDiameter = 0.0002;
            double threadBotDiameter = 0.0002;

            double threadFriction = 0.005;
                
            double expectedValueOffset = 0.06;
            double varianceOffset = 0.06;

            double expectedValueAngle = 0;
            double varianceAngle = 45;

            int threadPointCount = 0;

            //Модель
            int yarnCount = 10000;
            double dt = Math.Pow(10, -5);
            double windage = 0;

            //Барабаны
            double[,] rollParameter;

            //Зажим
            double clampLength = 0.08;
            double beltDistance = 0.14;
            double clampForce;


            ///Смещение

            expectedValueOffset = double.Parse(textBox9.Text);
            varianceOffset = double.Parse(textBox8.Text);

            ///Угол дезориентации

            expectedValueAngle = double.Parse(textBox13.Text);
            varianceAngle = double.Parse(textBox12.Text);

            ///Длина пряди

            expectedValueLength = double.Parse(textBox4.Text);
            varianceLength = double.Parse(textBox5.Text);

            ///Параметры пряди

            threadYoungModul = double.Parse(textBox31.Text) * Math.Pow(10, 10);
            threadPosition = double.Parse(textBox30.Text);
            threadPointCount = int.Parse(textBox29.Text);

            threadTopDiameter = double.Parse(textBox1.Text);
            threadMidDiameter = double.Parse(textBox1.Text);
            threadBotDiameter = double.Parse(textBox1.Text);


            ///Параметры модели

            dt = double.Parse(textBox2.Text) * Math.Pow(10, -7);
            yarnCount = int.Parse(textBox16.Text);

            ///Зажимной механизм

            beltDistance = double.Parse(textBox6.Text);
            clampLength = double.Parse(textBox15.Text);

            ///Барабаны

            int beatCount = int.Parse(textBox40.Text);

            double startAngle1 = double.Parse(textBox7.Text);
            double startAngle2 = double.Parse(textBox35.Text);

            rollParameter = new double[beatCount * 2, 6];

            for (int i = 0; i < beatCount; i++)
            {
                ///Левый барабан

                rollParameter[i, 0] = double.Parse(textBox39.Text);
                rollParameter[i, 1] = double.Parse(textBox3.Text);
                rollParameter[i, 2] = double.Parse(textBox42.Text);
                rollParameter[i, 3] = double.Parse(textBox38.Text) + double.Parse(textBox30.Text);
                rollParameter[i, 4] = startAngle1 + (i + 1) * (360 / beatCount);
                rollParameter[i, 5] = 1;

                ///Правый барабан

                rollParameter[i + beatCount, 0] = double.Parse(textBox39.Text);
                rollParameter[i + beatCount, 1] = double.Parse(textBox34.Text);
                rollParameter[i + beatCount, 2] = double.Parse(textBox42.Text);
                rollParameter[i + beatCount, 3] = double.Parse(textBox28.Text) + double.Parse(textBox30.Text);
                rollParameter[i + beatCount, 4] = startAngle2 + (i + 1) * (360 / beatCount); ;
                rollParameter[i + beatCount, 5] = -1;
            }

            ///Масса нити

            if (radioButton8.Checked)
            {
                threadWeight = double.Parse(textBox17.Text);
            }
            else
            {
                threadWeight = null;

                ///Распределение массы

                threadWeigthRasp = new double[4];

                threadWeigthRasp[3] = double.Parse(textBox24.Text);
                threadWeigthRasp[2] = double.Parse(textBox25.Text);
                threadWeigthRasp[1] = double.Parse(textBox26.Text);
                threadWeigthRasp[0] = double.Parse(textBox27.Text);
            }

            clampForce = double.Parse(textBox18.Text);


            WorkForm owner = this.Owner as WorkForm;
            if (owner != null)
            {
                ModelParameters p = new ModelParameters(threadWeight, threadWeigthRasp,expectedValueLength, 
                    varianceLength, threadTopDiameter, threadMidDiameter, threadBotDiameter,threadYoungModul,
                    threadPosition, threadFriction, expectedValueOffset,varianceOffset, expectedValueAngle, varianceAngle, 
                    rollParameter, clampLength, beltDistance, dt,threadPointCount, yarnCount, windage, clampForce);

                owner.MParameters = p;

                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Owner.Close();
        }

        private void Parameters_Load(object sender, EventArgs e)
        {

        }

        private void PictureRefresh()
        {
            double k = 300;

            int beatCount = int.Parse(textBox40.Text);

            double startAngle1 = double.Parse(textBox7.Text);
            double startAngle2 = double.Parse(textBox35.Text);

            double [,] rollParameter = new double[beatCount * 2, 6];

            for (int i = 0; i < beatCount; i++)
            {
                ///Левый барабан

                rollParameter[i, 0] = double.Parse(textBox39.Text);
                rollParameter[i, 1] = double.Parse(textBox3.Text);
                rollParameter[i, 2] = double.Parse(textBox42.Text);
                rollParameter[i, 3] = double.Parse(textBox38.Text) + double.Parse(textBox30.Text);
                rollParameter[i, 4] = startAngle1 + (i + 1) * (360 / beatCount);
                rollParameter[i, 5] = 1;

                ///Правый барабан

                rollParameter[i + beatCount, 0] = double.Parse(textBox39.Text);
                rollParameter[i + beatCount, 1] = double.Parse(textBox34.Text);
                rollParameter[i + beatCount, 2] = double.Parse(textBox42.Text);
                rollParameter[i + beatCount, 3] = double.Parse(textBox28.Text) + double.Parse(textBox30.Text);
                rollParameter[i + beatCount, 4] = startAngle2 + (i + 1) * (360 / beatCount); ;
                rollParameter[i + beatCount, 5] = -1;
            }

            Beater[] beaters = new Beater[rollParameter.GetLength(0)];

            for (int i = 0; i < rollParameter.GetLength(0); i++)
            {
                beaters[i] = new Beater(rollParameter[i, 0], rollParameter[i, 1],
                    rollParameter[i, 2], rollParameter[i, 3],
                    rollParameter[i, 4], (int)rollParameter[i, 5]);
            }

            mbit = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            graphics = Graphics.FromImage(mbit);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            try
            {
                /// била
                for (int i = 0; i < beaters.Length; i++)
                {
                    graphics.DrawLine(pen, (int)(k * beaters[i].Edge.X), (int)(k * beaters[i].Edge.Y),
                        (int)(k * beaters[i].Center.X), (int)(k * beaters[i].Center.Y));
                }

                pictureBox2.Image = mbit;
            }
            catch 
            {
                MessageBox.Show("Ошибка переполнения");
            }
        
        }

        private void textBox42_TextChanged(object sender, EventArgs e)
        {
            PictureRefresh();
        }
    }
}
