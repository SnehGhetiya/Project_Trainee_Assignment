using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductManagement;
using ProductManagement.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FluentAssertions;

namespace ProductManagement.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void TestApi()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Api() as ViewResult;

            // Assert
            result.Should().NotBeNull();
        }
    }
}
