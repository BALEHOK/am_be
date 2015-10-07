/*--------------------------------------------------------
* LengthlyOperationStepEventArgs.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/25/2009 3:41:26 PM
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

    public class LengthlyOperationStepEventArgs : EventArgs
    {
        public int Position
        {
            get;
            set;
        }
    }
}
