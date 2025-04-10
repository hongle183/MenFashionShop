using System;
using System.Linq;
using System.Web.Mvc;
using ShopOnline.Models;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Admin")]
    public class CRUDcategoryController : Controller
    {
        menfsEntities db = new menfsEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult DsCategory()
        {
            try
            {
                var dsCategory = db.ProductCategories.OrderByDescending(i => i.dateCreate)
                                                      .Select(i => new
                                                      {
                                                          categoryId = i.categoryId,
                                                          categoryName = i.categoryName,
                                                          dateCreate = i.dateCreate
                                                      }).ToList();

                return Json(new { code = 200, dsCategory = dsCategory, msg = "Successfully get list Category!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Get list Category false: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult AddCategory(string categoryName)
        {
            try
            {
                var category = new ProductCategory();
                category.categoryName = categoryName;
                category.status = true;
                category.dateCreate = DateTime.Now;

                db.ProductCategories.Add(category);
                db.SaveChanges();

                return Json(new { code = 200, msg = "Successfully added a new product category!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Detail(Guid categoryId)
        {
            try
            {
                var detail = db.ProductCategories.Where(i => i.categoryId == categoryId)
                                                   .Select(i => new
                                                   {
                                                       categoryId = i.categoryId,
                                                       categoryName = i.categoryName
                                                   }).SingleOrDefault();

                return Json(new { code = 200, detail = detail, msg = "Successfully get detail!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Failed get detail!" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Update(Guid categoryId, string categoryName)
        {
            try
            {
                var category = db.ProductCategories.Where(i => i.categoryId == categoryId)
                                                   .SingleOrDefault();
                category.categoryName = (string)categoryName;
                category.dateCreate = DateTime.Now;
                db.SaveChanges();
                return Json(new { code = 200, msg = "Successfully update!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Faily update" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(Guid categoryId)
        {
            try
            {
                ProductCategory category = db.ProductCategories.Where(i => i.categoryId == categoryId)
                                                                .SingleOrDefault();
                db.ProductCategories.Remove(category);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Successfully delete!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Failed delete! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}