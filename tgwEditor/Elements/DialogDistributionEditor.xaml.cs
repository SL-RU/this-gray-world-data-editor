using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace tgwEditor.Elements
{
    /// <summary>
    /// Логика взаимодействия для DialogDistributionEditor.xaml
    /// </summary>
    public partial class DialogDistributionEditor : UserControl
    {

        public ObservableCollection<DialogDistributionItemData> diaDistrs;

        public bool _characterMode = false;
        public bool CharacterMode
        {
            get
            {
                return _characterMode;
            }
            set
            {
                _characterMode = value;
                if (!value)
                    dialogs.Columns[1].Visibility = System.Windows.Visibility.Visible;
                else
                    dialogs.Columns[1].Visibility = System.Windows.Visibility.Collapsed;

            }
        }
        public string curCharID = "";

        public DialogDistributionEditor()
        {
            InitializeComponent();
        }

        public void SetDialogDistrCollection(ObservableCollection<DialogDistributionItemData> d)
        {
            diaDistrs = d;
            if (d != null)
            {
                dialogs.ItemsSource = d;
            }
        }

        private void addDia_Click(object sender, RoutedEventArgs e)
        {
            var ne = DialogDistributionItemData.New();
            if (CharacterMode)
                ne.Characters = curCharID;
            diaDistrs.Add(ne);
        }

        private void remPDia_Click(object sender, RoutedEventArgs e)
        {
            if (dialogs.SelectedItem != null)
            {
                for (int i = dialogs.SelectedItems.Count - 1; i >= 0; i--)
                {
                    diaDistrs.Remove(dialogs.SelectedItems[i] as DialogDistributionItemData);
                }
            }
        }


    }
}
