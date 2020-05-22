using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MeasurementApp.Models;

namespace MeasurementApp.Views
{
    /// <summary>
    /// Логика взаимодействия для CityView.xaml
    /// </summary>
    public partial class CityView : UserControl
    {
        public CityView()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var city in City.AllCities)
            {
                sb.Append($"Ид: {city.Id}, Имя: {city.Name}, хэш: {city.GetHashCode()}\n");
            }

            MessageBox.Show(sb.ToString());
        }
    }
}
