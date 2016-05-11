using AppFramework.Core.Classes;

namespace AppFramework.Core.Calculation
{
    public interface IAttributeCalculator
    {
        void CalculateAssetScreens(AssetWrapperForScreenView assetWrapper, long? screenId = null);

        Asset PostCalculateAsset(Asset asset, bool calculateDependencies = true);

        void CalculateDependencies(Asset asset);

        string Error { get; }

        /// <summary>
        /// Calculates attribute value by formula (CalculationFormula).
        /// Works only for attributes within same asset.
        /// </summary>
        /// <param name="asset">Asset instance</param>
        /// <param name="attrs">Calculated data container</param>
        /// <param name="expressionString">Custom formula text</param>
        /// <param name="callingAsset"></param>
        /// <returns>Calculated value</returns>
        object GetValue(Asset asset, ScreenAttrs attrs, string expressionString, long callingAsset = -1);
    }
}