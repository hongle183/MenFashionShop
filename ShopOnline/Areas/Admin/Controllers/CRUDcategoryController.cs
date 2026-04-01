using ShopOnline.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDcategoryController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();
        // GET: Admin/CRUDcategory
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetCategory()
        {
            try
            {
                // không lấy ICollection<>, List<>, hoặc navigation property → vòng lặp vô tận JSON.
                var listCategory = db.ProductCategories.OrderByDescending(i => i.dateCreate)
                                                      .Select(i => new
                                                      {
                                                          categoryId = i.categoryId,
                                                          categoryName = i.categoryName,
                                                          dateCreate = i.dateCreate
                                                      }).ToList();

                return Json(new { success = true, listCategory, msg = "Get list Category successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        // GET: Admin/CRUDcategory/Create
        [HttpGet]
        public PartialViewResult Create()
        {
            return PartialView();
        }
        // POST: Admin/CRUDcategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(string categoryName)
        {
            try
            {
                var check = db.ProductCategories.SingleOrDefault(model => model.categoryName == categoryName);
                if (check != null)
                {
                    return Json(new { success = false, msg = "This category already exists!" });
                }

                var category = new ProductCategory{
                    categoryName = categoryName,
                    status = true,
                    dateCreate = DateTime.Now
                };

                db.ProductCategories.Add(category);
                db.SaveChanges();

                return Json(new { success = true, msg = "New product category added successfully!!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        // GET: Admin/CRUDcategory/Edit/5
        [HttpGet]
        public PartialViewResult Edit(Guid categoryId)
        {
            var detail = db.ProductCategories.FirstOrDefault(i => i.categoryId == categoryId);

            return PartialView(detail);
        }
        // POST: Admin/CRUDcategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(ProductCategory category)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, message = "Data is invalid", errors });
            }

            try
            {
                var existing = db.ProductCategories.FirstOrDefault(i => i.categoryId == category.categoryId);
                if (existing == null)
                {
                    return Json(new { success = false, msg = "Category not found!" });
                }

                existing.categoryName = category.categoryName.Trim();
                existing.dateCreate = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, msg = "Category updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        // POST: Admin/CRUDcategory/Delete/5
        [HttpPost]
        public JsonResult Delete(Guid categoryId)
        {
            try
            {
                var hasProducts = db.Products.Any(p => p.categoryId == categoryId);

                if (hasProducts)
                {
                    return Json(new { success = false, msg = "Cannot delete this category because it still contains products!" });
                }

                var category = db.ProductCategories.FirstOrDefault(i => i.categoryId == categoryId);
                if (category == null)
                    return Json(new { success = false, msg = "Category not found!" });

                db.ProductCategories.Remove(category);
                db.SaveChanges();
                return Json(new { success = true, msg = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
    }
}