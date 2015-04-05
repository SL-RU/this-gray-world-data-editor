using System;
using System.Collections.Generic;
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
using tgwEditor.Elements;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor.CharEditor
{
    /// <summary>
    /// Логика взаимодействия для CharacterEditor.xaml
    /// </summary>
    public partial class CharacterEditor : UserControl, ILoadableWindow
    {
        public static CharacterEditor lastActive;

        public CharacterData source;

        public string lastEditedCharacterID = "";

        public CharacterEditor()
        {
            InitializeComponent();

            if (sData.APPEARANCE_TEMPLATE != null)
                foreach (var i in sData.APPEARANCE_TEMPLATE.Keys)
                {
                    typeF.Items.Add(i);
                }

            lastActive = this;
            IsEnabled = false;
        }

        public void openNewChar(CharacterData cd)
        {
            if (cd != null)
            {
                source = cd;

                IsEnabled = true;

                lastEditedCharacterID = cd.ID;

                dialogs.SetDialogDistrCollection(cd.dialogs);
                dialogs.CharacterMode = true;
                dialogs.curCharID = source.ID;

                MainLayout.DataContext = source;
                try
                {
                    realNameText.DataContext = sData.texts.Get(source.RealName.Val);
                }
                catch
                {
                    DispatcherTimer ds = new DispatcherTimer();
                    ds.Interval = TimeSpan.FromMilliseconds(50);
                    ds.Tick += ds_Tick;

                    ds.Start();
                }
                spec.ItemsSource = source.spec;
                kn.ItemsSource = source.kn;
                behEditor.setChar(source);

                inventoryEditor.SetCollection(cd.inventory);

                if (LaWindow != null)
                {
                    var sr = (App.Current.MainWindow as MainWindow).SecondaryRight;
                    var i = sr.Children.IndexOf(LaWindow);
                    sr.SelectedContentIndex = i;
                }
            }
            else
            {
                IsEnabled = false;
            }
        }

        void ds_Tick(object sender, EventArgs e)
        {
            try
            {
                realNameText.DataContext = sData.texts.Get(source.RealName.Val);
                ((DispatcherTimer)sender).Stop();
            }
            catch
            {
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CharacterData.CheckAvailibility(idField.Text, source) && !idField.Text.Contains(' ') && !idField.Text.Contains('\'') && !idField.Text.Contains('\"'))
            {
                idField.Foreground = new SolidColorBrush(Color.FromRgb(254, 255, 255));
            }
            else
            {
                idField.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            }
            if (LaWindow != null)
                LaWindow.Title = source.ID;
        }
        private void idField_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((idField.Foreground as SolidColorBrush).Color.R == 255)
            {
                MessageBox.Show("ID must contains only latin symbols and be UNIQUE!!! NO spaces or other symbols!!! ");
            }
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lastActive == this)
            {
                lastActive = null;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            appearance.Items.Clear();
            foreach (var i in sData.APPEARANCE_TEMPLATE[source.Type].Keys)
            {
                appearance.Items.Add(new CharacterAppearnceElement(source, i));
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            lastActive = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            source.kn.Add(KeyValDataPair.New("k", KeyValDataPair.VALUE_TYPE_STRING));
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            if (kn.SelectedItem != null)
            {
                var els = new List<KeyValDataPair>(kn.SelectedItems.Cast<KeyValDataPair>());
                foreach (KeyValDataPair s in els)
                {
                    source.kn.Remove(s);
                }
            }
        }

        public LayoutAnchorable LaWindow;

        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "Char";

            var w = this;
            l.Content = w;
            w.LaWindow = l;
            l.AutoHideMinWidth = l.FloatingWidth = w.MinWidth;


            return l;
        }

        public static CharacterEditor CreateNewWindow()
        {
            var ed = new CharacterEditor();
            MainWindow.instance.openWindowInPanel(ed.CreateWindow(), tgwEditor.MainWindow.MainWindowPanelType.SecondaryRight, true);
            return ed;
        }

        public void Loading()
        {
            openNewChar(sData.characters.Get(lastEditedCharacterID));
        }

        public void Saving()
        {

        }
    }
}
namespace tgwEditor
{
    public class TextKeyValuePairToTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as KeyValDataPair;
            if (val != null)
            {
                var v = sData.texts.Get(val.Val);
                if (v != null)
                    return v.Text;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
