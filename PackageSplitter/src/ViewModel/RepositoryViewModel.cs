using DataBaseRepository;
using DataBaseRepository.Model;
using PackageSplitter.Model.Split;
using PackageSplitter.src.Command;
using PackageSplitter.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PackageSplitter.ViewModel
{
    public class RepositoryViewModel : PropertyChangedBase
    {
        private ISplitManager _SplitManager;
        private RepositoryObject _SelectedFile;

        public RelayCommand LoadOraclePackageCommand { get; private set; }

        public RepositoryObject SelectedFile
        {
            get
            {
                return _SelectedFile;
            }
            set
            {
                _SelectedFile = value;
                if (value != null)
                {
                    Config.Instanse().LastFileUsed = value.RepFilePath;
                    Config.Instanse().Save();
                }
            }
        }

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

        private string _NewPackageName;
        public string NewPackageName
        {
            get
            {
                return _NewPackageName;
            }
            set
            {
                _NewPackageName = value;
                Config.Instanse().NewPackageName = value;
                Config.Instanse().Save();
            }
        }

        private string _NewPackageOwner;
        public string NewPackageOwner
        {
            get
            {
                return _NewPackageOwner;
            }
            set
            {
                _NewPackageOwner = value;
                Config.Instanse().NewPackageOwner = value;
                Config.Instanse().Save();
            }
        }

        private bool _AllowNationalChars;
        public bool AllowNationalChars
        {
            get
            {
                return _AllowNationalChars;
            }
            set
            {
                _AllowNationalChars = value;
                Config.Instanse().AllowNationalChars = value;
                Config.Instanse().Save();
            }
        }


        public RepositoryViewModel(ISplitManager splitManager)
        {
            _RepositoryPath = Config.Instanse().RepositoryPath;
            _NewPackageName = Config.Instanse().NewPackageName;
            _NewPackageOwner = Config.Instanse().NewPackageOwner;
            _AllowNationalChars = Config.Instanse().AllowNationalChars;

            _SplitManager = splitManager;
            LoadOraclePackageCommand = new RelayCommand(LoadOraclePackage, (x) => _SelectedFile != null);
        }

        private void LoadOraclePackage(object obj)
        {
            if (obj is RepositoryObject repositoryObject)
            {
                var repositoryPackage = new RepositoryPackage(repositoryObject);
                ParserWindow pw = new ParserWindow(repositoryPackage);
                pw.Show();
            }
        }
    }
}
