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
    /// Логика взаимодействия для KeyStringDataField.xaml
    /// </summary>
    public partial class AnswerDataField : UserControl
    {
        public AnswerData source;

        public SourceElement anchor;

        public AnswerDataField(AnswerData src, GraphDiaEdit wnd)
        {
            source = src;
            InitializeComponent();

            mainGrid.DataContext = src;
            anchor = new SourceElement(src.doScript, wnd);
            linkDock.Children.Add(anchor);


            src.doScript.ValueChanged_Event += Link_ValueChanged_Event;

            linkID.Text = source.doScript.Val;
            linkID.TextChanged += linkID_TextChanged;



            TextDataField td = new TextDataField(src.Text);
            text.Children.Add(td);
            DataContext = src;
        }


        void linkID_TextChanged(object sender, TextChangedEventArgs e)
        {
            anchor.ConnectTo(linkID.Text);
        }

        void Link_ValueChanged_Event(object sender, EventArgs e)
        {
            linkID.TextChanged -= linkID_TextChanged;

            linkID.Text = source.doScript.Val;

            linkID.TextChanged += linkID_TextChanged;

        }
    }
}
