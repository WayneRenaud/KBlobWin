using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBlobWin
{
    public class PolarCoordinatesScreenSaver : IScreenSaver
    {
        private double Angle, Radius, Inc, Crot, Div;
        private double Scale;

        //Current X and Y location
        private int X, Y;

        //Screen information
        private int HalfWidth, HalfHeight;

        public void Init(int width, int height, int boxSize)
        {
            Angle = 0;
            Radius = 0;
            Inc = (2 * Math.PI) / 720;
            Crot = 0;
            Div = Rand.SmallRand(4) - 1;

            Scale = (double)height / 3.0 - 4.0 * (double)boxSize;
            HalfWidth = width / 2;
            HalfHeight = height / 2;
        }

        public Point NextFrame()
        {
            //Polar coordinates equation
            if (Div < 1)
                Radius = Math.Cos(2.0 * Angle);
            else
                Radius = 1 / Div + Math.Cos(2 * Angle);

            X = (int)(Scale * Radius * Math.Cos(Angle + Crot)) + HalfWidth;
            Y = (int)(Scale * Radius * Math.Sin(Angle + Crot)) + HalfHeight;

            //Movement for next time
            Angle += Inc;
            if (Angle > 2 * Math.PI)
            {
                Angle -= 2 * Math.PI;
                Crot += Math.PI / 45.0;
            }

            return new Point(X, Y);
        }
    }
}
