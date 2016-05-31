using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace App
{
    class Yarn
    {
        double length;
        double hiddenLength;

        double position;
        double youngModul;
        double dT;
        double step;
        double y_end;

        int pointCount;

        double maxF; ///Максимальная сила натяжения
        double minF;

        double srF; ///Среднее значение силы натяжения
        int fCount;

        bool error;

        double f1; ///Коэффициент сопротивления среды
        double K; ///Коэффициент трения

        dPoint[] prev; ///Массив точек в предыдущий момент времени
        dPoint[] now; ///Массив точек в текущий момент времени

        //double[] weightRasp; ///Коэффициенты распределения массы
                             
        //double kf; ///Коэффициент массы

        double c; ///Коэффициент изгибной жесткости

        double[] pointWeight;
        double[] pointCrossSection;

        double bendRadius;

        //double threadHiddenLength; ///Длина нити в зажиме

        public Yarn(double?[] _weigthRasp, double _length, double? _topDiameter, double? _midDiameter, 
            double? _botDiameter, double _youngModul,double _dt, int _pointCount, double _position, double _angle,
            double _clampLength,double _beltDistance, double _offset, double _frict, double _hard, double? _plotn,
            double[] _weightLength, double _windage, Beater[] beaters)
        {
            //this.weightRasp = _weigthRasp;
            this.length = _length;
            //this.topDiameter = _topDiameter;
            //this.midDiameter = _midDiameter;
            //this.botDiameter = _botDiameter;
            this.youngModul = _youngModul;

            //this.cross_section = Math.PI*Math.Pow(_topDiameter,2)/4;

            this.dT = _dt;
            this.step = 0;
            this.pointCount = _pointCount;
            this.position = _position;

            this.prev = new dPoint[pointCount];
            this.now = new dPoint[pointCount];

            this.srF = 0;
            this.fCount = 0;

            this.K = _windage;
            this.f1 = _frict;

            c = _hard;
            
            this.maxF = 0;
            this.minF = 0;

            this.error = false;
            
            ///Определение координаты положения нити в зажиме

            double cos = Math.Cos(_angle * Math.PI / 180);
            double dl = (_length * cos - _beltDistance - 2 * _clampLength) / (2* cos);
            this.y_end = _clampLength + dl - _offset; ///Координата конца нити в зажиме
            this.length = _length - y_end; ///Длина свисающей части
            this.hiddenLength = _length - this.length; ///Длина части в зажиме

            

            #region Смещение
            
            double l = beaters[beaters.Length - 1].Center.X - _position;
            double h = beaters[beaters.Length - 1].Center.Y;
            double r = beaters[beaters.Length - 1].Radius;
            double d = 0.01;

            this.bendRadius = (h * h + Math.Pow((l - r - d), 2)) / (2 * (r + d - l));

            double alpha = length/bendRadius;
            double delta_alpha = alpha / (_pointCount-1);

            double center_x = beaters[beaters.Length-1].Center.X-r-d +bendRadius;
            double center_y = beaters[beaters.Length-1].Center.Y;

            double tgB = (center_x - _position) / center_y;
            double B = Math.Atan(tgB);


            for (int i = 0; i < pointCount; i++)
            {
                double x = center_x - bendRadius * Math.Sin(B + delta_alpha * i);
                double y = center_y - bendRadius * Math.Cos(B + delta_alpha * i); ;

                prev[i] = new dPoint(x, y);
                now[i] = new dPoint(x, y);
            }
            
            #endregion


            ///Задание масс и поперечных сечений
            ///


            PointsParameters(_topDiameter,_midDiameter,_botDiameter,_plotn,_weigthRasp, _weightLength);


        }

        public void Next(Beater[] beaters)
        {



            ///Считать pointWeight один раз!!




            step++;

            dPoint[] next = new dPoint[pointCount];

            #region Проверка на отражение

            for (int i = 0; i < pointCount; i++)
            {
                for (int j = 0; j < beaters.Length; j++)
                {
                    if (i > beaters[j].LastPoint && beaters[j].LastPoint != -1)
                    {
                        double distance = Math.Pow((Math.Pow((now[i].X - beaters[j].Center.X), 2) +
                                Math.Pow((now[i].Y - beaters[j].Center.Y), 2)), 0.5);

                        if (distance + (length / (pointCount - 1)) / 500 < beaters[j].Radius)
                        {

                            double k = (beaters[j].Center.Y - beaters[j].Edge.Y) /
                                (beaters[j].Center.X - beaters[j].Edge.X);

                            double b = -k * beaters[j].Edge.X + beaters[j].Edge.Y;

                            double f = k * now[i].X + b - now[i].Y;

                            //Проверка на пересечение
                            if (f >= 0)
                            {
                                double c = -1 / k;
                                double d = now[i].Y - c * now[i].X;

                                double x1 = (d - b) / (k - c);
                                double y1 = k * x1 + b;

                                double x_new = 0;

                                x_new = 2 * x1 - now[i].X;
                                
                                double y_new = 2 * y1 - now[i].Y;

                                now[i] = new dPoint(x_new, y_new);

                                c = -1 / k;
                                d = prev[i].Y - c * prev[i].X;

                                x1 = (d - b) / (k - c);
                                y1 = k * x1 + b;


                                if (k < 0)
                                {
                                    x_new = x1 + Math.Pow(10, -6) * Math.Pow((1 / (1 + k * k)), 0.5);
                                }
                                else
                                {
                                    x_new = x1 - Math.Pow(10, -6) * Math.Pow((1 / (1 + k * k)), 0.5);
                                }

                                y_new = y1 + Math.Pow(10, -6) * Math.Pow((1 / (1 + 1 / (k * k))), 0.5);

                                break;
                            }
                        }
                        else if (i == pointCount - 1)
                        {
                            beaters[j].LastPoint = -1;
                        }
                    }
                }
            }

            #endregion


            for (int i = 0; i < beaters.Length; i++)
                beaters[i].HasTouch = false;

            for (int i = 0; i < pointCount; i++)
            {
                bool kick = false;

                double x = 0;
                double y = 0;

                double Xb = 0;/// Координаты края била
                double Yb = 0;
                double Xc = 0;/// Координаты центра била
                double Yc = 0;

                double l_prev = 0; ///l(i,i-i)
                double l_next = 0; ///l(i,i+1)

                double l0 = length / (pointCount - 1);

                if (i > 0)
                {
                    l_prev = Math.Pow(Math.Pow((now[i - 1].X - now[i].X), 2) +
                        Math.Pow((now[i - 1].Y - now[i].Y), 2), 0.5);
                }
                if (i < pointCount - 1)
                {
                    l_next = Math.Pow(Math.Pow((now[i + 1].X - now[i].X), 2) +
                        Math.Pow((now[i + 1].Y - now[i].Y), 2), 0.5);

                    if (l_next > l0 * 2)
                    {
                        error = true;
                        break;
                    }
                }

                #region Проверка на пересечение

                if (i < pointCount - 1)
                {
                    for (int j = 0; j < beaters.Length; j++)
                    {
                        if (Transection(now[i], now[i + 1], beaters[j].Edge, beaters[j].Center))
                        {
                            Xb = beaters[j].Edge.X;
                            Yb = beaters[j].Edge.Y;
                            Xc = beaters[j].Center.X;
                            Yc = beaters[j].Center.Y;

                            beaters[j].LastPoint = i;

                            beaters[j].HasTouch = true;

                            kick = true;

                            break;
                        }
                    }
                }
#endregion

                #region Вычисление новых точек

                if (kick && i < pointCount - 1 && i > 0)
                {
                    /// Точка выше била

                    double l_str = Math.Pow((Math.Pow((Xb - now[i].X), 2) + Math.Pow((Yb - now[i].Y), 2)), 0.5) +
                        Math.Pow((Math.Pow((Xb - now[i + 1].X), 2) + Math.Pow((Yb - now[i + 1].Y), 2)), 0.5);

                    double cos = (Xb - now[i].X) / Math.Pow((Math.Pow((Xb - now[i].X), 2) +
                        Math.Pow((Yb - now[i].Y), 2)), 0.5);

                    double sin = (Yb - now[i].Y) / Math.Pow((Math.Pow((Xb - now[i].X), 2) +
                        Math.Pow((Yb - now[i].Y), 2)), 0.5);

                    double angle = Math.PI - Math.Atan((Yb - now[i].Y) / (now[i].X - Xb)) -
                        Math.Atan((now[i + 1].Y - Yb) / (now[i + 1].X - Xb));



                    ///Изгибная жесткость
                   
                    double alpha = Math.Atan((Xb - now[i].X) / (Yb - now[i].Y));
                    double alpha_prev = Math.Atan((now[i].X - now[i - 1].X) / (now[i].Y - now[i - 1].Y));

                    alpha -= alpha_prev;

                    double M1 = c * alpha_prev;
                    double M2 = c * alpha;

                    double F1, F2;

                    F1 = M1 / l_prev;
                    F2 = M2 / Math.Pow(Math.Pow((Xb - now[i].X), 2) / Math.Pow((Yb - now[i].Y), 2), 0.5);

                    //F1 = 0;
                    //F2 = 0;

                    x = 2 * now[i].X - prev[i].X + (Math.Pow(dT, 2) / pointWeight[i]) *
                        ((youngModul * pointCrossSection[i] * (l_prev / l0 - 1) * (now[i - 1].X - now[i].X) / l_prev) +
                        (youngModul * pointCrossSection[i] * (l_str / l0 - 1) * cos) * Math.Pow(Math.E, f1 * angle)
                       - K * ((now[i].X - prev[i].X) / dT))
                       + F2 * Math.Cos(alpha) - F1 * Math.Cos(alpha_prev);

                    y = 2 * now[i].Y - prev[i].Y + (Math.Pow(dT, 2) / pointWeight[i]) *
                        ((youngModul * pointCrossSection[i] * (l_prev / l0 - 1) * (now[i - 1].Y - now[i].Y) / l_prev) +
                        (youngModul * pointCrossSection[i] * (l_str / l0 - 1) * sin) * Math.Pow(Math.E, f1 * angle)
                        - K * ((now[i].Y - prev[i].Y) / dT))
                        + F2 * Math.Sin(alpha) - F1 * Math.Sin(alpha_prev);


                    next[i] = new dPoint(x, y);

                    /// Точка ниже била

                    cos = (Xb - now[i + 1].X) / Math.Pow((Math.Pow((Xb - now[i + 1].X), 2) +
                        Math.Pow((Yb - now[i + 1].Y), 2)), 0.5);

                    sin = (Yb - now[i + 1].Y) / Math.Pow((Math.Pow((Xb - now[i + 1].X), 2) +
                        Math.Pow((Yb - now[i + 1].Y), 2)), 0.5);


                    ///Изгибная жесткость
                   
                    alpha = Math.Atan((Xb - now[i].X) / (Yb - now[i].Y));
                    alpha_prev = Math.Atan((now[i].X - now[i - 1].X) / (now[i].Y - now[i - 1].Y));

                    alpha -= alpha_prev;

                    M1 = c * alpha_prev;
                    M2 = c * alpha;

                    F1 = M1 / Math.Pow(Math.Pow((Xb - now[i].X), 2) / Math.Pow((Yb - now[i].Y), 2), 0.5);
                    F2 = M2 / l_next;


                    //F1 = 0;
                    //F2 = 0;


                    x = 2 * now[i + 1].X - prev[i + 1].X + (Math.Pow(dT, 2) / pointWeight[i]) *
                        ((youngModul * pointCrossSection[i] * (l_str / l0 - 1) * cos) - K * ((now[i + 1].X - prev[i + 1].X) / dT))
                        +F2 * Math.Cos(alpha) - F1 * Math.Cos(alpha_prev);

                    y = 2 * now[i + 1].Y - prev[i + 1].Y + (Math.Pow(dT, 2) / pointWeight[i]) *
                        ((youngModul * pointCrossSection[i] * (l_str / l0 - 1) * sin) - K * ((now[i + 1].Y - prev[i + 1].Y) / dT))
                        +F2 * Math.Sin(alpha) - F1 * Math.Sin(alpha_prev);


                    if (i < pointCount - 3)
                    {
                        double l_next2 = Math.Pow((Math.Pow((now[i + 2].X - now[i + 1].X), 2) +
                            Math.Pow((now[i + 2].Y - now[i + 1].Y), 2)), 0.5);

                        x += (Math.Pow(dT, 2) / pointWeight[i]) * (youngModul * pointCrossSection[i] * (l_next2 / l0 - 1) *
                            (now[i + 2].X - now[i + 1].X) / l_next2);

                        y += (Math.Pow(dT, 2) / pointWeight[i]) * (youngModul * pointCrossSection[i] * (l_next2 / l0 - 1) *
                            (now[i + 2].Y - now[i + 1].Y) / l_next2);
                    }

                    next[i + 1] = new dPoint(x, y);

                    i++;
                }

                else
                {
                    if (i == 0)     /// Первая точка
                    {
                        x = position;
                        y = 0;
                    }
                    else if (i == pointCount - 1)    /// Последняя точка
                    {
                        ///Изгибная жесткость
                   
                        double alpha = Math.Atan((now[i].X - now[i - 1].X) / (now[i].Y - now[i - 1].Y));
                        double alpha_prev = Math.Atan((now[i - 1].X - now[i - 2].X) / (now[i - 1].Y - now[i - 2].Y));

                        alpha -= alpha_prev;

                        double M = c * alpha;

                        double F1;

                        F1 = M / l_prev;

                        //F1 = 0;

                        //double F1 = 0,  alpha_prev = 0;

                        x = 2 * now[i].X - prev[i].X + (Math.Pow(dT, 2) / pointWeight[i]) *
                           ((youngModul * pointCrossSection[i] * (l_prev - l0) * (now[i - 1].X - now[i].X) / (l0 * l_prev))
                           - K * ((now[i].X - prev[i].X) / dT))
                           - F1 * Math.Cos(alpha_prev);

                        y = 2 * now[i].Y - prev[i].Y + (Math.Pow(dT, 2) / pointWeight[i]) *
                            ((youngModul * pointCrossSection[i] * (l_prev - l0) * (now[i - 1].Y - now[i].Y) / (l0 * l_prev))
                           - K * ((now[i].Y - prev[i].Y) / dT))
                           - F1 * Math.Sin(alpha_prev);

                    }
                    else    /// Остальные точки
                    {
                        ///Изгибная жесткость
                   
                        double alpha = Math.Atan((now[i + 1].X - now[i].X) / (now[i + 1].Y - now[i].Y));
                        double alpha_prev = Math.Atan((now[i].X - now[i - 1].X) / (now[i].Y - now[i - 1].Y));

                        alpha -= alpha_prev;

                        double M1 = c * alpha_prev;
                        double M2 = c * alpha;

                        double F1, F2;

                        F1 = M1 / l_next;
                        F2 = M2 / l_prev;


                        //F1 = 0;
                        //F2 = 0;

                        //double F1 = 0, F2 = 0, alpha = 0, alpha_prev = 0;

                        x = 2 * now[i].X - prev[i].X + (Math.Pow(dT, 2) / pointWeight[i]) *
                            ((youngModul * pointCrossSection[i] * (l_prev - l0) * (now[i - 1].X - now[i].X) / (l0 * l_prev)) +
                            (youngModul * pointCrossSection[i] * (l_next - l0) * (now[i + 1].X - now[i].X) / (l0 * l_next))
                            - K * ((now[i].X - prev[i].X) / dT))
                            + F2 * Math.Cos(alpha) - F1 * Math.Cos(alpha_prev);

                        y = 2 * now[i].Y - prev[i].Y + (Math.Pow(dT, 2) / pointWeight[i]) *
                            ((youngModul * pointCrossSection[i] * (l_prev - l0) * (now[i - 1].Y - now[i].Y) / (l0 * l_prev)) +
                            (youngModul * pointCrossSection[i] * (l_next - l0) * (now[i + 1].Y - now[i].Y) / (l0 * l_next))
                            - K * ((now[i].Y - prev[i].Y) / dT))
                            + F2 * Math.Sin(alpha) - F1 * Math.Sin(alpha_prev);


                    }
                    next[i] = new dPoint(x, y);
                }

                #endregion
            }


            if (!error)
            {
                /// Перезапись массивов
                for (int i = 0; i < pointCount; i++)
                {
                    prev[i] = new dPoint(now[i].X, now[i].Y);
                    now[i] = new dPoint(next[i].X, next[i].Y);
                }

                #region Расчет силы натяжения

                /// Точки между которыми ведется расчет натяжения
                int pt1 = 0;
                int pt2 = 1;

                double l1 = 0;
                bool kick1 = false;


                double C = youngModul * pointCrossSection[pt2] / (length / pointCount);



                if (!kick1)
                {
                    l1 = Math.Pow((Math.Pow((now[pt2].X - now[pt1].X), 2) +
                        Math.Pow((now[pt2].Y - now[pt1].Y), 2)), 0.5);
                }

                double l2 = length / (pointCount - 1);

                double F = C * (l1 - l2);

                srF += F;
                fCount++;

                if (srF/fCount > maxF) { maxF = srF/fCount; }

                #endregion

            }
        }

        /// Проверка на пересечение
        
        bool Transection(dPoint t1, dPoint t2, dPoint b1, dPoint b2) 
        {
            double v1 = (b2.X - b1.X) * (t1.Y - b1.Y) - (b2.Y - b1.Y) * (t1.X - b1.X);
            double v2 = (b2.X - b1.X) * (t2.Y - b1.Y) - (b2.Y - b1.Y) * (t2.X - b1.X);
            double v3 = (t2.X - t1.X) * (b1.Y - t1.Y) - (t2.Y - t1.Y) * (b1.X - t1.X);
            double v4 = (t2.X - t1.X) * (b2.Y - t1.Y) - (t2.Y - t1.Y) * (b2.X - t1.X);
            return ((v1 * v2 < 0) && (v3 * v4 < 0));
        }

        public dPoint[] Points
        {
            get
            {
                return now;
            }
        }

        ///Вычисление массы точки

        private void PointsParameters(double? topDiameter,double? midDiameter,double? botDiameter,double? plotn,
            double?[] weightRasp,double[] weightLength)
        {
            
            pointWeight = new double[pointCount];
            pointCrossSection = new double[pointCount];

            if (topDiameter != null && midDiameter != null && botDiameter != null)
            {

            }
            else if(topDiameter!=null && midDiameter==null && botDiameter==null && plotn==null)
            {

            }
            else if (weightLength != null)
            {

            }
            else
            {
                MessageBox.Show("Критическая ошибка, неверно заданы массы нити", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }





            ///Масса нити реальная и эталонная
            double f1 = 7 * Math.Pow(10, -5) * Math.Pow(length * 100, 2) - 0.005 * length * 100 + 0.146;
            double f2 = 7 * Math.Pow(10, -5) * Math.Pow(75, 2) - 0.005 * 75 + 0.146;

            ///Коэффициент отношения масс

            double kf = (f1 / f2);



            //if (weight != null)
            //{
            //    return (double)weight / pointCount;
            //}
            //else
            //{
            //    double dl = length / pointCount;

            //    double a = (weightRasp[0] / 150 / 10) * ((pointNumber * dl) - ((pointNumber - 1) * dl)) +
            //        1 / 2 * (weightRasp[1] / 150 / 10) * (Math.Pow((pointNumber * dl), 2) - Math.Pow(((pointNumber - 1) * dl), 2)) +
            //        1 / 3 * (weightRasp[2] / 150 / 10) * (Math.Pow((pointNumber * dl), 3) - Math.Pow(((pointNumber - 1) * dl), 3)) +
            //        1 / 4 * (weightRasp[3] / 150 / 10) * (Math.Pow((pointNumber * dl), 4) - Math.Pow(((pointNumber - 1) * dl), 4));

            //    a *= kf;

            //    return a;
            //}



        }

        public double MaxF
        { get { return maxF; } }
        public double MinF
        { get { return minF; } }
        public double SrF
        { get { return srF / fCount; } }
        public bool Error
        {
            get { return error; }
        }
        public double Length
        {
            get { return length; }
        }
        public double BendRadius
        {
            get { return bendRadius; }
        }
        
        //public double Weight
        //{
        //    get 
        //    {
        //        if (weight != null)
        //        {
        //            return (double)weight;
        //        }
        //        else
        //        {

        //        }

        //        return; 
        //    }
        //}
       
    }
}
