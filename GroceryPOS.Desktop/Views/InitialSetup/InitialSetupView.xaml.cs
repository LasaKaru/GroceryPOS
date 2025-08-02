using GroceryPOS.Desktop.ViewModels.InitialSetup;
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

namespace GroceryPOS.Desktop.Views.InitialSetup
{
    /// <summary>
    /// Interaction logic for InitialSetupView.xaml
    /// </summary>
    public partial class InitialSetupView : UserControl
    {
        public InitialSetupView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is InitialSetupViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
                viewModel.RaiseCanExecuteChangedForPassword(); // Trigger CanExecute re-evaluation
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.DataContext is InitialSetupViewModel viewModel)
            {
                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
                viewModel.RaiseCanExecuteChangedForPassword(); // Trigger CanExecute re-evaluation
            }
        }
    }
}
