using GroceryPOS.Core.Enums;
using GroceryPOS.Core.Interfaces.Services;
using GroceryPOS.Desktop.ViewModels.Base;
using GroceryPOS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GroceryPOS.Desktop.ViewModels.InitialSetup
{
    public class InitialSetupViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly Action _onSetupComplete;

        // Properties for user input
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public ICommand CreateAdminCommand { get; }

        public InitialSetupViewModel(IAuthService authService, Action onSetupComplete)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _onSetupComplete = onSetupComplete ?? throw new ArgumentNullException(nameof(onSetupComplete));

            CreateAdminCommand = new RelayCommand(ExecuteCreateAdmin, CanExecuteCreateAdmin);
        }

        private bool CanExecuteCreateAdmin(object? parameter)
        {
            // Basic validation for fields
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   Password.Length >= 6 && // Minimum password length
                   Password == ConfirmPassword;
        }

        private async void ExecuteCreateAdmin(object? parameter)
        {
            if (!CanExecuteCreateAdmin(null)) return;

            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                AppLogger.LogInfo($"Attempting to create initial admin user: {Username}");

                // Check if an admin user was somehow created by another process in the meantime
                if (await _authService.IsAdminUserCreatedAsync())
                {
                    ErrorMessage = "An admin user already exists. Navigating to login.";
                    AppLogger.LogWarning("Initial setup aborted: Admin user already exists.");
                    // Allow a small delay then proceed to login
                    await Task.Delay(2000);
                    _onSetupComplete?.Invoke();
                    return;
                }

                bool registered = await _authService.RegisterUserAsync(
                    Username, Password, FirstName, LastName, UserRole.Admin);

                if (registered)
                {
                    SuccessMessage = "Admin user created successfully! Redirecting to login...";
                    AppLogger.LogInfo($"Initial admin user '{Username}' created successfully.");
                    // Allow user to read success message before navigating
                    await Task.Delay(2000);
                    _onSetupComplete?.Invoke(); // Trigger navigation to login
                }
                else
                {
                    ErrorMessage = "Failed to create admin user. Username might be taken or other issue.";
                    AppLogger.LogError($"Failed to create initial admin user '{Username}'.");
                }
            }
            catch (ApplicationException ex)
            {
                ErrorMessage = $"Setup error: {ex.Message}";
                AppLogger.LogError("ApplicationException during initial admin setup.", ex);
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred during setup. Please try again.";
                AppLogger.LogError("Unexpected error during initial admin setup.", ex);
            }
            finally
            {
                IsLoading = false;
                ((RelayCommand)CreateAdminCommand).RaiseCanExecuteChanged();
            }
        }

        // This method is important for the PasswordBox to notify the ViewModel
        public void RaiseCanExecuteChangedForPassword()
        {
            ((RelayCommand)CreateAdminCommand).RaiseCanExecuteChanged();
        }
    }
}
