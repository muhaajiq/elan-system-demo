using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Framework.Exceptions
{
    public class ClientContextNullReferenceException : Exception
    {
        public ClientContextNullReferenceException()
        {
        }

        public ClientContextNullReferenceException(string message)
            : base(message)
        {
        }
    }
}
