﻿using System;
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
    /// Логика взаимодействия для KeyGoodDataField.xaml
    /// </summary>
    public partial class GoodCountDataField : UserControl
    {
        public KeyValDataPair source;

        public GoodCountDataField(KeyValDataPair src)
        {
            source = src;
            InitializeComponent();
            mainGrid.DataContext = src;

            searchList.ItemsSource = sData.goods.Collection;

            lbl.Content = source.Key;
            src.ValueChanged_Event += src_ValueChanged_Event;
        }

        void src_ValueChanged_Event(object sender, EventArgs e)
        {
            lbl.Content = source.Key;
        }

        private void searchList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(searchList.SelectedItem != null)
            {
                source.Key = (searchList.SelectedItem as GoodsData).ID;
            }
        }
    }
}
