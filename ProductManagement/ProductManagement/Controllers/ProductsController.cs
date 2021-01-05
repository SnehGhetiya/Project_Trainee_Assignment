using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    [AllowAnonymous]
    public class ProductsController : ApiController
    {
        // GET : Products/Get
        public HttpResponseMessage Get()
        {
            // Listing all products from the database
            List<tbl_Product> productList = new List<tbl_Product>();
            using(db_PMEntities db = new db_PMEntities())
            {
                // Binding all the products list with the HttpResponse
                productList = db.tbl_Product.OrderBy(a => a.Name).ToList();
                HttpResponseMessage response;
                // Showing products list by using web api response
                response = Request.CreateResponse(HttpStatusCode.OK, productList);
                return response;
            }
        }
        
        
    }
}
