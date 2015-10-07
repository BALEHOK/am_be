using AppFramework.Core.Exceptions;
using AppFramework.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace AssetManager.Infrastructure.Helpers
{
    public static class ValidationExtension
    {
        public static void AddModelErrors(this ModelStateDictionary state,
            ValidationResult validationResult)
        {
            foreach (var error in validationResult.ResultLines)
                state.AddModelError(error.Key, error.Message);
        }
    }
}