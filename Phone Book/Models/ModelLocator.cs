using Microsoft.Extensions.DependencyInjection;
using Phone_Book.Infrastructure.DialogService;
using PhoneLibrary;

namespace Phone_Book.Models
{
	internal class ModelLocator
	{
		public MainViewModel MainModel => App.Host.Services.GetRequiredService<MainViewModel>();
		public CreateAbonent CreateAbonentModel => App.Host.Services.GetRequiredService<CreateAbonent>();
		public CreateGroup CreateGroupModel => App.Host.Services.GetRequiredService<CreateGroup>();
		public IPhoneBook PhoneBook => App.Host.Services.GetRequiredService<IPhoneBook>();
		public IDialog SystemDialogWindows => App.Host.Services.GetRequiredService<IDialog>();
	}
}
