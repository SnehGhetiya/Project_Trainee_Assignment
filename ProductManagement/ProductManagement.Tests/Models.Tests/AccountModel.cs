using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Models;
using ProductManagement.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Web.Mvc;

namespace ProductManagement.Tests.Models.Tests
{
    [TestClass]
    public class AccountModel
    {
        [TestMethod]
        public void HaveAEmailProperty()
        {
            //arrange
            const string testEmail = "abc@gmail.com";

            //act
            tbl_Login login = new tbl_Login { Email = testEmail };

            //assert
            login.Email.Should().Be(testEmail);
        }

        [TestMethod]
        public void HaveAPasswordProperty()
        {
            //arrange
            const string testPassword = "Abc@1234";

            //act
            tbl_Login login = new tbl_Login { Password = testPassword };

            //assert
            login.Password.Should().Be(testPassword);
        }
    }
}
