using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi.Commands
{
    public interface ISelectionAfter
    {
        Selection SelectionAfter { get; }
        Selection SelectionBefore { get; }
        bool UpdateSelection { get; set; }
    }
}
