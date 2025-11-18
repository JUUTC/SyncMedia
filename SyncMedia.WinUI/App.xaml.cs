using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SyncMedia.Core.Interfaces;
using SyncMedia.Core.Services;
using SyncMedia.WinUI.Services;
using SyncMedia.WinUI.ViewModels;
using SyncMedia.WinUI.Views;
using System;

namespace SyncMedia.WinUI
{
    public partial class App : Application
    {
        private static Window s_window;
        private static IServiceProvider s_serviceProvider;

        public static Window MainWindow => s_window;

        public App()
        {
            this.InitializeComponent();
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register Core services
            services.AddSingleton<LicenseManager>();
            services.AddSingleton(provider => 
            {
                var licenseManager = provider.GetRequiredService<LicenseManager>();
                return new FeatureFlagService(licenseManager.CurrentLicense);
            });
            
            // Register UI services
            services.AddSingleton<IAdvertisingService, MicrosoftAdvertisingService>();

            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<FolderConfigurationViewModel>();
            services.AddTransient<FileTypesViewModel>();
            services.AddTransient<NamingListViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SyncViewModel>();
            services.AddTransient<FilesViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
            services.AddTransient<HomePage>();
            services.AddTransient<FolderConfigurationPage>();
            services.AddTransient<FileTypesPage>();
            services.AddTransient<NamingListPage>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<SyncPage>();
            services.AddTransient<FilesPage>();

            s_serviceProvider = services.BuildServiceProvider();
        }

        public static T GetService<T>() where T : class
        {
            return s_serviceProvider.GetRequiredService<T>();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            s_window = GetService<MainWindow>();
            s_window.Activate();
        }
    }
}
