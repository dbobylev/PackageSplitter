using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace PackageSplitter.View.Templates
{
    class CellButton : Button
    {
        public Dictionary<eSplitterObjectType, eElementStateType> NewStates;

        public CellButton(eSplitterObjectType splitterObject, ePackageElementType elementType)
        {
            NewStates = new Dictionary<eSplitterObjectType, eElementStateType>();

            switch (elementType)
            {
                case ePackageElementType.Method:
                    break;
                case ePackageElementType.Variable:
                    break;
                default:
                    break;
            }
        }

        private void SetMethodStates(eSplitterObjectType splitterObject)
        {
            switch (splitterObject)
            {
                case eSplitterObjectType.None:
                    break;
                case eSplitterObjectType.OldSpec:
                    break;
                case eSplitterObjectType.OldBody:
                    break;
                case eSplitterObjectType.NewSpec:
                    break;
                case eSplitterObjectType.NewBody:
                    break;
                default:
                    break;
            }
        }
    }
}
