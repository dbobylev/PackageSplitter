using DataBaseRepository;
using DataBaseRepository.Model;
using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.src.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Expr = System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using OracleParser;

namespace PackageSplitter.Model.Split
{
    public class SplitManager: ISplitManager
    {
        private SplitOperations _splitOperations;

        public event Action<Package, RepositoryPackage> PackageLoaded;

        #region Singleton
        private static SplitManager _instance;
        public static SplitManager Instance()
        {
            if (_instance == null)
                _instance = new SplitManager();
            return _instance;
        }
        private SplitManager()
        {
            _splitOperations = new SplitOperations();
        }
        #endregion

        public void LoadSplitterPackage(Splitter splitterPackage)
        {
            _splitOperations._splitter = splitterPackage;
        }

        public void LoadOracleParsedPackage(RepositoryPackage repositoryPackage)
        {
            try
            {
                _splitOperations._package = OraParser.Instance().GetPackage(repositoryPackage);
                PackageLoaded?.Invoke(_splitOperations._package, repositoryPackage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("При разборе пакета, произошла ошибка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TextWindow textWindow = new TextWindow(ex.ToString());
                textWindow.Show();
            }
        }

        public void RunSplit(eSplitterObjectType splitterObjectType, eSplitParam param)
        {
            _splitOperations.RunSplit(splitterObjectType, param);
        }
    }
}
 