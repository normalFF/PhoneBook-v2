using Phone_Book.Infrastructure.DialogService.Abonents;
using Phone_Book.Infrastructure.DialogService.Groups;

namespace Phone_Book.Infrastructure.DialogService
{
	internal interface IDialogService : IDialogAbonentService, IDialogGroupService
	{
	}
}
