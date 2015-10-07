using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFramework.Core.ConstantsEnumerators;
using AssetSite.Wizard;
using Core = AppFramework.Core.Classes;
using AC = AppFramework.Core.AC.Authentication;

namespace AssetSite.Helpers
{
    /// <summary>
    /// Wrapper for safety communication with the session
    /// http://www.dev102.com/2008/05/07/why-should-you-wrap-your-aspnet-session-object/
    /// </summary>
    public static class SessionWrapper
    {
        /// <summary>
        /// Cleans the Wizard session variables
        /// </summary>
        public static void CleanWizardSession()
        {
            int i = 0;
            while (i < HttpContext.Current.Session.Keys.Count)
            {
                if (HttpContext.Current.Session.Keys[i].EndsWith("Wizard")
                    || HttpContext.Current.Session.Keys[i].Contains(Step10.TREE_IDENT_CONST))
                {
                    HttpContext.Current.Session.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            HttpContext.Current.Session[SessionVariables.AssetTypeWizard_Taxonomies] = null;
            HttpContext.Current.Session[SessionVariables.AssetTypeWizard_CurrentTaxonomy] = null;
        }

        private static T GetFromSession<T>(string key)
        {
            object obj = HttpContext.Current.Session[key];
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }

        private static void SetInSession<T>(string key, T value)
        {
            if (value == null)
            {
                HttpContext.Current.Session.Remove(key);
            }
            else
            {
                HttpContext.Current.Session[key] = value;
            }
        }

        private static T GetFromApplication<T>(string key)
        {
            return (T)HttpContext.Current.Application[key];
        }

        private static void SetInApplication<T>(string key, T value)
        {
            if (value == null)
            {
                HttpContext.Current.Application.Remove(key);
            }
            else
            {
                HttpContext.Current.Application[key] = value;
            }
        }

        /// <summary>
        /// Gets and sets the asset from/to session
        /// </summary>
        public static Core.Asset Asset
        {
            get { return GetFromSession<Core.Asset>("Asset"); }
            set { SetInSession<Core.Asset>("Asset", value); }
        }

        /// <summary>
        /// Gets and sets the asset type from/to session
        /// </summary>
        public static Core.AssetType AssetType
        {
            get { return GetFromSession<Core.AssetType>("AssetType"); }
            set { SetInSession<Core.AssetType>("AssetType", value); }
        }

        /// <summary>
        /// Gets and sets the AssetTypeUID from/to session
        /// </summary>
        public static long AssetTypeUID
        {
            get { return GetFromSession<long>("AssetTypeUID"); }
            set 
            {
                if (value == 0) { SetInSession<long?>("AssetTypeUID", null); }
                else { SetInSession<long>("AssetTypeUID", value); }                
            }
        }

        /// <summary>
        /// Gets and sets authentication data from/to session
        /// </summary>
        public static AC.AuthenticationStorage UserRights
        {
            get { return GetFromSession<AC.AuthenticationStorage>("UserRights"); }
            set { SetInSession<AC.AuthenticationStorage>("UserRights", value); }
        }

        /// <summary>
        /// Gets and sets selected node of Taxonomy tree
        /// </summary>
        public static string SelectedNodePath
        {
            get { return GetFromSession<string>("AssetTreeSelectedNode"); }
            set { SetInSession<string>("AssetTreeSelectedNode", value); }
        }
    }
}
