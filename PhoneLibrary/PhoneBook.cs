using System.Collections.ObjectModel;
using System.Text.Json;

namespace PhoneLibrary
{
	public class PhoneBook : IPhoneBook
	{
		private static readonly string[] _baseGroups = new string[] { "Семья", "Работа", "Друзья", "Общие" };
		private static readonly string[] _basePhoneType = new string[] { "Домашний", "Рабочий", "Мобильный" };
		private int _id;
		private IReadOnlyCollection<string> groupCollection;
		public IReadOnlyCollection<string> phoneTypeCollection;

		internal List<string> Group { get; set; }
		internal List<string> PhoneType { get; set; }
		internal List<Abonent> AbonentsList { get; set; }
		internal bool IsSaved { get; private set; }

		public PhoneBook()
		{
			Group = _baseGroups.ToList();
			PhoneType = _basePhoneType.ToList();
			AbonentsList = new();

			groupCollection = new ReadOnlyCollection<string>(Group);
			phoneTypeCollection = new ReadOnlyCollection<string>(PhoneType);
			IsSaved = true;

			_id = 0;
		}

		public IReadOnlyCollection<string> GetPhoneTypes() => phoneTypeCollection;

		public IReadOnlyCollection<string> GetGroupsName() => groupCollection;

		public IReadOnlyCollection<AbonentModel> GetAbonents() => new ReadOnlyCollection<AbonentModel>(AbonentsList.Select(i => i.GetModel()).ToList());

		private void Initialize(List<AbonentModel> abonentModels)
		{
			List<string> groups = new();
			List<string> types = new();
			List<Abonent> abonents = new();

			foreach (AbonentModel modelItem in abonentModels)
			{
				List<string> itemGroups = modelItem.Groups.Where(i => !string.IsNullOrEmpty(i)).Where(i => !groups.Contains(i)).ToList();
				List<string> itemTypes = modelItem.Phones.Select(i => i.Type).Where(i => !string.IsNullOrEmpty(i)).Where(i => !types.Contains(i)).ToList();
				if (itemGroups is not null) groups.AddRange(itemGroups);
				if (itemTypes is not null) types.AddRange(itemTypes);

				modelItem.Id = _id;
				_id = _id + 1;
				abonents.Add(new Abonent(modelItem));
			}

			Group.AddRange(groups);
			PhoneType.AddRange(types);
			AbonentsList.AddRange(abonents);
			IsSaved = true;
		}

		public void AddAbonent(AbonentModel abonentModel)
		{
			if (abonentModel.Id is not null) throw new InvalidOperationException("Объект уже содержится в телефонной книге");

			abonentModel.Id = _id;
			Abonent abonent = new Abonent(abonentModel);

			List<string> itemGroups = abonentModel.Groups.Where(i => !string.IsNullOrEmpty(i)).Where(i => !Group.Contains(i)).ToList();
			List<string> itemTypes = abonentModel.Phones.Select(i => i.Type).Where(i => !string.IsNullOrEmpty(i)).Where(i => !PhoneType.Contains(i)).ToList();
			if (itemGroups is not null) Group.AddRange(itemGroups);
			if (itemTypes is not null) PhoneType.AddRange(itemTypes);
			AbonentsList.Add(abonent);

			_id = _id + 1;
			IsSaved = false;
		}

		public void UpdateAbonent(AbonentModel abonentModel)
		{
			if (abonentModel.Id is null) throw new InvalidOperationException("Объект отсутствует в телефонной книге");

			Abonent abonent = new Abonent(abonentModel);
			for (int i = 0; i < AbonentsList.Count; i++)
			{
				if (abonent.Id == AbonentsList[i].Id) AbonentsList[i] = abonent;
			}

			List<string> groups = new();
			List<string> phones = new();

			foreach (Abonent abonentItem in AbonentsList)
			{
				if (abonentItem.Groups is not null) groups.AddRange(abonentItem.Groups.Where(i => !groups.Contains(i)));
				phones.AddRange(abonentItem.PhoneNumbers.Select(i => i.Type).Where(i => !phones.Contains(i)));
			}

			phones = PhoneType.Where(i => !phones.Contains(i)).ToList();
			groups = Group.Where(i => !groups.Contains(i)).ToList();
			foreach (string item in phones) PhoneType.Remove(item);
			foreach (string item in groups) Group.Remove(item);
			IsSaved = false;
		}

