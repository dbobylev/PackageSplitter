using System;

namespace PackageSplitter.Model
{
    /// <summary>
    /// Тип объекта сплиттера: Тело/Спека, Исходная/Новая
    /// </summary>
    [Flags]
    public enum eSplitterObjectType
    {
        None = 0,
        OldSpec = 1 << 0,
        OldBody = 1 << 1,
        NewSpec = 1 << 2,
        NewBody = 1 << 3
    }
}
