using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
        private Configuration currentConfig;
        private List<Configuration>? configurations = new List<Configuration>();
        private string? newConfig;
        private Uri? uri;

        public MainWindow()
        {
            InitializeComponent();
            InitializeConfigurations();
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

            foreach(Configuration config in configurations)
            {
                if(config.MapName == CurrentMapsListBox.SelectedItem && CurrentMapsListBox.SelectedItem != null)
                {
                    CurrentSelection.Text = $"Map Selected: {config.MapName}";
                    currentConfig = config;
                    MapBackround.Source = config.GetConfigBackground();
                }
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

        //Misc functions
        private void EnterConfigMenu()
        {
            Play.Visibility = Visibility.Collapsed;
            CreateMap.Visibility = Visibility.Collapsed;
            ShowMaps.Visibility = Visibility.Collapsed;
            Configure.Visibility = Visibility.Collapsed;

            RemovePoint.Visibility = Visibility.Visible;
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
            RemovePoint.Visibility = Visibility.Collapsed;
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
