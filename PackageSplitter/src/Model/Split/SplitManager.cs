﻿using DataBaseRepository;
using DataBaseRepository.Model;
using OracleParser.Model;
using OracleParser.Model.PackageModel;
using PackageSplitter.Model.SplitterGrid;
using PackageSplitter.src.View;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void RunSplitNewSpec(eSplitterObjectType objectType, eSplitParam param)
        {
            var Allelements = _splitterPackage.Elements.Where(x => x.NewSpec == eElementStateType.Add).Select(x => x.PackageElementName);
            var SpecElements = _package.elements.Where(x => Allelements.Contains(x.Name) && x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.Spec]).ToArray();
            var BodyElements = _package.elements.Where(x => Allelements.Contains(x.Name) && !x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.BodyDeclaration]).ToArray();

            var repositoryObjectType = objectType.GetRepositoryType();
            var NewText = GetNewText(SpecElements, eRepositoryObjectType.Package_Spec);
            NewText += GetNewText(BodyElements, eRepositoryObjectType.Package_Body, true);
            
            if (param.HasFlag(eSplitParam.GenerateHeader))
                NewText = AddHeader(NewText, repositoryObjectType);

            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(NewText);

            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(NewText);
                tw.Show();
            }
        }

        public void RunSplitNewBody(eSplitterObjectType objectType, eSplitParam param)
        {
            var Allelements = _splitterPackage.Elements.Where(x => x.NewBody == eElementStateType.Add).Select(x => x.PackageElementName);
            var SpecElements = _package.elements.Where(x => Allelements.Contains(x.Name) && !x.HasBody).Select(x => x.Position[ePackageElementDefinitionType.Spec]).ToArray();
            var BodyElements = _package.elements.Where(x => Allelements.Contains(x.Name) && x.HasBody).Select(x => x.Position[ePackageElementDefinitionType.BodyFull]).ToArray();

            var repositoryObjectType = objectType.GetRepositoryType();
            var NewText = GetNewText(SpecElements, eRepositoryObjectType.Package_Spec);
            NewText += GetNewText(BodyElements, eRepositoryObjectType.Package_Body);

            if (param.HasFlag(eSplitParam.GenerateHeader))
                NewText = AddHeader(NewText, repositoryObjectType);

            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(NewText);

            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(NewText);
                tw.Show();
            }
        }

        public void RunSplitOldSpec(eSplitParam param)
        {
            // Добавление

            var NamesToAdd =  _splitterPackage.Elements.Where(x => x.OldSpec == eElementStateType.Add).Select(x => x.PackageElementName);
            var MethodPieces = _package.elements
                .Where(x => NamesToAdd.Contains(x.Name) && x.ElementType == ePackageElementType.Method)
                .Select(x => x.Position[ePackageElementDefinitionType.BodyDeclaration])
                .ToArray();
            //var Variables = ...

            var textToAdd = GetNewText(MethodPieces, eRepositoryObjectType.Package_Body, true).Split("\r\n");
            var Filetext = File.ReadAllLines(_splitterPackage.RepositoryPackage.SpecRepFullPath);
            var LastLine = _package.elements.Where(x => x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineEnd).OrderBy(x => x).Last();
            var BegPartOfText = Filetext.Take(LastLine);
            var EndPartOfText = Filetext.Skip(LastLine);
            var AllTextLines = BegPartOfText.Concat(new[] { string.Empty }).Concat(textToAdd).Concat(EndPartOfText).ToArray();

            // Удаление

            var NamesToDelete = _splitterPackage.Elements.Where(x => x.OldSpec == eElementStateType.Delete).Select(x => x.PackageElementName);
            var NumLinesToDelete = _package.elements
                .Where(x => NamesToDelete.Contains(x.Name))
                .Select(x => x.Position[ePackageElementDefinitionType.Spec])
                .SelectMany(x=>Enumerable.Range(x.LineBeg - 1, x.LineEnd - x.LineBeg  + 1))
                .OrderBy(x => x)
                .ToArray();

            string FinalTextString;
            if (NumLinesToDelete.Any())
            {
                var LinesCount = AllTextLines.Count();
                int NumLinesToDeleteIndex = 0;
                var FinalText = new StringBuilder();
                for (int i = 0; i < LinesCount; i++)
                    if (NumLinesToDeleteIndex < NumLinesToDelete.Length && i == NumLinesToDelete[NumLinesToDeleteIndex])
                        NumLinesToDeleteIndex++;
                    else
                        FinalText.AppendLine(AllTextLines[i]);
                FinalTextString = FinalText.ToString();
            }
            else
            {
                FinalTextString = string.Join("\r\n", AllTextLines);
            }

            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(FinalTextString);

            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(FinalTextString);
                tw.Show();
            }
        }


        public void RunSplitOldBody(eSplitParam param)
        {
            var NameToDelete = _splitterPackage.Elements.Where(x => x.OldBody == eElementStateType.Delete || x.OldBody == eElementStateType.CreateLink).Select(x=>x.PackageElementName);
            var LinesToDelete = _package.elements
                .Where(x => NameToDelete.Contains(x.Name))
                .Select(x => x.Position[ePackageElementDefinitionType.BodyFull])
                .SelectMany(x => Enumerable.Range(x.LineBeg - 1, x.LineEnd - x.LineBeg + 1))
                .OrderBy(x=>x)
                .ToArray();
            var NameToCreteLink = _splitterPackage.Elements.Where(x => x.OldBody == eElementStateType.CreateLink).Select(x => x.PackageElementName);
            var PartOfLinks = _package.elements
                .Where(x => NameToCreteLink.Contains(x.Name))
                .Select(x => new { 
                    Line = x.Position[ePackageElementDefinitionType.BodyFull].LineBeg, 
                    Code = GetLink(x, _splitterPackage.RepositoryPackage)
                });

            var answer = new StringBuilder();
            string FinalTextString;
            if (LinesToDelete.Any())
            {
                var FileText = File.ReadAllLines(_splitterPackage.RepositoryPackage.BodyRepFullPath);
                var LineToDeleteIndex = 0;
                for (int i = 0; i < FileText.Length; i++)
                {
                    if (LineToDeleteIndex < LinesToDelete.Length && i == LinesToDelete[LineToDeleteIndex])
                    {
                        LineToDeleteIndex++;
                        if (PartOfLinks.Any(x => x.Line == i))
                            foreach (var item in PartOfLinks.First(x => x.Line == i).Code)
                                answer.AppendLine(item);
                    }
                    else
                        answer.AppendLine(FileText[i]);
                }
                FinalTextString = answer.ToString();
            }
            else
            {
                FinalTextString = File.ReadAllText(_splitterPackage.RepositoryPackage.BodyRepFullPath);
            }


            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(FinalTextString);

            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(FinalTextString);
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

        private string[] GetLink(PackageElement element, RepositoryPackage repositoryPackage)
        {
            var position = element.Position[ePackageElementDefinitionType.BodyDeclaration];
            var text = DBRep.Instance().GetTextOfFile(repositoryPackage.BodyRepFullPath, position.LineBeg, position.LineEnd, position.ColumnEnd);
            var NewPackageNameText = $"{Config.Instanse().NewPackageOwner }.{ Config.Instanse().NewPackageName}";
            var IndentName = string.Join(string.Empty, Enumerable.Range(0, NewPackageNameText.Count()).Select(x => " "));

            string parametersText = string.Empty;
            if (element.Parametres.Any())
            {
                parametersText += "(";
                for (int i = 0; i < element.Parametres.Count; i++)
                {
                    var paramName = element.Parametres[i].Name;
                    parametersText += $"{(i==0?string.Empty: $"     {IndentName}")}{paramName} => {paramName},\r\n";
                }
                parametersText = parametersText.TrimEnd(new char[] { '\r', '\n', ',' });
                parametersText += ");\r\n";
            }
            else
            {
                parametersText += ";\r\n";
            }

            text = $"{text} is\r\n" +
                   $"  begin\r\n" +
                   $"    {NewPackageNameText}{parametersText}" +
                   $"  end {element.Name};";
            return text.Split("\r\n");
        }
    }
}
 