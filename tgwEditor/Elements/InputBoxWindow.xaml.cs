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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace tgwEditor.Elements
{
    /// <summary>
    /// Логика взаимодействия для InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow : MetroWindow
    {
        public InputBoxWindow()
        {
            InitializeComponent();


        }

        public delegate void TextEntered(InputBoxWindow wnd, string txt, bool canceled);
        public event TextEntered OnTextEnteredEvent;

        public static InputBoxWindow CreateNew(string description, string header, string text)
        {
            var v = (new InputBoxWindow());
            v.Show();
            v.lable.Content = description;
            v.Title = header;
            v.text.Text = text;
            return v;
        }
        public static InputBoxWindow CreateNew(string description)
        {
            return CreateNew(description, description, "");
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if (OnTextEnteredEvent != null)
                OnTextEnteredEvent(this, text.Text, false);
            Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            if (OnTextEnteredEvent != null)
                OnTextEnteredEvent(this, null, true);
            Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (OnTextEnteredEvent != null)
                OnTextEnteredEvent(this, null, true);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Point c = Mouse.GetPosition(this);
            this.Left += c.X - this.RenderSize.Width / 2;
            this.Top += c.Y - this.RenderSize.Height / 2;

            text.Focus();
            text.SelectAll();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            ok_Click(null, new RoutedEventArgs());
        }


        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                cancel_Click(null, new RoutedEventArgs());
            }
            if (e.Key == Key.Enter)
            {
                ok_Click(null, new RoutedEventArgs());
            }
        }

        private void MetroWindow_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.Left == 0 || this.Top == 0)
            {
                Point c = Mouse.GetPosition(this);
                this.Left += c.X - this.RenderSize.Width / 2;
                this.Top += c.Y - this.RenderSize.Height / 2;

            }
        }
    }
}
