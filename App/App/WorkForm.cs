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

        double[,] rezultArray; ///Массив с результатами

        DateTime Time; ///Время работы модели

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
           
            Time = new DateTime(2016, 1, 1, 0, 0, 0);

            rezultArray = new double[parameters.yarnCount, 3];

            InitializeComponent();

            animation1.ClampParameters(parameters.clampType, parameters.threadPosition);

            dataGridView1.Columns.Add("number", "№");
            dataGridView1.Columns.Add("status", "Статус");
            dataGridView1.Columns.Add("length", "Длина");
            dataGridView1.Columns.Add("weight", "Масса");
            dataGridView1.Columns.Add("force", "F max");


            for (int i = 0; i < rezultArray.GetLength(0); i++)
            {
                int r = rnd.Next(0,4);
                
                if (r == 1)
                    rezultArray[i, 0] = 0;
                if (r == 2)
                    rezultArray[i, 0] = 1;
                if (r == 3)
                    rezultArray[i, 0] = 2;
            }
            
            WriteRezult();
        }

        /// <summary>
        /// Запуск следующей нити
        /// </summary>
        private void NextYarn()
        {
            if (currentNumberYarn != 0)
            {
                if (currentYarn.Error)
                {
                    //MessageBox.Show("Ошибка в движении нити, продолжить работу?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    rezultArray[currentNumberYarn - 1, 0] = 2;

                    dataGridView1.Rows.Add(currentNumberYarn, "Ошибка",currentYarn.Length,currentYarn.Weight, "-");
                    dataGridView1.Rows[currentNumberYarn - 1].Cells[0].Style.BackColor = Color.Red;
                }
                else
                {
                    if (currentYarn.MaxF < currentYarn.ClampF)
                    {
                        //MessageBox.Show("Прядь сохранена. Максимальная сила натяжения - " + currentYarn.MaxF + " H.");
                        rezultArray[currentNumberYarn - 1, 0] = 1;

                        dataGridView1.Rows.Add(currentNumberYarn, "Пройдена", currentYarn.Length, currentYarn.Weight, currentYarn.MaxF.ToString("f5"));
                        dataGridView1.Rows[currentNumberYarn - 1].Cells[0].Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        //MessageBox.Show("Прядь вылетела. Максимальная сила натяжения - " + currentYarn.MaxF + " H.");
                        rezultArray[currentNumberYarn - 1, 0] = 0;

                        dataGridView1.Rows.Add(currentNumberYarn, "Вылетела", currentYarn.Length, currentYarn.Weight, currentYarn.MaxF.ToString("f5"));
                        dataGridView1.Rows[currentNumberYarn - 1].Cells[0].Style.BackColor = Color.LightGray;
                    }
                }


                rezultArray[currentNumberYarn - 1, 1] = currentYarn.MaxF;
                rezultArray[currentNumberYarn - 1, 2] = currentYarn.Weight;
            }

            if (currentNumberYarn == parameters.yarnCount)
            {
                timer2.Stop();

                //MessageBox.Show("Процесс закончен. Затрачено времени - " + label3.Text.ToString());

                WriteRezult();

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
                    parameters.threadYoungModul, parameters.dt, parameters.threadPointCount,
                    parameters.threadPosition, angleArray[(int)currentNumberYarn], parameters.clampLength,
                    parameters.beltDistance, parameters.clampType,parameters.clampForce, offsetArray[(int)currentNumberYarn], 
                    parameters.threadFriction,parameters.threadHard, parameters.threadDensity,
                    parameters.weightLength, parameters.windage, beaters);

                label11.Text = lengthArray[currentNumberYarn].ToString("f5") + " м";
                label12.Text = currentYarn.ClampY.ToString("f5") + " м";
                label10.Text = (currentNumberYarn + 1).ToString();
                label1.Text = (currentYarn.Weight * 1000).ToString("f5") + " г";

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

                if (currentYarn.MaxF > currentYarn.ClampF)
                {
                    stop = true;
                    break;
                }

                if (currentYarn.Error)
                {
                    stop = true;
                    break;
                }
                else
                {
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
            }

            label6.Text = currentYarn.MaxF.ToString("f5") + " Н";

            if (checkBox2.Checked)
            {
                chart1.Series[0].Points.Add(currentYarn.SrF);

                if (chart1.Series[0].Points.Count > 200)
                {
                    chart1.Series[0].Points.RemoveAt(0);
                }
            }

            if (checkBox1.Checked)
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

            number -= 6;

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
            application.Visible = false;
            application.SheetsInNewWorkbook = 4;
            application.Workbooks.Add();
            Excel.Workbook book;
            Excel.Worksheet sheet;

            book = application.ActiveWorkbook;

            //sheet = book.Sheets[1];
            //sheet.Name = "Результаты";

            //sheet.Cells[1, 1] = "Результат";
            //sheet.Cells[1, 2] = "Сила натяжения";
            //sheet.Cells[1, 3] = "Масса";

            //for (int i = 0; i < rezultArray.GetLength(0); i++)
            //{
            //    sheet.Cells[i + 3, 1] = rezultArray[i,0];
            //    sheet.Cells[i + 3, 2] = rezultArray[i, 1];
            //    sheet.Cells[i + 3, 3] = rezultArray[i, 2];
            //}

            sheet = book.Sheets[1];
            sheet.Name = "Результат";
            int[,] intervals;
            double min;
            double n;
            int count;


            #region Запись результатов

            if (parameters.varianceLength != 0)
            {
                intervals = new int[(int)(Math.Round(parameters.varianceLength * 100 * 2)), 3];
                ///0 - всего стеблей в интервале
                ///1 - количество выпавших стеблей
                ///2 - количество обработанных стеблей             

                min = (int)(parameters.expectedValueLength * 100 - parameters.varianceLength * 100);

                count = 0;

                for (int i = 0; i < lengthArray.Length; i++)
                {
                    n = lengthArray[i] * 100;

                    if (n >= parameters.expectedValueLength * 100 + parameters.varianceLength * 100)
                    { n--; }

                    n -= min;

                    if (rezultArray[i, 0] != 2) //Если ошибка, не учитываем этот стебель
                    {
                        intervals[(int)n, 0]++;
                        count++;
                    }

                    if (rezultArray[i, 0] == 0)
                        intervals[(int)n, 1]++;
                    else if (rezultArray[i, 0] == 1)
                        intervals[(int)n, 2]++;
                }

                sheet.Cells[1, 1] = "Длина волокна, см";
                sheet.Cells[1, 2] = "Количество стеблей";
                sheet.Cells[1, 3] = "Доля стеблей";
                sheet.Cells[1, 4] = "Количество выпавших стеблей";
                sheet.Cells[1, 5] = "Доля массы выпавших стеблей";
                sheet.Cells[1, 6] = "Количество обработанных стеблей";
                sheet.Cells[1, 7] = "Доля массы обработанных стеблей";


                for (int i = 0; i < intervals.GetLength(0); i++)
                {
                    string interv = "[" + (min + i) + ";" + (min + i + 1);

                    if (i < intervals.GetLength(0) - 1)
                        interv += ")";
                    else
                        interv += "]";

                    sheet.Cells[i + 2, 1] = interv;
                    sheet.Cells[i + 2, 2] = intervals[i, 0];
                    sheet.Cells[i + 2, 3] = (double)intervals[i, 0] / (double)count;
                    sheet.Cells[i + 2, 4] = intervals[i, 1];
                    sheet.Cells[i + 2, 5] = (double)intervals[i, 1] / (double)count;
                    sheet.Cells[i + 2, 6] = intervals[i, 2];
                    sheet.Cells[i + 2, 7] = (double)intervals[i, 2] / (double)count;
                }

                book.Sheets[1].Activate();
                Excel.ChartObjects chartsobjrcts = (Excel.ChartObjects)sheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartsobjrct = chartsobjrcts.Add(400, 20, 400, 300);
                chartsobjrct.Chart.ChartWizard(sheet.get_Range("G1", "G" + intervals.GetLength(0).ToString()),
                    Excel.XlChartType.xlColumnStacked, 2, Excel.XlRowCol.xlColumns, Type.Missing,
                    0, true, "Выход волокна", "Длина стеблей, см", "Доля выхода волокна", Type.Missing);

                Excel.Range range;
                range= sheet.Range[sheet.Cells[1, 1], sheet.Cells[intervals.GetLength(0)+1,7]];
                range.ColumnWidth = 12;
                range.WrapText = true;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            }
            else
            {
                sheet.Cells[2, 1] = "Длина постоянна = ";
                sheet.Cells[2, 2] = parameters.expectedValueLength;
                sheet.Cells[2, 4] = "Влияние длины не учитывается";
            }

            

            #endregion


            sheet = book.Sheets[2];
            sheet.Name = "Длины";

            #region Распределение длин

            if (parameters.varianceLength != 0)
            {
                intervals = new int[(int)(Math.Round(parameters.varianceLength * 100 * 2)), 1];
      
                min = (int)(parameters.expectedValueLength * 100 - parameters.varianceLength * 100);

                for (int i = 0; i < lengthArray.Length; i++)
                {
                    n = lengthArray[i] * 100;

                    if (n >= parameters.expectedValueLength * 100 + parameters.varianceLength * 100)
                    { n--; }

                    n -= min;

                    if (rezultArray[i, 0] != 2) //Если ошибка, не учитываем этот стебель
                    {
                        intervals[(int)n, 0]++;
                    }
                }

                sheet.Cells[1, 1] = "Длина волокна, см";
                sheet.Cells[1, 2] = "Количество стеблей";

                for (int i = 0; i < intervals.GetLength(0); i++)
                {
                    string interv = "[" + (min + i) + ";" + (min + i + 1);

                    if (i < intervals.GetLength(0) - 1)
                        interv += ")";
                    else
                        interv += "]";

                    sheet.Cells[i + 2, 1] = interv;
                    sheet.Cells[i + 2, 2] = intervals[i, 0];
                }

                book.Sheets[2].Activate();
                Excel.ChartObjects chartsobjrcts = (Excel.ChartObjects)sheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartsobjrct = chartsobjrcts.Add(150, 20, 400, 300);

                chartsobjrct.Chart.ChartWizard(sheet.get_Range("A1", "B" + intervals.Length.ToString()),
                    Excel.XlChartType.xlLine, 2, Excel.XlRowCol.xlColumns, Type.Missing,
                    0, true, "Распределение длин", "Длина", "Количество", Type.Missing);

                Excel.Range range;
                range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[intervals.GetLength(0) + 1, 2]];
                range.ColumnWidth = 12;
                range.WrapText = true;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            }
            else
            {
                sheet.Cells[2, 1] = "Длина постоянна = ";
                sheet.Cells[2, 2] = parameters.expectedValueLength;
                sheet.Cells[4, 1] = "Влияние длины не учитывается";
            }

            #endregion


            sheet = book.Sheets[3];
            sheet.Name = "Смещения";

            #region Запись смещений

            if (parameters.varianceOffset != 0)
            {
                intervals = new int[(int)(Math.Round(parameters.varianceOffset * 100 * 2)),1];

                min = (int)(parameters.expectedValueOffset * 100 - parameters.varianceOffset * 100);

                count = 0;

                for (int i = 0; i < offsetArray.Length; i++)
                {
                    n = offsetArray[i] * 100;

                    if (n >= parameters.expectedValueOffset * 100 + parameters.varianceOffset * 100)
                    { n--; }

                    n -= min;

                    if (rezultArray[i, 0] != 2) //Если ошибка, не учитываем этот стебель
                    {
                        intervals[(int)n, 0]++;
                        count++;
                    }
                }

                sheet.Cells[1, 1] = "Смещение стебля, см";
                sheet.Cells[1, 2] = "Количество стеблей";

                for (int i = 0; i < intervals.GetLength(0); i++)
                {
                    string interv = "[" + (min + i) + ";" + (min + i + 1);

                    if (i < intervals.GetLength(0) - 1)
                        interv += ")";
                    else
                        interv += "]";

                    sheet.Cells[i + 2, 1] = interv;

                    sheet.Cells[i + 2, 2] = intervals[i,0];
                }

                book.Sheets[3].Activate();
                Excel.ChartObjects chartsobjrcts = (Excel.ChartObjects)sheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartsobjrct = chartsobjrcts.Add(150, 20, 400, 300);

                chartsobjrct.Chart.ChartWizard(sheet.get_Range("A1", "B" + intervals.GetLength(0).ToString()),
                    Excel.XlChartType.xlLine, 2, Excel.XlRowCol.xlColumns, Type.Missing,
                    0, true, "Распределение смещений", "Смещение", "Количество", Type.Missing);
                
                Excel.Range range;
                range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[intervals.GetLength(0) + 1, 2]];
                range.ColumnWidth = 12;
                range.WrapText = true;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            }
            else
            {
                sheet.Cells[2, 1] = "Смещение постоянно = ";
                sheet.Cells[2, 2] = parameters.expectedValueOffset;
                sheet.Cells[4, 1] = "Влияние смещения не учитывается";
            }

            #endregion


            sheet = book.Sheets[4];
            sheet.Name = "Дезориентация";

            #region Запись углов

            if (parameters.varianceAngle != 0)
            {
                intervals = new int[(int)(Math.Round(parameters.varianceAngle * 2)),1];

                min = (int)(parameters.expectedValueAngle - parameters.varianceAngle);

                for (int i = 0; i < angleArray.Length; i++)
                {
                    n = angleArray[i];

                    if (n >= parameters.expectedValueAngle + parameters.varianceAngle)
                    { n--; }

                    n -= min;

                    if(rezultArray[i,0]!=2)
                        intervals[(int)n,0]++;
                }

                sheet.Cells[1, 1] = "Дезориентация волокна, град";
                sheet.Cells[1, 2] = "Количество стеблей";

                for (int i = 0; i < intervals.GetLength(0); i++)
                {
                    string interv = "[" + (min + i) + ";" + (min + i + 1);

                    if (i < intervals.GetLength(0) - 1)
                        interv += ")";
                    else
                        interv += "]";

                    sheet.Cells[i + 2, 1] = interv;

                    sheet.Cells[i + 2, 2] = intervals[i,0];
                }

                book.Sheets[4].Activate();
                Excel.ChartObjects chartsobjrcts = (Excel.ChartObjects)sheet.ChartObjects(Type.Missing);
                Excel.ChartObject chartsobjrct = chartsobjrcts.Add(150, 20, 400, 300);

                chartsobjrct.Chart.ChartWizard(sheet.get_Range("A1", "B" + intervals.GetLength(0).ToString()),
                    Excel.XlChartType.xlLine, 2, Excel.XlRowCol.xlColumns, Type.Missing,
                    0, true, "Распределение углов дезориентации", "Дезориентация", "Количество", Type.Missing);

                Excel.Range range;
                range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[intervals.GetLength(0) + 1, 2]];
                range.ColumnWidth = 12;
                range.WrapText = true;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            }
            else
            {
                sheet.Cells[2, 1] = "Угол дезориентации постоянен = ";
                sheet.Cells[2, 2] = parameters.expectedValueAngle;
                sheet.Cells[4, 1] = "Влияние дезориентации не учитывается";
            }

            #endregion


            book.Sheets[1].Activate();

            application.Visible = true;
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
            Time = Time.AddSeconds(1);

            label3.Text = (Time.Day - 1).ToString() + ":" + Time.Hour.ToString() + ":"
                + Time.Minute.ToString() + ":" + Time.Second.ToString();
        }

        public ModelParameters MParameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

    }
}
