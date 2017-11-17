using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBlobWin
{
    public class HorizontalSineScreenSaver : IScreenSaver
    {
        //Current box location
        private int ScreenWidth, ScreenHeight, BoxSize;

        private float Radians, RadianIncrement, Flip, Period;

        public void Init(int width, int height, int boxSize)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            BoxSize = boxSize;

            Setup();
        }

        private void Setup()
        {
            //Configuration item determines the max number of sine waves
            Period = Rand.SmallRand(int.Parse(ConfigurationManager.AppSettings["humps"]));
            Radians = 0;
            RadianIncrement = (Period * (float)Math.PI) / (Period * 90 * 4);
            Flip = 1;
        }

        public Point NextFrame()
        {
            int xlen = ScreenWidth - (4 * BoxSize);
            int ylen = ScreenHeight - (4 * BoxSize);

            // Calc X as offset on angle line, Y as vertical offset on -1 to 1 sine of angle
            int X = (int)((Radians / (Period * Math.PI)) * (float)xlen);
            int Y = (int)((float)(ylen / 4) * (Flip * Math.Sin(Radians))) + (ScreenHeight / 2);

            // Set new radians for next time
            Radians += RadianIncrement;
            if (Radians > Period * Math.PI)
            {
                RadianIncrement *= -1;
                Radians += RadianIncrement;
                Flip *= -1;
            }
            else if (Radians < 0)
                Setup();

            return new Point(X, Y);
        }
    }
}
