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
using Xceed.Wpf.AvalonDock.Layout;


namespace tgwEditor.Quests
{
    /// <summary>
    /// Логика взаимодействия для QuestsEditor.xaml
    /// </summary>
    public partial class QuestsEditor : UserControl, ILoadableWindow
    {
        public static QuestsEditor instance = null;

        public string _editingQuestID = "";
        public string editingQuestID
        {
            get
            {
                return _editingQuestID;
            }
            set
            {
                _editingQuestID = value;
                //var qd = sData.quests.Get(value);
                //if(qd != null)
                //{
                //    setEditingQuest
                //}
            }
        }

        public QuestsEditor()
        {
            InitializeComponent();
            instance = this;
        }

        public QuestData source;
        public LayoutAnchorable la = null;

        public void setEditingQuest(QuestData qd)
        {
            if (qd != null)
            {
                this.IsEnabled = true;
                DataContext = qd;
                source = qd;

                editingQuestID = qd.ID;

                vars.ItemsSource = qd.vars;

                dialogs.SetDialogDistrCollection(qd.dialogs);

                script.SetScriptFromKeyVal(source.Script);
                script.ScriptType = "quest";

                nameGrid.Children.Clear();
                var v = new TextDataField(qd.Name);
                nameGrid.Children.Add(v);

                if (la != null)
                {
                    var sr = (App.Current.MainWindow as MainWindow).SecondaryRight;
                    var i = sr.Children.IndexOf(la);
                    sr.SelectedContentIndex = i;
                }
            }
            else
            {
                this.IsEnabled = false;
            }
        }


        private void addPVal_Click(object sender, RoutedEventArgs e)
        {
            if (source != null)
            {
                source.vars.Add(KeyValDataPair.New("name", KeyValDataPair.VALUE_TYPE_STRING));
            }
        }

        public static LayoutAnchorable CreateNew()
        {
            LayoutAnchorable al = new LayoutAnchorable();
            var v = new QuestsEditor();
            al.Content = v;
            al.Title = "QuestsEditor";

            v.la = al;

            al.AutoHideMinWidth = al.FloatingWidth = v.MinWidth;

            al.CanClose = false;
            al.Closing += al_Closing;
            al.CanFloat = false;
            

            return al;
        }

        static void al_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public static QuestsEditor OpenNewInMainWindow()
        {
            var v = CreateNew();
            (App.Current.MainWindow as MainWindow).openWindowInPanel(v, MainWindow.MainWindowPanelType.SecondaryRight, true);

            return v.Content as QuestsEditor;

        }

        public void Loading()
        {
            setEditingQuest(sData.quests.Get(editingQuestID));
        }

        public void Saving()
        {
            
        }
    }
}
