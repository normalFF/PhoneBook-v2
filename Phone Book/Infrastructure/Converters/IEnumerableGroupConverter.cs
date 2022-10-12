using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#pragma warning disable CS8604, CS8603

namespace Phone_Book.Infrastructure.Converters
{
	internal class IEnumerableGroupConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 1)
			{
				var collection = values[0] as IEnumerable<string>;
				return collection.ToList();
			}
			if (values.Length == 2)
			{
				try
				{
					var collection = values[0] as IEnumerable<string>;
					var selecteditems = values[1] as IEnumerable<string>;
					return collection.Where(t => !selecteditems.Contains(t)).ToList();
				}
				catch (Exception)
				{
					return null;
				}
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
