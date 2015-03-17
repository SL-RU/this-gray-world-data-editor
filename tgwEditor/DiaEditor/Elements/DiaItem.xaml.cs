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
    public partial class DiaItem : DiaProjectElement
    {
        public ScriptData source;


        public List<AnswerDataField> answers;


        public DiaItem(ScriptData src, ProjectScriptItemData prjD, GraphDiaEdit wd) : base(prjD, wd)
        {
            window = wd;
            source = src;
            prjData = prjD;
            InitializeComponent();


            tAnch.window = window;
            tAnch.DataKey = source.ID;

            source.answers.CollectionChanged += answers_CollectionChanged;

            SetPosition();


            answers = new List<AnswerDataField>();

            VarsC.SetCollection(source.vars, "link,text,good");
            VarsC.window = wd;

            this.DataContext = source;
            ScriptEditor.SetScriptSource(source);
            ScriptEditor.ScriptType = "dia";

            foreach (AnswerData ad in source.answers)
            {
                addAnswerUI(ad);
            }
        }

        void answers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            List<AnswerData> an = new List<AnswerData>();
            List<AnswerDataField> atod = new List<AnswerDataField>();

            //add
            foreach (AnswerDataField af in answers)
            {
                if (source.answers.Contains(af.source))
                {
                    an.Add(af.source);
                }
                else
                {
                    atod.Add(af);
                }
            }

            //rem
            foreach (AnswerDataField af in atod)
            {
                answersGrid.Children.Remove(af);
                answers.Remove(af);
            }

            //create
            foreach (AnswerData ad in source.answers)
            {
                if (!an.Contains(ad))
                {
                    addAnswerUI(ad);
                }
            }
        }

        void addAnswerUI(AnswerData ad)
        {
            var a = new AnswerDataField(ad, window);
            a.SetValue(DockPanel.DockProperty, Dock.Top);
            answersGrid.Children.Add(a);
            answers.Add(a);

            #region ContextMenu
            ContextMenu m = new ContextMenu();

            MenuItem del = new MenuItem();
            del.Tag = a;
            del.Click += (x, y) =>
            {
                var k = (x as MenuItem).Tag as AnswerDataField;
                source.answers.Remove(k.source);
                if (k.anchor.connection != null)
                    k.anchor.connection.Delete();
            };
            del.Header = "Delete";
            m.Items.Add(del);
            a.ContextMenu = m;
            #endregion
        }


        public void Delete()
        {
            window.RemoveProjectElement(this);
            for (int i = tAnch.connections.Count - 1; i >= 0; i--)
            {
                tAnch.connections[i].Delete();
            }
            foreach (AnswerDataField ad in answers)
            {
                if (ad.anchor.connection != null)
                    ad.anchor.connection.Delete();
            }

            foreach (ScriptLinkField ad in VarsC.links)
            {
                if (ad.sr.connection != null)
                    ad.sr.connection.Delete();
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }



        private void addAnswer_Click(object sender, RoutedEventArgs e)
        {
            source.answers.Add(AnswerData.New());

        }

        private void copyID_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(source.ID);

        }
    }
}
