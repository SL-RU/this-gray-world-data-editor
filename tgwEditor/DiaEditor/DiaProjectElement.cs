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

namespace tgwEditor.DiaEditor
{
    /// <summary>
    /// Логика взаимодействия для DiaProjectElementxaml.xaml
    /// </summary>
    public partial class DiaProjectElement : UserControl
    {

        public GraphDiaEdit window;
        public ProjectScriptItemData prjData;

        public DiaProjectElement(ProjectScriptItemData prDat, GraphDiaEdit wnd)
        {
            InitDrag();
        }


        //Pr
        #region projectData
        Nullable<Point> dragStart = null;
        public void InitDrag()
        {
            MouseButtonEventHandler mouseDown = (sender, args) =>
            {
                var element = (UIElement)sender;
                dragStart = args.GetPosition(element);
                element.CaptureMouse();
            };
            MouseButtonEventHandler mouseUp = (sender, args) =>
            {
                var element = (UIElement)sender;
                dragStart = null;
                element.ReleaseMouseCapture();
            };
            MouseEventHandler mouseMove = (sender, args) =>
            {
                if (dragStart != null && args.LeftButton == MouseButtonState.Pressed)
                {
                    var element = (UIElement)sender;
                    var p2 = args.GetPosition(window.DataGrid);
                    Canvas.SetLeft(element, p2.X - dragStart.Value.X);
                    Canvas.SetTop(element, p2.Y - dragStart.Value.Y);
                    Draging();
                }
            };
            Action<UIElement> enableDrag = (element) =>
            {
                element.MouseDown += mouseDown;
                element.MouseMove += mouseMove;
                element.MouseUp += mouseUp;
            };

            enableDrag(this);
        }

        void Draging()
        {
            prjData.pos = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        }
        public void SetPosition()
        {
            if (prjData != null && prjData.pos != null)
            {
                Canvas.SetLeft(this, prjData.pos.X);
                Canvas.SetTop(this, prjData.pos.Y);
            }
        }
        #endregion
    }
}
