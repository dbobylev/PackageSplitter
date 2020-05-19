using DataBaseRepository;
using DataBaseRepository.Model;
using OracleParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageSplitter.Model.Split
{
    class SplitManager
    {
        private RepositoryPackage _RepositoryPackage;

        public SplitManager(RepositoryPackage repositoryPackage)
        {
            _RepositoryPackage = repositoryPackage;
        }

        public string GetNewSpecText(PieceOfCode[] samples)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < samples.Length; i++)
            {
                sb.Append(DBRep.Instance().GetTextOfFile(_RepositoryPackage.SpecRepFullPath, samples[i].LineBeg, samples[i].LineEnd));
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
