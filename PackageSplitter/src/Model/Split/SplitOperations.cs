using DataBaseRepository;
using DataBaseRepository.Model;
using OracleParser.Model;
using OracleParser.Model.PackageModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Expr = System.Linq.Expressions;
using System.Text;

namespace PackageSplitter.Model.Split
{
    public abstract class SplitOperations
    {
        private const ePackageElementType NOT_METHOD_TYPES = ePackageElementType.Type | ePackageElementType.Variable | ePackageElementType.Cursor;
        private const ePackageElementType ALL_ELEMENT_TYPES = ePackageElementType.Method | NOT_METHOD_TYPES;

        /// <summary>
        /// Распарсенный пакет
        /// </summary>
        protected Package _package;

        /// <summary>
        /// Настройки разбиения пакета
        /// </summary>
        protected Splitter _splitter;

        protected SplitOperations()
        {

        }

        #region SplitBase

        /// <summary>
        /// Генерация спецификации нового пакета
        /// </summary>
        /// <param name="addHeader">Добавить заголовок (create or replace ... end; )</param>
        /// <returns></returns>
        protected string RunSplitNewSpec(bool addHeader)
        {
            // Все переменные которые должны быть скопированы
            var AllVariables = GetNames(eSplitterObjectType.NewSpec, eElementStateType.Add, NOT_METHOD_TYPES);
            // Все методы которые должны быть скопирваны
            var AllMethods = GetNames(eSplitterObjectType.NewSpec, eElementStateType.Add, ePackageElementType.Method);

            // Текст нового тела пакета
            var NewText = string.Empty;

            // Добавляем переменные из исходной спецификации
            NewText += GetTextPart(AllVariables, ePackageElementDefinitionType.Spec);
            // Добавляем переменные из исходного тела пакета
            NewText += GetTextPart(AllVariables, ePackageElementDefinitionType.BodyFull);
            // Добавляем методы из исходной спецификации
            NewText += GetTextPart(AllMethods, ePackageElementDefinitionType.Spec);
            // Добавляем методы из тела пакета, если их не было в спецификации
            NewText += GetTextPart(AllMethods, ePackageElementDefinitionType.BodyDeclaration);

            if (addHeader)
                NewText = AddHeader(NewText, eRepositoryObjectType.Package_Spec);

            return NewText;
        }

        /// <summary>
        /// Генерация тела нового пакета
        /// </summary>
        /// <param name="addHeader">Добавить заголовок (create or replace ... end; )</param>
        /// <returns>Текст нового тела пакета</returns>
        protected string RunSplitNewBody(bool addHeader)
        {
            TempRepositoryObject tmpObj = null;

            // Првоеряем должны ли быть обновленны ссылки в новом теле пакета
            if (CheckNewBodyLinks(out ParsedLink[] PosToUpgrade))
            {
                // Обновляем ссылки а исходном пакете, записываем результат во временный файл
                tmpObj = MakePrefix(PosToUpgrade);
                // Подменняем ссылку на файл, что бы генерация нового тела пакета шла из обновленного исходного тела пакета
                _package.repositoryPackage = tmpObj.TempRepObject;
            }

            // Все переменные которые должны быть скопированы
            var AllVariables = GetNames(eSplitterObjectType.NewBody, eElementStateType.Add, NOT_METHOD_TYPES);
            // Все методы которые должны быть скопирваны
            var AllMethods = GetNames(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method);

            // Текст тела нового пакета
            var NewText = string.Empty;
            // Дорбавляем переменные из исходной спецификации
            NewText += GetTextPart(AllVariables, ePackageElementDefinitionType.Spec);
            // Добавляем переменные из исходного тела
            NewText += GetTextPart(AllVariables, ePackageElementDefinitionType.BodyFull);
            // Добавляем методы из исходно тела пакета
            NewText += GetTextPart(AllMethods, ePackageElementDefinitionType.BodyFull);

            // Если была подмена ссылки на файл
            if (tmpObj != null)
            {
                // Возвращает ссылку назад
                _package.repositoryPackage = tmpObj.OriginalRepObject;
                // Удаляем временный файл
                tmpObj.DeleteTempFile();
            }

            if (addHeader)
                NewText = AddHeader(NewText, eRepositoryObjectType.Package_Body);

            return NewText;
        }

