namespace PhoneLibrary
{
	public interface IPhoneBook
	{
		public IEnumerable<string> GetPhoneTypes();

		public IEnumerable<string> GetGroupsName();

		public IEnumerable<AbonentModel> GetAbonents();

		public void Save(string filePath);

		public void Load(string filePath);

		public void AddAbonent(AbonentModel abonent);

		public bool CheckAbonentByProperties(AbonentModel abonent);

		public void RemoveAbonent(AbonentModel abonent);

		public void AddGroup(string groupName, AbonentModel[] arrayAbonents);

		public void RemoveGroup(string groupName);

		public void ValidatePhoneNumber(PhoneNumberModel phoneModel);

		public bool IsSaved();

		public void Clear();
	}
}
