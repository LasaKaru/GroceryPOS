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

namespace GroceryPOS.Desktop.ViewModels.Authentication
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly Action<UserRole> _onLoginSuccess; // Callback for successful login

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password = string.Empty;
        public string Password // This property should ideally be handled carefully in a secure app
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }

        // Constructor for Dependency Injection
        public LoginViewModel(IAuthService authService, Action<UserRole> onLoginSuccess)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _onLoginSuccess = onLoginSuccess ?? throw new ArgumentNullException(nameof(onLoginSuccess));

            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);

            // Check if admin user is already created on startup
            // This is asynchronous, but we can't await in a constructor directly.
            // We'll handle this check more robustly during application startup flow.
            // For now, assume a default admin needs to be created if not present.
        }

        private bool CanExecuteLogin(object? parameter)
        {
            // Can only execute login if username and password are not empty and not currently loading
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !IsLoading;
        }

        private async void ExecuteLogin(object? parameter)
        {
            // We can use a PasswordBox for secure password input in XAML.
            // The 'parameter' here might be the PasswordBox.Password property if passed.
            // For simplicity here, we're using the bound Password property.
            if (!CanExecuteLogin(null)) return; // Re-check condition just in case

            IsLoading = true;
            ErrorMessage = string.Empty; // Clear previous errors
            SuccessMessage = string.Empty; // Clear previous success messages

            try
            {
                AppLogger.LogInfo($"Login attempt for user: {Username}");
                var user = await _authService.AuthenticateUserAsync(Username, Password);

                if (user != null)
                {
                    SuccessMessage = "Login successful!";
                    AppLogger.LogInfo($"User {Username} logged in successfully with role {user.Role}.");
                    _onLoginSuccess?.Invoke(user.Role); // Invoke the callback for successful login
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                    AppLogger.LogWarning($"Login failed for user: {Username}. Invalid credentials.");
                }
            }
            catch (ApplicationException ex) // Catch our custom application exceptions
            {
                ErrorMessage = $"Login error: {ex.Message}";
                AppLogger.LogError($"ApplicationException during login for {Username}.", ex);
            }
            catch (Exception ex) // Catch any other unexpected errors
            {
                ErrorMessage = "An unexpected error occurred during login. Please try again.";
                AppLogger.LogError($"Unexpected error during login for {Username}.", ex);
            }
            finally
            {
                IsLoading = false;
                // Trigger CanExecuteChanged for the LoginCommand to re-evaluate its state
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        // Method to manually raise CanExecuteChanged for LoginCommand
        public void RaiseLoginCanExecuteChanged()
        {
            ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
        }
    }
}