        /// <summary>
        /// Генерации спецификации исходного пакета
        /// </summary>
        /// <returns></returns>
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
            var MethodNameToAdd = GetNames(eSplitterObjectType.OldSpec, eElementStateType.Add, ePackageElementType.Method);
            if (MethodNameToAdd.Any())
            {
                // Ищем последнюю строку последнего метода
                PosMethod = _package.elements.Where(x => x.HasSpec).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineEnd).OrderBy(x => x).Last();
                // Вставляем метку, для последующей вставки новых методов
                FileLines = FileLines.Insert(PosMethod + 1 /*На следующей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelMethod });
                // Текст новых методов
                TextMethod = GetTextPart(MethodNameToAdd, ePackageElementDefinitionType.BodyDeclaration);
            }

            // Переменные которые должны быть добавлены
            var VariableToAdd = GetNames(eSplitterObjectType.OldSpec, eElementStateType.Add, NOT_METHOD_TYPES);
            if (VariableToAdd.Any())
            {
                //вставляем перед первым найденным методом
                PosVariable = _package.elements.Where(x => x.HasSpec && x.ElementType == ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.Spec].LineBeg).OrderBy(x => x).First();
                // Вставляем метку, для последующей вставки новых переменных
                FileLines = FileLines.Insert(PosVariable - 1 /*На предыдущей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });

                // Текст новых переменных
                TextVariable = GetTextPart(VariableToAdd, ePackageElementDefinitionType.BodyFull);
            }

            // Все кто должны быть удалены
            var AllDelete = GetNames(eSplitterObjectType.OldSpec, eElementStateType.Delete, ALL_ELEMENT_TYPES);
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

        /// <summary>
        /// Генерация тела исходного пакета
        /// </summary>
        /// <returns></returns>
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

            /* Если мы удаляем метод из исходного пакета, на который продолжают ссылатся в этом же пакете.
             * То к таким ссылкам добавляем префикс нового пакета. (Обязательно проверяем, что бы эти пакеты были в новой спецификации
             */

            // Методы которые удалены из исходного тела пакета
            var DeletedMethods = GetNames(eSplitterObjectType.OldBody, eElementStateType.Delete, ePackageElementType.Method).Select(x=>x.ToUpper());
            // Методы которые остались в исходном пакете
            var ExistedMethods = GetNames(eSplitterObjectType.OldBody, eElementStateType.Exist, ePackageElementType.Method);
            // Позиции ссылок из методов которые остались к удаленным методам
            var LinksToDeletedMethod = _package.elements
                .Where(x => x.ElementType == ePackageElementType.Method && ExistedMethods.Contains(x.Name))
                .SelectMany(x => x.Links.Where(x => DeletedMethods.Contains(x.Text.ToUpper())))
                .OrderBy(x=>x.LineBeg)
                .ThenBy(x=>x.ColumnBeg)
                .ToArray();

            if (LinksToDeletedMethod.Any())
            {
                // Методы объявленные в новой спецификации
                var NewSpecMethods = GetNames(eSplitterObjectType.NewSpec, eElementStateType.Add, ePackageElementType.Method).Select(x => x.ToUpper());
                // Вычитаем из имен ссылок на удаленные методы те которые существуют в новой смпецификации, если что-то останется, то выводим ошибку
                var WrongLinks = LinksToDeletedMethod.Select(x => x.Text.ToUpper()).Distinct().Except(NewSpecMethods);
                if (WrongLinks.Any())
                    throw new Exception($"В исходном пакете остались ссылки на методы, которые были удалены(в исходном пакете) и не объявлены в новой спецификации: {string.Join(", ", WrongLinks)}");

                // Ссылка которую будем добавлять 
                var LinkStr = $"{Config.Instanse().NewPackageName}.".ToLower();

                // Добавляем название схемы в префикс если название схемы у нового объекта отличается
                if (Config.Instanse().NewPackageOwner.ToUpper() != _package.repositoryPackage.Owner.ToUpper())
                    LinkStr = $"{Config.Instanse().NewPackageOwner.ToLower()}.{LinkStr}";

                // Индекс для коллекции LinksToDeletedMethod
                var LinksToDeletedMethodIndex = 0;
                // Пробигаемся по всем строчкам в исходном пакете
                for (int i = 0; i < FileLines.Length; i++)
                {
                    // Доходим до номера строчки, где мы должны вставить префикс
                    if (i + 1 == LinksToDeletedMethod[LinksToDeletedMethodIndex].LineBeg)
                    {
                        // Замен в одной строке может быть несколько, считаем эти замены
                        var OneLineReplaceCounter = 0;

                        // Добавляем префикс
                        while (LinksToDeletedMethodIndex < LinksToDeletedMethod.Length && i + 1 == LinksToDeletedMethod[LinksToDeletedMethodIndex].LineBeg)
                            FileLines[i] = FileLines[i].Insert(LinksToDeletedMethod[LinksToDeletedMethodIndex++].ColumnBeg + LinkStr.Length * OneLineReplaceCounter++, LinkStr);

                        // Выходлим из цикла если ссылок больше не осталось
                        if (LinksToDeletedMethodIndex >= LinksToDeletedMethod.Length)
                            break;
                    }
                }
            }

            #endregion

            // Переменные которые должны быть добавлены
            var VariableToAdd = GetNames(eSplitterObjectType.OldBody, eElementStateType.Add, NOT_METHOD_TYPES);
            if (VariableToAdd.Any())
            {
                // Вставляем перед первым найденным методом
                PosVariable = _package.elements.Where(x => x.HasBody && x.ElementType == ePackageElementType.Method).Select(x => x.Position[ePackageElementDefinitionType.BodyFull].LineBeg).OrderBy(x => x).First();
                // Вставляем метку, для последующей вставки новых переменных
                FileLines = FileLines.Insert(PosVariable - 1 /*На предыдущей строке*/ - 1 /* Нумерация позиций начинается с 1*/, new string[] { string.Empty, labelVariable });
                // Текст новых переменных
                TextVariable = GetTextPart(VariableToAdd, ePackageElementDefinitionType.Spec);
            }

            /* Все кто должны быть удалены
             * Мы будем удалять методы помеченные как "Удалить" и "Создать ссылку"
             * В момент удаления блока метода "Создать ссылку", вместо него будем подставлять текст из метода GetLink
             */
            var AllDelete = GetNames(eSplitterObjectType.OldBody, eElementStateType.Delete | eElementStateType.CreateLink, ALL_ELEMENT_TYPES);
            // Номера строк для удаления
            var LinesToDelete = _package.elements
                .Where(x => AllDelete.Contains(x.Name) && x.HasBody)
                .Select(x => x.Position[ePackageElementDefinitionType.BodyFull])
                .SelectMany(x => Enumerable.Range(x.LineBeg - 1, x.LineEnd - x.LineBeg + 1))
                .OrderBy(x => x)
                .ToArray();

            // Все кто должен обратиться в ссылки на новый пакет
            var NameToCreteLink = GetNames(eSplitterObjectType.OldBody, eElementStateType.CreateLink);
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
                // Так как мы вставили метки(двух строчные) для новых переменных, строки для удаления должны быть смещены (При необходимости)
                for (int i = 0; i < LinesToDelete.Length; i++)
                    LinesToDelete[i] = MoveLine(LinesToDelete[i]);

                /* Удаление строк:
                 * Пробегаемся по всем строчкам исходного файла FileLines
                 * И копируем в новую переменную, те которые должны остаться
                 */
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

            // Заменяем метки
            oldBodyText = oldBodyText.Replace(labelVariable, TextVariable);

            return oldBodyText;
        }

