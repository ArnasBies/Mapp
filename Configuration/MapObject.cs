using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapp
{
    internal class MapObject
    {
        public readonly double x, y;
        public readonly string ObjectName;

        public MapObject(double X, double Y, string objectName)
        {
            x = X;
            y = Y;
            ObjectName = objectName;
        }
    }
}
