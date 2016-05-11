using System;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// We can have different formulas for the same attribute on different screens.
    /// This makes it impossible to alter value of attributes within the asset (take a look at asset precalculation -
    /// we to store precalculated value somewhere else.
    /// For now any attribute can appear only once on a screen. While it is true,
    /// the following wrapper postones the need to rewrite the whole fucking asset API
    /// </summary>
    public class AssetWrapperForScreenView
    {
        private readonly Asset _asset;
        private readonly Dictionary<long, ScreenAttrs> _screenAttrs = new Dictionary<long, ScreenAttrs>();

        public Asset Asset
        {
            get { return _asset; }
        }

        public AssetWrapperForScreenView(Asset asset)
        {
            _asset = asset;
        }

        /// <summary>
        /// Get the copy of asset attributes for a screen
        /// </summary>
        public ScreenAttrs ScreenAttributes(long screenId)
        {
            if (!_screenAttrs.ContainsKey(screenId))
            {
                if (Asset.Configuration.Panels.All(p => p.ScreenId != screenId))
                {
                    throw new ArgumentException(string.Format("Asset {0} of type {1} has no screen {2}", Asset.ID,
                        Asset.Configuration.ID, screenId));
                }
                _screenAttrs.Add(screenId, new ScreenAttrs(_asset.Attributes.Select(a => a.Copy())));
            }

            return _screenAttrs[screenId];
        }
    }
}