using OracleParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model
{
    public class SplitterPackageElement
    {
        public PieceOfCode PosSpec { get; private set; }
        public PieceOfCode PosBody { get; private set; }

        public string PackageElementName { get; private set; }

        public eElementStateType OldSpec { get; set; }
        public eElementStateType OldBody { get; set; }
        public eElementStateType NewSpec { get; set; }
        public eElementStateType NewBody { get; set; }

        public SplitterPackageElement(string packageElementNAme)
        {
            PackageElementName = packageElementNAme;
            OldSpec = eElementStateType.Empty;
            OldBody = eElementStateType.Empty;
            NewSpec = eElementStateType.Empty;
            NewBody = eElementStateType.Empty;
        }

        public void SetOldSpec(PieceOfCode posSpec)
        {
            PosSpec = posSpec;
            OldSpec = eElementStateType.Exist;
        }

        public void SetOldBody(PieceOfCode posBody)
        {
            PosBody = posBody;
            OldBody = eElementStateType.Exist;
        }
    }
}
