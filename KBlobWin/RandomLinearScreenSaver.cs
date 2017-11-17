using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBlobWin
{
    public class RandomLinearScreenSaver : IScreenSaver
    {
        //How much the X and Y locations move each frame
        private int XInc, YInc;

        //Current X and Y location
        private int X, Y;

        //Screen information
        private int ScreenWidth, ScreenHeight, BoxSize;

        public void Init(int width, int height, int boxSize)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            BoxSize = boxSize;

            // initialize the blob movement dictators with random vals
            // incrementals on axis
            XInc = Rand.SmallRand(3);
            YInc = Rand.SmallRand(2);

            // start position
            X = Rand.SmallRand(width - boxSize - XInc * 2);
            Y = Rand.SmallRand(height - boxSize - YInc * 2);
        }

        public Point NextFrame()
        {
            int direction;

            //Move the blob painter to a new location
            //Check for wall hit to change direction
            if (X + BoxSize + XInc > ScreenWidth - 1 || X + XInc < 0)
            {
                if (XInc > 0)
                    direction = -1;
                else
                    direction = 1;

                XInc = Rand.SmallRand(3) * direction;
            }
            if (Y + BoxSize + YInc > ScreenHeight - 1 || Y + YInc < 0)
            {
                if (YInc > 0)
                    direction = -1;
                else
                    direction = 1;

                YInc = Rand.SmallRand(2) * direction;
            }

            //Move box
            X += XInc;
            Y += YInc;

            //Return the new location of the box
            return new Point(X, Y);
        }
    }
}
