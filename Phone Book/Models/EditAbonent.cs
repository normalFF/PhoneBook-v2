using Phone_Book.Infrastructure.Command;
using Phone_Book.Infrastructure.DialogService;
using Phone_Book.Models.Base;
using PhoneLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System;
using Phone_Book.Infrastructure.WindowClose;
using System.Windows.Controls;
using System.Linq;

namespace Phone_Book.Models
{
	internal class EditAbonent : BaseViewModel, IWindowClose
	{
		public bool CanClose()
		{
			if (model is not null)
			{
				return TryUpdateUser(model);
			}
			else
			{
				int result = dialogs.ShowMessageQuestion("Закрыть окно редактирования пользователя?", "Выход");

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
		private IReadOnlyCollection<string> phoneBookGroups;
		private IReadOnlyCollection<string> phoneTypeCollection;
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
		private string abonentImageBase64;
		private Image abonentImage;

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
		public IReadOnlyCollection<string> PhoneBookGroups
		{
			get => phoneBookGroups;
			set
			{
				if (phoneBookGroups != value) Set(ref phoneBookGroups, value);
				else OnPropertyChanged(nameof(PhoneBookGroups));
			}
		}
		public IReadOnlyCollection<string> PhoneTypeCollection
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
		public Image AbonentImage
		{
			get => abonentImage;
			set => Set(ref abonentImage, value);
		}


		public ICommand AddAbonentInPhoneBook { get; private set; }
		public ICommand CloseWindow { get; private set; }
		public ICommand AddPhoneCommand { get; private set; }
		public ICommand DeletePhoneCommand { get; private set; }
		public ICommand AddGroupCommand { get; private set; }
		public ICommand DeleteGroupCommand { get; private set; }
		public ICommand AddImageCommand { get; private set; }
		public ICommand DeleteImageCommand { get; private set; }

		public EditAbonent(IPhoneBook phoneBook, IDialog dialogWindows)
		{
			dialogs = dialogWindows;
			this.phoneBook = phoneBook;
			SelectedGroups = new();
			PhoneBookGroups = this.phoneBook.GetGroupsName();
			PhoneTypeCollection = this.phoneBook.GetPhoneTypes();

			Name = GlobalSelectAbonent.Name;
			Surname = GlobalSelectAbonent.Surname;
			Residence = GlobalSelectAbonent.Residence;
			DateOfBirth = GlobalSelectAbonent.DateOfBirth;
			Phones = new ObservableCollection<PhoneNumberModel>(GlobalSelectAbonent.Phones);
			Groups = new ObservableCollection<string>(GlobalSelectAbonent.Groups);
			if (GlobalSelectAbonent.ImageBase64 is not null)
			{
				TryAddImage(Convert.FromBase64String(GlobalSelectAbonent.ImageBase64));
			}

			CommandInitialize();
		}

		private void CommandInitialize()
		{
			DeleteGroupCommand = new CommandBase(OnDeleteGroup, (obj) => !string.IsNullOrEmpty(SelectAbonentGroup) && SelectedGroups.Contains(SelectAbonentGroup));
			DeletePhoneCommand = new CommandBase(OnDeletePhone, (obj) => Phones.Contains(SelectPhone));
			AddGroupCommand = new CommandBase(OnAddGroup, (obj) => !string.IsNullOrEmpty(SelectPhoneBookGroup) && !SelectedGroups.Contains(SelectPhoneBookGroup));
			AddPhoneCommand = new CommandBase(OnAddPhone, (obj) => (!string.IsNullOrEmpty(SelectType) || !string.IsNullOrEmpty(InputNewType)) && !string.IsNullOrEmpty(InputNewPhone));
			AddAbonentInPhoneBook = new CommandBase(OnAddAbonentInPhoneBook, (obj) => !string.IsNullOrEmpty(Name) && Phones.Count != 0);
			CloseWindow = new CommandBase(OnCloseWindow, (obj) => true);
			AddImageCommand = new CommandBase(OnAddImage, (obj) => true);
			DeleteImageCommand = new CommandBase(OnDeleteImage, (obj) => !string.IsNullOrEmpty(abonentImageBase64));
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
			try
			{
				phoneBook.ValidatePhoneNumber(newPhone);
			}
			catch (Exception ex)
			{
				dialogs.ShowMessageException(ex.InnerException is null ? ex.Message : ex.InnerException.Message, "Ошибка валидации");
				return;
			}
			if (!Phones.Contains(newPhone))
			{
				Phones.Add(newPhone);
				InputNewType = string.Empty;
				InputNewPhone = string.Empty;
			}
			else
			{
				dialogs.ShowMessageInfo("Телефон с данным номером уже есть в списке", "Внимание");
			}
		}

		private void OnCloseWindow(object obj)
		{
			model = null;
			Close.Invoke();
		}

		private void OnAddAbonentInPhoneBook(object obj)
		{
			model = (AbonentModel)GlobalSelectAbonent.Clone();
			model.Name = Name;
			model.Surname = Surname;
			model.Residence = Residence;
			model.Phones = Phones.ToArray();
			model.Groups = SelectedGroups.ToArray();
			model.DateOfBirth = DateOfBirth;
			model.ImageBase64 = abonentImageBase64;
			Close.Invoke();
		}

		private bool TryUpdateUser(AbonentModel abonent)
		{
			try
			{
				phoneBook.UpdateAbonent(abonent);
			}
			catch (Exception ex)
			{
				dialogs.ShowMessageException(ex.InnerException == null ? ex.Message : ex.InnerException.Message, "Ошибка внесения изменений");
				return false;
			}
			return true;
		}

		private void OnAddImage(object obj)
		{
			var result = dialogs.OpenImageFile();
			if (!string.IsNullOrEmpty(result))
			{
				var imageArray = File.ReadAllBytes(result);
				TryAddImage(imageArray);
			}
		}

		private void TryAddImage(byte[] bytes)
		{
			abonentImageBase64 = Convert.ToBase64String(bytes);
			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = new MemoryStream(bytes);
			bi.EndInit();

			Image nImage = new Image();
			nImage.Source = bi;
			AbonentImage = nImage;
		}

		private void OnDeleteImage(object obj)
		{
			abonentImageBase64 = string.Empty;
			AbonentImage = null;
		}
	}
}
