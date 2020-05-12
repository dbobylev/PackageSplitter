using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PackageSplitter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MAIN_SETTINGS_FILENAME = "PackageSplitterSettings.Json";

        protected override void OnStartup(StartupEventArgs e)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile(MAIN_SETTINGS_FILENAME).Build();
            Seri.InitConfig(configuration);
            Seri.Log.Information("Application begin");
            Config.InitConfig(configuration, MAIN_SETTINGS_FILENAME);
            base.OnStartup(e);
        }
    }
}
