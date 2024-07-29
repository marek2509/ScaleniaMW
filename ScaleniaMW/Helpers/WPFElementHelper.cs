using ScaleniaMW.Entities;
using ScaleniaMW.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScaleniaMW.Helpers
{
    public static class WPFElementHelper
    {
        static long number = 0;
        public static DockPanel GetParcelWithDeleteBtn(WZDEDzKW wzdedzkw, WZDEDzKWRepository _wZDEDzKWRepository, RoutedEventHandler routedEventHandler)
        {
            DockPanel panel = new DockPanel();
            Label lbl = new Label();
            lbl.Content = $"{wzdedzkw.Dzialka.Obreb.ID}-{wzdedzkw.Dzialka.IDD}";
            lbl.Width = 70;

            panel.Children.Add(lbl);
            Button btn = new Button();
            btn.Content = "-";
            btn.Width = 20;
            btn.Height = btn.Width;
            btn.Foreground = Brushes.IndianRed;

            btn.Click += (s, e) =>
            {
                _wZDEDzKWRepository.Delete(wzdedzkw.ID);
            };
            btn.Click += routedEventHandler;
            panel.Children.Add(btn);
            panel.Width = 110;
            Console.WriteLine(number);
            panel.Background = number++ % 2 == 0 ? Brushes.AliceBlue : Brushes.Transparent;
            if(number == long.MaxValue)
            {
                number = 0;
            }
            return panel;
        }
    }
}
