using Phone_Book.Infrastructure.Command;
using Phone_Book.Infrastructure.DialogService;
using Phone_Book.Infrastructure.DialogService.Abonents;
using Phone_Book.Infrastructure.WindowClose;
using Phone_Book.Models.Base;
using PhoneLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Phone_Book.Models
{
	internal class CreateGroup : BaseViewModel, IWindowClose
	{
		IDialogAbonentService dialogService;
		private IPhoneBook phoneBook;
		private string _groupName;
		private IReadOnlyCollection<AbonentModel> phoneBookAbonents;
		private AbonentModel selectAbonentInPhoneBook;
		private AbonentModel selectAbonentInSelectedAbonents;
		private bool isAddGroup;

		public string GroupName
		{
			get => _groupName;
			set => Set(ref _groupName, value);
		}
		public IReadOnlyCollection<AbonentModel> PhoneBookAbonents
		{
			get => phoneBookAbonents;
			set => Set(ref phoneBookAbonents, value);
		}
		public ObservableCollection<AbonentModel> SelectAbonents { get; set; }
		public AbonentModel SelectAbonentInPhoneBook
		{
			get => selectAbonentInPhoneBook;
			set
			{
				if (Set(ref selectAbonentInPhoneBook, value)) GlobalSelectAbonent = value;
			}
		}
		public AbonentModel SelectAbonentInSelectedAbonents
		{
			get => selectAbonentInSelectedAbonents;
			set
			{
				if (Set(ref selectAbonentInSelectedAbonents, value)) GlobalSelectAbonent = value;
			}
		}

		public ICommand AddAbonentInGroup { get; private set; }
		public ICommand DeleteAbonentInGroup { get; private set; }
		public ICommand EditAbonentCommand { get; private set; }
		public ICommand ViewAbonentCommand { get; private set; }
		public ICommand AddGroupInPhoneBookCommand { get; private set; }
		public ICommand CloseCommand { get; private set; }
		public Action Close { get; set; }

		public CreateGroup(IPhoneBook book, IDialogAbonentService dialog)
		{
			phoneBook = book;
			dialogService = dialog;

			PhoneBookAbonents = book.GetAbonents();
			SelectAbonents = new ObservableCollection<AbonentModel>();
			CommandInitialize();
		}

		private void CommandInitialize()
		{
			AddAbonentInGroup = new CommandBase(OnAddAbonentInGroup, (obj) => SelectAbonentInPhoneBook is not null && !SelectAbonents.Contains(SelectAbonentInPhoneBook));
			DeleteAbonentInGroup = new CommandBase(OnDeleteInGroup, (obj) => SelectAbonentInSelectedAbonents is not null && SelectAbonents.Contains(SelectAbonentInSelectedAbonents));
			EditAbonentCommand = new CommandBase(OnEditAbonent, (obj) => SelectAbonentInSelectedAbonents is not null || SelectAbonentInPhoneBook is not null);
			ViewAbonentCommand = new CommandBase(OnViewAbonent, (obj) => SelectAbonentInSelectedAbonents is not null || SelectAbonentInPhoneBook is not null);
			AddGroupInPhoneBookCommand = new CommandBase(OnAddGroupInPhone, (obj) => !string.IsNullOrEmpty(GroupName) && SelectAbonents.Count > 0);
			CloseCommand = new CommandBase(OnClose, (obj) => true);
		}

		private void OnAddAbonentInGroup(object obj)
		{
			SelectAbonents.Add(SelectAbonentInPhoneBook);
			SelectAbonentInPhoneBook = null;
			OnPropertyChanged(nameof(PhoneBookAbonents));
		}

		private void OnDeleteInGroup(object obj)
		{
			SelectAbonents.Remove(SelectAbonentInSelectedAbonents);
			SelectAbonentInSelectedAbonents = null;
			OnPropertyChanged(nameof(PhoneBookAbonents));
		}

		private void OnEditAbonent(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.EditAbonent, (value) => result = value);
			PhoneBookAbonents = phoneBook.GetAbonents();
			OnPropertyChanged(nameof(SelectAbonents));
		}

		private void OnViewAbonent(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.AbonentInfo, (value) => result = value);
		}

		private void OnAddGroupInPhone(object obj)
		{
			isAddGroup = true;
			Close?.Invoke();
			isAddGroup = false;
		}

		private void OnClose(object obj)
		{
			isAddGroup = false;
			Close?.Invoke();
		}

		public bool CanClose()
		{
			if (isAddGroup)
			{
				if (string.IsNullOrEmpty(GroupName) || SelectAbonents.Count == 0) return true;
				else
				{
					if (phoneBook.GetGroupsName().Contains(GroupName))
					{
						dialogService.ShowMessageInfo($"Группа с названием '{GroupName}' уже существует", "Внимание");
						return false;
					}
					try
					{
						phoneBook.AddGroup(GroupName, SelectAbonents.ToArray());
					}
					catch (Exception ex)
					{
						dialogService.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка");
						return false;
					}
					return true;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(GroupName) && SelectAbonents.Count == 0) return true;
				int result = dialogService.ShowMessageQuestion("Закрыть окно создания группы?", "Закрыть окно");

				switch (result)
				{
					case 1:
						return true;
					default:
						return false;
				}
			}
		}
	}
}
