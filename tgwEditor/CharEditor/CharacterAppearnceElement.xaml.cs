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

namespace tgwEditor.CharEditor
{
    /// <summary>
    /// Логика взаимодействия для CharacterAppearnceElement.xaml
    /// </summary>
    public partial class CharacterAppearnceElement : UserControl
    {
        KeyValDataPair col3D, col2D, col1D, typeD;
        public string part = "";
        public CharacterData ch;
        public CharacterAppearnceElement(CharacterData Ch, string Part)
        {
            part = Part;
            ch = Ch;
            InitializeComponent();

           
            if ((ch.appearance.Where(x => x.Key == part).Count()) <=  0)
            {
                typeD = KeyValDataPair.New(part, "app");
                ch.appearance.Add(typeD);
                typeD.Val = sData.APPEARANCE_TEMPLATE[ch.Type][part][0];
            }
            else
            {
                typeD = ch.appearance.Where(x => x.Key == part).First();
            }
            partLable.Content = part.Substring(0, part.Length );

            type.ItemsSource = sData.APPEARANCE_TEMPLATE[ch.Type][part];
            type.DataContext = typeD;

            updateType();
        }


        public void updateType()
        {
            string partType = typeD.Val;

            #region Colors init
            int colsCount = int.Parse(partType.Substring(partType.Length - 1));

            int width = 50;

            if (colsCount < 3)
            {
                col3Column.Width = new GridLength(0, GridUnitType.Pixel);
                col3.DataContext = null;
            }
            else
            {
                if (ch.appearance.Where(x => x.Key == part + ".col3").Count() <= 0)
                {
                    ch.appearance.Add(KeyValDataPair.New(part + ".col3", "app"));
                }
                col3D = ch.appearance.Where(x => x.Key == part + ".col3").First();
                col3.DataContext = col3D;
                col3Column.Width = new GridLength(width, GridUnitType.Pixel);
            }

            if (colsCount < 2)
            {
                col2Column.Width = new GridLength(0, GridUnitType.Pixel);
                col2.DataContext = null;
            }
            else
            {
                if (ch.appearance.Where(x => x.Key == part + ".col2").Count() <= 0)
                {
                    ch.appearance.Add(KeyValDataPair.New(part + ".col2", "app"));
                }
                col2D = ch.appearance.Where(x => x.Key == part + ".col2").First();
                col2.DataContext = col2D;
                col2Column.Width = new GridLength(width, GridUnitType.Pixel);
            }

            if (colsCount < 1)
            {
                col1Column.Width = new GridLength(0, GridUnitType.Pixel);
                col1.DataContext = null;
            }
            else
            {
                if (ch.appearance.Where(x => x.Key == part + ".col1").Count() <= 0)
                {
                    ch.appearance.Add(KeyValDataPair.New(part + ".col1", "app"));
                }
                col1D = ch.appearance.Where(x => x.Key == part + ".col1").First();
                col1.DataContext = col1D;
                col1Column.Width = new GridLength(width, GridUnitType.Pixel);

            }
        }
            #endregion

        private void type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateType();
        }
    }

}
