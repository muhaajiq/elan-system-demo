using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Framework.Exceptions
{
    public class ListNullReferenceException : Exception
    {
        public ListNullReferenceException()
        {
        }

        public ListNullReferenceException(string message)
            : base(message)
        {
        }
    }
}
