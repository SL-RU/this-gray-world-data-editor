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
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor
{
    /// <summary>
    /// Логика взаимодействия для ScriptsViewWindow.xaml
    /// </summary>
    public partial class ScriptsViewWindow : UserControl
    {
        public ScriptsViewWindow()
        {
            InitializeComponent();
            scriptsList.ItemsSource = sData.scripts.Collection;
        }

        private void sendB_Click(object sender, RoutedEventArgs e)
        {
            if(scriptsList.SelectedItem != null)
            {
                var scr = scriptsList.SelectedItem as ScriptData;
                sData.sendScript(scr.ID);
            }
        }

        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "Scripts";
            l.Content = new ScriptsViewWindow();

            return l;
        }
    }
}
