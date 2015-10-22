using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;

namespace AppFramework.Core.Classes.Tasks.Runners
{
    class SearchRunner : ITaskRunner
    {
        public SearchConfigurationDescriptor Params { get; set; }

        public SearchRunner()
        {
            Params = new SearchConfigurationDescriptor();
        }

        public virtual TaskResult Run(Entities.Task task)
        {
            var uriBuilder = new UriBuilder(
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Host,
                HttpContext.Current.Request.Url.Port);
            
            switch (this.Params.SearchType)
            {
                case SearchType.SearchByKeywords:
                    uriBuilder.Path = "/Search/SimpleSearchKeywords.aspx";
                    break;
                case SearchType.SearchByCategory:
                    uriBuilder.Path = "/Search/ResultByCategory.aspx";
                    break;
                case SearchType.SearchByContext:
                    uriBuilder.Path = "/Search/ResultByContext.aspx";
                    break;
                case SearchType.SearchByDocuments:
                    uriBuilder.Path = "/Search/ResultByDocuments.aspx";
                    break;
                case SearchType.SearchByType:
                    uriBuilder.Path = "/Search/ResultByType.aspx";
                    break;
            }

            var dictionary = buildDictionaryFromSearchParams(Params);

            uriBuilder.Query = string.Join("&", 
                dictionary.Select(p => string.Format("{0}={1}", p.Key, p.Value)));

            return new TaskResult((TaskFunctionType)task.FunctionType)
            {
                Status = TaskStatus.Sussess,
                NavigationResult = uriBuilder.ToString(),
                NavigationResultArguments = dictionary
            };
        }

        private Dictionary<string, object> buildDictionaryFromSearchParams(
            SearchConfigurationDescriptor @params)
        {
            var dictionary = new Dictionary<string, object>();
            var properties = @params.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(@params, null);
                if (value is List<AttributeElement>)
                {
                    // these parameters passing via session
                    var guid = Guid.NewGuid().ToString();
                    HttpContext.Current.Session[guid] = value;
                    dictionary.Add(property.Name, guid);
                }
                else if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    // normal url parameter
                    dictionary.Add(
                        property.Name, 
                        property.PropertyType.IsEnum
                            ? ((int) value).ToString()
                            : HttpContext.Current.Server.UrlEncode(value.ToString()));
                }
            }
            return dictionary;
        }
    }
}
