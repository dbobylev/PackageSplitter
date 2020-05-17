using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PackageSplitter.Model
{
    public enum eElementStateType
    {
        [Description("Оставить")]
        Exist,
        [Description("Отсутствует")]
        Empty,
        [Description("Добавить")]
        Add,
        [Description("Удалить")]
        Delete,
        [Description("Создать ссылку")]
        CreateLink
    }
}
