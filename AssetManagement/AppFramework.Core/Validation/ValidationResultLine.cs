namespace AppFramework.Core.Validation
{
    public class ValidationResultLine
    {
        public ValidationResultLine(string key)
        {
            Key = key;
            IsValid = true;
        }

        public bool IsValid { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }

        public static ValidationResultLine Success
        {
            get
            {
                return new ValidationResultLine(string.Empty)
                {
                    IsValid = true
                };
            }
        }

        public static ValidationResultLine Error(string key, string message)
        {
            return new ValidationResultLine(key)
            {
                IsValid = false,
                Message = message
            };
        }
    }
}