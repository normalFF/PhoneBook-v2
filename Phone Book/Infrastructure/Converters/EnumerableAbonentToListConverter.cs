using PhoneLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Phone_Book.Infrastructure.Converters
{
	internal class EnumerableAbonentToListConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 1)
			{
				if (values[0] is IEnumerable<AbonentModel> enumerable) return enumerable.ToList();
				else return values[0];
			}
			else
			{
				IEnumerable<AbonentModel> mainenumerable = values[0] as IEnumerable<AbonentModel>;
				IEnumerable<AbonentModel> enumerable = values[1] as IEnumerable<AbonentModel>;

				return mainenumerable.Where(x => !enumerable.Select(s => s.Id).Contains(x.Id)).ToList();
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
