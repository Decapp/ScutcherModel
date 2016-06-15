using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace App
{
    public partial class Parameters : Form
    {
        Pen penBeat;
        Pen whiteBeat;
        Pen penDash;

        double[] weightLength;
        double[] clampForce;

        public Parameters()
        {
            InitializeComponent();

            penBeat = new Pen(Color.Black, 3f);
            whiteBeat = new Pen(Color.White, 4f);
            penDash = new Pen(Brushes.Gray, 1f);
            penDash.DashStyle = DashStyle.Dash;

            PictureRefresh();

            radioButton8.Checked = true;
            radioButton3.Checked = true;

            ///Коэффициенты функции массы от длины
            double[] x1 = { 93, 78, 85, 73, 98, 79, 80, 92, 68, 87, 75, 77, 79, 58, 80 };

            double[] y1 = { 0.256, 0.129, 0.310, 0.142, 0.313, 0.203, 0.155, 0.218, 0.111, 
                             0.181, 0.163, 0.150, 0.167, 0.106, 0.146 };

            this.weightLength = ApprFunction(x1, y1);

            textBox41.Text = weightLength[2].ToString("f5");
            textBox43.Text = weightLength[1].ToString("f5");
            textBox44.Text = weightLength[0].ToString("f5");

            for (double i = 50; i <= 100; i += 1)
            {
                double j = weightLength[2] * Math.Pow(i, 2) + weightLength[1] * i + weightLength[0];
                chart2.Series[0].Points.AddXY(i, j);
            }
        }


        /// <summary>
        /// Сохранение и запуск основной формы
        /// </summary>
        private void SaveAndLoad()
        {
            #region Переменные

            //Нить
            double?[] threadWeigthRasp = { 0 };

            double expectedValueLength = 0;
            double varianceLength = 0;

            double threadYoungModul = 0;
            double threadPosition = 0;
            double? threadTopDiameter = 0;
            double? threadMidDiameter = 0;
            double? threadBotDiameter = 0;

            double threadFriction = 0;
            double threadHard = 0;
            double? threadPlotn = 0;

            double expectedValueOffset = 0;
            double varianceOffset = 0;

            double expectedValueAngle = 0;
            double varianceAngle = 0;

            int threadPointCount = 0;

            //Модель
            int yarnCount = 0;
            double dt = 0;
            double windage = 0;

            //Барабаны
            double[,] rollParameter = new double[0, 0];

            //Зажим
            double clampLength = 0;
            double beltDistance = 0;
            int clampType = 0;

            int beatCount = 0;

            double startAngle1 = 0;
            double startAngle2 = 0;

            #endregion

            ///Изменение параметров

            double a;
            int b;

            #region Смещение

            if (double.TryParse(textBox9.Text, out a) || textBox9.Text != "0")
                expectedValueOffset = a;
            else
            {
                errorProvider1.SetError(textBox9, "Неверное значение");
                return;
            }


            if (double.TryParse(textBox8.Text, out a))
                varianceOffset = a;
            else
            {
                errorProvider1.SetError(textBox8, "Неверное значение");
                return;
            }

            #endregion

            #region Угол дезориентации

            if (double.TryParse(textBox13.Text, out a) || textBox13.Text != "0")
                expectedValueAngle = a;
            else
            {
                errorProvider1.SetError(textBox13, "Неверное значение");
                return;
            }


            if (double.TryParse(textBox12.Text, out a))
                varianceAngle = a;
            else
            {
                errorProvider1.SetError(textBox12, "Неверное значение");
                return;
            }

            #endregion

            #region Длина пряди

            if (double.TryParse(textBox4.Text, out a) && textBox4.Text != "0")
                expectedValueLength = a;
            else
            {
                errorProvider1.SetError(textBox4, "Неверное значение");
                return;
            }


            if (double.TryParse(textBox5.Text, out a))
                varianceLength = a;
            else
            {
                errorProvider1.SetError(textBox5, "Неверное значение");
                return;
            }

            #endregion

            #region Параметры пряди

            if (double.TryParse(textBox31.Text, out a) && textBox31.Text != "0")
                threadYoungModul = a * Math.Pow(10, 10);
            else
            {
                errorProvider1.SetError(textBox31, "Неверное значение");
                return;
            }


            if (double.TryParse(textBox30.Text, out a))
                threadPosition = a;
            else
            {
                errorProvider1.SetError(textBox30, "Неверное значение");
                return;
            }


            threadPointCount = (int)numericUpDown1.Value;


            if (double.TryParse(textBox11.Text, out a))
                threadFriction = a;
            else
            {
                errorProvider1.SetError(textBox11, "Неверное значение");
                return;
            }

            if (double.TryParse(textBox14.Text, out a))
                threadHard = a * Math.Pow(10, -10);
            else
            {
                errorProvider1.SetError(textBox14, "Неверное значение");
                return;
            }

            #endregion

            #region Параметры модели

            if (double.TryParse(textBox2.Text, out a) && textBox2.Text != "0")
                dt = a * Math.Pow(10, -7);
            else
            {
                errorProvider1.SetError(textBox2, "Неверное значение");
                return;
            }


            if (int.TryParse(textBox16.Text, out b) && textBox16.Text != "0")
                yarnCount = b;
            else
            {
                errorProvider1.SetError(textBox16, "Неверное значение");
                return;
            }


            if (double.TryParse(textBox10.Text, out a))
                windage = a;
            else
            {
                errorProvider1.SetError(textBox10, "Неверное значение");
                return;
            }

            #endregion

            #region Зажимной механизм

            if (double.TryParse(textBox15.Text, out a))
                beltDistance = a;
            else
            {
                errorProvider1.SetError(textBox15, "Неверное значение"); return;
            }
            if (radioButton3.Checked)
            {
                clampType = 1;

                double b1 = 0;
                double h = 0;

                if (double.TryParse(textBox6.Text, out a))
                    b1 = a;
                else
                {
                    errorProvider1.SetError(textBox6, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox23.Text, out a))
                    h = a;
                else
                {
                    errorProvider1.SetError(textBox23, "Неверное значение");
                    return;
                }

                clampLength = 2 * h + b1;
            }
            else if (radioButton4.Checked)
            {
                clampType = 2;

                double R = 0;

                if (double.TryParse(textBox33.Text, out a) && textBox33.Text != "0")
                    R = a;
                else
                {
                    errorProvider1.SetError(textBox33, "Неверное значение");
                    return;
                }

                clampLength = Math.PI * R;
            }
            else
            {
                clampType = 3;

                double b1 = 0;
                double h = 0;
                double R = 0;

                if (double.TryParse(textBox36.Text, out a))
                    b1 = a;
                else
                {
                    errorProvider1.SetError(textBox36, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox48.Text, out a))
                    h = a;
                else
                {
                    errorProvider1.SetError(textBox48, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox37.Text, out a))
                {
                    R = a;
                    if (R < b1)
                    {
                        errorProvider1.SetError(textBox37, "Неверное значение");
                        return;
                    }
                }
                else
                {
                    errorProvider1.SetError(textBox37, "Неверное значение");
                    return;
                }

                double asin = Math.Asin(b1 / (2 * R));
                double alpha = 2 * asin;
                clampLength = 2 * h + R * alpha;
            }

            #endregion

            #region Барабаны

            if (int.TryParse(textBox40.Text, out b) && textBox40.Text != "0")
                beatCount = b;
            else
            {
                errorProvider1.SetError(textBox40, "Неверное значение");
                return;
            }

            if (double.TryParse(textBox7.Text, out a))
                startAngle1 = a;
            else
            {
                errorProvider1.SetError(textBox7, "Неверное значение");
                return;
            }

            if (double.TryParse(textBox35.Text, out a))
                startAngle2 = a;
            else
            {
                errorProvider1.SetError(textBox35, "Неверное значение");
                return;
            }

            rollParameter = new double[beatCount * 2, 6];

            for (int i = 0; i < beatCount; i++)
            {
                try
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
                catch
                {
                    MessageBox.Show("Пожалуйста, проверьте параметры барабанов", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            #endregion

            #region Масса нити

            if (radioButton8.Checked)
            {

                if (double.TryParse(textBox20.Text, out a) && textBox20.Text != "0")
                    threadTopDiameter = a;
                else
                {
                    errorProvider1.SetError(textBox20, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox22.Text, out a) && textBox22.Text != "0")
                    threadPlotn = a;
                else
                {
                    errorProvider1.SetError(textBox22, "Неверное значение");
                    return;
                }

                threadMidDiameter = null;
                threadBotDiameter = null;
                threadWeigthRasp = null;

            }
            else if (radioButton2.Checked)
            {
                if (double.TryParse(textBox1.Text, out a) && textBox1.Text != "0")
                    threadTopDiameter = a;
                else
                {
                    errorProvider1.SetError(textBox1, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox17.Text, out a) && textBox17.Text != "0")
                    threadMidDiameter = a;
                else
                {
                    errorProvider1.SetError(textBox17, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox19.Text, out a) && textBox19.Text != "0")
                    threadBotDiameter = a;
                else
                {
                    errorProvider1.SetError(textBox19, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox21.Text, out a) && textBox21.Text != "0")
                    threadPlotn = a;
                else
                {
                    errorProvider1.SetError(textBox21, "Неверное значение");
                    return;
                }

                threadWeigthRasp = null;
            }
            else
            {
                ///Распределение массы

                threadWeigthRasp = new double?[4];

                if (double.TryParse(textBox27.Text, out a) && textBox27.Text != "0")
                    threadWeigthRasp[3] = a;
                else
                {
                    errorProvider1.SetError(textBox27, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox26.Text, out a) && textBox26.Text != "0")
                    threadWeigthRasp[2] = a;
                else
                {
                    errorProvider1.SetError(textBox26, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox25.Text, out a) && textBox25.Text != "0")
                    threadWeigthRasp[1] = a;
                else
                {
                    errorProvider1.SetError(textBox25, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox24.Text, out a) && textBox24.Text != "0")
                    threadWeigthRasp[0] = a;
                else
                {
                    errorProvider1.SetError(textBox24, "Неверное значение");
                    return;
                }

                if (double.TryParse(textBox22.Text, out a) && textBox22.Text != "0")
                    threadPlotn = a;
                else
                {
                    errorProvider1.SetError(textBox22, "Неверное значение");
                    return;
                }

                threadTopDiameter = null;
                threadMidDiameter = null;
                threadBotDiameter = null;
            }

            #endregion

            WorkForm owner = this.Owner as WorkForm;
            if (owner != null)
            {
                ModelParameters p = new ModelParameters(threadWeigthRasp, expectedValueLength,
                    varianceLength, threadTopDiameter, threadMidDiameter, threadBotDiameter, threadYoungModul,
                    threadPosition, threadFriction, threadHard, threadPlotn, expectedValueOffset, varianceOffset,
                    expectedValueAngle, varianceAngle, rollParameter, clampLength, beltDistance, clampType, dt,
                    threadPointCount, yarnCount, windage, clampForce, weightLength);

                owner.MParameters = p;
                owner.Visible = true;

                this.Close();
            }
        }

        /// <summary>
        /// Кнопка "ОК" Сохранение параметров, переход к модели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            SaveAndLoad();
        }

        /// <summary>
        /// Кнопка "Отмена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Owner.Close();
        }

        /// <summary>
        /// Перерисовка бильных барабанов
        /// </summary>
        private void PictureRefresh()
        {
            double k = 330;
            try
            {
                int beatCount = int.Parse(textBox40.Text);

                double startAngle1 = double.Parse(textBox7.Text);
                double startAngle2 = double.Parse(textBox35.Text);

                double[,] rollParameter = new double[beatCount * 2, 6];

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

                for (int i = 0; i < beaters.Length; i++)
                {
                    beaters[i] = new Beater(rollParameter[i, 0], rollParameter[i, 1], rollParameter[i, 2],
                        rollParameter[i, 3], rollParameter[i, 4], (int)rollParameter[i, 5]);
                }

                Bitmap mbit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics graphics = Graphics.FromImage(mbit);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;



                ///Положение нити

                double a;
                if (double.TryParse(textBox30.Text, out a))
                {
                    Point st = new Point((int)(a * k), 0);
                    Point end = new Point((int)(a * k), pictureBox2.Height);

                    graphics.DrawLine(penDash, st, end);

                    pictureBox7.Location = new Point((int)(a * k - 49), pictureBox7.Location.Y);
                }

                /// била
                for (int i = 0; i < beaters.Length; i++)
                {
                    graphics.DrawLine(penBeat, (int)(k * beaters[i].Edge.X), (int)(k * beaters[i].Edge.Y),
                        (int)(k * beaters[i].Center.X), (int)(k * beaters[i].Center.Y));
                }

                ///Центры барабанов

                Point left1 = new Point((int)(k * beaters[0].Center.X) - 6, (int)(k * beaters[0].Center.Y) - 6);
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


                pictureBox2.Image = mbit;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Изменение параметров била и положения нити
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox42_TextChanged(object sender, EventArgs e)
        {
            PictureRefresh();
            Control c = (Control)sender;
            errorProvider1.SetError(c, "");
        }

        /// <summary>
        /// Аппроксимация функции
        /// </summary>
        public double[] ApprFunction(double[] x, double[] y)
        {
            double[,] a = new double[3, 3];
            double[] b = new double[3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        a[i, j] = x.Length;
                    }
                    else
                    {
                        int k = i + j;

                        for (int g = 0; g < x.Length; g++)
                        {
                            a[i, j] += Math.Pow(x[g], k);
                        }
                    }
                }

                for (int g = 0; g < x.Length; g++)
                {
                    b[i] += y[g] * Math.Pow(x[g], i);
                }
            }

            double[] delta = new double[4];

            delta[0] = a[0, 0] * a[1, 1] * a[2, 2] + a[0, 1] * a[1, 2] * a[2, 0] +
                a[1, 0] * a[2, 1] * a[0, 2] - a[0, 2] * a[1, 1] * a[2, 0] -
                a[0, 1] * a[1, 0] * a[2, 2] - a[0, 0] * a[1, 2] * a[2, 1];

            delta[1] = b[0] * a[1, 1] * a[2, 2] + a[0, 1] * a[1, 2] * b[2] +
                b[1] * a[2, 1] * a[0, 2] - a[0, 2] * a[1, 1] * b[2] -
                a[0, 1] * b[1] * a[2, 2] - b[0] * a[1, 2] * a[2, 1];

            delta[2] = a[0, 0] * b[1] * a[2, 2] + b[0] * a[1, 2] * a[2, 0] +
                a[1, 0] * b[2] * a[0, 2] - a[0, 2] * b[1] * a[2, 0] -
                b[0] * a[1, 0] * a[2, 2] - a[0, 0] * a[1, 2] * b[2];

            delta[3] = a[0, 0] * a[1, 1] * b[2] + a[0, 1] * b[1] * a[2, 0] +
                a[1, 0] * a[2, 1] * b[0] - b[0] * a[1, 1] * a[2, 0] -
                a[0, 1] * a[1, 0] * b[2] - a[0, 0] * b[1] * a[2, 1];

            double a0 = delta[1] / delta[0];
            double a1 = delta[2] / delta[0];
            double a2 = delta[3] / delta[0];

            double[] d = new double[3];
            d[0] = delta[1] / delta[0];
            d[1] = delta[2] / delta[0];
            d[2] = delta[3] / delta[0];

            return d;
        }

        /// <summary>
        /// Изменение варианта распределения массы вдоль стебля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                groupBox5.Enabled = true;
                groupBox12.Enabled = false;
                groupBox13.Enabled = false;
            }
            else if (radioButton2.Checked)
            {
                groupBox5.Enabled = false;
                groupBox12.Enabled = true;
                groupBox13.Enabled = false;
            }
            else
            {
                groupBox5.Enabled = false;
                groupBox12.Enabled = false;
                groupBox13.Enabled = true;
            }
        }

        /// <summary>
        /// Изменение типа зажимного механизма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();

            if (radioButton3.Checked)
            {
                groupBox2.Enabled = true;
                groupBox14.Enabled = false;
                groupBox15.Enabled = false;


                ///Коэффициенты функции силы зажима
                double[] x = { 0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09 };

                double[] y = { 0.7, 0.9, 0.11, 2.1, 2, 4, 3.5, 6.5, 7, 8 };

                this.clampForce = ApprFunction(x, y);

                textBox47.Text = clampForce[2].ToString("f5");
                textBox46.Text = clampForce[1].ToString("f5");
                textBox45.Text = clampForce[0].ToString("f5");

                for (double i = 0; i <= 0.1; i += 0.01)
                {
                    double j = clampForce[2] * Math.Pow(i, 2) + clampForce[1] * i + clampForce[0];
                    chart1.Series[0].Points.AddXY(i, j / 10);
                }
            }
            else if (radioButton4.Checked)
            {
                groupBox2.Enabled = false;
                groupBox14.Enabled = true;
                groupBox15.Enabled = false;


                ///Коэффициенты функции силы зажима
                double[] x = { 0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09 };

                double[] y = { 1.3, 1.7, 1.6, 2.8, 2.4, 4.9, 4.7, 7.3, 7.9, 8.7 };

                this.clampForce = ApprFunction(x, y);

                textBox47.Text = clampForce[2].ToString("f5");
                textBox46.Text = clampForce[1].ToString("f5");
                textBox45.Text = clampForce[0].ToString("f5");

                for (double i = 0; i <= 0.1; i += 0.01)
                {
                    double j = clampForce[2] * Math.Pow(i, 2) + clampForce[1] * i + clampForce[0];
                    chart1.Series[0].Points.AddXY(i, j / 10);
                }
            }
            else
            {
                groupBox2.Enabled = false;
                groupBox14.Enabled = false;
                groupBox15.Enabled = true;


                ///Коэффициенты функции силы зажима
                double[] x = { 0, 0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09 };

                double[] y = { 1, 0.9, 1.2, 2.1, 2.7, 4, 3.5, 6.2, 7.1, 7 };

                this.clampForce = ApprFunction(x, y);

                textBox47.Text = clampForce[2].ToString("f5");
                textBox46.Text = clampForce[1].ToString("f5");
                textBox45.Text = clampForce[0].ToString("f5");

                for (double i = 0; i <= 0.1; i += 0.01)
                {
                    double j = clampForce[2] * Math.Pow(i, 2) + clampForce[1] * i + clampForce[0];
                    chart1.Series[0].Points.AddXY(i, j / 10);
                }
            }


        }

        private void Parameters_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveAndLoad();
            }
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            Control c = (Control)sender;
            errorProvider1.SetError(c, "");
        }
    }
}
