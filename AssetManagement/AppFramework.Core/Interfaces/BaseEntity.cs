using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Interfaces
{
    public class BaseEntity: IBaseEntity
    {
        public bool IsNew
        {
            get;
            set;
        }

        /// <summary>
        /// this marks entity as newly created
        /// </summary>
        public BaseEntity()
        {
            this.IsNew = true;
        }

        public virtual void Save()
        {
        }
    }
}