		public void RemoveAbonent(AbonentModel abonent)
		{
			if (abonent is null) throw new ArgumentNullException($"Ошибка операции. Значение параметра {abonent}");
			if (abonent.Id is null) throw new ArgumentNullException($"Ошибка операции. Элемент отсутствует в телефонной книге");

			Abonent selectAbonent = AbonentsList.Where(i => i.Id == abonent.Id).First();
			AbonentsList.Remove(selectAbonent);

			List<string> groups = new();
			List<string> phones = new();

			foreach (Abonent abonentItem in AbonentsList)
			{
				if (abonentItem.Groups is not null) groups.AddRange(abonentItem.Groups.Where(i => !groups.Contains(i)));
				phones.AddRange(abonentItem.PhoneNumbers.Select(i => i.Type).Where(i => !phones.Contains(i)));
			}

			phones = phones.Where(i => !PhoneType.Contains(i)).ToList();
			groups = groups.Where(i => !Group.Contains(i)).ToList();
			foreach (string item in phones) PhoneType.Remove(item);
			foreach (string item in groups) Group.Remove(item);
			IsSaved = false;
		}

		public void AddGroup(string groupName, AbonentModel[] arrayAbonents)
		{
			if (string.IsNullOrEmpty(groupName)) throw new ArgumentNullException("Отсутствует название группы");
			if (arrayAbonents is null || arrayAbonents?.Length == 0) throw new ArgumentNullException("Отсутствует список абонентов");

			foreach (AbonentModel item in arrayAbonents)
			{
				if (item is null) throw new NullReferenceException($"Ошибка операции. Коллекция содержит элемент, имеющий значение {item}");
				if (item.Id is null) throw new NullReferenceException($"Ошибка операции. Коллекция содержит элемент, которого нет в телефонной книге");
			}

			if (!Group.Contains(groupName)) Group.Add(groupName);
			int[] arrayId = arrayAbonents.Select(i => (int)i.Id).ToArray();

			foreach (Abonent abonent in AbonentsList)
			{
				if (arrayId.Contains(abonent.Id))
				{
					abonent.AddGroup(groupName);
				}
			}
			IsSaved = false;
		}

		public void RemoveGroup(string groupName, AbonentModel[] abonents)
		{
			var arrayId = abonents.Select(i => (int)i.Id).ToList();
			foreach (Abonent abonent in AbonentsList)
			{
				if (arrayId.Contains(abonent.Id)) abonent.RemoveGroup(groupName);
			}
			if (AbonentsList.Where(i => i.Groups.Contains(groupName)).Count() == 0) Group.Remove(groupName);
			IsSaved = false;
		}

		public void RemoveGroup(string groupName)
		{
			if (Group.Contains(groupName))
			{
				foreach (Abonent item in AbonentsList) item.RemoveGroup(groupName);
				Group.Remove(groupName);
			}
			IsSaved = false;
		}

		public void Save(string filePath)
		{
			var serializedCollection = GetAbonents();
			foreach (AbonentModel item in serializedCollection) item.Id = null;

			File.WriteAllText(filePath, string.Empty);
			using (FileStream fs = new FileStream(filePath, FileMode.Create))
			{
				JsonSerializer.Serialize(fs, AbonentsList.Select(i => i.GetModel()).ToList());
			}
			IsSaved = true;
		}

		public void Load(string filePath)
		{
			Clear();

			List<AbonentModel>? abonents;
			using (FileStream fs = new FileStream(filePath, FileMode.Open))
			{
				abonents = JsonSerializer.Deserialize<List<AbonentModel>>(fs);
			}

			if (abonents is null) return;
			else
			{
				Initialize(abonents);
			}
		}

		public bool CheckAbonentByProperties(AbonentModel abonent)
		{
			foreach (Abonent item in AbonentsList)
			{
				if (item.Equals(abonent)) return true;
			}
			return false;
		}

		public void Clear()
		{
			AbonentsList.Clear();
			Group.Clear();
			PhoneType.Clear();
			_id = 0;
		}

		public void ValidatePhoneNumber(PhoneNumberModel phoneModel)
		{
			PhoneNumber.ValidatePhoneModel(phoneModel);
		}

		bool IPhoneBook.IsSaved()
		{
			return IsSaved;
		}
	}
}
