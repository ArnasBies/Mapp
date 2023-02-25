using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Media;
using System.Windows.Media.Imaging;

namespace Mapp
{
    class Configuration
    {
        //members
        //private string backgroundPath;
        private BitmapImage background;
        private List<Point>? MapObjects;

        public readonly string MapName;

        //constructors
        public Configuration(Uri backgroundImageUri, string mapName)
        {
            MapName = mapName;
            MapObjects = new();
            background = new(backgroundImageUri);
        }

        //functions
        public void addObjectToMap(Point p)
        {
            MapObjects?.Add(p);
        }

        public void SetConfiguration()
        {
        }
    }
}
