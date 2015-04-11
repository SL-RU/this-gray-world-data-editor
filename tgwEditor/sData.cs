using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using tgwEditor.DiaEditor;

namespace tgwEditor
{

    /// <summary>
    /// sDATA 
    /// Texts
    /// Scripts
    /// DialogOrigins
    /// </summary>
    public partial class sData
    {
        public static XmlDBController<ScriptData> scripts;
        public static XmlDBController<DialogOriginData> dialogOrigins;
        public static XmlDBController<TextData> texts;
        public static string AddNewText()
        {
            TextData td = TextData.New("");
            texts.Add(td);
            return td.ID;
        }
        public static XmlDBController<GoodsData> goods;
        public static XmlDBController<QuestData> quests;
        public static XmlDBController<CharacterData> characters;
        public static XmlDBController<KeyValDataPair> global;


        public static string ScriptsFilePath = "scr.xml",
            TextFilePath = "txt.xml",
            CharacterFilePath = "char.xml",
            QuestsFilePath = "quests.xml",
            GoodsFilePath = "goods.xml",
            DialogOriginsPath = "dia.xml",
            GlobalFilePath = "global.xml";

        public static string path
        {
            get
            {
                return AppSettings.FolderPath;
            }
            set
            {
                AppSettings.FolderPath = value;
            }
        }

        public static void Init()
        {
            log = new ObservableCollection<string>();

            LOG("--------------------------PROGRAMM STARTED--------------------------");



            scripts = new XmlDBController<ScriptData>(ScriptsFilePath, ScriptData.XElementName);
            dialogOrigins = new XmlDBController<DialogOriginData>(DialogOriginsPath, DialogOriginData.XElementName);
            texts = new XmlDBController<TextData>(TextFilePath, TextData.XElementName);
            goods = new XmlDBController<GoodsData>(GoodsFilePath, GoodsData.XElementName);
            quests = new XmlDBController<QuestData>(QuestsFilePath, QuestData.XElementName);
            characters = new XmlDBController<CharacterData>(CharacterFilePath, CharacterData.XElementName);
            global = new XmlDBController<KeyValDataPair>(GlobalFilePath, KeyValDataPair.XElementName);

            DispatcherTimer autoSaveTimer = new DispatcherTimer();
            autoSaveTimer.Interval = TimeSpan.FromMinutes(1);
            autoSaveTimer.Tick += autoSaveTimer_Tick;
            autoSaveTimer.Start();

            CHAR_EDITOR_INIT();
        }

        static void autoSaveTimer_Tick(object sender, EventArgs e)
        {
            AutoSave();
        }


        public static void AutoSave()
        {
            LOG("Autosaving " + path + " begin");
            string cPath = path;
            path = "tmp\\" + DateTime.Now.Ticks.ToString() + "\\";
            try
            {
                if (!Directory.Exists("tmp"))
                {
                    Directory.CreateDirectory("tmp");

                }

                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + "\\dia\\");
                Directory.CreateDirectory(path + "\\imges\\");
                Directory.CreateDirectory(path + "\\audio\\");

                Save();

                for (int i = 0; i < GraphDiaEdit.openedProjects.Count; i++)
                {
                    var p = GraphDiaEdit.openedProjects[i];
                    p.SaveProject();
                }

                LOG("Autosaving done");

            }
            catch (Exception ex)
            {
                LOG_WARNING("Autosaving failed! " + ex.ToString());
            }
            path = cPath;


        }
        public static void InitDirectory(string dir)
        {
            sData.LOG("Init project in folder " + dir);
            if (!dir.EndsWith("\\"))
            {
                dir += '\\';
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!Directory.Exists(dir + "dia\\"))
            {
                Directory.CreateDirectory(dir + "dia\\");
            }
            if (!Directory.Exists(dir + "imges\\"))
            {
                Directory.CreateDirectory(dir + "imges\\");
            }
            if (!Directory.Exists(dir + "audio\\"))
            {
                Directory.CreateDirectory(dir + "audio\\");
            }
            sData.LOG("Init complete");
        }
        public static void MergeToNewDirectory(string newDir)
        {
            sData.LOG("Merging begin. From " + path + " to " + newDir);
            // Get the subdirectories for the specified directory.

            AppSettings.DirectoryCopy(path, newDir, true);

            sData.LOG("Merging complete!");
        }


