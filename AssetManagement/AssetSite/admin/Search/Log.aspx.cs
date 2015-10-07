using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine.Enumerations;

namespace AssetSite.admin.Search
{
    public partial class Log : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Grid1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected string GetSearchType(short type)
        {
            return ExtensionMethods.NumToEnum<SearchType>((int)type).ToString();
        }

        protected string GetSearchUrl(short type)
        {
            string url = "/Search/Search.aspx";
            SearchType eType = ExtensionMethods.NumToEnum<SearchType>((int)type);
            switch (eType)
            {
                case SearchType.SearchByBarcode:
                    url = "/Search/SearchByBarCode.aspx";
                    break;
                case SearchType.SearchByCategory:
                    url = "/Search/SearchByCategory.aspx";
                    break;
                case SearchType.SearchByContext:
                    url = "/Search/SearchByContext.aspx";
                    break;
                case SearchType.SearchByDocuments:
                    url = "/Search/SearchByDocuments.aspx";
                    break;
                case SearchType.SearchByKeywords:
                    url = "/Search/Search.aspx";
                    break;
                case SearchType.SearchByType:
                    url = "/Search/SearchByType.aspx";
                    break;
                default: break;
            }
            return url;
        }
    }
}