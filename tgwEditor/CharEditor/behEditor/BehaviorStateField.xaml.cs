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
using System.Xml.Linq;
using System.Collections.ObjectModel;


namespace tgwEditor.CharEditor.behEditor
{
    /// <summary>
    /// Логика взаимодействия для BehaviorStateField.xaml
    /// </summary>
    public partial class BehaviorStateField : UserControl
    {
        public ObservableCollection<LocationBehaviorStateData> par;

        LocationBehaviorStateData source;
        public BehaviorStateField()
        {
            InitializeComponent();
            Random r =new Random();
            Bline.Fill = Bline.Stroke = new SolidColorBrush(Color.FromRgb((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255)));

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            source = DataContext as LocationBehaviorStateData;
            behs.ItemsSource = source.handlers;
        }



        private void add_Click(object sender, RoutedEventArgs e)
        {
            if (source != null)
            {
                var key = KeyValDataPair.New("click", KeyValDataPair.VALUE_TYPE_SCRIPT_ID);
                ScriptData sd = ScriptData.New();
                key.Val = sd.ID;
                source.handlers.Add(key);
                sData.scripts.Add(sd);
            }
            
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            if (source.parent != null)
                (source.parent as CharacterData).behStates.Remove(source);
        }


    }
}
