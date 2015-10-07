using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppFramework.Core.Classes.SearchEngine;
using System.Security.Principal;
using System.Diagnostics;

namespace AppFramework.Core.Tests
{
    [TestClass]
    public class CategorySearchTests
    {
        private Stopwatch _stopWatch;
        [TestInitialize]
        public void TestInitialize()
        {
            IPrincipal principal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { });
            System.Threading.Thread.CurrentPrincipal = principal;

            _stopWatch = new Stopwatch();            
        }

        [TestMethod]
        public void CategorySearchTest()
        {
            throw new NotImplementedException();
            //int totalItems;
            //long[] taxItemsIds = new long[] { 435 };
            //string keywords = string.Empty; //"maes annie";

            //_stopWatch.Start();
            //SearchOutput searchResult = SearchEngine.FindByCategoriesAndKeywords(taxItemsIds,
            //    keywords, Classes.SearchEngine.Enumerations.TimePeriodForSearch.CurrentTime, 0, int.MaxValue, out totalItems, true);
            //_stopWatch.Stop();
            //TimeSpan ts = _stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //    ts.Hours, ts.Minutes, ts.Seconds,
            //    ts.Milliseconds / 10);
            //Debug.Print(elapsedTime);

            //Assert.IsTrue(totalItems > 0); //32
        }
    }
}