        public static void Save()
        {
            try
            {
                LOG("Starting saving everything/s/");

                dialogOrigins.Save();
                scripts.Save();
                texts.Save();
                characters.Save();
                quests.Save();
                goods.Save();
                global.Save();

                if (MainWindow.instance != null)
                {
                    MainWindow.instance.Saving();
                }

                LOG("Everything saved/e/");
            }
            catch (Exception ex)
            {
                LOG_WARNING("Error on savisn sData " + ex.ToString());
            }
        }


        public static void Load()
        {
            LOG("Starting loading everything/s/");

            scripts.Load();

            texts.Load();

            dialogOrigins.Load();

            characters.Load();

            quests.Load();

            goods.Load();

            global.Load();

            if (MainWindow.instance != null)
            {
                MainWindow.instance.Loading();
            }

            LOG("Everything loaded/e/");

        }

        //I have no idea wtf is it 0_0
        #region TransWindowActivity
        public delegate void sendedScriptHandler(string scriptID);
        static public event sendedScriptHandler sebdedScriptEvent;
        public static void sendScript(string scriptID)
        {
            if (sData.sebdedScriptEvent != null)
                sData.sebdedScriptEvent(scriptID);
        }
        #endregion

        public static ObservableCollection<string> log;
        public static void LOG(string text)
        {
            log.Add((text));
            File.AppendAllText("log.txt", "[" + DateTime.Now.ToString() + "]: " + text + "\n");
        }