        #endregion

        /// <summary>
        /// Найти ссылки из нового тела пакета, на методы и переменные в исходном теле пакета (т.е. на те, которые по разным причинам не были скопированы)
        /// </summary>
        /// <returns></returns>
        public virtual bool AnalizeLinks()
        {
            // Имена всех элементов в пакете
            var AllNames = _package.elements.Select(x => x.Name.ToUpper());

            // Имена всех скопирвоанных элементов в новой спеке и новом теле
            var NewNames = GetNames(eSplitterObjectType.NewBody, eElementStateType.Add, ALL_ELEMENT_TYPES)
                   .Concat(GetNames(eSplitterObjectType.NewSpec, eElementStateType.Add, ALL_ELEMENT_TYPES))
                   .Distinct()
                   .Select(x => x.ToUpper());

            // Имена всех методов в новом теле пакета
            var AllNewBodies = GetNames(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method).Select(x => x.ToUpper());

            // Список имен элементов на которые ссылаются в этих методах (которые добавлены в новом теле пакета)
            var AllLinks = _package.elements.Where(x => AllNewBodies.Contains(x.Name.ToUpper())).SelectMany(x => x.Links.ToArray()).Select(x => x.Text).Distinct();

            // Из всех элементов вычитаем те которые были скопированы в новое тело/спеку и берем пересечение с ссылками которые есть в новом теле.
            var LinkedOldNames = AllNames.Except(NewNames).Intersect(AllLinks);

            // Проверяем эти методы на флаг IsRequiried
            var NewRequiriedElements = _splitter.Elements.Where(x => LinkedOldNames.Contains(x.PackageElementName.ToUpper()) && !x.IsRequiried);

            // При отсутствии флага, проставляем его (Ячейки подсветятся желтым)
            if (NewRequiriedElements.Any())
                NewRequiriedElements.ToList().ForEach(x => x.IsRequiried = true);

            // Удаляем IsRequiried у тех методов которые больше не имеют ссылок из нового тела
            _splitter.Elements.Where(x => x.IsRequiried && !LinkedOldNames.Contains(x.PackageElementName.ToUpper())).ToList().ForEach(x => { x.MakePrefix = false; x.IsRequiried = false; });

            return NewRequiriedElements.Any();
        }

