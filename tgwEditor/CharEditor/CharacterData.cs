using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tgwEditor
{

    /// <summary>
    /// sData для персонажа и его поведения.
    /// </summary>
    public partial class sData
    {
        public static void CHAR_EDITOR_INIT()
        {
            APPEARANCE_TEMPLATE = new Dictionary<string, Dictionary<string, List<string>>>();
            SPEC_TEMPLATE = new List<string>();

            if (File.Exists("charTemplate.txt"))
            {
                StreamReader sr = new StreamReader("charTemplate.txt");
                string s = "";
                s = sr.ReadLine();
                while (s != "[app]")
                {
                    if (!s.StartsWith("["))
                        SPEC_TEMPLATE.Add(s);
                    s = sr.ReadLine();
                }
                string type = "", part = "";
                while (s != "" && s != null)
                {
                    if (s.StartsWith("-"))
                    {
                        part = s.Substring(1);
                        APPEARANCE_TEMPLATE[type].Add(part, new List<string>());
                    }
                    else if (s.StartsWith("="))
                    {
                        type = s.Substring(1);
                        APPEARANCE_TEMPLATE.Add(type, new Dictionary<string, List<string>>());
                    }
                    else
                    {
                        if (APPEARANCE_TEMPLATE.ContainsKey(type))
                        {
                            if (APPEARANCE_TEMPLATE[type].ContainsKey(part))
                            {
                                APPEARANCE_TEMPLATE[type][part].Add(s);
                            }
                        }
                    }
                    s = sr.ReadLine();
                }
                sr.Close();
            }
        }
        public static Dictionary<string, Dictionary<string, List<string>>> APPEARANCE_TEMPLATE;
        public static List<string> SPEC_TEMPLATE;


    }

    /// <summary>
    /// Информация персонажа
    /// </summary>
    public class CharacterData : XmlAbstractSerializer
    {
        public const string XElementName = "char";

        public string Tags
        {
            get { return source.Attribute("tags").Value.ToString(); }
            set { source.Attribute("tags").Value = value; }
        }

        public string Type
        {
            get { return (source.Attribute("type").Value.ToString()); }
            set { source.Attribute("type").Value = value; }
        }

        public override string ID
        {
            get { return source.Attribute("name").Value.ToString(); }
            set { source.Attribute("name").Value = value; }
        }

        public bool Instantiate
        {
            get
            {
                return source.Attribute("instantiate").Value.ToString() == "1";
            }
            set
            {
                source.Attribute("instantiate").Value = value ? "1" : "0";
            }
        }

        public KeyValDataPair RealName
        {
            get;
            set;
        }


        public ObservableCollection<KeyValDataPair> spec, kn, appearance;
        public ObservableCollection<LocationBehaviorStateData> behStates;
        public ObservableCollection<DialogDistributionItemData> dialogs;
        public ObservableCollection<KeyValDataPair> inventory;

        public CharacterData(XElement xe)
            : base(xe)
        {
            RealName = new KeyValDataPair(xe.Elements(KeyValDataPair.XElementName).Where(x => x.Attribute("valType").Value == KeyValDataPair.VALUE_TYPE_TEXTID).First());

            spec = new ObservableCollection<KeyValDataPair>();
            kn = new ObservableCollection<KeyValDataPair>();
            appearance = new ObservableCollection<KeyValDataPair>();
            inventory = new ObservableCollection<KeyValDataPair>();
            behStates = new ObservableCollection<LocationBehaviorStateData>();
            dialogs = new ObservableCollection<DialogDistributionItemData>();

            foreach (XElement kx in xe.Elements("key"))
            {
                switch (kx.Attribute("valType").Value)
                {
                    case "app":
                        appearance.Add(new KeyValDataPair(kx));
                        break;
                    case KeyValDataPair.VALUE_TYPE_INT:
                        spec.Add(new KeyValDataPair(kx));
                        break;
                    case KeyValDataPair.VALUE_TYPE_STRING:
                        kn.Add(new KeyValDataPair(kx));
                        break;
                    case "inv":
                        inventory.Add(new KeyValDataPair(kx));
                        break;
                }
            }

            foreach (XElement x in source.Elements(DialogDistributionItemData.XElementName))
            {
                dialogs.Add(new DialogDistributionItemData(x));
            }

            foreach (XElement kx in xe.Elements(LocationBehaviorStateData.XElementName))
            {
                behStates.Add(new LocationBehaviorStateData(kx) { parent = this });
            }

            appearance.CollectionChanged += appearance_CollectionChanged;
            kn.CollectionChanged += kn_CollectionChanged;
            spec.CollectionChanged += spec_CollectionChanged;
            behStates.CollectionChanged += behStates_CollectionChanged;
            dialogs.CollectionChanged += dialogs_CollectionChanged;
            inventory.CollectionChanged += inventory_CollectionChanged;
        }

        void inventory_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        void dialogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DialogDistributionItemData li in e.OldItems)
                {
                    //Removed items
                    source.Elements().Where(x => x == li.source).Remove();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DialogDistributionItemData li in e.NewItems)
                {
                    //Added items
                    source.AddFirst(li.source);
                }
            }
        }


        void appearance_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        void kn_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        void spec_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        void behStates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (LocationBehaviorStateData li in e.OldItems)
                {
                    //Removed items
                    source.Elements().Where(x => x == li.source).Remove();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (LocationBehaviorStateData li in e.NewItems)
                {
                    //Added items
                    source.AddFirst(li.source);

                    li.parent = this;
                }
            }
        }



        #region Static
        /// <summary>
        /// Create new this class
        /// </summary>
        /// <param name="key">ID</param>
        /// <param name="valType">Type of value</param>
        /// <returns></returns>
        public static CharacterData New(string name)
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("type", "pony"));
            nl.Add(new XAttribute("tags", ""));
            nl.Add(new XAttribute("name", name));
            nl.Add(new XAttribute("ID", Guid.NewGuid().ToString()));

            nl.Add(new XAttribute("instantiate", "1"));
            nl.Add(KeyValDataPair.New("realName", KeyValDataPair.VALUE_TYPE_TEXTID).source);

            CharacterData li = new CharacterData(nl);
            if (sData.SPEC_TEMPLATE != null)
            {
                foreach (string t in sData.SPEC_TEMPLATE)
                {
                    li.spec.Add(KeyValDataPair.New(t, KeyValDataPair.VALUE_TYPE_INT));
                }
            }

            return li;
        }


        public static bool CheckAvailibility(string name, CharacterData cur)
        {
            //sData.LOG("Checking name " + name + " availibility");
            if (sData.characters.Collection.Count != 0)
            {
                foreach (CharacterData sd in sData.characters.Collection)
                {
                    if (name == sd.ID && cur != sd)
                    {
                        sData.LOG("Checking name " + name + " availibility\nUNAVAiLIBLE");
                        return false;
                    }
                }
            }
            //sData.LOG("Availible");

            return true;
        }
        public static bool CheckAvailibility(string name)
        {
            return CheckAvailibility(name, null);
        }
        #endregion
    }

    /// <summary>
    /// Информация состояния поведения
    /// </summary>
    public class LocationBehaviorStateData : XmlAbstractSerializer
    {
        public const string XElementName = "beh";

        public object parent = null;


        public string Tags
        {
            get { return source.Attribute("tags").Value.ToString(); }
            set { source.Attribute("tags").Value = value; }
        }

        public string Location
        {
            get { return source.Attribute("location").Value.ToString(); }
            set { source.Attribute("location").Value = value; }
        }


        public ObservableCollection<KeyValDataPair> handlers;

        public LocationBehaviorStateData(XElement xe)
            : base(xe)
        {

            handlers = new ObservableCollection<KeyValDataPair>();

            foreach (XElement x in source.Elements())
            {
                handlers.Add(new KeyValDataPair(x) { parent = this });
            }

            handlers.CollectionChanged += loc_CollectionChanged;
        }

        void loc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    li.parent = this;
                }
            }
        }

        public static LocationBehaviorStateData New()
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("tags", ""));
            nl.Add(new XAttribute("location", "*"));

            LocationBehaviorStateData li = new LocationBehaviorStateData(nl);

            return li;
        }
    }
}
