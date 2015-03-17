using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tgwEditor
{
    /// <summary>
    /// Информация Квеста
    /// </summary>
    public class GoodsData : XmlAbstractSerializer
    {
        public const string XElementName = "good";


        public override string ID
        {
            get { return source.Attribute("name").Value.ToString(); }
            set { source.Attribute("name").Value = value; }
        }

        public ObservableCollection<KeyValDataPair> vars
        {
            get;
            set;
        }

        public GoodsData(XElement xe)
            : base(xe)
        {


            vars = new ObservableCollection<KeyValDataPair>();



            foreach (XElement x in source.Elements(KeyValDataPair.XElementName))
            {
                var k = new KeyValDataPair(x);

                vars.Add(k);

            }

            vars.CollectionChanged += vars_CollectionChanged;
        }

        void vars_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (KeyValDataPair li in e.OldItems)
                {
                    //Removed items
                    source.Elements().Where(x => x == li.source).Remove();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValDataPair li in e.NewItems)
                {
                    //Added items
                    source.AddFirst(li.source);
                }
            }
        }


        public static GoodsData New()
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("name", "EnterItemID"));
            nl.Add(new XAttribute("ID", Guid.NewGuid().ToString()));


            GoodsData li = new GoodsData(nl);

            return li;
        }
    }
}
