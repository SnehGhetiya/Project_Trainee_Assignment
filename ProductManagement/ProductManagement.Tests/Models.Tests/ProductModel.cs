using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ProductManagement.Models;

namespace ProductManagement.Tests.Models.Tests
{
    [TestClass]
    public class ProductModel
    {
        [TestMethod]
        public void HaveADescriptionProperty()
        {
            //arrange
            const string testDescription = "This is a test";

            //act
            tbl_Product product = new tbl_Product { Short_Description = testDescription };

            //assert
            product.Short_Description.Should().Be(testDescription);
        }

        [TestMethod]
        public void HaveAQuantityProperty()
        {
            //arraneg
            const int testQuantity = 40;

            //act
            tbl_Product product = new tbl_Product { Quantity = testQuantity };

            //assert
            product.Quantity.Should().Be(testQuantity);
        }

        [TestMethod]
        public void HaveACategoryProperty()
        {
            //arrange
            const string testCategory = "Vegetable";

            //act
            tbl_Product product = new tbl_Product { Category = testCategory };

            //assert
            product.Category.Should().Be(testCategory);
        }
    }
}
