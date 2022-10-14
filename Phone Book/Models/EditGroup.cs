using System;
using Phone_Book.Infrastructure.Command;
using Phone_Book.Infrastructure.DialogService.Abonents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Phone_Book.Infrastructure.WindowClose;
using Phone_Book.Models.Base;
using PhoneLibrary;
using System.Linq;

namespace Phone_Book.Models
{
	internal class EditGroup : BaseViewModel, IWindowClose
	{
		public Action Close { get; set; }

		public bool CanClose()
		{
			if (isEditGroup)
			{
				if (SelectAbonents.Count == 0)
				{
					int result = dialogService.ShowMessageQuestion("Удалить группу?", "Удаление группы");

					switch (result)
					{
						case 1:
							try
							{
								phoneBook.RemoveGroup(GroupName);
								GroupName = string.Empty;
							}
							catch (Exception ex)
							{
								dialogService.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка");
								return false;
							}
							return true;
						default:
							return false;
					}
				}
				else
				{
					AbonentModel[] newAbonents = SelectAbonents.Where(i => !savedSelectedAbonents.Select(j => j.Id).Contains(i.Id)).ToArray();
					AbonentModel[] deleteAbonents = PhoneBookAbonents.Where(i => savedSelectedAbonents.Select(j => j.Id).Contains(i.Id) && i.Groups.Contains(GroupName)).ToArray();

					try
					{
						phoneBook.AddGroup(GroupName, newAbonents);
						phoneBook.RemoveGroup(GroupName, deleteAbonents);
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
				int result = dialogService.ShowMessageQuestion("Закрыть окно редактирования группы без сохранения изменений", "Сохранить изменения?");

				switch (result)
				{
					case 1:
						return false;
					default:
						return true;
				}
			}
		}

		IDialogAbonentService dialogService;
		private IPhoneBook phoneBook;
		private string _groupName;
		private IReadOnlyCollection<AbonentModel> phoneBookAbonents;
		private IReadOnlyCollection<AbonentModel> savedSelectedAbonents;
		private AbonentModel selectAbonentInPhoneBook;
		private AbonentModel selectAbonentInSelectedAbonents;
		private bool isEditGroup;

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
		public ICommand ViewAbonentCommand { get; private set; }
		public ICommand AddGroupInPhoneBookCommand { get; private set; }
		public ICommand CloseCommand { get; private set; }

		public EditGroup(IPhoneBook book, IDialogAbonentService dialog)
		{
			phoneBook = book;
			dialogService = dialog;

			PhoneBookAbonents = book.GetAbonents();
			SelectAbonents = new ObservableCollection<AbonentModel>(phoneBook.GetAbonents().Where(i => i.Groups.Contains(GlobalSelectGroup)).ToList());
			savedSelectedAbonents = new ReadOnlyCollection<AbonentModel>(phoneBook.GetAbonents().Where(i => i.Groups.Contains(GlobalSelectGroup)).ToList());
			GroupName = GlobalSelectGroup;
			CommandInitialize();
		}

		private void CommandInitialize()
		{
			AddAbonentInGroup = new CommandBase(OnAddAbonentInGroup, (obj) => SelectAbonentInPhoneBook is not null && !SelectAbonents.Contains(SelectAbonentInPhoneBook));
			DeleteAbonentInGroup = new CommandBase(OnDeleteInGroup, (obj) => SelectAbonentInSelectedAbonents is not null && SelectAbonents.Contains(SelectAbonentInSelectedAbonents));
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

		private void OnViewAbonent(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.AbonentInfo, (value) => result = value);
		}

		private void OnAddGroupInPhone(object obj)
		{
			isEditGroup = true;
			Close?.Invoke();
			isEditGroup = false;
		}

		private void OnClose(object obj)
		{
			isEditGroup = false;
			Close?.Invoke();
		}
	}
}
