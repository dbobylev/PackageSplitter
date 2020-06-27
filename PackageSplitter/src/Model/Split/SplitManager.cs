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
    public class SplitManager: SplitOperations, ISplitManager, IOracleParsedPackageSetter
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

        public event Action<Package> OraclePackageSetted;

        public void SetSplitterPackage(Splitter splitterPackage)
        {
            _splitter = splitterPackage;
        }

        public void SetOracleParsedPackage(Package package)
        {
            _package = package;
            OraclePackageSetted?.Invoke(_package);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Сгенерировать текст нового объекта
        /// </summary>
        /// <param name="splitterObjectType"></param>
        /// <param name="param"></param>
        public void RunSplit(eSplitterObjectType splitterObjectType, eSplitParam param)
        {
            string FinalObjectText = string.Empty;

            // Получаем текст объекта
            switch (splitterObjectType)
            {
                case eSplitterObjectType.OldSpec: FinalObjectText = RunSplitOldSpec(); break;
                case eSplitterObjectType.OldBody: FinalObjectText = RunSplitOldBody(); break;
                case eSplitterObjectType.NewSpec: FinalObjectText = RunSplitNewSpec(param.HasFlag(eSplitParam.GenerateHeader)); break;
                case eSplitterObjectType.NewBody: FinalObjectText = RunSplitNewBody(param.HasFlag(eSplitParam.GenerateHeader)); break;
                default:
                    break;
            }

            // Заменяем двойные пробелы - одинарными
            FinalObjectText = Regex.Replace(FinalObjectText, "\r\n\\s*\r\n\\s*\r\n", "\r\n\r\n");

            // Копируем текст в буфер
            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(FinalObjectText);

            // Открываем текст в новом окне
            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(FinalObjectText);
                tw.Show();
            }

            // Обновляем репозиторий
            if (param.HasFlag(eSplitParam.DirectlyUpdateRep))
            {
                RepositoryObject repositoryObject;
                if (splitterObjectType.IsNew())
                    repositoryObject = new RepositoryObject(Config.Instanse().NewPackageName, Config.Instanse().NewPackageOwner, splitterObjectType.IsSpec() ? eRepositoryObjectType.Package_Spec : eRepositoryObjectType.Package_Body);
                else
                {
                    repositoryObject = splitterObjectType.IsSpec() ? _package.repositoryPackage.GetObjectSpec() : _package.repositoryPackage.GetObjectBody();

                    /* Мы должны одновременно обновить в репозитории и спеку и тело
                     * Последовательно мы это сделать не можем, так как генерация текста зависит от обоих частей
                     * Обновляем соседнюю часть:
                     */
                    var SecondParttext = splitterObjectType.IsSpec() ? RunSplitOldBody() : RunSplitOldSpec();
                    var SecondPartObj = splitterObjectType.IsSpec() ? _package.repositoryPackage.GetObjectBody() : _package.repositoryPackage.GetObjectSpec();
                    DBRep.Instance().SaveTextToFile(SecondParttext, SecondPartObj);
                }

                DBRep.Instance().SaveTextToFile(FinalObjectText, repositoryObject);
            }
        }

        /// <summary>
        /// Найти ссылки из нового тела пакета, на методы и переменные в исходном теле пакета (т.е. на те, которые по разным причинам не были скопированы)
        /// </summary>
        /// <returns></returns>
        public override bool AnalizeLinks()
        {
            return base.AnalizeLinks();
        }
    }
}
 