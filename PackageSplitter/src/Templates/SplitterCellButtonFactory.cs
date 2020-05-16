using PackageSplitter.Splitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PackageSplitter.Templates
{
    class SplitterCellButtonFactory
    {
        private Dictionary<eSplitterObjectType, CellAction[]> _ButtonActionsByObject = new Dictionary<eSplitterObjectType, CellAction[]>()
        {
            { eSplitterObjectType.OldSpec, new CellAction[]
                {
                    new CellAction(eSplitterObjectType.OldSpec, eSplitterCellActionType.Delete),
                    new CellAction(eSplitterObjectType.OldSpec, eSplitterCellActionType.Add)
                }
            },
            { eSplitterObjectType.OldBody, new CellAction[]
                {
                    new CellAction(eSplitterObjectType.OldBody | eSplitterObjectType.OldSpec, eSplitterCellActionType.Delete),
                    new CellAction(eSplitterObjectType.OldBody, eSplitterCellActionType.Add),
                    new CellAction(eSplitterObjectType.OldBody | eSplitterObjectType.NewSpec | eSplitterObjectType.NewBody, eSplitterCellActionType.MakeLink)
                }
            },
            { eSplitterObjectType.NewSpec, new CellAction[]
                {
                    new CellAction(eSplitterObjectType.NewSpec, eSplitterCellActionType.Delete),
                    new CellAction(eSplitterObjectType.NewSpec | eSplitterObjectType.NewBody, eSplitterCellActionType.Add)
                }
            },
            { eSplitterObjectType.NewBody, new CellAction[]
                {
                    new CellAction(eSplitterObjectType.NewBody | eSplitterObjectType.NewSpec, eSplitterCellActionType.Delete),
                    new CellAction(eSplitterObjectType.NewBody, eSplitterCellActionType.Add)
                }
            },
        };

        public SplitterCellButtonFactory()
        {

        }

        public SplitterCellButton[] GetButtons(int ElementID, eSplitterObjectType splitterObjectType, SplitterCellActionHandler handler)
        {
            return _ButtonActionsByObject[splitterObjectType]
                .Select(x => new SplitterCellAction(ElementID, x))
                .Select(x => new SplitterCellButton(x, handler))
                .ToArray();
        }
    }
}