        #region private helpers

        /// <summary>
        /// Получить имена элементов в зависимости от объекта, типа эемента и состояния элемента
        /// </summary>
        /// <param name="splitterObjectType">Тип объекта</param>
        /// <param name="elementStates">Состояние элемента (можно несколько)</param>
        /// <param name="packageElementType">Тип элемента  (можно несколько)</param>
        /// <returns></returns>
        private IEnumerable<string> GetNames(eSplitterObjectType splitterObjectType, eElementStateType elementStates, ePackageElementType packageElementType = ePackageElementType.Method)
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

            /* Пример результата отбора
             * _splitter.Elements.Where(x => packageElementType.HasFlag(x.PackageElementType) && elementStates.HasFlag(x.NewSpec));
             * где вместо x.NewSpec подставится поле в зависимости от splitterObjectType
             */
            return _splitter.Elements.Where(Filter).Select(x => x.PackageElementName);
        }

        /// <summary>
        /// Получить косок текста для определенных элементов
        /// </summary>
        /// <param name="names">Имена элементов</param>
        /// <param name="codeDefinitionPart">Участок откуда забираем код</param>
        /// <returns></returns>
        private string GetTextPart(IEnumerable<string> names, ePackageElementDefinitionType codeDefinitionPart)
        {
            bool? hasSpec = null, hasBody = null;
            switch (codeDefinitionPart)
            {
                case ePackageElementDefinitionType.Spec: hasSpec = true; break;
                case ePackageElementDefinitionType.BodyFull: hasBody = true; break;
                /* BodyDeclaration - Участок метода в теле пакета, который сгодится для спецификации 
                 * (т.е. текст функции/процедуры в теле пакета до IS/AS)
                 * Эти участки подлежат копированию при отсутствии объявлении метода в исходной спецификации
                 */
                case ePackageElementDefinitionType.BodyDeclaration: hasSpec = false; break;
                default: break;
            }

            var CodeParts = _package.elements
                                    .Where(x => names.Contains(x.Name)
                                             && x.HasSpec == (hasSpec ?? x.HasSpec)
                                             && x.HasBody == (hasBody ?? x.HasBody))
                                    .Select(x => x.Position[codeDefinitionPart]).ToArray();

            var repositoryObjectType = codeDefinitionPart == ePackageElementDefinitionType.Spec ? eRepositoryObjectType.Package_Spec : eRepositoryObjectType.Package_Body;

            return GetTextPart(CodeParts, repositoryObjectType, codeDefinitionPart == ePackageElementDefinitionType.BodyDeclaration);
        }

