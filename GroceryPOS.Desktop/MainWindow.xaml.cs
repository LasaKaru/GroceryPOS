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
using System.Windows.Shapes;
using GroceryPOS.Desktop.ViewModels;

namespace GroceryPOS.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The MainWindowViewModel will be injected via the constructor
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            // Set the DataContext of the window to its ViewModel
            this.DataContext = viewModel;
        }
    }
}
