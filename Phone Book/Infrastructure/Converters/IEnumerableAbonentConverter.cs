using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using PhoneLibrary;

#pragma warning disable CS8604, CS8603

namespace Phone_Book.Infrastructure.Converters
{
	internal class IEnumerableAbonentConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 1)
			{
				var collection = values[0] as IEnumerable<AbonentModel>;
				return collection.ToList();
			}
			if (values.Length == 2)
			{
				try
				{
					var collection = values[0] as IEnumerable<AbonentModel>;
					var param = values[1] as string;

					if (string.IsNullOrEmpty(param)) return collection.ToList();
					else return collection.Where(t => t.Groups.Contains(param)).ToList();
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

		private List<T> Filter<T>(IEnumerable<T> mainList, IEnumerable<T> filterList)
		{
			return mainList.Where(i => !filterList.Contains(i)).ToList();
		}
	}
}
