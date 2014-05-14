using System;

using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InvokerWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

            MakePortable(InvokerWPF.Properties.Settings.Default,"poppedcollar.cfg");

        }
        private static void MakePortable(ApplicationSettingsBase settings, string filename)
        {
            var portableSettingsProvider =
                new PortableSettingsProvider(filename);
            settings.Providers.Add(portableSettingsProvider);
            foreach (System.Configuration.SettingsProperty prop in settings.Properties)
                prop.Provider = portableSettingsProvider;
            settings.Reload();
        }
    }
}
