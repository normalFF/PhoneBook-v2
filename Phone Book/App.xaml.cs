using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using PhoneLibrary;
using Phone_Book.Models;
using Phone_Book.Infrastructure.DialogService;
using Phone_Book.Infrastructure.DialogService.Abonents;
using Phone_Book.Infrastructure.DialogService.Groups;

namespace Phone_Book
{
	public partial class App : Application
	{
		private static IHost _host;
		public static IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

		public static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
		{
			services.AddSingleton<IPhoneBook, PhoneBook>();
			services.AddSingleton<IDialogService, ServiceDialogs>();
			services.AddSingleton<IDialogAbonentService, ServiceDialogs>();
			services.AddSingleton<IDialogGroupService, ServiceDialogs>();
			services.AddSingleton<IDialog, ServiceDialogs>();
			services.AddSingleton<MainViewModel>();
			services.AddTransient<CreateAbonent>();
			services.AddTransient<BrowseAbonent>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			IHost host = Host;
			base.OnStartup(e);
			await host.StartAsync().ConfigureAwait(false);
		}

		protected override async void OnExit(ExitEventArgs e)
		{
			IHost host = Host;
			await host.StopAsync().ConfigureAwait(false);
			host.Dispose();
			base.OnExit(e);
		}
	}
}
