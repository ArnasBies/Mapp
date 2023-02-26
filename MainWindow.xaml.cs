using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

            Globals.configurations.Add(new Configuration(uri, newConfig));
            
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
            foreach (Configuration config in Globals.configurations)
            {
                CurrentMapsListBox.Items.Add(config.MapName);
            }
            
        }

        private void SelectMap_Click(object sender, RoutedEventArgs e)
        {
            //sets the selected map configuration as the current configuration
            SelectMap.Visibility = Visibility.Collapsed;
            CurrentMaps.Visibility = Visibility.Collapsed;

            foreach(Configuration config in Globals.configurations)
            {
                if(config.MapName == CurrentMapsListBox.SelectedItem)
                {
                    CurrentSelection.Text = $"Map Selected: {config.MapName}";
                    config.SetConfiguration();
                }
            }

            CurrentMapsListBox.Items.Clear();
            ShowMaps.Visibility = Visibility.Visible;
        }

        //Misc functions
        private void InitializeConfigurations()
        {
            //gets all of the maps by the names of the directories of the configurations
            foreach(string configPath in Directory.GetDirectories($"{Environment.CurrentDirectory}/configurations/"))
            {
                string configName = configPath.Split(System.IO.Path.DirectorySeparatorChar).Last().Split("/").Last();
                try
                {
                    Globals.configurations.Add(new Configuration(new Uri($"{configPath}/{configName}.png"), configName));
                }
                catch
                {
                    Globals.configurations.Add(new Configuration(new Uri($"{configPath}/{configName}.jpg"), configName));
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

        private void Configure_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
