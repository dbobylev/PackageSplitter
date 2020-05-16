﻿using PackageSplitter.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Splitter
{
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