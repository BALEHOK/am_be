using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Validation.Operators
{
    using System;
    using System.Data.SqlClient;
    using AppFramework.DataProxy;

    public class ValidationOperatorUnique : ValidationOperatorBase
    {
        public override ValidationResultLine Validate(string value)
        {
            if (AssetTypeAttribute == null)
                throw new NullReferenceException("AssetTypeAttribute");

            var result = new ValidationResultLine(string.Empty);
            if (!string.IsNullOrEmpty(value))
            {
                var unitOfWork = new UnitOfWork();

                bool isUnique = true;
                if (Attribute != null)
                {
                    isUnique = unitOfWork.IsValueUnique(Attribute.ParentAsset.GetConfiguration().DBTableName,
                                        AssetTypeAttribute.DBTableFieldName,
                                        value,
                                        Attribute.ParentAsset.ID);
                }
                else
                {
                    int count;
                    var countObj = unitOfWork.SqlProvider.ExecuteScalar(string.Format("SELECT COUNT(*) FROM [{0}] WHERE [{1}] = @value",
                        AssetTypeAttribute.Parent.DBTableName,
                        AssetTypeAttribute.DBTableFieldName),
                        new SqlParameter[] { new SqlParameter("@value", value) });

                    if (countObj != null && int.TryParse(countObj.ToString(), out count))
                    {
                        isUnique = count == 0;
                    }
                }

                if (!isUnique)
                {
                    result.IsValid = false;
                    result.Message = string.IsNullOrEmpty(AssetTypeAttribute.ValidationMessage) ?
                                       string.Format("An entity with the attribute <i>{0}</i> equals <i>{1}</i> already exists. Value must be unique.",
                                           AssetTypeAttribute.NameLocalized, value) :
                                     AssetTypeAttribute.ValidationMessage;
                }
            }
            return result;
        }
    }
}
