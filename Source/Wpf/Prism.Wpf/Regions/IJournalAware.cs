using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public interface IJournalAware
    {
        bool PersistInHistory();
    }
}
