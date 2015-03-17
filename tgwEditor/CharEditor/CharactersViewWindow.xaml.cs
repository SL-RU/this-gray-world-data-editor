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

namespace tgwEditor.CharEditor
{
    /// <summary>
    /// Логика взаимодействия для CharactersViewWindow.xaml
    /// </summary>
    public partial class CharactersViewWindow : UserControl
    {
        public MainWindow MainWindow;
        public CharactersViewWindow()
        {
            MainWindow = MainWindow.instance;

            InitializeComponent();

            lst.ItemsSource = sData.characters.Collection;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (findF.Text != "")
            {
                List<CharacterData> f = sData.characters.Collection.Where(x => x.ID.Contains(findF.Text) || sData.texts.Get(x.RealName.Val).Text.Contains(findF.Text)).ToList();
                lst.ItemsSource = f;
            }
            else
            {
                lst.ItemsSource = sData.characters.Collection;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            CharacterData el = sData.characters.Add(CharacterData.New("Enter original character ID!!!"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lst.SelectedItem != null)
            {
                for (int i = lst.SelectedItems.Count - 1; i >= 0; i--)
                {
                    sData.characters.Collection.Remove(lst.SelectedItems[i] as CharacterData);
                }
            }
        }

        private void lst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lst.SelectedItem != null)
            {
                if (CharacterEditor.lastActive != null)
                {
                    CharacterEditor.lastActive.openNewChar(lst.SelectedItem as CharacterData);
                }
                else
                {
                    CharacterEditor ed = new CharacterEditor();
                    MainWindow.openWindowInPanel(ed.CreateWindow(), tgwEditor.MainWindow.MainWindowPanelType.SecondaryRight, true);
                    ed.openNewChar(lst.SelectedItem as CharacterData);
                }
            }
        }

        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "Characters";
            var v = new CharactersViewWindow();
            l.Content = v;

            l.AutoHideMinWidth = l.FloatingWidth = v.MinWidth;

            return l;
        }


    }
}
