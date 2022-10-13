using Phone_Book.Models.Base;
using PhoneLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Phone_Book.Models
{
	internal class BrowseAbonent : BaseViewModel
	{
		private string name;
		private string surname;
		private string residence;
		private DateTime? dateOfBirth;
		private List<string> groups;
		private List<PhoneNumberModel> phones;
		private Image image;

		public string Name
		{
			get => name; set => Set(ref name, value);
		}
		public string Surname
		{
			get => surname; set => Set(ref surname, value);
		}
		public string Residence
		{
			get => residence; set => Set(ref residence, value);
		}
		public DateTime? DateOfBirth
		{
			get => dateOfBirth; set => Set(ref dateOfBirth, value);
		}
		public List<string> Groups
		{
			get => groups; set => Set(ref groups, value);
		}
		public List<PhoneNumberModel> Phones
		{
			get => phones; set => Set(ref phones, value);
		}
		public Image Image
		{
			get => image; set => Set(ref image, value);
		}

		public BrowseAbonent()
		{
			Name = GlobalSelectAbonent.Name;
			Surname = GlobalSelectAbonent.Surname;
			Residence = GlobalSelectAbonent.Residence;
			DateOfBirth = GlobalSelectAbonent.DateOfBirth;
			Groups = GlobalSelectAbonent.Groups.ToList();
			Phones = GlobalSelectAbonent.Phones.ToList();
			if (!string.IsNullOrEmpty(GlobalSelectAbonent.ImageBase64))
			{
				byte[] imageArray = Convert.FromBase64String(GlobalSelectAbonent.ImageBase64);
				BitmapImage bi = new BitmapImage();
				bi.BeginInit();
				bi.CacheOption = BitmapCacheOption.OnLoad;
				bi.StreamSource = new MemoryStream(imageArray);
				bi.EndInit();

				Image nImage = new Image();
				nImage.Source = bi;
				Image = nImage;
			}
		}
	}
}
