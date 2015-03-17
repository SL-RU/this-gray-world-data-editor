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
    /// Логика взаимодействия для ScriptLinkField.xaml
    /// </summary>
    public partial class ScriptLinkField : UserControl
    {
        public KeyValDataPair source;
        public SourceElement sr;
        GraphDiaEdit window;
        public ScriptLinkField(KeyValDataPair src, GraphDiaEdit wnd)
        {
            source = src;
            window = wnd;
            InitializeComponent();
            main.DataContext = src;

            sr = new SourceElement(src, wnd);
            linkDock.Children.Add(sr);

            sr.ConnectedEvent += sr_ConnectedEvent;
            targetText.TextChanged += targetText_TextChanged;
        }

        void targetText_TextChanged(object sender, TextChangedEventArgs e)
        {
            sr.ConnectTo(targetText.Text);
        }

        void sr_ConnectedEvent(string id)
        {
            targetText.TextChanged -= targetText_TextChanged;

            targetText.Text = id.ToString();

            targetText.TextChanged += targetText_TextChanged;

        }
    }
}
