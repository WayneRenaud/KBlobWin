using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBlobWin
{
    public static class Rand
    {
        public static Random RandomObject = new Random();

        public static int SmallRand(int max)
        {
            return RandomObject.Next(1, max);
        }

        public static double GetDouble()
        {
            return RandomObject.NextDouble();
        }
    }
}
