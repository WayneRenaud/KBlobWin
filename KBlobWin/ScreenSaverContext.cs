using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBlobWin
{
    public class ScreenSaverContext
    {
        private Rectangle Rect;
        private List<IScreenSaver> ScreenSavers;
        private int CurrentScreenSaverIndex;
        private bool isRandom;

        int ScreenWidth, ScreenHeight, BoxSize;

        public ScreenSaverContext(int screenWidth, int screenHeight, int startSaver)
        {
            if (startSaver == 999)
                isRandom = true;
            else
                isRandom = false;

            CurrentScreenSaverIndex = startSaver;
            Reset(screenWidth, screenHeight);
        }

        public void Reset(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            BoxSize = screenHeight / 70 + 1;

            //Init the rect
            Rect = new Rectangle();
            Rect.Height = BoxSize;
            Rect.Width = BoxSize;

            //Initialize the algorithms
            ScreenSavers = new List<IScreenSaver>();
            ScreenSavers.Add(new RandomLinearScreenSaver());
            ScreenSavers.Add(new HorizontalSineScreenSaver());
            ScreenSavers.Add(new CircularBounceScreenSaver());
            ScreenSavers.Add(new PolarCoordinatesScreenSaver());

            //Randomize if needed
            if (isRandom)
                CurrentScreenSaverIndex = Rand.SmallRand(ScreenSavers.Count) - 1;

            //Start the first screensaver
            ScreenSavers[CurrentScreenSaverIndex].Init(ScreenWidth, ScreenHeight, BoxSize);
        }

        public Rectangle GetNextRect()
        {
            Point location = ScreenSavers[CurrentScreenSaverIndex].NextFrame();
            Rect.X = location.X;
            Rect.Y = location.Y;

            return Rect;
        }

        public void NextScreenSaver()
        {
            //Rotate to the next screen saver
            CurrentScreenSaverIndex++;
            CurrentScreenSaverIndex %= ScreenSavers.Count;

            //Start it up
            ScreenSavers[CurrentScreenSaverIndex].Init(ScreenWidth, ScreenHeight, BoxSize);
        }

        private void RandomScreenSaver()
        {
            CurrentScreenSaverIndex = Rand.SmallRand(ScreenSavers.Count) - 1;
            ScreenSavers[CurrentScreenSaverIndex].Init(ScreenWidth, ScreenHeight, BoxSize);
        }

        internal void PrevScreenSaver()
        {
            //Rotate to the previous screen saver
            CurrentScreenSaverIndex--;
            if (CurrentScreenSaverIndex < 0)
                CurrentScreenSaverIndex = ScreenSavers.Count - 1;

            //Start it up
            ScreenSavers[CurrentScreenSaverIndex].Init(ScreenWidth, ScreenHeight, BoxSize);
        }
    }
}
