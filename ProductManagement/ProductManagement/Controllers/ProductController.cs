using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProductManagement.Models;

namespace ProductManagement.Controllers
{
    public class ProductController : Controller
    {
        // Tracing the logs by log4net variable
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ProductController));
        // Entity variable for database connectivity
        private readonly db_PMEntities db = new db_PMEntities();
        // Declaring list for product categories
        private readonly List<string> Categories = new List<string> { "Fruit", "Vegetable", "Drink" };
        // Declaring list for product quantities
        private readonly List<int> Quantities = new List<int> { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

        // GET: Product/List
        public ActionResult List(string sortType, string CurrentFilter, string Search, int? page)
        {
            // Taking value of sorting from the view
            ViewBag.CurrentSort = sortType;
            // Sorting products by name
            ViewBag.SortingName = String.IsNullOrEmpty(sortType) ? "name_desc" : "";
            // Sorting products by price
            ViewBag.SortingPrice = String.IsNullOrEmpty(sortType) ? "price" : "price_desc";
            // Message shows sweet alert after deleting product
            ViewBag.msg = TempData["Message"] as string;
            // If search string is null then stay on first page
            if (Search != null)
            {
                page = 1;
            }
            else
            {
                Search = CurrentFilter;
            }
            // Taking value of search provided by the user
            ViewBag.CurrentFilter = Search;
            var products = from s in db.tbl_Product
                           select s;
            // Finding product by using search string
            if (!String.IsNullOrEmpty(Search))
            {
                products = products.Where(s => s.Name.Contains(Search)
                || s.Price.ToString().Contains(Search.ToUpper()));
            }
            // Switching sort
            switch (sortType)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;

                case "price":
                    products = products.OrderBy(s => s.Price);
                    break;

                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;

                default:
                    products = products.OrderBy(s => s.Name);
                    break;
            }
            // Declaring page number and size for the pagination
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        // POST : Product/List
        [HttpPost]
        public ActionResult List(FormCollection formCollection)
        {
            // Message shows sweet alert after deleting product
            ViewBag.msg = TempData["Message"] as string;
            // String of ids for multiple delete
            string[] ids = formCollection["ids"].Split(new char[] { ',' });
            // Deleting every items by using loop
            foreach (string id in ids)
            {
                var product = this.db.tbl_Product.Find(int.Parse(id));
                Session["OldSmallImage"] = product.Small_ImagePath;
                Session["OldLargeImage"] = product.Large_ImagePath;
                this.db.tbl_Product.Remove(product);
                this.db.SaveChanges();
            }
            try
            {
                // Based on ids deleting images from the server folder
                if (System.IO.File.Exists(Server.MapPath(Session["OldSmallImage"].ToString())))
                {
                    System.IO.File.Delete(Server.MapPath(Session["OldSmallImage"].ToString()));
                }
                if (System.IO.File.Exists(Server.MapPath(Session["OldLargeImage"].ToString())))
                {
                    System.IO.File.Delete(Server.MapPath(Session["OldLargeImage"].ToString()));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return RedirectToAction("List");
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Finding product by id in the database
            tbl_Product tbl_Product = db.tbl_Product.Find(id);
            if (tbl_Product == null)
            {
                return HttpNotFound();
            }
            return View("Details");
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            try
            {
                // Storing values of the categories and quantities for the view
                ViewBag.Categories = Categories;
                ViewBag.Quantities = Quantities;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Category,Price,Quantity,Short_Description,Long_Description,Small_ImagePath,Large_ImagePath")] tbl_Product tbl_Product, HttpPostedFileBase smallImageFile, HttpPostedFileBase largeImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // List of allowed image extensions
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".Jpg" };
                    // Fetching file from the form and storing the path of the file in the database
                    // and image into the Image folder of the project
                    string smallImageName = Path.GetFileName(smallImageFile.FileName);
                    string _file1 = DateTime.Now.ToString("ddMMyyyyHHmmss") + smallImageName;
                    string smallImageExtension = Path.GetExtension(smallImageFile.FileName);
                    tbl_Product.Small_ImagePath = "~/Images/" + _file1;
                    string smallImagePath = Path.Combine(Server.MapPath("~/Images/"), _file1);
                    if (!allowedExtensions.Contains(smallImageExtension) || smallImageFile.ContentLength >= 1000000)
                    {
                        ModelState.AddModelError("", "Invalid extension or size of the file");
                    }
                    else
                    {
                        if (largeImageFile != null)
                        {
                            // Fetching file from the form and storing the path of the file in the database
                            // and image into the Image folder of the project
                            string largeImageName = Path.GetFileName(largeImageFile.FileName);
                            string _file2 = DateTime.Now.ToString("ddMMyyyyHHmmss") + largeImageName;
                            string largeImageExtension = Path.GetExtension(largeImageFile.FileName);
                            tbl_Product.Large_ImagePath = "~/Images/" + _file2;
                            string largeImagePath = Path.Combine(Server.MapPath("~/Images/"), _file2);
                            if (!allowedExtensions.Contains(largeImageExtension) || largeImageFile.ContentLength >= 1000000)
                            {
                                ModelState.AddModelError("", "Invalid extension or size of the file");
                            }
                            else
                            {
                                // Storing image paths and other variables into the database
                                db.tbl_Product.Add(tbl_Product);
                                if (db.SaveChanges() > 0)
                                {
                                    // Storing images into the Image folder
                                    smallImageFile.SaveAs(smallImagePath);
                                    largeImageFile.SaveAs(largeImagePath);
                                    ViewBag.msg = "Record Added";
                                    return RedirectToAction("List");
                                }
                            }
                        }
                        else
                        {
                            // If only small image is submitted then storing small image path and image to the
                            // specified location
                            db.tbl_Product.Add(tbl_Product);
                            if (db.SaveChanges() > 0)
                            {
                                smallImageFile.SaveAs(smallImagePath);
                                ViewBag.msg = "Record Added";
                                return RedirectToAction("List");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Large image is not added");
                    ModelState.Clear();
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                ModelState.AddModelError("", "Something went wrong");
            }
            return View(tbl_Product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Finding product by id in the database
            tbl_Product tbl_Product = db.tbl_Product.Find(id);
            if (tbl_Product == null)
            {
                return HttpNotFound();
            }
            try
            {
                // Storing uploaded images into the session to display on the view
                Session["OldSmallImage"] = tbl_Product.Small_ImagePath;
                Session["OldLargeImage"] = tbl_Product.Large_ImagePath;
                // Storing values of the categories and quantities for the view
                ViewBag.Categories = Categories;
                ViewBag.Quantities = Quantities;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return View(tbl_Product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Category,Price,Quantity,Short_Description,Long_Description,Small_ImagePath,Large_ImagePath")] tbl_Product tbl_Product, HttpPostedFile smallImageFile, HttpPostedFile largeImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // List of allowed image extensions
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".Jpg" };
                    if (smallImageFile != null && largeImageFile != null)
                    {
                        // Fetching file from the form and storing the path of the file in the database
                        // and image into the Image folder of the project
                        string smallImageName = Path.GetFileName(smallImageFile.FileName);
                        string _file1 = DateTime.Now.ToString("ddMMyyyyHHmmss") + smallImageName;
                        string smallImageExtension = Path.GetExtension(smallImageFile.FileName);

                        string largeImageName = Path.GetFileName(largeImageFile.FileName);
                        string _file2 = DateTime.Now.ToString("ddMMyyyyHHmmss") + largeImageName;
                        string largeImageExtension = Path.GetExtension(largeImageFile.FileName);

                        if (!allowedExtensions.Contains(smallImageExtension) && (!allowedExtensions.Contains(largeImageExtension)))
                        {
                            ModelState.AddModelError("", "Image type is not valid");
                        }

                        if (smallImageFile.ContentLength >= 1000000 && largeImageFile.ContentLength >= 10000000)
                        {
                            ModelState.AddModelError("", "Image size is not valid");
                        }

                        else
                        {
                            // Saving the new uploaded small images
                            tbl_Product.Small_ImagePath = "~/Images/" + _file1;
                            string smallImagePath = Path.Combine(Server.MapPath("~/Images/"), _file1);
                            smallImageFile.SaveAs(smallImagePath);
                            // Saving the new uploaded large image
                            tbl_Product.Large_ImagePath = "~/Images/" + _file2;
                            string largeImagePath = Path.Combine(Server.MapPath("~/Images/"), _file2);
                            largeImageFile.SaveAs(largeImagePath);
                            // Deleting old small and large image from the folder
                            if (System.IO.File.Exists(Server.MapPath(Session["OldSmallImage"].ToString())))
                            {
                                System.IO.File.Delete(Server.MapPath(Session["OldSmallImage"].ToString()));
                            }
                            if (System.IO.File.Exists(Server.MapPath(Session["OldLargeImage"].ToString())))
                            {
                                System.IO.File.Delete(Server.MapPath(Session["OldLargeImage"].ToString()));
                            }
                        }
                    }
                    else if (smallImageFile != null && largeImageFile == null)
                    {
                        // If largeimagefile is null then storing small image path in the database and image into the folder
                        string smallImageName = Path.GetFileName(smallImageFile.FileName);
                        string _file1 = DateTime.Now.ToString("ddMMyyyyHHmmss") + smallImageName;
                        string smallImageExtension = Path.GetExtension(smallImageFile.FileName);
                        tbl_Product.Large_ImagePath = Session["OldLargeImage"].ToString();

                        if (!allowedExtensions.Contains(smallImageExtension))
                        {
                            ModelState.AddModelError("", "Image type is not valid");
                        }

                        if (smallImageFile.ContentLength >= 1000000)
                        {
                            ModelState.AddModelError("", "Image size is not valid");
                        }

                        else
                        {
                            // Saving image
                            tbl_Product.Small_ImagePath = "~/Images/" + _file1;
                            string smallImagePath = Path.Combine(Server.MapPath("~/Images/"), _file1);
                            smallImageFile.SaveAs(smallImagePath);
                            // Deleting old image
                            if (System.IO.File.Exists(Server.MapPath(Session["OldSmallImage"].ToString())))
                            {
                                System.IO.File.Delete(Server.MapPath(Session["OldSmallImage"].ToString()));
                            }
                        }
                    }
                    else if (smallImageFile == null && largeImageFile != null)
                    {
                        // If smallimagefile is null then storing small image path in the database and image into the folder
                        string largeImageName = Path.GetFileName(largeImageFile.FileName);
                        string _file2 = DateTime.Now.ToString("ddMMyyyyHHmmss") + largeImageName;
                        string largeImageExtension = Path.GetExtension(largeImageFile.FileName);
                        tbl_Product.Small_ImagePath = Session["OldSmallImage"].ToString();

                        if (!allowedExtensions.Contains(largeImageExtension))
                        {
                            ModelState.AddModelError("", "Image type is not valid");
                        }

                        if (largeImageFile.ContentLength >= 1000000)
                        {
                            ModelState.AddModelError("", "Image size is not valid");
                        }

                        else
                        {
                            // Saving image
                            tbl_Product.Large_ImagePath = "~/Images/" + _file2;
                            string largeImagePath = Path.Combine(Server.MapPath("~/Images/"), _file2);
                            largeImageFile.SaveAs(largeImagePath);
                            // Deleting old image
                            if (System.IO.File.Exists(Server.MapPath(Session["OldLargeImage"].ToString())))
                            {
                                System.IO.File.Delete(Server.MapPath(Session["OldLargeImage"].ToString()));
                            }
                        }
                    }
                    else
                    {
                        // If both image is not uploaded then storing old images again
                        tbl_Product.Small_ImagePath = Session["OldSmallImage"].ToString();
                        tbl_Product.Large_ImagePath = Session["OldLargeImage"].ToString();
                    }
                    // Saving into the database
                    db.Entry(tbl_Product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                ModelState.AddModelError("", "Something went wrong");
            }
            // Storing values of the categories and quantities for the view
            ViewBag.Categories = Categories;
            ViewBag.Quantities = Quantities;
            return View(tbl_Product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Finding product by id in the database
            tbl_Product tbl_Product = db.tbl_Product.Find(id);
            if (tbl_Product == null)
            {
                return HttpNotFound();
            }
            try
            {
                // Storing old images into the session
                Session["OldSmallImage"] = tbl_Product.Small_ImagePath;
                Session["OldLargeImage"] = tbl_Product.Large_ImagePath;
                db.tbl_Product.Remove(tbl_Product);
                if (db.SaveChanges() > 0)
                {
                    // Deleting the images from the server folder
                    if (System.IO.File.Exists(Server.MapPath(Session["OldSmallImage"].ToString())))
                    {
                        System.IO.File.Delete(Server.MapPath(Session["OldSmallImage"].ToString()));
                    }
                    if (System.IO.File.Exists(Server.MapPath(Session["OldLargeImage"].ToString())))
                    {
                        System.IO.File.Delete(Server.MapPath(Session["OldLargeImage"].ToString()));
                    }
                    // Sweetalert message after successful delete
                    TempData["Message"] = "Your product " + tbl_Product.Name + " has been deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            return RedirectToAction("List", "Product");
        }
    }
}