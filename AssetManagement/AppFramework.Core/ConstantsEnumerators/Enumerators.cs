namespace AppFramework.ConstantsEnumerators
{
    using System.ComponentModel;

    public enum BatchExecute
    {
        WebSite,
        WindowsService

    }

    public enum ApplicationType
    {
        AssetManager,
        SOBenBUB,
        Combined
    }

    public enum PredefinedRoles
    {
        Administrators = 1,
        Users = 2,
        OnlyPerson = 3,
        SuperUser = 4
    }

    public enum BatchStatus
    {
        Created = 0,
        Running = 1,
        Finished = 2,
        Error = 3,
        Canceled = 4,
        Waiting = 5,
        Skipped = 6,
        FinishedWithErrors = 7,
        InStack = 8
    }

    public enum ProviderParameter
    {
        ReadPath,
        WritePath,
        XMLString,
        Schema,
        LDAPCredentials
    }

    public enum ImportExportOperationType
    {
        Import,
        Export,
        Synk
    }

    public enum ImportExportStatus
    {
        New,
        Completed,
        Error
    }

    public enum ImportExportParameter
    {
        AssetTypeId,
        Guid,
        DeleteOnSuccess,
        AssetIdentifier,
        UserID,
        FilePath,
        Sheets
    }

    /// <summary>
    /// Predefined entities
    /// </summary>
    public enum PredefinedEntity
    {
        User,
        Location,
        Department,
        Document,
        Faq,
        Activity
    }

    public enum BatchActionType
    {
        CreateAssetsRevision,
        TaxonomyBatch,

        [Description("Rebuild Active Index")]
        RebuildSearchIndexActive,

        [Description("Rebuild History Index")]
        RebuildSearchIndexHistory,

        PublishAssetType,
        ImportAssets,
        MoveToLocation,
        RebuildTypeIndex,
        UpdateAssetsReferences,
        UpdateAssets,
        RecalculateAssets,

        //[Description("Rebuild reporting view(s)")]
    	RebuildReportingView,
        SynkAssets
    }

    /// <summary>
    /// Possible types of DataSource
    /// </summary>
    public enum DataSourceType
    {
        UNKNOWN,
        XLS,
        XLSX,
        XML,
    }

    public class Enumerators
    {
        /// <summary>
        /// 
        /// </summary>
        public enum DocTypes
        {
            XHTML = 1,
            DOC,
            PDF,
            XLS,
            RTF,
            HTM,
            HTML
        }

        public enum Directions
        {
            UP,
            DOWN
        }

        public enum DataType
        {
            Int,
            Long,
            Bool,
            String,
            Char,
            DateTime,
            Asset,
            Revision,
            CurrentDate,
            Assets,
            DynList,
            DynLists,
            Password,
            Float,
            Permission,
            Barcode,
            File,
            Document,
            Role,
            Email,
            Money,
            USD,
            Euro,
            Url,
            Image,
            Text,
            GoogleMaps,
            Place,
            Richtext,
            Zipcode,
            ChildAssets
        }

        public enum DBTableNames
        {
            ADynEntityUser
        }

        public enum ConnectionString
        {
            Host,
            Database,
            User,
            Password,
            IsIntergatedSecurity
        }

        public enum AssetTypeClass
        {
            DataAssetType,
            NormalAssetType
        }

        public enum TypeAutoGenerateName
        {
            None = 0,
            InsertOnly,
            InsertUpdate
        }

        /// <summary>
        /// ToolbarButton types
        /// </summary>
        public enum ToolbarButtonType
        {
            Print,
            History,
            Edit,
            Delete,
            Documents,
            Undo,
            Save,
            CurrentVersion,
            SaveAndAdd,
            Template,
            Restore,
        }

        public enum MediaType
        {
            Image,
            File
        }

        public enum ScreenState
        {
            Runtime,
            Design
        }
    }
}
