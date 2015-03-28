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

namespace tgwEditor.Elements
{
    /// <summary>
    /// Логика взаимодействия для KeyGoodDataField.xaml
    /// </summary>
    public partial class KeyImageDataField : UserControl
    {
        public KeyValDataPair source;

        public KeyImageDataField(KeyValDataPair src)
        {
            source = src;
            InitializeComponent();
            mainGrid.DataContext = src;

            filesBinding.ItemsSource = ImagesViewWindow.imges;

            lbl.Text = source.Val;
            src.ValueChanged_Event += src_ValueChanged_Event;
        }

        void src_ValueChanged_Event(object sender, EventArgs e)
        {
            lbl.Text = source.Val;
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchTextBox.Text != "")
            {
                filesBinding.ItemsSource = ImagesViewWindow.imges.Where(x => x.name.ToUpper().Contains(searchTextBox.Text.ToUpper()) ||
                    x.description.ToUpper().Contains(searchTextBox.Text.ToUpper()) ||
                    x.dataFileName.ToUpper().Contains(searchTextBox.Text.ToUpper()));
            }
            else
                filesBinding.ItemsSource = ImagesViewWindow.imges;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                source.Val = ((sender as Button).DataContext as FileBinding).dataFileName;
            }
        }
    }
}
