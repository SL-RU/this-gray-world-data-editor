using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Xml.Linq;
using tgwEditor.Elements;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor
{
    /// <summary>
    /// Логика взаимодействия для ScriptsViewWindow.xaml
    /// </summary>
    /// 
    public partial class ImagesViewWindow : UserControl, ILoadableWindow
    {
        public static ObservableCollection<FileBinding> imges;
        FileSystemWatcher watcher;

        public ImagesViewWindow()
        {
            watcher = new FileSystemWatcher();
            if (Directory.Exists(sData.path + "imges\\"))
            {
                watcher = new FileSystemWatcher();
                watcher.Path = sData.path + "imges";

                watcher.Created += watcher_Created;
                watcher.Deleted += watcher_Deleted;
                watcher.Renamed += watcher_Renamed;
                watcher.EnableRaisingEvents = true;
            }
            InitializeComponent();
            Init();
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Rescan()));
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Rescan()));
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Rescan()));
        }


        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "Images";
            l.Content = new ImagesViewWindow();

            return l;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (findF.Text != "")
            {
                filesBinding.ItemsSource = imges.Where(x => x.name.ToUpper().Contains(findF.Text.ToUpper()) ||
                    x.description.ToUpper().Contains(findF.Text.ToUpper()) ||
                    x.dataFileName.ToUpper().Contains(findF.Text.ToUpper()));
            }
            else
                filesBinding.ItemsSource = imges;
        }

        XDocument DB_source;
        string db_filepath = "\\imges\\img_db.xml";
        public void Init()
        {
            DB_source = new XDocument();
            DB_source.Add(new XElement("images"));
            imges = new ObservableCollection<FileBinding>();
            filesBinding.ItemsSource = imges;

            Rescan();
        }

        public FileBinding GetImageData(string fileName)
        {
            var v = DB_source.Root.Elements().Where(x => x.Attribute("fileName").Value == fileName);
            if (v.Count() > 0)
                return new FileBinding(v.First());
            return null;
        }

        public void Rescan()
        {
            if (Directory.Exists(sData.path + "imges\\"))
            {
                watcher.EnableRaisingEvents = false;
                imges.Clear();
                var v = (Directory.EnumerateFiles(sData.path + "\\imges"));
                foreach (var i in v)
                {
                    if (i.EndsWith(".png") || i.EndsWith(".jpg"))
                    {
                        var data = GetImageData(i.Remove(0, (sData.path + "\\imges\\").Length));
                        if (data == null)
                        {
                            data = FileBinding.New(i);
                            DB_source.Root.Add(data.source);
                        }
                        imges.Add(data);
                    }
                }
                watcher.EnableRaisingEvents = true;
            }
        }

        public void Loading()
        {
            watcher.EnableRaisingEvents = false;
            string filepath = sData.path + db_filepath;
            if (File.Exists(filepath))
                DB_source = XDocument.Load(filepath);
            Rescan();
            watcher.Path = sData.path + "//imges";
            watcher.EnableRaisingEvents = true;
        }

        public void Saving()
        {
            DB_source.Save(sData.path + db_filepath);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            watcher.EnableRaisingEvents = false;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var f in files)
                {
                    int ind = 0;
                    string name, sname = f.Split('\\').Last();
                    name = sname;
                    if (File.Exists(sData.path + "\\imges\\" + name))
                    {
                        while (File.Exists(sData.path + "\\imges\\" + name))
                        {
                            name = ind.ToString() + sname;
                            ind++;
                        }
                    }
                    File.Copy(f, sData.path + "\\imges\\" + name);
                }
                Rescan();
            }
            watcher.EnableRaisingEvents = true;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as MenuItem).DataContext != null))
            {
                var file = ((sender as MenuItem).DataContext as FileBinding);
                if (File.Exists(file.filePath))
                {
                    File.Delete(file.filePath);
                    Rescan();
                }
            }
        }

        private void LableItem_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as MenuItem).DataContext != null))
            {
                var file = ((sender as MenuItem).DataContext as FileBinding);

                var wi = InputBoxWindow.CreateNew("Change image lable", "Image lable", file.name);
                wi.OnTextEnteredEvent += new InputBoxWindow.TextEntered(delegate(InputBoxWindow w, string s, bool b)
                {
                    if (!b)
                    {
                        file.name = s;
                    }
                });
            }
        }

        private void DescriptionItem_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as MenuItem).DataContext != null))
            {
                var file = ((sender as MenuItem).DataContext as FileBinding);

                var wi = InputBoxWindow.CreateNew("Change image description", "Image description", file.description);
                wi.OnTextEnteredEvent += new InputBoxWindow.TextEntered(delegate(InputBoxWindow w, string s, bool b)
                {
                    if (!b)
                    {
                        file.description = s;
                    }
                });
            }
        }
    }
    public class FileBinding : INotifyPropertyChanged
    {
        public XElement source;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null && propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public FileBinding(XElement xe)
        {
            source = xe;
            image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(filePath);
            image.EndInit();
        }

        /// <summary>
        /// global file name
        /// </summary>
        public string filePath
        {
            get
            {
                return sData.path + "\\imges\\" + dataFileName;
            }
            //set { dataFileName = value.Remove(0, (sData.path + "\\imges\\").Length); }
        }
        /// <summary>
        /// relative file name
        /// </summary>
        public string dataFileName
        {
            get
            {
                return source.Attribute("fileName").Value.ToString();
            }
            //set { source.Attribute("fileName").Value = value; }
        }
        public string name
        {
            get { return source.Attribute("name").Value.ToString(); }
            set { source.Attribute("name").Value = value; OnPropertyChanged(); }
        }
        public string description
        {
            get { return source.Attribute("description").Value.ToString(); }
            set { source.Attribute("description").Value = value; OnPropertyChanged(); }
        }

        public BitmapImage image
        {
            get;
            set;
        }

        public static FileBinding New(string FullFilename)
        {
            XElement x = new XElement("img");
            x.Add(new XAttribute("fileName", FullFilename.Remove(0, (sData.path + "\\imges\\").Length)));
            x.Add(new XAttribute("name", FullFilename.Remove(0, (sData.path + "\\imges\\").Length)));
            x.Add(new XAttribute("description", FullFilename.Remove(0, (sData.path + "\\imges\\").Length)));
            x.Add(new XAttribute("ID", Guid.NewGuid().ToString()));
            return new FileBinding(x);
        }
    }
}
