using System;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Net.Mail;

/// <summary>
/// Generic Unhandled error handling class for ASP.NET websites and web services. 
/// Intended as a last resort for errors which crash our application, so we can get feedback on what 
/// caused the error. Can log errors to a web page, to the event log, a local text file, or via email.
/// </summary>
/// <remarks>
/// to use:
/// 
/// 1) in ASP.NET applications:
/// 
///     Reference the HttpModule UehHttpModule in web.config:
///
///         <httpModules>
///	 	        <add name="ASPUnhandledException" 
///                  type="ASPUnhandledException.UehHttpModule, ASPUnhandledException" />
///		    </httpModules>
///
/// 2) in .NET Web Services:
///
///     Reference the SoapExtension UehSoapExtension in web.config:
///
///		    <soapExtensionTypes>
///				<add type="ASPUnhandledException.UehSoapExtension, ASPUnhandledException"
///				     priority="1" group="0" />
///			</soapExtensionTypes>
///
/// more background information on Exceptions at:
///   http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnbda/html/exceptdotnet.asp
///
///  Jeff Atwood
///  http://www.codinghorror.com/
/// </remarks>

namespace AppFramework.CustomException
{
    public class Handler
    {
        private CustomExConfigHandler Config;

        private bool _blnLogToEventLog;
        private bool _blnLogToFile;
        private bool _blnLogToEmail;
        private bool _blnLogToUI;
        private string _strLogFilePath;
        private string _strIgnoreRegExp;
        private bool _blnIgnoreDebugErrors;

        private string _strException = "";
        private string _strExceptionType = "";
        private string _strViewstate = "";

        private System.Collections.Specialized.NameValueCollection _ResultCollection = new System.Collections.Specialized.NameValueCollection();

        private const string _strViewstateKey = "__VIEWSTATE";
        private const string _strRootException = "System.Web.HttpUnhandledException";
        private const string _strRootWsException = "System.Web.Services.Protocols.SoapException";
        private const string _strDefaultLogName = "UnhandledExceptionLog.txt";

        public Handler()
        {
            Config = (CustomExConfigHandler)ConfigurationManager.GetSection("AppFramework.UnhandledExceptionHandling");

            _blnLogToEventLog = Config.Logging.LogToEventLog;
            _blnLogToFile = Config.Logging.LogToFile;
            _blnLogToEmail = Config.Logging.LogToEmail;
            _blnLogToUI = Config.Logging.LogToUI;
            _strLogFilePath = Config.Logging.PathLogFile;
            _strIgnoreRegExp = Config.IgnoreErrors.IgnoreRegEx;
            _blnIgnoreDebugErrors = Config.IgnoreErrors.IgnoreDebug;
        }

        public Handler(bool LogToUI)
        {
            Config = (CustomExConfigHandler)ConfigurationManager.GetSection("AppFramework.UnhandledExceptionHandling");

            _blnLogToEventLog = Config.Logging.LogToEventLog;
            _blnLogToFile = Config.Logging.LogToFile;
            _blnLogToEmail = Config.Logging.LogToEmail;
            _blnLogToUI = LogToUI;
            _strLogFilePath = Config.Logging.PathLogFile;
            _strIgnoreRegExp = Config.IgnoreErrors.IgnoreRegEx;
            _blnIgnoreDebugErrors = Config.IgnoreErrors.IgnoreDebug; 
        }

