using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    public class CannotNavigateException : Exception
    {
        public CannotNavigateException(string message)
            : base(message)
        {

        }
    }
}
