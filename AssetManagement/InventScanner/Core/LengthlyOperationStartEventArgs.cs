﻿/*--------------------------------------------------------
* LengthlyOperationStartedEventArgs.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/25/2009 3:30:38 PM
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

    public class LengthlyOperationStartEventArgs : EventArgs
    {
        public int StepsCount
        {
            get;
            set;
        }
    }
}
