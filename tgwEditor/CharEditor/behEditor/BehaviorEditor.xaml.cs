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
            //behs.ItemsSource = source.behStates;
            if(cd.behStates.Count() < 1)
            {
                var l = LocationBehaviorStateData.New();
                l.Location = "*";
                cd.behStates.Add(l);
                var key = KeyValDataPair.New("click", KeyValDataPair.VALUE_TYPE_SCRIPT_ID);
                ScriptData sd = ScriptData.New();
                key.Val = sd.ID;
                l.handlers.Add(key);
                sData.scripts.Add(sd);
            }
            var el = cd.behStates.Where(x => x.Location == "*").First();

            scr.SetScriptFromKeyVal(el.handlers.First());
        }


        private void addClick(object sender, RoutedEventArgs e)
        {
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
