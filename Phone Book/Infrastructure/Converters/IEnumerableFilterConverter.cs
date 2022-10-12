using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using PhoneLibrary;

namespace Phone_Book.Infrastructure.Converters
{
	internal class IEnumerableFilterConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 0) return null;
			else if (values[0] is IEnumerable<string> enumerableString) return values.Length == 2 ? FilteredCollection(enumerableString, values[1]) : enumerableString.ToList();
			else if (values[0] is IEnumerable<AbonentModel> enumerableAbonents) return values.Length == 2 ? FilteredCollection(enumerableAbonents, values[1]) : enumerableAbonents.ToList();
			else if (values[0] is IEnumerable<PhoneNumberModel> enumerablePhones) return values.Length == 2 ? FilteredCollection(enumerablePhones, values[1]) : enumerablePhones.ToList();
			else return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private List<T> FilteredCollection<T>(IEnumerable<T> mainCollection, object filterCollection)
			=> filterCollection is IEnumerable<T> enumerableCollection
			? mainCollection.Where(i => !enumerableCollection.Contains(i)).ToList()
			: null;
	}
}
