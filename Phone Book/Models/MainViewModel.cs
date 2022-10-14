using Phone_Book.Infrastructure.Command;
using Phone_Book.Infrastructure.DialogService;
using Phone_Book.Models.Base;
using Phone_Book.Infrastructure.DialogService.Abonents;
using Phone_Book.Infrastructure.DialogService.Groups;
using PhoneLibrary;
using System.Collections.Generic;
using System.Windows.Input;
using Phone_Book.Infrastructure.WindowClose;
using System;
using System.Linq;

namespace Phone_Book.Models
{
	internal class MainViewModel : BaseViewModel, IWindowClose
	{
		public Action Close { get; set; }
		public bool CanClose()
		{
			if (!phoneBook.IsSaved())
			{
				int result = dialogService.ShowMessageQuestion("Сохраненить изменения?", "Сохранение изменений");
				switch (result)
				{
					case 1:
						if (string.IsNullOrEmpty(FileWay))
						{
							FileWay = dialogService.SaveJsonFile();
							if (string.IsNullOrEmpty(FileWay)) return false;
							else
							{
								try
								{
									phoneBook.Save(FileWay);
								}
								catch (Exception ex)
								{
									dialogService.ShowMessageException("Ошибка сохранения", ex.InnerException == null ? ex.Message : ex.InnerException.Message);
									return false;
								}
								return true;
							}
						}
						else
						{
							try
							{
								phoneBook.Save(FileWay);
							}
							catch (Exception ex)
							{
								dialogService.ShowMessageException("Ошибка сохранения", ex.InnerException == null ? ex.Message : ex.InnerException.Message);
								return false;
							}
							return true;
						}
					case -1:
						return true;
					default:
						return false;
				}
			}
			else return true;
		}

		public ICommand OpenAddAbonentDialog { get; private set; }
		public ICommand OpenEditAbonentDialog { get; private set; }
		public ICommand OpenAbonentInfoDialog { get; private set; }
		public ICommand OpenAddGroupDialog { get; private set; }
		public ICommand OpenEditGroupDialog { get; private set; }
		public ICommand DeleteGroupCommand { get; private set; }
		public ICommand DeleteAbonentCommand { get; private set; }
		public ICommand LoadFileCommand { get; private set; }
		public ICommand SaveFileCommand { get; private set; }
		public ICommand SaveFileAsCommand { get; private set; }

		private IDialogService dialogService;
		private IPhoneBook phoneBook;
		private IReadOnlyCollection<AbonentModel> abonents;
		private IReadOnlyCollection<string> groups;
		private AbonentModel selectAbonent;
		private AbonentModel selectAbonentInGroup;
		private string selectGroup;
		public string fileWay;

		public IReadOnlyCollection<AbonentModel> Abonents
		{
			get => abonents;
			set 
			{
				if (abonents == value) OnPropertyChanged(nameof(Abonents));
				else Set(ref abonents, value);
			}
		}
		public AbonentModel SelectAbonent
		{
			get => selectAbonent;
			set
			{
				if (Set(ref selectAbonent, value)) GlobalSelectAbonent = value;
			}
		}
		public AbonentModel SelectAbonentInGroup
		{
			get => selectAbonentInGroup;
			set
			{
				if (Set(ref selectAbonentInGroup, value)) GlobalSelectAbonent = value;
			}
		}
		public IReadOnlyCollection<string> Groups
		{
			get => groups;
			set
			{
				if (groups == value) OnPropertyChanged(nameof(Groups));
				else Set(ref groups, value);
			}
		}
		public string SelectGroup
		{
			get => selectGroup;
			set
			{
				if (Set(ref selectGroup, value)) GlobalSelectGroup = value;
			}
		}
		public string FileWay
		{
			get => fileWay;
			set => Set(ref fileWay, value);
		}

		public MainViewModel(IPhoneBook book, IDialogService dialogService)
		{
			phoneBook = book;
			this.dialogService = dialogService;
			Abonents = phoneBook.GetAbonents();
			Groups = phoneBook.GetGroupsName();
			InitializeCommand();
		}

		private void InitializeCommand()
		{
			OpenAddAbonentDialog = new CommandBase(OnOpenAddAbonentDialog, (obj) => true);
			OpenAddGroupDialog = new CommandBase(OnOpenAddGroupDialog, (obj) => true);
			OpenEditAbonentDialog = new CommandBase(OnOpenEditAbonentDialog, (obj) => GlobalSelectAbonent != null);
			OpenAbonentInfoDialog = new CommandBase(OnOpenAbonentInfoDialog, (obj) => GlobalSelectAbonent != null);
			DeleteGroupCommand = new CommandBase(OnDeleteGroup, (obj) => !string.IsNullOrEmpty(SelectGroup));
			DeleteAbonentCommand = new CommandBase(OnDeleteAbonent, (obj) => GlobalSelectAbonent is not null);
			OpenEditGroupDialog = new CommandBase(OnEditGroup, (obj) => !string.IsNullOrEmpty(GlobalSelectGroup) && Abonents.Where(i => i.Groups.Contains(GlobalSelectGroup)).Count() > 0);
			LoadFileCommand = new CommandBase(OnLoadFile, (obj) => true);
			SaveFileCommand = new CommandBase(OnSaveFile, (obj) => true);
			SaveFileAsCommand = new CommandBase(OnSaveAsCommand, (obj) => true);
		}

