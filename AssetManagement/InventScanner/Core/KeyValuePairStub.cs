/*--------------------------------------------------------
* KeyValuePairStub.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/27/2009 11:53:05 AM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    [DataObject]
    public class KeyValuePairStub
    {
        [DataObjectField(false)]
        public object Key { get; set; }

        [DataObjectField(true)]
        public object Value { get; set; }
    }
}