        /// <summary>
        /// Получить текст из заданных участков кода.
        /// </summary>
        /// <param name="samples">Участки кода</param>
        /// <param name="repositoryObjectType">Откуда будем доставать код (спецификация/тело)</param>
        /// <param name="IsBodyDeclarationCopy">При копировании текста из участка BodyDeclaration в новую спецификацию, необходимо добавить точку с запятой</param>
        /// <returns></returns>
        private string GetTextPart(PieceOfCode[] samples, eRepositoryObjectType repositoryObjectType, bool IsBodyDeclarationCopy = false)
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

        /// <summary>
        /// Добавить заголовок к тексту
        /// </summary>
        /// <param name="text"></param>
        /// <param name="repositoryObjectType"></param>
        /// <returns></returns>
        private string AddHeader(string text, eRepositoryObjectType repositoryObjectType)
        {
            var bodyWord = repositoryObjectType == eRepositoryObjectType.Package_Body ? "body " : string.Empty;
            var NewPackageName = $"{Config.Instanse().NewPackageOwner}.{Config.Instanse().NewPackageName}";
            return $"create or replace package {bodyWord}{NewPackageName} is\r\n\r\n{text}\r\nend {Config.Instanse().NewPackageName};\r\n/";
        }

        /// <summary>
        /// Получить текст ссылки на метод в новом пакете
        /// </summary>
        /// <param name="element"></param>
        /// <param name="repositoryPackage"></param>
        /// <returns></returns>
        private string[] GetLink(PackageElement element, RepositoryPackage repositoryPackage)
        {
            Func<int, string> GetSpaces = (n) => string.Join(string.Empty, Enumerable.Range(0, n).Select(x => " "));

            // Получаем текст метода до ключевого слова IS/AS
            var position = element.Position[ePackageElementDefinitionType.BodyDeclaration];
            var text = DBRep.Instance().GetTextOfFile(repositoryPackage.BodyRepFullPath, position.LineBeg, position.LineEnd, position.ColumnEnd);

            // Считаем количество пробелов до начала метода (до слова function/procedure)
            var SpaceBeforeTextBegin = 0;
            var ch = text[SpaceBeforeTextBegin];
            while (ch == ' ')
                ch = text[++SpaceBeforeTextBegin];
            var beginIndent = GetSpaces(SpaceBeforeTextBegin);

            // Текст ссылки на метод в новом пакете
            var NewPackageCallText = $"{Config.Instanse().NewPackageName.ToLower()}.{element.Name}";
            // Добавляем схему если новая схема отличается от текущей
            if (repositoryPackage.Owner.ToUpper() != Config.Instanse().NewPackageOwner.ToUpper())
                NewPackageCallText = $"{Config.Instanse().NewPackageOwner.ToLower()}.{NewPackageCallText}";
            // Добавляем слово return для функции
            if (text[SpaceBeforeTextBegin] == 'f' || text[SpaceBeforeTextBegin] == 'F')
                NewPackageCallText = $"return {NewPackageCallText}";
            // Отступ длинною в текст ссылки (необходим для отступов параметров)
            var IndentName = GetSpaces(NewPackageCallText.Count());

            // Добавляем параметры
            string parametersText = string.Empty;
            if (element.Parametres.Any())
            {
                parametersText += "(";
                for (int i = 0; i < element.Parametres.Count; i++)
                {
                    var paramName = element.Parametres[i].Name;
                    // Первый параметр без отступов, остальные с отступами
                    parametersText += $"{(i == 0 ? string.Empty : $"{beginIndent}  {IndentName} ")}{paramName} => {paramName},\r\n";
                }
                parametersText = parametersText.TrimEnd(new char[] { '\r', '\n', ',' });
                parametersText += ");\r\n";
            }
            else
                parametersText += ";\r\n";

            // Формируем финальный текст метода
            text = $"{text} is\r\n" +
                   $"{beginIndent}begin\r\n" +
                   $"{beginIndent}  {NewPackageCallText}{parametersText}" +
                   $"{beginIndent}end {element.Name};";
            return text.Split("\r\n");
        }


