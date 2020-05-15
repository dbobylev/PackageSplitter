using OracleParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.Model
{
    public class PackageElement
    {
        public string Name { get; private set; }
        public bool IsMissing { get; private set; } = false;

        public eElementStateType OldSpec { get; private set; }
        public eElementStateType OldBody { get; set; }
        public eElementStateType NewSpec { get; private set; }
        public eElementStateType NewBody { get; private set; }

        private PieceOfCode PosSpec;
        private PieceOfCode PosBody;

        public PackageElement(string name)
        {
            Name = name;
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
