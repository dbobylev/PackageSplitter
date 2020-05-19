using PackageSplitter.Model.SplitterGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PackageSplitter.View.Templates
{
    class SplitterCellButtonFactory
    {
        private Dictionary<eSplitterObjectType, SplitterAction[]> _ButtonActionsByObject = new Dictionary<eSplitterObjectType, SplitterAction[]>()
        {
            { eSplitterObjectType.OldSpec, new SplitterAction[]
                {
                    new SplitterAction(eCellSplitterActionType.Delete, eSplitterObjectType.OldSpec),
                    new SplitterAction(eCellSplitterActionType.Add, eSplitterObjectType.OldSpec)
                }
            },
            { eSplitterObjectType.OldBody, new SplitterAction[]
                {
                    new SplitterAction(eCellSplitterActionType.Delete, eSplitterObjectType.OldBody | eSplitterObjectType.OldSpec),
                    new SplitterAction(eCellSplitterActionType.Add, eSplitterObjectType.OldBody),
                    new SplitterAction(eCellSplitterActionType.MakeLink, eSplitterObjectType.OldBody | eSplitterObjectType.NewSpec | eSplitterObjectType.NewBody)
                }
            },
            { eSplitterObjectType.NewSpec, new SplitterAction[]
                {
                    new SplitterAction(eCellSplitterActionType.Delete, eSplitterObjectType.NewSpec),
                    new SplitterAction(eCellSplitterActionType.Add, eSplitterObjectType.NewSpec | eSplitterObjectType.NewBody)
                }
            },
            { eSplitterObjectType.NewBody, new SplitterAction[]
                {
                    new SplitterAction(eCellSplitterActionType.Delete, eSplitterObjectType.NewBody | eSplitterObjectType.NewSpec),
                    new SplitterAction(eCellSplitterActionType.Add, eSplitterObjectType.NewBody)
                }
            },
        };

        public SplitterCellButtonFactory()
        {

        }

        public SplitterCellButton[] GetButtons(int ElementID, eSplitterObjectType splitterObjectType, CellSplitterActionHandler handler)
        {
            return _ButtonActionsByObject[splitterObjectType]
                .Select(x => new CellSplitterAction(ElementID, x))
                .Select(x => new SplitterCellButton(x, handler))
                .ToArray();
        }
    }
}
