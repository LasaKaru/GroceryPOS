using GroceryPOS.Utilities.Logging;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection; // For IServiceCollection
using System;
using GroceryPOS.Core; // For AddCoreServices extension method

namespace GroceryPOS.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            // Determine the base application directory where our DB and logs will reside
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GroceryPOS");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            string databasePath = Path.Combine(appDataPath, "GroceryPOS.db"); // Our main database file

            // Configure and build the host for Dependency Injection
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Add logging if not already setup by CreateDefaultBuilder (it usually adds console logging)
                    // services.AddSingleton<ILoggerProvider, ...>(); // For more advanced logging

                    // Add all core services and repositories via our extension method
                    services.AddCoreServices(databasePath);

                    // Register WPF windows and view models (we'll add these later)
                    services.AddSingleton<MainWindow>(); // MainWindow is a singleton
                    // services.AddTransient<LoginView>(); // Views can be transient
                    // services.AddTransient<LoginViewModel>(); // ViewModels can be transient or scoped
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Start the host, which will initialize all registered services
                await _host.StartAsync();

                // Resolve the main window from the DI container
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.Show();
                AppLogger.LogInfo("Application started successfully.");
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Application failed to start.", ex);
                MessageBox.Show($"Application failed to start: {ex.Message}\nCheck logs for more details.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(); // Close the application gracefully
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Stop the host gracefully when the application exits
            if (_host != null)
            {
                AppLogger.LogInfo("Application shutting down.");
                await _host.StopAsync();
                _host.Dispose(); // Dispose of the host resources
                AppLogger.LogInfo("Application shut down complete.");
            }
        }
    }

}
