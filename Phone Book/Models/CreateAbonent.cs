using System;
using System.Linq;
using System.Collections.ObjectModel;
using Phone_Book.Models.Base;
using PhoneLibrary;
using Phone_Book.Infrastructure.Command;
using System.Windows.Input;
using System.Collections.Generic;
using Phone_Book.Infrastructure.DialogService;
using Phone_Book.Infrastructure.WindowClose;

namespace Phone_Book.Models
{
	internal class CreateAbonent : BaseViewModel, IWindowClose
	{
		public bool CanClose()
		{
			if (model is not null)
			{
				if (phoneBook.CheckAbonentByProperties(model))
				{
					int result = dialogs.ShowMessageQuestion("Пользователь с данными значениями полей уже существует. Всё равно добавить?", "Внимание");
					switch (result)
					{
						case 1:
							return TryAddUser(model);
						default:
							return false;
					}
				}
				return TryAddUser(model);
			}
			else
			{
				int result = dialogs.ShowMessageQuestion("Закрыть окно добавления пользователя?", "Выход");

				switch (result)
				{
					case 1:
						return true;
					default:
						return false;
				}
			}
		}
		public Action Close { get; set; }

		private AbonentModel? model;
		private IDialog dialogs;
		private IPhoneBook phoneBook;
		private IEnumerable<string> phoneBookGroups;
		private IEnumerable<string> phoneTypeCollection;
		private PhoneNumberModel selectPhone;
		private string surname;
		private string name;
		private string residence;
		private DateTime? dateOfBirth;
		private string selectPhoneBookGroup;
		private string selectAbonentGroup;
		private string inputNewPhone;
		private string inputNewType;
		private string selectType;

		public string Name
		{
			get => name;
			set
			{
				if (!string.Equals(name, value, StringComparison.OrdinalIgnoreCase)) name = value;
			}
		}
		public string Surname
		{
			get => surname;
			set
			{
				if (!string.Equals(surname, value, StringComparison.OrdinalIgnoreCase)) surname = value;
			}
		}
		public string Residence
		{
			get => residence;
			set
			{
				if (!string.Equals(residence, value)) residence = value;
			}
		}
		public string SelectPhoneBookGroup
		{
			get => selectPhoneBookGroup;
			set => Set(ref selectPhoneBookGroup, value);
		}
		public string SelectAbonentGroup
		{
			get => selectAbonentGroup;
			set => Set(ref selectAbonentGroup, value);
		}
		public string InputNewPhone
		{
			get => inputNewPhone;
			set => Set(ref inputNewPhone, value);
		}
		public string InputNewType
		{
			get => inputNewType;
			set => Set(ref inputNewType, value);
		}
		public string SelectType
		{
			get => selectType;
			set => Set(ref selectType, value);
		}
		public DateTime? DateOfBirth
		{
			get => dateOfBirth;
			set
			{
				if (!Equals(dateOfBirth, value)) dateOfBirth = value;
			}
		}
		public ObservableCollection<PhoneNumberModel> Phones { get; }
		public ObservableCollection<string> SelectedGroups { get; }
		public IEnumerable<string> PhoneBookGroups
		{
			get => phoneBookGroups;
			set
			{
				if (phoneBookGroups != value) Set(ref phoneBookGroups, value);
				else OnPropertyChanged(nameof(PhoneBookGroups));
			}
		}
		public IEnumerable<string> PhoneTypeCollection
		{
			get => phoneTypeCollection;
			set
			{
				if (phoneTypeCollection != value) Set(ref phoneTypeCollection, value);
				else OnPropertyChanged(nameof(PhoneTypeCollection));
			}
		}
		private ObservableCollection<string> Groups { get; }
		public PhoneNumberModel SelectPhone
		{
			get => selectPhone;
			set => Set(ref selectPhone, value);
		}

		public ICommand AddAbonentInPhoneBook { get; private set; }
		public ICommand CloseWindow { get; private set; }
		public ICommand AddPhoneCommand { get; private set; }
		public ICommand DeletePhoneCommand { get; private set; }
		public ICommand AddGroupCommand { get; private set; }
		public ICommand DeleteGroupCommand { get; private set; }

		public CreateAbonent(IPhoneBook phoneBook, IDialog dialogWindows)
		{
			dialogs = dialogWindows;
			this.phoneBook = phoneBook;
			Phones = new();
			Groups = new();
			SelectedGroups = new();
			PhoneBookGroups = this.phoneBook.GetGroupsName();
			PhoneTypeCollection = this.phoneBook.GetPhoneTypes();
			CommandInitialize();
		}

		private void CommandInitialize()
		{
			DeleteGroupCommand = new CommandBase(OnDeleteGroup, (obj) => !string.IsNullOrEmpty(SelectAbonentGroup) && SelectedGroups.Contains(SelectAbonentGroup));
			DeletePhoneCommand = new CommandBase(OnDeletePhone, (obj) => Phones.Contains(SelectPhone));
			AddGroupCommand = new CommandBase(OnAddGroup, (obj) => !SelectedGroups.Contains(SelectPhoneBookGroup));
			AddPhoneCommand = new CommandBase(OnAddPhone, (obj) => (SelectType != null || !string.IsNullOrEmpty(InputNewType)) && !string.IsNullOrEmpty(InputNewPhone));
			AddAbonentInPhoneBook = new CommandBase(OnAddAbonentInPhoneBook, (obj) => !string.IsNullOrEmpty(Name) && Phones.Count != 0);
			CloseWindow = new CommandBase(OnCloseWindow, (obj) => true);
		}

		private void OnDeleteGroup(object obj)
		{
			SelectedGroups.Remove(SelectAbonentGroup);
			SelectAbonentGroup = string.Empty;
			OnPropertyChanged(SelectAbonentGroup);
		}

		private void OnDeletePhone(object obj)
		{
			Phones.Remove(SelectPhone);
			SelectPhone = null;
		}

		private void OnAddGroup(object obj)
		{
			SelectedGroups.Add(SelectPhoneBookGroup);
			OnPropertyChanged(nameof(PhoneBookGroups));
			SelectPhoneBookGroup = string.Empty;
		}

		private void OnAddPhone(object obj)
		{
			var newPhone = new PhoneNumberModel() { Type = string.IsNullOrEmpty(InputNewType) ? SelectType : InputNewType, Number = InputNewPhone };
			if (!Phones.Contains(newPhone))
			{
				Phones.Add(newPhone);
				InputNewType = string.Empty;
				InputNewPhone = string.Empty;
			}
		}

		private void OnCloseWindow(object obj)
		{
			model = null;
			Close.Invoke();
		}	

		private void OnAddAbonentInPhoneBook(object obj)
		{
			model = new AbonentModel()
			{
				Name = name,
				Surname = surname,
				DateOfBirth = dateOfBirth,
				Residence = residence,
				Phones = Phones.ToArray(),
				Groups = Groups.ToArray()
			};
			Close.Invoke();
		}

		private bool TryAddUser(AbonentModel abonent)
		{
			try
			{
				phoneBook.AddAbonent(abonent);
			}
			catch (Exception ex)
			{
				dialogs.ShowMessageException(ex.InnerException == null ? ex.Message : ex.InnerException.Message, "Ошибка добавления абонента");
				return false;
			}
			return true;
		}
	}
}
