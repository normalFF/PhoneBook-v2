using System;

namespace Phone_Book.Infrastructure.DialogService.Groups
{
	internal interface IDialogGroupService : IDialog
	{
		void ShowDialog(EnumDialogsWindowsFromGroup groupEnum, Action<bool> actionCallBack);
	}
}
