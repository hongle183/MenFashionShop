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
        public ActionResult Shop(string meta)
        {
            ViewBag.meta = meta;
            return View();
        }

        public PartialViewResult ListProduct(int? page, string meta, string searching, string brand, decimal? minPrice, decimal? maxPrice) // Show product
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;

            List<Product> list = db.Products.Where(model => model.ProductCategory.meta.Equals(meta) || meta == null && model.status == true).OrderByDescending(model => model.dateCreate).ToList();

            if(searching != null)
            {
                var result = list.Where(model => model.productName.ToLower().Contains(searching.ToLower()) || searching == null);
                ViewBag.searching = searching;
                ViewBag.count = result.Count();
                return PartialView(result.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                if (brand != null)
                {
                    var result = list.Where(model => model.brand == brand);
                    ViewBag.count = result.Count();
                    return PartialView(result.ToPagedList(pageNumber, pageSize));
                } else if (minPrice.HasValue && maxPrice.HasValue)
                {
                    var result = list.Where(model => model.price >= minPrice && model.price <= maxPrice);
                    ViewBag.minPrice = minPrice;
                    ViewBag.maxPrice = maxPrice;
                    ViewBag.count = result.Count();
                    return PartialView(result.ToPagedList(pageNumber, pageSize));
                }
                else
                {                    
                    ViewBag.count = list.Count();
                    return PartialView(list.ToPagedList(pageNumber, pageSize));
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
            foreach (Product i in db.Products)
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
        public PartialViewResult RelationProduct(Guid? product, Guid? category)
        {
            var categories = db.Products.Where(model => model.productId != product && model.categoryId == category).Take(4).ToList();
            return PartialView(categories);
        }

        public ActionResult ProductDetail(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }

            ShortGuid gid = (ShortGuid)id;
            var detail = db.Products.Where(model => model.productId == gid.Guid).Single();
            ViewBag.NewPrice = detail.price - ((detail.price * detail.discount) / 100);
            if (detail == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(detail);
        }
    }
}