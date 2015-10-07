using System;
using System.Collections.Generic;
using System.Text;

namespace AppFramework.ConstantsEnumerators
{
    public enum BatchStatus
    {
        Created,
        Running,
        Finished,
        Error,
        Canceled,
        Waiting,
        Skipped,
        FinishedWithErrors
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
        Export
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
        DeleteOnSuccess
    }

    /// <summary>
    /// Predefined entities
    /// </summary>
    public enum PredefinedEntity
    {
        User,
        Location,
        Department,
        Document
    }

    public enum BatchActionType
    {
        CreateAssetsRevision,
        MoveToHistory,
        RebuildAssetsRelation,
        TaxonomyToSearch,
        SearchIndexing,
        ActivateAssetType,
        ImportAssets
    }

    /// <summary>
    /// Possible types of DataSource
    /// </summary>
    public enum DataSourceType
    {
        UNKNOWN,
        XML,        
        XLS,
        AD
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
            Document
        }

        /// <summary>
        /// Insert will increment the revision number
        /// Update will modify current revision
        /// ToSession wouldn't perform any actions
        /// </summary>
        public enum SaveMethod
        {
            Insert,          
            ToSession,
            Batch
        }

        public enum ConnectionString
        {
            Host,
            Database,
            User,
            Password,
            IsIntergatedSecurity
        }
    }
}
