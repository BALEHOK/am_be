using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.PL
{
    public interface IConfirmationDialog
    {
        string OnConfirmAction { get; set; }
    }
}
