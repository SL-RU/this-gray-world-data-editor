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
using tgwEditor.Quests;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor
{
    /// <summary>
    /// Логика взаимодействия для ScriptsViewWindow.xaml
    /// </summary>
    public partial class GoodsViewWindow : Grid
    {
        public GoodsViewWindow()
        {
            InitializeComponent();
            scriptsList.ItemsSource = sData.goods.Collection;

            //Topmost = true;
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (findF.Text != "")
            {
                try
                {
                    List<GoodsData> f = sData.goods.Collection.Where(x => x.ID.Contains(findF.Text)).ToList();
                    scriptsList.ItemsSource = f;
                }
                catch
                {

                }
            }
            else
            {
                scriptsList.ItemsSource = sData.texts.Collection;
            }


        }

        private void scriptsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (scriptsList.SelectedItem != null)
            {
                editor.setEditingGood(scriptsList.SelectedItem as GoodsData);
            }
        }

        public static LayoutAnchorable CreateNew()
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new GoodsViewWindow();
            la.Title = "Goods view window";
            return la;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var v = GoodsData.New();
            sData.goods.Add(v);


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
