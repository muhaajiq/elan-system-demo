using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public interface IItemWithStatus
    {
        bool IsAdded { get; set; }
        bool IsRemoved { get; set; }
        DateTime? DateAdded { get; set; }
        DateTime? DateRemoved { get; set; }
        string? Remark { get; set; }
    }

}
