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

        public List<MapObject>? MapObjects { get; private set; }

        public readonly string MapName;

        //constructors
        public Configuration(Uri backgroundImageUri, string mapName)
        {
            MapName = mapName;
            MapObjects = new();
            background = new(backgroundImageUri);
        }

        //functions
        public void AddObjectToMap(MapObject p)
        {
            MapObjects?.Add(p);
        }

        public BitmapImage GetConfigBackground()
        {
            return background;
        }
    }
}
