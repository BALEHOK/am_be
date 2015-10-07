using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Interfaces
{
    public interface IBaseEntity
    {
        bool IsNew { get; set; }
        
        void Save();
    }
}
