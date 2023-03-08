using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Configuration? currentConfig;
        private List<Configuration>? configurations;
        private Uri? uri;
        private bool inAddingMode;

        private string? newConfig;
        private string? tempObjectName;

        public MainWindow()
        {
            configurations = new List<Configuration>();
            inAddingMode = false;

            InitializeComponent();
            InitializeConfigurations();
            RetrieveMapObjects();

            BackgroundArea.MouseLeftButtonDown += OnCanvasClick;
        }

        //Button handlers
        private void CreateMap_Click(object sender, RoutedEventArgs e)
        {
            //throws out a box requiring to enter the name of a new map
            NameBox.Visibility = Visibility.Visible;
            Submit.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Visible;

            //gets image uri from file explorer
            uri = GetImageFromFiles();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //retrieves new map name from textbox and also gets copies map image to configurations folder
            newConfig = NameBox.Text;

            configurations.Add(new Configuration(uri, newConfig));
            
            Directory.CreateDirectory(Environment.CurrentDirectory + "/configurations/" + newConfig);
            File.Copy(uri.AbsolutePath, $"{Environment.CurrentDirectory}/configurations/{newConfig}/{newConfig}{uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf("."))}");
            File.Create($"{Environment.CurrentDirectory}/configurations/{newConfig}/{newConfig}.txt");

            NameBox.Visibility = Visibility.Collapsed;
            Submit.Visibility = Visibility.Collapsed;
            Cancel.Visibility = Visibility.Collapsed;
            NameBox.Clear();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NameBox.Visibility = Visibility.Collapsed;
            Submit.Visibility = Visibility.Collapsed;
            Cancel.Visibility = Visibility.Collapsed;
            NameBox.Clear();
        }

        private void ShowMaps_Click(object sender, RoutedEventArgs e)
        {
            //pops up the list of current map configurations
            ShowMaps.Visibility = Visibility.Collapsed;
            SelectMap.Visibility = Visibility.Visible;
            CurrentMaps.Visibility = Visibility.Visible;

            foreach (Configuration config in configurations)
            {
                CurrentMapsListBox.Items.Add(config.MapName);
            }
            
        }

        private void SelectMap_Click(object sender, RoutedEventArgs e)
        {
            //sets the selected map configuration as the current configuration
            SelectMap.Visibility = Visibility.Collapsed;
            CurrentMaps.Visibility = Visibility.Collapsed;

            if (configurations != null)
            {
                foreach (Configuration config in configurations)
                {
                    if (config.MapName == CurrentMapsListBox.SelectedItem && CurrentMapsListBox.SelectedItem != null)
                    {
                        CurrentSelection.Text = $"Map Selected: {config.MapName}";
                        currentConfig = config;
                        MapBackround.Source = config.GetConfigBackground();
                    }
                }
            }

            if(CurrentMapsListBox.SelectedItems.Count > 0)
            {
                Configure.Visibility = Visibility.Visible;
                Play.Visibility = Visibility.Visible;
            }

            CurrentMapsListBox.Items.Clear();
            ShowMaps.Visibility = Visibility.Visible;
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            EnterConfigMenu();
        }

        private void ExitConfig_Click(object sender, RoutedEventArgs e)
        {
            ExitConfigMenu();
        }

        private void ShowPoints_Click(object sender, RoutedEventArgs e)
        {
            //Shows needed UI and adds shows map objects
            ShowPoints.Visibility = Visibility.Collapsed;
            CurrentPoints.Visibility = Visibility.Visible;
            Hide.Visibility = Visibility.Visible;

            if (currentConfig != null)
            {
                foreach (MapObject p in currentConfig.MapObjects)
                {
                    CurrentPointsListBox.Items.Add(p.ObjectName);
                }
            }
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            //just resets the point box and hides it
            CurrentPointsListBox.Items.Clear();
            ShowPoints.Visibility = Visibility.Visible;
            Hide.Visibility = Visibility.Collapsed;
            CurrentPoints.Visibility = Visibility.Collapsed;
        }

        private void AddPoint_Click(object sender, RoutedEventArgs e)
        {
            MapObjectName.Visibility = Visibility.Visible;
            AddPoint.Visibility = Visibility.Collapsed;
            SubmitObject.Visibility = Visibility.Visible;

            ToolTip.Text = "Enter the name of your \nobject into the textbox";
        }

        private void SubmitObject_Click(object sender, RoutedEventArgs e)
        {
            tempObjectName = MapObjectName.Text;

            MapObjectName.Visibility = Visibility.Collapsed;
            SubmitObject.Visibility = Visibility.Collapsed;
            MapObjectName.Clear();

            ToolTip.Text = "Click on your object on the map";
            inAddingMode = true;
        }

        private void RemoveObject_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(CurrentPointsListBox.SelectedItem.ToString())) return;

            string[] allLines = File.ReadAllLines($"{Environment.CurrentDirectory}/configurations/{currentConfig.MapName}/{currentConfig.MapName}.txt");

            if(allLines.Length == 1 || allLines == null || allLines.Length == 0)
            {
                File.WriteAllText($"{Environment.CurrentDirectory}/configurations/{currentConfig.MapName}/{currentConfig.MapName}.txt", "");

                currentConfig.MapObjects.Clear();
                for (int i = 0; i < configurations.Count; i++) if (configurations[i].MapName == currentConfig.MapName)  configurations[i].MapObjects.Clear();
                ResetObjectBox();
                return;
            }

            string[] changedLines = new string[allLines.Length - 1];
            int changedArrIndex = 0;
            string fullLineName;

            foreach (string line in allLines)
            {
                fullLineName = "";

                if(String.IsNullOrEmpty(line)) continue;
                string[] lineArr = line.Split(" ");

                for (int i = 2; i < lineArr.Length; i++) fullLineName += lineArr[i] + " ";
                
                if(fullLineName.Replace(" ", "") != CurrentPointsListBox.SelectedItem.ToString().Replace(" ", ""))
                {
                    changedLines[changedArrIndex] = line;
                    changedArrIndex++;
                }
            }

            File.WriteAllLines($"{Environment.CurrentDirectory}/configurations/{currentConfig.MapName}/{currentConfig.MapName}.txt", changedLines);

            RetrieveMapObjects();

            for(int i = 0; i < configurations.Count; i++) if (configurations[i].MapName == currentConfig.MapName) currentConfig = configurations[i];

            ResetObjectBox();
        }

        private void OnCanvasClick(object sender, MouseEventArgs e)
        {
            if(inAddingMode)
            {
                Point p = Mouse.GetPosition(BackgroundArea);

                if (configurations != null)
                {
                    for(int i = 0; i < configurations.Count; i++)
                    {
                        if (currentConfig.MapName == configurations[i].MapName)
                        {
                            configurations[i].AddObjectToMap(new MapObject(p.X / BackgroundArea.ActualWidth, p.Y / ActualHeight, tempObjectName));
                            currentConfig = configurations[i];
                        }
                    }
                }

                using (StreamWriter sw = new($"{Environment.CurrentDirectory}/configurations/{currentConfig.MapName}/{currentConfig.MapName}.txt", append: true))
                {
                    sw.WriteLine($"{p.X / BackgroundArea.ActualWidth} {p.Y / BackgroundArea.ActualHeight} {tempObjectName}");
                }

                AddPoint.Visibility = Visibility.Visible;
                ToolTip.Text = "";
                ResetObjectBox();
                inAddingMode = false;
            }
        }

        //Misc functions
        private void ResetObjectBox()
        {
            CurrentPointsListBox.Items.Clear();

            foreach (MapObject p in currentConfig.MapObjects)
            {
                CurrentPointsListBox.Items.Add(p.ObjectName);
            }
        }

        private void EnterConfigMenu()
        {
            Play.Visibility = Visibility.Collapsed;
            CreateMap.Visibility = Visibility.Collapsed;
            ShowMaps.Visibility = Visibility.Collapsed;
            Configure.Visibility = Visibility.Collapsed;

            RemoveObject.Visibility = Visibility.Visible;
            AddPoint.Visibility = Visibility.Visible;
            ExitConfig.Visibility = Visibility.Visible;
            ShowPoints.Visibility = Visibility.Visible;
        }

        private void ExitConfigMenu()
        {
            Play.Visibility = Visibility.Visible;
            CreateMap.Visibility = Visibility.Visible;
            ShowMaps.Visibility = Visibility.Visible;
            Configure.Visibility = Visibility.Visible;

            ShowPoints.Visibility = Visibility.Collapsed;
            RemoveObject.Visibility = Visibility.Collapsed;
            AddPoint.Visibility = Visibility.Collapsed;
            ExitConfig.Visibility = Visibility.Collapsed;
        }

        private void InitializeConfigurations()
        {
            //gets all of the maps by the names of the directories of the configurations

            int failedAttempts;

            foreach(string configPath in Directory.GetDirectories($"{Environment.CurrentDirectory}/configurations/"))
            {
                string configName = configPath.Split(System.IO.Path.DirectorySeparatorChar).Last().Split("/").Last();
                failedAttempts = 0;
                try
                {
                    configurations.Add(new Configuration(new Uri($"{configPath}/{configName}.png"), configName));
                }
                catch
                {
                    failedAttempts++;
                }
                try
                {
                    configurations.Add(new Configuration(new Uri($"{configPath}/{configName}.jpg"), configName));
                }
                catch
                {
                    failedAttempts++;
                }

                if(failedAttempts == 2)
                {
                    Directory.Delete(configPath);
                }

            }
        }

        private void RetrieveMapObjects()
        {
            double x, y;
            string? name;
            string? line;

            if (configurations == null) return;
            //retrieves each map's map objects and adds them to the maps
            for(int i = 0; i < configurations.Count; i++)
            {
                configurations[i].MapObjects.Clear();
                using (StreamReader sr = new($"{Environment.CurrentDirectory}/configurations/{configurations[i].MapName}/{configurations[i].MapName}.txt"))
                {
                    while (sr.Peek != null)
                    {
                        line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) break;

                        string[] currentConfig = line.Split(" ");

                        x = double.Parse(currentConfig[0]);
                        y = double.Parse(currentConfig[1]);
                        name = "";

                        for (int j = 2; j < currentConfig.Length; j++) name += currentConfig[j] + " ";

                        configurations[i].AddObjectToMap(new MapObject(x, y, name));
                    }
                }
            }
        }

        private Uri? GetImageFromFiles()
        {
            //opens file explorer dialog and retrieves selescted file
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.png)|*.png|Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                return new Uri(dlg.FileName);
            }
            return null;
        }
    }
}