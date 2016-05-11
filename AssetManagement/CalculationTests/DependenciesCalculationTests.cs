using System;
using System.Linq;
using System.Security.Claims;
using System.Transactions;
using System.Web.Security;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AssetSite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculationTests
{
    [TestClass]
    public class DependenciesAppTests : AppTest
    {    
        [TestInitialize]
        public void Init()
        {
            InitUnity();

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
            // ReSharper disable once ObjectCreationAsStatement
            new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        //[TestMethod]
        // unit tests shouldn't be run on a database!
        // TODO: rewrite with xUnit and mock DB
        public void AddDependency()
        {
            var order = Helper.CreateAsset("ADynEntitytestoder", "testorder");            
            order["DiscountPercent"].Value = "10";
            order = AssetsService.InsertAsset(order);

            var detail = Helper.CreateAsset("ADynEntitytestorderdetails", "order_detail_1");            
            detail["OrderIdlink"].ValueAsId = order.ID;
            detail["Product_Amount"].Value = "2";
            detail["Product_Price"].Value = "10";
            detail = AssetsService.InsertAsset(detail);

            Assert.AreEqual("20", detail["Order_Details_Amount"].Value);

            order = AssetsService.GetAssetById(order.ID, Helper.GetTypeByName("ADynEntitytestoder"));

            Assert.AreEqual("20", order["OrderAmount"].Value);
        }

        //[TestMethod]
        // unit tests shouldn't be run on a database!
        // TODO: rewrite with xUnit and mock DB
        public void ChangeDependency()
        {
            var order = Helper.CreateAsset("ADynEntitytestoder", "testorder");
            order["DiscountPercent"].Value = "10";
            order = AssetsService.InsertAsset(order);

            var detail = Helper.CreateAsset("ADynEntitytestorderdetails", "order_detail_1");
            detail["Name"].Value = "order_detail_1";
            detail["OrderIdlink"].ValueAsId = order.ID;
            detail["Product_Amount"].Value = "2";
            detail["Product_Price"].Value = "10";
            detail = AssetsService.InsertAsset(detail);

            Assert.AreEqual("20", detail["Order_Details_Amount"].Value);

            detail["Include_in_Discount"].Value = true.ToString();

            AssetsService.InsertAsset(detail);

            order = AssetsService.GetAssetById(order.ID, Helper.GetTypeByName("ADynEntitytestoder"));

            Assert.AreEqual("18", order["OrderAmount"].Value);
        }

        //[TestMethod]
        // unit tests shouldn't be run on a database!
        // TODO: rewrite with xUnit and mock DB
        public void DeleteDependency()
        {
            var order = Helper.CreateAsset("ADynEntitytestoder", "testorder");
            order["DiscountPercent"].Value = "10";
            order = AssetsService.InsertAsset(order);

            var detail = Helper.CreateAsset("ADynEntitytestorderdetails", "order_detail_1");
            detail["Name"].Value = "order_detail_1";
            detail["OrderIdlink"].ValueAsId = order.ID;
            detail["Product_Amount"].Value = "2";
            detail["Product_Price"].Value = "10";
            detail = AssetsService.InsertAsset(detail);

            order = AssetsService.GetAssetById(order.ID, Helper.GetTypeByName("ADynEntitytestoder"));
            Assert.AreEqual("20", order["OrderAmount"].Value);

            detail = AssetsService.GetAssetById(detail.ID, Helper.GetTypeByName("ADynEntitytestorderdetails"));
            
            AssetsService.DeleteAsset(detail);

            order = AssetsService.GetAssetById(order.ID, Helper.GetTypeByName("ADynEntitytestoder"));
            Assert.AreEqual("0", order["OrderAmount"].Value);
        }
    }
}