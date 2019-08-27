using MahApps.Metro.Controls;
using System.Windows;

namespace Countdown
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : MetroWindow
    {
        public ControlWindow(DataObject dataObject)
        {
            InitializeComponent();

            DataContext = dataObject;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
