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
using System.Windows.Threading;
using tgwEditor.CharEditor.behEditor;

namespace tgwEditor.CharEditor
{
    /// <summary>
    /// Логика взаимодействия для BehaviourEditor.xaml
    /// </summary>
    public partial class BehaviorEditor : UserControl
    {
        public CharacterData source;


        public BehaviorEditor()
        {
            InitializeComponent();

        }


        public void setChar(CharacterData cd)
        {
            source = cd;
            behs.ItemsSource = source.behStates;
        }


        private void addClick(object sender, RoutedEventArgs e)
        {
            if(source!=null)
            {
                source.behStates.Add(LocationBehaviorStateData.New());
            }
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
