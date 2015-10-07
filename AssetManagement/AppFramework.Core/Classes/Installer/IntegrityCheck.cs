/*--------------------------------------------------------
* IntegrityCheck.cs
* 
* Copyright: DAXX
* Author: aNesterov
* Created: 11/2/2009 3:54:19 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace AppFramework.Core.Classes.Installer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Web;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Xml.Serialization;

    [Serializable]
    [XmlInclude(typeof(CheckAssemblyExists)), XmlInclude(typeof(CheckAssemblyVersion)), XmlInclude(typeof(CheckFolderRights))]
    public abstract class CheckRule
    {
        public string Name { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsValid { get { return this.Validate(); } }
        public abstract bool Validate();
    }

    [Serializable]    
    public class CheckAssemblyVersion : CheckRule
    {
        public override bool Validate()
        {
            bool result = true;

            try
            {
                Assembly.Load(this.Parameter);
                // AppDomain.CurrentDomain.Load(this.Parameter);
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }

    [Serializable]   
    public class CheckAssemblyExists : CheckRule
    {
        public override bool Validate()
        {
            bool result = true;

            try
            {
                AssemblyName aname = new AssemblyName();
                aname.Name = this.Parameter;
                Assembly.Load(aname);
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }

    [Serializable]    
    public class CheckFolderRights : CheckRule
    {
        public override bool Validate()
        {
            HttpServerUtility server = HttpContext.Current.Server;
            if (server != null)
            {
                bool result = false;
                try
                {
                    string file = Path.Combine(server.MapPath(this.Parameter), "install.test");
                    using (File.Create(file))
                    {
                    }

                    File.Delete(file);

                    result = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
            else
            {
                this.ErrorMessage = "Run check only from web-interface!";
                return false;
            }
        }
    }

    [Serializable]    
    public class CheckRules : List<CheckRule>
    {
    }

    [Serializable]
    public class IntegrityChecker
    {
        [XmlArray]
        private CheckRules rules = new CheckRules();

        public IntegrityChecker()
        {
        }

        public IntegrityChecker(string configName)
            : base()
        {
            this.LoadConfig(configName);
        }

        public CheckRules Rules
        {
            get
            {
                return this.rules;
            }
        }

        public void SaveConfig(string name)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.rules.GetType());
            string path = IntegrityChecker.GetConfigPath(name);
            using (var file = File.Create(path))
            {
                xmlSerializer.Serialize(file, this.rules);
            }
        }

        /// <summary>
        /// Determines whether is config exists with the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if config exists with the specified name; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConfigExists(string name)
        {
            string path = IntegrityChecker.GetConfigPath(name);
            return File.Exists(path);
        }

        private static string GetConfigPath(string configName)
        {
            HttpServerUtility server = HttpContext.Current.Server;
            string path = Path.Combine(server.MapPath("~/Config.Files"), configName + ".config");
            return path;
        }

        public static string DefaultConfigName
        {
            get
            {
                return "IntegrityCheck";
            }
        }

        public bool LoadConfig(string name)
        {
            bool result = false;
            string path = IntegrityChecker.GetConfigPath(name);
            XmlSerializer xmlSerializer = new XmlSerializer(this.rules.GetType());
            try
            {
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    this.rules = xmlSerializer.Deserialize(file) as CheckRules;
                }

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
