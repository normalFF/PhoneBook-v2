using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace PhoneLibrary
{
	internal class Abonent
	{
		private List<string> _groups { get; set; }
		private List<PhoneNumber> _phoneNumbers { get; set; }

		internal string Name { get; private set; }
		internal string Surname { get; private set; }
		internal DateTime? DateOfBirth { get; private set; }
		internal string Residence { get; private set; }
		internal int Id { get; private set; }
		internal string ImageBase64 { get; private set; }
		internal IReadOnlyCollection<string> Groups { get; }
		internal IReadOnlyCollection<PhoneNumber> PhoneNumbers { get; }

		public Abonent(AbonentModel model)
		{
			ValidateAbonentModel(model);

			Name = model.Name;
			Surname = model.Surname;
			DateOfBirth = model.DateOfBirth;
			Residence = model.Residence;
			ImageBase64 = model.ImageBase64;
			Id = (int)model.Id;

			_groups = model.Groups == null ? new List<string>() : ((string[])model.Groups.Clone()).ToList();
			_phoneNumbers = model.Phones.Select(item => new PhoneNumber(item)).ToList();
			
			PhoneNumbers = new ReadOnlyCollection<PhoneNumber>(_phoneNumbers);
			Groups = new ReadOnlyCollection<string>(_groups);
		}

		public void AddGroup(string group)
		{
			if (!_groups.Contains(group)) _groups.Add(group);
		}

		public void RemoveGroup(string group)
		{
			_groups.Remove(group);
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
				ImageBase64 = ImageBase64,
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

		public override bool Equals(object? obj)
		{
			if (obj is not null)
			{
				if (obj is Abonent abonent)
				{
					return string.Equals(Name, abonent.Name) && string.Equals(Surname, abonent.Surname)
															 && string.Equals(Residence, abonent.Residence)
															 && Equals(DateOfBirth, abonent.DateOfBirth);
				}
				else if (obj is AbonentModel abonentModel)
				{
					return string.Equals(Name, abonentModel.Name) && string.Equals(Surname, abonentModel.Surname)
															 && string.Equals(Residence, abonentModel.Residence)
															 && Equals(DateOfBirth, abonentModel.DateOfBirth);
				}
			}
			return false;
		}
	}
}