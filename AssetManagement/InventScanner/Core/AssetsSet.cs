/*--------------------------------------------------------
* AssetsSet.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/5/2009 6:30:23 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Data;
    using System.Xml.Serialization;

    [Serializable]
    public sealed class AssetsSet : DataSet
    {
        public AssetsSet()
        {
            this.Tables.Add(new AssetsTable());
            this.Tables.Add(new LogTable());
        }

        [XmlIgnore]
        public AssetsTable AssetsTable
        {
            get { return this.Tables[0] as AssetsTable; }
        }

        [XmlIgnore]
        public LogTable LogTable
        {
            get { return this.Tables[1] as LogTable; }
        }
    }
}
