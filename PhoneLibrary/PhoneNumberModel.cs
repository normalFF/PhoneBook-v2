using PhoneLibrary.CustomValidationAttribute;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PhoneLibrary
{
	[Serializable]
	public class PhoneNumberModel
	{
		private string _numbers = "0123456789";

		[Required(ErrorMessage = "Тип телефона не может быть пустым значением")]
		public string Type { get; set; }

		[Required(ErrorMessage = "Номер телефона не может быть пустым значением")]
		[RegularExpression(@"(\+|\d|\()([\d\-\(\) ]|){9,}\d", ErrorMessage = "Номер телефона не соответствует допустимому формату")]
		[CustomRange(4, 15, ErrorMessage = "Номер содержит недопустимое количество цифр")]
		public string Number { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is PhoneNumberModel model)
			{
				string objNumber = string.Join("", model.Number.Where(i => _numbers.Contains(i)).ToArray());
				string currentNumber = string.Join("", Number.Where(i => _numbers.Contains(i)).ToArray());
				
				return objNumber.Contains(currentNumber) || currentNumber.Contains(objNumber);
			}
			return false;
		}
	}
}
