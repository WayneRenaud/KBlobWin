using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;

namespace KBlobWin
{
    class KBlobWin
    {
        static void Main(string[] args)
        {
            //Kick things off
            MainWindow win = new MainWindow();
            win.StartLoop();
        }
    }
}
