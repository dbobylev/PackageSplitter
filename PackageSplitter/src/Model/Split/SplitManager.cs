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
    public class SplitManager: SplitOperations, ISplitManager
    {
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
        }
        #endregion

        public event Action<Package> PackageLoaded;

        public void LoadSplitterPackage(Splitter splitterPackage)
        {
            _splitter = splitterPackage;
        }

        public void LoadOracleParsedPackage(RepositoryPackage repositoryPackage)
        {
            try
            {
                _package = OraParser.Instance().GetPackage(repositoryPackage);
                PackageLoaded?.Invoke(_package);
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
            string FinalObjectText = string.Empty;
            switch (splitterObjectType)
            {
                case eSplitterObjectType.OldSpec: FinalObjectText = RunSplitOldSpec(); break;
                case eSplitterObjectType.OldBody: FinalObjectText = RunSplitOldBody(); break;
                case eSplitterObjectType.NewSpec: FinalObjectText = RunSplitNewSpec(); break;
                case eSplitterObjectType.NewBody: FinalObjectText = RunSplitNewBody(); break;
                default:
                    break;
            }

            FinalObjectText = Regex.Replace(FinalObjectText, "\r\n\\s*\r\n\\s*\r\n", "\r\n\r\n");

            if (param.HasFlag(eSplitParam.GenerateHeader) && splitterObjectType.IsNew())
                FinalObjectText = AddHeader(FinalObjectText, splitterObjectType.GetRepositoryType());

            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(FinalObjectText);

            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(FinalObjectText);
                tw.Show();
            }

            if (param.HasFlag(eSplitParam.DirectlyUpdateRep))
            {
                RepositoryObject repositoryObject;
                if (splitterObjectType.IsNew())
                    repositoryObject = new RepositoryObject(Config.Instanse().NewPackageName, Config.Instanse().NewPackageOwner, splitterObjectType.IsSpec() ? eRepositoryObjectType.Package_Spec : eRepositoryObjectType.Package_Body);
                else
                    repositoryObject = splitterObjectType.IsSpec() ? _package.repositoryPackage.GetObjectSpec() : _package.repositoryPackage.GetObjectBody();

                DBRep.Instance().SaveTextToFile(FinalObjectText, repositoryObject);
            }
        }

        public override void AnalizeLinks()
        {
            base.AnalizeLinks();
        }
    }
}
 