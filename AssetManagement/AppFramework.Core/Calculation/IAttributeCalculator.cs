using System.Collections.Generic;
using AppFramework.Core.Classes;

namespace AppFramework.Core.Calculation
{
    public interface IAttributeCalculator
    {
        object GetValue(Asset asset, string expressionString, long callingAsset = -1);

        Asset PreCalculateAsset(Asset asset, long? screenId = null, bool overwrite = true);

        Asset PostCalculateAsset(Asset asset, bool calculateDependencies = true);

        void CalculateDependencies(Asset asset);

        string Error { get; }
    }
}