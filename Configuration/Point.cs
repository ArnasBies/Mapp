using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapp
{
    internal class Point
    {
        public readonly float x, y;
        public readonly string ObjectName;

        public Point(int X, int Y, string objectName)
        {
            x = X;
            y = Y;
            ObjectName = objectName;
        }
    }
}
