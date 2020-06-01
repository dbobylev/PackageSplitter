using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PackageSplitter.Model
{
    [Flags]
    public enum eElementStateType
    {
        [Description("None")]
        None                            = 0,
        [Description("Оставить")]
        Exist                           = 1 << 0,
        [Description("Отсутствует")]
        Empty                           = 1 << 1,
        [Description("Добавить")]
        Add                             = 1 << 2,
        [Description("Удалить")]
        Delete                          = 1 << 3,
        [Description("Создать ссылку")]
        CreateLink                      = 1 << 4
    }
}
