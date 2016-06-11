using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace App
{
    public class Beater
    {
        double speed; ///Скорость вращения
        double length; ///Длина планки
        double angle; ///Угол поворота
        double startAngle; ///Начальный угол

        int direction; ///Направление движения
                       ///-1 - влево, 1 - вправо

        int lastPoint; ///Последняя точка нити, контактирующая с бильной планкой
        bool hasTouch; ///Имеется ли контакт с нитью
        bool circlePassed; ///Пройден ли оборот

        dPoint center; ///Центр бильного барабана

        public Beater(double _speed, double _yPosition, double _length, double _xPosition, double _angle, int _direction)
        {
            this.speed = _speed;
            this.length = _length;
            this.angle = _angle * (Math.PI / 180);
            this.startAngle = this.angle;
            this.center = new dPoint(_xPosition, _yPosition);
            this.lastPoint = -1;
            this.direction = _direction;
            this.hasTouch = false;
        }

        /// <summary>
        /// Движение бильной планки по окружности
        /// </summary>
        /// <param name="dT">Промежуток времени</param>
        public void Go(double dT)
        {
            this.angle += -1 * direction * speed * (Math.PI / 30) * dT;
            
            if (angle > startAngle + Math.PI)
                circlePassed = true;
        }

        /// <summary>
        /// Точка на конце бильной планки
        /// </summary>
        public dPoint Edge
        {
            get 
            {
                dPoint _p = new dPoint((center.X - length * Math.Sin(angle)),
                        (center.Y - length * Math.Cos(angle)));
               
                return _p;
            }
        }
       
        public dPoint Center
        {
            get { return center; }
        }
        public double Angle
        {
            get { return angle; }
        }
        public double Radius
        {
            get { return length; }
        }
        public int LastPoint
        {
            get { return lastPoint; }
            set { this.lastPoint = value; }
        }
        public bool HasTouch
        {
            get { return hasTouch; }
            set { hasTouch = value; }
        }
        public bool CirclePassed
        {
            get { return circlePassed; }
            set { circlePassed = value; }
        }
    }
}
