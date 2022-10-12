using PhoneLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Phone_Book.Infrastructure.Converters
{
	internal class EnumerableToListConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IEnumerable<AbonentModel> result) return result.ToList();
			else if (value is IEnumerable<string> nextResult) return nextResult.ToList();
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
