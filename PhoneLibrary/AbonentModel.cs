using PhoneLibrary.CustomValidationAttribute;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace PhoneLibrary
{
	[Serializable]
	public class AbonentModel : ICloneable
	{
		[Required(ErrorMessage = "Имя абонента не может быть пустым значением")]
		[StringLength(20, MinimumLength = 2, ErrorMessage = "Длина имени не менее 2-х и не более 20 символов")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Фамилия абонента не может быть пустым значением")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Длина фамилии не менее 3-х и не более 30 символов")]
		public string Surname { get; set; }

		[CustomDateRange("1/1/1980", ErrorMessage = "Указанная дата является некорректной")]
		public DateTime? DateOfBirth { get; set; }

		public string Residence { get; set; }

		public string ImageBase64 { get; set; }

		[Required(ErrorMessage = "Список телефонов не может быть пустым")]
		[MinLength(1, ErrorMessage = "Минимальное количество телефонов 1")]
		public PhoneNumberModel[] Phones { get; set; }
		
		public string[] Groups { get; set; }

		public int? Id { get; internal set; }

		public object Clone()
		{
			return new AbonentModel()
			{
				Name = Name,
				Surname = Surname,
				DateOfBirth = DateOfBirth,
				Residence = Residence,
				ImageBase64 = ImageBase64,
				Phones = Phones.Select(i => (PhoneNumberModel)i.Clone()).ToArray(),
				Groups = (string[])Groups.Clone(),
				Id = Id
			};
		}
	}
}
