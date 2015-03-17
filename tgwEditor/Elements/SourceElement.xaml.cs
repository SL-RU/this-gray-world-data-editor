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
using tgwEditor.DiaEditor;

namespace tgwEditor.Elements
{
    /// <summary>
    /// Логика взаимодействия для SourceElement.xaml
    /// </summary>
    public partial class SourceElement : UserControl
    {
        public SourceElement(KeyValDataPair src, GraphDiaEdit wnd)
        {
            InitializeComponent();
            kvData = src;
            window = wnd;

            if(src.Key != "")
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += dt_Tick;
                timer.Start();
            }
        }

        DispatcherTimer timer;

        public delegate void Connected(string id);
        public event Connected ConnectedEvent;



        void dt_Tick(object sender, EventArgs e)
        {
            var di = window.getExistingScript(kvData.Val);
            if (di != null)
            {
                ITargetAnswer(di.tAnch, true);
                (sender as DispatcherTimer).Stop();
            }
        }

        public void ConnectTo(string id)
        {
            kvData.Val = id;
            timer.Start();
        }

        public GraphDiaEdit window;

        public KeyValDataPair kvData = null;

        public ConnectionLine connection;

        public bool IFindConnection = false;

        public void ITargetAnswer(TargetElement te, bool load)
        {
            bool New = true;
            if (connection != null)
            {

                connection.Delete();

            }
            connection = new ConnectionLine(this, te, window.ConnectionGrid);
            if (!load)
            {
                window.tool = "";
                window.toolSender = false;
                if (New)
                {
                    kvData.Val = connection.targetElement.DataKey.ToString();
                    connecting = false;
                }
            }
            else
            {
                if (New)
                {
                    te.connections.Add(connection);
                }
                kvData.Val = te.DataKey;
            }
            if(ConnectedEvent != null)
            {
                if (New)
                    ConnectedEvent(connection.targetElement.DataKey);
            }
        }

        public void ITargetAnswer(TargetElement t)
        {
            ITargetAnswer(t, false);
        }

        public void remConnection()
        {
            connection = null;
            kvData.Val = "-1";
        }

        public bool connecting = false;

        private void mainEl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (window.tool == GraphDiaEdit.TOOL_CONNECTION || kvData == null || IFindConnection)
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
            else
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void mainEl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IFindConnection)
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            }
            else
            {
                mainEl.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            }
        }


        private void mainEl_Click(object sender, RoutedEventArgs e)
        {
            if (window.tool == "" && kvData != null)
            {
                window.tool = GraphDiaEdit.TOOL_CONNECTION;
                window.toolSender = this;
                mainEl.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                IFindConnection = true;
                connecting = true;
            }
            else
            {
                if (window.tool == GraphDiaEdit.TOOL_CONNECTION)
                {
                    if (window.toolSender == this)
                    {
                        connecting = false;
                        window.tool = "";
                        window.toolSender = null;
                    }
                    else
                    {
                        if(window.toolSender != null)
                        {
                            (window.toolSender as SourceElement).connecting = false;
                        }
                        window.tool = GraphDiaEdit.TOOL_CONNECTION;
                        window.toolSender = this;
                        mainEl.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                        IFindConnection = true;
                        connecting = true;
                    }
                }
            }
        }
    }
}
