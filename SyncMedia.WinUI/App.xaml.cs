using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SyncMedia.WinUI.Services;
using SyncMedia.WinUI.ViewModels;
using SyncMedia.WinUI.Views;
using System;

namespace SyncMedia.WinUI
{
    public partial class App : Application
    {
        private Window m_window;
        private IServiceProvider _serviceProvider;

        public App()
        {
            this.InitializeComponent();
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register Core services
            // services.AddSingleton<ISyncService, SyncService>();

            // Register ViewModels
            services.AddTransient<MainViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = _serviceProvider.GetRequiredService<MainWindow>();
            m_window.Activate();
        }
    }
}
