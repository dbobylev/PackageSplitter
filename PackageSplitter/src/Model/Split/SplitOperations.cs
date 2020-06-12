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
    public abstract class SplitOperations
    {
        private const ePackageElementType NOT_METHOD_TYPES = ePackageElementType.Type | ePackageElementType.Variable | ePackageElementType.Cursor;
        private const ePackageElementType ALL_ELEMENT_TYPES = ePackageElementType.Method | NOT_METHOD_TYPES;

        protected Package _package;
        protected Splitter _splitter;

        protected SplitOperations()
        {

        }

        public virtual void AnalizeLinks()
        {
            var AllNames = _package.elements.Select(x => x.Name.ToUpper());
            var NewNames = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ALL_ELEMENT_TYPES)
                   .Concat(GetName(eSplitterObjectType.NewSpec, eElementStateType.Add, ALL_ELEMENT_TYPES))
                   .Distinct()
                   .Select(x => x.ToUpper());
            var AllNewBodies = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method).Select(x=>x.ToUpper());
            var AllLinks = _package.elements.Where(x => AllNewBodies.Contains(x.Name.ToUpper())).SelectMany(x => x.Links.ToArray()).Select(x => x.Text).Distinct();
            var LinkedOldNames = AllNames.Except(NewNames).Intersect(AllLinks);

            _splitter.Elements.Where(x => LinkedOldNames.Contains(x.PackageElementName.ToUpper()) && !x.IsRequiried).ToList().ForEach(x => x.IsRequiried = true);
            _splitter.Elements.Where(x => x.IsRequiried && !LinkedOldNames.Contains(x.PackageElementName.ToUpper())).ToList().ForEach(x => { x.MakePrefix = false; x.IsRequiried = false; });
        }

        #region Split
        protected string RunSplitNewSpec()
        {
            var AllVariables = GetName(eSplitterObjectType.NewSpec, eElementStateType.Add, NOT_METHOD_TYPES);
            var AllMethods = GetName(eSplitterObjectType.NewSpec, eElementStateType.Add, ePackageElementType.Method);

            var NewText = string.Empty;
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.Spec);
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.BodyFull);
            NewText += GetNewText(AllMethods, ePackageElementDefinitionType.Spec);
            NewText += GetNewText(AllMethods, ePackageElementDefinitionType.BodyDeclaration);

            return NewText;
        }

        protected string RunSplitNewBody()
        {
            var AllVariables = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, NOT_METHOD_TYPES);
            var AllMethods = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method);

            var NewText = string.Empty;
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.Spec);
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.BodyFull);
            NewText += GetNewText(AllMethods, ePackageElementDefinitionType.BodyFull);

            return NewText;
        }

        protected string RunSplitOldSpec()
        {
            var labelMethod = Guid.NewGuid().ToString();
            var labelVariable = Guid.NewGuid().ToString();
            var PosMethod = int.MaxValue;
            var PosVariable = int.MaxValue;
            var TextMethod = string.Empty;
            var TextVariable = string.Empty;
            var oldSpecText = string.Empty;

            // Строки исходного файла
            var FileLines = File.ReadAllLines(_splitter.RepositoryPackage.SpecRepFullPath);

            // Методы который должны бють добавлены
            var MethodNameToAdd = GetName(eSplitterObjectType.OldSpec, eElementStateType.Add, ePackageElementType.Method);
            if (MethodNameToAdd.Any())
            {
                // Ищем последнюю строку последнего метода
                PosMethod = _package.elements.Where(x => MethodNameToAdd.Contains(x.Name) && x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineEnd).OrderBy(x => x).Last();
                // Вставляем метку, для последующей вставки новых методов
                FileLines = FileLines.Insert(PosMethod + 1 /*На следующей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelMethod });
                // Текст новых методов
                TextMethod = GetNewText(MethodNameToAdd, ePackageElementDefinitionType.BodyDeclaration);
            }

            // Переменные которые должны быть ддобавлены
            var VariableToAdd = GetName(eSplitterObjectType.OldSpec, eElementStateType.Add, NOT_METHOD_TYPES);
            if (VariableToAdd.Any())
            {
                // Ищем последнюю строку с объявлением перменной
                if (_package.elements.Any(x => x.ElementType != ePackageElementType.Method && x.HasSpec))
                {
                    PosVariable = _package.elements.Where(x => x.HasSpec && x.ElementType != ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineEnd).OrderBy(x => x).Last();
                    // Вставляем метку, для последующей вставки новых переменных
                    FileLines = FileLines.Insert(PosVariable + 1 /*На следующей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });
                }
                // Если переменных в пакете еще нет, вставляем перед первым найденным методом
                else
                {
                    PosVariable = _package.elements.Where(x => x.HasSpec && x.ElementType == ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineBeg).OrderBy(x => x).First();
                    // Вставляем метку, для последующей вставки новых переменных
                    FileLines = FileLines.Insert(PosVariable - 1 /*На предыдущей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });
                }
                // Текст новых переменных
                TextVariable = GetNewText(VariableToAdd, ePackageElementDefinitionType.BodyFull);
            }

            // Все кто должны быть удалены
            var AllDelete = GetName(eSplitterObjectType.OldSpec, eElementStateType.Delete, ALL_ELEMENT_TYPES);
            // Номера строк для удаления
            var LinesToDelete = _package.elements
                .Where(x => AllDelete.Contains(x.Name) && x.HasSpec)
                .Select(x => x.Position[ePackageElementDefinitionType.Spec])
                .SelectMany(x => Enumerable.Range(x.LineBeg - 1, x.LineEnd - x.LineBeg + 1))
                .OrderBy(x => x)
                .ToArray();

            if (LinesToDelete.Any())
            {
                // Так как мы вставили метки(двух строчные) для новых переменных и методов, строки для удаления должны быть смещены (При необходимости)
                for (int i = 0; i < LinesToDelete.Length; i++)
                {
                    if (LinesToDelete[i] >= PosMethod)
                        LinesToDelete[i] += 2;

                    if (LinesToDelete[i] >= PosVariable)
                        LinesToDelete[i] += 2;
                }

                // Удаление строк
                var LinesCount = FileLines.Count();
                var LinesToDeleteIndex = 0;
                var sb = new StringBuilder();
                for (int i = 0; i < LinesCount; i++)
                    if (LinesToDeleteIndex < LinesToDelete.Length && i == LinesToDelete[LinesToDeleteIndex])
                        LinesToDeleteIndex++;
                    else
                        sb.AppendLine(FileLines[i]);
                oldSpecText = sb.ToString();
            }
            else
                oldSpecText = string.Join("\r\n", FileLines);

            // Заменяем метки;
            oldSpecText = oldSpecText.Replace(labelVariable, TextVariable);
            oldSpecText = oldSpecText.Replace(labelMethod, TextMethod);

            return oldSpecText;
        }

        protected string RunSplitOldBody()
        {
            var labelVariable = Guid.NewGuid().ToString();
            var PosVariable = int.MaxValue;
            Func<int, int> MoveLine = (x) => (x >= PosVariable ? x + 2 : x);
            var TextVariable = string.Empty;
            var oldBodyText = string.Empty;

            // Строки исходного файла
            var FileLines = File.ReadAllLines(_splitter.RepositoryPackage.BodyRepFullPath);

            // Переменные которые должны быть ддобавлены
            var VariableToAdd = GetName(eSplitterObjectType.OldBody, eElementStateType.Add, NOT_METHOD_TYPES);
            if (VariableToAdd.Any())
            {
                // Ищем последнюю строку с объявлением перменной
                if (_package.elements.Any(x => x.ElementType != ePackageElementType.Method && x.HasBody))
                {
                    PosVariable = _package.elements.Where(x => x.HasBody && x.ElementType != ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.BodyFull].LineEnd).OrderBy(x => x).Last();
                    // Вставляем метку, для последующей вставки новых переменных
                    FileLines = FileLines.Insert(PosVariable + 1 /*На следующей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });
                }
                // Если переменных в пакете еще нет, вставляем перед первым найденным методом
                else
                {
                    PosVariable = _package.elements.Where(x => x.HasBody && x.ElementType == ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.BodyFull].LineBeg).OrderBy(x => x).First();
                    // Вставляем метку, для последующей вставки новых переменных
                    FileLines = FileLines.Insert(PosVariable - 1 /*На предыдущей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });
                }
                // Текст новых переменных
                TextVariable = GetNewText(VariableToAdd, ePackageElementDefinitionType.Spec);
            }

            // Все кто должны быть удалены
            var AllDelete = GetName(eSplitterObjectType.OldBody, eElementStateType.Delete | eElementStateType.CreateLink, ALL_ELEMENT_TYPES);
            // Номера строк для удаления
            var LinesToDelete = _package.elements
                .Where(x => AllDelete.Contains(x.Name) && x.HasBody)
                .Select(x => x.Position[ePackageElementDefinitionType.BodyFull])
                .SelectMany(x => Enumerable.Range(x.LineBeg - 1, x.LineEnd - x.LineBeg + 1))
                .OrderBy(x => x)
                .ToArray();

            // Все кто должен обратиться в ссылки на новый пакет
            var NameToCreteLink = GetName(eSplitterObjectType.OldBody, eElementStateType.CreateLink);
            var PartOfLinks = _package.elements
                .Where(x => NameToCreteLink.Contains(x.Name))
                .Select(x => new
                {
                    Line = MoveLine(x.Position[ePackageElementDefinitionType.BodyFull].LineBeg),
                    Code = GetLink(x, _splitter.RepositoryPackage)
                })
                .ToArray();

            if (LinesToDelete.Any())
            {
                // Так как мы вставили метки(двух строчные) для новых переменных и методов, строки для удаления должны быть смещены (При необходимости)
                for (int i = 0; i < LinesToDelete.Length; i++)
                    LinesToDelete[i] = MoveLine(LinesToDelete[i]);

                // Удаление строк
                var LinesCount = FileLines.Count();
                var LinesToDeleteIndex = 0;
                var sb = new StringBuilder();
                for (int i = 0; i < LinesCount; i++)
                    if (LinesToDeleteIndex < LinesToDelete.Length && i == LinesToDelete[LinesToDeleteIndex])
                    {
                        LinesToDeleteIndex++;
                        // Вставляем блок ссылки на новый пакет
                        if (PartOfLinks.Any(x => x.Line == i))
                            foreach (var item in PartOfLinks.First(x => x.Line == i).Code)
                                sb.AppendLine(item);
                    }
                    else
                        sb.AppendLine(FileLines[i]);
                oldBodyText = sb.ToString();
            }
            else
                oldBodyText = string.Join("\r\n", FileLines);

            // Заменяем метки;
            oldBodyText = oldBodyText.Replace(labelVariable, TextVariable);

            return oldBodyText;
        }

        private IEnumerable<string> GetName(eSplitterObjectType splitterObjectType, eElementStateType elementStates, ePackageElementType packageElementType = ePackageElementType.Method)
        {
            var x = Expr.Expression.Parameter(typeof(SplitterElement), "x");
            var HasFlagMethod = typeof(Enum).GetMethod("HasFlag", new[] { typeof(Enum) });

            var xPackageElementType = Expr.Expression.Convert(Expr.Expression.PropertyOrField(x, "PackageElementType"), typeof(Enum));
            var xPackageElementTypeHasFlagExpression = Expr.Expression.Call(Expr.Expression.Constant(packageElementType, typeof(Enum)), HasFlagMethod, xPackageElementType);

            var xObjectType = Expr.Expression.Convert(Expr.Expression.PropertyOrField(x, splitterObjectType.ToString()), typeof(Enum));
            var xObjectTypeHasFlagExpression = Expr.Expression.Call(Expr.Expression.Constant(elementStates, typeof(Enum)), HasFlagMethod, xObjectType);

            var FinalExpression = Expr.Expression.AndAlso(xPackageElementTypeHasFlagExpression, xObjectTypeHasFlagExpression);
            var Filter = (Func<SplitterElement, bool>)Expr.Expression.Lambda(FinalExpression, x).Compile();

            Seri.Log.Verbose($"GetName Filter: {FinalExpression}");

            return _splitter.Elements.Where(Filter).Select(x => x.PackageElementName);
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
                sb.Append(DBRep.Instance().GetTextOfFile(_splitter.RepositoryPackage.RepFullPath(repositoryObjectType), samples[i].LineBeg, samples[i].LineEnd, samples[i].ColumnEnd));
                if (IsBodyDeclarationCopy)
                    sb.Append(";");
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }

        protected string AddHeader(string text, eRepositoryObjectType repositoryObjectType)
        {
            var bodyWord = repositoryObjectType == eRepositoryObjectType.Package_Body ? "body " : string.Empty;
            var NewPackageName = $"{Config.Instanse().NewPackageOwner}.{Config.Instanse().NewPackageName}";
            return $"create or replace package {bodyWord}{NewPackageName} is\r\n\r\n{text}\r\nend {Config.Instanse().NewPackageName};\r\n/";
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
                    parametersText += $"{(i == 0 ? string.Empty : $"     {IndentName}")}{paramName} => {paramName},\r\n";
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

        #endregion
    }
}
