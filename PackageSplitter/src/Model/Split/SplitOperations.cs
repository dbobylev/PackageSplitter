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
using System.Threading;
using System.Windows.Xps.Serialization;

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

        public virtual bool AnalizeLinks()
        {
            var answer = false;
            var AllNames = _package.elements.Select(x => x.Name.ToUpper());
            var NewNames = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ALL_ELEMENT_TYPES)
                   .Concat(GetName(eSplitterObjectType.NewSpec, eElementStateType.Add, ALL_ELEMENT_TYPES))
                   .Distinct()
                   .Select(x => x.ToUpper());
            var AllNewBodies = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method).Select(x => x.ToUpper());
            var AllLinks = _package.elements.Where(x => AllNewBodies.Contains(x.Name.ToUpper())).SelectMany(x => x.Links.ToArray()).Select(x => x.Text).Distinct();
            var LinkedOldNames = AllNames.Except(NewNames).Intersect(AllLinks);
            var NewRequiriedElements = _splitter.Elements.Where(x => LinkedOldNames.Contains(x.PackageElementName.ToUpper()) && !x.IsRequiried);
           
            if (NewRequiriedElements.Any())
            {
                answer = true;
                NewRequiriedElements.ToList().ForEach(x => x.IsRequiried = true);
            }
            _splitter.Elements.Where(x => x.IsRequiried && !LinkedOldNames.Contains(x.PackageElementName.ToUpper())).ToList().ForEach(x => { x.MakePrefix = false; x.IsRequiried = false; });
            return answer;
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
            // Делаем заготовку для временного файла
            var tmpObj = new TempRepositoryObject(_package);

            // Проверяем был ли создан временный файл с обновленнными ссылками
            var NeedUpdatePrefix = MakePrefix(tmpObj);
            if (NeedUpdatePrefix)
                // Заменяем путь до пакета на временный (с обновленными ссылками)
                _package.repositoryPackage = tmpObj.TempRepObject;

            var AllVariables = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, NOT_METHOD_TYPES);
            var AllMethods = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method);

            var NewText = string.Empty;
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.Spec);
            NewText += GetNewText(AllVariables, ePackageElementDefinitionType.BodyFull);
            NewText += GetNewText(AllMethods, ePackageElementDefinitionType.BodyFull);

            if (NeedUpdatePrefix)
            {
                _package.repositoryPackage = tmpObj.OriginalRepObject;
                tmpObj.DeleteTempFile();
            }

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
            var FileLines = File.ReadAllLines(_package.repositoryPackage.SpecRepFullPath);

            // Методы который должны бють добавлены
            var MethodNameToAdd = GetName(eSplitterObjectType.OldSpec, eElementStateType.Add, ePackageElementType.Method);
            if (MethodNameToAdd.Any())
            {
                // Ищем последнюю строку последнего метода
                PosMethod = _package.elements.Where(x => x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineEnd).OrderBy(x => x).Last();
                // Вставляем метку, для последующей вставки новых методов
                FileLines = FileLines.Insert(PosMethod + 1 /*На следующей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelMethod });
                // Текст новых методов
                TextMethod = GetNewText(MethodNameToAdd, ePackageElementDefinitionType.BodyDeclaration);
            }

            // Переменные которые должны быть добавлены
            var VariableToAdd = GetName(eSplitterObjectType.OldSpec, eElementStateType.Add, NOT_METHOD_TYPES);
            if (VariableToAdd.Any())
            {
                //вставляем перед первым найденным методом
                PosVariable = _package.elements.Where(x => x.HasSpec && x.ElementType == ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineBeg).OrderBy(x => x).First();
                // Вставляем метку, для последующей вставки новых переменных
                FileLines = FileLines.Insert(PosVariable - 1 /*На предыдущей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });

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
                    if (LinesToDelete[i] >= PosMethod - 2)
                        LinesToDelete[i] += 2;

                    if (LinesToDelete[i] >= PosVariable - 2)
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
            Func<int, int> MoveLine = (x) => (x >= PosVariable - 2 ? x + 2 : x);
            var TextVariable = string.Empty;
            var oldBodyText = string.Empty;

            // Строки исходного файла
            var FileLines = File.ReadAllLines(_package.repositoryPackage.BodyRepFullPath);

            #region Заменяем ссылки

            var DeletedMethods = GetName(eSplitterObjectType.OldBody, eElementStateType.Delete, ePackageElementType.Method).Select(x=>x.ToUpper());
            var ExistedMethods = GetName(eSplitterObjectType.OldBody, eElementStateType.Exist, ePackageElementType.Method);
            var LinksToDeletedMethod = _package.elements
                .Where(x => x.ElementType == ePackageElementType.Method && ExistedMethods.Contains(x.Name))
                .SelectMany(x => x.Links.Where(x => DeletedMethods.Contains(x.Text.ToUpper())))
                .OrderBy(x=>x.LineBeg)
                .ThenBy(x=>x.ColumnBeg)
                .ToArray();
            if (LinksToDeletedMethod.Any())
            {
                var NewSpecMethods = GetName(eSplitterObjectType.NewSpec, eElementStateType.Add, ePackageElementType.Method).Select(x => x.ToUpper());
                var WrongLinks = LinksToDeletedMethod.Select(x => x.Text.ToUpper()).Distinct().Except(NewSpecMethods);
                if (WrongLinks.Any())
                    throw new Exception($"В исходном пакете остались ссылки на методы, которые были удалены и не объявлены в новой спецификации: {string.Join(", ", WrongLinks)}");

                // Ссылка которую будем добавлять 
                var LinkStr = $"{Config.Instanse().NewPackageName}.".ToLower();

                // Добавляем название схемы в префикс если название схемы у нового объекта отличается
                if (Config.Instanse().NewPackageOwner.ToUpper() != _package.repositoryPackage.Owner.ToUpper())
                    LinkStr = $"{Config.Instanse().NewPackageOwner.ToLower()}.{LinkStr}";

                var LinksToDeletedMethodIndex = 0;
                for (int i = 0; i < FileLines.Length; i++)
                {
                    if (i + 1 == LinksToDeletedMethod[LinksToDeletedMethodIndex].LineBeg)
                    {
                        var OneLineReplaceCounter = 0;
                        while (LinksToDeletedMethodIndex < LinksToDeletedMethod.Length && i + 1 == LinksToDeletedMethod[LinksToDeletedMethodIndex].LineBeg)
                            FileLines[i] = FileLines[i].Insert(LinksToDeletedMethod[LinksToDeletedMethodIndex++].ColumnBeg + LinkStr.Length * OneLineReplaceCounter++, LinkStr);
                        if (LinksToDeletedMethodIndex >= LinksToDeletedMethod.Length)
                            break;
                    }
                }
            }

            #endregion

            // Переменные которые должны быть добавлены
            var VariableToAdd = GetName(eSplitterObjectType.OldBody, eElementStateType.Add, NOT_METHOD_TYPES);
            if (VariableToAdd.Any())
            {
                // Ищем последнюю строку с объявлением перменной
                if (_package.elements.Any(x => x.ElementType != ePackageElementType.Method && x.HasBody))
                {
                    PosVariable = _package.elements.Where(x => x.HasBody && x.ElementType != ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.BodyFull].LineEnd).OrderBy(x => x).Last();
                    // Вставляем метку, для последующей вставки новых переменных
                    FileLines = FileLines.Insert(PosVariable + 1 /*На следующей строке*/ -1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });
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
                    Code = GetLink(x, _package.repositoryPackage)
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
                sb.Append(DBRep.Instance().GetTextOfFile(_package.repositoryPackage.RepFullPath(repositoryObjectType), samples[i].LineBeg, samples[i].LineEnd, samples[i].ColumnEnd));
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
            Func<int, string> GetSpaces = (n) => string.Join(string.Empty, Enumerable.Range(0, n).Select(x => " "));

            var position = element.Position[ePackageElementDefinitionType.BodyDeclaration];
            var text = DBRep.Instance().GetTextOfFile(repositoryPackage.BodyRepFullPath, position.LineBeg, position.LineEnd, position.ColumnEnd);

            var SpaceBeforeTextBegin = 0;
            var ch = text[SpaceBeforeTextBegin];
            while (ch == ' ')
                ch = text[++SpaceBeforeTextBegin];
            var beginIndent = GetSpaces(SpaceBeforeTextBegin);

            var NewPackageCallText = $"{Config.Instanse().NewPackageName.ToLower()}.{element.Name}";
            if (repositoryPackage.Owner.ToUpper() != Config.Instanse().NewPackageOwner.ToUpper())
                NewPackageCallText = $"{Config.Instanse().NewPackageOwner.ToLower()}.{NewPackageCallText}";
            if (text[SpaceBeforeTextBegin] == 'f' || text[SpaceBeforeTextBegin] == 'F')
                NewPackageCallText = $"return {NewPackageCallText}";
            var IndentName = GetSpaces(NewPackageCallText.Count());

            string parametersText = string.Empty;
            if (element.Parametres.Any())
            {
                parametersText += "(";
                for (int i = 0; i < element.Parametres.Count; i++)
                {
                    var paramName = element.Parametres[i].Name;
                    parametersText += $"{(i == 0 ? string.Empty : $"{beginIndent}  {IndentName} ")}{paramName} => {paramName},\r\n";
                }
                parametersText = parametersText.TrimEnd(new char[] { '\r', '\n', ',' });
                parametersText += ");\r\n";
            }
            else
                parametersText += ";\r\n";

            text = $"{text} is\r\n" +
                   $"{beginIndent}begin\r\n" +
                   $"{beginIndent}  {NewPackageCallText}{parametersText}" +
                   $"{beginIndent}end {element.Name};";
            return text.Split("\r\n");
        }

        /// <summary>
        /// Добавляем префиксы исходного пакета к помеченным ссылкам в новом теле пакета.
        /// Создаём временный файл тела пакета, копию исходного тела, где обновим все необходимые ссылки
        /// Этот временный файл будем использовать при генерации нового тела пакета, таким образом в новом пакете будут ссылки на исходжный пакет.
        /// Такой подход необходим так как у нас имеются точные позициии ссылок в _исхордном пакете_ (т.е. при создании нового, мы не знаем в каком месте окажется ссылка, которую нужно обновить)
        /// </summary>
        /// <param name="tmpRepObject"></param>
        /// <returns>Имеются обновленные данные: Да/Нет</returns>
        private bool MakePrefix(TempRepositoryObject tmpRepObject)
        {
            // Переменная ответа, пока обновлять ничего не нужно
            var answer = false;

            // Название всех ссылок(объектов) которые мы должны обновить
            var AllPrefixNames = _splitter.Elements.Where(x => x.MakePrefix).Select(x => x.PackageElementName.ToUpper());

            // Назхвание всех новых методов в новом теле пакета, где будем обновлять ссылки
            var AllNewBodyNames = GetName(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method);

            // Все позхиции ссылок, которые должны быть обновлены 
            var PosToUpgrade = _package.elements
                .Where(x => AllNewBodyNames.Contains(x.Name) && x.Links.Any(u => AllPrefixNames.Contains(u.Text)))
                .SelectMany(x => x.Links.ToArray())
                .Where(x => AllPrefixNames.Contains(x.Text))
                .OrderBy(x => x.LineBeg)
                .ThenBy(x => x.ColumnBeg)
                .ToArray();

            if (PosToUpgrade.Any())
            {
                // Имеются ссылки для обновления, создаём временные файлы ниже
                answer = true;

                // Счётчик строчек
                var LineCounter = 0;

                // Индекс для отсортированной коллекции PosToUpgrade
                var PosIndex = 0;

                // Ссылка которую будем добавлять 
                var LinkStr = $"{tmpRepObject.OriginalRepObject.Name}.".ToLower();

                // Добавляем название схемы в префикс если название схемы у нового объекта отличается
                if (Config.Instanse().NewPackageOwner.ToUpper() != tmpRepObject.OriginalRepObject.Owner.ToUpper())
                    LinkStr = $"{tmpRepObject.OriginalRepObject.Owner}.{LinkStr}";

                // Создаём временный файл тела исходного пакета с обновленными ссылками
                using (StreamReader sr = new StreamReader(tmpRepObject.OriginalRepObject.BodyRepFullPath))
                {
                    using (StreamWriter sw = new StreamWriter(tmpRepObject.TempRepObject.BodyRepFullPath))
                    {
                        while (sr.Peek() >= 0)
                        {
                            var str = sr.ReadLine();
                            LineCounter++;
                            // В обной строчке может быть несколько замен, считаем их
                            var OneLineReplaceCounter = 0;
                            while (PosIndex < PosToUpgrade.Length && LineCounter == PosToUpgrade[PosIndex].LineBeg)
                            {
                                // Вставляем ссылку
                                str = str.Insert(PosToUpgrade[PosIndex++].ColumnBeg + LinkStr.Length * OneLineReplaceCounter++, LinkStr);
                            }
                            sw.WriteLine(str);
                        }
                    }
                }

                // Создаём временный файл Спеки исходного пакета, т.к. они должны быть в паре в рамках объекта RepositoryObject
                using (StreamReader sr = new StreamReader(tmpRepObject.OriginalRepObject.SpecRepFullPath))
                    using (StreamWriter sw = new StreamWriter(tmpRepObject.TempRepObject.SpecRepFullPath))
                        while (sr.Peek() >= 0)
                            sw.WriteLine(sr.ReadLine());
            }

            return answer;
        }

        #endregion
    }
}
