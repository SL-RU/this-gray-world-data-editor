using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Windows.Shapes;
using System.Xml.Linq;
using tgwEditor.Elements;
using Xceed.Wpf.AvalonDock.Layout;

namespace tgwEditor.DiaEditor
{
    /// <summary>
    /// Эдитор диалогов и прочего
    /// </summary>
    public partial class GraphDiaEdit : UserControl
    {

        public static string TOOL_CONNECTION = "TOOL_CONNECTION";
        public string tool
        {
            get { return _tool; }
            set { _tool = value; toolLable.Content = value; }
        }
        public string _tool = "";
        public object toolSender;


        //All elements in editor
        public List<DiaProjectElement> allPrjElements;



        public GraphDiaEdit()
        {
            InitializeComponent();
            tool = "";
            allPrjElements = new List<DiaProjectElement>();

            InitProject();

            ProjectName = "prj";
            int i = 0;
            while (File.Exists(FilePath))
            {
                ProjectName = "prj" + i.ToString();
                i++;
            }


            sData.sebdedScriptEvent += sData_sebdedScriptEvent;


            tools.DataContext = this;


            openedProjects.Add(this);

        }


        //Event rom script window
        void sData_sebdedScriptEvent(string scriptID)
        {
            var v = AddProjectElementToEditor(new DiaItem(sData.scripts.Get(scriptID), ProjectScriptItemData.New(scriptID), this));
            prjScripts.Add(v.prjData);
            if (lastRightMousePos != null)
            {
                v.prjData.pos = lastRightMousePos;
                v.SetPosition();
            }
        }

        #region Items add functions
        /// <summary>
        /// Add new script element
        /// </summary>
        /// <returns></returns>
        public DiaItem AddNewDiaItem()
        {
            ScriptData sd = ScriptData.New();
            sData.scripts.Add(sd);

            ProjectScriptItemData pd = ProjectScriptItemData.New(sd.ID);
            pd.pos = new Point(DataGrid.ActualWidth / 2, DataGrid.ActualHeight / 2);
            prjScripts.Add(pd);

            DiaItem da = new DiaItem(sd, pd, this);
            AddProjectElementToEditor(da);

            return da;
        }
        public DiaItem getExistingScript(string id)
        {
            for (int i = 0; i < allPrjElements.Count; i++)
            {
                DiaProjectElement v = allPrjElements[i];
                if (v is DiaItem)
                {
                    if (((DiaItem)(v)).source.ID == id)
                        return ((DiaItem)(v));
                }
            }
            return null;
        }

        /// <summary>
        /// Add new dialog origin
        /// </summary>
        /// <returns></returns>
        public DialogOrigin AddNewDialogOrigin()
        {
            DialogOriginData dd = DialogOriginData.New();
            sData.dialogOrigins.Add(dd);

            ProjectScriptItemData pd = ProjectScriptItemData.New(dd.ID);
            pd.pos = new Point(DataGrid.ActualWidth / 2, DataGrid.ActualHeight / 2);
            pd.ScriptType = ProjectScriptItemData.SCRIPT_TYPE_ORIGIN;
            prjScripts.Add(pd);


            DialogOrigin di = new DialogOrigin(dd, pd, this);
            AddProjectElementToEditor(di);
            return di;
        }

        /// <summary>
        /// Add new note
        /// </summary>
        /// <returns></returns>
        public NoteItem AddNewNoteItem()
        {

            ProjectScriptItemData pd = ProjectScriptItemData.New("");
            pd.pos = new Point(DataGrid.ActualWidth / 2, DataGrid.ActualHeight / 2);
            pd.ScriptType = ProjectScriptItemData.NOTE_TYPE;
            prjScripts.Add(pd);

            NoteItem da = new NoteItem(pd, this);
            AddProjectElementToEditor(da);

            return da;
        }

        public MetronomeElement AddNewMetronomeElement()
        {
            ProjectScriptItemData pd = ProjectScriptItemData.New("");
            pd.pos = new Point(DataGrid.ActualWidth / 2, DataGrid.ActualHeight / 2);
            pd.ScriptType = ProjectScriptItemData.METRONOME;
            prjScripts.Add(pd);

            MetronomeElement da = new MetronomeElement(pd, this);
            AddProjectElementToEditor(da);

            return da;
        }

        /// <summary>
        /// Add project element to editor
        /// </summary>
        /// <param name="di">element</param>
        /// <returns></returns>
        public DiaProjectElement AddProjectElementToEditor(DiaProjectElement di)
        {
            DataGrid.Children.Add(di);
            allPrjElements.Add(di);
            di.SetPosition();
            return di;
        }

