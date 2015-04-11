using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using tgwEditor.scripts;

namespace tgwEditor.Elements
{
    /// <summary>
    /// Логика взаимодействия для LuaEditor.xaml
    /// </summary>
    public partial class LuaEditor : UserControl
    {
        public TextDocument doc;
        public ScriptData source;
        public string ScriptType = "*";

        /// <summary>
        /// Currently focused editor.
        /// </summary>
        public static LuaEditor lastEd = null;


        public LuaEditor()
        {

            doc = new TextDocument();
            InitializeComponent();
            editor.Document = doc;
            doc.Text = "NUUUUUUUUUUUUULLLLLLLLLLLLLL!!!!! ERRROOOOOOOOOOOOOORRRRRRRRRRRRR";
            doc.Changed += doc_Changed;
            editor.SyntaxHighlighting = HighlightingLoader.Load(new XmlTextReader(new StringReader(tgwEditor.Properties.Resources.LuaHighLighting)), HighlightingManager.Instance);

            focusedIndicator.Visibility = System.Windows.Visibility.Collapsed;

            editor.TextArea.TextEntered += TextArea_TextEntered;
            editor.TextArea.TextEntering += TextArea_TextEntering;
        }


        CompletionWindow completionWindow;
        void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0])
                    && e.Text[0] != ':'
                    && e.Text[0] != '='
                    && e.Text[0] != '('
                    && e.Text[0] != ')'
                    && e.Text[0] != ';'
                    && e.Text[0] != '\"'
                    && e.Text[0] != '\'')
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (completionWindow == null)
            {
                if (doc != null && AppSettings.LuaAPIHelp != null)
                {
                    var Line = doc.GetLineByOffset(editor.CaretOffset);
                    string lineText = doc.GetText(Line.Offset, Line.Length);

                    // Open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(editor.TextArea);
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

                    if (!lineText.Contains("(") && !lineText.Contains(".") && !lineText.Contains(":") && !lineText.Contains("="))
                    {

                        //Add global completion data
                        if (AppSettings.LuaAPIHelp.ContainsKey("*"))
                        {
                            foreach (var s in AppSettings.LuaAPIHelp["*"])
                            {
                                data.Add(new LuaCompletionData(s));
                            }
                        }
                        //add class methods
                        if (AppSettings.LuaAPIHelp.ContainsKey(":"))
                        {
                            foreach (var s in AppSettings.LuaAPIHelp[":"])
                            {
                                data.Add(new LuaCompletionData(s));
                            }
                        }

                        //add class fields
                        if (AppSettings.LuaAPIHelp.ContainsKey("."))
                        {
                            foreach (var s in AppSettings.LuaAPIHelp["."])
                            {
                                data.Add(new LuaCompletionData(s));
                            }
                        }

                        //Add local completion data
                        if (AppSettings.LuaAPIHelp.ContainsKey(ScriptType) && ScriptType != "*")
                        {
                            foreach (var s in AppSettings.LuaAPIHelp[ScriptType])
                            {
                                data.Add(new LuaCompletionData(s));
                            }
                        }
                        completionWindow.Show();
                        completionWindow.Closed += delegate
                        {
                            completionWindow = null;
                        };
                    }
                }
            }
        }

        public void SetScriptSource(ScriptData sd)
        {
            if (sd != null)
            {
                source = sd;
                doc.Text = source.Script;
            }
            else
            {
                source = null;
                doc.Text = "NULL! \nThere is not such script!!!";
            }
        }
        public void SetScriptFromKeyVal(KeyValDataPair kv)
        {
            ScriptData sd = sData.scripts.Get(kv.Val);
            SetScriptSource(sd);
        }

        void doc_Changed(object sender, DocumentChangeEventArgs e)
        {
        }

        private void editor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (source != null)
            {
                source.Script = doc.Text;
            }
        }

        public void OnEditFocusLost()
        {
            focusedIndicator.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void editor_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lastEd != null)
                lastEd.OnEditFocusLost();
            lastEd = this;
            focusedIndicator.Visibility = System.Windows.Visibility.Visible;
        }
    }


    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class LuaCompletionData : ICompletionData
    {
        public LuaCompletionData(string data)
        {
            Source = ScriptsMethodsList.ParseAPIString(data);
            vars = LuaApiVarible.ParseList(Source.Key);
            string txt = Source.Key, func4descr = Source.Key;

            foreach (var vr in vars)
            {
                txt = txt.Replace(vr.Source, vr.VarType);
                func4descr = func4descr.Replace(vr.Source, vr.VarName);
            }

            this.Text = txt;
            _description = func4descr + "\n\n" + Source.Value;

        }
        public List<LuaApiVarible> vars;
        public KeyValuePair<string, string> Source;

        public double Priority
        {
            get
            {
                return 1;
            }
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        string _description = "";

        public object Description
        {
            get { return _description; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
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
                            textArea.Document.Replace(completionSegment.Offset - 1, completionSegment.Length + 1, txt + "\n");
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
                //textArea.Document.Replace(completionSegment.Offset - 1, completionSegment.Length + 1, txt + " ");
            }

        }

    }


}
