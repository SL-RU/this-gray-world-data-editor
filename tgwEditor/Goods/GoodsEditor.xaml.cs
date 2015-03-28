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
using System.Windows.Navigation;
using System.Windows.Shapes;
using tgwEditor.Elements;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor.Goods
{
    /// <summary>
    /// Логика взаимодействия для GoodsEditor.xaml
    /// </summary>
    public partial class GoodsEditor : UserControl
    {
        public static GoodsEditor instance = null;

        public GoodsEditor()
        {
            InitializeComponent();
            instance = this;
        }

        public GoodsData source;
        public LayoutAnchorable la = null;

        public void setEditingGood(GoodsData qd)
        {
            DataContext = qd;
            source = qd;

            vars.SetCollection(source.vars, "text,number,string,good,image");

            if (la != null)
            {
                var sr = (App.Current.MainWindow as MainWindow).SecondaryRight;
                var i = sr.Children.IndexOf(la);
                sr.SelectedContentIndex = i;
            }
        }

    }
}
