using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class VarsControl : UserControl
    {
        public ObservableCollection<KeyValDataPair> source;
        public GraphDiaEdit window = null;

        public List<KeyStringDataField> stringVals;
        public List<KeyIntDataField> intVals;
        public List<ScriptLinkField> links;
        public List<KeyTextDataField> texts;
        public List<KeyGoodDataField> goods;
        public List<KeyImageDataField> images;
        // СДЕЛАТЬ НОРМАЛЬНОЕ НАСЛЕДОВАНИЕ! БЫДЛОКОДЕР!

        public VarsControl()
        {
            InitializeComponent();

            stringVals = new List<KeyStringDataField>();
            intVals = new List<KeyIntDataField>();
            links = new List<ScriptLinkField>();
            texts = new List<KeyTextDataField>();
            goods = new List<KeyGoodDataField>();
            images = new List<KeyImageDataField>();
        }


        void vars_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            List<KeyValDataPair> kvpl = new List<KeyValDataPair>();

            List<KeyStringDataField> ksd = new List<KeyStringDataField>();
            List<KeyIntDataField> kid = new List<KeyIntDataField>();
            List<ScriptLinkField> slf = new List<ScriptLinkField>();
            List<KeyTextDataField> ktd = new List<KeyTextDataField>();
            List<KeyImageDataField> kim = new List<KeyImageDataField>();

            //Find
            foreach (KeyStringDataField ks in stringVals)
            {
                if (!source.Contains(ks.source) && ks.source.ValueType == KeyValDataPair.VALUE_TYPE_STRING)
                {
                    ksd.Add(ks);
                }
                else
                {
                    kvpl.Add(ks.source);
                }
            }
            foreach (KeyIntDataField ks in intVals)
            {
                if (!source.Contains(ks.source) && ks.source.ValueType == KeyValDataPair.VALUE_TYPE_INT)
                {
                    kid.Add(ks);
                }
                else
                {
                    kvpl.Add(ks.source);
                }
            }
            foreach (ScriptLinkField ks in links)
            {
                if (!source.Contains(ks.source) && ks.source.ValueType == KeyValDataPair.VALUE_TYPE_SCRIPT_ID)
                {
                    slf.Add(ks);
                }
                else
                {
                    kvpl.Add(ks.source);
                }
            }
            foreach (KeyTextDataField ks in texts)
            {
                if (!source.Contains(ks.source) && ks.source.ValueType == KeyValDataPair.VALUE_TYPE_TEXTID)
                {
                    ktd.Add(ks);
                }
                else
                {
                    kvpl.Add(ks.source);
                }
            }
            foreach (KeyImageDataField ks in images)
            {
                if (!source.Contains(ks.source) && ks.source.ValueType == KeyValDataPair.VALUE_TYPE_IMAGE_PATH)
                {
                    kim.Add(ks);
                }
                else
                {
                    kvpl.Add(ks.source);
                }
            }

            //Rem
            foreach (KeyStringDataField u in ksd)
            {
                varGrid.Children.Remove(u);
                stringVals.Remove(u);
            }
            foreach (KeyIntDataField u in kid)
            {
                varGrid.Children.Remove(u);
                intVals.Remove(u);
            }
            foreach (ScriptLinkField u in slf)
            {
                varGrid.Children.Remove(u);
                links.Remove(u);
            }
            foreach (KeyTextDataField u in ktd)
            {
                varGrid.Children.Remove(u);
                texts.Remove(u);
            }
            foreach (KeyImageDataField u in kim)
            {
                varGrid.Children.Remove(u);
                images.Remove(u);
            }

            //Add
            foreach (KeyValDataPair kv in source)
            {
                if (!kvpl.Contains(kv))
                {
                    addVarUI(kv);
                }
            }
        }

        /// <summary>
        /// Sets an source collection for the element
        /// </summary>
        /// <param name="col">New collection</param>
        /// <param name="types">Types of available varibles. Types: link, text, number, string, good, image.
        /// Write it and separate with comma. Like this: "link,text,good,image"</param>
        public void SetCollection(ObservableCollection<KeyValDataPair> col, string types)
        {
            newVarType.Items.Clear();
            varGrid.Children.Clear();
            foreach (string s in types.Split(','))
            {
                newVarType.Items.Add(s);
            }
            newVarType.SelectedItem = types.Split(',')[0];

            if (source != null)
            {
                source.CollectionChanged -= vars_CollectionChanged;
            }

            source = col;

            foreach (KeyValDataPair kv in source)
            {
                addVarUI(kv);
            }
            source.CollectionChanged += vars_CollectionChanged;

        }

        void addVarUI(KeyValDataPair kv)
        {
            switch (kv.ValueType)
            {
                case KeyValDataPair.VALUE_TYPE_STRING: var uStr = new KeyStringDataField(kv);
                    uStr.SetValue(DockPanel.DockProperty, Dock.Top);
                    varGrid.Children.Add(uStr);
                    stringVals.Add(uStr);

                    #region ContextMenu
                    ContextMenu m = new ContextMenu();

                    MenuItem del = new MenuItem();
                    del.Tag = uStr;
                    del.Click += (x, y) =>
                    {
                        var k = (x as MenuItem).Tag as KeyStringDataField;
                        source.Remove(k.source);
                    };
                    del.Header = "Delete";
                    m.Items.Add(del);
                    uStr.ContextMenu = m;
                    #endregion
                    break;
                case KeyValDataPair.VALUE_TYPE_INT: var uInt = new KeyIntDataField(kv);
                    uInt.SetValue(DockPanel.DockProperty, Dock.Top);
                    varGrid.Children.Add(uInt);
                    intVals.Add(uInt);

                    #region ContextMenu
                    m = new ContextMenu();

                    del = new MenuItem();
                    del.Tag = uInt;
                    del.Click += (x, y) =>
                    {
                        var k = (x as MenuItem).Tag as KeyIntDataField;
                        source.Remove(k.source);
                    };
                    del.Header = "Delete";
                    m.Items.Add(del);
                    uInt.ContextMenu = m;
                    #endregion
                    break;
                case KeyValDataPair.VALUE_TYPE_SCRIPT_ID: var uLnk = new ScriptLinkField(kv, window);
                    uLnk.SetValue(DockPanel.DockProperty, Dock.Top);
                    varGrid.Children.Add(uLnk);
                    links.Add(uLnk);

                    #region ContextMenu
                    m = new ContextMenu();

                    del = new MenuItem();
                    del.Tag = uLnk;
                    del.Click += (x, y) =>
                    {
                        var k = (x as MenuItem).Tag as ScriptLinkField;
                        source.Remove(k.source);
                        if (k.sr.connection != null)
                            k.sr.connection.Delete();
                    };
                    del.Header = "Delete";
                    m.Items.Add(del);
                    uLnk.ContextMenu = m;
                    #endregion
                    break;
                case KeyValDataPair.VALUE_TYPE_TEXTID: var uTxt = new KeyTextDataField(kv, window);
                    uTxt.SetValue(DockPanel.DockProperty, Dock.Top);
                    varGrid.Children.Add(uTxt);
                    texts.Add(uTxt);

                    #region ContextMenu
                    m = new ContextMenu();

                    del = new MenuItem();
                    del.Tag = uTxt;
                    del.Click += (x, y) =>
                    {
                        var k = (x as MenuItem).Tag as KeyTextDataField;
                        source.Remove(k.source);
                    };
                    del.Header = "Delete";
                    m.Items.Add(del);
                    uTxt.ContextMenu = m;
                    #endregion
                    break;
                case KeyValDataPair.VALUE_TYPE_GOOD_ID:
                    var uGood = new KeyGoodDataField(kv);
                    uGood.SetValue(DockPanel.DockProperty, Dock.Top);
                    varGrid.Children.Add(uGood);
                    goods.Add(uGood);
                    #region ContextMenu
                    m = new ContextMenu();

                    del = new MenuItem();
                    del.Tag = uGood;
                    del.Click += (x, y) =>
                    {
                        var k = (x as MenuItem).Tag as KeyGoodDataField;
                        source.Remove(k.source);
                    };
                    del.Header = "Delete";
                    m.Items.Add(del);
                    uGood.ContextMenu = m;
                    #endregion
                    break;
                case KeyValDataPair.VALUE_TYPE_IMAGE_PATH:
                    var uImg = new KeyImageDataField(kv);
                    uImg.SetValue(DockPanel.DockProperty, Dock.Top);
                    varGrid.Children.Add(uImg);
                    images.Add(uImg);
                    #region ContextMenu
                    m = new ContextMenu();

                    del = new MenuItem();
                    del.Tag = uImg;
                    del.Click += (x, y) =>
                    {
                        var k = (x as MenuItem).Tag as KeyGoodDataField;
                        source.Remove(k.source);
                    };
                    del.Header = "Delete";
                    m.Items.Add(del);
                    uImg.ContextMenu = m;
                    #endregion
                    break;
            }
        }


        private void addVar_Click(object sender, RoutedEventArgs e)
        {
            switch (newVarType.SelectedItem as string)
            {
                case "link": source.Add(KeyValDataPair.New("link" + links.Count, KeyValDataPair.VALUE_TYPE_SCRIPT_ID)); break;
                case "text": source.Add(KeyValDataPair.New("txt" + texts.Count, KeyValDataPair.VALUE_TYPE_TEXTID)); break;
                case "number": source.Add(KeyValDataPair.New("n" + intVals.Count, KeyValDataPair.VALUE_TYPE_INT)); break;
                case "string": source.Add(KeyValDataPair.New("s" + stringVals.Count, KeyValDataPair.VALUE_TYPE_STRING)); break;
                case "good": source.Add(KeyValDataPair.New("good" + goods.Count, KeyValDataPair.VALUE_TYPE_GOOD_ID)); break;
                case "image": source.Add(KeyValDataPair.New("image" + images.Count, KeyValDataPair.VALUE_TYPE_IMAGE_PATH)); break;
            }
        }
    }

    public class AbstractDataField
    {
        public KeyValDataPair source
        {
            get;
            set;
        }
    }
}
