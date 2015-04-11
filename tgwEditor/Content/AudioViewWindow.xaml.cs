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
using NAudio.Wave;
using NAudio.Lame;



namespace tgwEditor
{
    /// <summary>
    /// Логика взаимодействия для ScriptsViewWindow.xaml
    /// </summary>
    /// 
    public partial class AudioViewWindow : UserControl, ILoadableWindow
    {
        public static ObservableCollection<FileBinding> audios;
        public static AudioViewWindow instance;

        public MediaPlayer mp = new MediaPlayer();

        
        FileSystemWatcher watcher;


        public AudioViewWindow()
        {
            instance = this;
            watcher = new FileSystemWatcher();
            if (Directory.Exists(sData.path + "audio\\"))
            {
                watcher = new FileSystemWatcher();
                watcher.Path = sData.path + "audio";

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
            l.Title = "Audio";
            l.Content = new AudioViewWindow();

            l.CanClose = false;
            l.CanFloat = false;

            return l;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (findF.Text != "")
            {
                filesBinding.ItemsSource = audios.Where(x => x.name.ToUpper().Contains(findF.Text.ToUpper()) ||
                    x.description.ToUpper().Contains(findF.Text.ToUpper()) ||
                    x.dataFileName.ToUpper().Contains(findF.Text.ToUpper()));
            }
            else
                filesBinding.ItemsSource = audios;
        }

        XDocument DB_source;
        string db_filepath = "audio\\audio_db.xml";
        public void Init()
        {
            if (Directory.Exists(sData.path))
            {
                if (!Directory.Exists(sData.path + "audio"))
                {
                    Directory.CreateDirectory(sData.path + "audio");
                }
            }

            DB_source = new XDocument();
            DB_source.Add(new XElement("audio"));
            audios = new ObservableCollection<FileBinding>();
            filesBinding.ItemsSource = audios;

            Rescan();
        }

        public FileBinding GetAudioData(string fileName)
        {
            var v = DB_source.Root.Elements().Where(x => x.Attribute("fileName").Value == fileName);
            if (v.Count() > 0)
                return new FileBinding(v.First(), (sData.path + "audio\\"), FileBinding.AUDIO_TYPE);
            return null;
        }

        public void Rescan()
        {
            if (Directory.Exists(sData.path + "audio"))
            {
                watcher.EnableRaisingEvents = false;
                audios.Clear();
                var v = (Directory.EnumerateFiles(sData.path + "audio"));


                if (v.Where(x=>x.EndsWith(".wav") || x.EndsWith(".mp3")).Count() > 0) //hide tip
                    drop_tip.Visibility = System.Windows.Visibility.Collapsed;
                else
                    drop_tip.Visibility = System.Windows.Visibility.Visible;

                foreach (var i in v)
                {
                    if (i.EndsWith(".wav"))
                    {
                        var data = GetAudioData(i.Remove(0, (sData.path + "audio\\").Length));
                        if (data == null)
                        {
                            data = FileBinding.New(i, sData.path + "audio\\", FileBinding.AUDIO_TYPE);
                            DB_source.Root.Add(data.source);
                        }
                        audios.Add(data);
                    }
                    else if (i.EndsWith(".mp3")) 
                    {
                        //convert mp3 files to wave and deleting source
                        string newI = i.Remove(i.Length - 5) + ".wav";
                        Codec.MP3ToWave(i, newI);
                        File.Delete(i);

                        var data = GetAudioData(newI.Remove(0, (sData.path + "audio\\").Length));
                        if (data == null)
                        {
                            data = FileBinding.New(newI, sData.path + "audio\\", FileBinding.AUDIO_TYPE);
                            DB_source.Root.Add(data.source);
                        }
                        audios.Add(data);
                    }

                }
                watcher.EnableRaisingEvents = true;
            }
        }

        public void Loading()
        {
            if (Directory.Exists(sData.path))
            {
                if (!Directory.Exists(sData.path + "audio"))
                {
                    Directory.CreateDirectory(sData.path + "audio");
                }
            }
            watcher.EnableRaisingEvents = false;
            string filepath = sData.path + db_filepath;
            if (File.Exists(filepath))
                DB_source = XDocument.Load(filepath);
            Rescan();
            watcher.Path = sData.path + "audio";
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
                    if (File.Exists(sData.path + "audio\\" + name))
                    {
                        while (File.Exists(sData.path + "audio\\" + name))
                        {
                            name = ind.ToString() + sname;
                            ind++;
                        }
                    }
                    File.Copy(f, sData.path + "audio\\" + name);
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

                var wi = InputBoxWindow.CreateNew("Change audio lable", "Audio lable", file.name);
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

                var wi = InputBoxWindow.CreateNew("Change audio description", "Audio description", file.description);
                wi.OnTextEnteredEvent += new InputBoxWindow.TextEntered(delegate(InputBoxWindow w, string s, bool b)
                {
                    if (!b)
                    {
                        file.description = s;
                    }
                });
            }
        }


        private void Play_Click(object sender, RoutedEventArgs e)
        {
            var f = ((sender as Button).DataContext as FileBinding).filePath;
            mp.Open(new Uri(f));
            mp.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
        }
    }

    public static class Codec
    {
        // Convert WAV to MP3 using libmp3lame library
        public static void WaveToMP3(string waveFileName, string mp3FileName, int bitRate = 128)
        {
            using (var reader = new WaveFileReader(waveFileName))
            using (var writer = new LameMP3FileWriter(mp3FileName, reader.WaveFormat, bitRate))
                reader.CopyTo(writer);
        }

        // Convert MP3 file to WAV using NAudio classes only
        public static void MP3ToWave(string mp3FileName, string waveFileName)
        {
            using (var reader = new Mp3FileReader(mp3FileName))
            using (var writer = new WaveFileWriter(waveFileName, reader.WaveFormat))
                reader.CopyTo(writer);
        }
    }
}
