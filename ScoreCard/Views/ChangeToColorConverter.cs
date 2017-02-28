using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ScoreCard.Models;

namespace ScoreCard.Views
{
    [ValueConversion(typeof(object), typeof(Brush))]
    public class ChangeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brushes = parameter as Brush[];
            if (brushes?.Length != 2)
            {
                return DependencyProperty.UnsetValue;
            }
            return value is PossibilityChange ? brushes[0] : brushes[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
