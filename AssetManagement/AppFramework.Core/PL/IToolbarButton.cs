using System;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.PL
{
    public interface IToolbarButton
    {
        Enumerators.ToolbarButtonType ButtonType { get; }
        Action OnButtonClick { get; set; }
    }
}
