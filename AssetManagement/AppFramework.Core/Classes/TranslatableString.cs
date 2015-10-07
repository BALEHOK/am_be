using System;
using System.Globalization;
using System.Xml.Serialization;
using AppFramework.Core.Classes.Caching;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// Provides translation for string objects.
    /// </summary>    
    [Serializable]
    public class TranslatableString
    {
        /// <summary>
        /// Translation key to perform searching
        /// </summary>      
        [XmlElement]
        public string Key
        {
            get { return this._key; }
            set { this._key = value; }
        }

        private string _key;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="keyName"></param>
        public TranslatableString(string keyName)
        {
            _key = keyName;
        }

        /// <summary>
        /// Returns the translation for current culture
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetTranslation()
        {
            return GetTranslation(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Translation of the object.
        /// </summary>
        /// <returns>string translation</returns>
        public string GetTranslation(CultureInfo culture)
        {
            var cache = CacheFactory.GetCache<TranslatableString>("GetTranslationDb");
            string translation = cache.Get(Key, culture);
            return translation == string.Empty ? Key : translation;
        }

        /// <summary>
        /// Generates the cache key.
        /// </summary>       
        /// <returns></returns>
        [CacheKey("GetTranslationDb")]
        public static string GetCacheKey(string key, CultureInfo culture)
        {
            return string.Format("StringResource_{0}_{1}", key, culture.Name);
        }

        /// <summary>
        /// Returns the translation without using cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <returns>Translation or empty string if nothing found</returns>       
        [CacheValue("GetTranslationDb")]
        public static TranslatableString GetTranslationDb(string key, CultureInfo culture)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var resource = unitOfWork.StringResourcesRepository
                .SingleOrDefault(s => s.CultureName.StartsWith(culture.Name) && s.ResourceKey == key);
            string result = string.Empty;
            if (!object.Equals(null, resource))
            {
                result = resource.ResourceValue;
            }
            return result;
        }

        /// <summary>
        /// Add a new translation of this object for given culture
        /// </summary>
        /// <param name="translation">Translation of the object</param>
        /// <param name="culture">For which language this translation is</param>
        /// <returns></returns>
        public void AddTranslation(string translation, CultureInfo culture)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var resource = unitOfWork.StringResourcesRepository
                .SingleOrDefault(s => s.CultureName == culture.Name && s.ResourceKey == Key);
            if (object.Equals(null, resource))
            {
                resource = new Entities.StringResources()
                {
                    ResourceKey = Key,
                    ResourceValue = translation,
                    CultureName = culture.Name
                };
                unitOfWork.StringResourcesRepository.Insert(resource);
            }
            else
            {
                resource.ResourceValue = translation;
                unitOfWork.StringResourcesRepository.Update(resource);
            }
            unitOfWork.Commit();
            Cache<TranslatableString>.Flush();
        }

        /// <summary>
        /// Performs automatic conversion from string to TranslatableString
        /// </summary>
        /// <param name="str">String value</param>
        /// <returns>TranslatableString object</returns>
        public static implicit operator TranslatableString(string str)
        {
            TranslatableString obj = new TranslatableString(str);
            return obj;
        }

        /// <summary>
        /// Performs automatic conversion from TranslatableString to string value 
        /// </summary>
        /// <param name="obj">TranslatableString object</param>
        /// <returns>String value</returns>
        public static implicit operator string(TranslatableString obj)
        {
            return object.Equals(null, obj) ? string.Empty : obj.Key;
        }
    }
}
