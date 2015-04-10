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
    public partial class DialogOriginsViewWindow : UserControl
    {
        public DialogOriginsViewWindow()
        {
            InitializeComponent();
            scriptsList.ItemsSource = sData.dialogOrigins.Collection;

        }

        private void sendB_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "DiaOrigins";
            l.Content = new DialogOriginsViewWindow();

            l.CanClose = false;
            l.CanFloat = false;

            return l;
        }
    }
}
