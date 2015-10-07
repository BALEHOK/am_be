using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;

using AppFramework.CustomResourceProviders;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.CustomException
{
    public class CustomExConfigHandler: ConfigurationSection
    {
        public CustomExConfigHandler()
        {

        }

        ~CustomExConfigHandler()
        {

        }

        [ConfigurationProperty("AppName", IsRequired = true)]
        public string AppName
        {
            get { return (string)this["AppName"]; }
            set { this["AppName"] = value; }
        }

        [ConfigurationProperty("ContactInfo", IsRequired = true)]
        public string ContactInfo
        {
            get { return (string)this["ContactInfo"]; }
            set { this["ContactInfo"] = value; }
        }

        [ConfigurationProperty("EmailTo", IsRequired = false)]
        public string EmailTo
        {
            get { return (string)this["EmailTo"]; }
            set { this["EmailTo"] = value; }
        }

        [ConfigurationProperty("EmailFrom", IsRequired = false)]
        public string EmailFrom
        {
            get { return (string)this["EmailFrom"]; }
            set { this["EmailFrom"] = value; }
        }

        [ConfigurationProperty("Logging")]
        public LoggingElement Logging
        {
            get { return (LoggingElement)this["Logging"]; }
            set { this["Logging"] = value; }
        }

        [ConfigurationProperty("IgnoreErrors")]
        public IgnoreErrorsElement IgnoreErrors
        {
            get { return (IgnoreErrorsElement)this["IgnoreErrors"]; }
            set { this["IgnoreErrors"] = value; }
        }

        [ConfigurationProperty("Messages")]
        public MessagesElement Messages
        {
            get { return (MessagesElement)this["Messages"]; }
            set { this["Messages"] = value; }
        }
    }

    public class LoggingElement : ConfigurationElement
    {
        public LoggingElement()
        {

        }

        [ConfigurationProperty("LogToEventLog", DefaultValue = false, IsRequired = false)]
        public bool LogToEventLog
        {
            get { return (bool)this["LogToEventLog"]; }
            set { this["EmailTo"] = value; }
        }

        [ConfigurationProperty("LogToFile", DefaultValue = false, IsRequired = false)]
        public bool LogToFile
        {
            get { return (bool)this["LogToFile"]; }
            set { this["LogToFile"] = value; }
        }

        [ConfigurationProperty("LogToEmail", DefaultValue = false, IsRequired = false)]
        public bool LogToEmail
        {
            get { return (bool)this["LogToEmail"]; }
            set { this["LogToEmail"] = value; }
        }

        [ConfigurationProperty("LogToUI", DefaultValue = false, IsRequired = false)]
        public bool LogToUI
        {
            get { return (bool)this["LogToUI"]; }
            set { this["LogToUI"] = value; }
        }

        [ConfigurationProperty("PathLogFile", DefaultValue = "", IsRequired = false)]
        public string PathLogFile
        {
            get { return (string)this["PathLogFile"]; }
            set { this["PathLogFile"] = value; }
        }
    }

    public class IgnoreErrorsElement : ConfigurationElement
    {
        public IgnoreErrorsElement()
        {

        }

        [ConfigurationProperty("IgnoreRegEx", IsRequired = false)]
        public string IgnoreRegEx
        {
            get { return (string)this["IgnoreRegEx"]; }
            set { this["IgnoreRegEx"] = value; }
        }

        [ConfigurationProperty("IgnoreDebug", DefaultValue = false, IsRequired = false)]
        public bool IgnoreDebug
        {
            get { return (bool)this["IgnoreDebug"]; }
            set { this["IgnoreDebug"] = value; }
        }
    }

    public class MessagesElement : ConfigurationElement
    {
        public MessagesElement()
        {

        }

        [ConfigurationProperty("PageHeader", IsRequired = true)]
        public string PageHeader
        {
            get 
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionPageHeader").ToString();
                }
                catch (Exception)
                {
                    return (string)this["PageHeader"]; 
                }
            }
            set { this["PageHeader"] = value; }
        }

        [ConfigurationProperty("PageTitle", IsRequired = true)]
        public string PageTitle
        {
            get
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionPageTitle").ToString();
                }
                catch (Exception)
                {
                    return (string)this["PageTitle"];
                }
            }
            set { this["PageTitle"] = value; }
        }

        [ConfigurationProperty("WhatHappenedTitle", IsRequired = true)]
        public string WhatHappenedTitle
        {
            get
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionWhatHappenedTitle").ToString();
                }
                catch (Exception)
                {
                    return (string)this["WhatHappenedTitle"];
                }
            }
            set { this["WhatHappenedTitle"] = value; }
        }

        [ConfigurationProperty("WhatHappenedBody", IsRequired = true)]
        public string WhatHappenedBody
        {
            get 
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionWhatHappenedBody").ToString();
                }
                catch (Exception)
                {
                    return (string)this["WhatHappenedBody"]; 
                }
            }
            set { this["WhatHappenedBody"] = value; }
        }

        [ConfigurationProperty("HowUserIsAffectedTitle", IsRequired = true)]
        public string HowUserIsAffectedTitle
        {
            get 
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionHowAffectTitle").ToString();
                }
                catch (Exception)
                {
                    return (string)this["HowUserIsAffectedTitle"]; 
                }
            }
            set { this["HowUserIsAffectedTitle"] = value; }
        }

        [ConfigurationProperty("HowUserIsAffectedBody", IsRequired = true)]
        public string HowUserIsAffectedBody
        {
            get
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionHowAffectBody").ToString();
                }
                catch (Exception)
                {
                    return (string)this["HowUserIsAffectedBody"];
                }
            }
            set { this["HowUserIsAffectedBody"] = value; }
        }

        [ConfigurationProperty("WhatUserCanDoTitle", IsRequired = true)]
        public string WhatUserCanDoTitle
        {
            get 
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionWhatDoTitle").ToString();
                }
                catch (Exception)
                {
                    return (string)this["WhatUserCanDoTitle"]; 
                }
            }
            set { this["WhatUserCanDoTitle"] = value; }
        }

        [ConfigurationProperty("WhatUserCanDoBody", IsRequired = true)]
        public string WhatUserCanDoBody
        {
            get
            {
                try
                {
                    return HttpContext.GetGlobalResourceObject(Constants.labels, "labExceptionWhatDoBody").ToString();
                }
                catch (Exception)
                {
                    return (string)this["WhatUserCanDoBody"];
                }
            }
            set { this["WhatUserCanDoBody"] = value; }
        }

        [ConfigurationProperty("SubjectMail", IsRequired = true)]
        public string SubjectMail
        {
            get { return (string)this["SubjectMail"]; }
            set { this["SubjectMail"] = value; }
        }
    }
}
