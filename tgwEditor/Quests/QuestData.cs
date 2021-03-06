﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tgwEditor
{


    /// <summary>
    /// Информация Квеста
    /// </summary>
    public class QuestData : XmlAbstractSerializer
    {
        public const string XElementName = "quest";


        public override string ID
        {
            get { return source.Attribute("Qname").Value.ToString(); }
            set { source.Attribute("Qname").Value = value; }
        }

        public string Description
        {
            get { return source.Attribute("ed.descr").Value.ToString(); }
            set { source.Attribute("ed.descr").Value = value; }
        }

        public KeyValDataPair Script;


        public KeyValDataPair Name;

        public ObservableCollection<DialogDistributionItemData> dialogs;
        public ObservableCollection<KeyValDataPair> vars;


        public QuestData(XElement xe)
            : base(xe)
        {

            dialogs = new ObservableCollection<DialogDistributionItemData>();
            vars = new ObservableCollection<KeyValDataPair>();

            foreach (XElement x in source.Elements(DialogDistributionItemData.XElementName))
            {
                dialogs.Add(new DialogDistributionItemData(x));
            }

            foreach (XElement x in source.Elements(KeyValDataPair.XElementName))
            {
                var k = new KeyValDataPair(x);
                if (k.ValueType == KeyValDataPair.VALUE_TYPE_SCRIPT_ID)
                    Script = k;
                else if (k.ValueType == KeyValDataPair.VALUE_TYPE_TEXTID)
                    Name = k;
                else
                    vars.Add(k);

            }

            dialogs.CollectionChanged += loc_CollectionChanged;
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

        void loc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        public static QuestData New()
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("ed.descr", ""));
            nl.Add(new XAttribute("Qname", "EnterQuestID"));
            nl.Add(new XAttribute("ID", Guid.NewGuid().ToString()));
            KeyValDataPair name = KeyValDataPair.New("name", KeyValDataPair.VALUE_TYPE_TEXTID);
            nl.Add(name.source);
            KeyValDataPair kv = KeyValDataPair.New("script", KeyValDataPair.VALUE_TYPE_SCRIPT_ID);
            nl.Add(kv.source);
            ScriptData sd = ScriptData.New();
            sData.scripts.Add(sd);
            kv.Val = sd.ID.ToString();
            QuestData li = new QuestData(nl);
            return li;
        }
    }


    //Иформация ответа для квестов. Будет использоваться системой начала диалогов персонажей.
    public class DialogDistributionItemData : XmlAbstractSerializer, INotifyPropertyChanged
    {
        public const string XElementName = "DialogsDistributionItem";

        public string Characters
        {
            get { return source.Attribute("chars").Value.ToString(); }
            set { source.Attribute("chars").Value = value; }
        }

        public string Dialogs
        {
            get { return source.Attribute("dias").Value.ToString(); }
            set { source.Attribute("dias").Value = value; }
        }


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

        //Kostyl' dlya WPF binding
        public string AnswerFilter
        {
            get
            {
                return answer.Filter;
            }
            set
            {
                answer.Filter = value;
                NotifyPropertyChanged();
            }
        }

        public AnswerData answer;

        public DialogDistributionItemData(XElement xe)
            : base(xe)
        {


            foreach (XElement x in source.Elements(AnswerData.XElementName))
            {
                answer = new AnswerData(x);
            }
        }
        public static DialogDistributionItemData New()
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("chars", ""));
            nl.Add(new XAttribute("dias", ""));

            nl.Add(AnswerData.New().source);

            DialogDistributionItemData li = new DialogDistributionItemData(nl);

            return li;
        }
    }
}