        /// <summary>
        /// turns a single stack frame object into an informative string
        /// </summary>
        private string StackFrameToString(StackFrame sf)
        {
            StringBuilder sb = new StringBuilder();
            int intParam;
            MemberInfo mi = sf.GetMethod();

            {
                //-- build method name
                sb.Append("   ");
                sb.Append(mi.DeclaringType.Namespace);
                sb.Append(".");
                sb.Append(mi.DeclaringType.Name);
                sb.Append(".");
                sb.Append(mi.Name);

                //-- build method params
                sb.Append("(");
                intParam = 0;
                foreach (ParameterInfo param in sf.GetMethod().GetParameters())
                {
                    intParam += 1;
                    if (intParam > 1)
                        sb.Append(", ");
                    sb.Append(param.Name);
                    sb.Append(" As ");
                    sb.Append(param.ParameterType.Name);
                }
                sb.Append(")");
                sb.Append(Environment.NewLine);

                //-- if source code is available, append location info
                sb.Append("       ");
                if (sf.GetFileName() == null || sf.GetFileName().Length == 0)
                {
                    sb.Append("(unknown file)");
                    //-- native code offset is always available
                    sb.Append(": N ");
                    sb.Append(string.Format("{0:#00000}", sf.GetNativeOffset()));
                }

                else
                {
                    sb.Append(System.IO.Path.GetFileName(sf.GetFileName()));
                    sb.Append(": line ");
                    sb.Append(string.Format("{0:#0000}", sf.GetFileLineNumber()));
                    sb.Append(", col ");
                    sb.Append(string.Format("{0:#00}", sf.GetFileColumnNumber()));
                    //-- if IL is available, append IL location info
                    if (sf.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                    {
                        sb.Append(", IL ");
                        sb.Append(string.Format("{0:#0000}", sf.GetILOffset()));
                    }
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// enhanced stack trace generator
        /// </summary>
        private string EnhancedStackTrace(StackTrace st, string strSkipClassName)
        {
            int intFrame;

            StringBuilder sb = new StringBuilder();

            sb.Append(Environment.NewLine);
            sb.Append("---- Stack Trace ----");
            sb.Append(Environment.NewLine);

            for (intFrame = 0; intFrame <= st.FrameCount - 1; intFrame++)
            {
                StackFrame sf = st.GetFrame(intFrame);
                MemberInfo mi = sf.GetMethod();

                if (strSkipClassName != "" && mi.DeclaringType.Name.IndexOf(strSkipClassName) > -1)
                {
                }
                //-- don't include frames with this name
                else
                {
                    sb.Append(StackFrameToString(sf));
                }
            }
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        /// <summary>
        /// enhanced stack trace generator, using existing exception as start point
        /// </summary>
        private string EnhancedStackTrace(Exception ex)
        {
            return EnhancedStackTrace(new StackTrace(ex, true), "");
        }

        /// <summary>
        /// enhanced stack trace generator, using current execution as start point
        /// </summary>
        private string EnhancedStackTrace()
        {
            return EnhancedStackTrace(new StackTrace(true), "ASPUnhandledException");
        }


        /// <summary>
        /// ASP.Net web-service specific exception handler, to be called from UehSoapExtension
        /// </summary>
        /// <returns>
        /// string with "<detail></detail>" XML element, suitable for insertion into SOAP message
        /// </returns>
        /// <remarks>
        /// existing SOAP detail message, prior to insertion, looks like:
        /// 
        ///   <soap:Fault>
        ///     <faultcode>soap:Server</faultcode>
        ///     <faultstring>Server was unable to process request.</faultstring>
        ///     <detail />    <==  
        ///   </soap:Fault>
        /// </remarks>
        public string HandleWebServiceException(System.Web.Services.Protocols.SoapMessage sm)
        {
            _blnLogToUI = false;
            HandleException(sm.Exception);

            XmlDocument doc = new XmlDocument();
            XmlNode DetailNode = doc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);

            XmlNode TypeNode = doc.CreateNode(XmlNodeType.Element, "ExceptionType", SoapException.DetailElementName.Namespace);
            TypeNode.InnerText = _strExceptionType;
            DetailNode.AppendChild(TypeNode);

            XmlNode MessageNode = doc.CreateNode(XmlNodeType.Element, "ExceptionMessage", SoapException.DetailElementName.Namespace);
            MessageNode.InnerText = sm.Exception.Message;
            DetailNode.AppendChild(MessageNode);

            XmlNode InfoNode = doc.CreateNode(XmlNodeType.Element, "ExceptionInfo", SoapException.DetailElementName.Namespace);
            InfoNode.InnerText = _strException;
            DetailNode.AppendChild(InfoNode);

            return DetailNode.OuterXml.ToString();
        }

        public static string GetComponentRenderingErrorText()
        {
            return "<i>" + HttpContext.GetGlobalResourceObject(ConstantsEnumerators.Constants.labels, "labExceptionComponent") + "</i>";
        }

        /// <summary>
        /// ASP.Net exception handler, to be called from UehHttpModule
        /// </summary>
        public void HandleException(Exception ex)
        {

            //-- don't bother us with debug exceptions (eg those running on localhost)
            if (_blnIgnoreDebugErrors)
            {
                if (Debugger.IsAttached)
                    return;
                string strHost = HttpContext.Current.Request.Url.Host.ToLower();
                if (strHost == "localhost" || strHost == "127.0.0.1")
                {
                    return;
                }
            }

            //-- turn the exception into an informative string
            try
            {
                _strException = ExceptionToString(ex);
                _strExceptionType = ex.GetType().FullName;
                //-- ignore root exceptions
                if (_strExceptionType == _strRootException | _strExceptionType == _strRootWsException)
                {
                    if ((ex.InnerException != null))
                    {
                        _strExceptionType = ex.InnerException.GetType().FullName;
                    }
                }
            }
            catch (Exception e)
            {
                _strException = "Error '" + e.Message + "' while generating exception string";
            }

            //-- some exceptions should be ignored: ones that match this regex
            //-- note that we are using the entire full-text string of the exception to test regex against
            //-- so any part of text can match.
            if ((_strIgnoreRegExp != null) && _strIgnoreRegExp.Length > 0)
            {
                if (Regex.IsMatch(_strException, _strIgnoreRegExp, RegexOptions.IgnoreCase))
                {
                    return;
                }
            }

            //-- log this error to various locations
            try
            {
                //-- event logging takes < 100ms
                if (_blnLogToEventLog)
                    ExceptionToEventLog();
                //-- textfile logging takes < 50ms
                if (_blnLogToFile)
                    ExceptionToFile();
                //-- email takes under 1 second
                if (_blnLogToEmail)
                    ExceptionToEmail();
            }
            catch (Exception e)
            {
                //-- generic catch because any exceptions inside the UEH
                //-- will cause the code to terminate immediately
            }

            //-- display message to the user
            if (_blnLogToUI)
                ExceptionToPage();
        }

        /// <summary>
        /// turns exception into a formatted string suitable for display to a (technical) user
        /// </summary>
        private string FormatExceptionForUser()
        {
            StringBuilder sb = new StringBuilder();
            string strBullet = "•";

            {
                sb.Append(Environment.NewLine);
                sb.Append("The following information about the error was automatically captured: ");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                if (_blnLogToEventLog)
                {
                    sb.Append(" ");
                    sb.Append(strBullet);
                    sb.Append(" ");
                    if (_ResultCollection["LogToEventLog"] == "" || _ResultCollection["LogToEventLog"] == null)
                    {
                        sb.Append("an event was written to the application log");
                    }
                    else
                    {
                        sb.Append("an event could NOT be written to the application log due to an error:");
                        sb.Append(Environment.NewLine);
                        sb.Append("   '");
                        sb.Append(_ResultCollection["LogToEventLog"]);
                        sb.Append("'");
                        sb.Append(Environment.NewLine);
                    }
                    sb.Append(Environment.NewLine);
                }
                if (_blnLogToFile)
                {
                    sb.Append(" ");
                    sb.Append(strBullet);
                    sb.Append(" ");
                    if (_ResultCollection["LogToFile"] == "" || _ResultCollection["LogToFile"] == null)
                    {
                        sb.Append("details were written to a text log at:");
                        sb.Append(Environment.NewLine);
                        sb.Append("   ");
                        sb.Append(_strLogFilePath);
                        sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("details could NOT be written to the text log due to an error:");
                        sb.Append(Environment.NewLine);
                        sb.Append("   '");
                        sb.Append(_ResultCollection["LogToFile"]);
                        sb.Append("'");
                        sb.Append(Environment.NewLine);
                    }
                }
                if (_blnLogToEmail)
                {
                    sb.Append(" ");
                    sb.Append(strBullet);
                    sb.Append(" ");
                    if (_ResultCollection["LogToEmail"] == "" || _ResultCollection["LogToEmail"] == null)
                    {
                        sb.Append("an email was sent to: ");
                        sb.Append(Config.EmailTo);
                        sb.Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("email could NOT be sent due to an error:");
                        sb.Append(Environment.NewLine);
                        sb.Append("   '");
                        sb.Append(_ResultCollection["LogToEmail"]);
                        sb.Append("'");
                        sb.Append(Environment.NewLine);
                    }
                }
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("Detailed error information follows:");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(_strException);
            }
            return sb.ToString();
        }

        /// <summary>
        /// write an exception to the Windows NT event log
        /// </summary>
        private bool ExceptionToEventLog()
        {
            try
            {
                //System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
                System.Diagnostics.EventLog.WriteEntry(WebCurrentUrl(), Environment.NewLine + _strException, EventLogEntryType.Error);
                //appLog.Log = "Application";
                //appLog.Source = WebCurrentUrl();
                //if (!EventLog.SourceExists(WebCurrentUrl()))
                //{
                //    EventLog.CreateEventSource(WebCurrentUrl(), "Application");
                //}
                //appLog.WriteEntry(Environment.NewLine + _strException, EventLogEntryType.Error);
                return true;
            }
            catch (Exception ex)
            {
                _ResultCollection.Add("LogToEventLog", ex.Message);
            }
            return false;
        }

        /// <summary>
        /// replace generic constants in display strings with specific values
        /// </summary>
        private string FormatDisplayString(string strOutput)
        {
            string strTemp;
            if (strOutput == null)
            {
                strTemp = "";
            }
            else
            {
                strTemp = strOutput;
            }
            strTemp = strTemp.Replace("(app)", Config.AppName);
            strTemp = strTemp.Replace("(contact)", Config.ContactInfo);
            return strTemp;
        }

        /// <summary>
        /// writes text plus newline to http response stream
        /// </summary>
        private void WriteLine(string strAny)
        {
            HttpContext.Current.Response.Write(strAny);
            HttpContext.Current.Response.Write(Environment.NewLine);
        }

        /// <summary>
        /// writes current exception info to a web page
        /// </summary>
        private void ExceptionToPage()
        {
            string strWhatHappendTitle = Config.Messages.WhatHappenedTitle;
            string strWhatHappenedBody = Config.Messages.WhatHappenedBody;
            string strHowUserAffectedTitle = Config.Messages.HowUserIsAffectedTitle;
            string strHowUserAffectedBody = Config.Messages.HowUserIsAffectedBody;
            string strWhatUserCanDoTitle = Config.Messages.WhatUserCanDoTitle;
            string strWhatUserCanDoBody = Config.Messages.WhatUserCanDoBody;

            string strPageHeader = Config.Messages.PageHeader;
            string strPageTitle = Config.Messages.PageTitle;

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();

            WriteLine("<HTML>");
            WriteLine("<HEAD>");
            WriteLine("<TITLE>");
            WriteLine(FormatDisplayString(strPageTitle));
            WriteLine("</TITLE>");
            WriteLine("<STYLE>");
            WriteLine("body {font-family:\"Verdana\";font-weight:normal;font-size: .7em;color:black; background-color:white;}");
            WriteLine("b {font-family:\"Verdana\";font-weight:bold;color:black;margin-top: -5px}");
            WriteLine("H1 { font-family:\"Verdana\";font-weight:normal;font-size:18pt;color:red }");
            WriteLine("H2 { font-family:\"Verdana\";font-weight:normal;font-size:14pt;color:maroon }");
            WriteLine("pre {font-family:\"Lucida Console\";font-size: .9em}");
            WriteLine("</STYLE>");
            WriteLine("</HEAD>");
            WriteLine("<BODY>");
            WriteLine("<H1>");
            WriteLine(FormatDisplayString(strPageHeader));
            WriteLine("<hr width=100% size=1 color=silver></H1>");
            WriteLine("<H2>" + strWhatHappendTitle + ":</H2>");
            WriteLine("<BLOCKQUOTE>");
            WriteLine(FormatDisplayString(strWhatHappenedBody));
            WriteLine("</BLOCKQUOTE>");
            WriteLine("<H2>" + strHowUserAffectedTitle + ":</H2>");
            WriteLine("<BLOCKQUOTE>");
            WriteLine(FormatDisplayString(strHowUserAffectedBody));
            WriteLine("</BLOCKQUOTE>");
            WriteLine("<H2>" + strWhatUserCanDoTitle + ":</H2>");
            WriteLine("<BLOCKQUOTE>");
            WriteLine(FormatDisplayString(strWhatUserCanDoBody));
            WriteLine("</BLOCKQUOTE>");
            WriteLine("<INPUT type=button value=\"More Info >>\" onclick=\"this.style.display='none'; document.getElementById('MoreInfo').style.display='block'\">");
            WriteLine("<DIV style='display:none;' id='MoreInfo'>");
            WriteLine("<H2>More info:</H2>");
            WriteLine("<TABLE width=\"100%\" bgcolor=\"#ffffcc\">");
            WriteLine("<TR><TD>");
            WriteLine("<CODE><PRE>");
            WriteLine(FormatExceptionForUser());
            WriteLine("</PRE></CODE>");
            WriteLine("<TD><TR>");
            WriteLine("</DIV>");
            WriteLine("</BODY>");
            WriteLine("</HTML>");
            HttpContext.Current.Response.Flush();
            //-- 
            //-- If you use Server.ClearError to clear the exception in Application_Error, 
            //-- the defaultredirect setting in the Web.config file will not redirect the user 
            //-- because the unhandled exception no longer exists. If you want the 
            //-- defaultredirect setting to properly redirect users, do not clear the exception 
            //-- in Application_Error.
            //--
            HttpContext.Current.Server.ClearError();

            //-- don't let the calling page output any additional .Response HTML
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// write current exception info to a text file; 
        /// requires write permissions for the target folder
        /// </summary>
        private bool ExceptionToFile()
        {
            if (System.IO.Path.GetFileName(_strLogFilePath) == "")
            {
                _strLogFilePath = System.IO.Path.Combine(_strLogFilePath, _strDefaultLogName);
            }
            System.IO.StreamWriter sw = null; ;
            try
            {
                sw = new System.IO.StreamWriter(_strLogFilePath, true);
                sw.Write(_strException);
                sw.WriteLine();
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                _ResultCollection.Add("LogToFile", ex.Message);
            }
            finally
            {
                if ((sw != null))
                {
                    sw.Close();
                }
            }
            return false;
        }


        /// <summary>
        /// send current exception info via email
        /// </summary>
        private bool ExceptionToEmail()
        {
            string strMailTo = Config.EmailTo;
            if (strMailTo == "")
            {
                //-- don't bother mailing if we don't have anyone to mail to..
                _blnLogToEmail = false;
                return true;
            }

            MailMessage mm = new MailMessage();
            mm.From = new MailAddress(Config.EmailFrom);
            mm.To.Add(new MailAddress(Config.EmailTo));
            mm.Priority = MailPriority.High;
            mm.Subject = Config.Messages.SubjectMail + " - " + _strExceptionType;
            mm.Body = _strException;

            if (_strViewstate.Length > 0)
            {
                System.IO.MemoryStream memorystream = new System.IO.MemoryStream();
                System.IO.TextWriter streamwriter = new System.IO.StreamWriter(memorystream);
                streamwriter.Write(_strViewstate);
                memorystream.Seek(0, System.IO.SeekOrigin.Begin);
                mm.Attachments.Add(new Attachment(memorystream, "viewstate.txt", System.Net.Mime.MediaTypeNames.Text.Plain));
            }
            try
            {
                SmtpClient smtpclient = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SmtpClient"]);
                smtpclient.Send(mm);
                return true;
            }
            catch (Exception ex)
            {
                _ResultCollection.Add("LogToEmail", ex.Message);
                //-- we're in an unhandled exception handler
            }
            return false;
        }


        /// <summary>
        /// exception-safe WindowsIdentity.GetCurrent retrieval; returns "domain\username"
        /// </summary>
        /// <remarks>
        /// per MS, this can sometimes randomly fail with "Access Denied" on NT4
        /// </remarks>
        private string CurrentWindowsIdentity()
        {
            try
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// exception-safe System.Environment "domain\username" retrieval
        /// </summary>
        private string CurrentEnvironmentIdentity()
        {
            try
            {
                return System.Environment.UserDomainName + "\\" + System.Environment.UserName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// retrieve Process identity with fallback on error to safer method
        /// </summary>
        private string ProcessIdentity()
        {
            string strTemp = CurrentWindowsIdentity();
            if (strTemp == "")
            {
                return CurrentEnvironmentIdentity();
            }
            return strTemp;
        }

        /// <summary>
        /// returns current URL; "http://localhost:85/mypath/mypage.aspx?test=1&apples=bear"
        /// </summary>
        private string WebCurrentUrl()
        {
            string strUrl;
            {
                strUrl = "http://" + HttpContext.Current.Request.ServerVariables["server_name"];
                if (HttpContext.Current.Request.ServerVariables["server_port"] != "80")
                {
                    strUrl += ":" + HttpContext.Current.Request.ServerVariables["server_port"];
                }
                strUrl += HttpContext.Current.Request.ServerVariables["url"];

                if (HttpContext.Current.Request.ServerVariables["query_string"].Length > 0)
                {
                    strUrl += "?" + HttpContext.Current.Request.ServerVariables["query_string"];
                }
            }
            return strUrl;
        }

        /// <summary>
        /// returns brief summary info for all assemblies in the current AppDomain
        /// </summary>
        private string AllAssemblyDetailsToString()
        {
            StringBuilder sb = new StringBuilder();
            System.Collections.Specialized.NameValueCollection nvc;
            const string strLineFormat = "    {0, -30} {1, -15} {2}";

            sb.Append(Environment.NewLine);
            sb.Append(string.Format(strLineFormat, "Assembly", "Version", "BuildDate"));
            sb.Append(Environment.NewLine);
            sb.Append(string.Format(strLineFormat, "--------", "-------", "---------"));
            sb.Append(Environment.NewLine);

            foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    nvc = AssemblyAttribs(a);
                    //-- assemblies without versions are weird (dynamic?)
                    if (nvc["Version"] != "0.0.0.0")
                    {
                        sb.Append(string.Format(strLineFormat, System.IO.Path.GetFileName(nvc["CodeBase"]), nvc["Version"], nvc["BuildDate"]));
                        sb.Append(Environment.NewLine);
                    }
                }
                catch (Exception)
                {
                    // Do nothing ...
                    // Go to next assembly ...
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// returns more detailed information for a single assembly
        /// </summary>
        private string AssemblyDetailsToString(Assembly a)
        {
            StringBuilder sb = new StringBuilder();
            System.Collections.Specialized.NameValueCollection nvc = AssemblyAttribs(a);

            {
                sb.Append("Assembly Codebase:     ");
                try
                {
                    sb.Append(nvc["CodeBase"]);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                sb.Append("Assembly Full Name:    ");
                try
                {
                    sb.Append(nvc["FullName"]);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                sb.Append("Assembly Version:      ");
                try
                {
                    sb.Append(nvc["Version"]);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                sb.Append("Assembly Build Date:   ");
                try
                {
                    sb.Append(nvc["BuildDate"]);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// retrieve relevant assembly details for this exception, if possible
        /// </summary>
        private string AssemblyInfoToString(Exception ex)
        {
            //-- ex.source USUALLY contains the name of the assembly that generated the exception
            //-- at least, according to the MSDN documentation..
            System.Reflection.Assembly a = GetAssemblyFromName(ex.Source);
            if (a == null)
            {
                return AllAssemblyDetailsToString();
            }
            else
            {
                return AssemblyDetailsToString(a);
            }
        }

        /// <summary>
        /// gather some system information that is helpful in diagnosing exceptions
        /// </summary>
        private string SysInfoToString( // ERROR: Unsupported modifier : In, Optional
    bool blnIncludeStackTrace)
        {
            StringBuilder sb = new StringBuilder();

            {
                sb.Append("Date and Time:         ");
                sb.Append(DateTime.Now);
                sb.Append(Environment.NewLine);

                sb.Append("Machine Name:          ");
                try
                {
                    sb.Append(Environment.MachineName);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                sb.Append("Process User:          ");
                sb.Append(ProcessIdentity());
                sb.Append(Environment.NewLine);

                sb.Append("Remote User:           ");
                sb.Append(HttpContext.Current.Request.ServerVariables["REMOTE_USER"]);
                sb.Append(Environment.NewLine);

                sb.Append("Remote Address:        ");
                sb.Append(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                sb.Append(Environment.NewLine);

                sb.Append("Remote Host:           ");
                sb.Append(HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]);
                sb.Append(Environment.NewLine);

                sb.Append("URL:                   ");
                sb.Append(WebCurrentUrl());
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

                sb.Append("NET Runtime version:   ");
                sb.Append(System.Environment.Version.ToString());
                sb.Append(Environment.NewLine);

                sb.Append("Application Domain:    ");
                try
                {
                    sb.Append(System.AppDomain.CurrentDomain.FriendlyName);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                if (blnIncludeStackTrace)
                {
                    sb.Append(EnhancedStackTrace());
                }
            }


            return sb.ToString();
        }

        /// <summary>
        /// translate an exception object to a formatted string, with additional system info
        /// </summary>
        private string ExceptionToString(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            {
                sb.Append(ExceptionToStringPrivate(ex, true));
                //-- get ASP specific settings
                try
                {
                    sb.Append(GetASPSettings());
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// private version, called recursively for nested exceptions (inner, outer, etc)
        /// </summary>
        private string ExceptionToStringPrivate(Exception ex,  // ERROR: Unsupported modifier : In, Optional
    bool blnIncludeSysInfo)
        {

            StringBuilder sb = new StringBuilder();

            if ((ex.InnerException != null))
            {
                //-- sometimes the original exception is wrapped in a more relevant outer exception
                //-- the detail exception is the "inner" exception
                //-- see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnbda/html/exceptdotnet.asp

                //-- don't return the outer root ASP exception; it is redundant.
                if (ex.GetType().ToString() == _strRootException | ex.GetType().ToString() == _strRootWsException)
                {
                    return ExceptionToStringPrivate(ex.InnerException, true);
                }
                else
                {
                    {
                        sb.Append(ExceptionToStringPrivate(ex.InnerException, false));
                        sb.Append(Environment.NewLine);
                        sb.Append("(Outer Exception)");
                        sb.Append(Environment.NewLine);
                    }
                }
            }

            {
                //-- get general system and app information
                //-- we only really want to do this on the outermost exception in the stack
                if (blnIncludeSysInfo)
                {
                    sb.Append(SysInfoToString(false));
                    sb.Append(AssemblyInfoToString(ex));
                    sb.Append(Environment.NewLine);
                }

                //-- get exception-specific information

                sb.Append("Exception Type:        ");
                try
                {
                    sb.Append(ex.GetType().FullName);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                sb.Append("Exception Message:     ");
                try
                {
                    sb.Append(ex.Message);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                sb.Append("Exception Source:      ");
                try
                {
                    sb.Append(ex.Source);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                sb.Append("Exception Target Site: ");
                try
                {
                    sb.Append(ex.TargetSite.Name);
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);

                try
                {
                    sb.Append(EnhancedStackTrace(ex));
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
                sb.Append(Environment.NewLine);
            }


            return sb.ToString();
        }

        /// <summary>
        /// exception-safe file attrib retrieval; we don't care if this fails
        /// </summary>
        private DateTime AssemblyLastWriteTime(System.Reflection.Assembly a)
        {
            try
            {
                return System.IO.File.GetLastWriteTime(a.Location);
            }
            catch (Exception ex)
            {
                return DateTime.MaxValue;
            }
        }

        /// <summary>
        /// returns build datetime of assembly, using calculated build time if possible, or filesystem time if not
        /// </summary>
        private DateTime AssemblyBuildDate(System.Reflection.Assembly a,  // ERROR: Unsupported modifier : In, Optional
    bool blnForceFileDate)
        {

            System.Version v = a.GetName().Version;
            DateTime dt;

            if (blnForceFileDate)
            {
                dt = AssemblyLastWriteTime(a);
            }
            else
            {
                dt = ((DateTime)Convert.ToDateTime("01/01/2000")).AddDays(v.Build).AddSeconds(v.Revision * 2);
                if (TimeZone.IsDaylightSavingTime(dt, TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year)))
                {
                    dt = dt.AddHours(1);
                }
                if (dt > DateTime.Now | v.Build < 730 | v.Revision == 0)
                {
                    dt = AssemblyLastWriteTime(a);
                }
            }

            return dt;
        }

        /// <summary>
        /// returns string name / string value pair of all attribs for the specified assembly
        /// </summary>
        /// <remarks>
        /// note that Assembly* values are pulled from AssemblyInfo file in project folder
        ///
        /// Trademark       = AssemblyTrademark string
        /// Debuggable      = True
        /// GUID            = 7FDF68D5-8C6F-44C9-B391-117B5AFB5467
        /// CLSCompliant    = True
        /// Product         = AssemblyProduct string
        /// Copyright       = AssemblyCopyright string
        /// Company         = AssemblyCompany string
        /// Description     = AssemblyDescription string
        /// Title           = AssemblyTitle string
        /// </remarks>
        private System.Collections.Specialized.NameValueCollection AssemblyAttribs(System.Reflection.Assembly a)
        {
            string Name;
            string Value;
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

            foreach (object attrib in a.GetCustomAttributes(false))
            {
                Name = attrib.GetType().ToString();
                Value = "";
                switch (Name)
                {
                    case "System.Diagnostics.DebuggableAttribute":
                        Name = "Debuggable";
                        Value = ((System.Diagnostics.DebuggableAttribute)attrib).IsJITTrackingEnabled.ToString();
                        break;
                    case "System.CLSCompliantAttribute":
                        Name = "CLSCompliant";
                        Value = ((System.CLSCompliantAttribute)attrib).IsCompliant.ToString();
                        break;
                    case "System.Runtime.InteropServices.GuidAttribute":
                        Name = "GUID";
                        Value = ((System.Runtime.InteropServices.GuidAttribute)attrib).Value.ToString();
                        break;
                    case "System.Reflection.AssemblyTrademarkAttribute":
                        Name = "Trademark";
                        Value = ((AssemblyTrademarkAttribute)attrib).Trademark.ToString();
                        break;
                    case "System.Reflection.AssemblyProductAttribute":
                        Name = "Product";
                        Value = ((AssemblyProductAttribute)attrib).Product.ToString();
                        break;
                    case "System.Reflection.AssemblyCopyrightAttribute":
                        Name = "Copyright";
                        Value = ((AssemblyCopyrightAttribute)attrib).Copyright.ToString();
                        break;
                    case "System.Reflection.AssemblyCompanyAttribute":
                        Name = "Company";
                        Value = ((AssemblyCompanyAttribute)attrib).Company.ToString();
                        break;
                    case "System.Reflection.AssemblyTitleAttribute":
                        Name = "Title";
                        Value = ((AssemblyTitleAttribute)attrib).Title.ToString();
                        break;
                    case "System.Reflection.AssemblyDescriptionAttribute":
                        Name = "Description";
                        Value = ((AssemblyDescriptionAttribute)attrib).Description.ToString();
                        break;
                    default:
                        break;
                    //Console.WriteLine(Name)
                }
                if (Value != "")
                {
                    if (nvc[Name] == "")
                    {
                        nvc.Add(Name, Value);
                    }
                }
            }

            //-- add some extra values that are not in the AssemblyInfo, but nice to have
            {
                nvc.Add("CodeBase", a.CodeBase.Replace("file:///", ""));
                nvc.Add("BuildDate", AssemblyBuildDate(a, false).ToString());
                nvc.Add("Version", a.GetName().Version.ToString());
                nvc.Add("FullName", a.FullName);
            }

            return nvc;
        }

        /// <summary>
        /// matches assembly by Assembly.GetName.Name; returns nothing if no match
        /// </summary>
        private System.Reflection.Assembly GetAssemblyFromName(string strAssemblyName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetName().Name == strAssemblyName)
                {
                    return a;
                }
            }
            return null;
        }

        /// <summary>
        /// returns formatted string of all ASP.NET collections (QueryString, Form, Cookies, ServerVariables)
        /// </summary>
        private string GetASPSettings()
        {
            StringBuilder sb = new StringBuilder();

            const string strSuppressKeyPattern = "^ALL_HTTP|^ALL_RAW|VSDEBUGGER";

            {
                sb.Append("---- ASP.NET Collections ----");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(HttpVarsToString(HttpContext.Current.Request.QueryString, "QueryString", false, ""));
                sb.Append(HttpVarsToString(HttpContext.Current.Request.Form, "Form", false, ""));
                sb.Append(HttpVarsToString(HttpContext.Current.Request.Cookies));
                sb.Append(HttpVarsToString(HttpContext.Current.Session));
                sb.Append(HttpVarsToString(HttpContext.Current.Cache));
                sb.Append(HttpVarsToString(HttpContext.Current.Application));
                sb.Append(HttpVarsToString(HttpContext.Current.Request.ServerVariables, "ServerVariables", true, strSuppressKeyPattern));
            }

            return sb.ToString();
        }

        /// <summary>
        /// returns formatted string of all ASP.NET Cookies
        /// </summary>
        private string HttpVarsToString(HttpCookieCollection c)
        {
            if (c.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("Cookies");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            foreach (string strKey in c)
            {
                AppendLine(sb, strKey, c[strKey].Value);
            }

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// returns formatted summary string of all ASP.NET app vars
        /// </summary>
        private string HttpVarsToString(HttpApplicationState a)
        {
            if (a.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("Application");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            foreach (string strKey in a)
            {
                AppendLine(sb, strKey, a[strKey]);
            }

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// returns formatted summary string of all ASP.NET Cache vars
        /// </summary>
        private string HttpVarsToString(System.Web.Caching.Cache c)
        {
            if (c.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("Cache");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            foreach (System.Collections.DictionaryEntry de in c)
            {
                AppendLine(sb, Convert.ToString(de.Key), de.Value);
            }

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// returns formatted summary string of all ASP.NET Session vars
        /// </summary>
        private string HttpVarsToString(System.Web.SessionState.HttpSessionState s)
        {
            //-- sessions can be disabled
            if (s == null)
                return "";
            if (s.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("Session");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            foreach (string strKey in s)
            {
                AppendLine(sb, strKey, s[strKey]);
            }

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// returns formatted string of an arbitrary ASP.NET NameValueCollection
        /// </summary>
        private string HttpVarsToString(System.Collections.Specialized.NameValueCollection nvc, string strTitle,  // ERROR: Unsupported modifier : In, Optional
    bool blnSuppressEmpty,  // ERROR: Unsupported modifier : In, Optional
    string strSuppressKeyPattern)
        {

            if (!nvc.HasKeys())
                return "";

            StringBuilder sb = new StringBuilder();
            sb.Append(strTitle);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            bool blnDisplay;

            foreach (string strKey in nvc)
            {
                blnDisplay = true;

                if (blnSuppressEmpty)
                {
                    blnDisplay = nvc[strKey] != "";
                }

                if (strKey == _strViewstateKey)
                {
                    _strViewstate = nvc[strKey];
                    blnDisplay = false;
                }

                if (blnDisplay && strSuppressKeyPattern != "")
                {
                    blnDisplay = !Regex.IsMatch(strKey, strSuppressKeyPattern);
                }

                if (blnDisplay)
                {
                    AppendLine(sb, strKey, nvc[strKey]);
                }
            }

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// attempts to coerce the value object using the .ToString() method if possible, 
        /// then appends a formatted key/value string pair to a StringBuilder. 
        /// will display the type name if the object cannot be coerced.
        /// </summary>
        private void AppendLine(StringBuilder sb, string Key, object Value)
        {

            string strValue;
            if (Value == null)
            {
                strValue = "(Nothing)";
            }
            else
            {
                try
                {
                    strValue = Value.ToString();
                }
                catch (Exception ex)
                {
                    strValue = "(" + Value.GetType().ToString() + ")";
                }
            }

            AppendLine(sb, Key, strValue);
        }


        /// <summary>
        /// appends a formatted key/value string pair to a StringBuilder
        /// </summary>
        private void AppendLine(StringBuilder sb, string Key, string strValue)
        {

            sb.Append(string.Format("    {0, -30}{1}", Key, strValue));
            sb.Append(Environment.NewLine);
        }
    }
}