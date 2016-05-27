using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public class dPoint
    {
        double x;
        double y;

        public dPoint(double x_new, double y_new)
        {
            this.x = x_new;
            this.y = y_new;
        }

        public double X
        { get { return x; } }
        public double Y
        { get { return y; } }
    }
}
