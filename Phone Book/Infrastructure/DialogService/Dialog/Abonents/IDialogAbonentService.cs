using System;

namespace Phone_Book.Infrastructure.DialogService.Abonents
{
	internal interface IDialogAbonentService : IDialog
	{
		void ShowDialog(EnumDialogsWindowsFromAbonent abonentEnum, Action<bool> actionCallBack);
	}
}
