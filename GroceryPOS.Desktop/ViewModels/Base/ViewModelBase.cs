using GroceryPOS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Desktop.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                    // Optionally, log the error message
                    if (!string.IsNullOrEmpty(value))
                    {
                        AppLogger.LogWarning($"UI Error Message: {value}");
                    }
                }
            }
        }

        private string _successMessage = string.Empty;
        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                if (_successMessage != value)
                {
                    _successMessage = value;
                    OnPropertyChanged();
                    // Optionally, log the success message
                    if (!string.IsNullOrEmpty(value))
                    {
                        AppLogger.LogInfo($"UI Success Message: {value}");
                    }
                }
            }
        }


        /// <summary>
        /// Raises the PropertyChanged event for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. If not provided,
        /// the name of the calling member is used.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the value of a property and raises PropertyChanged if the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">A reference to the backing field of the property.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="propertyName">The name of the property. Automatically inferred if not provided.</param>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
