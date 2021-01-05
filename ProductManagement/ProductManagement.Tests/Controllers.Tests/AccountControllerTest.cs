using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ProductManagement.Controllers;
using System.Web.Mvc;

namespace ProductManagement.Tests.Controllers.Tests
{
    [TestClass]
    class AccountControllerTest
    {
        [TestMethod]
        public void TestRegister()
        {
            //arrange
            AccountController controller = new AccountController();

            //act
            ViewResult result = controller.Register() as ViewResult;

            //assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void TestLogin()
        {
            //arrange
            AccountController controller = new AccountController();

            //act
            ViewResult result = controller.Login() as ViewResult;

            //assert
            result.Should().NotBeNull();
        }
    }
}
