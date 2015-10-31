using AppFramework.Core.Exceptions;
using System.Web.Http.ModelBinding;

namespace AssetManager.Infrastructure.Helpers
{
    public static class ValidationExtension
    {
        public static void AddModelErrors(this ModelStateDictionary state,
            AssetValidationException validationEx)
        {
            foreach (var error in validationEx.ValidationResult.ResultLines)
                state.AddModelError(error.Key, error.Message);
        }

        public static void AddModelErrors(this ModelStateDictionary state,
            InvalidFormulaException formulaEx)
        {
            state.AddModelError("", formulaEx.Message);
        }
    }
}