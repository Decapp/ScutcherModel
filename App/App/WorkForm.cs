using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace App
{
    public partial class WorkForm : Form
    {

        Yarn currentYarn; ///Текущий стебель
        int currentNumberYarn; ///Номер текущего стебля

        Beater[] beaters; ///Бильные планки

        Random rnd;

        ModelParameters parameters; ///Параметры модели

        Excel.Application application; ///Приложение Excel (Для вывода данных)

        double[] lengthArray; ///Массив длин
        double[] offsetArray; ///Массив смещений
        double[] angleArray; ///Массив углов дезориентации

        double Time; ///Время работы модели

        double[,] rezultArray; ///Массив с результатами


        public WorkForm()
        {
            Parameters param = new Parameters(); ///Ввод параметров
            param.Owner = this;
            param.ShowDialog();

            rnd = new Random();

            ///Имитационное моделирование потока слоя

            #region Массивы длин, смещений и углов

            ///Массив длин

            lengthArray = new double[parameters.yarnCount];

            for (int i = 0; i < parameters.yarnCount; i++)
            {
                double num = genNormNumber(parameters.expectedValueLength * 100,
                    parameters.varianceLength * 100);

                lengthArray[i] = num / 100;
            }


            ///Массив смещений

            offsetArray = new double[parameters.yarnCount];

            for (int i = 0; i < parameters.yarnCount; i++)
            {
                double num = genNormNumber(parameters.expectedValueOffset * 100,
                    parameters.varianceOffset * 100);

                offsetArray[i] = num / 100;
            }


            ///Массив углов

            angleArray = new double[parameters.yarnCount];

            for (int i = 0; i < parameters.yarnCount; i++)
            {
                double num = genNormNumber(parameters.expectedValueAngle,
                    parameters.varianceAngle);

                angleArray[i] = num;
            }


            #endregion

            Time = 0;

            rezultArray = new double[parameters.yarnCount, 3];

            InitializeComponent();
            
            animation1.ClampParameters(parameters.clampType, parameters.threadPosition);
        }

        /// <summary>
        /// Запуск следующей нити
        /// </summary>
        private void NextYarn()
        {
            ///Тут должны быть расчеты
            ///Расчет ведется для предыдущей нити
            ///Берется макс натяжение в нити
            ///Остальное расчитывается здесь


            if (currentYarn != null)
            {
                //chart1.Series[0].Points.Add(currentYarn.MaxF);
                //chart1.Series[1].Points.Add(currentYarn.MinF);
                //chart1.Series[2].Points.Add(currentYarn.SrF);
            }

            if (currentNumberYarn != 0)
            {
                if (currentYarn.SrF < parameters.clampForce)
                {
                    MessageBox.Show("Прядь сохранена. Максимальная сила натяжения - " + currentYarn.MaxF + " H.");
                    rezultArray[currentNumberYarn - 1, 1] = 1;
                }
                else
                {
                    MessageBox.Show("Прядь вылетела. Максимальная сила натяжения - " + currentYarn.MaxF + " H.");
                    rezultArray[currentNumberYarn - 1, 1] = 0;
                }

                if (currentYarn.Error)
                {
                    MessageBox.Show("Ошибка в движении нити, продолжить работу?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    rezultArray[currentNumberYarn - 1, 1] = 2;
                }

                rezultArray[currentNumberYarn - 1, 2] = currentYarn.MaxF;
            }

            if (currentNumberYarn == parameters.yarnCount)
            {
                timer2.Stop();


                string text = ""+Environment.NewLine;

                for (int i = 0; i < rezultArray.GetLength(0); i++)
                {
                    text += i.ToString() + " - " + rezultArray[i, 1].ToString() 
                        + " - " + rezultArray[i, 2]+Environment.NewLine;
                }

                text += Environment.NewLine;

                MessageBox.Show("Процесс закончен. Затрачено времени - "+label3.Text.ToString()+"c."+text);

                //WriteRezult();

                return;
            }
            else
            {
                beaters = new Beater[parameters.rollParameter.GetLength(0)];

                for (int i = 0; i < parameters.rollParameter.GetLength(0); i++)
                {
                    beaters[i] = new Beater(parameters.rollParameter[i, 0], parameters.rollParameter[i, 1],
                        parameters.rollParameter[i, 2], parameters.rollParameter[i, 3],
                        parameters.rollParameter[i, 4], (int)parameters.rollParameter[i, 5]);
                }

                currentYarn = new Yarn(parameters.threadWeightRasp,
                    lengthArray[(int)currentNumberYarn], parameters.threadTopDiameter,
                    parameters.threadMidDiameter, parameters.threadBotDiameter, 
                    parameters.threadYoungModul,parameters.dt, parameters.threadPointCount,
                    parameters.threadPosition, angleArray[(int)currentNumberYarn], parameters.clampLength,
                    parameters.beltDistance, offsetArray[(int)currentNumberYarn],parameters.threadFriction,
                    parameters.threadHard,parameters.threadDensity,parameters.weightLength,
                    parameters.windage, beaters);

                label1.Text = currentYarn.Length.ToString();

                currentNumberYarn++;
                
                timer1.Start();
            }
        }

        /// <summary>
        /// Кнопка "Старт-Пауза"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (currentNumberYarn == 0)
            {
                NextYarn();
                button1.Text = "Пауза";
                timer2.Start();
            }
            else
            {
                if (timer1.Enabled)
                {
                    timer1.Stop();
                    button1.Text = "Продолжить";
                }
                else
                {
                    timer1.Start();
                    button1.Text = "Пауза";
                }
            }
        }

        /// <summary>
        /// Основной таймер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            bool stop = false;

            for (int i = 0; i < 1000; i++)
            {
                currentYarn.Next(beaters);

                if (currentYarn.MaxF > parameters.clampForce)
                {
                    stop = true;
                    break;
                }

                foreach (Beater b in beaters)
                {
                    b.Go(parameters.dt);
                    if (b.CirclePassed)
                    {
                        stop = true;
                        break;
                    }
                }
            }


            if (checkBox2.Checked)
            {
                chart1.Series[0].Points.Add(currentYarn.SrF);

                if (chart1.Series[0].Points.Count > 200)
                {
                    chart1.Series[0].Points.RemoveAt(0);
                }
            }

            if(checkBox1.Checked)
                animation1.Draw(currentYarn.Points, beaters);

            if (stop)
            {
                timer1.Stop();
                NextYarn();
            }

            if (currentYarn.Error)
            {
                timer1.Stop();
                NextYarn();
            }
        }

        /// <summary>
        /// Генерация нормально распределенного числа
        /// </summary>
        /// <param name="expectedValue">Математическое ожидание</param>
        /// <param name="variance">Дисперсия</param>
        /// <returns>Возвращает нормально распределенное число</returns>
        private double genNormNumber(double expectedValue, double variance)
        {
            double number = 0;

            for (int i = 0; i < 12; i++)
            {
                double add = rnd.NextDouble();
                number += add;
            }

            number -=6;

            double x = expectedValue + Math.Pow(variance, 0.5) * number;

            if (x < expectedValue - variance) { x = expectedValue - variance; }
            if (x > expectedValue + variance) { x = expectedValue + variance; }
            
            return x;
        }

        /// <summary>
        /// Запись результатов в Excel
        /// </summary>
        private void WriteRezult()
        {
            application = new Excel.Application();
            application.Visible = true;
            application.SheetsInNewWorkbook = 3;
            application.Workbooks.Add();
            Excel.Workbook book;
            Excel.Worksheet sheet;

            book = application.ActiveWorkbook;

            sheet = book.Sheets[1];
            sheet.Name = "Результаты";

            sheet.Cells[1, 1] = "Номер нити";
            sheet.Cells[1, 2] = "Результат";
            sheet.Cells[1, 3] = "Макс.Натяжение";

            for (int i = 0; i < rezultArray.GetLength(0); i++)
            {
                sheet.Cells[i + 3, 1] = i + 1;
                sheet.Cells[i + 3, 2] = rezultArray[i, 1];
                sheet.Cells[i + 3, 3] = rezultArray[i, 2];
            }


            sheet = book.Sheets[2];
            sheet.Name = "Распределение параметров";
            int[] intervals;
            double min;
            double n;

            #region Запись длин

            if (parameters.varianceLength == 0)
            {
                intervals = new int[(int)(Math.Round(parameters.varianceLength * 100 * 2))];

                min = (int)(parameters.expectedValueLength * 100 - parameters.varianceLength * 100);

                for (int i = 0; i < lengthArray.Length; i++)
                {
                    n = lengthArray[i] * 100;

                    if (n >= parameters.expectedValueLength * 100 + parameters.varianceLength * 100)
                    { n--; }

                    n -= min;

                    intervals[(int)n]++;
                }

                for (int i = 0; i < intervals.Length; i++)
                {
                    sheet.Cells[i + 2, 1] = "[" + (min + i) + ";" + (min + i + 1) + "]";
                    sheet.Cells[i + 2, 2] = intervals[i];
                }
            }
            else
            {
                sheet.Cells[2, 1] = "Длина постоянна = ";
                sheet.Cells[2, 2] = parameters.expectedValueLength;
            }

            #endregion

            #region Запись смещений

            if (parameters.varianceOffset == 0)
            {
                intervals = new int[(int)(Math.Round(parameters.varianceOffset * 100 * 2))];

                min = (int)(parameters.expectedValueOffset * 100 - parameters.varianceOffset * 100);

                for (int i = 0; i < offsetArray.Length; i++)
                {
                    n = offsetArray[i] * 100;

                    if (n >= parameters.expectedValueOffset * 100 + parameters.varianceOffset * 100)
                    { n--; }

                    n -= min;

                    intervals[(int)n]++;
                }

                for (int i = 0; i < intervals.Length; i++)
                {
                    sheet.Cells[i + 2, 4] = "[" + (min + i) + ";" + (min + i + 1) + "]";
                    sheet.Cells[i + 2, 5] = intervals[i];
                }
            }
            else
            {
                sheet.Cells[2, 4] = "Смещение постоянно = ";
                sheet.Cells[2, 6] = parameters.expectedValueOffset;
            }

            #endregion

            #region Запись углов

            if (parameters.varianceAngle == 0)
            {
                intervals = new int[(int)(Math.Round(parameters.varianceAngle * 2))];

                min = (int)(parameters.expectedValueAngle - parameters.varianceAngle);

                for (int i = 0; i < angleArray.Length; i++)
                {
                    n = angleArray[i];

                    if (n >= parameters.expectedValueAngle + parameters.varianceAngle)
                    { n--; }

                    n -= min;

                    intervals[(int)n]++;
                }

                for (int i = 0; i < intervals.Length; i++)
                {
                    sheet.Cells[i + 2, 7] = "[" + (min + i) + ";" + (min + i + 1) + "]";
                    sheet.Cells[i + 2, 8] = intervals[i];
                }
            }
            else
            {
                sheet.Cells[2, 7] = "Угол дезориентации постоянен = ";
                sheet.Cells[2, 8] = parameters.expectedValueAngle;
            }

            #endregion

            sheet.Columns.EntireColumn.AutoFit();
        }

        private void параметрыМоделиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parameters pr = new Parameters();
            pr.Show();
        }
 
        /// <summary>
        /// Таймер на время работы модели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            Time++;
            label3.Text = Time.ToString();
        }

        public ModelParameters MParameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

    }
}
