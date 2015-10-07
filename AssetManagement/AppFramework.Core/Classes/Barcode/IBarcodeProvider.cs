/*--------------------------------------------------------
* IBarcodeManager.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 7/30/2009 3:56:51 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace AppFramework.Core.Classes.Barcode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using System.Drawing;

    /// <summary>
    /// Interface for managing barcodes.
    /// </summary>
    public interface IBarcodeProvider
    {
        /// <summary>
        /// Generates the barcode.
        /// </summary>
        /// <returns>Return <see cref="String"/> value</returns>
        string GenerateBarcode();

        /// <summary>
        /// Barcodes the exist.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Return <see cref="Boolean"/> value - if barcode already exist</returns>
        bool BarcodeExist(string barcode);

        /// <summary>
        /// Gets the default barcode value.
        /// </summary>
        /// <value>The default value.</value>
        string DefaultValue
        {
            get;
        }

        /// <summary>
        /// Determines whether is barcode valid.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>
        /// 	<c>true</c> if is barcode valid; otherwise, <c>false</c>.
        /// </returns>
        bool ValidateBarcode(string barcode);

        /// <summary>
        /// Generates the barcode image.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Return <see cref="Image"/> value</returns>
        Image GenerateBarcodeImage(string barcode);        
    }
}