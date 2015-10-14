namespace AppFramework.Core.Classes.Barcode
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    public class DefaultBarcodeProvider : IBarcodeProvider
    {
        private static readonly Regex BarcodeRegex = new Regex("^[0-9a-zA-Z]{7," + ApplicationSettings.BarcodeLength + "}$", RegexOptions.Compiled);
        
        public string GenerateBarcode()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            StringBuilder sb = new StringBuilder();
            int length = ApplicationSettings.BarcodeLength;
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(10));
            }
            return sb.ToString();
        }

        public bool BarcodeExist(string barcode)
        {
            throw new NotImplementedException();
        }

        public bool ValidateBarcode(string barcode)
        {
            return BarcodeRegex.IsMatch(barcode);
        }

        public System.Drawing.Image GenerateBarcodeImage(string barcode)
        {
            return Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum.Draw(barcode, 50);
        }

        /// <summary>
        /// Gets the default barcode value
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int length = ApplicationSettings.BarcodeLength;
                for (int i = 0; i < length; i++)
                {
                    sb.Append("0");
                }

                return sb.ToString();
            }
        }
    }
}
