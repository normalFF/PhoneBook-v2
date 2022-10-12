using System.ComponentModel.DataAnnotations;

namespace PhoneLibrary
{
	internal class PhoneNumber
	{
		private static string _numbers = "0123456789";
		public string Type { get; private set; }
		public string Number { get; private set; }
		public string NumeralNumber { get; private set; }

		public PhoneNumber(PhoneNumberModel model)
		{
			Type = model.Type;
			Number = model.Number;
			NumeralNumber = string.Join("", Number.Where(i => _numbers.Contains(i)).ToArray());
		}

		public PhoneNumberModel GetModel()
		{
			return new PhoneNumberModel() { Type = Type, Number = Number };
		}

		public override bool Equals(object obj)
		{
			if (obj is not null)
			{
				if (obj is PhoneNumber phone)
				{
					return phone.NumeralNumber.Contains(NumeralNumber) || NumeralNumber.Contains(phone.NumeralNumber);
				}
			}
			return false;
		}

		public void ValidatePhoneModel(PhoneNumberModel phoneModel)
		{
			if (phoneModel == null) throw new ArgumentNullException($"{nameof(phoneModel)} не может быть равен {phoneModel}");
			var context = new ValidationContext(phoneModel, serviceProvider: null, items: null);
			var validationResults = new List<ValidationResult>();

			if (!Validator.TryValidateObject(phoneModel, context, validationResults, true)) throw new ValidationException(validationResults[0].ErrorMessage);
		}
	}
}
