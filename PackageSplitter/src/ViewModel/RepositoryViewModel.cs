using DataBaseRepository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageSplitter.ViewModel
{
    public class RepositoryViewModel : PropertyChangedBase
    {
        private string _SelectedOwner;
        public string SelectedOwner
        {
            get 
            {
                return _SelectedOwner; 
            }
            set
            {
                _SelectedOwner = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Config.Instanse().LastOwnerUsed = value;
                    Config.Instanse().Save();
                }
            }
        }

        private string _RepositoryPath;
        public string RepositoryPath
        {
            get
            {
                return _RepositoryPath;
            }
            set
            {
                _RepositoryPath = value;
                if (Directory.Exists(value))
                {
                    Config.Instanse().RepositoryPath = value;
                    Config.Instanse().Save();
                }
            }
        }

        public RepositoryViewModel()
        {
            _RepositoryPath = Config.Instanse().RepositoryPath;
        }
    }
}
