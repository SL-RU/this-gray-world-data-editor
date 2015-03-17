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


namespace tgwEditor.DiaEditor
{
    /// <summary>
    /// Логика взаимодействия для TargetElement.xaml
    /// </summary>
    public partial class TargetElement : UserControl
    {
        public string DataKey = "";
        public string type = "";

        public TargetElement()
        {
            InitializeComponent();
            mainEl.Background = new SolidColorBrush(Color.FromArgb(250, 255, 255, 1));

        }

        public GraphDiaEdit window;

        public List<ConnectionLine> connections = new List<ConnectionLine>();

        private void mainEl_MouseMove(object sender, MouseEventArgs e)
        {
            if (window.tool == GraphDiaEdit.TOOL_CONNECTION)
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(250, 0, 0, 255));
            }
            else
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
        }

        private void mainEl_MouseLeave(object sender, MouseEventArgs e)
        {
            mainEl.Background = new SolidColorBrush(Color.FromArgb(250, 255, 255, 1));
        }

        private void mainEl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (window.tool == GraphDiaEdit.TOOL_CONNECTION)
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(250, 0, 0, 255));
            }
            else
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
            }
        }

        public void remConnection(ConnectionLine c)
        {
            if (connections.Contains(c))
            {
                connections.Remove(c);
            }
        }

        private void mainEl_Click(object sender, RoutedEventArgs e)
        {
            if (window.tool == GraphDiaEdit.TOOL_CONNECTION)
            {
                SourceElement se = window.toolSender as SourceElement;
                se.ITargetAnswer(this);
                connections.Add(se.connection);
            }
        }
    }
}
