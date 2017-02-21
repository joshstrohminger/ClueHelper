using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ClueHelper.Models;

namespace ScoreCard
{
    [ValueConversion(typeof(Possibility), typeof(string))]
    public class PossibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value as Possibility?)
            {
                case Possibility.Maybe:
                    return "•";
                case Possibility.NotHolding:
                    return "✕";
                case Possibility.Holding:
                    return "🂠";
                case Possibility.Unknown:
                case null:
                    return string.Empty;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
