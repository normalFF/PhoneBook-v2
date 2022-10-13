using PhoneLibrary.IdGenerators;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Encodings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhoneLibrary
{
	public class PhoneBook : IPhoneBook
	{
		private static readonly string[] _baseGroups = new string[] { "Семья", "Работа", "Друзья", "Общие" };
		private static readonly string[] _basePhoneTypse = new string[] { "Домашний", "Рабочий", "Мобильный" };

		private readonly IGenerateId _generate;
		private readonly List<string> _group;
		private readonly List<string> _phoneType;
		private readonly List<Abonent> _abonentsList;
		private bool _isSaved;

		public PhoneBook()
		{
			_group = new List<string>() { "Семья", "Работа", "Друзья", "Общие" };
			_phoneType = new List<string>() { "Домашний", "Рабочий", "Мобильный" };
			_abonentsList = new();
			_isSaved = true;
			_generate = new MyGenerateId(this);
		}

		public void AddAbonent(AbonentModel abonentModel)
		{
			if (abonentModel.Id is not null) throw new InvalidOperationException("Объект уже содержится в телефонной книге");

			abonentModel.Id = _generate.GetId();
			Abonent abonent = new Abonent(abonentModel);
			_abonentsList.Add(abonent);
			var newTypes = abonentModel.Phones.Where(i => !_phoneType.Contains(i.Type)).Select(i => i.Type).ToArray();
			if (newTypes.Length != 0) _phoneType.AddRange(newTypes);
			_isSaved = false;
		}

		public void UpdateAbonent(AbonentModel abonentModel)
		{
			if (abonentModel.Id is null) throw new InvalidOperationException("Объект отсутствует в телефонной книге");

			for (int i = 0; i < _abonentsList.Count; i++)
			{
				if (abonentModel.Id == _abonentsList[i].Id) _abonentsList[i] = new(abonentModel);
			}
		}

		public void AddPhoneAbonent(int id, PhoneNumberModel phone)
		{
			Abonent abonent = _abonentsList.Where(i => i.Id == id).First();
			if (phone is null) throw new ArgumentNullException("Не указан номер телефона");
			if (abonent is null) throw new ArgumentNullException($"Абонента с идентификатором {id} не существует");

			abonent.AddPhones(phone);
			_isSaved = false;
		}

		public IEnumerable<AbonentModel> GetAbonents()
		{
			return _abonentsList.Select(item => item.GetModel());
		}

		public IEnumerable<string> GetGroupsName()
		{
			return (IEnumerable<string>)_group.ToArray().Clone();
		}

		public IEnumerable<string> GetPhoneTypes()
		{
			return (IEnumerable<string>)_phoneType.ToArray().Clone();
		}

		public void RemoveGroup(string groupName)
		{
			if (_group.Contains(groupName))
			{
				foreach (Abonent item in _abonentsList) item.RemoveGroup(groupName);
				_group.Remove(groupName);
			}
		}

		public void RemoveAbonent(AbonentModel abonent)
		{
			if (abonent is null) throw new ArgumentNullException($"Ошибка операции. Значение параметра {abonent}");
			if (abonent.Id is null) throw new ArgumentNullException($"Ошибка операции. Элемент отсутствует в телефонной книге");

			Abonent selectAbonent = _abonentsList.Where(i => i.Equals(abonent)).First();
			_abonentsList.Remove(selectAbonent);
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
			foreach (AbonentModel item in arrayAbonents)
				_abonentsList.Where(i => i.Id == item.Id).First()?.AddGroup(groupName);
		}

		public bool IsSaved()
		{
			return _isSaved;
		}

		public void Save(string filePath)
		{
			var serializedCollection = GetAbonents();
			foreach (AbonentModel item in serializedCollection) item.Id = null;

			var result = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(serializedCollection, new JsonSerializerOptions() { WriteIndented = true } ));
			using (FileStream str = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
			{
				str.Write(result);
			}
			_isSaved = true;
		}

		public void Load(string filePath)
		{
			if (File.Exists(filePath))
			{
				var serializedString = string.Empty;

				using (FileStream streamRead = File.OpenRead(filePath))
				{
					byte[] bytes = new byte[streamRead.Length];
					streamRead.Read(bytes, 0, bytes.Length);
					serializedString = Encoding.UTF8.GetString(bytes);
				}

				IEnumerable<AbonentModel> resultDeserialize = JsonSerializer.Deserialize<IEnumerable<AbonentModel>>(serializedString);
				if (resultDeserialize is null) throw new SerializationException();

				Clear();
				try
				{
					foreach (AbonentModel item in resultDeserialize)
					{
						AddAbonent(item);
					}
					_isSaved = true;
				}
				catch (Exception ex)
				{
					_abonentsList.Clear();
					_isSaved = true;
					throw new Exception(ex.InnerException is null ? ex.Message : ex.InnerException.Message);
				}
			}
			else
			{
				throw new FileNotFoundException();
			}
		}

		public bool CheckAbonentByProperties(AbonentModel abonent)
		{
			foreach (Abonent item in _abonentsList)
			{
				if (item.Equals(abonent)) return true;
			}
			return false;
		}

		public void Clear()
		{
			_abonentsList.Clear();
			_group.Clear();
			_group.AddRange(_baseGroups);
			_phoneType.Clear();
			_phoneType.AddRange(_basePhoneTypse);
		}

		public void ValidatePhoneNumber(PhoneNumberModel phoneModel)
		{
			PhoneNumber.ValidatePhoneModel(phoneModel);
		}
	}
}
