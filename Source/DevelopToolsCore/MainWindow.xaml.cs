using DevelopToolsCore.ViewModels;
using HandyControl.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
namespace DevelopToolsCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : GlowWindow
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