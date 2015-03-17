using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Элемент начала диалога
    /// </summary>
    public partial class DialogOrigin : DiaProjectElement
    {
        public DialogOriginData source;

        public SourceElement sr;
        public DialogOrigin(DialogOriginData src, ProjectScriptItemData prDat, GraphDiaEdit wnd)
            : base(prDat, wnd)
        {
            source = src;
            prjData = prDat;
            window = wnd;

            InitializeComponent();

            sr = new SourceElement(src.Link, wnd);
            linkGrid.Children.Add(sr);
            DataContext = source;
            linkF.DataContext = source.Link;

            src.Link.ValueChanged_Event += Link_ValueChanged_Event;

            linkID.Text = source.Link.Val;
            linkID.TextChanged += linkID_TextChanged;

            //context menu
            var c = new ContextMenu();
            var bu = new MenuItem();
            bu.Header = "Delete";
            bu.Click += bu_Click;
            c.Items.Add(bu);
            ContextMenu = c;
        }

        //delete button
        void bu_Click(object sender, RoutedEventArgs e)
        {
            window.RemoveProjectElement(this);
            if (sr.connection != null)
            {
                sr.connection.Delete();
            }
        }

        void linkID_TextChanged(object sender, TextChangedEventArgs e)
        {
            sr.ConnectTo(linkID.Text);
        }

        void Link_ValueChanged_Event(object sender, EventArgs e)
        {
            linkID.TextChanged -= linkID_TextChanged;

            linkID.Text = source.Link.Val.ToString();

            linkID.TextChanged += linkID_TextChanged;

        }

        private void copyID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(source.ID);
        }

    }
}
