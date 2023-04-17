using DevelopTools.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;

namespace DevelopTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperLink)
            {
                Process.Start(new ProcessStartInfo(hyperLink.NavigateUri.AbsoluteUri));
            }
        }
    }
}
