using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManagement;
using ProductManagement.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ProductManagement.Models;

namespace ProductManagement.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void TestDetails()
        {
            //arrange
            ProductController controller = new ProductController();

            //act
            var result = controller.Details(28) as ViewResult;

            //assert
            Assert.IsNotNull(result);
        }
    }
}
