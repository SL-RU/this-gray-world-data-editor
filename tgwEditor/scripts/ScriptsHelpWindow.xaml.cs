using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
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
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor.scripts
{
    /// <summary>
    /// Логика взаимодействия для ScriptsHelpWindow.xaml
    /// </summary>
    public partial class ScriptsHelpWindow : UserControl
    {
        public static string LuaAPIfile = "lua.txt";


        public ScriptsHelpWindow()
        {
            InitializeComponent();

            if (AppSettings.LuaAPIHelp != null)
            {
                ScriptsMethodsList lsm = null;

                foreach (var c in AppSettings.LuaAPIHelp)
                {
                    lsm = new ScriptsMethodsList(c.Key);
                    pan.Children.Add(lsm);
                    foreach (string s in c.Value)
                    {
                        lsm.Parse(s);
                    }
                }
            }
        }


        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutAnchorable CreateWindow()
        {
            LayoutAnchorable l = new LayoutAnchorable();
            l.Title = "ScriptsHelp";
            l.Content = new ScriptsHelpWindow();

            l.CanClose = false;
            l.CanFloat = false;
            
            return l;
        }
    }
}
