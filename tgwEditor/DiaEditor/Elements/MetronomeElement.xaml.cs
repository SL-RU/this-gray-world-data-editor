using ICSharpCode.AvalonEdit.Document;
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
using System.Xml.Linq;
using tgwEditor.Elements;

namespace tgwEditor.DiaEditor
{
    /// <summary>
    /// Логика взаимодействия для DiaItem.xaml
    /// </summary>
    public partial class MetronomeElement : DiaProjectElement
    {

        public TextDocument doc;

        public MetronomeElement(ProjectScriptItemData prjD, GraphDiaEdit wd)
            : base(prjD, wd)
        {
            window = wd;
            prjData = prjD;
            InitializeComponent();
        }


        //Remove button
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            window.RemoveProjectElement(this);
        }
    }
}
