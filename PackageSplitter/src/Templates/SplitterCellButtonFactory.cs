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
                    new CellAction(eSplitterCellActionType.Delete, eSplitterObjectType.OldSpec),
                    new CellAction(eSplitterCellActionType.Add, eSplitterObjectType.OldSpec)
                }
            },
            { eSplitterObjectType.OldBody, new CellAction[]
                {
                    new CellAction(eSplitterCellActionType.Delete, eSplitterObjectType.OldBody | eSplitterObjectType.OldSpec),
                    new CellAction(eSplitterCellActionType.Add, eSplitterObjectType.OldBody),
                    new CellAction(eSplitterCellActionType.MakeLink, eSplitterObjectType.OldBody | eSplitterObjectType.NewSpec | eSplitterObjectType.NewBody)
                }
            },
            { eSplitterObjectType.NewSpec, new CellAction[]
                {
                    new CellAction(eSplitterCellActionType.Delete, eSplitterObjectType.NewSpec),
                    new CellAction(eSplitterCellActionType.Add, eSplitterObjectType.NewSpec | eSplitterObjectType.NewBody)
                }
            },
            { eSplitterObjectType.NewBody, new CellAction[]
                {
                    new CellAction(eSplitterCellActionType.Delete, eSplitterObjectType.NewBody | eSplitterObjectType.NewSpec),
                    new CellAction(eSplitterCellActionType.Add, eSplitterObjectType.NewBody)
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
