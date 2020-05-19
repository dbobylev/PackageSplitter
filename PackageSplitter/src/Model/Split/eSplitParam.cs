using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model.Split
{
    [Flags]
    enum eSplitParam
    {
        None              =  0,
        DirectlyUpdateRep =  1 << 0,
        CopyToClipBoard   =  1 << 1,
        OpenNewWindow     =  1 << 2,
        GenerateHeader    =  1 << 3
    }
}
