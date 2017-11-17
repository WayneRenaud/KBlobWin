using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBlobWin
{
    public class CircularBounceScreenSaver : IScreenSaver
    {
        private double Radians, RInc, SRadians, Radius;
        private double Deviate, DevRadInc;

        //Current X and Y location
        private int X, Y;

        //Screen information
        private int HalfWidth, HalfHeight;

        public void Init(int width, int height, int boxSize)
        {
            HalfWidth = width / 2;
            HalfHeight = height / 2;

            Radians = 0;
            RInc = (2 * Math.PI) / 360;
            SRadians = 0;
            Deviate = Rand.SmallRand(height / 20) + (height / 15);
            Radius = height / 2 - Deviate * 2 - 2 * boxSize;
            DevRadInc = (Rand.GetDouble() * 10 * 2 * Math.PI) / 360;
        }

        public Point NextFrame()
        {
            int deviate;

            //Calculate the new deviation of the circle's main radius
            deviate = (int)(Math.Sin(SRadians) * Deviate);

            //Calculate box position as a circle with a sine perturbed radius
            X = (int)(Math.Cos(Radians) * (Radius + deviate)) + HalfWidth;
            Y = (int)(Math.Sin(Radians) * (Radius + deviate)) + HalfHeight;

            //Increase greater circle render angle
            Radians += RInc;
            if (Radians > 2 * Math.PI)
                Radians -= 2 * Math.PI;

            SRadians += DevRadInc;

            return new Point(X, Y);
        }
    }
}
