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
    /// Логика взаимодействия для KeyStringDataField.xaml
    /// </summary>
    public partial class KeyIntDataField : UserControl
    {
        public KeyValDataPair source;
        public KeyIntDataField(KeyValDataPair src)
        {
            source = src;
            InitializeComponent();
            mainGrid.DataContext = src;
        }
    }
}
