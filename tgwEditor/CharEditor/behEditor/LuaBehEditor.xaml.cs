using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

namespace tgwEditor.CharEditor.behEditor
{
    /// <summary>
    /// Логика взаимодействия для LuaEditor.xaml
    /// </summary>
    public partial class LuaBehEditor : UserControl
    {
        public KeyValDataPair source;


        public LuaBehEditor()
        {
            InitializeComponent();

            Random r = new Random();
            Background = new SolidColorBrush(Color.FromRgb((byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255)));

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            source = DataContext as KeyValDataPair;
            editor.SetScriptFromKeyVal(source);
            editor.ScriptType = "char_beh";
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (source.parent != null)
                (source.parent as LocationBehaviorStateData).handlers.Remove(source);
        }


    }
   
}
