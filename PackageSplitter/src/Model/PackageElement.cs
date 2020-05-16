using OracleParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model
{
    public class PackageElement
    {
        private PieceOfCode _PosSpec;
        private PieceOfCode _PosBody;

        public string PackageElementName { get; private set; }

        public eElementStateType OldSpec { get; set; }
        public eElementStateType OldBody { get; set; }
        public eElementStateType NewSpec { get; set; }
        public eElementStateType NewBody { get; set; }

        public PackageElement(string packageElementNAme)
        {
            PackageElementName = packageElementNAme;
            OldSpec = eElementStateType.Empty;
            OldBody = eElementStateType.Empty;
            NewSpec = eElementStateType.Empty;
            NewBody = eElementStateType.Empty;
        }

        public void SetOldSpec(PieceOfCode posSpec)
        {
            _PosSpec = posSpec;
            OldSpec = eElementStateType.Exist;
        }

        public void SetOldBody(PieceOfCode posBody)
        {
            _PosBody = posBody;
            OldBody = eElementStateType.Exist;
        }
    }
}
