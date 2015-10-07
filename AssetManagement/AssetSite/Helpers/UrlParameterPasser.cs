using System.Collections;
using System.Web;

namespace AssetSite.Helpers
{
    public abstract class BaseParameterPasser
    {
        private string _url = string.Empty;

        public BaseParameterPasser()
        {
            if (HttpContext.Current != null)
                _url = HttpContext.Current.Request.Url.ToString();
        }

        public BaseParameterPasser(string Url)
        {
            this._url = Url;
        }

        public virtual void PassParameters()
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Response.Redirect(Url);
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public abstract string this[string name]
        {
            get;
            set;
        }

        public abstract ICollection Keys
        {
            get;
        }
    }

    public class UrlParameterPasser : BaseParameterPasser
    {
        private SortedList localQueryString = null;

        public UrlParameterPasser() : base() { }
        public UrlParameterPasser(string url)
        {
            if (url.Contains("?"))
            {
                var parts = url.Split('?');
                base.Url = parts[0];
                foreach (var paramPair in parts[1].Split('&'))
                {
                    var key = HttpContext.Current.Server.UrlDecode(paramPair.Split('=')[0]);
                    var value = HttpContext.Current.Server.UrlDecode(paramPair.Split('=')[1]);
                    this[key] = value;
                }
            }
        }

        public override void PassParameters()
        {
            // add parameters, if any exist
            if (localQueryString.Count > 0)
            {
                // see if we need to add the ?
                if (base.Url.IndexOf("?") == -1)
                    base.Url += "?";
                else
                    base.Url += "&";

                bool firstOne = true;
                foreach (DictionaryEntry o in localQueryString)
                {
                    if (!firstOne)
                        base.Url += "&";
                    else
                        firstOne = false;

                    base.Url += string.Concat(
                                HttpContext.Current.Server.UrlEncode(o.Key.ToString()),
                                "=",
                                HttpContext.Current.Server.UrlEncode(o.Value.ToString()));
                }
            }
            base.PassParameters();
        }

        public override string this[string name]
        {
            get
            {
                if (localQueryString == null)
                {
                    if (HttpContext.Current != null)
                        return HttpContext.Current.Request.QueryString[name];
                    else
                        return null;
                }
                else
                    return localQueryString[name].ToString();
            }
            set
            {
                if (localQueryString == null)
                    localQueryString = new SortedList();

                // add if it is new, or replace the old value
                if ((localQueryString[name]) == null)
                    localQueryString.Add(name, value);
                else
                    localQueryString[name] = value;
            }
        }

        public override ICollection Keys
        {
            get
            {
                if (localQueryString == null)
                {
                    if (HttpContext.Current != null)
                        return HttpContext.Current.Request.QueryString.Keys;
                    else
                        return null;
                }
                else
                    return localQueryString.Keys;
            }
        }
    }
}