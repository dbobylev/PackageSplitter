using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PackageSplitter.Model
{
    public enum eElementStateType
    {
        [Description("Есть")]
        Exist,
        [Description("Нет")]
        Empty,
        [Description("Добавить")]
        Add,
        [Description("Удалить")]
        Delete,
        [Description("Создать ссылку")]
        CreateLink
    }
}
