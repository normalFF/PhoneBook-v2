using Bogus;
using PhoneLibrary;

namespace UnitTests
{
	[TestFixture]
	public class Tests
	{
		public AbonentModel[] abonents;
		public IPhoneBook book;
		Random rn;
		public Faker f;

		[OneTimeSetUp]
		public void SetUp()
		{
			rn = new Random();
			f = new Faker();

			book = new PhoneBook();
			abonents = new AbonentModel[10];

			for (int i = 0; i < abonents.Length; i++)
			{
				PhoneNumberModel[] phones = new PhoneNumberModel[3];
				for (int j = 0; j < phones.Length; j++)
				{
					phones[j] = new PhoneNumberModel()
					{
						Type = "Test Type",
						Number = f.Phone.PhoneNumberFormat(rn.Next(0, 8))
					};
				}

				abonents[i] = new AbonentModel()
				{
					Name = f.Person.FirstName,
					Surname = f.Person.LastName,
					DateOfBirth = DateTime.Today.AddDays(rn.Next(0, 16) - 8),
					Phones = phones
				};
			}
		}

		[Test]
		public void TestModelValidation()
		{
			for (int i = 0; i < abonents.Length; i++)
			{
				
			}
		}
		
		[Test]
		public void TestGeneratePhones()
		{
			for (int i = 0; i < 20; i++)
			{
				int random = rn.Next(0, 8);
				Console.WriteLine($"random value: {random};   {f.Phone.PhoneNumberFormat(random)}");
			}
		}
	}
}