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
using tgwEditor.DiaEditor;

namespace tgwEditor.Elements
{
    /// <summary>
    /// Логика взаимодействия для KeyTextDataField.xaml
    /// </summary>
    public partial class KeyTextDataField : UserControl
    {
        public KeyValDataPair source;
        public KeyTextDataField(KeyValDataPair src, GraphDiaEdit wnd)
        {
            source = src;
            InitializeComponent();

            TextDataField tf = new TextDataField(src);
            tf.SetValue(Grid.ColumnProperty, 1);
            main.Children.Add(tf);
            main.DataContext = src;
        }
    }
}
