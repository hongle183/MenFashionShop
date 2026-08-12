using ShopOnline.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDsizeController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();
        // GET: Admin/CRUDsize
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetSize()
        {
            try
            {
                // không lấy ICollection<>, List<>, hoặc navigation property → vòng lặp vô tận JSON.
                var listSize = db.Sizes.OrderByDescending(i => i.dateCreate)
                                       .Select(i => new { i.sizeId, i.code, i.dateCreate })
                                       .ToList();

                return Json(new { success = true, listSize, msg = "Get list Size successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Admin/CRUDsize/Create
        [HttpGet]
        public PartialViewResult Create()
        {
            return PartialView();
        }
        // POST: Admin/CRUDsize/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(string code)
        {
            try
            {
                var check = db.Sizes.SingleOrDefault(model => model.code == code);
                if (check != null)
                {
                    return Json(new { success = false, msg = "This size already exists!" });
                }

                var size = new Size
                {
                    code = code,
                    dateCreate = DateTime.Now
                };

                db.Sizes.Add(size);
                db.SaveChanges();

                return Json(new { success = true, msg = "New size added successfully!!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        // GET: Admin/CRUDsize/Edit/5
        [HttpGet]
        public PartialViewResult Edit(Guid sizeId)
        {
            var detail = db.Sizes.FirstOrDefault(i => i.sizeId == sizeId);

            return PartialView(detail);
        }
        // POST: Admin/CRUDsize/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Size size)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, message = "Data is invalid", errors });
            }

            try
            {
                var existing = db.Sizes.FirstOrDefault(i => i.sizeId == size.sizeId);
                if (existing == null)
                {
                    return Json(new { success = false, msg = "Size not found!" });
                }

                var check = db.Sizes.SingleOrDefault(i => i.code == size.code && i.sizeId != size.sizeId);
                if (check != null)
                {
                    return Json(new { success = false, msg = "This size already exists!" });
                }

                existing.code = size.code.Trim();
                existing.dateCreate = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, msg = "Size updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        // POST: Admin/CRUDsize/Delete/5
        [HttpPost]
        public JsonResult Delete(Guid sizeId)
        {
            try
            {
                var hasVariants = db.Variants.Any(v => v.sizeId == sizeId);

                if (hasVariants)
                {
                    return Json(new { success = false, msg = "Cannot delete this size because it still contains variants!" });
                }

                var size = db.Sizes.FirstOrDefault(i => i.sizeId == sizeId);
                if (size == null)
                    return Json(new { success = false, msg = "Size not found!" });

                db.Sizes.Remove(size);
                db.SaveChanges();
                return Json(new { success = true, msg = "Size deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
    }
}