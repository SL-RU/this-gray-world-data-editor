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

            scriptsList.SelectionMode = SelectionMode.Extended;
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
                scriptsList.ItemsSource = sData.goods.Collection;
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

            ///Template
            v.vars.Add(KeyValDataPair.New("name", KeyValDataPair.VALUE_TYPE_TEXTID));
            v.vars.Add(KeyValDataPair.New("description", KeyValDataPair.VALUE_TYPE_TEXTID));
            v.vars.Add(KeyValDataPair.New("cost", KeyValDataPair.VALUE_TYPE_INT));
            v.vars.Add(KeyValDataPair.New("mass", KeyValDataPair.VALUE_TYPE_INT));
            v.vars.Add(KeyValDataPair.New("icon", KeyValDataPair.VALUE_TYPE_IMAGE_PATH));
            ///

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (scriptsList.SelectedItems.Count > 0)
            {
                for (int i = scriptsList.SelectedItems.Count - 1; i >= 0; i--)
                {
                    sData.goods.Collection.Remove(scriptsList.SelectedItems[i] as GoodsData);
                }
            }
        }
    }
}
