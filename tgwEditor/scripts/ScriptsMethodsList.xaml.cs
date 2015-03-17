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
using tgwEditor.Elements;

namespace tgwEditor.scripts
{
    /// <summary>
    /// Логика взаимодействия для ScriptsMethodsList.xaml
    /// </summary>
    public partial class ScriptsMethodsList : UserControl
    {
        public class ItemsClass
        {
            public string _meth, _descr;
            public string meth
            {
                get { return _meth; }
                set { _meth = value; }
            }
            public string descr
            {
                get { return _descr; }
                set { _descr = value; }
            }
            public string Source = "";
        }

        ObservableCollection<ItemsClass> els = new ObservableCollection<ItemsClass>();

        public ScriptsMethodsList(string name)
        {
            InitializeComponent();
            list.ItemsSource = els;
            ex.Header = name;
        }

        /// <summary>
        /// Returns KeyValuePair with parsed description and method name. Where Key  - method, and Value - description
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <returns>Key  - method, and Value - description</returns>
        public static KeyValuePair<string, string> ParseAPIString(string s)
        {
            string d = s; ;
            string me = "", de = "";
            char lc = ' ';
            bool part = false;
            for (int i = 0; i <= d.Length - 1; i++)
            {
                if (d[i] != '-')
                {
                    if (part)
                    {
                        de += d[i].ToString();
                    }
                    else
                    {
                        me += d[i].ToString();
                    }

                    if (lc == '-')
                    {
                        if (part)
                        {
                            de += "-";
                        }
                        else
                        {
                            me += "-";
                        }
                    }
                    lc = d[i];
                }
                else
                {
                    if (lc == '-')
                    {
                        part = true;
                        lc = ' ';
                    }
                    else
                    {
                        lc = '-';
                    }
                }

            }
            return new KeyValuePair<string, string>(me, de);
        }

        public void Parse(string s)
        {
            var Source = ParseAPIString(s);


            var vars = LuaApiVarible.ParseList(Source.Key);
            string txt = Source.Key, func4descr = Source.Key;

            foreach (var vr in vars)
            {
                txt = txt.Replace(vr.Source, vr.VarType);
                func4descr = func4descr.Replace(vr.Source, vr.VarName);
            }

            els.Add(new ItemsClass() { meth = txt, descr = Source.Value, Source = s });
        }

        private void MethButtonClick(object sender, RoutedEventArgs e)
        {
            if (LuaEditor.lastEd != null)
            {
                ItemsClass ic = ((sender as Button).DataContext) as ItemsClass;

                var Source = ParseAPIString(ic.Source);
                var vars = LuaApiVarible.ParseList(Source.Key);


                string txt = Source.Key;
                int doned = 0;

                foreach (var v in vars)
                {
                    if (AppSettings.ShowInputBoxWindowOnLuaAutocomplete)
                    {
                        var inpBox = InputBoxWindow.CreateNew(Source.Value + "\n\n" + v.VarName + "  - " + v.VarType, "Input argument value", "");
                        inpBox.OnTextEnteredEvent += delegate(InputBoxWindow wnd, string t, bool canceled)
                        {
                            if (!canceled)
                            {
                                txt = txt.Replace(v.Source, t);
                            }
                            else
                            {
                                txt = txt.Replace(v.Source, v.VarName);
                            }
                            doned += 1;
                            if (doned == vars.Count)
                            {
                                LuaEditor.lastEd.doc.Insert(LuaEditor.lastEd.editor.CaretOffset, txt + "\n");
                            }
                        };
                    }
                    else
                    {
                        txt = txt.Replace(v.Source, v.VarName);
                        doned += 1;
                    }
                }

                if (doned == vars.Count)
                {
                    LuaEditor.lastEd.doc.Insert(LuaEditor.lastEd.editor.CaretOffset, txt + "\n");
                }

            }
        }
    }

    public class LuaApiVarible
    {
        public string VarName = "",
            VarType = "",
            Source = "";


        /// <summary>
        /// Parse varible from API help. 
        /// Varible must look like: $(var_name|type)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static LuaApiVarible Parse(string s)
        {
            string dt = s.Substring(2);
            dt = dt.Remove(dt.Length - 1);
            var v = dt.Split('|');

            return new LuaApiVarible()
            {
                VarName = v[0],
                VarType = v[1],
                Source = s
            };
        }

        public static List<LuaApiVarible> ParseList(string s)
        {
            List<LuaApiVarible> vars = new List<LuaApiVarible>();

            char last = ' ';
            int first = -1;

            for (int i = 0; i <= s.Length - 1; i++)
            {
                if (first == -1)
                {
                    if (last == '$' && s[i] == '(')
                    {
                        first = i - 1;
                        last = ' ';
                    }
                    else
                    {
                        last = s[i];
                    }
                }
                else
                {
                    if (s[i] == ')')
                    {
                        int len = i - first;
                        vars.Add(LuaApiVarible.Parse(s.Substring(first, len + 1)));
                        first = -1;
                    }
                }
            }

            return vars;
        }
    }
}
