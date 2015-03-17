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
    public partial class QuestsViewWindow : Grid
    {
        public QuestsViewWindow()
        {
            InitializeComponent();
            scriptsList.ItemsSource = sData.quests.Collection;

            //Topmost = true;
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

                if (findF.Text != "")
                {
                    try
                    {
                        List<QuestData> f = sData.quests.Collection.Where(x => x.ID.Contains(findF.Text) || sData.texts.Get(x.Name.Val).Text.ToString().Contains(findF.Text)).ToList();
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
            if(scriptsList.SelectedItem != null)
            {
                if(QuestsEditor.instance != null)
                {
                    QuestsEditor.instance.setEditingQuest((QuestData)scriptsList.SelectedItem);
                }
                else
                {
                    QuestsEditor.OpenNewInMainWindow().setEditingQuest((QuestData)scriptsList.SelectedItem);
                }
            }
        }

        public static LayoutAnchorable CreateNew()
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new QuestsViewWindow();
            la.Title = "Quests view window";
            return la;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var v = QuestData.New();
            sData.quests.Add(v);

            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
