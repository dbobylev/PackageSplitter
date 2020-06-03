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
using Expr = System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;

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

            if (param.HasFlag(eSplitParam.GenerateHeader) && splitterObjectType.IsNew())
                FinalObjectText = AddHeader(FinalObjectText, splitterObjectType.GetRepositoryType());

            if (param.HasFlag(eSplitParam.CopyToClipBoard))
                Clipboard.SetText(FinalObjectText);

            if (param.HasFlag(eSplitParam.OpenNewWindow))
            {
                TextWindow tw = new TextWindow(FinalObjectText);
                tw.Show();
            }
        }


        private string RunSplitNewSpec()
        {
            var AllVariables = GetName(eSplitterObjectType.NewSpec, eElementStateType.Add, ePackageElementType.Variable);
            var AllMethods = GetName(eSplitterObjectType.NewSpec, eElementStateType.Add, ePackageElementType.Method);

            var NewText = string.Empty;
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.Spec);
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.BodyFull);
            NewText += GetNewText(AllMethods, ePackageElementDefinitionType.Spec);
            NewText += GetNewText(AllMethods, ePackageElementDefinitionType.BodyDeclaration);

            return NewText;
        }

        private string RunSplitNewBody()
        {
            var AllVariables = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Variable);
            var AllMethods = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method);

            var NewText = string.Empty;
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.Spec);
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.BodyFull);
            NewText += GetNewText(AllMethods, ePackageElementDefinitionType.BodyFull);

            return NewText;
        }

        private string RunSplitOldSpec()
        {
            // Добавление

            var NamesToAdd = GetName(eSplitterObjectType.OldSpec, eElementStateType.Add);
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

            var NamesToDelete = GetName(eSplitterObjectType.OldSpec, eElementStateType.Delete);
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
                var NumLinesToDeleteIndex = 0;
                var FinalText = new StringBuilder();
                for (int i = 0; i < LinesCount; i++)
                    if (NumLinesToDeleteIndex < NumLinesToDelete.Length && i == NumLinesToDelete[NumLinesToDeleteIndex])
                        NumLinesToDeleteIndex++;
                    else
                        FinalText.AppendLine(AllTextLines[i]);
                FinalTextString = FinalText.ToString();
            }
            else
                FinalTextString = string.Join("\r\n", AllTextLines);

            return FinalTextString;
        }


        private string RunSplitOldBody()
        {
            var NameToDelete = GetName(eSplitterObjectType.OldBody, eElementStateType.Delete | eElementStateType.CreateLink);
            var LinesToDelete = _package.elements
                .Where(x => NameToDelete.Contains(x.Name))
                .Select(x => x.Position[ePackageElementDefinitionType.BodyFull])
                .SelectMany(x => Enumerable.Range(x.LineBeg - 1, x.LineEnd - x.LineBeg + 1))
                .OrderBy(x=>x)
                .ToArray();
            var NameToCreteLink = GetName(eSplitterObjectType.OldBody, eElementStateType.CreateLink);
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

            return FinalTextString;
        }

        private IEnumerable<string> GetName(eSplitterObjectType splitterObjectType, eElementStateType elementStates, ePackageElementType packageElementType = ePackageElementType.Method)
        {
            var x = Expr.Expression.Parameter(typeof(SplitterPackageElement), "x");
            var HasFlagMethod = typeof(Enum).GetMethod("HasFlag", new[] { typeof(Enum) });

            var xPackageElementType = Expr.Expression.Convert(Expr.Expression.PropertyOrField(x, "PackageElementType"), typeof(Enum));
            var xPackageElementTypeHasFlagExpression = Expr.Expression.Call(Expr.Expression.Constant(packageElementType, typeof(Enum)), HasFlagMethod, xPackageElementType);

            var xObjectType = Expr.Expression.Convert(Expr.Expression.PropertyOrField(x, splitterObjectType.ToString()), typeof(Enum));
            var xObjectTypeHasFlagExpression = Expr.Expression.Call(Expr.Expression.Constant(elementStates, typeof(Enum)), HasFlagMethod, xObjectType);
            
            var FinalExpression = Expr.Expression.AndAlso(xPackageElementTypeHasFlagExpression, xObjectTypeHasFlagExpression);
            var Filter = (Func<SplitterPackageElement, bool>)Expr.Expression.Lambda(FinalExpression, x).Compile();

            Seri.Log.Verbose($"GetName Filter: {FinalExpression}");

            return _splitterPackage.Elements.Where(Filter).Select(x => x.PackageElementName);
        }

        private PieceOfCode[] GetCodePositions(IEnumerable<string> names, ePackageElementDefinitionType codeDefinitionPart, bool? hasSpec, bool? hasBody)
        {
            return _package.elements
                .Where(x => names.Contains(x.Name)
                         && x.HasSpec == (hasSpec ?? x.HasSpec)
                         && x.HasBody == (hasBody ?? x.HasBody))
                .Select(x => x.Position[codeDefinitionPart]).ToArray();
        }

        private string GetNewText(IEnumerable<string> names, ePackageElementDefinitionType codeDefinitionPart)
        {
            bool? hasSpec = null, hasBody = null;
            switch (codeDefinitionPart)
            {
                case ePackageElementDefinitionType.Spec: hasSpec = true; break;
                case ePackageElementDefinitionType.BodyFull: hasBody = true; break;
                case ePackageElementDefinitionType.BodyDeclaration: hasSpec = false; break;
                default: break;
            }

            var CodeParts = GetCodePositions(names, codeDefinitionPart, hasSpec, hasBody);
            var repositoryObjectType = codeDefinitionPart == ePackageElementDefinitionType.Spec ? eRepositoryObjectType.Package_Spec : eRepositoryObjectType.Package_Body;

            return GetNewText(CodeParts, repositoryObjectType, codeDefinitionPart == ePackageElementDefinitionType.BodyDeclaration);
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
                parametersText += ";\r\n";

            text = $"{text} is\r\n" +
                   $"  begin\r\n" +
                   $"    {NewPackageNameText}{parametersText}" +
                   $"  end {element.Name};";
            return text.Split("\r\n");
        }
    }
}
 