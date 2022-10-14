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
				if (values[0] is ICollection<AbonentModel> collectionAbonents)
				{
					return collectionAbonents.ToList();
				}
				if (values[0] is ICollection<string> collectionString)
				{
					return collectionString.ToList();
				}
				return null;
			}
			if (values.Length == 2)
			{
				if (values[1] is string groupName)
				{
					try
					{
						var collection = values[0] as ICollection<AbonentModel>;
						return collection.Where(i => i.Groups.Contains(groupName));
					}
					catch (Exception)
					{
						return null;
					}
				}
				else if (values[1] is ICollection<AbonentModel> abonents)
				{
					try
					{
						var collection = values[0] as ICollection<AbonentModel>;
						return collection.Where(i => !abonents.Select(j => j.Id).Contains(i.Id)).ToList();
					}
					catch (Exception)
					{
						return null;
					}
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
