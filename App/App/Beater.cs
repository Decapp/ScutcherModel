using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace App
{
    public class Beater
    {
        double speed;
        double heigth;
        double length;
        double xPosition;
        double angle;
        double startAngle;

        int direction; ///-1 - влево, 1 - вправо

        int lastPoint;

        bool hasTouch;
        bool circlePassed;

        dPoint center;

        public Beater(double _speed, double _heigth, double _length, double _xPosition, double _angle, int _direction)
        {
            this.speed = _speed;
            this.heigth = _heigth;
            this.length = _length;
            this.xPosition = _xPosition;
            this.angle = _angle * (Math.PI / 180);
            this.startAngle = this.angle;

            this.center = new dPoint(xPosition, heigth);
            this.lastPoint = -1;
            this.direction = _direction;
            this.hasTouch = false;
        }

        public void Go(double dT)
        {
            this.angle += -1*direction*speed * (Math.PI / 30) * dT;
            if (angle > startAngle + 2*Math.PI)
            { circlePassed = true; }
        }

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
