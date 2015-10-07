namespace AssetSite.Helpers
{
    public class Culture
    {
        /// <summary>
        /// Initialize current thread culture from cookie
        /// </summary>
        public static void InitCulture()
        {           
            var culture = System.Globalization.CultureInfo.GetCultureInfo(CookieWrapper.Language);
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}