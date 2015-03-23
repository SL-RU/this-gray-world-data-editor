using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Логика взаимодействия для VarsControl.xaml
    /// </summary>
    public partial class InventoryEditor : UserControl
    {
        public ObservableCollection<KeyValDataPair> source;
        public GraphDiaEdit window = null;

        public List<GoodCountDataField> goodsVars;

        public InventoryEditor()
        {
            InitializeComponent();

            goodsVars = new List<GoodCountDataField>();
        }



        /// <summary>
        /// Sets an source collection for the element
        /// </summary>
        /// <param name="col">New collection</param>
        /// <param name="types">Types of available varibles. Types: link, text, number, string, good.
        /// Write it and separate with comma. Like this: "link,text,good"</param>
        public void SetCollection(ObservableCollection<KeyValDataPair> col)
        {
            if (source != null)
                source.CollectionChanged -= source_CollectionChanged;
            varGrid.Children.Clear();
            source = col;
            foreach(var s in source)
            {
                addVarUI(s);
            }
            source.CollectionChanged += source_CollectionChanged;
        }

        void source_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (KeyValDataPair li in e.OldItems)
                {
                    //Removed items
                    foreach(var v in goodsVars)
                    {
                        if(v.source == li)
                        {
                            varGrid.Children.Remove(v);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValDataPair li in e.NewItems)
                {
                    //Added items
                    addVarUI(li);
                }
            }
        }

        void addVarUI(KeyValDataPair kv)
        {
            GoodCountDataField c = new GoodCountDataField(kv);
            c.SetValue(DockPanel.DockProperty, Dock.Top);
            varGrid.Children.Add(c);
            goodsVars.Add(c);

            #region ContextMenu
            ContextMenu m = new ContextMenu();

            MenuItem del = new MenuItem();
            del.Tag = c;
            del.Click += (x, y) =>
            {
                var k = (x as MenuItem).Tag as KeyStringDataField;
                source.Remove(k.source);
            };
            del.Header = "Delete";
            m.Items.Add(del);
            c.ContextMenu = m;
            #endregion

        }


        private void addVar_Click(object sender, RoutedEventArgs e)
        {
            source.Add(KeyValDataPair.New("", "inv"));
        }
    }
}
