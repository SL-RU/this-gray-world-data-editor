using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using tgwEditor.CharEditor;
using tgwEditor.DiaEditor;
using tgwEditor.Quests;
using tgwEditor.scripts;
using Xceed.Wpf.AvalonDock.Layout;
using MahApps.Metro.Controls;
using tgwEditor.Elements;

namespace tgwEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public static MainWindow instance;

        public MainWindow()
        {
            InitializeComponent();

            instance = this;

            sData.Init();
            AppSettings.Load();
            if (AppSettings.FolderPath == "")
            {
                SetDataPath_Click(this, null);
            }
            LogList.ItemsSource = sData.log;
            sData.log.CollectionChanged += log_CollectionChanged;


            #region Adding view windows
            openWindowInPanel(ImagesViewWindow.CreateWindow(), MainWindowPanelType.Left, true);
            openWindowInPanel(ProjectsViewWindow.CreateWindow(), MainWindowPanelType.Left, true);
            openWindowInPanel(TextViewWindow.CreateWindow(), MainWindowPanelType.Left, true);
            openWindowInPanel(ScriptsViewWindow.CreateWindow(), MainWindowPanelType.Left, true);
            openWindowInPanel(DialogOriginsViewWindow.CreateWindow(), MainWindowPanelType.Left, true);
            openWindowInPanel(GlobalVarsViewWindow.CreateWindow(), MainWindowPanelType.Left, true);

            openWindowInPanel(CharactersViewWindow.CreateWindow(), MainWindowPanelType.Right, true);
            openWindowInPanel(ScriptsHelpWindow.CreateWindow(), MainWindowPanelType.Right, true);
            openWindowInPanel(QuestsViewWindow.CreateNew(), MainWindowPanelType.Right, true);
            openWindowInPanel(GoodsViewWindow.CreateNew(), MainWindowPanelType.Right, true);

            #endregion



            sData.Load();

        }

        #region Log functions
        /// <summary>
        /// Log window and state bar updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void log_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LogList.SelectedIndex = LogList.Items.Count - 0;
            LogList.ScrollIntoView(LogList.SelectedItem);

            string ls = sData.log.Last();
            if (ls.EndsWith("/s/") || ls.EndsWith("/e/"))
            {
                logPreviewBox.Text = ls.Remove(ls.Length - 3);
                if (ls.EndsWith("/s/"))
                {
                    logProcessingBar.IsActive = true;
                }
                else
                {
                    logProcessingBar.IsActive = false;
                }
            }
            else
            {
                logPreviewBox.Text = ls;
            }
        }
        #endregion

        public enum MainWindowPanelType
        {
            Left,
            Right,
            SecondaryRight
        }
        public List<ILoadableWindow> Registered = new List<ILoadableWindow>();

        [System.Obsolete("use openWindowInPanel")]
        public void openDocumentLayoutSecondary(LayoutAnchorable dc, bool register)
        {
            SecondaryRight.Children.Add(dc);
            if (register)
            {
                if (dc is ILoadableWindow)
                {
                    RegisterLoadobleWindow((ILoadableWindow)dc);
                }
            }
        }
        public void openWindowInPanel(LayoutAnchorable lc, MainWindowPanelType panel, bool register)
        {
            switch (panel)
            {
                case MainWindowPanelType.Left: Left.Children.Add(lc); Left.DockMinWidth = Math.Max(lc.AutoHideMinWidth, Left.DockMinWidth); break;
                case MainWindowPanelType.Right: Right.Children.Add(lc); Right.DockMinWidth = Math.Max(lc.AutoHideMinWidth, Right.DockMinWidth); break;
                case MainWindowPanelType.SecondaryRight: SecondaryRight.Children.Add(lc); SecondaryRight.DockMinWidth = Math.Max(lc.AutoHideMinWidth, SecondaryRight.DockMinWidth); break;
            }
            if (register)
            {

                if (typeof(ILoadableWindow).IsAssignableFrom(lc.Content.GetType()))
                {
                    RegisterLoadobleWindow((ILoadableWindow)lc.Content);
                }
            }
        }

        public void RegisterLoadobleWindow(ILoadableWindow wnd)
        {
            Registered.Add(wnd);
        }
        public void UnRegisterLoadobleWindow(ILoadableWindow wnd)
        {
            if (Registered.Contains(wnd))
            {
                Registered.Remove(wnd);
            }
        }

        public void Saving()
        {
            int i = 0;
            i++;
            foreach (var l in Registered)
            {
                if (l != null)
                {
                    try
                    {
                        l.Saving();
                    }
                    catch
                    {

                    }
                }
            }
        }
        public void Loading()
        {
            foreach (var l in Registered)
            {
                if (l != null)
                {
                    try
                    {
                        l.Loading();
                    }
                    catch
                    {

                    }
                }
            }
        }

        #region Menu
        private void LoadAllData_Click(object sender, RoutedEventArgs e)
        {
            sData.Load();
        }

        private void newPrj_Click(object sender, RoutedEventArgs e)
        {
            Documents.Children.Add(GraphDiaEdit.CreateWindow());
            Documents.SelectedContentIndex = Documents.Children.Count - 1;

        }



        private void SaveAllData_Click(object sender, RoutedEventArgs e)
        {
            sData.Save();
        }

        private void SetDataPath_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();

            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filename = dlg.SelectedPath;

                filename += "\\";
                sData.LOG("Choosen path = " + filename);
                sData.InitDirectory(filename);

                //merge
                if (sender != this)
                {
                    if (MessageBox.Show("Path updated SUCCESSFULY. Do you want merge everything?", "Merge?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        sData.MergeToNewDirectory(filename);
                        AppSettings.FolderPath = filename;
                        AppSettings.UpdateConfigFile();
                        sData.LOG("Path updated SUCCESSFULY.");
                    }
                    else
                    {
                        AppSettings.FolderPath = filename;
                        AppSettings.UpdateConfigFile();
                        sData.LOG("Path updated SUCCESSFULY.");
                        MessageBox.Show("Now restart application!");
                    }
                }
                else
                {
                    AppSettings.FolderPath = filename;
                    AppSettings.UpdateConfigFile();
                    sData.LOG("Path updated SUCCESSFULY.");
                }


            }

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you save all data?\nMenu -> DATA -> Save ALL data \nDo you want QUIT???", "DO YOU WANT QUIT???", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                App.Current.Shutdown();
            }
        }

        private void OpenCharEditorWindow(object sender, RoutedEventArgs e)
        {
            CharacterEditor.CreateNewWindow();
        }

        private void OpenProjectFolderInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (sData.path != "")
                Process.Start(sData.path);
            else
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory);

            }
        }

        #endregion
    }

    /// <summary>
    /// Interface for editor windows. Save and load events
    /// </summary>
    public interface ILoadableWindow
    {
        void Loading();
        void Saving();
    }
}
