using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using AppFramework.ConstantsEnumerators;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public class ApplicationSettings
    {
        private static string _activityCompletedStatus;
        public const int BulkInsertTheshold = 1000;

        public static CultureInfo DisplayCultureInfo
        {
            get
            {
                //return new CultureInfo(CultureName, true);
                //return System.Threading.Thread.CurrentThread.CurrentCulture;
                var cultureInfo = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name, true);
                cultureInfo.NumberFormat.CurrencyDecimalDigits = 2;
                return cultureInfo;
            }
        }

        public static CultureInfo PersistenceCultureInfo
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
                //return new CultureInfo("en-US", true);
            }
        }

        public static ApplicationType ApplicationType
        {
            get
            {
                var result = ApplicationType.AssetManager;
                if (ConfigurationManager.AppSettings["ApplicationType"] != null)
                {
                    if (ConfigurationManager.AppSettings["ApplicationType"] == "SOBnBUB")
                    {
                        result = ApplicationType.SOBenBUB;
                    }
                    else if (ConfigurationManager.AppSettings["ApplicationType"] == "Combined")
                    {
                        result = ApplicationType.Combined;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the list of words which must be excluded from search query
        /// </summary>
        public static List<string> SearchExcludeWords
        {
            get { return GetStringPropertyByName("SearchExcludeWords").Split(' ', ',').ToList(); }
            set
            {
                var words = value.ToArray();
                Array.Sort(words);
                SetStringPropertyByName("SearchExcludeWords", string.Join(" ", words));
            }
        }

        /// <summary>
        /// Gets and sets if application configuration page must be shown.
        /// </summary>
        public static bool ShowConfigurationPage
        {
            get { return GetBoolPropertyByName("ShowConfigurationPage"); }
            set { SetBoolPropertyByName("ShowConfigurationPage", value); }
        }

        public static void UpdateConnectionString(string host, string database, string user, string password,
            string isIntegrated)
        {
            Routines.SetSectionValueToConnectionString(Enumerators.ConnectionString.User, user);
            Routines.SetSectionValueToConnectionString(Enumerators.ConnectionString.Password, password);
            Routines.SetSectionValueToConnectionString(Enumerators.ConnectionString.Host, host);
            Routines.SetSectionValueToConnectionString(Enumerators.ConnectionString.Database, database);
            Routines.SetSectionValueToConnectionString(Enumerators.ConnectionString.IsIntergatedSecurity, isIntegrated);
        }

        /// <summary>
        /// Gets and sets the connection string of application
        /// </summary>
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[0].ConnectionString; }
        }

        /// <summary>
        /// Gets the Application version.
        /// </summary>
        public static Version ApplicationVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        private static bool GetBoolPropertyByName(string propertyName)
        {
            var unitOfWork = new UnitOfWork();
            var entity = unitOfWork.AppSettingsRepository.SingleOrDefault(a => a.PropertyName == propertyName);
            var res = false;
            if (entity != null)
            {
                res = entity.PropertyValue.ToLower() == true.ToString().ToLower();
            }
            return res;
        }

        private static void SetBoolPropertyByName(string propertyName, bool value)
        {
            SetStringPropertyByName(propertyName, value.ToString());
        }

        private static void SetStringPropertyByName(string propertyName, string value)
        {
            var unitOfWork = new UnitOfWork();
            var entity = unitOfWork.AppSettingsRepository.SingleOrDefault(a => a.PropertyName == propertyName);
            if (entity == null)
            {
                entity = new AppSettings
                {
                    PropertyName = propertyName
                };
                entity.PropertyValue = value;
                unitOfWork.AppSettingsRepository.Insert(entity);
            }
            else
            {
                entity.PropertyValue = value;
                unitOfWork.AppSettingsRepository.Update(entity);
            }
            unitOfWork.Commit();
        }

        private static string GetStringPropertyByName(string propertyName)
        {
            var unitOfWork = new UnitOfWork();
            var entity = unitOfWork.AppSettingsRepository.SingleOrDefault(a => a.PropertyName == propertyName);
            var res = string.Empty;
            if (entity != null)
            {
                res = entity.PropertyValue;
            }
            return res;
        }

        /// <summary>
        /// Returns number of records for one page of all GridViews
        /// </summary>
        /// <returns></returns>
        public static int RecordsPerPage
        {
            get
            {
                var recordsNum = 20;
                if (ConfigurationManager.AppSettings["RecordsPerPage"] != null)
                {
                    var tmp = ConfigurationManager.AppSettings["RecordsPerPage"];
                    int.TryParse(tmp, out recordsNum);
                }
                return recordsNum;
            }
        }

        /// <summary>
        /// Gets the batch default schedule time.
        /// </summary>
        /// <value>The batch default schedule time.</value>
        public static DateTime? BatchDefaultScheduleTime
        {
            get
            {
                DateTime? nw = null;
                DateTime nowTime;
                var tmp = ConfigurationManager.AppSettings["BatchDefaultScheduleTime"];
                if (DateTime.TryParse(tmp, out nowTime))
                {
                    // if selected time already passed - move schedule on next day                    
                    if (DateTime.Now > nowTime)
                        //new DateTime(now.Year, now.Month, now.Day, nowTime.Hour, nowTime.Minute, nowTime.Second))
                    {
                        nowTime = nowTime.AddDays(1);
                    }
                    nw = nowTime;
                }
                return nw;
            }
        }

        /// <summary>
        /// Gets the file path to XML with predefined asset type.
        /// </summary>
        public static string PredefinedATPath
        {
            get { return MapPath(ConfigurationManager.AppSettings["PredefinedATPath"]); }
        }

        /// <summary>
        /// Gets the filepath to XML file with predefined assets.
        /// </summary>
        public static string PredefinedAssetsPath
        {
            get { return MapPath(ConfigurationManager.AppSettings["PredefinedAssetsPath"]); }
        }

        /// <summary>
        /// Gets the path for files uploading on import.
        /// With end slash.
        /// </summary>
        public static string UploadOnImportPath
        {
            get { return MapPath(ConfigurationManager.AppSettings["UploadOnImportPath"] ?? "~/App_Data/"); }
        }

        /// <summary>
        /// Gets the path to the default XSD schema
        /// </summary>
        public static string AssetsSchemaPath
        {
            get { return MapPath(ConfigurationManager.AppSettings["AssetsSchemaPath"]); }
        }

        private static string MapPath(string relativePath)
        {
            return HttpContext.Current.Request.MapPath(relativePath);
        }

        /// <summary>
        /// Gets the template path.
        /// </summary>
        /// <value>The template path.</value>
        public static string TemplatesPath
        {
            get
            {
                var tmp = ConfigurationManager.AppSettings["TemplatesPath"];
                if (string.IsNullOrEmpty(tmp))
                {
                    tmp = "~/admin/Reports/Templates/";
                }
                return tmp;
            }
        }

        public static string AssetTemplatesPath
        {
            get
            {
                var tmp = ConfigurationManager.AppSettings["AssetTemplatePath"];
                if (String.IsNullOrEmpty(tmp))
                    tmp = "~/App_Data/Asset_Templates/";
                return MapPath(tmp);
            }
        }

        /// <summary>
        /// Path for assets export
        /// </summary>
        public static string ExportPath
        {
            get { return MapPath(ConfigurationManager.AppSettings["ExportPath"]); }
        }

        /// <summary>
        /// Path to AD user attributes XML
        /// </summary>
        public static string ActiveDirectoryUserFieldsPath
        {
            get { return MapPath(ConfigurationManager.AppSettings["ActiveDirectoryUserFieldsPath"]); }
        }

        /// <summary>
        /// Gets the documents path.
        /// </summary>
        /// <value>The documents path.</value>
        public static string DocumentsPath
        {
            get
            {
                var tmp = ConfigurationManager.AppSettings["DocumentsPath"];
                return string.IsNullOrEmpty(tmp) ? "~/Documents/" : tmp;
            }
        }

        /// <summary>
        /// Gets the temp path.
        /// </summary>
        /// <value>The temp path.</value>
        public static string TempFullPath
        {
            get
            {
                var path = ConfigurationManager.AppSettings["TempPath"] ?? "~/App_Data/";
                return MapPath(path);
            }
        }

        public static int MaxPasswordLength
        {
            get { return 25; }
        }

        /// <summary>
        /// Gets the length of the barcode.
        /// </summary>
        /// <value>The length of the barcode.</value>
        public static int BarcodeLength
        {
            get
            {
                int tmpRet;
                var tmp = ConfigurationManager.AppSettings["BarcodeLength"];

                if (string.IsNullOrEmpty(tmp) || !int.TryParse(tmp, out tmpRet))
                {
                    return 7;
                }

                if (tmpRet < 7 || tmpRet > 48)
                {
                    throw new ConfigurationErrorsException(
                        "Barcode length must be within range 7 - 48 characters. See BarcodeLength parameter in AppSettings.");
                }

                return tmpRet;
            }
        }

        /// <summary>
        /// Gets the path to DB frame file.
        /// </summary>
        public static string DatabaseFrame
        {
            get { return MapPath(ConfigurationManager.AppSettings["DatabaseFrame"]); }
        }

        /// <summary>
        /// Gets the path to DB SPs file.
        /// </summary>
        public static string DatabaseProcedures
        {
            get { return MapPath(ConfigurationManager.AppSettings["DatabaseProcedures"]); }
        }

        public static string CnnStringsFilePath
        {
            get { return MapPath("~/Config.Files/connectionStrings.config"); }
        }

        /// <summary>
        /// Gets the file path to XML file with predefined extra attributes for Document AssetType.
        /// </summary>
        public static string DocumentExtraAttributes
        {
            get
            {
                return Path.Combine(MapPath(ConfigurationManager.AppSettings["ExtraAttributesPath"]),
                    "DocumentExtraAttributes.xml");
            }
        }

        /// <summary>
        /// Gets the file path to XML file with predefined extra attributes for FAQ AssetType.
        /// </summary>
        public static string FaqExtraAttributes
        {
            get
            {
                return Path.Combine(MapPath(ConfigurationManager.AppSettings["ExtraAttributesPath"]),
                    "FaqExtraAttributes.xml");
            }
        }

        /// <summary>
        /// Gets the file path to XML file with predefined extra attributes for User AssetType.
        /// </summary>
        public static string UserExtraAttributes
        {
            get
            {
                return Path.Combine(MapPath(ConfigurationManager.AppSettings["ExtraAttributesPath"]),
                    "UserExtraAttributes.xml");
            }
        }

        /// <summary>
        /// Panel controls width
        /// </summary>
        public static string ControlsWidth
        {
            get { return ConfigurationManager.AppSettings["ControlsWidth"]; }
        }

        /// <summary>
        /// Upload folder
        /// </summary>
        public static string UploadFolder
        {
            get { return ConfigurationManager.AppSettings["UploadFolder"]; }
        }

        public static string IsNetworkUploadFolder
        {
            get { return ConfigurationManager.AppSettings["IsNetworkUploadFolder"]; }
        }

        /// <summary>
        /// Returns the set of restricted attributes which are editable only by Administrators
        /// </summary>
        public static IEnumerable<string> RestrictedAttributes
        {
            get
            {
                return new string[9]
                {
                    "Name", "Password", "Email", "Users", "Permissions On Users", "Role", "Staff", "Unionist",
                    "Contact"
                };
            }
        }

        public static TimeSpan CacheManagerTimeout
        {
            get { return TimeSpan.FromSeconds(15); }
        }

        public static string ActivityCompletedStatus
        {
            get
            {
                if (string.IsNullOrEmpty(_activityCompletedStatus))
                {
                    _activityCompletedStatus = ConfigurationManager.AppSettings["ActivityCompletedStatus"];
                }
                return _activityCompletedStatus;
            }
        }
    }
}