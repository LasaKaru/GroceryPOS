using GroceryPOS.Core.Enums;
using GroceryPOS.Desktop.ViewModels.Base;
using GroceryPOS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroceryPOS.Desktop.ViewModels
{
    public class MainDashboardViewModel : ViewModelBase
    {
        private UserRole _currentUserRole; // Store the logged-in user's role
        private UserControl? _currentContent; // The actual content view shown in the dashboard

        public UserControl? CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        public ICommand NavigateToHomeCommand { get; }
        public ICommand NavigateToProductsCommand { get; }
        public ICommand NavigateToSalesCommand { get; }
        public ICommand NavigateToUsersCommand { get; } // For Admin only
        public ICommand LogoutCommand { get; }

        // Callback for logout action
        private readonly Action _onLogout;

        public MainDashboardViewModel(UserRole userRole, Action onLogout)
        {
            _currentUserRole = userRole;
            _onLogout = onLogout ?? throw new ArgumentNullException(nameof(onLogout));

            // Initialize commands
            NavigateToHomeCommand = new RelayCommand(ExecuteNavigateToHome);
            NavigateToProductsCommand = new RelayCommand(ExecuteNavigateToProducts);
            NavigateToSalesCommand = new RelayCommand(ExecuteNavigateToSales);
            NavigateToUsersCommand = new RelayCommand(ExecuteNavigateToUsers, CanExecuteNavigateToUsers); // Conditional for admin
            LogoutCommand = new RelayCommand(ExecuteLogout);

            // Set initial content
            ExecuteNavigateToHome(null);
        }

        // Command Executions
        private void ExecuteNavigateToHome(object? parameter)
        {
            // Placeholder for Home View
            CurrentContent = new UserControl { Content = "Welcome to the Dashboard!" };
            AppLogger.LogInfo("Navigated to Home content.");
            // TODO: Replace with actual HomeView
        }

        private void ExecuteNavigateToProducts(object? parameter)
        {
            // Placeholder for Products View
            CurrentContent = new UserControl { Content = "Products Management (Coming Soon!)" };
            AppLogger.LogInfo("Navigated to Products content.");
            // TODO: Replace with actual ProductsView
        }

        private void ExecuteNavigateToSales(object? parameter)
        {
            // Placeholder for Sales View
            CurrentContent = new UserControl { Content = "Sales Operations (Coming Soon!)" };
            AppLogger.LogInfo("Navigated to Sales content.");
            // TODO: Replace with actual SalesView
        }

        private bool CanExecuteNavigateToUsers(object? parameter)
        {
            // Only allow navigation to Users if the current user is an Admin
            return _currentUserRole == UserRole.Admin;
        }

        private void ExecuteNavigateToUsers(object? parameter)
        {
            // Placeholder for Users View (Admin Only)
            CurrentContent = new UserControl { Content = $"User Management (Access for {_currentUserRole} Only!)" };
            AppLogger.LogInfo($"Navigated to Users content by {_currentUserRole}.");
            // TODO: Replace with actual UsersView
        }

        private void ExecuteLogout(object? parameter)
        {
            AppLogger.LogInfo($"User with role {_currentUserRole} initiating logout.");
            _onLogout?.Invoke(); // Trigger the logout action in MainWindowViewModel
        }
    }
}