        public static void LOG_WARNING(string text)
        {
            text = ("WARNING!!! :\"" + text + ":\"");
            log.Add(text);
            File.AppendAllText("log.txt", "[" + DateTime.Now.ToString() + "]: " + text + "\n");

        }
    }

    public class XmlDBController<T> where T : XmlAbstractSerializer
    {
        public ObservableCollection<T> Collection;
        public XDocument DBFile;
        public string FileName = "x.xml";
        public string XElementName = "";
        public XmlDBController(string dbPath, string xElementName)
        {
            FileName = dbPath;
            XElementName = xElementName;
            Collection = new ObservableCollection<T>();
            DBFile = new XDocument();
            DBFile.Add(new XElement(XElementName + "s"));
            Collection.CollectionChanged += Collection_CollectionChanged;
            sData.LOG(XElementName + "s DB inited");
        }

        void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (T li in e.OldItems)
                {
                    //Removed items
                    DBFile.Root.Elements().Where(x => x == li.source).Remove();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (T li in e.NewItems)
                {
                    //Added items
                    DBFile.Root.AddFirst(li.source);
                }
            }
        }

        public bool isLoading = false;
        public void Load()
        {
            sData.LOG("Loading " + XElementName + "s from " + sData.path + FileName);
            if (File.Exists(sData.path + FileName))
            {
                isLoading = true;


                Collection.CollectionChanged -= Collection_CollectionChanged;
                Collection.Clear();


                DBFile = XDocument.Load(sData.path + FileName);
                foreach (XElement xe in DBFile.Root.Elements(XElementName))
                {
                    Add((T)Activator.CreateInstance(typeof(T), xe));
                }
                Collection.CollectionChanged += Collection_CollectionChanged;
                isLoading = false;

                sData.LOG("DONE!");

            }
            else
            {
                sData.LOG_WARNING(XElementName.ToUpper() + "S DB " + sData.path + FileName + " NOT EXISTED!!!");
            }
        }
        public void Save()
        {
            sData.LOG("Saving " + XElementName + "s to " + sData.path + FileName);
            DBFile.Save(sData.path + FileName);
            sData.LOG("DONE!");
        }
        public T Get(string ID)
        {
            foreach (T sc in Collection)
            {
                if (sc.ID == ID)
                    return sc;
            }
            return null;
        }
        public T Add(T el)
        {
            Collection.Add(el);
            return el;
        }
    }

    public class XmlAbstractSerializer
    {
        public XElement source;
        public virtual string ID
        {
            get;
            set;
        }

        public XmlAbstractSerializer(XElement xe)
        {
            source = xe;
        }


    }

    /// <summary>
    /// SCRIPT DATA
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// </summary>
    public class ScriptData : XmlAbstractSerializer
    {
        public const string XElementName = "script";

        public string Tags
        {
            get { return source.Attribute("tags").Value.ToString(); }
            set { source.Attribute("tags").Value = value; }
        }

        private string _id = null;
        public override string ID
        {
            get
            {
                if (_id != null)
                    return _id;
                else
                    return (_id = source.Attribute("ID").Value.ToString());
            }
            set { source.Attribute("ID").Value = value; }
        }

        public string Script
        {
            get { return source.Attribute("script").Value.ToString(); }
            set { source.Attribute("script").Value = value; }
        }


        public ObservableCollection<KeyValDataPair> vars;
        public ObservableCollection<AnswerData> answers;

        public ScriptData(XElement xe)
            : base(xe)
        {

            vars = new ObservableCollection<KeyValDataPair>();

            answers = new ObservableCollection<AnswerData>();


            foreach (XElement e in xe.Elements(KeyValDataPair.XElementName))
            {
                vars.Add(new KeyValDataPair(e));
            }
            foreach (XElement e in xe.Elements(AnswerData.XElementName))
            {
                answers.Add(new AnswerData(e));
            }

            vars.CollectionChanged += vars_CollectionChanged;

            answers.CollectionChanged += answers_CollectionChanged;
        }

        void answers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (AnswerData li in e.OldItems)
                {
                    //Removed items
                    source.Elements().Where(x => x == li.source).Remove();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AnswerData li in e.NewItems)
                {
                    //Added items
                    source.AddFirst(li.source);
                }
            }
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

        #region Static
        /// <summary>
        /// Create new this class
        /// </summary>
        /// <param name="key">ID</param>
        /// <param name="valType">Type of value</param>
        /// <returns></returns>
        public static ScriptData New()
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("ID", getNewID()));
            nl.Add(new XAttribute("tags", ""));

            nl.Add(new XAttribute("script", ""));
            ScriptData li = new ScriptData(nl);
            return li;
        }


        public static string getNewID()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion
    }



    // /////////////////////////////////////////////////////////////////////
    // KeyValuePair
    public class KeyValDataPair : XmlAbstractSerializer
    {
        public const string XElementName = "key",
            VALUE_TYPE_INT = "int",
            VALUE_TYPE_STRING = "string",
            VALUE_TYPE_TEXTID = "textID",
            VALUE_TYPE_SCRIPT_ID = "scriptID",
            VALUE_TYPE_GOOD_ID = "goodID",
            VALUE_TYPE_IMAGE_PATH = "img";
        //"inv" - for inventory
        //"app" - for appearance 

        public object parent = null;


        public string Key
        {
            get { return source.Attribute("key").Value.ToString(); }
            set
            {
                source.Attribute("key").Value = value;
                if (ValueChanged_Event != null)
                {
                    ValueChanged_Event(this, null);
                }
            }
        }

        public event EventHandler ValueChanged_Event;
        public string Val
        {
            get { return source.Attribute("val").Value.ToString(); }
            set
            {
                if (value != null)
                {
                    source.Attribute("val").Value = value;
                    if (ValueChanged_Event != null)
                    {
                        ValueChanged_Event(this, null);
                    }
                }
            }
        }

        public KeyValDataPair(XElement xe)
            : base(xe)
        {

        }

        public string ValueType
        {
            get { return source.Attribute("valType").Value.ToString(); }
            set { source.Attribute("valType").Value = value; }
        }


        /// <summary>
        /// Create new this class
        /// </summary>
        /// <param name="key">ID</param>
        /// <param name="valType">Type of value</param>
        /// <returns></returns>
        public static KeyValDataPair New(string key, string valType)
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("key", key));
            switch (valType)
            {
                case VALUE_TYPE_STRING:
                    nl.Add(new XAttribute("val", ""));
                    break;
                case VALUE_TYPE_INT:
                    nl.Add(new XAttribute("val", "0"));
                    break;
                case VALUE_TYPE_SCRIPT_ID:
                    nl.Add(new XAttribute("val", "-1"));
                    break;
                case VALUE_TYPE_TEXTID:
                    nl.Add(new XAttribute("val", sData.AddNewText()));
                    break;
                default:
                    nl.Add(new XAttribute("val", ""));
                    break;
            }
            nl.Add(new XAttribute("valType", valType));
            nl.Add(new XAttribute("ID", Guid.NewGuid().ToString()));
            KeyValDataPair li = new KeyValDataPair(nl);
            return li;
        }
    }


    // /////////////////////////////////////////////////////////////////////
    // Text
    public class TextData : XmlAbstractSerializer, INotifyPropertyChanged
    {
        public const string XElementName = "text";

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _ID = "";
        public override string ID
        {
            get
            {
                if (_ID == "")
                {
                    _ID = source.Attribute("ID").Value;
                }
                return _ID;
            }
            set
            {
                //source.Attribute("ID").Value = value.ToString();
            }
        }

        public string Text
        {
            get { return source.Attribute("text").Value.ToString(); }
            set
            {
                source.Attribute("text").Value = value;
                NotifyPropertyChanged();
            }
        }

        public string Sound
        {
            get { return source.Attribute("sound").Value.ToString(); }
            set
            {
                source.Attribute("sound").Value = value;
                NotifyPropertyChanged();
            }
        }

        public TextData(XElement xe)
            : base(xe)
        {
        }

        /// <summary>
        /// Create new this class
        /// </summary>
        /// <param name="key">ID</param>
        /// <returns></returns>
        public static TextData New(string text)
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("ID", getNewID()));
            nl.Add(new XAttribute("text", text));
            nl.Add(new XAttribute("sound", text));
            TextData li = new TextData(nl);
            return li;
        }

        public static int lastID = -1;
        public static string getNewID()
        {
            return Guid.NewGuid().ToString();
        }

    }



    // /////////////////////////////////////////////////////////////////////
    // ANSWER
    public class AnswerData : XmlAbstractSerializer
    {
        public const string XElementName = "answer";

        public string Filter
        {
            get { return source.Attribute("filter").Value.ToString(); }
            set { source.Attribute("filter").Value = value; }
        }

        public string Description
        {
            get { return source.Attribute("ed.decscr").Value.ToString(); }
            set { source.Attribute("ed.decscr").Value = value; }
        }

        public KeyValDataPair doScript, Text;


        public AnswerData(XElement xe)
            : base(xe)
        {
            doScript = new KeyValDataPair(xe.Elements(KeyValDataPair.XElementName).Where(x => x.Attribute("key").Value == "script").First());
            Text = new KeyValDataPair(xe.Elements(KeyValDataPair.XElementName).Where(x => x.Attribute("key").Value == "text").First());
        }

        /// <summary>
        /// Create new this class
        /// </summary>
        /// <param name="key">ID</param>
        /// <param name="valType">Type of value</param>
        /// <returns></returns>
        public static AnswerData New()
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("filter", "true"));
            nl.Add(new XAttribute("ed.decscr", ""));

            nl.Add(KeyValDataPair.New("script", KeyValDataPair.VALUE_TYPE_SCRIPT_ID).source);
            nl.Add(KeyValDataPair.New("text", KeyValDataPair.VALUE_TYPE_TEXTID).source);


            AnswerData li = new AnswerData(nl);
            return li;
        }
    }


    public class DialogOriginData : XmlAbstractSerializer
    {
        public const string XElementName = "DiaOrigin";

        private string _id = null;

        public override string ID
        {
            get
            {
                if (_id != null)
                    return _id;
                else
                    return (_id = source.Attribute("ID").Value.ToString());
            }
            set
            {
                //source.Attribute("ID").Value = value.ToString();
            }
        }

        public string Description
        {
            get { return source.Attribute("ed.decscr").Value.ToString(); }
            set { source.Attribute("ed.decscr").Value = value; }
        }

        public KeyValDataPair Link;

        public DialogOriginData(XElement xe)
            : base(xe)
        {
            Link = new KeyValDataPair(xe.Element("key"));
        }

        /// <summary>
        /// Create new this class
        /// </summary>
        /// <param name="key">ID</param>
        /// <returns></returns>
        public static DialogOriginData New()
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("ID", getNewID()));
            nl.Add(new XAttribute("ed.decscr", ""));
            nl.Add(KeyValDataPair.New("link", KeyValDataPair.VALUE_TYPE_SCRIPT_ID).source);
            DialogOriginData li = new DialogOriginData(nl);
            return li;
        }

        public static string getNewID()
        {
            return Guid.NewGuid().ToString();
        }
    }

}


