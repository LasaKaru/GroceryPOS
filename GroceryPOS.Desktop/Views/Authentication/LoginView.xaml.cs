using GroceryPOS.Desktop.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GroceryPOS.Desktop.Views.Authentication
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        // Event handler for PasswordBox PasswordChanged event
        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel viewModel)
            {
                // Update the ViewModel's Password property from the PasswordBox
                viewModel.Password = PasswordBox.Password;
                // Manually trigger CanExecuteChanged for the LoginCommand
                // because Password property is not bound with UpdateSourceTrigger.
                viewModel.RaiseLoginCanExecuteChanged();
            }
        }
    }
}
