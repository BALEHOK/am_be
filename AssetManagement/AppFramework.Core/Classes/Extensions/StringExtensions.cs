namespace AppFramework.Core.Classes.Extensions
{
    public static class StringExtensions
    {
        public static string Localized(this string str)
        {
            return new TranslatableString(str).GetTranslation();
        }
    }
}
