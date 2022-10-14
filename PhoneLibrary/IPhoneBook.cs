namespace PhoneLibrary
{
	public interface IPhoneBook
	{
		public IReadOnlyCollection<string> GetPhoneTypes();

		public IReadOnlyCollection<string> GetGroupsName();

		public IReadOnlyCollection<AbonentModel> GetAbonents();

		public void Save(string filePath);

		public void Load(string filePath);

		public void AddAbonent(AbonentModel abonent);
		public void UpdateAbonent(AbonentModel abonent);
		public bool CheckAbonentByProperties(AbonentModel abonent);

		public void RemoveAbonent(AbonentModel abonent);

		public void AddGroup(string groupName, AbonentModel[] arrayAbonents);

		public void RemoveGroup(string groupName);

		public void RemoveGroup(string groupName, AbonentModel[] abonents);

		public void ValidatePhoneNumber(PhoneNumberModel phoneModel);

		public bool IsSaved();

		public void Clear();
	}
}
