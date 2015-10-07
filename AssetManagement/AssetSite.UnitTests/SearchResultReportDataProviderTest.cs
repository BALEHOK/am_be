using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.Exceptions;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetSite.Reports.DataProviders;
using Moq;
using Ploeh.AutoFixture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace AssetSite.UnitTests
{
	public class SearchResultReportDataProviderTest
	{
		[Fact]
		public void SearchResultReportDataProvider_WhenExporterIsNull_Throws()
		{
			// Arrange
			// Act
			// Assert
			Assert.Throws<ArgumentNullException>(() => 
				new SearchResultReportDataProvider(null, default(long), null, ""));
		}

		[Fact]
		public void SearchResultReportDataProvider_WhenFilterIsNull_Throws()
		{
			// Arrange
			var exporter = new Mock<IAssetsExporter>();
			// Act
			// Assert
			Assert.Throws<ArgumentNullException>(() =>
				new SearchResultReportDataProvider(exporter.Object, default(long), null, ""));
		}

		[Fact]
		public void SearchResultReportDataProvider_GetData_ReturnsDataTable()
		{
			// Arrange
			var accessManagerStub = new Mock<IAccessManager>();
			var uowStub = new Mock<IUnitOfWork>();

			var anonymousFilter = new List<AttributeElement>
			{
				new AttributeElement{ },
			};
			var anonymousAssetTypeUid = 123;
			var table = new DataTable();
			table.Columns.Add("Dosage", typeof(int)); // Add five columns.
			table.Columns.Add("Drug", typeof(string));
			table.Columns.Add("Name", typeof(string));
			table.Columns.Add("Date", typeof(DateTime));
			table.Rows.Add(15, "Abilify", "Jacob", DateTime.Now); // Add five data rows.
			table.Rows.Add(40, "Accupril", "Emma", DateTime.Now);
			table.Rows.Add(40, "Accutane", "Michael", DateTime.Now);
			table.Rows.Add(20, "Aciphex", "Ethan", DateTime.Now);
			table.Rows.Add(45, "Actos", "Emily", DateTime.Now);
			var exporterMock = new Mock<IAssetsExporter>();
			exporterMock.Setup(e => e.ExportToDataTable(anonymousAssetTypeUid, anonymousFilter))
				.Returns(table);

			var sut = new SearchResultReportDataProvider(
				exporterMock.Object,
				anonymousAssetTypeUid,
				anonymousFilter, 
				"");
			// Act
            var result = sut.GetData(anonymousAssetTypeUid);
			// Assert
			Assert.NotNull(result);
		}
	}
}