        /// <summary>
        /// Проверяем наличие ссылок в новом теле пакета (которые мы должны добавить)
        /// </summary>
        /// <param name="PosToUpgrade">Позиции ссылок которые необходимо добавить в новом теле пакета</param>
        /// <returns>Имееются ли ссылки для обновления: Да/Нет</returns>
        private bool CheckNewBodyLinks(out ParsedLink[] PosToUpgrade)
        {
            // Название всех ссылок(объектов) которые мы должны обновить
            var AllPrefixNames = _splitter.Elements.Where(x => x.MakePrefix).Select(x => x.PackageElementName.ToUpper());

            // Назхвание всех новых методов в новом теле пакета, где будем обновлять ссылки
            var AllNewBodyNames = GetNames(eSplitterObjectType.NewBody, eElementStateType.Add, ePackageElementType.Method);

            // Все позхиции ссылок, которые должны быть обновлены 
            PosToUpgrade = _package.elements
                .Where(x => AllNewBodyNames.Contains(x.Name) && x.Links.Any(u => AllPrefixNames.Contains(u.Text)))
                .SelectMany(x => x.Links.ToArray())
                .Where(x => AllPrefixNames.Contains(x.Text))
                .OrderBy(x => x.LineBeg)
                .ThenBy(x => x.ColumnBeg)
                .ToArray();

            return PosToUpgrade.Any();
        }

        /// <summary>
        /// Добавляем префиксы к указанным ссылкам.
        /// --------------------------------------------------------------------------------------------------------------------------------------
        /// Новое тело пакета генерируется из исходного тела пакета. Так как мы имеем точные позиции ссылок для обновления в исходном теле пакета, 
        /// мы создадим временный файл исходного тела пакета, где обновим наши ссылки. Затем генерация нового тела пакета будет произведенна 
        /// из этого временного файла с правильными ссылаками.
        /// --------------------------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// <param name="PosToUpgrade">Позиции для обновления</param>
        /// <returns>Временные файл с обновленными ссылками</returns>
        private TempRepositoryObject MakePrefix(ParsedLink[] PosToUpgrade)
        { 
            var tmpRepObject = new TempRepositoryObject(_package);

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

            // Создаём временный файл Спеки исходного пакета (просто копируем), т.к. они должны быть в паре в рамках объекта RepositoryObject
            using (StreamReader sr = new StreamReader(tmpRepObject.OriginalRepObject.SpecRepFullPath))
                using (StreamWriter sw = new StreamWriter(tmpRepObject.TempRepObject.SpecRepFullPath))
                    while (sr.Peek() >= 0)
                        sw.WriteLine(sr.ReadLine());

            return tmpRepObject;
        }

        #endregion
    }
}
