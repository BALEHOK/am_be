using AppFramework.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using AppFramework.UnitTests.Common.Fixtures;
using AppFramework.Core.UnitTests.Fixtures;
using Ploeh.AutoFixture;
using AppFramework.Core.DataTypes;
using AppFramework.ConstantsEnumerators;
using Ploeh.AutoFixture.Xunit;
using Moq;
using AppFramework.Core.Classes.DynLists;

namespace AppFramework.Core.UnitTests
{
    public class AttributeValueFormatterTests
    {
        [Theory]
        [InlineAutoDomainData(1, "anonymousName")]
        [InlineAutoDomainData(null, "")]
        public void AttributeValueFormatter_GetAssetValue_ReturnsAssetName(
            long assetId,
            string assetName,
            bool active,      
            long assetUid,
            [Frozen]Mock<ILinkedEntityFinder> linkedEntityFinderMock,            
            AttributeValueFormatter sut)
        {
            // Arrange  
            var attributeConfig = new AssetTypeAttribute
            {
                DataType = new CustomDataType
                {
                    Code = Enumerators.DataType.Asset
                },
                RelatedAssetTypeID = 1,
                RelatedAssetTypeAttributeID = 1
            };
            linkedEntityFinderMock
                .Setup(x => x.GetRelatedAssetDisplayName(
                    attributeConfig.RelatedAssetTypeID.Value,
                    attributeConfig.RelatedAssetTypeAttributeID.Value,
                    assetId,
                    active))
                .Returns(assetName);
            // Act
            string result = sut.GetDisplayValue(attributeConfig, assetId, true);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetName, result);
        }

        [Theory]
        [InlineAutoDomainData(PredefinedRoles.Administrators)]
        [InlineAutoDomainData(null)]
        public void AttributeValueFormatter_GetRoleValue_ReturnsRoleName(
            PredefinedRoles role,
            long assetUid,
            AttributeValueFormatter sut)
        {
            // Arrange  
            var attributeConfig = new AssetTypeAttribute
            {
                DataType = new CustomDataType
                {
                    Code = Enumerators.DataType.Role
                },
            };
            // Act
            string result = sut.GetDisplayValue(attributeConfig, (int)role, true);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(role.ToString(), result);
        }

        [Theory]
        [InlineAutoDomainData("anonymous string")]
        public void AttributeValueFormatter_GetDynListValue_ReturnsDynListsNames(
           string dynListValue,
           long assetUid,
           [Frozen]Mock<IDynamicListsService> dynamicListsServiceMock,
           AttributeValueFormatter sut)
        {
            // Arrange 
            var attributeConfig = new AssetTypeAttribute
            {
                DataType = new CustomDataType
                {
                    Code = Enumerators.DataType.DynList
                },
            };
            dynamicListsServiceMock
                .Setup(x => x.GetLegacyListValues(attributeConfig, assetUid))
                .Returns(new List<DynamicListValue> 
                {
                    new DynamicListValue { Value = dynListValue }
                });
            // Act
            string result = sut.GetDisplayValue(attributeConfig, null, true);
            // Assert
            Assert.Equal(dynListValue, result);
        }
    }
}
