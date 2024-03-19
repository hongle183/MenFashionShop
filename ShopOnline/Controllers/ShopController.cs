using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;
using PagedList;
using CSharpVitamins;

namespace shopOnline.Controllers
{
    public class ShopController : Controller
    {
        menfsEntities1 db = new menfsEntities1();
        // GET: Shop
        public ActionResult Shop()
        {
            return View();
        }
        public PartialViewResult ListProduct(string brand, Guid? categories,int? page, string searching) // Show product
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;
            if(searching != null)
            {
                ViewBag.categories = categories;
                var list = db.Products.Where(model => model.productName.Contains(searching) || searching == null && model.status == true).OrderByDescending(model => model.dateCreate).ToPagedList(pageNumber, pageSize);
                return PartialView(list);
            }
            else
            {
                if (brand != null && categories == null)
                {
                    ViewBag.categories = categories;
                    var list = db.Products.OrderByDescending(model => model.dateCreate).Where(model => model.brand == brand && model.status == true).ToPagedList(pageNumber, pageSize);
                    return PartialView(list);
                }
                else if (brand == null && categories != null)
                {
                    ViewBag.categories = categories;
                    var list = db.Products.OrderByDescending(model => model.dateCreate).Where(model => model.categoryId == categories && model.status == true).ToPagedList(pageNumber, pageSize);
                    return PartialView(list);
                }
                else
                {
                    var list = db.Products.OrderByDescending(model => model.dateCreate).Where(model => model.status == true).ToPagedList(pageNumber, pageSize);
                    return PartialView(list);
                }
            }
        }
        public PartialViewResult Categories() // List categories
        {
            var list = db.ProductCategories.ToList();
            return PartialView(list);
        }
        public PartialViewResult Brand() // List brand
        {
            List<string> brand = new List<string>();
            foreach (Product i in this.db.Products)
            {
                if (i.brand != null)
                {
                    if (!brand.Contains(i.brand.Trim())) { // can't trim() null
                        brand.Add(i.brand.Trim());
                    }
                }                  
            }
            return PartialView(brand);
        }        
        public PartialViewResult RelationProduct(Guid? category)
        {
            var categories = db.Products.Where(model => model.categoryId == category).Take(4).ToList();
            return PartialView(categories);
        }

        public ActionResult ProductDetail(string meta)
        {
            if (meta == null)
            {
                return RedirectToAction("Error", "Home");
            }
            ShortGuid id =(ShortGuid)meta;
            var detail = db.Products.Where(model => model.productId == id.Guid).Single();
            ViewBag.NewPrice = detail.price - ((detail.price * detail.discount) / 100);
            if (detail == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(detail);
        }
    }
}