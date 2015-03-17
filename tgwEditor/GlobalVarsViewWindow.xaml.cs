using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor
{
    /// <summary>
    /// Логика взаимодействия для ScriptsViewWindow.xaml
    /// </summary>
    public partial class TextViewWindow : UserControl
    {
        public TextViewWindow()
        {
            InitializeComponent();
            scriptsList.ItemsSource = sData.texts.Collection;

            //Topmost = true;

            foreach (TextData d in sData.texts.Collection)
            {
                d.PropertyChanged+= d_TextChanged_Event; 
            }

            //sData.texts.Collection.CollectionChanged += texts_CollectionChanged;
        }


        void d_TextChanged_Event(object sender, EventArgs e)
        {
            //scriptsList.Items.Refresh();
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (findF.Text != "")
            {
                List<TextData> f = sData.texts.Collection.Where(x => x.Text.Contains(findF.Text) || x.ID.ToString().Contains(findF.Text)).ToList();
                scriptsList.ItemsSource = f;
            }
            else
            {
                scriptsList.ItemsSource = sData.texts.Collection;
            }
        }

        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "Texts";
            var v = new TextViewWindow();
            l.Content = v;

            l.AutoHideMinWidth = l.FloatingWidth = v.MinWidth;

            return l;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            findF.Focus();
        }
    }
}
