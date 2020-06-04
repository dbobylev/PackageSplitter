using OracleParser.Model.PackageModel;
using PackageSplitter.Model;
using PackageSplitter.Model.SplitterGrid;
using PackageSplitter.View.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace PackageSplitter.View.Templates
{
    class SplitterCellButtonFactory
    {
        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> AddOldSpec = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.OldSpec, eElementStateType.Add);
        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> AddOldBody = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.OldBody, eElementStateType.Add);
        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> AddNewSpec = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.NewSpec, eElementStateType.Add);
        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> AddNewBody = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.NewBody, eElementStateType.Add);

        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> DelOldSpec = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.OldSpec, eElementStateType.Delete);
        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> DelOldBody = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.OldBody, eElementStateType.Delete);
        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> DelNewSpec = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.NewSpec, eElementStateType.Delete);
        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> DelNewBody = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.NewBody, eElementStateType.Delete);

        private readonly KeyValuePair<eSplitterObjectType, eElementStateType> LinkOldBody = new KeyValuePair<eSplitterObjectType, eElementStateType>(eSplitterObjectType.OldBody, eElementStateType.CreateLink);


        public SplitterCellButtonFactory()
        {

        }

        public SplitterCellButton[] GetButtons(eSplitterObjectType splitterObject, ePackageElementType elementType)
        {

            switch (elementType)
            {
                case ePackageElementType.Method: return GetButtonsForMethod(splitterObject);
                case ePackageElementType.Type:
                case ePackageElementType.Variable: return GetButtonsForVariables(splitterObject);
                default: return new SplitterCellButton[] { };
            }
        }

        private SplitterCellButton[] GetButtonsForMethod(eSplitterObjectType splitterObject)
        {
            switch (splitterObject)
            {
                case eSplitterObjectType.OldSpec:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddOldSpec, AddOldBody })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelOldSpec }))
                    };
                case eSplitterObjectType.OldBody:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddOldBody })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelOldSpec, DelOldBody })),
                        new SplitterCellButton(eSplitterCellButtonType.LinkButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] {LinkOldBody, AddNewSpec, AddNewBody}))
                    };
                case eSplitterObjectType.NewSpec:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddNewSpec, AddNewBody })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelNewSpec }))
                    };
                case eSplitterObjectType.NewBody:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddNewBody })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelNewSpec, DelNewBody }))
                    };
                default:
                    return null;
            }
        }

        private SplitterCellButton[] GetButtonsForVariables(eSplitterObjectType splitterObject)
        {
            switch (splitterObject)
            {
                case eSplitterObjectType.OldSpec:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddOldSpec, DelOldBody })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelOldSpec })),
                        new SplitterCellButton(eSplitterCellButtonType.CopyButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddNewSpec }))
                    };
                case eSplitterObjectType.OldBody:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddOldBody, DelOldSpec })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelOldBody })),
                        new SplitterCellButton(eSplitterCellButtonType.CopyButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddNewBody }))
                    };
                case eSplitterObjectType.NewSpec:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddNewSpec, DelNewBody })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelNewSpec }))
                    };
                case eSplitterObjectType.NewBody:
                    return new SplitterCellButton[]
                    {
                        new SplitterCellButton(eSplitterCellButtonType.AddButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { AddNewBody, DelNewSpec })),
                        new SplitterCellButton(eSplitterCellButtonType.RemoveButton, new Dictionary<eSplitterObjectType, eElementStateType>(new[] { DelNewBody }))
                    };
                default:
                    return null;
            }
        }
    }
}
