﻿using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Globalization;

using Ehrungsprogramm.Contracts.Services;
using Ehrungsprogramm.Contracts.Views;
using Ehrungsprogramm.Core.Contracts.Services;
using Ehrungsprogramm.Core.Services;
using Ehrungsprogramm.Models;
using Ehrungsprogramm.Services;
using Ehrungsprogramm.ViewModels;
using Ehrungsprogramm.Views;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MahApps.Metro.Controls.Dialogs;

namespace Ehrungsprogramm
{
    // For more inforation about application lifecyle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

    // WPF UI elements use language en-US by default.
    // If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
    // Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
    public partial class App : Application
    {
        private IHost _host;

        public T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public App()
        {
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            // Set the language for all FrameworkElements to the current culture
            // see: https://serialseb.com/blog/2007/04/03/wpf-tips-1-have-all-your-dates-times/
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c =>
                    {
                        c.SetBasePath(appLocation);
                    })
                    .ConfigureServices(ConfigureServices)
                    .Build();

            await _host.StartAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // TODO WTS: Register your services, viewmodels and pages here

            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Activation Handlers

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Services
            services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPersonService, PersonService>();
            services.AddSingleton<IPrintService, PrintService>();
            services.AddSingleton<IDialogCoordinator>(DialogCoordinator.Instance);

            // Views and ViewModels
            services.AddTransient<IShellWindow, ShellWindow>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();

            services.AddTransient<ManageDatabaseViewModel>();
            services.AddTransient<ManageDatabasePage>();

            services.AddTransient<PersonsViewModel>();
            services.AddTransient<PersonsPage>();

            services.AddTransient<RewardsBLSVViewModel>();
            services.AddTransient<RewardsBLSVPage>();

            services.AddTransient<RewardsTSVViewModel>();
            services.AddTransient<RewardsTSVPage>();

            services.AddTransient<PersonDetailViewModel>();
            services.AddTransient<PersonDetailPage>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
        }
    }
}
