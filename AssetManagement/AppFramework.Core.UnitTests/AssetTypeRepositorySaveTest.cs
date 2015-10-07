using AppFramework.Core.Classes;
using AppFramework.Core.UnitTests.Fixtures;
using AppFramework.DataProxy;
using AppFramework.UnitTests.Common.Fixtures;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;
using Xunit.Extensions;

namespace AppFramework.Core.UnitTests
{
    public class AssetTypeRepositorySaveTest : TransactionalMethodTest<AssetTypeRepository>
    {
        [Theory, AutoDomainData]
        public void AssetTypeRepository_Save_UsesTransaction(
            long userId,
            List<TaxonomyContainer> containers,
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            AssetTypeRepository sut)
        {
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetTypeCustomization(unitOfWorkMock.Object));
            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            RunInTransaction("Save", () =>
            {
                sut.Save(assetType, userId, containers);
                Assert.NotNull(Transaction.Current);
            });
        }

        [Theory, AutoDomainData]
        public void AssetTypeRepository_Save_RollbackOnError(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            long userId,
            List<TaxonomyContainer> containers,
            AssetTypeRepository sut)
        {
            var fixture = new Fixture() { OmitAutoProperties = true }
                .Customize(new AssetTypeCustomization(unitOfWorkMock.Object));
            var assetType = fixture.Create<AppFramework.Core.Classes.AssetType>();

            unitOfWorkMock.Setup(u => u.Commit())
                .Callback(() => { throw new Exception(); });

            var result = RunInTransaction("Save", () =>
            {
                sut.Save(assetType, userId, containers);
                Assert.NotNull(Transaction.Current);
            });

            Assert.False(result);
        }
    }
}
