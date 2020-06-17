using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PackageSplitter.Model.Parser
{
    [Flags]
    public enum eParseStatus
    {
        [Description("None")]
        None                                = 0 << 0,

        [Description("В ожидании")]
        Wait                                = 1 << 0,

        [Description("Обрабатывается")]
        InProgress                          = 1 << 1,

        [Description("Готов")]
        Done                                = 1 << 2,

        [Description("Ошибка")]
        Fail                                = 1 << 3
    }
}
