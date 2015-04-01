using ICSharpCode.AvalonEdit.Document;
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

namespace tgwEditor.Elements
{
    /// <summary>
    /// Логика взаимодействия для KeyTextDataField.xaml
    /// </summary>
    public partial class TextDataField : UserControl
    {
        public TextData Text;
        public KeyValDataPair Link;

        public TextDocument doc;

        public TextDataField(KeyValDataPair link)
        {
            InitializeComponent();
            Link = link;
            link.ValueChanged_Event += link_ValueChanged_Event;
            changeId();
        }


        void link_ValueChanged_Event(object sender, EventArgs e)
        {
            changeId();
        }
        public void changeId()
        {
            Text = sData.texts.Get(Link.Val);
            if (Text != null)
            {
                main.DataContext = Text;
                newIDTextBox.Text = Link.Val;

                doc = new TextDocument();
                doc.Text = Text.Text;
                textBox.Document = doc;

                idLable.Text = Text.ID;

                Text.PropertyChanged += Text_PropertyChanged;
            }
            else
            {
                DataContext = null;
                if (doc != null)
                    doc.Text = "WARNING!!! Null binding!!!";
            }


        }

        void Text_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (doc.Text != Text.Text)
                doc.Text = Text.Text;
        }


        bool openedTools = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (openedTools)
            {
                tools.MaxHeight = 0;
                openedTools = !openedTools;
            }
            else
            {
                tools.MaxHeight = 800;
                openedTools = !openedTools;
            }
        }

        private void set_text_Click(object sender, RoutedEventArgs e)
        {

            if (newIDTextBox.Text != "")
            {
                TextData td = sData.texts.Get(newIDTextBox.Text);
                if (td != null)
                {
                    Link.Val = newIDTextBox.Text;
                    changeId();
                    info.Content = "Done!";
                }
                else
                {
                    info.Content = "This text does not exists!";
                }

            }
            else
            {
                info.Content = "Unavilable id. Enter int value.";
            }
        }

        private void new_text_Click(object sender, RoutedEventArgs e)
        {
            string td = sData.AddNewText();
            Link.Val = td.ToString();
            changeId();
            info.Content = "Done! Created " + td.ToString();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Text.Text = doc.Text;
        }

        private void moreSettingsButton_MouseMove(object sender, MouseEventArgs e)
        {
            var sl = (moreSettingsButton.Foreground as SolidColorBrush).Color;
            (moreSettingsButton.Foreground as SolidColorBrush).Color = Color.FromArgb(200, sl.R, sl.G, sl.B);
        }

        private void moreSettingsButton_MouseLeave(object sender, MouseEventArgs e)
        {
            var sl = (moreSettingsButton.Foreground as SolidColorBrush).Color;
            (moreSettingsButton.Foreground as SolidColorBrush).Color = Color.FromArgb(10, sl.R, sl.G, sl.B);
        }


    }
}
