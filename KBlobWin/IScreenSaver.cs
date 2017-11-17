using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace KBlobWin
{
    public interface IScreenSaver
    {
        void Init(int width, int height, int boxSize);
        Point NextFrame();
    }
}
