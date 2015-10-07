using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetManager.Infrastructure.Services
{
    public interface IHistoryService
    {
        IEnumerable<AssetHistoryAttributeModel> GetChangesBetweenRevisions(
            Asset revisionA, Asset revisionB);
    }

    public class HistoryService : IHistoryService
    {
        private readonly IAttributeValueFormatter _valueFormatter;

        public HistoryService(IAttributeValueFormatter valueFormatter)
        {
            if (valueFormatter == null)
                throw new ArgumentNullException("valueFormatter");
            _valueFormatter = valueFormatter;
        }

        public IEnumerable<AssetHistoryAttributeModel> GetChangesBetweenRevisions(
            Asset revisionA, Asset revisionB)
        {
            foreach (var attributeA in revisionA.AttributesPublic)
            {
                if (revisionB == null)
                    yield break;

                var attributeB = revisionB
                    .AttributesPublic
                    .SingleOrDefault(a => a.Configuration.DBTableFieldName ==
                        attributeA.Configuration.DBTableFieldName);

                var valueA = _valueFormatter.GetDisplayValue(
                            attributeA.Configuration,
                            attributeA.Data.Value,
                            !revisionA.IsHistory);

                if (attributeB == null)
                {
                    // added attribute
                    yield return new AssetHistoryAttributeModel
                    {
                        NewValue = valueA,
                        Id = attributeA.Configuration.ID,
                        Name = attributeA.Configuration.Name,
                        DataType = attributeA.Configuration.DataTypeEnum.ToString().ToLower()
                    };
                }
                else
                {
                    var valueB = _valueFormatter.GetDisplayValue(
                            attributeB.Configuration,
                            attributeB.Data.Value,
                            !revisionB.IsHistory);

                    if (valueB != valueA)
                    {
                        // changed attribute
                        yield return new AssetHistoryAttributeModel
                        {
                            NewValue = valueA,
                            OldValue = valueB,
                            Id = attributeA.Configuration.ID,
                            Name = attributeA.Configuration.Name,
                            DataType = attributeA.Configuration.DataTypeEnum.ToString().ToLower()
                        };
                    }
                }
            }
        }
    }
}
