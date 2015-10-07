using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.Validation
{
    public class ValidationResult
    {
        private List<ValidationResultLine> _resultLines;
        private bool _isValid;

        public ValidationResult()
        {
            _resultLines = new List<ValidationResultLine>();
            IsValid = true;
        }

        public static ValidationResult operator +(ValidationResult x, ValidationResult y)
        {
            x.ResultLines.AddRange(y.ResultLines);            
            return x;
        }

        public static ValidationResult operator +(ValidationResult x, ValidationResultLine y)
        {
            x.ResultLines.Add(y);
            return x;
        }

        public static ValidationResult operator +(ValidationResult x, ValidationResultLine[] y)
        {
            x.ResultLines.AddRange(y);
            return x;
        }

        public List<ValidationResultLine> ResultLines
        {
            get
            {
                if (Equals(_resultLines, null))
                {
                    _resultLines = new List<ValidationResultLine>();
                }
                return _resultLines;
            }
            set { _resultLines = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether result of evaluting Validation Expression true.
        /// Unlike IsValid property this doesn't calculating automaticly and set when evaluting expression
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is operation valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get
            {
                _isValid = ResultLines.All(line => line.IsValid);
                return _isValid;
            }
            private set
            {
                _isValid = value;
            }
        }

        /// <summary>
        /// Gets the error message, separated by given string
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        public string GetErrorMessage(string separator = "\r\n")
        {
            return _resultLines != null
                ? string.Join(
                    separator,
                    _resultLines
                        .Where(l => !l.IsValid)
                        .Select(l => l.Message))
               : string.Empty;
        }

        public void AddError(string errorMessage)
        {
            ResultLines.Add(new ValidationResultLine(string.Empty)
            {
                IsValid = false,
                Message = errorMessage
            });
        }

        public void AddErrorFormatted(string errorMessage, params object[] args)
        {
            AddError(string.Format(errorMessage, args));
        }

        public static ValidationResult Success
        {
            get
            {
                return new ValidationResult
                {
                    IsValid = true,
                    ResultLines = new List<ValidationResultLine>()
                };
            }
        }
    }
}