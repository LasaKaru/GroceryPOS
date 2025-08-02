//using GroceryPOS.Utilities.Logging;
//using Microsoft.Extensions.Hosting;
//using System.Configuration;
//using System.Data;
//using System.IO;
//using System.Windows;
//using Microsoft.Extensions.DependencyInjection; // For IServiceCollection
//using System;
//using GroceryPOS.Core; // For AddCoreServices extension method
//using GroceryPOS.Desktop.Views.Authentication; // For LoginView
//using GroceryPOS.Desktop.ViewModels.Authentication;
using GroceryPOS.Core; // For AddCoreServices extension method
using GroceryPOS.Core.Context;
using GroceryPOS.Core.Interfaces.Repositories; // For IRepository, IUserRepository
using GroceryPOS.Data.Repositories; // For UserRepository
using GroceryPOS.Data.Repositories.Base; // For RepositoryBase
using GroceryPOS.Desktop.ViewModels; // For MainWindowViewModel
using GroceryPOS.Desktop.ViewModels.Authentication; // For LoginViewModel
using GroceryPOS.Desktop.ViewModels.InitialSetup;
using GroceryPOS.Desktop.Views.Authentication; // For LoginView
using GroceryPOS.Utilities.Logging; // For AppLogger
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;
using GroceryPOS.Desktop.Views.InitialSetup;

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
            // Determine the base application directory for our DB and logs
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GroceryPOS");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            string databasePath = Path.Combine(appDataPath, "GroceryPOS.db"); // Our main database file

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // 1. Register Core Services (from GroceryPOS.Core)
                    // This includes AppDbContext, IAuthService, AuthService
                    services.AddCoreServices(databasePath);

                    // 2. Register Repository Implementations (from GroceryPOS.Data)
                    // Repositories are typically scoped to a request/operation
                    services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
                    services.AddScoped<IUserRepository, UserRepository>();

                    // 3. Register WPF Windows and ViewModels (from GroceryPOS.Desktop)
                    services.AddSingleton<MainWindow>(); // MainWindow is a singleton
                    services.AddSingleton<MainWindowViewModel>(); // MainWindowViewModel is a singleton

                    // LoginViewModel and LoginView are typically transient as we might want new instances
                    // each time we navigate to them, or to pass specific data (like a callback).
                    // Note: For LoginViewModel, we'll manually instantiate it in MainWindowViewModel
                    // to pass the OnLoginSuccess callback, so it doesn't strictly need to be registered
                    // here if only instantiated manually. However, registering it is good practice
                    // if other parts of the app might resolve it directly via IServiceProvider.
                    //services.AddTransient<LoginViewModel>();
                    //services.AddTransient<LoginView>();

                    // Initial setup view/viewmodel placeholders
                    // services.AddTransient<InitialSetupViewModel>();
                    // services.AddTransient<InitialSetupView>();
                    services.AddTransient<InitialSetupViewModel>();
                    services.AddTransient<Views.InitialSetup.InitialSetupView>();

                    services.AddTransient<MainDashboardViewModel>(); // MainDashboardViewModel could be singleton or transient depending on app scope, but transient is safer for now.
                    services.AddTransient<Views.MainDashboardView>(); // MainDashboardView
                })
                .Build();
        }

        //protected override async void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    try
        //    {
        //        // Start the host, which will initialize all registered services
        //        await _host.StartAsync();

        //        // Resolve the main window from the DI container
        //        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        //        mainWindow.Show();
        //        AppLogger.LogInfo("Application started successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        AppLogger.LogError("Application failed to start.", ex);
        //        MessageBox.Show($"Application failed to start: {ex.Message}\nCheck logs for more details.", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        Shutdown(); // Close the application gracefully
        //    }
        //}
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // 1. Start the host, which initializes all registered services
                await _host.StartAsync();

                // *** FIX: Add the database migration logic here ***
                // Use a DI scope to get services for a single operation.
                using (var scope = _host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var dbContext = services.GetRequiredService<AppDbContext>();

                    // This is the key line: It creates the database if it doesn't exist
                    // and applies all pending migrations.
                    await dbContext.Database.MigrateAsync();

                    AppLogger.LogInfo("Database migrations applied successfully.");
                }
                // *** END OF FIX ***

                // 2. Resolve the main window from the DI container and show it
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
