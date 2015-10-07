using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AssetSite.Search
{
    public class SearchPage : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Request["Track"] != null)
            {
                long searchTrackId = long.Parse(Request["Track"].ToString());
                var track = new SearchTracker(UnitOfWork).GetTrackingById(searchTrackId);
                if (track == null)
                    throw new NullReferenceException("Cannot retrieve requested track");

                string url = getSearchUrl(track.SearchType);
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(track.Parameters);
                switch ((SearchType)track.SearchType)
                {
                    case SearchType.SearchByKeywords:
                    case SearchType.SearchByBarcode:
                    case SearchType.SearchByDocuments:
                        string keywords = xd.SelectSingleNode("/parameters/param").InnerText;
                        url += "?Params=" + keywords;
                        break;

                    case SearchType.SearchByCategory:
                        url += "?" + string.Join("&", (from string pairparam in
                                                           from XmlNode node in xd.SelectSingleNode("/parameters").ChildNodes
                                                           select string.Format("{0}={1}", node.Attributes["name"].Value, node.InnerText)
                                                       select pairparam).ToArray());

                        break;

                    case SearchType.SearchByContext:
                    case SearchType.SearchByType:
                        SearchParameters @params = SearchParameters.GetFromXml(track.Parameters);
                        Guid guid = Guid.NewGuid();
                        Session[guid.ToString()] = @params["elements"] as List<AttributeElement>;
                        url += "?Params=" + guid.ToString() + "&Time=" + @params["period"].ToString();
                        if ((SearchType)track.SearchType == SearchType.SearchByType)
                        {
                            url += "&TypeUID=" + @params["atUid"].ToString();
                        }
                        break;

                    default: break;
                }
                Response.Redirect(url + "&FromLog=true");
            }
        }

        private string getSearchUrl(short type)
        {
            string url = "/Search/Search.aspx";
            SearchType eType = AppFramework.Core.Classes.ExtensionMethods.NumToEnum<SearchType>((int)type);
            switch (eType)
            {
                case SearchType.SearchByBarcode:
                    url = "/Search/ResultByBarCode.aspx";
                    break;
                case SearchType.SearchByCategory:
                    url = "/Search/ResultByCategory.aspx";
                    break;
                case SearchType.SearchByContext:
                    url = "/Search/ResultByContext.aspx";
                    break;
                case SearchType.SearchByDocuments:
                    url = "/Search/ResultByDocuments.aspx";
                    break;
                case SearchType.SearchByKeywords:
                    url = "/Search/SimpleSearchKeywords.aspx";
                    break;
                case SearchType.SearchByType:
                    url = "/Search/ResultByType.aspx";
                    break;
                default: break;
            }
            return url;
        }
    }
}