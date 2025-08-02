using GroceryPOS.Core.Enums;
using GroceryPOS.Core.Interfaces.Services;
using GroceryPOS.Desktop.ViewModels.Authentication;
using GroceryPOS.Desktop.ViewModels.Base;
using GroceryPOS.Desktop.Views.Authentication;
using GroceryPOS.Utilities.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
//using GroceryPOS.Desktop.ViewModels.InitialSetup; // For InitialSetupViewModel (placeholder)
//using GroceryPOS.Desktop.Views.InitialSetup; // For InitialSetupView (placeholder)

namespace GroceryPOS.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider; // To resolve views/viewmodels
        private readonly IAuthService _authService;

        private UserControl? _currentView;
        public UserControl? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public MainWindowViewModel(IServiceProvider serviceProvider, IAuthService authService)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            // Determine initial view to show
            InitializeApplicationFlowAsync().ConfigureAwait(false);
        }

        private async Task InitializeApplicationFlowAsync()
        {
            try
            {
                AppLogger.LogInfo("Initializing application flow...");

                // Check if an admin user exists (for initial setup)
                bool isAdminCreated = await _authService.IsAdminUserCreatedAsync();

                if (!isAdminCreated)
                {
                    AppLogger.LogInfo("No admin user found. Navigating to Initial Setup.");
                    // Navigate to Initial Setup View/ViewModel
                    NavigateToInitialSetup();
                }
                else
                {
                    AppLogger.LogInfo("Admin user found. Navigating to Login.");
                    // Navigate to Login View/ViewModel
                    NavigateToLogin();
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError("Error during application flow initialization.", ex);
                // Show a critical error and possibly close app
                ErrorMessage = "A critical error occurred during startup. Please restart the application.";
                // In a real app, you might want a specific error view here.
            }
        }

        private void NavigateToLogin()
        {
            // Resolve LoginView and LoginViewModel from DI container
            // Pass a callback action to the LoginViewModel for when login is successful
            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            loginViewModel = new LoginViewModel(_authService, OnLoginSuccess); // Re-instantiate to provide callback

            var loginView = new LoginView { DataContext = loginViewModel };
            CurrentView = loginView;
            AppLogger.LogInfo("Navigated to LoginView.");
        }

        private void NavigateToInitialSetup()
        {
            // For now, let's just show a simple message or a placeholder view
            // We will create the actual InitialSetup views in the next steps
            var initialSetupView = new UserControl(); // Placeholder
            initialSetupView.Content = "Welcome! No admin user found. Please complete initial setup.";
            CurrentView = initialSetupView;
            AppLogger.LogInfo("Navigated to Initial Setup Placeholder.");
            // TODO: In next steps, replace this with actual InitialSetupView and ViewModel
            // var initialSetupViewModel = _serviceProvider.GetRequiredService<InitialSetupViewModel>();
            // initialSetupViewModel = new InitialSetupViewModel(_authService, OnSetupComplete); // Provide callback
            // var initialSetupView = new InitialSetupView { DataContext = initialSetupViewModel };
            // CurrentView = initialSetupView;
        }

        private void OnLoginSuccess(UserRole userRole)
        {
            AppLogger.LogInfo($"Login success callback received. User Role: {userRole}");
            // Here, based on userRole, you would navigate to the main dashboard or specific screens.
            // For now, let's just show a success message or a placeholder.
            var dashboardView = new UserControl(); // Placeholder for Dashboard
            dashboardView.Content = $"Login successful! Welcome, you are a {userRole}.";
            CurrentView = dashboardView;
            AppLogger.LogInfo("Navigated to Dashboard Placeholder.");
            // TODO: In later steps, replace with actual DashboardView and ViewModel
        }

        // Other navigation methods (e.g., NavigateToDashboard, NavigateToSettings) would go here
    }
}
