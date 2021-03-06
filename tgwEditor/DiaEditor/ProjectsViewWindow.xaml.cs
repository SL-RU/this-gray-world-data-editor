﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using tgwEditor.DiaEditor;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor
{
    /// <summary>
    /// Логика взаимодействия для ScriptsViewWindow.xaml
    /// </summary>
    public partial class ProjectsViewWindow : UserControl
    {
        MainWindow mainWindow;
        public ProjectsViewWindow()
        {
            mainWindow = App.Current.MainWindow as MainWindow;
            InitializeComponent();

            if (!Directory.Exists(sData.path + "dia\\"))
                Directory.CreateDirectory(sData.path + "dia\\");

            Rescan();

            FileSystemWatcher watch = new FileSystemWatcher(sData.path + "dia\\");
            watch.Created += watch_Created;
            watch.Deleted += watch_Deleted;
            watch.Renamed += watch_Renamed;

            dialogs.ItemsSource = Dialogs;
        }

        void watch_Renamed(object sender, RenamedEventArgs e)
        {
            Rescan();
        }

        void watch_Deleted(object sender, FileSystemEventArgs e)
        {
            Rescan();
        }

        void watch_Created(object sender, FileSystemEventArgs e)
        {
            Rescan();
        }
        public class FileTemplate
        {
            public string path 
            {
                get;
                set;
            }
            public string name
            {
                get;
                set;
            }
        }

        public List<FileTemplate> Dialogs = new List<FileTemplate>();


        public void Rescan()
        {
            Dialogs.Clear();
            if (!Directory.Exists(sData.path + "dia\\"))
                Directory.CreateDirectory(sData.path + "dia\\");

            foreach (var fl in Directory.EnumerateFiles(sData.path + "dia\\"))
            {
                Dialogs.Add(new FileTemplate() { path = fl, name = fl.Split('\\').Last().Replace(".diaprj", "") });
            }

            dialogs.ItemsSource = Dialogs;
        }


        private void New_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Documents.Children.Add(GraphDiaEdit.CreateWindow());
            mainWindow.Documents.SelectedContentIndex = mainWindow.Documents.Children.Count - 1;
        }

        private void dialogs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dialogs.SelectedItem != null)
            {
                GraphDiaEdit w = new GraphDiaEdit();
                w.ProjectName = (dialogs.SelectedItem as FileTemplate).name;
                
                mainWindow.Documents.Children.Add(GraphDiaEdit.CreateWindow(w));
                w.LoadProjectGui();

                mainWindow.Documents.SelectedContentIndex = mainWindow.Documents.Children.Count - 1;
            }
            
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Rescan();
        }


        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "Projects";
            var v = new ProjectsViewWindow();
            l.Content = v;

            l.AutoHideMinWidth = l.FloatingWidth = v.MinWidth;

            return l;
        }
    }
}
