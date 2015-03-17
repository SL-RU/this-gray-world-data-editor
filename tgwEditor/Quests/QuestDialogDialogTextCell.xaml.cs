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


namespace tgwEditor.Quests
{
    /// <summary>
    /// Логика взаимодействия для QuestDialogDistributeItem.xaml
    /// </summary>
    public partial class QuestDialogDialogTextCell : UserControl
    {

        public DialogDistributionItemData source;

        public QuestDialogDialogTextCell()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            source = (DialogDistributionItemData)DataContext;

            TextDataField td = new TextDataField(source.answer.Text);
            text.Children.Add(td);
        }
    }
}
