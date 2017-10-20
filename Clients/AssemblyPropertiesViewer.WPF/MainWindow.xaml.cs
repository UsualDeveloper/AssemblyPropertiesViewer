using AssemblyPropertiesViewer.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace AssemblyPropertiesViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<ShowPropertiesMessage>(this, OpenResultsWindow);
        }

        private void OpenResultsWindow(ShowPropertiesMessage message)
        {
            var propertiesWindow = new PropertiesWindow();
            propertiesWindow.Owner = Window.GetWindow(this);

            propertiesWindow.DataContext = message.Content;

            propertiesWindow.Show();
        }
    }
}
