using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using ClueHelper.Models;

namespace ScoreCard
{
    [ValueConversion(typeof(List<Card>), typeof(string))]
    public class CardListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cards = value as IList<Card>;
            return null == cards ? string.Empty : string.Join(",", cards);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