		private void OnDeleteAbonent(object obj)
		{
			try
			{
				phoneBook.RemoveAbonent(GlobalSelectAbonent);
			}
			catch (Exception ex)
			{
				dialogService.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка");
			}
		}

		private void OnEditGroup(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromGroup.EditGroups, (value) => result = value);
			Abonents = phoneBook.GetAbonents();
			Groups = phoneBook.GetGroupsName();
		}

		private void OnDeleteGroup(object obj)
		{
			try
			{
				phoneBook.RemoveGroup(SelectGroup);
				SelectGroup = string.Empty;
			}
			catch (Exception ex)
			{
				dialogService.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка");
			}
		}

		private void OnSaveAsCommand(object obj)
		{
			if (Abonents.Count == 0)
			{
				dialogService.ShowMessageInfo("Нет абонентов для сохранения", "Сохранение");
			}
			else
			{
				string newfilePath = dialogService.SaveJsonFile();
				if (!string.IsNullOrEmpty(newfilePath))
				{
					try
					{
						phoneBook.Save(newfilePath);
						FileWay = newfilePath;
					}
					catch (Exception ex)
					{
						dialogService.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка");
					}
				}
			}
		}

		private void OnSaveFile(object obj)
		{
			if (Abonents.Count == 0)
			{
				dialogService.ShowMessageInfo("Нет абонентов для сохранения", "Сохранение");
			}
			else if (string.IsNullOrEmpty(FileWay))
			{
				string fileWay = dialogService.SaveJsonFile();
				if (string.IsNullOrEmpty(fileWay)) return;
				else
				{
					try
					{
						phoneBook.Save(fileWay);
					}
					catch (Exception ex)
					{
						dialogService.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка");
					}
				}
			}
			else
			{
				try
				{
					phoneBook.Save(FileWay);
				}
				catch (Exception ex)
				{
					dialogService.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка");
				}
			}
		}

		private void OnOpenAddAbonentDialog(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.AddAbonent, (value) => result = value);
			Abonents = phoneBook.GetAbonents();
			Groups = phoneBook.GetGroupsName();
		}

		private void OnOpenAddGroupDialog(object obj)
		{
			if (Abonents.Count != 0)
			{
				bool result = false;
				dialogService.ShowDialog(EnumDialogsWindowsFromGroup.AddGroup, (value) => result = value);
				Abonents = phoneBook.GetAbonents();
				Groups = phoneBook.GetGroupsName();
			}
		}

		private void OnOpenEditAbonentDialog(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.EditAbonent, (value) => result = value);
			Abonents = phoneBook.GetAbonents();
			Groups = phoneBook.GetGroupsName();
		}

		private void OnOpenAbonentInfoDialog(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.AbonentInfo, (value) => result = value);
		}

		private void OnLoadFile(object obj)
		{
			if (!phoneBook.IsSaved())
			{
				int result = dialogService.ShowMessageQuestion("Сохранить текущие изменения?", "Сохранение изменений");

				switch (result)
				{
					case 1:
						if (string.IsNullOrEmpty(FileWay))
						{
							FileWay = dialogService.SaveJsonFile();
							if (string.IsNullOrEmpty(FileWay)) return;

							try
							{
								phoneBook.Save(FileWay);
							}
							catch (Exception ex)
							{
								dialogService.ShowMessageException(ex.InnerException == null ? ex.Message : ex.InnerException.Message, "Ошибка сохранения");
								return;
							}
							phoneBook.Clear();
						}
						break;
					case -1:
						phoneBook.Clear();
						break;
					default:
						return;
				}
			}

			FileWay = dialogService.OpenJsonFile();
			if (!string.IsNullOrEmpty(FileWay))
			{
				try
				{
					phoneBook.Load(FileWay);
				}
				catch (Exception ex)
				{
					dialogService.ShowMessageException(ex.InnerException == null ? ex.Message : ex.InnerException.Message, "Ошибка при открытии файла");
				}
			}
			Abonents = phoneBook.GetAbonents();
			Groups = phoneBook.GetGroupsName();
		}
	}
}
