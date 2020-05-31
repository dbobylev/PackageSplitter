using DataBaseRepository;
using DataBaseRepository.Model;
using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model.SplitterGrid;
using PackageSplitter.src.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PackageSplitter.Model.Split
{
    public class SplitManager
    {
        private Package _package;
        private SplitterPackage _splitterPackage;

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

        public void SetParsedPackage(Package package)
        {
            _package = package;
        }

        public void SetSplitterPackage(SplitterPackage splitterPackage)
        {
            _splitterPackage = splitterPackage;
        }

        public void RunSplit(eSplitterObjectType objectType, eSplitParam param)
        {
            var Allelements = _splitterPackage.Elements.Where(x => x.NewSpec == eElementStateType.Add).Select(x => x.PackageElementName);
            var SpecElements = _package.elements.Where(x => Allelements.Contains(x.Name) && x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.Spec]).ToArray();
            var BodyElements = _package.elements.Where(x => Allelements.Contains(x.Name) && !x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.BodyDeclaration]).ToArray();

            string NewText;
            var repositoryObjectType = objectType.GetRepositoryType();
            var IsNew = objectType == eSplitterObjectType.NewBody || objectType == eSplitterObjectType.NewSpec;

            if (IsNew)
            {
                NewText = GetNewText(SpecElements, eRepositoryObjectType.Package_Spec);
                NewText += GetNewText(BodyElements, eRepositoryObjectType.Package_Body, true);
                if (param.HasFlag(eSplitParam.GenerateHeader))
                    NewText = AddHeader(NewText, repositoryObjectType);
            }
            else
            {
                NewText = "123";
            }

            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(NewText);

            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(NewText);
                tw.Show();
            }
        }

        private string GetNewText(PieceOfCode[] samples, eRepositoryObjectType repositoryObjectType, bool IsBodyDeclarationCopy = false)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < samples.Length; i++)
            {
                sb.Append(DBRep.Instance().GetTextOfFile(_splitterPackage.RepositoryPackage.RepFullPath(repositoryObjectType), samples[i].LineBeg, samples[i].LineEnd, samples[i].ColumnEnd));
                if (IsBodyDeclarationCopy)
                    sb.Append(";");
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private string AddHeader(string text, eRepositoryObjectType repositoryObjectType)
        {
            var bodyWord = repositoryObjectType == eRepositoryObjectType.Package_Body ? "body " : string.Empty;
            var NewPackageName = $"{Config.Instanse().NewPackageOwner}.{Config.Instanse().NewPackageName}";
            return $"create or replace package {bodyWord}{NewPackageName} is\r\n\r\n{text}\r\nend {NewPackageName};";
        }
    }
}
