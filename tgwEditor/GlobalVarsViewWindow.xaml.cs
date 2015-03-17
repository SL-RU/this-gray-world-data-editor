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
    public partial class GlobalVarsViewWindow : UserControl, ILoadableWindow
    {
        public GlobalVarsViewWindow()
        {
            InitializeComponent();

            vars.SetCollection(sData.global.Collection, "text,link,number,string,good");
        }


        void d_TextChanged_Event(object sender, EventArgs e)
        {
            //scriptsList.Items.Refresh();
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (findF.Text != "")
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "Global";
            var v = new GlobalVarsViewWindow();
            l.Content = v;

            l.AutoHideMinWidth = l.FloatingWidth = v.MinWidth;

            return l;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            findF.Focus();
        }

        public void Loading()
        {
            vars.SetCollection(sData.global.Collection, "text,link,number,string,good");
        }

        public void Saving()
        {
            
        }
    }
}
