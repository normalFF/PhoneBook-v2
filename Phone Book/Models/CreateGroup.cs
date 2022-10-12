using Phone_Book.Models.Base;
using PhoneLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Phone_Book.Models
{
	internal class CreateGroup : BaseViewModel
	{
		private IPhoneBook _phoneBook;
		private string _groupName;
		private IEnumerable<AbonentModel> abonents;

		public string GroupName
		{
			get => _groupName;
			set => Set(ref _groupName, value);
		}
		public IEnumerable<AbonentModel> Abonents
		{
			get => abonents;
			set
			{
				if (abonents == value) OnPropertyChanged(nameof(Abonents));
				else Set(ref abonents, value);
			}
		}
		public ObservableCollection<AbonentModel> SelectAbonents { get; set; }

		public CreateGroup(IPhoneBook book)
		{
			_phoneBook = book;
			SelectAbonents = new ObservableCollection<AbonentModel>();
		}
	}
}
