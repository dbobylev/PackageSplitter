using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UpdateableConfig;

namespace PackageSplitter
{
    class Config : UpdateableJsonConfig
    {
        public string RepositoryPath
        {
            get => GetValue<string>("RepositoryPath");
            set => SetValue("RepositoryPath", value);
        }

        public string LastOwnerUsed
        {
            get => GetValue<string>("LastOwnerUsed");
            set => SetValue("LastOwnerUsed", value);
        }

        public string LastFileUsed
        {
            get => GetValue<string>("LastFileUsed");
            set => SetValue("LastFileUsed", value);
        }

        public string NewPackageName
        {
            get => GetValue<string>("NewPackageName");
            set => SetValue("NewPackageName", value);
        }

        public string NewPackageOwner
        {
            get => GetValue<string>("NewPackageOwner");
            set => SetValue("NewPackageOwner", value);
        }
        

        private static Config _Instanse;
        public static Config Instanse()
        {
            if (_Instanse == null)
                throw new ArgumentNullException("Configuration not loaded");
            return _Instanse;
        }
        public static Config InitConfig(IConfigurationRoot config, string filepath)
        {
            _Instanse = new Config(config, filepath);
            return _Instanse;
        }
        private Config(IConfigurationRoot config, string filepath) : base(config, filepath) { }
    }
}
