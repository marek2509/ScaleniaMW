﻿using ScaleniaMW.EWOPIS.Widoki;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ScaleniaMW.EWOPIS
{

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class EwopisMainWindow : Window
    {
        public EwopisMainWindow()
        {
            InitializeComponent();


            goToNewWindow();

        }

        async void goToNewWindow()
        {
            for (int i = 1; i < 10; i++)
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("./img/LOGO_DUŻY" + i + ".png", UriKind.Relative);
                bi3.EndInit();
                imgLogo.Source = bi3;
                await Task.Delay(100);
            }
        
            WindowDzialkiDoEwopis windowDzialkiDoEwopis = new WindowDzialkiDoEwopis();
            windowDzialkiDoEwopis.Show();
            windowMainEwopis.Close();
        }
    }
}
