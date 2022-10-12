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
						if (string.IsNullOrEmpty(fileWay))
						{
							fileWay = dialogService.SaveJsonFile();
							if (string.IsNullOrEmpty(fileWay)) return false;
							else
							{
								try
								{
									phoneBook.Save(fileWay);
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
								phoneBook.Save(fileWay);
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
		public ICommand LoadFileCommand { get; private set; }

		private IDialogService dialogService;
		private IPhoneBook phoneBook;
		private IEnumerable<AbonentModel> abonents;
		private IEnumerable<string> groups;
		private AbonentModel selectAbonent;
		private AbonentModel selectAbonentInGroup;
		private string selectGroup;
		private string fileWay;

		public IEnumerable<AbonentModel> Abonents
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
			set => Set(ref selectAbonent, value);
		}
		public AbonentModel SelectAbonentInGroup
		{
			get => selectAbonentInGroup;
			set => Set(ref selectAbonentInGroup, value);
		}
		public IEnumerable<string> Groups
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
			set => Set(ref selectGroup, value);
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
			OpenEditAbonentDialog = new CommandBase(OnOpenEditAbonentDialog, (SelectAbonent) => SelectAbonent != null);
			OpenAbonentInfoDialog = new CommandBase(OnOpenAbonentInfoDialog, (SelectAbonent) => SelectAbonent != null);
			LoadFileCommand = new CommandBase(OnLoadFile, (obj) => true);
		}

		private void OnOpenAddAbonentDialog(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.AddAbonent, (value) => result = value);
			OnPropertyChanged(nameof(Abonents));
		}

		private void OnOpenAddGroupDialog(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromGroup.AddGroup, (value) => result = value);
		}

		private void OnOpenEditAbonentDialog(object obj)
		{
			bool result = false;
			dialogService.ShowDialog(EnumDialogsWindowsFromAbonent.EditAbonent, (value) => result = value);
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
						if (string.IsNullOrEmpty(fileWay))
						{
							fileWay = dialogService.SaveJsonFile();
							if (string.IsNullOrEmpty(fileWay)) return;

							try
							{
								phoneBook.Save(fileWay);
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

			fileWay = dialogService.OpenJsonFile();
			if (!string.IsNullOrEmpty(fileWay))
			{
				try
				{
					phoneBook.Load(fileWay);
				}
				catch (Exception ex)
				{
					dialogService.ShowMessageException(ex.InnerException == null ? ex.Message : ex.InnerException.Message, "Ошибка при открытии файла");
				}
			}
		}
	}
}
