using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PhoneLibrary;

namespace Phone_Book.Infrastructure.Converters
{
	internal class PhoneNumberConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is List<PhoneNumberModel> numbers)
			{
				var returnResult = string.Empty;
				var dicktionaryPhones = numbers.GroupBy(t => t.Type);

				foreach (var item in dicktionaryPhones)
				{
					returnResult += $"Телефоны: {item.Key}\n";
					foreach (var phones in item)
					{
						returnResult += $"	{phones.Number}\n";
					}
				}

				return returnResult.Trim('\n');
			}

			return "Отсутствует";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
