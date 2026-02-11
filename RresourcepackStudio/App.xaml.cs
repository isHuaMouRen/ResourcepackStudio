using RresourcepackStudio.Windows;
using System.Configuration;
using System.Data;
using System.Windows;

namespace RresourcepackStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(e);

            var window = new WindowMain();
            window.Show();

            Current.MainWindow = window;
        }
    }

}
