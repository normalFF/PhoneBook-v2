using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618, CS8765

namespace PhoneLibrary
{
	internal class Abonent
	{
		private List<string> _groups { get; set; }
		private List<PhoneNumber> _phoneNumbers { get; set; }

		public string Name { get; private set; }
		public string Surname { get; private set; }
		public DateTime? DateOfBirth { get; private set; }
		public string Residence { get; private set; }
		public int Id { get; private set; }
		public string ImageBase64 { get; private set; }

		public Abonent(AbonentModel model)
		{
			ValidateAbonentModel(model);
			SetPropertyByModel(model);
		}

		public void AddPhones(PhoneNumberModel phoneModel)
		{
			var phone = new PhoneNumber(phoneModel);
			if (!_phoneNumbers.Contains(phone)) _phoneNumbers.Add(phone);
		}

		public void RemovePhones(PhoneNumberModel phoneModel)
		{
			var phone = new PhoneNumber(phoneModel);
			_phoneNumbers.Remove(phone);
		}

		public void AddGroup(string group)
		{
			if (!_groups.Contains(group)) _groups.Add(group);
		}

		public void RemoveGroup(string group)
		{
			_groups.Remove(group);
		}

		public void RenameAnonent(AbonentModel model)
		{
			ValidateAbonentModel(model);
			if (model.Id == null) throw new ArgumentNullException($"Отсутствует индентификатор");
			if (model.Id != Id) throw new InvalidOperationException("Идентификатор модели не соответствует идентификатору объекта");

			SetPropertyByModel(model);
		}

		public AbonentModel GetModel()
		{
			return new AbonentModel()
			{
				Name = Name,
				Surname = Surname,
				DateOfBirth = DateOfBirth,
				Residence = Residence,
				Groups = (string[])_groups.ToArray().Clone(),
				Phones = _phoneNumbers.Select(item => item.GetModel()).ToArray(),
				Id = Id
			};
		}

		private void ValidateAbonentModel(AbonentModel model)
		{
			if (model == null) throw new ArgumentNullException($"{nameof(model)} не может быть {model}");
			var context = new ValidationContext(model, serviceProvider: null, items: null);
			var validationResults = new List<ValidationResult>();

			if (!Validator.TryValidateObject(model, context, validationResults, true)) throw new ValidationException(validationResults[0].ErrorMessage);
		}

		private void SetPropertyByModel(AbonentModel model)
		{
			Name = model.Name;
			Surname = model.Surname;
			DateOfBirth = model.DateOfBirth;
			Residence = model.Residence;
			_groups = model.Groups == null ? new List<string>() : ((string[])model.Groups.Clone()).ToList();
			_phoneNumbers = model.Phones.Select(item => new PhoneNumber(item)).ToList();
		}

		public override bool Equals(object obj)
		{
			if (obj is Abonent abonent)
			{
				return string.Equals(Name, abonent.Name) && string.Equals(Surname, abonent.Surname)
														 && string.Equals(Residence, abonent.Residence)
														 && Equals(DateOfBirth, abonent.DateOfBirth)
														 && Id == abonent.Id;
			}
			else if (obj is AbonentModel abonentModel)
			{
				bool equalsResult = string.Equals(Name, abonentModel.Name) && string.Equals(Surname, abonentModel.Surname)
														 && string.Equals(Residence, abonentModel.Residence)
														 && Equals(DateOfBirth, abonentModel.DateOfBirth);

				if (abonentModel.Id is null) return equalsResult;
				return equalsResult && Id == abonentModel.Id;
			}
			return false;
		}
	}
}