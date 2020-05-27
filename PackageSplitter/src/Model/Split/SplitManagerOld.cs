using DataBaseRepository;
using DataBaseRepository.Model;
using OracleParser.Model;
using PackageSplitter.Model.SplitterGrid;
using PackageSplitter.Model;
using PackageSplitter.Model.Split;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using PackageSplitter.src.View;

namespace PackageSplitter.Model.Split
{
    class SplitManagerOld
    {
        private RepositoryPackage _RepositoryPackage;

        public SplitManagerOld(RepositoryPackage repositoryPackage)
        {
            _RepositoryPackage = repositoryPackage;
        }

        public void Generate(PieceOfCode[] samples, eSplitterObjectType splitterObjectType, eSplitParam param)
        {
            string NewText;
            var repositoryObjectType = splitterObjectType.GetRepositoryType();
            var IsNew = splitterObjectType == eSplitterObjectType.NewBody || splitterObjectType == eSplitterObjectType.NewSpec;
            
            if (IsNew)
            {
                NewText = GetNewText(samples, repositoryObjectType);
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

        private string GetNewText(PieceOfCode[] samples, eRepositoryObjectType repositoryObjectType)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < samples.Length; i++)
            {
                sb.Append(DBRep.Instance().GetTextOfFile(_RepositoryPackage.RepFullPath(repositoryObjectType), samples[i].LineBeg, samples[i].LineEnd));
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