        public void RemoveProjectElement(DiaProjectElement e)
        {
            prjScripts.Remove(e.prjData);
            DataGrid.Children.Remove(e);
            allPrjElements.Remove(e);
        }

        /// <summary>
        /// Export texts vars from scripts with varKey name
        /// </summary>
        /// <param name="varName"></param>
        public void ExportTextFromProject(string varKey)
        {
            string s = "Project: " + ProjectName + "\n\n";
            int id = 0;
            foreach (var v in allPrjElements)
            {
                if (v is DiaItem)
                {
                    var vv = (v as DiaItem).source.vars.Where(x => x.Key == varKey);
                    if (vv.Count() > 0)
                    {
                        var vvv = vv.Where(x => sData.texts.Get(x.Val) != null);
                        if (vvv.Count() > 0)
                        {
                            var kp = vvv.First();
                            if (kp != null)
                            {
                                id++;
                                s += "-- " + id.ToString() + "\n" + sData.texts.Get(kp.Val).Text + "\n";
                            }
                        }
                    }
                }
            }
            Clipboard.SetText(s);
            sData.LOG("Project's text exported to clipboard buffer! Done.");

        }
        #endregion


        public void LoadScrFromPrjItem(ProjectScriptItemData pr)
        {
            switch (pr.ScriptType)
            {
                case ProjectScriptItemData.SCRIPT_TYPE_SCRIPT:
                    ScriptData sd = sData.scripts.Get(pr.ScriptID);
                    sData.LOG("Loading id = " + pr.ScriptID.ToString());
                    if (sd != null)
                    {
                        DiaItem da = new DiaItem(sd, pr, this);
                        AddProjectElementToEditor(da);
                    }
                    else
                    {
                        sData.LOG_WARNING("Script id=" + pr.ScriptID.ToString() + " is not existing");
                    }
                    break;
                case ProjectScriptItemData.SCRIPT_TYPE_ORIGIN:
                    DialogOriginData dod = sData.dialogOrigins.Get(pr.ScriptID);
                    sData.LOG("Loading dialog origin id = " + pr.ScriptID.ToString());
                    if (dod != null)
                    {
                        DialogOrigin da = new DialogOrigin(dod, pr, this);
                        AddProjectElementToEditor(da);
                    }
                    else
                    {
                        sData.LOG_WARNING("Dialog origin id=" + pr.ScriptID.ToString() + " is not existing");
                    }
                    break;
                case ProjectScriptItemData.NOTE_TYPE:
                    NoteItem note = new NoteItem(pr, this);
                    AddProjectElementToEditor(note);
                    break;
                case ProjectScriptItemData.METRONOME:
                    MetronomeElement metr = new MetronomeElement(pr, this);
                    AddProjectElementToEditor(metr);
                    break;

            }
        }


        #region GUIevents and functions

        public LayoutDocument guiWindow = null;
        /// <summary>
        /// creates LayoutAnchrorable window
        /// </summary>
        /// <returns></returns>
        public static LayoutDocument CreateWindow()
        {
            LayoutDocument l = new LayoutDocument();
            l.Title = "prj";
            l.Content = new GraphDiaEdit()
                {
                    guiWindow = l
                };


            return l;
        }

        /// <summary>
        /// creates LayoutDocument window from existing dialogEditor element
        /// </summary>
        /// <param name="gd"></param>
        /// <returns></returns>
        public static LayoutDocument CreateWindow(GraphDiaEdit gd)
        {
            LayoutDocument l = new LayoutDocument();
            l.Title = "prj";
            l.Content = gd;
            gd.guiWindow = l;


            return l;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sData.LOG("Saving All. Dialog Project.");
            SaveProject();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            AddNewDiaItem();
        }


        Point lastRightMousePos;
        private void MenuItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastRightMousePos = e.GetPosition(DataGrid);
        }

        #region AddButtons
        private void MenuItemAddScript_Click(object sender, RoutedEventArgs e)
        {
            var v = AddNewDiaItem();
            v.prjData.pos = lastRightMousePos;
            v.SetPosition();
        }
        private void MenuItemAddDiaScript_Click(object sender, RoutedEventArgs e)
        {
            var sd = AddNewDiaItem();

            ///////////////////TEMPLATE

            sd.source.Script = "show_text(txt)\n\nanswers_clear()\nanswers_add()";
            sd.source.answers.Add(AnswerData.New());
            sd.source.vars.Add(KeyValDataPair.New("txt", KeyValDataPair.VALUE_TYPE_TEXTID));

            /////////////////////////////

            sd.ScriptEditor.SetScriptSource(sd.source);

            sd.prjData.pos = lastRightMousePos;
            sd.SetPosition();
        }
        private void AddDiaOrigin_Click(object sender, RoutedEventArgs e)
        {
            var v = AddNewDialogOrigin();
            v.prjData.pos = lastRightMousePos;
            v.SetPosition();
        }

