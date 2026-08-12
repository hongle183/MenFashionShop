using ShopOnline.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ShopOnline.Areas.Admin.Controllers
{
    [CustomAuthorize("Manager")]
    public class CRUDcolorController : Controller
    {
        private readonly menfsEntities db = new menfsEntities();
        // GET: Admin/CRUDcolor
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetColor()
        {
            try
            {
                // không lấy ICollection<>, List<>, hoặc navigation property → vòng lặp vô tận JSON.
                var listColor = db.Colors.OrderByDescending(i => i.dateCreate)
                                         .Select(i => new { i.colorId, i.code, i.hex, i.dateCreate })
                                         .ToList();

                return Json(new { success = true, listColor, msg = "Get list Color successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Admin/CRUDcolor/Create
        [HttpGet]
        public PartialViewResult Create()
        {
            return PartialView();
        }
        // POST: Admin/CRUDcolor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(string code, string hex)
        {
            try
            {
                var check = db.Colors.SingleOrDefault(model => model.hex == hex);
                if (check != null)
                {
                    return Json(new { success = false, msg = "This color already exists!" });
                }

                var color = new Color
                {
                    code = code,
                    hex = hex,
                    dateCreate = DateTime.Now
                };

                db.Colors.Add(color);
                db.SaveChanges();

                return Json(new { success = true, msg = "New color added successfully!!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        // GET: Admin/CRUDcolor/Edit/5
        [HttpGet]
        public PartialViewResult Edit(Guid colorId)
        {
            var detail = db.Colors.FirstOrDefault(i => i.colorId == colorId);

            return PartialView(detail);
        }
        // POST: Admin/CRUDcolor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Color color)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();

                return Json(new { success = false, message = "Data is invalid", errors });
            }

            try
            {
                var existing = db.Colors.FirstOrDefault(i => i.colorId == color.colorId);
                if (existing == null)
                {
                    return Json(new { success = false, msg = "Color not found!" });
                }

                var check = db.Colors.SingleOrDefault(i => i.hex == color.hex && i.colorId != color.colorId);
                if (check != null)
                {
                    return Json(new { success = false, msg = "This color already exists!" });
                }

                existing.code = color.code.Trim();
                existing.hex = color.hex.Trim();
                existing.dateCreate = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true, msg = "Color updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
        // POST: Admin/CRUDcolor/Delete/5
        [HttpPost]
        public JsonResult Delete(Guid colorId)
        {
            try
            {
                var hasVariants = db.Variants.Any(v => v.colorId == colorId);

                if (hasVariants)
                {
                    return Json(new { success = false, msg = "Cannot delete this color because it still contains variants!" });
                }

                var color = db.Colors.FirstOrDefault(i => i.colorId == colorId);
                if (color == null)
                    return Json(new { success = false, msg = "Color not found!" });

                db.Colors.Remove(color);
                db.SaveChanges();
                return Json(new { success = true, msg = "Color deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "Error: " + ex.Message });
            }
        }
    }
}