        private void MenuItemMetronome_Click(object sender, RoutedEventArgs e)
        {
            var v = AddNewMetronomeElement();
            v.prjData.pos = lastRightMousePos;
            v.SetPosition();
        }

        #endregion

        //load project button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadProjectGui();
        }



        private void filePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            filePath.Text.Replace(".", "");
            filePath.Text.Replace("/", "");
            filePath.Text.Replace("\\", "");
            filePath.Text.Replace(",", "");

            ProjectName = filePath.Text;

            if (File.Exists(FilePath))
            {
                filePath.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                filePath.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }


        private void LayoutDocument_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you save project? \nDo you want close this PRJ???", "DO YOU WANT CLOSE PROJECT???", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;

            }
            else
                if (openedProjects.Contains(this))
                    openedProjects.Remove(this);
        }
        private void MenuItemNote_Click(object sender, RoutedEventArgs e)
        {
            var v = AddNewNoteItem();
            v.prjData.pos = lastRightMousePos;
            v.SetPosition();
        }

        #endregion

        #region ManageProject
        public bool projectLoaded;
        public XDocument Xproject;
        public string ProjectName
        {
            get;
            set;
        }
        public static List<GraphDiaEdit> openedProjects = new List<GraphDiaEdit>();
        public string FilePath
        {
            get
            {
                return sData.path + "dia\\" + ProjectName + ".diaprj";
            }
        }
        public ObservableCollection<ProjectScriptItemData> prjScripts = new ObservableCollection<ProjectScriptItemData>();

        void InitProject()
        {
            Xproject = new XDocument();
            Xproject.Add(new XElement("scene"));
            prjScripts.CollectionChanged += prjScripts_CollectionChanged;
        }

        void prjScripts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ProjectScriptItemData li in e.OldItems)
                {
                    //Removed items
                    Xproject.Root.Elements().Where(x => x == li.source).Remove();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ProjectScriptItemData li in e.NewItems)
                {
                    //Added items
                    Xproject.Root.AddFirst(li.source);
                }
            }
        }

        private void LoadProject()
        {
            sData.LOG("Loading dialog project " + ProjectName + " from " + FilePath);
            if (File.Exists(FilePath))
            {
                try
                {
                    allPrjElements = new List<DiaProjectElement>();
                    prjScripts = new ObservableCollection<ProjectScriptItemData>();
                    Xproject = XDocument.Load(FilePath);

                    if (guiWindow != null)
                        guiWindow.Title = ProjectName;

                    foreach (XElement xe in Xproject.Root.Elements(ProjectScriptItemData.XElementName))
                    {
                        prjScripts.Add(new ProjectScriptItemData(xe));
                    }
                    prjScripts.CollectionChanged += prjScripts_CollectionChanged;
                }
                catch (Exception ex)
                {
                    sData.LOG_WARNING(ex.ToString());
                }

            }
            else
            {
                sData.LOG("Dialog project file " + FilePath + " not existed");
            }
        }

        public void SaveProject()
        {
            try
            {
                sData.Save();
                Xproject.Save(FilePath);
                sData.LOG("Dia prj " + ProjectName + " saved to " + FilePath);
                if (guiWindow != null)
                    guiWindow.Title = ProjectName;
            }
            catch (Exception ex)
            {
                sData.LOG_WARNING("Error on saving project " + ProjectName + " to file " + FilePath + " \nError " + ex.ToString());
            }
        }

        public void LoadProjectGui()
        {
            sData.LOG("Loading all. Dialog Project.");

            allPrjElements.Clear();
            DataGrid.Children.Clear();
            ConnectionGrid.Children.Clear();
            LoadProject();
            foreach (ProjectScriptItemData pd in prjScripts)
            {
                LoadScrFromPrjItem(pd);
            }


            sData.LOG("Loading all. Dialog Project. DONE");
        }

        private void LoadOnlyProject_Click(object sender, RoutedEventArgs e)
        {
            sData.LOG("Loading only prj. Dialog Project.");

            allPrjElements.Clear();
            DataGrid.Children.Clear();
            ConnectionGrid.Children.Clear();
            LoadProject();
            foreach (ProjectScriptItemData pd in prjScripts)
            {
                LoadScrFromPrjItem(pd);
            }


            sData.LOG("Loading all. Dialog Project. DONE");
        }
        #endregion

        private void export_click(object sender, RoutedEventArgs e)
        {
            ExportTextFromProject("txt");
        }





    }

    /// <summary>
    /// Соединительная линия между SourceElement и TargetElement
    /// </summary>
    public class ConnectionLine
    {
        Point s;
        Point t;

        public SourceElement sourceElement;
        public TargetElement targetElement;

        Line line;

        Grid layout;

        byte r, g, b;

        public ConnectionLine(SourceElement source, TargetElement target, Grid Layout)
        {
            sourceElement = source;
            targetElement = target;
            layout = Layout;

            line = new Line();

            SolidColorBrush Brush = new SolidColorBrush();

            var rnd = new Random();

            r = (byte)rnd.Next(0, 150);
            g = (byte)rnd.Next(0, 150);
            b = (byte)rnd.Next(0, 150);

            Brush.Color = Color.FromArgb(90, r, g, b);

            line.StrokeThickness = 6;
            line.Stroke = Brush;

            UpdatePos();

            layout.Children.Add(line);

            line.MouseEnter += line_MouseEnter;
            line.MouseLeave += line_MouseLeave;

            line.LayoutUpdated += line_LayoutUpdated;

            var c = new ContextMenu();
            var bu = new MenuItem();
            bu.Header = "Delete";
            bu.Click += bu_Click;
            c.Items.Add(bu);
            line.ContextMenu = c;
        }

        void bu_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        int updateCount = 100;
        void line_LayoutUpdated(object sender, EventArgs e)
        {
            if (updateCount >= 20)
            {
                UpdatePos();
                updateCount = 0;
            }
            updateCount++;
        }

        void line_MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Color.FromArgb(80, r, g, b);
            line.Stroke = Brush;
        }

        void line_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Color.FromArgb(240, r, g, b);
            line.Stroke = Brush;
        }

        public void source_MouseMove(object sender, MouseEventArgs e)
        {
            UpdatePos();
        }
        public void Delete()
        {
            layout.Children.Remove(line);
            sourceElement.remConnection();
            targetElement.remConnection(this);
        }

        void UpdatePos()
        {
            s = sourceElement.TranslatePoint(new Point(0, 0), layout);
            t = targetElement.TranslatePoint(new Point(0, 0), layout);


            line.X1 = s.X + sourceElement.RenderSize.Width / 2;
            line.X2 = t.X + targetElement.RenderSize.Width / 2;
            line.Y1 = s.Y + sourceElement.RenderSize.Height / 2;
            line.Y2 = t.Y + targetElement.RenderSize.Height / 2;
        }
    }

    /// <summary>
    /// Информация об элементе в проекте
    /// </summary>
    public class ProjectScriptItemData
    {
        public const string XElementName = "script",
            SCRIPT_TYPE_ORIGIN = "dia",
            SCRIPT_TYPE_SCRIPT = "scr",
            NOTE_TYPE = "note",
            METRONOME = "metronome";

        public XElement source;

        public string ScriptID
        {
            get { return source.Attribute("ID").Value.ToString(); }
            set { source.Attribute("ID").Value = value; }
        }
        public Point pos
        {
            get { return new Point(int.Parse(source.Attribute("pos_x").Value), int.Parse(source.Attribute("pos_y").Value)); }
            set { source.Attribute("pos_x").Value = ((int)value.X).ToString(); source.Attribute("pos_y").Value = ((int)value.Y).ToString(); }
        }
        public string ScriptType
        {
            get { return source.Attribute("scriptType").Value.ToString(); }
            set { source.Attribute("scriptType").Value = value; }
        }

        public string Data
        {
            get { return source.Attribute("data").Value.ToString(); }
            set { source.Attribute("data").Value = value; }
        }

        public ProjectScriptItemData(XElement src)
        {
            source = src;
        }

        public static ProjectScriptItemData New(string ID)
        {
            XElement nl = new XElement(XElementName);
            nl.Add(new XAttribute("ID", ID));
            nl.Add(new XAttribute("scriptType", SCRIPT_TYPE_SCRIPT));
            nl.Add(new XAttribute("pos_x", 0));
            nl.Add(new XAttribute("pos_y", 0));
            nl.Add(new XAttribute("data", ""));

            ProjectScriptItemData li = new ProjectScriptItemData(nl);
            return li;
        }
    }

}


